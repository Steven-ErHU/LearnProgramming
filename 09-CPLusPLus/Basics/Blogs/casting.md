# 关于 C++ 中的强制转换 - 基础篇

## 引言

假设有基类 `A`，包含了虚函数 `func1`，以及有派生类 `B`，继承于类 `A`，派生类 `B` 中实现了函数 `func1`。此时可以用 `A` 类型的指针指向 `B` 类型的对象，并用 `A` 类型的指针调用 `B` 类型对象中的函数 `func1`。这时，就形成了**多态**。包含虚函数的类 `A`，我们也称为**多态类**。

由于派生类 `B` 完整包含了 基类 `A` 的所有定义，将 `B` 类型的指针转换为 `A` 类型的指针**总是安全的**。

而将 `A` 类型的指针强制转换为 `B` 类型的指针时，如果 `A` 类型指针指向的对象确实为 `B` 类型的对象，那么转换**也是安全的**。此时，该 `B` 类型对象被称为完整对象(complete object)。

## 强制转换有哪些类型？

C++ 包含了以下几种强制转换运算符，这些运算符用于消除老式 C 语言转换中的存在的歧义和隐患：
> - `dynamic_cast`
> - `static_cast`
> - `const_cast`
> - `reinterpret_cast`
> - `safe_cast`

本文会着重介绍如何使用 `dynamic_cast` 和 `static_cast`。

提醒：
> 除非必须，不要使用 `const_cast` 和 `reinterpret_cast`，因为它们存在一些老式 C 语言转换中的隐患。


### `dynamic_cast` 运算符

语法：
```
 dynamic_cast <type-id> (expression)
```

`type-id` 必须是一个指针或者引用，指向/引用已定义的类类型或者 void。如果`type-id` 是指针，则 `expression` 必须也为指针类型，如果 `type-id` 是引用，`expression` 必须为左值类型。

如果 `type-id` 是 void*，那么在运行时将检测 `expression` 的实际类型。其结果返回 `expression` 指向的完整对象。

如非需要，现代 C++ 中应该避免使用 `void` 指针，因为容易出错。

下面看些示例，了解 `dynamic_cast` 的使用方式。

示例1：
```cpp
class Root { };
class Base : public Root { };
class Derived : public Base { };

void f(Derived* pd) {
   Base* pb = dynamic_cast<Base*>(pd);   // ok: Base is a direct base class
                                   // pb points to Base subobject of pd
   Root* pr = dynamic_cast<Root*>(pd);   // ok: Root is an indirect base class
                                   // pr points to Root subobject of pd
}
```
示例1 中提到了**子对象**(subobject)的概念，注意与**子类型**进行区分：
> - `Root` 类型包含子类型 `Base`，`Base` 类型包含子类型 `Derived`。  
> - `Derived` 对象包含了 `Base` 类型的子对象，`Base` 类型的子对象又包含了 `Root` 类型的子对象。
 
再联系下前面说的：派生类完整包含了基类的所有定义。

示例2：
```cpp
class B {virtual void f();};
class D : public B {virtual void f();};

void f() {
   B* pb = new D;   // unclear but ok
   B* pb2 = new B;

   D* pd = dynamic_cast<D*>(pb);   // ok: pb actually points to a D
   D* pd2 = dynamic_cast<D*>(pb2);   // pb2 was nullptr.
}
```
示例2 中 通过 `dynamic_cast` 转换为 `pd2` 时，不会报错，返回 `nullptr`。但如果同样地情况转换的对象是引用类型，那么运行时会抛出 `std::bad_cast` 异常。如果 `pb2` 指向/引用的对象无效，同样也会抛出异常。

示例3：
```cpp
#include <stdio.h>
#include <iostream>

struct A {
    virtual void test() {
        printf_s("in A\n");
   }
};

struct B : A {
    virtual void test() {
        printf_s("in B\n");
    }

    void test2() {
        printf_s("test2 in B\n");
    }
};

struct C : B {
    virtual void test() {
        printf_s("in C\n");
    }

    void test2() {
        printf_s("test2 in C\n");
    }
};

void Globaltest(A& a) {
    try {
        C &c = dynamic_cast<C&>(a);
        printf_s("in GlobalTest\n");
    }
    catch(std::bad_cast) {
        printf_s("Can't cast to C\n");
    }
}

int main() {
    A *pa = new C;
    A *pa2 = new B;

    pa->test();

    B * pb = dynamic_cast<B *>(pa);
    if (pb)
        pb->test2();

    C * pc = dynamic_cast<C *>(pa2);
    if (pc)
        pc->test2();

    C ConStack;
    Globaltest(ConStack);

   // will fail because B knows nothing about C
    B BonStack;
    Globaltest(BonStack);
}
```

Output:
```Output
in C
test2 in B
in GlobalTest
Can't cast to C
```
### `static_cast` 运算符

语法：
```
static_cast <type-id> (expression)
```

`static_cast` 通常用于数值类型转换，例如枚举和整型，整型和浮点类型的转换。

在标准 C++ 中，`static_cast` 转换没有运行时检测来保证安全性。在 C++/CX 中，则包含了编译和运行时检测。

`static_cast` 运算符能够用于将基类指针转换为派生类指针，但这样的转换不总是安全的。

下面还是通过示例进行讲解。

示例1：
```cpp
class B {};
class D : public B {};

void f(B* pb, D* pd) {
   D* pd2 = static_cast<D*>(pb);   // Not safe, D can have fields
                                   // and methods that are not in B.

   B* pb2 = static_cast<B*>(pd);   // Safe conversion, D always
                                   // contains all of B.
}
```

示例1 中 `pd2` 不为空，当用指针 `pd2` 调用 `B` 类型对象不存在的方法或者成员时可能会发生运行时错误（比如调用虚函数）或者返回非预期的值。

示例2：
```cpp
typedef unsigned char BYTE;

void f() {
   char ch;
   int i = 65;
   float f = 2.5;
   double dbl;

   ch = static_cast<char>(i);   // int to char
   dbl = static_cast<double>(f);   // float to double
   i = static_cast<BYTE>(ch);
}
```

示例2 中 `static_cast` 运算符显示地将内置类型进行转换。

关于 `static_cast` 运算符，还有以下几种使用情况：
> - `static_cast` 能够显式的将整型转换为枚举类型。如果整型值不在枚举值范围内，那么返回的枚举值是未定义的。 
> - `static_cast` 能将任何 `expression` 显式地转换为 `void` 类型。
> - `static_cast` 操作符不会去除 `const`，`volatile`，`__unaligned` 属性。

## 区分几种强制转换的使用场景

`dynamic_cast` 主要用于多态类型的强制转换，而 `static_cast` 主要用于非多态类型的强制转换。

`static_cast` 转换不像 `dynamic_cast` 那样安全。因为 `static_cast` 没有运行时检测。通过 `dynamic_cast` 进行转换时，一旦存在歧义，就会导致失败，然而 `static_cast` 会像没有错误发生一样返回结果。尽管 `dynamic_cast` 更加安全，但 `dynamic_cast` 仅适用于指针和引用，并且运行时检测是需要消耗性能的。

示例：
```cpp
class B {
public:
   virtual void Test(){}
};
class D : public B {};

void f(B* pb) {
   D* pd1 = dynamic_cast<D*>(pb);
   D* pd2 = static_cast<D*>(pb);
}
```

如果 `pb` 实际指向类型 `D` 或者 `pd == 0`，那么 `pd1` 和 `pd2` 将获得相同的值。

如果 `pb` 实际指向类型 `B`，那么 `dynamic_cast` 会返回 `0`。但是 `static_cast` 依赖于 `expression` 认定 `pb` 指向 `D` 类型对象，于是简单的返回 `D` 类型的指针。

结果就是，`static_cast` 转换会继续执行，但其返回结果是未定义的。这就需要调用者去进一步验证转换结果是有效的。

## 引用
https://docs.microsoft.com/en-us/cpp/cpp/casting?view=msvc-160