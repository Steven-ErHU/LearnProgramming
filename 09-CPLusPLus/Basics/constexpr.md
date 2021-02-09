## **`constexpr`** 是什么？

关键字 **`constexpr`** (*constant expression*) 是在 C++11 中引入的，并且在 C++14 中进行了优化。

**`constexpr`** 和 **`const`** 一样可以用来修饰变量：试图修改 **`constexpr`** 变量时，编译器将会报警。

不同于 **`const`**， **`constexpr`** 还可以修饰函数和类的构造函数。 **`constexpr`** 表示值或者返回值是常量，并且如果可能，在编译时计算它们。

一个 **`constexpr`** 整型值能够用在任何 **`const`** 整型值可以用的地方，例如模板参数和数组的申明。

当值在编译时计算而不是运行时计算时，它能够使程序运行得更快，并使用更少的内存。

为了限制编译时常量计算的复杂性，以及其对编译时间潜在的影响， C++14 标准需要 **`constexpr`** 类型必须为字面值类型。

### 语法

> **`constexpr`** *literal-type* *identifier* **=** *constant-expression* **;**\
> **`constexpr`** *literal-type* *identifier* **{** *constant-expression* **}** **;**\
> **`constexpr`** *literal-type* *identifier* **(** *params* **)** **;**\
> **`constexpr`** *ctor* **(** *params* **)** **;**

### 参数

*params*\
一个或者多个参数, 每个参数必须是字面值类型并且本身是 **常量表达式** 。

### 返回值
一个 **`constexpr`** 变量或者函数必须返回字面值类型。

### **`constexpr`** 变量

**`constexpr`** 和 **`const`** 的主要区别是 **`const`** 变量的初始化可以被延时到运行时，而 **`constexpr`** 变量必须在编译时初始化。所有的 **`constexpr`** 变量都是 **`const`**。

- 当变量由字面值类型初始化时，能够声明为 **`constexpr`**。如果初始化时由构造函数执行的，那么该构造函数也必须声明为 **`constexpr`**。

- 引用类型可以声明为 **`constexpr`** 只要满足：引用对象是由 常量表达式初始化的，并且初始化时任何隐式转换也是常量表达式。

- 所有 **`constexpr`** 变量或者函数的声明必须拥有 **`constexpr`** 说明符。

**举例：**
```cpp
constexpr float x = 42.0;
constexpr float y{108};
constexpr float z = exp(5, 3);
constexpr int i; // Error! Not initialized
int j = 0;
constexpr int k = j + 1; //Error! j not a constant expression
```

### **`constexpr`** 函数

**`constexpr`** 函数的返回值是在编译时计算的。调用代码需要返回值在编译时初始化一个 **`constexpr`** 变量， 或者提供一个非类型模板参数。 当参数是 **`constexpr`** 值时，**`constexpr`** 函数产生一个编译时的常量。当被调用时传入非 **`constexpr`** 参数，或者其返回值在非编译时请求，**`constexpr`** 函数和普通函数一样将产生提个运行时的值。(这种行为能够让你避免编写两个相同功能的函数，一个为 **`constexpr`** 版本，一个为非 **`constexpr`** 版本)

**`constexpr`** 函数或者构造函数默认是 **`inline`** 的.

以下规则适用于 **`constexpr`** 函数:

- 必须接受并返回字面值类型。
- 可以是递归的.
- 不能是虚构的。当类有虚基类时，构造函数不能定义为 **`constexpr`**。
- 函数体可以定义为 `= default` 或者 `= delete`。
- 函数体不能包含 **`goto`** 语句或者 **`try`** 块。
- 显式特化/具体化(???)的非 **`constexpr`** 模板能够声明为 **`constexpr`**。
- 显式特化/具体化(???)的非 **`constexpr`** 模板不必声明为 **`constexpr`**。
  
以下 **`constexpr`** 函数规则适用于 Visual Studio 2017 及以后的版本：

- 可以包含 **`if`** 和 **`switch`** 语句，以及所有循环语句包括 **`for`**，**`while`** 和 **do-while**
- 可以包含初始化的局部变量，并且必须是字面值类型，不能是 **`static`** 或者 thread-local。该局部变量不必是 **`const`** 的。
- 非 **`static`** 的 **`constexpr`** 成员函数不必隐式为 **`const`**???

```cpp
constexpr float exp(float x, int n)
{
    return n == 0 ? 1 :
        n % 2 == 0 ? exp(x * x, n / 2) :
        exp(x * x, (n - 1) / 2) * x;
}
```

> 提示：
> 
> 在 Visual Studio 调试器中, 当调试一个非优化的调试版本，你能够通过在函数内部设置断点来区分 **`constexpr`** 函数是否在编译时计算。如果断点能够触发，则为运行时计算，否则，为编译时计算。

### 示例

以下例子演示了 **`constexpr`** 变量, 函数, 以及用户自定义类型。 在 `main()` 最后, **`constexpr`** 成员函数 `GetValue()` 是在运行时调用的，因为其返回值没有被要求在编译时确定。

```cpp
#include <iostream>

using namespace std;

// Pass by value
constexpr float exp(float x, int n)
{
    return n == 0 ? 1 :
        n % 2 == 0 ? exp(x * x, n / 2) :
        exp(x * x, (n - 1) / 2) * x;
}

// Pass by reference
constexpr float exp2(const float& x, const int& n)
{
    return n == 0 ? 1 :
        n % 2 == 0 ? exp2(x * x, n / 2) :
        exp2(x * x, (n - 1) / 2) * x;
}

// Compile-time computation of array length
template<typename T, int N>
constexpr int length(const T(&)[N])
{
    return N;
}

// Recursive constexpr function
constexpr int fac(int n)
{
    return n == 1 ? 1 : n * fac(n - 1);
}

// User-defined type
class Foo
{
public:
    constexpr explicit Foo(int i) : _i(i) {}
    constexpr int GetValue() const
    {
        return _i;
    }
private:
    int _i;
};

int main()
{
    // foo is const:
    constexpr Foo foo(5);
    // foo = Foo(6); //Error!

    // Compile time:
    constexpr float x = exp(5, 3);
    constexpr float y { exp(2, 5) };
    constexpr int val = foo.GetValue();
    constexpr int f5 = fac(5);
    const int nums[] { 1, 2, 3, 4 };
    const int nums2[length(nums) * 2] { 1, 2, 3, 4, 5, 6, 7, 8 };

    // Run time:
    cout << "The value of foo is " << foo.GetValue() << endl;
}
```

## constexpr 和 const 的异同

**相同处：**<br/>
**`constexpr`** 和 **`const`** 一样可以用来修饰变量：试图修改 **`constexpr`** 变量时，编译器将会报警。一个 **`constexpr`** 整型值能够用在任何 **`const`** 整型值可以用的地方，例如模板参数和数组的申明。

**不同处：<br/>
**`constexpr`** 还可以修饰函数和类的构造函数。**`constexpr`** 表示值或者返回值是常量，并且如果可能，在编译时计算它们。
当一个值在编译时计算而不是运行时计算时，它能够使程序运行得更快，并使用更少的内存。

所有的 **`constexpr`** 对象都是 **`const`** 的，但不是所有的 **`const`** 对象都是 **`constexpr`** 的。

### 示例

```cpp
int sz;                             // non-constexpr variable
…

constexpr auto arraySize1 = sz;     // error! sz's value not
                                    // known at compilation

std::array<int, sz> data1;          // error! same problem

constexpr auto arraySize2 = 10;     // fine, 10 is a 
                                    //compile-time constant

std::array<int, arraySize2> data2;  // fine, arraySize2
                                    // is constexpr
```

```cpp
int sz;                             // as before
…

const auto arraySize = sz;          // fine, arraySize is
                                    // const copy of sz

std::array<int, arraySize> data;    // error! arraySize's value
                                    // not known at compilation
```

## 应该使用 constexpr 的场景
只要允许，尽可能使用 **`constexpr`**，当值在编译时计算而不是运行时计算时，它能够使程序运行得更快，并使用更少的内存。

## 不应该使用 constexpr 的场景

**`constexpr`** 是对象或者函数接口的一部分，所以如果你使用了 **`constexpr`** 但反悔了，移除 **`constexpr`** 可能会导致大量的调用代码编译失败。(比如添加 I/O 操作用于调试或者性能调优可能导致这样的问题，因为 I/O 语句通常不是在 **`constexpr`** 函数中执行的。)

## 引用
- Microsoft Docs (https://docs.microsoft.com/en-us/cpp/cpp/constexpr-cpp?view=msvc-160)

- Effective Modern C++