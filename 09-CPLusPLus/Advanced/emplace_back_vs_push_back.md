# 你想知道的 `push_back` 和 `emplace_back`

## 引言

C++ 11 后，标准库容器 `std::vector` 包含了成员函数 `emplace` 和 `emplace_back`。`emplace` 在容器指定位置插入元素，`emplace_back` 在容器末尾添加元素。

`emplace` 和 `emplace_back` 原理类似，本文仅讨论 `push_back` 和 `emplace_back`。

## 定义

首先看下 Microsoft Docs 对 `push_back` 和 `emplace_back` 的定义：

- `push_back`：Adds an element to the end of the vector.
- `emplace_back`：Adds an element **constructed in place** to the end of the vector.

两者的定义我用了加粗字体作区分，那么现在问题就在于什么叫做 **constructed in place** ？

再来看下官方文档（www.cplusplus.com）怎么介绍 `emplace_back` 的：

> template <class... Args>
  void emplace_back (Args&&... args);
>>
>> Inserts a new element at the end of the vector, right after its current last element. This new element is **constructed in place using `args` as the arguments for its constructor**.
This **effectively** increases the container size by one, which causes an automatic reallocation of the allocated storage space if -and only if- the new vector size surpasses the current vector capacity.
The element is constructed in-place by calling `allocator_traits::construct` with `args` forwarded.
A similar member function exists, `push_back`, which either **copies or moves** an existing object into the container.

**简而言之，`push_back` 会构造一个临时对象，这个临时对象会被拷贝或者移入到容器中，然而 `emplace_back` 会直接根据传入的参数在容器的适当位置进行构造而避免拷贝或者移动。**

## 为什么我们有了 `emplace_back` 还需要 `push_back`？

这部分内容进一步对如何区分 `push_back` 和 `emplace_back` 做了解答。
Stack Overflow 有一项回答我认为已经解释的较为清楚，因此这里部分转译过来。

翻译带有个人理解，非直译，原文参考：https://stackoverflow.com/questions/10890653/why-would-i-ever-use-push-back-instead-of-emplace-back

以下为译文：

关于这个问题我在过去 4 年思考良多，我敢说大多数关于 `push_back` 和 `emplace_back` 的解释都不够完善。

去年，我在一次关于 C++ 的介绍中（链接参考原文）讨论了 `push_back` 和 `emplace_back` 的相关议题，这两者最主要的区别来自于：**是使用隐式构造函数还是显示构造函数**（`implicit` vs. `explicit` constructors）。

先看下面的示例：

```cpp
std::vector<T> v;

v.push_back(x);
v.emplace_back(x);
```

传统观点认为 `push_back` 会构造一个临时对象，这个临时对象会被移入到 `v` 中，然而 `emplace_back` 会直接根据传入的参数在适当位置进行构造而避免拷贝或者移动。从标准库代码的实现角度来说这是对的，但是对于提供了优化的编译器来讲，上面示例中最后两行表达式生成的代码其实没有区别。

真正的区别在于，`emplace_back` 更加强大，它可以调用任何类型的（只要存在）构造函数。而 `push_back` 会更加严谨，它只调用隐式构造函数。隐式构造函数被认为是安全的。如果能够通过对象 `T` 隐式构造对象 `U`，就认为 `U` 能够完整包含 `T` 的所有内容，这样将 `T` 传递给 `U` 通常是安全的。正确使用隐式构造的例子是用 `std::uint32_t` 对象构造 `std::uint64_t` 对象，错误使用隐式构造的例子是用 `double` 构造 `std::uint8_t`。

我们必须在编码时小心翼翼。我们不想使用强大/高级的功能，因为**它越是强大，就越有可能发生意想不到的错误**。如果想要调用显示构造函数，那么就调用 `emplace_back`。如果只希望调用隐式构造函数，那么请使用更加安全的 `push_back`。

再看个示例：
```cpp
std::vector<std::unique_ptr<T>> v;
T a;
v.emplace_back(std::addressof(a)); // compiles
v.push_back(std::addressof(a)); // fails to compile
```

`std::unique_ptr<T>` 包含了显示构造函数通过 `T*` 进行构造。因为 `emplace_back` 能够调用显示构造函数，所以传递一个裸指针并不会产生编译错误。然而，当 `v` 超出了作用域，`std::unique_ptr<T>` 的析构函数会尝试 `delete` 类型 `T*` 的指针，而类型 `T*` 的指针并不是通过 `new` 来分配的，因为它保存的是栈对象的地址，因此 `delete` 行为是未定义的。

这不是为了示例而特意写的代码，而是一个我遇到的实际问题。原本 `v` 是 `std::vector<T *>` 类型，迁移到 C++ 11 后，我修改为 `std::vector<std::unique_ptr<T>>`。并且，那时我错误地认为 `emplace_back` 能够做 `push_back` 所能做的所有事情，因此将 `push_back` 也改为了 `emplace_back`。

如果我保留使用更加安全的 `push_back`，那么我会立马发现这个 bug。不幸的是，我意外地隐藏了这个 bug 并直到几个月后才重新发现它。

## 引用
- https://docs.microsoft.com/en-us/cpp/standard-library/vector-class?view=msvc-160
- https://www.cplusplus.com/reference/vector/vector/emplace_back/
- https://stackoverflow.com/questions/10890653/why-would-i-ever-use-push-back-instead-of-emplace-back