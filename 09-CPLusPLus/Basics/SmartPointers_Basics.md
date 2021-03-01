
## 简介

在现代 C++ 编程中，标准库包含了智能指针(Smart pointers)。

智能指针用来确保程序不会出现内存和资源的泄漏，并且是"异常安全"(exception-safe)的。

## 智能指针的使用

智能指针定义在头文件 `memory` 里的命名空间 **`std`** 中。它对于*资源获取即初始化(RAII, Resource Acquisition Is Initialization)* 编程理念至关重要。该理念的目的是保证对象初始化的时候也是资源获取的时候，从而使对象的所有资源在单行代码中创建。

实践中，*RAII* 的主要原则就是把任何在堆上分配的资源(比如动态分配的内存或者系统对象的处理)的所有权提供给在栈上分配的对象(其析构函数包含释放资源及相关清理的代码)。

大多数时候，当你初始化一个原始指针或者资源句柄使其指向实际的资源时，立即将其传给智能指针。

在现代 C++ 中，原始指针只用于包含在局部作用域，循环或者工具函数的小块代码中(对性能有要求，并且对资源的所有权也不容易混淆)。

**原始指针和智能指针的声明比较如下：**

```cpp
void UseRawPointer()
{
    // Using a raw pointer -- not recommended.
    Song* pSong = new Song(L"Nothing on You", L"Bruno Mars"); 

    // Use pSong...

    // Don't forget to delete!
    delete pSong;   
}

void UseSmartPointer()
{
    // Declare a smart pointer on stack and pass it the raw pointer.
    unique_ptr<Song> song2(new Song(L"Nothing on You", L"Bruno Mars"));

    // Use song2...
    wstring s = song2->duration_;
    //...

} // song2 is deleted automatically here.
```

如上所示，智能指针是一个在栈上声明的类模板，并由指向分配在堆上的对象的原始指针初始化。当智能指针初始化后，它就拥有了原始指针的所有权。这意味着智能指针需要负责原始指针指向的内存释放。智能指针的析构函数包含了 **`delete`** 的调用，并且由于智能指针是在栈上声明的，其析构函数会在智能指针对象离开作用域时被调用，即使在栈中发生了异常。

通过使用指针运算符(`->` 和 `*`)访问被封装的指针，智能指针类重载了这些运算符以返回被封装的原始指针。

C++ 智能指针的理念类似于在 C# 语言中创建对象的过程：创建对象后让系统负责在正确的时间将其删除。不同之处在于，没有独立的垃圾回收器运行于后台；内存是按照标准 C++ 规范对内存进行管理的，使运行时环境更加快速和高效。

> [!重要]<br/>
> 总是在单独的行上创建智能指针，而不是在参数列表中，从而避免由于特定的参数列表分配规则出现一些轻微的内存泄漏

以下示例显示了 C++ 标准库中的 `unique_ptr` 是如何封装指向大型对象的指针的。


```cpp
class LargeObject
{
public:
    void DoSomething(){}
};

void ProcessLargeObject(const LargeObject& lo){}

void SmartPointerDemo()
{    
    // Create the object and pass it to a smart pointer
    std::unique_ptr<LargeObject> pLarge(new LargeObject());

    //Call a method on the object
    pLarge->DoSomething();

    // Pass a reference to a method.
    ProcessLargeObject(*pLarge);

} //pLarge is deleted automatically when function block goes out of scope.
```
上述示例演示了使用智能指针的关键步骤：

1. 将智能指针声明为局部变量(不要在智能指针上使用 **`new`** 或者 `malloc` 表达式)。
2. 在类型参数上，指定被封装指针指向的对象类型。
3. 将指向由 **`new`** 创建的对象的指针传给智能指针的构造函数。
4. 使用重载的操作符 `->` 和 `*` 来访问对象。
5. 让智能指针来 **`delete`** 对象。

智能指针在设计上兼顾了内存和性能的高效性。例如，`unique_ptr` 唯一的数据成员是被封装的原始指针，这意味着 `unique_ptr` 具有原始指针同样地大小，4 字节或者 8 字节。通过智能指针重载的操作符 `->` 和 `*` 来访问并不比直接使用原始指针来访问慢多少。

智能指针有其自己的成员函数，通过 `.` 来访问。例如，一些 C++ 标准库的智能指针有用于重置的成员函数来释放对原始指针的所有权。这可以用于在智能指针超出作用域前释放智能指针管理的内存，看下面的示例：

```cpp
void SmartPointerDemo2()
{
    // Create the object and pass it to a smart pointer
    std::unique_ptr<LargeObject> pLarge(new LargeObject());

    //Call a method on the object
    pLarge->DoSomething();

    // Free the memory before we exit function block.
    pLarge.reset();

    // Do some other work...

}
```

智能指针通常提供了获取原始指针的方式。 C++ 标准库中的智能指针包含了成员函数 `get` 来获取原始指针。 `CComPtr` 有公共的类成员 `p`。通过获取原始指针，你能够使用智能指针来管理你自己代码涉及的内存并依然能够将原始指针传递给不支持智能指针的代码。

```cpp
void SmartPointerDemo4()
{
    // Create the object and pass it to a smart pointer
    std::unique_ptr<LargeObject> pLarge(new LargeObject());

    //Call a method on the object
    pLarge->DoSomething();

    // Pass raw pointer to a legacy API
    LegacyLargeObjectFunction(pLarge.get());    
}
```

## 智能指针的种类

以下部分总结了在 Windows 环境下不同种类的智能指针，以及如何使用它们。

### C++ 标准库中的智能指针

优先使用下列智能指针来封装原始指针指向的纯旧对象(plain old C++ objects，POCO):

- `unique_ptr`<br/>
    - 对封装的原始指针是独占的
    - 默认用于 POCO，除非你明确的知道你需要一个 `shared_ptr`
    - 可以移入新的所有者，但不能拷贝或者共享
    - 替代 `auto_ptr`，`auto_ptr` 已作废
    - 对比 `boost::scoped_ptr`，`unique_ptr` 更加小巧和高效
    - 长度为一个指针的大小，并且支持右值引用来快速执行 C++ 标准库容器的插入和遍历操作

- `shared_ptr`<br/>
    - 引用计数智能指针
    - 当你需要将原始指针分派给多个所有者时使用，例如，当你从容器返回一个指针的拷贝并且想要保留它
    - 原始指针不会被 **`delete`** 直到**所有的** `shared_ptr` 超出作用域或者放弃所有权。
    - 长度为两个指针的大小，一个用于对象，另一个用于包含引用计数的共享控制块

- `weak_ptr`<br/>
    - 结合 `shared_ptr` 使用的特殊智能指针。
    - `weak_ptr` 提供了对被一个或者多个 `shared_ptr` 所拥有的对象的访问，但不参与引用计数。
    - 如果你想要监测某个对象，不要求其不被释放，可以使用 `weak_ptr`
    - 在某些情况下，用于解决 `shared_ptr` 实例间的循环引用。

### 扩展
- 用于 COM 组件的智能指针
- 用于 POCO对象的ATL智能指针

## 引用
- Microsoft Docs (https://docs.microsoft.com/en-us/cpp/cpp/smart-pointers-modern-cpp?view=msvc-160)
