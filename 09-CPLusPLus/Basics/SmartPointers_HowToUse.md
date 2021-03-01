# 智能指针的创建和使用


## unique_ptr

**`unique_ptr`** 指针是不能共享的，它不能被拷贝到另一个 **`unique_ptr`**，或通过值传递给函数，或任何 C++ 标准算法中需要拷贝操作的地方。一个 **`unique_ptr`** 只能将内存资源的所有权转移到另一个 **`unique_ptr`**，并且其不再享有资源的所有权。建议严格控制对象的 **owner** ，最好只有一个，避免增加程序的逻辑复杂性。因此，当你需要一个用于 POCO 的智能指针时，使用 **`unique_ptr`**， 并且在创建 **`unique_ptr`** 的时候使用 **`make_unique`** 函数。

Therefore, when you need a smart pointer for a plain C++ object, use `unique_ptr`, and when you construct a `unique_ptr`, use the [make_unique](../standard-library/memory-functions.md#make_unique) helper function.

The following diagram illustrates the transfer of ownership between two `unique_ptr` instances.

![Moving the ownership of a unique&#95;ptr](media/unique_ptr.png "Moving the ownership of a unique&#95;ptr")

`unique_ptr` is defined in the `<memory>` header in the C++ Standard Library. It is exactly as efficient as a raw pointer and can be used in C++ Standard Library containers. The addition of `unique_ptr` instances to C++ Standard Library containers is efficient because the move constructor of the `unique_ptr` eliminates the need for a copy operation.

## Example 1

The following example shows how to create `unique_ptr` instances and pass them between functions.

[!code-cpp[stl_smart_pointers#210](codesnippet/CPP/how-to-create-and-use-unique-ptr-instances_1.cpp)]

These examples demonstrate this basic characteristic of `unique_ptr`: it can be moved, but not copied. "Moving" transfers ownership to a new `unique_ptr` and resets the old `unique_ptr`.

## Example 2

The following example shows how to create `unique_ptr` instances and use them in a vector.

[!code-cpp[stl_smart_pointers#211](codesnippet/CPP/how-to-create-and-use-unique-ptr-instances_2.cpp)]

In the range for  loop, notice that the `unique_ptr` is passed by reference. If you try to pass by value here, the compiler will throw an error because the `unique_ptr` copy constructor is deleted.

## Example 3

The following example shows how to initialize a `unique_ptr` that is a class member.

[!code-cpp[stl_smart_pointers#212](codesnippet/CPP/how-to-create-and-use-unique-ptr-instances_3.cpp)]

## Example 4

You can use [make_unique](../standard-library/memory-functions.md#make_unique) to create a `unique_ptr` to an array, but you cannot use `make_unique` to initialize the array elements.

[!code-cpp[stl_smart_pointers#213](codesnippet/CPP/how-to-create-and-use-unique-ptr-instances_4.cpp)]

For more examples, see [make_unique](../standard-library/memory-functions.md#make_unique).

## See also

[Smart Pointers (Modern C++)](smart-pointers-modern-cpp.md)<br/>
[make_unique](../standard-library/memory-functions.md#make_unique)
