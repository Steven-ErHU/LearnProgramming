# C++ 中的虚函数表及虚函数执行原理

为了实现虚函数，C++ 使用了虚函数表来达到延迟绑定的目的。虚函数表在动态/延迟绑定行为中用于查询调用的函数。

尽管要描述清楚虚函数表的机制会多费点口舌，但其实其本身还是比较简单的。

首先，每个包含虚函数的类（或者继承自的类包含了虚函数）都有一个自己的虚函数表。这个表是一个在编译时确定的静态数组。虚函数表包含了指向每个虚函数的函数指针以供类对象调用。

其次，编译器还在基类中定义了一个隐藏指针，我们称为 `*__vptr`，`*__vptr` 是在类实例创建时自动设置的，以指向类的虚函数表。`*__vptr` 是一个真正的指针，这和 `*this` 指针不同，`*this` 指针实际是一个函数参数，使编译器来达到自引用的目的。

结果就是，每个类对象都会多分配一个指针的大小，并且 `*__vptr` 是被派生类继承的。

如果你不清楚这些组件是怎么配合运作的，看下面的例子：

```cpp
class Base
{
public:
    virtual void function1() {};
    virtual void function2() {};
};
 
class D1: public Base
{
public:
    virtual void function1() {};
};
 
class D2: public Base
{
public:
    virtual void function2() {};
};
```

因为这里有 3 个类，编译器会创建 3 个虚函数表。

然后编译器会在使用了虚函数的最上层基类中定义一个隐藏指针。尽管这个过程编译器会自动处理，但我们还是通过下面的例子来说明指针添加的位置：

```cpp
class Base
{
public:
    FunctionPointer *__vptr;
    virtual void function1() {};
    virtual void function2() {};
};
 
class D1: public Base
{
public:
    virtual void function1() {};
};
 
class D2: public Base
{
public:
    virtual void function2() {};
};
```

`*__vptr` 在类对象创建的时候会设置成指向类的虚函数表。例如，类型 `Base` 被实例化的时候，`*__vptr` 就指向 `Base` 的虚函数表。类型 `D1` 或者 `D2` 被实例化的时候，`*__vptr` 就指向 `D1` 或者 `D2` 的虚函数表。

现在我们来看下虚函数表是怎么创建的。因为示例中每个类仅有 2 个虚函数，所以每个虚函数表会存放两个函数指针（分别指向 `function1()` 和 `function2()`）。

`Base` `对象的虚函数表最简单。Base` 对象只能访问 `Base` 类型的成员，不能访问 `D1` 或者 `D2` 的函数。所以 `Base` 的虚函数表中的两个指针分别指向 `Base::function1()` 和 `Base::function2()`。

`D1` 的虚函数表稍复杂点，`D1` 对象能够访问 `D1` 以及 `Base` 的成员。`D1` 重写了 `function1()`，但没有重写 `function2()`，所以 `D1` 的虚函数表中的两个指针分别指向 `D1::function1()` 和 `Base::function2()`。

`D2` 的虚函数表同理 `D1`，包含了分别指向 `Base::function1()` 和 `D2::function2()` 的指针。

![Virtual Table](..\assets\images\VTable.gif "Virtual Table") <br/>

考虑如果创建 `D1` 对象时会发生什么：

```cpp
int main()
{
    D1 d1;
}
```

因为 `d1` 是 `D1` 类型对象，`d1` 有它自己的 `*__vptr` 指向 `D1` 类型的虚函数表。

现在创建一个 `Base` 类型指针 `*dPtr` 指向 `d1`：

```cpp
int main()
{
    D1 d1;
    Base *dPtr = &d1;
 
    return 0;
}
```
重点：
> 因为 `dPtr` 是 `Base` 类型指针，它只指向 `d1` 对象的 `Base` 类型部分(即，指向 `d1` 对象中的 `Base` 子对象)，而  `*__vptr` 也在 `Base` 类型部分。所以 `dPtr` 可以访问 `Base` 类型部分中的 `*__vptr`。同时，这里注意，`dPtr->__vptr` 指向的是 `D1` 的虚拟函数表，这是在 `d1` 初始化时就确定的。所以结果，尽管 `dPtr` 是 `Base` 类型指针，但它能够访问 `D1` 的虚函数表。

因此，当有调用 `dPtr->function1()` 时，发生了什么？

```cpp
int main()
{
    D1 d1;
    Base *dPtr = &d1;
    dPtr->function1();
 
    return 0;
}
```

首先，程序识别到 `function1()` 是一个虚函数。<br/>
其次，程序使用 `dPtr->__vptr` 获取到了 `D1` 的虚函数表。<br/>
然后，它在 `D1` 的虚函数表中寻找可以调用的 `function1()` 版本，这里是 `D1::function1()`。<br/>
因此，`dPtr->function1()` 实际调用了 `D1::function1()`。

通过虚函数表，编译器和程序能够确定调用什么版本的虚函数，尽管使用的是指向/引用基类的指针或者引用。

调用虚函数会比调用非虚函数更慢，有以下几个原因：
- 必须使用 `*__vptr` 获取正确的虚函数。
- 必须建立虚函数表的索引来获取想要调用的函数。
- 调用找到的函数。

结果就是必须进行三次操作才能完成对函数的调用。但是对于现代计算机系统，这些额外操作增加的时间几乎可以忽略不计。

另外，每个使用虚函数表的类都有 `*__vptr` 指针，从而每个类对象都会多一个指针的空间。虚函数很强大，但是它确实产生了性能开销。