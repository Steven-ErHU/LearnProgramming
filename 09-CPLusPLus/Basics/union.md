# `union`

> [!注意]
> 从 C++ 17 开始，`std::variant` 替代 union，`std::variant` 是类型安全的。

**`union`** 是用户自定义的类型，其所有成员共享同一块内存。这个定义意味着任何时候，一个 union 只能包含一个它某个成员的对象。这也意味着无论 union 有多少个成员，它占用的内存等于成员中最大的成员需要占用的内存。

当你拥有大量对象，但是仅有有限的内存时，union 就能派上用场。然而，union 需要额外的小心才能正确使用。你必须负责保证你访问的总是你设置的成员。如果任何成员类型包含非默认构造函数，那么你必须编写额外的代码来构建和销毁成员。在你使用 union 之前，请考虑你需要解决的问题是否可以使用类的继承来实现。

## 语法

> **`union`** *`tag`*<sub>opt</sub> **`{`** *`member-list`* **`};`**

### 参数

*`tag`*<br/>
union 的类型名称

*`member-list`*<br/>
union 可以包含的成员。

## union 的声明

使用 **`union`** 关键字来声明 union 类型，使用大括号来包含成员。

```cpp
// declaring_a_union.cpp
union RecordType    // Declare a simple union type
{
    char   ch;
    int    i;
    long   l;
    float  f;
    double d;
    int *int_ptr;
};

int main()
{
    RecordType t;
    t.i = 5; // t holds an int
    t.f = 7.25; // t now holds a float
}
```

## 使用 union

In the previous example, any code that accesses the union needs to know which member holds the data. The most common solution to this problem is called a *discriminated union*. It encloses the union in a struct, and includes an enum member that indicates the member type currently stored in the union. The following example shows the basic pattern:
前面例子中，任何访问 union 的代码都需要知道哪个成员包含了数据。常用的解决方案是将 union 放入一个结构体, 然后用一个枚举值来记录 union 中当前包含数据的成员类型是哪个，看下面示例：

```cpp
#include <queue>

using namespace std;

enum class WeatherDataType
{
    Temperature, Wind
};

struct TempData
{
    int StationId;
    time_t time;
    double current;
    double max;
    double min;
};

struct WindData
{
    int StationId;
    time_t time;
    int speed;
    short direction;
};

struct Input
{
    WeatherDataType type;
    union
    {
        TempData temp;
        WindData wind;
    };
};

// Functions that are specific to data types
void Process_Temp(TempData t) {}
void Process_Wind(WindData w) {}

void Initialize(std::queue<Input>& inputs)
{
    Input first;
    first.type = WeatherDataType::Temperature;
    first.temp = { 101, 1418855664, 91.8, 108.5, 67.2 };
    inputs.push(first);

    Input second;
    second.type = WeatherDataType::Wind;
    second.wind = { 204, 1418859354, 14, 27 };
    inputs.push(second);
}

int main(int argc, char* argv[])
{
    // Container for all the data records
    queue<Input> inputs;
    Initialize(inputs);
    while (!inputs.empty())
    {
        Input const i = inputs.front();
        switch (i.type)
        {
        case WeatherDataType::Temperature:
            Process_Temp(i.temp);
            break;
        case WeatherDataType::Wind:
            Process_Wind(i.wind);
            break;
        default:
            break;
        }
        inputs.pop();

    }
    return 0;
}
```

上面的例子中，结构体 `Input` 中 union 没有命名，所以称为匿名的 union。它的成可以直接通过结构体对象访问，就像访问结构体自己的成员一样。

上面例子有一个问题，那就是你能够使用类继承来实现运行时多态，达到同样地目的，并且桁架易于理解和维护，只是运行会比使用 union 慢。同时，使用 union， 你可以存储不相关的类型。union 可以让你动态的改变存储对象的类型，但不需要更改 union 对象本身的类型。例如，你可以创建一个 union 类型数组，包含不同类型的不同对象。

上面的例子，结构体 `Input` 很容易被误用。这取决于用户正确地通过枚举值来访问成员数据。你可以将 union 声明为 **`private`** 然后提供特定的访问函数，参考后面的示例。

## 不受限的 union (C++11)

C++ 03 及以前，union 能够包含类类型的非静态数据成员，只要类型没有用户自定义的构造函数，西沟含糊或者赋值操作符。

C++ 11 开始，这些限制取消了。如果 union 包含这样一个成员，编译器会自动将任何非自定义的特殊成员函数标记为 **`deleted`**。如果 union 在类或者结构体中，且是匿名的，那么任何类或者结构体的非自定义的特殊成员函数都会被标记为 **`deleted`**。下面例子中某个 union 成员的成员需要这种处理：

```cpp
// for MyVariant
#include <crtdbg.h>
#include <new>
#include <utility>

// for sample objects and output
#include <string>
#include <vector>
#include <iostream>

using namespace std;

struct A
{
    A() = default;
    A(int i, const string& str) : num(i), name(str) {}

    int num;
    string name;
    //...
};

struct B
{
    B() = default;
    B(int i, const string& str) : num(i), name(str) {}

    int num;
    string name;
    vector<int> vec;
    // ...
};

enum class Kind { None, A, B, Integer };

#pragma warning (push)
#pragma warning(disable:4624)
class MyVariant
{
public:
    MyVariant()
        : kind_(Kind::None)
    {
    }

    MyVariant(Kind kind)
        : kind_(kind)
    {
        switch (kind_)
        {
        case Kind::None:
            break;
        case Kind::A:
            new (&a_) A();
            break;
        case Kind::B:
            new (&b_) B();
            break;
        case Kind::Integer:
            i_ = 0;
            break;
        default:
            _ASSERT(false);
            break;
        }
    }

    ~MyVariant()
    {
        switch (kind_)
        {
        case Kind::None:
            break;
        case Kind::A:
            a_.~A();
            break;
        case Kind::B:
            b_.~B();
            break;
        case Kind::Integer:
            break;
        default:
            _ASSERT(false);
            break;
        }
        kind_ = Kind::None;
    }

    MyVariant(const MyVariant& other)
        : kind_(other.kind_)
    {
        switch (kind_)
        {
        case Kind::None:
            break;
        case Kind::A:
            new (&a_) A(other.a_);
            break;
        case Kind::B:
            new (&b_) B(other.b_);
            break;
        case Kind::Integer:
            i_ = other.i_;
            break;
        default:
            _ASSERT(false);
            break;
        }
    }

    MyVariant(MyVariant&& other)
        : kind_(other.kind_)
    {
        switch (kind_)
        {
        case Kind::None:
            break;
        case Kind::A:
            new (&a_) A(move(other.a_));
            break;
        case Kind::B:
            new (&b_) B(move(other.b_));
            break;
        case Kind::Integer:
            i_ = other.i_;
            break;
        default:
            _ASSERT(false);
            break;
        }
        other.kind_ = Kind::None;
    }

    MyVariant& operator=(const MyVariant& other)
    {
        if (&other != this)
        {
            switch (other.kind_)
            {
            case Kind::None:
                this->~MyVariant();
                break;
            case Kind::A:
                *this = other.a_;
                break;
            case Kind::B:
                *this = other.b_;
                break;
            case Kind::Integer:
                *this = other.i_;
                break;
            default:
                _ASSERT(false);
                break;
            }
        }
        return *this;
    }

    MyVariant& operator=(MyVariant&& other)
    {
        _ASSERT(this != &other);
        switch (other.kind_)
        {
        case Kind::None:
            this->~MyVariant();
            break;
        case Kind::A:
            *this = move(other.a_);
            break;
        case Kind::B:
            *this = move(other.b_);
            break;
        case Kind::Integer:
            *this = other.i_;
            break;
        default:
            _ASSERT(false);
            break;
        }
        other.kind_ = Kind::None;
        return *this;
    }

    MyVariant(const A& a)
        : kind_(Kind::A), a_(a)
    {
    }

    MyVariant(A&& a)
        : kind_(Kind::A), a_(move(a))
    {
    }

    MyVariant& operator=(const A& a)
    {
        if (kind_ != Kind::A)
        {
            this->~MyVariant();
            new (this) MyVariant(a);
        }
        else
        {
            a_ = a;
        }
        return *this;
    }

    MyVariant& operator=(A&& a)
    {
        if (kind_ != Kind::A)
        {
            this->~MyVariant();
            new (this) MyVariant(move(a));
        }
        else
        {
            a_ = move(a);
        }
        return *this;
    }

    MyVariant(const B& b)
        : kind_(Kind::B), b_(b)
    {
    }

    MyVariant(B&& b)
        : kind_(Kind::B), b_(move(b))
    {
    }

    MyVariant& operator=(const B& b)
    {
        if (kind_ != Kind::B)
        {
            this->~MyVariant();
            new (this) MyVariant(b);
        }
        else
        {
            b_ = b;
        }
        return *this;
    }

    MyVariant& operator=(B&& b)
    {
        if (kind_ != Kind::B)
        {
            this->~MyVariant();
            new (this) MyVariant(move(b));
        }
        else
        {
            b_ = move(b);
        }
        return *this;
    }

    MyVariant(int i)
        : kind_(Kind::Integer), i_(i)
    {
    }

    MyVariant& operator=(int i)
    {
        if (kind_ != Kind::Integer)
        {
            this->~MyVariant();
            new (this) MyVariant(i);
        }
        else
        {
            i_ = i;
        }
        return *this;
    }

    Kind GetKind() const
    {
        return kind_;
    }

    A& GetA()
    {
        _ASSERT(kind_ == Kind::A);
        return a_;
    }

    const A& GetA() const
    {
        _ASSERT(kind_ == Kind::A);
        return a_;
    }

    B& GetB()
    {
        _ASSERT(kind_ == Kind::B);
        return b_;
    }

    const B& GetB() const
    {
        _ASSERT(kind_ == Kind::B);
        return b_;
    }

    int& GetInteger()
    {
        _ASSERT(kind_ == Kind::Integer);
        return i_;
    }

    const int& GetInteger() const
    {
        _ASSERT(kind_ == Kind::Integer);
        return i_;
    }

private:
    Kind kind_;
    union
    {
        A a_;
        B b_;
        int i_;
    };
};
#pragma warning (pop)

int main()
{
    A a(1, "Hello from A");
    B b(2, "Hello from B");

    MyVariant mv_1 = a;

    cout << "mv_1 = a: " << mv_1.GetA().name << endl;
    mv_1 = b;
    cout << "mv_1 = b: " << mv_1.GetB().name << endl;
    mv_1 = A(3, "hello again from A");
    cout << R"aaa(mv_1 = A(3, "hello again from A"): )aaa" << mv_1.GetA().name << endl;
    mv_1 = 42;
    cout << "mv_1 = 42: " << mv_1.GetInteger() << endl;

    b.vec = { 10,20,30,40,50 };

    mv_1 = move(b);
    cout << "After move, mv_1 = b: vec.size = " << mv_1.GetB().vec.size() << endl;

    cout << endl << "Press a letter" << endl;
    char c;
    cin >> c;
}
```

union 不能存储引用类型。
union 也不支持继承。

## 初始化 union

可以同时声明和初始化 union。初始化值会赋值给 union 的第一个成员。

```cpp
#include <iostream>
using namespace std;

union NumericType
{
    short       iValue;
    long        lValue;
    double      dValue;
};

int main()
{
    union NumericType Values = { 10 };   // iValue = 10
    cout << Values.iValue << endl;
    Values.dValue = 3.1416;
    cout << Values.dValue << endl;
}
/* Output:
10
3.141600
*/
```

The `NumericType` union is arranged in memory (conceptually) as shown in the following figure.
`NumericType` 在内存（概念上的）中非分布如下图所示：

![Storage of data in a numeric type union](assets\images\vc38ul1.png "Storage of data in a NumericType union") <br/>

## 匿名 union

An anonymous union is one declared without a *`class-name`* or *`declarator-list`*.

匿名 union 声明时没有类名或者声明列表。

> **`union  {`**  *`member-list`*  **`}`**

匿名 union 中的变量可以像非成员变量一样被访问，所以在作用域内变量名称必须唯一。

匿名 union 有一些额外的限制:
- 如果直接在文件中或者命名空间内定义 union，那么必须将 union 声明为 **`static`**。
- 只能包含 **`public`** 成员。
- 不能包含成员函数。
