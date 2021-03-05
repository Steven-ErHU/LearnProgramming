# 强制转换

在 C++ 中，如果基类包含虚函数，那么基类指针可以调用虚函数在派生类对象中的实现。包含虚函数的类有时也称为“多态类”。

由于派生类完整包含了其所有基类的定义，将指针强制转换为其中任一个基类都是安全的。将基类型的指针强制转换为派生类类型指针时，如果指针指向的对象确实也为派生类类型，那么强制转换也是安全的。这种情况下，被强制转换的对象称为“完整对象”。该基类指针称为指向完整对象的“子对象”。例如，考虑下图描述的类层次结构。

![Class hierarchy](assets\images\vc38zz1.gif "Class hierarchy") <br/>

`C` 类型对象可以表示如下。

![Class C with sub&#45;objects B and A](assets\images\vc38zz2.gif "Class C with sub&#45;objects B and A") <br/>

`C` 类型的实例是“完整对象”，包含了类型`B` 和 `A` 的子对象。
通过运行时信息，可以查看一个指针是否实际指向一个完整对象，并且安全的强制转换为类型层次中其他的对象。 

`dynamic_cast` 操作符可以用于这些类型强制转换，同时进行运行时检测来保证操作安全。

`static_cast` 操作符用于非多态类型的强制转换。

## 强制转换运算符

C++ 包含了几种强制转换运算符，这些运算符用于消除老式 C 语言转换中的一些歧义和危险性：
- `dynamic_cast` 用于多态类型的转换。
- `static_cast` 用于非多态类型的转换。
- `const_cast` 用于删除 `const`, `volatile`, 和 `__unaligned` 属性。
- `reinterpret_cast` 用于位的简单重释。
- `safe_cast` 在 C++/CLI 中用于生成可验证的 MSIL.

不是万不得已，不要使用 `const_cast` 和 `reinterpret_cast`，因为这些操作符具有老式 C 语言转换中的一些危险性。然而，为了完全替代老式风格的强制转换，他们任然是必要的。

### `dynamic_cast` 运算符

将操作数 expression 转换为类型 type-id 的对象。

#### 语法
```
dynamic_cast <type-id> (expression)
```

#### 备注

`type-id` 必须是一个指针或者引用，指向已定义的类类型或者 void。如果`type-id` 是指针，则 `expression` 必须也为指针类型，如果 `type-id` 是引用，`expression` 必须为左值类型。

**`dynamic_cast`** 的行为在托管代码中有两项重要更改：
- 用 **`dynamic_cast`** 强制转换为枚举类型的指针在运行时会失效，返回 0，而不是预期的指针。
- 当 `type-id` 是指向值类型的内部指针，**`dynamic_cast`** 在运行时会失效，不会抛出异常，返回 0 值指针。

如果 `type-id` 是指针，明确的指向 `expression` 某个基类型，那么将返回指向 `type-id` 类型子对象的指针。例如：

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

这种类型的转换称为“向上转换”，因为指针沿着类型层次向上移动，从派生类到基类。“向上转换” 是隐式转换。

如果 `type-id` 是 void*，那么在运行时将检测 `expression` 的实际类型。其结果返回 `expression` 指向的完整对象。例如：

```cpp
class A {virtual void f();};
class B {virtual void f();};

void f() {
   A* pa = new A;
   B* pb = new B;
   void* pv = dynamic_cast<void*>(pa);
   // pv now points to an object of type A

   pv = dynamic_cast<void*>(pb);
   // pv now points to an object of type B
}
```

如果 `type-id` 不为 void*，那么运行时将检测是否能够将 `expression` 指向的对象转换为 `type-id` 指向的类型。

如果 `expression` 类型是 `type-id` 类型的基类型，运行时将检测 `expression` 是否指向 `type-id` 类型的完整对象，如果是，那么返回指向 `type-id` 类型完整对象的指针。例如：

```cpp
class B {virtual void f();};
class D : public B {virtual void f();};

void f() {
   B* pb = new D;   // unclear but ok
   B* pb2 = new B;

   D* pd = dynamic_cast<D*>(pb);   // ok: pb actually points to a D
   D* pd2 = dynamic_cast<D*>(pb2);   // pb2 points to a B not a D
}
```
这种类型的转换称为“向下转换”，因为指针沿着类型层次向下移动，从基类到派生类。

在多重继承的情况中，可能存在歧义。考虑如下的类继承结构：

![Class hierarchy that shows multiple inheritance](assets\images\vc39011.gif "Class hierarchy that shows multiple inheritance") <br/>

指向 `D` 类型对象的指针能够安全地转换为 `B` 或 `C` 类型。然而，如果 `D` 转换为 `A` 类型对象的指针，返回结果是 `A` 类型的哪个实例？这会导致转换存在歧义，产生错误。为了避免这种情况，可以进行两次明确的转换。例如：

```cpp
class A {virtual void f();};
class B : public A {virtual void f();};
class C : public A {virtual void f();};
class D : public B, public C {virtual void f();};

void f() {
   D* pd = new D;
   A* pa = dynamic_cast<A*>(pd);   // C4540, ambiguous cast fails at runtime
   B* pb = dynamic_cast<B*>(pd);   // first cast to B
   A* pa2 = dynamic_cast<A*>(pb);   // ok: unambiguous
}
```

进一步，虚基类会引入更多的歧义。考虑如下的结构：

![Class hierarchy that shows virtual base classes](assets\images\vc39012.gif "Class hierarchy that shows virtual base classes") <br/>

假设 `A` 是虚基类，给定一个指向 `E` 实例的 `A` 类型指针，通过 **`dynamic_cast`** 转换为 `B` 类型指针会有歧义，产生错误。你必须先将其转换为 `E` 类型对象，然后明确地将其转换为正确的 `B` 对象。

考虑如下类型结构：

![Class hierarchy that shows duplicate base classes](assets\images\vc39013.gif "Class hierarchy that shows duplicate base classes") <br/>

给定一个指向 `E` 对象的 `D` 类型指针，要转换为最左侧的 `A` 类型，需要进行三次操作。
Given an object of type `E` and a pointer to the `D` subobject, to navigate from the `D` subobject to the left-most `A` subobject, three conversions can be made. 
通过 **`dynamic_cast`** 将 `D` 指针转换为 `E` 类型，到 `B` 类型，最后到最左侧的 `A` 类型。例如：

```cpp
class A {virtual void f();};
class B : public A {virtual void f();};
class C : public A { };
class D {virtual void f();};
class E : public B, public C, public D {virtual void f();};

void f(D* pd) {
   E* pe = dynamic_cast<E*>(pd);
   B* pb = pe;   // upcast, implicit conversion
   A* pa = pb;   // upcast, implicit conversion
}
```

**`dynamic_cast`** 操作符还可以用于“交叉强制转换”。例如，将指针从 `B` 类型转换到 `D` 类型，只要实例的完整对象是 `E` 类型。因此，从 `D` 类型到 `A` 类型其实只需要两步，先从 `D` 到 `B`，再到 `A`。看下面例子：

```cpp
class A {virtual void f();};
class B : public A {virtual void f();};
class C : public A { };
class D {virtual void f();};
class E : public B, public C, public D {virtual void f();};

void f(D* pd) {
   B* pb = dynamic_cast<B*>(pd);   // cross cast
   A* pa = pb;   // upcast, implicit conversion
}
```

通过 **`dynamic_cast`**，空指针可以转换为目标类型的空指针。


当你调用 `dynamic_cast <type-id> (expression)`, 如果 `expression` 不能够安全的转换为  `type-id` 类型, 那么运行时检测就会失败。 例如:

```cpp
class A {virtual void f();};
class B {virtual void f();};

void f() {
   A* pa = new A;
   B* pb = dynamic_cast<B*>(pa);   // fails at runtime, not safe;
   // B not derived from A
}
```

如果强制换行失败，对于指针，将返回空指针。对于引用，将抛出 `bad_cast` 异常，如果 `expression` 指向或者引用一个无效对象，将抛出 `__non_rtti_object` 异常。

#### Example

下面的例子创建了基类型（struct A）指针，指向派生类对象（struct C）。同时基类包含了虚函数，因此产生了运行时的多态。示例中还调用了非虚函数。

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

### static_cast 操作符

将 `expression` 转换为 `type-id` 类型，仅依照 `expression` 中显示的类型。

#### 语法

```
static_cast <type-id> (expression)
```

#### 备注

在标准 C++ 中，`static_cast` 转换没有运行时检测来保证安全性。在 C++/CX 中，则包含了编译和运行时检测。

`static_cast` 操作符能够用于将基类指针转换为派生类指针，但这样的转换不总是安全的。

通常，`static_cast` 用于数值类型转换，例如枚举和整型，整型和浮点类型的转换，并且假定你清楚转换涉及到的数据类型。

`static_cast` 转换不像 `dynamic_cast` 那样安全。因为 `static_cast` 没有运行时检测。通过 `dynamic_cast` 进行有歧义的转换会导致失败，然而 `static_cast` 会像没有错误发生一样返回结果。尽管 `dynamic_cast` 更加安全，但 `dynamic_cast` 仅适用于指针和引用，并且运行时检测是需要消耗性能的。

下面的示例中，行 *D* pd2 = static_cast<D*>(pb);* 是非安全的，因为 D 可以有 B 不包含的成员变量和函数。然而，行 *B* pb2 = static_cast<B*>(pd);* 是安全的，因为 D 包含 所有 B 的成员。

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

对比 `dynamic_cast`，`static_cast` 没有运行时检测。例如，通过 pd2 调用 D 类型成员（但是非 B 类型成员）函数将违背访问原则。
`static_cast` 操作符仅依赖于 `expression` 中提供的类型，因此是不安全的。例如：

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
如果 pb 实际指向类型 D 或者 pd == 0，那么 pd1 和 pd2 将获得相同的值。

如果 pb 实际指向类型 B，那么 `dynamic_cast` 会返回 0。但是 `static_cast` 依赖于编程人员认定 pb 指向 D 类型对象，于是简单的返回 D 类型的指针。

结果就是，`static_cast` 转换会继续执行，但其返回结果是未定义的。这就需要编程人员去进一步验证转换结果是有效的。同样地行为也适用于内置类型。

`static_cast` 操作符 也能够用于任何隐式转换，包括标准的转换和用户自定义的转换，例如：

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
`static_cast` 操作符能够显式的将整型转换为枚举类型。如果整型值不在枚举值范围内，那么返回的枚举值是未定义的。

`static_cast` 操作符能够将原空指针转换为目标类型的空指针。

通过 `static_cast`， 任何 `expression` 能够显式地转换为 void 类型。

目标 void 类型可以包含 `const`，`volatile`，`__unaligned` 属性。

`static_cast` 操作符不会去除 `const`，`volatile`，`__unaligned` 属性。

### const_cast  操作符

移除类的 `const`，`volatile`，`__unaligned` 属性。

#### 语法

```
const_cast <type-id> (expression)
```

示例：
```cpp
#include <iostream>

using namespace std;
class CCTest {
public:
   void setNumber( int );
   void printNumber() const;
private:
   int number;
};

void CCTest::setNumber( int num ) { number = num; }

void CCTest::printNumber() const {
   cout << "\nBefore: " << number;
   const_cast<CCTest *>(this)->number--;
   cout << "\nAfter: " << number;
}

int main() {
   CCTest X;
   X.setNumber(8);
   X.printNumber();
}
```
在包含 `const_cast` 的那行代码中，this 指针的数据类型是 const CCTest *。
`const_cast` 操作符将其转换为类型 CCTest *，从而允许其成员被修改。

### reinterpret_cast 操作符
允许指针被转换为任何其他类型的指针。同样允许整型与指针类型的转换。

#### 语法
```
reinterpret_cast <type-id> (expression)
```

Misuse of the reinterpret_cast operator can easily be unsafe. Unless the desired conversion is inherently low-level, you should use one of the other cast operators.

The reinterpret_cast operator can be used for conversions such as char* to int*, or One_class* to Unrelated_class*, which are inherently unsafe.

The result of a reinterpret_cast cannot safely be used for anything other than being cast back to its original type. Other uses are, at best, nonportable.

The reinterpret_cast operator cannot cast away the const, volatile, or __unaligned attributes. See const_cast Operator for information on removing these attributes.

The reinterpret_cast operator converts a null pointer value to the null pointer value of the destination type.

One practical use of reinterpret_cast is in a hash function, which maps a value to an index in such a way that two distinct values rarely end up with the same index.

```cpp
#include <iostream>
using namespace std;

// Returns a hash code based on an address
unsigned short Hash( void *p ) {
   unsigned int val = reinterpret_cast<unsigned int>( p );
   return ( unsigned short )( val ^ (val >> 16));
}

using namespace std;
int main() {
   int a[20];
   for ( int i = 0; i < 20; i++ )
      cout << Hash( a + i ) << endl;
}


```

Output:
```output
64641
64645
64889
64893
64881
64885
64873
64877
64865
64869
64857
64861
64849
64853
64841
64845
64833
64837
64825
64829
```

The reinterpret_cast allows the pointer to be treated as an integral type. The result is then bit-shifted and XORed with itself to produce a unique index (unique to a high degree of probability). The index is then truncated by a standard C-style cast to the return type of the function.