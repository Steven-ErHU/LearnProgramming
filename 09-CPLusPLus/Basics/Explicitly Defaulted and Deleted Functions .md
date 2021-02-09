在 C++ 11 中，**"= default"** 和 **"= delete"** 函数使我们能够显示指定**成员函数**是否自动生成。

**"= delete"** 使我们能够避免**所有函数** (特殊成员函数，普通成员函数和非成员函数) 参数中出现错误的类型提升 (导致非预期的函数调用)。

> C++ 特殊成员函数：<br/>
> 即使用户不自定义，编译器也会自动生成的成员函数，包括
> - 构造函数
> - 拷贝构造函数
> - 拷贝赋值运算符
> - 移动构造函数
> - 移动赋值运算符
> - 析构函数

## **"= default"** 函数

**`= default`** 可以用于任何特殊成员函数，比如显示指定特殊成员函数使用默认的函数实现，定义非公有的特殊函数，或者在某些情况下恢复特殊成员函数的自动生成。

**示例：**

```cpp
struct widget
{
  widget() = default;

  inline widget& operator=(const widget&);
};

inline widget& widget::operator=(const widget&) = default;
```
你可以在类的外部指定特殊成员函数为 **`= default`**，只要它是内联的。

由于特殊成员函数的性能优势，当需要默认行为时建议使用自动生成的特殊成员函数而不是使用空函数体。为此可以显示指定特殊成员函数为 **`= default`** 或者不声明它。

## **"= delete"** 函数

定义函数(任何函数)为 **`= delete`** 防止其被定义和调用。

### 防止编译器生成你不需要的**特殊**成员函数。

和 **`= default`**不同，**"= delete"** 函数必须在函数声明的时候指定其为 **`= default`**， 而不能在之后重新声明其为 **`= default`**。

**示例：**
```cpp
struct widget
{
  // deleted operator new prevents widget from being dynamically allocated.
  void* operator new(std::size_t) = delete;
};
```

防止成员和非成员函数参数发生错误的类型提升导致非预期的函数调用。

**示例：**
```cpp
// deleted overload prevents call through type promotion of float to double from succeeding.
void call_with_true_double_only(float) = delete;
void call_with_true_double_only(double param) { return; }
```


上面这个例子中，通过传递 **`float`** 类型参数来调用 `call_with_true_double_only` 会产生一个编译错误, 但是使用 **`int`** 类型参数则不会，**`int`** 类型的参数会被转换提升为 **`double`** 类型， 尽管这不是想要的结果。

为保证任何非 **`double`** 参数的调用能够产生编译错误，可以声明一个模板函数并指定其为 **`= delete`**。

**示例：**
```cpp
template < typename T >
void call_with_true_double_only(T) = delete; //prevent call through type promotion of any T to double from succeeding.

void call_with_true_double_only(double param) { return; } // also define for const double, double&, etc. as needed.
```

## **"= default"** 和 **"= delete"** 函数的益处

在 C++ 中，如果用户不自定义，编译器可以自动生成特殊成员函数。这对简单类型是方便的，但是复杂类型经常会自定义特殊成员函数，使用 **`= delete`** 可以防止这些特殊成员函数被自动创建。

在实践中:
- 如果任何构造函数已经显示声明，那么默认构造函数就不会自动生成。
- 如果虚析构函数已经显示声明，那么默认西沟函数就不会自动生成。
- 如果移动构造函数或者移动赋值操作符已经显示声明，那么：
  - 拷贝构造函数就不会自动生成
  - 拷贝赋值运算符就不会自动生成
- 如果拷贝构造函数，拷贝赋值操作符，移动构造函数或者移动赋值操作符已经显示声明，那么：
  - 移动构造函数就不会自动生成
  - 移动赋值运算符就不会自动生成
  
**注意:**
> C++11 标准还规定了如下规则：
> - 如果拷贝构造函数或者析构函数被显示声明，那么拷贝赋值运算符就不会自动生成
> - 如果拷贝赋值运算符或者析构函数被显示声明，那么拷贝构造函数就不会自动生成
> 
> 这两种情况下， Visual Studio 仍然会自动生成必要的函数，并不会产生告警。

在 C++11 之前，非拷贝类型的通用实现:

```cpp
struct noncopyable
{
  noncopyable() {};

private:
  noncopyable(const noncopyable&);
  noncopyable& operator=(const noncopyable&);
};
```

上述代码存在一些问题：
- 拷贝构造函数声明为私有导致了默认构造函数无法自动生成，必须显示声明默认构造函数,即使他什么也不做。
- 即使显示声明的默认构造函数声明也不做，编译器也将其认为是复杂的，其不如自动生产的默认构造函数来的高效，并且 `noncopyable` 也不会认为是 POD 类型。
- 即使拷贝构造函数和拷贝赋值运算符被声明为私有的，成员函数和友元仍然能够调用他们，如果他们声明了但未定义，调用会产生链接错误。
- 尽管这是通用的实现方式，但是其意图并不明显，除非你了解所有特殊成员函数自动生成的规则。

在 C++11 中， 非拷贝类型可以以更直接的方式实现从而解决了上述问题：

```cpp
struct noncopyable
{
  noncopyable() = default;
  noncopyable(const noncopyable&) = delete;
  noncopyable& operator=(const noncopyable&) = delete;
};
```

