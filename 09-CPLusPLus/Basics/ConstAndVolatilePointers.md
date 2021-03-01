# const 和 volatile 指针

关键字 **`const`** 和 **`volatile`** 规定了指针的处理方式：

- **`const`** 规定指针在初始化后是受保护的，不能够再修改。

- **`volatile`** 规定了变量的值能够被用户应用程序外部的操作所修改。

因此，关键字 **`volatile`** 可以声明共享内存中的对象来和中断服务例程进行通信。共享内存可以被多个进程或者全局数据块使用。

每次程序调用声明为 **`volatile`** 的变量时，编译器将会从内存中重新读取值。这显著缩小了可能的优化空间。然而，当对象的状态能够发生非预期的变化时，这是保证程序正常运行的唯一方法。

**声明指针为 **`const`** 或 **`volatile`**：**

> ```cpp
> const char *cpch;
> volatile char *vpch;
> ```

声明指针的值为 **`const`** 或 **`volatile`**：

> ```cpp
> char * const pchc;
> char * volatile pchv;
> ```

*const 指针的用法已经在 "C++ 中的 const 关键字" 中举例说明，此处不再赘述。*