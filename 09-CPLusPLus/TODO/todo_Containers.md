# C++ 标准库容器

## 标准库容器

容器是存储类型对象的集合，它在编程中应用非常广泛：动态数组（array），队列（queue），栈（stack），堆（优先队列），链表（list），树（set），相关（associative）数组（map）...

标准库容器提供了各种成员函数来操作其元素，并且它被设计成类模板，为类型提供了最大的灵活性。

许多容器包含了相同的成员函数及功能。使用哪种类型额容器并不是简单的依据容器的功能来断定的，还需要考虑效率。尤其是序列容器（sequence containers），需要综合考虑元素的存取，插入，删除等操作的效率问题。

stack, queue 和 priority_queue 是作为容器的适配器（container adaptors）来实现的。容器适配器没有完整的容器类，但是提供了依赖于其他容器类（如 deque 或者 list）的特定接口来操作元素。

## 标准库容器列表

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

## array

数组是存放在连续内存区域的一系列同类型对象的集合。传统 C 风格数组很容易出 bug，但是在遗留代码中还是非常常见的。

现代 C++ 中，强烈建议使用 std::vector 和 std::array。这两种容器都将元素存储在连续的内存区域，并且提供了更强的类型安全，它们提供的迭代器总是保证能够指向容器内有效的数据。

## std::array

`std::array` 提供的对象包含了类型 Ty 的数组，数组内有 N 个元素。

`std::array` 是静态的，其元素个数是固定的。

### 语法
> template <class Ty, std::size_t N> <br/>
> class array;

`std::array` 包含的成员请参考 https://docs.microsoft.com/en-us/cpp/standard-library/array-class-stl?view=msvc-160

## std::vector

`std::vector` 线性存储类型 Type 的元素。

`std::vector` 可以频繁的在序列头尾添加和删除元素，同时也支持在序列中间插入删除元素。

### 语法
> template <class Type, class Allocator = allocator<Type>> <br/>
class vector

`std::deque` 在序列头尾添加删除元素时效率更高。`std::list` 在序列中间添加删除元素时效率更高。

## std::deque

### 语法
> template < class T, class Alloc = allocator<T> > <br/>
> class deque;

deque（发音 "deck"）是 double-ended queue 的缩写，它是可以在首尾动态增删元素的顺序容器。

`std::deque` 允许随机访问任意元素，并自动控制容器的扩展和收缩。因此 `std::deque` 提供的功能非常类似 `std::vector`，并且在首尾增删元素更加高效。但是，和 `std::vector` `std::deque` 不保证元素顺序存储于连续的内存空间：通过移动指针来访问 `std::deque` 中的元素其行为是未定义的。

`std::vector` 和 `std::deque` 提供了类似的接口，但是在内部实现两者区别很大：`std::vector` 使用单个数组实现，有时需要重新分配内存来满足元素的增加，`std::deque` 的元素可以分散在不同的存储区域，容器负责保存必要的信息来支持随机访问。因此 `std::deque` 比 `std::vector` 在实现上更加复杂，但是在特定场景下 `std::deque` 在动态增加元素时更加高效，尤其是当元素较多，内存重新分配开销较大的时候。

对于频繁在容器中间增删元素的操作，`std::deque` 性能较弱。

## std::forward_list

### 语法
> template < class T, class Alloc = allocator<T> > <br/>
> class forward_list;

`std::forward_list` 是顺序容器，支持在容器任意位置随时进行增删元素。`std::forward_list` 为单链表实现，元素存储于随机的内存区域，通过元素之间的链（指针）实现关联。
`std::forward_list` 和 `std::list` 最大的不同是 `std::forward_list` 中元素之间的链是单向的，而 `std::list` 中元素的链是双向的。因此 `std::list` 中每个元素会多占用一个指针的位置，并且元素的增删操作不如 `std::forward_list` 高效。

相比其他标准顺序容器（array，vector 和 deque），`std::forward_list` 更加适合在容器任意位置增删元素，因此在算法中经常用到，比如排序算法。

`std::forward_list` 和 `std::list` 最大的缺点是不支持元素的随机存取，并且占用额外的空间用于存储元素之间的关联信息。

`std::forward_list` 模板类设计上和 C 风格的单链表效率相同，并且是标准容器中唯一不含 size 成员函数的，这主要是为了避免额外的存储空间用于存放计数，并且影响元素增删的效率。如果确实需要 size 信息，可以通过遍历 `std::forward_list` 进行计数。

## std::list

### 语法
> template < class T, class Alloc = allocator<T> > <br/>
> class list;

`std::list` 是顺序容器，支持在任意位置增删元素。`std::list` 类似于 `std::forward_list`，但是 `std::list` 可以双向遍历。

## std::stack

### 语法
> template <class T, class Container = deque<T> > <br/>
> class stack;

`std::stack` 被设计用于 **LIFO（last-in first-out）** 的场景，元素只能从容器的某一端进出。`std::stack` 可以通过封装其他标准容器实现，只要满足以下操作：
- `empty`
- `size`
- `back`
- `push_back`
- `pop_back`

比如 `std::vector`，`std::deque` 和 `std::list` 都满足以上条件。`std::stack` 默认使用 `std::deque` 实现。

## std::queue

### 语法
> template <class T, class Container = deque<T> > <br/>
> class queue;

`std::queue` 被设计用于 **FIFO（first-in first-out）** 的场景，元素只能从容器的某一端进，另一端出。`std::queue` 可以通过封装其他标准容器实现，只要满足以下操作：
- `empty`
- `size`
- `front`
- `back`
- `push_back`
- `pop_front`

比如 `std::deque` 和 `std::list` 都满足以上条件。`std::stack` 默认使用 `std::deque` 实现。

## std::priority_queue

### 语法
> template < class T,
>            class Container = vector<T>, 
>            class Compare = less<typename Container::value_type> >  <br/>
> class priority_queue;

根据某些严格弱排序规则，`std::priority_queue` 保证第一个元素永远是最大的。其类似于堆，元素可以在任意位置插入，并且只有最大的对元素才能被检索。

`std::priority_queue` 可以通过封装其他标准容器实现，只要满足以下操作：
- `empty`
- `size`
- `front`
- `push_back`
- `pop_back`

比如 `std::deque` 和 `std::vector` 都满足以上条件。`std::stack` 默认使用 `std::vector` 实现。

## std::set

### 语法
> template < class T,
>            class Compare = less<T>,
>            class Alloc = allocator<T> ><br/>
> class set;

`std::set` 的元素根据特定的顺序存储，元素的值必须是唯一的。容器内的元素不能够修改，只能插入或者删除。

TODO

## 引用 
- https://www.cplusplus.com/reference/stl/
- https://docs.microsoft.com/en-us/cpp/cpp/arrays-cpp?view=msvc-160#:~:text=%20Arrays%20(C++)%20%201%20Stack%20declarations.%20In,...%20%204%20See%20also.%20%20More

## std::map

