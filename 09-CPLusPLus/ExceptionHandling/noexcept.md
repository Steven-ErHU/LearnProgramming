# __noexcept__ 关键字

## __noexcept__ 是什么？

noexcept 是自 C++11 引入的新特性，指定函数是否可能会引发异常，以下是 noexcept 的标准语法：
> *noexcept-expression*:\
> &nbsp;&nbsp;&nbsp;&nbsp;**`noexcept`**\
> &nbsp;&nbsp;&nbsp;&nbsp;**noexcept(** *constant-expression* **)**

*constant-expression* 是一个 `bool` 类型的常量表达式，是一种异常规范（*exception specification*），属于C++的语言特性，表示是否会发生异常。
`noexcept` 等效于 `noexcept(true)`。

`noexcept(true)` 或者 `noexcept`  表示函数不会抛出或者传递异常，如果函数发生异常，将调用 `std::terminate` 立即终止程序。

`noexcept(false)` or 或者不使用 `noexcept` (析构函数或释放函数默认声明为 `noexcept`), 表示函数所有可能的异常都会被抛出.

<br/>

## 应该使用 __nonexcept__ 的情形

建议将所有不会抛出异常的函数声明为 `noexcept`。
当函数声明为 `noexcept` 后，编译器能够在一些不同的上下文环境中产生更加高效的代码。

<br/>

## 不应该使用 __nonexcept__ 的情形

函数可以标记为 `noexcept` 当且仅当内部调用的所有函数也都直接或者间接的标记为 `noexcept` 或者 `const`。
编译器没有义务检查所有层级代码是否会抛出异常到 `noexcept` 函数。
如果标记了 `noexcept` 的函数确实抛出了异常，那么`std::terminate`将会被立即调用，并且不能保证函数内部的对象能够被析构。

比起优化，正确性会更为重要。
当你在最开始声明一个函数为 `noexcept`, 而后又反悔想要去掉 `noexcept` 标记，那么你将会影响到调用端的代码。

<br/>

## 示例


下面的函数被标记为有条件的 `noexcept`：函数是否为 `noexcept` 取决于 `noexcept` 的子句表达式是否为 `noexcept`。
例如，有两个包含 Widget 对象的数组，交换两个数组的函数是否为 `noexcept` 取决于 交换两个数组中元素的函数是否为 `noexcept`，即，交换两个 Widget 对象是否为 `noexcept`。因此 Widget 对象 swap 的实现决定了 Widget 数组的交换函数是否为 `noexcept`。
同样地，包含 Widgets 的 std::pair 对象的交换函数是否应该为 `noexcept` 取决于交换两个 Widget 对象是否为 `noexcept`。 
上层的数据结构操作可以为 `noexcept` 仅当下层的数据结构操作为 `noexcept`。这就促使你，只要允许，就尽可能地提供 `noexcept` 的函数。

```cpp
template <class T, size_t N>
void swap(T (&a)[N], T (&b)[N]) noexcept(noexcept(swap(*a, *b))); 

template <class T1, class T2>
struct pair {
    …
    void swap(pair& p) noexcept(noexcept(swap(first, p.first)) && noexcept(swap(second, p.second)));
    …
};
```
<br/>

## 总结
- `noexcept` 是函数接口的一部分，这意味着调用者会依赖它。
- `noexcept` 函数可优化性要高于 `non-noexcept` 函数。
- `noexcept` 用在数据移动，交换，内存释放函数，析构函数中会更有价值。
- 大多数函数本质上是属于 `non-noexcept` 的。

<br/>

## 扩展

在 C++17 之前还有一种异常规范 (dynamic exception pecification) `throw(optional_type_list)`。
C++17 之后 `throw(optional_type_list)` 已被移除(除了 `throw()`)，`throw()` 等同于 `noexcept`。

应该避免使用 `throw(optional_type_list)` 或者 `throw()`。