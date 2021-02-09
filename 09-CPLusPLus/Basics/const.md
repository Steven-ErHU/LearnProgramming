# const 

## 常见用法示例

### 变量申明
> &nbsp;&nbsp;&nbsp;&nbsp;**`const declaration`**

```cpp
//E0137: expression must be a modifiable lvalue
//E0144: a value of type "const int *" cannot be used to initialize an entity of type "int *"

    //const value
    int var1 = 1;                       //OK
    const int var2 = 2;                 //OK
    int const var3 = 3;                 //OK

    var1 = 10;                          //OK
    var2 = 10;                          //E0137
    var3 = 10;                          //E0137

    int* pVar1 = &var1;                 //OK
    int* pVar2 = &var2;                 //E0144

    *pVar1 = 20;                        //OK

    int const* pVar3 = &var1;           //OK
    int const* pVar4 = &var2;           //OK
    const int* pVar5 = &var3;           //OK

    *pVar3 = 10;                        //E0137
    pVar3 = pVar1;                      //OK

    //const ptr
    int* const pVar6 = &var1;           //OK
    *pVar6 = 20;                        //OK
    pVar6 = pVar1;                      //E0137
    
    //const value and const ptr
    int const* const pVar7 = &var1;     //OK
    *pVar7 = 20;                        //E0137
    pVar7 = pVar1;                      //E0137
```

**补充：**<br/> 
**对于函数形参，同上。**

### 函数声明
> &nbsp;&nbsp;&nbsp;&nbsp;**`member-function const`**

声明成员函数为 `const` 意味着该函数被禁止修改调用它的对象，包括：
- 无法修改任何非静态的数据成员
- 调用任何非静态成员函数。

关键字 `const` 必须同时标记到声明和实现。

非类成员函数无法声明为 `const`。

## C and C++ const 的区别

当在 C 源码中声明 `const` 变量：

```cpp
const int i = 2;
```

在其他模块中使用：
```cpp
extern const int i;
```

同样的行为在 C++ 源码中定义：
```cpp
extern const int i = 2;
```

如果想要在 C 模块中调用 C++ 模块的变量，那么需要在 C++ 中按如下定义变量以防止名称被 C++ 编译器破坏：

```cpp
extern "C" const int x = 10;
```
## 使用 const 的场景

只要允许，尽可能地使用 `const`。