## C++ 标准库容器

### 标准库容器

容器是存储类型对象的集合，它在编程中应用非常广泛：动态数组（array），队列（queue），栈（stack），堆（优先队列），链表（list），树（set），相关（associative）数组（map）...

标准库容器提供了各种成员函数来操作其元素，并且它被设计成类模板，为类型提供了最大的灵活性。

许多容器包含了相同的成员函数及功能。使用哪种类型额容器并不是简单的依据容器的功能来断定的，还需要考虑效率。尤其是序列容器（sequence containers），需要综合考虑元素的存取，插入，删除等操作的效率问题。

stack, queue 和 priority_queue 是作为容器的适配器（container adaptors）来实现的。容器适配器没有完整的容器类，但是提供了依赖于其他容器类（如 deque 或者 list）的特定接口来操作元素。

### 标准库容器列表

- Sequence containers:
    - array 
    - vector
    - deque
    - forward_list 
    - list

- Container adaptors:
    - stack
    - queue
    - priority_queue

- Associative containers:
    - set
    - multiset
    - map
    - multimap

- Unordered associative containers:
    - unordered_set 
    - unordered_multiset 
    - unordered_map 
    - unordered_multimap 

### array

数组是存放在连续内存区域的一系列同类型对象的集合。传统 C 风格数组很容易出 bug，但是在遗留代码中还是非常常见的。

现代 C++ 中，强烈建议使用 std::vector 和 std::array。这两种容器都将元素存储在连续的内存区域，并且提供了更强的类型安全，它们提供的迭代器总是保证能够指向容器内有效的数据。

#### std::array

`std::array` 提供的对象包含了类型 Ty 的数组，数组内有 N 个元素。

`std::array` 是静态的，其元素个数是固定的。

#### 语法
> template <class Ty, std::size_t N> <br/>
> class array;

`std::array` 包含的成员请参考 https://docs.microsoft.com/en-us/cpp/standard-library/array-class-stl?view=msvc-160


示例1，`std::array` 初始化：

```cpp
array<int, 4> ai = { 1, 2, 3 };
```
实例化对象 `ai`，包含四个 `int` 类型元素，并初始化四个元素分别为 1，2，3 和 0。


示例2：

```cpp
#include <array>
#include <iostream>

typedef std::array<int, 4> Myarray;

int main()
{
    typedef std::array<int, 4> Myarray;

    Myarray c0 = { 0, 1, 2, 3 };

    // display contents " 0 1 2 3"
    for (const auto& it : c0)
    {
        std::cout << " " << it;
    }
    std::cout << std::endl;

    Myarray c1(c0);

    // display contents " 0 1 2 3"
    for (const auto& it : c1)
    {
        std::cout << " " << it;
    }
    std::cout << std::endl;

    return (0);
}
```
输出:
```
 0 1 2 3
 0 1 2 3
```

示例3，array::at：

```cpp
#include <array>
#include <iostream>

typedef std::array<int, 4> Myarray;
int main()
{
    Myarray c0 = { 0, 1, 2, 3 };

    // display odd elements " 1 3"
    std::cout << " " << c0.at(1);
    std::cout << " " << c0.at(3);
    std::cout << std::endl;

    return (0);
}
```
如果索引位置无效，抛出 `out_of_range` 异常。

示例4，array::front 和 array::back：
```cpp
#include <array>
#include <iostream>

typedef std::array<int, 4> Myarray;
int main()
{
    Myarray c0 = { 0, 1, 2, 3 };

    // display first element " 0"
    std::cout << " " << c0.front();
    std::cout << std::endl;

    // display last element " 3"
    std::cout << " " << c0.back();
    std::cout << std::endl;

    return (0);
}
```

输出：

```cpp
 0
 3
```

示例5，array::begin 和 array::end，注意 `*` 号:

```cpp
#include <array>
#include <iostream>

typedef std::array<int, 4> Myarray;
int main()
{
    Myarray c0 = { 0, 1, 2, 3 };

    // display first element " 0"
    std::cout << " " << *c0.begin();
    std::cout << std::endl;

    // Error: cannot dereference out of range array iterator
    std::cout << " " << *c0.end();
    std::cout << std::endl;

    return (0);
}
```

示例6，array::cbegin 和 array::const_iterator：

```cpp
#include <array>
#include <iostream>

typedef std::array<int, 4> Myarray;
int main()
{
    Myarray c0 = { 0, 1, 2, 3 };

    // display first element " 0"
    Myarray::iterator it1 = c0.begin();

    //E0312 no suitable user - defined conversion from "std::_Array_const_iterator<int, 4U>" to "std::_Array_iterator<int, 4U>"
    Myarray::iterator it2 = c0.cbegin(); 

    Myarray::const_iterator it3 = c0.cbegin();


    *it1 = 10;
    *it3 = 10; //E0137 expression must be a modifiable lvalue

    return (0);
}
```

array::cend 同理。

示例7，array::const_pointer：
```cpp
#include <array>
#include <iostream>

typedef std::array<int, 4> Myarray;
int main()
{
    Myarray c0 = { 0, 1, 2, 3 };

    // display first element " 0"
    Myarray::const_pointer ptr = &*c0.begin();

    //E0413	no suitable conversion function from "std::_Array_iterator<int, 4U>" to "const int *" exists
    Myarray::const_pointer ptr = c0.begin(); 

    std::cout << " " << *ptr;
    std::cout << std::endl;

    return (0);
}
```

示例8，array::const_reference：

```cpp
#include <array>
#include <iostream>

typedef std::array<int, 4> Myarray;
int main()
{
    Myarray c0 = { 0, 1, 2, 3 };

    // display first element " 0"
    Myarray::const_reference ref = *c0.begin();
    std::cout << " " << ref;
    std::cout << std::endl;

    ref = 10; //E0137 expression must be a modifiable lvalue

    return (0);
}
```

示例9，array::rbegin 和 array::const_reverse_iterator：

```cpp
#include <array>
#include <iostream>

typedef std::array<int, 4> Myarray;
int main()
{
    Myarray c0 = { 0, 1, 2, 3 };

    // display last element " 3"
    Myarray::const_reverse_iterator it2 = c0.rbegin();
    std::cout << " " << *it2;
    std::cout << std::endl;

    return (0);
}
```
array::crbegin 同理。

注意：

> array::end，array::cend，array::rend 和 array::crend 用于判断迭代器是否到达末尾，不能够解引用。

示例10，array::data：
```cpp
#include <array>
#include <iostream>

typedef std::array<int, 4> Myarray;
int main()
{
    Myarray c0 = { 0, 1, 2, 3 };

    // return the address of the first element
    Myarray::pointer ptr = c0.data();

    // display first element " 0"
    std::cout << " " << *ptr;
    std::cout << std::endl;

    return (0);
}

```

示例11，array::difference_type：
```cpp
#include <array>
#include <iostream>

typedef std::array<int, 4> Myarray;
int main()
{
    Myarray c0 = { 0, 1, 2, 3 };

    // display distance first-last " -4"
    Myarray::difference_type diff = c0.begin() - c0.end();
    std::cout << " " << diff;
    std::cout << std::endl;

    return (0);
}
```

array::difference_type 为 std::ptrdiff_t 类型;

示例12，array::empty：

```cpp
#include <array>
#include <iostream>

typedef std::array<int, 4> Myarray;
int main()
{
    Myarray c0 = { 0, 1, 2, 3 };

    // display whether c0 is empty " false"
    std::cout << std::boolalpha << " " << c0.empty();
    std::cout << std::endl;

    std::array<int, 0> c1;

    // display whether c1 is empty " true"
    std::cout << std::boolalpha << " " << c1.empty();
    std::cout << std::endl;

    return (0);
}
```

示例12，array::fill：

```cpp
#include <array>
#include <iostream>

int main()
{
    using namespace std;
    array<int, 2> v1 = { 1, 2 };

    cout << "v1 = ";
    for (const auto& it : v1)
    {
        std::cout << " " << it;
    }
    cout << endl;

    v1.fill(3);
    cout << "v1 = ";
    for (const auto& it : v1)
    {
        std::cout << " " << it;
    }
    cout << endl;
}
```

输出：
```
v1 =  1 2
v1 =  3 3
```

示例13，array::max_size，同 array::size：

```cpp
#include <array>
#include <iostream>

typedef std::array<int, 4> Myarray;
int main()
{
    Myarray c0 = { 0, 1, 2, 3 };

    // display (maximum) size " 4"
    std::cout << " " << c0.max_size();
    std::cout << std::endl;

    return (0);
}
```

示例14，array::operator[]：
```cpp
#include <array>
#include <iostream>

typedef std::array<int, 4> Myarray;
int main()
{
    Myarray c0 = { 0, 1, 2, 3 };

    // display odd elements " 1 3"
    std::cout << " " << c0[1];
    std::cout << " " << c0[3];
    std::cout << std::endl;

    return (0);
}
```
示例15，array::operator=：
```cpp
#include <array>
#include <iostream>

typedef std::array<int, 4> Myarray;
int main()
{
    Myarray c0 = { 0, 1, 2, 3 };

    Myarray c1;
    c1 = c0;

    // display copied contents " 0 1 2 3"
    for (auto it : c1)
    {
        std::cout << " " << it;
    }
    std::cout << std::endl;

    return (0);
}
```

示例16，array::swap：
```cpp
#include <array>
#include <iostream>

typedef std::array<int, 4> Myarray;
int main()
{
    Myarray c0 = { 0, 1, 2, 3 };

    // display contents " 0 1 2 3"
    for (const auto& it : c0)
    {
        std::cout << " " << it;
    }
    std::cout << std::endl;

    Myarray c1 = { 4, 5, 6, 7 };
    c0.swap(c1);

    // display swapped contents " 4 5 6 7"
    for (const auto& it : c0)
    {
        std::cout << " " << it;
    }
    std::cout << std::endl;

    std::swap(c0, c1);

    // display swapped contents " 0 1 2 3"
    for (const auto& it : c0)
    {
        std::cout << " " << it;
    }
    std::cout << std::endl;

    return (0);
}
```

#### std::vector



## 引用 
- https://www.cplusplus.com/reference/stl/
- https://docs.microsoft.com/en-us/cpp/cpp/arrays-cpp?view=msvc-160#:~:text=%20Arrays%20(C++)%20%201%20Stack%20declarations.%20In,...%20%204%20See%20also.%20%20More
- 