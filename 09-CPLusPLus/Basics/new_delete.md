C++ 支持使用操作符 **`new`** 和 **`delete`** 来动态分配和释放对象。

**`new`** 运算符调用特殊函数 **`operator new`**，**`delete`** 运算符调用特殊函数 **`operator delete`**。

如果 **`new`** 分配内存失败，异常 `std::bad_alloc`会被抛出。

**可以如下测试内存分配失败的情况：**

```cpp
#include <iostream>
using namespace std;
#define BIG_NUMBER 100000000
int main() {
   int *pI = new int[BIG_NUMBER];
   if( pI == 0x0 ) {
      cout << "Insufficient memory" << endl;
      return -1;
   }
}
```
使用 **`new`** 操作符动态分配的内存可以通过 **`delete`** 操作符进行释放。
**`delete`** 操作符会触发调用类的析构函数(如果有的话)。

**以下示例为用户自定义的  **`operator new`** 和 **`operator delete`**：**

```cpp
#include <iostream>
using namespace std;

int fLogMemory = 0;      // Perform logging (0=no; nonzero=yes)?
int cBlocksAllocated = 0;  // Count of blocks allocated.

// User-defined operator new.
void *operator new( size_t stAllocateBlock ) {
   static int fInOpNew = 0;   // Guard flag.

   if ( fLogMemory && !fInOpNew ) {
      fInOpNew = 1;
      clog << "Memory block " << ++cBlocksAllocated
          << " allocated for " << stAllocateBlock
          << " bytes\n";
      fInOpNew = 0;
   }
   return malloc( stAllocateBlock );
}

// User-defined operator delete.
void operator delete( void *pvMem ) {
   static int fInOpDelete = 0;   // Guard flag.
   if ( fLogMemory && !fInOpDelete ) {
      fInOpDelete = 1;
      clog << "Memory block " << cBlocksAllocated--
          << " deallocated\n";
      fInOpDelete = 0;
   }

   free( pvMem );
}

int main( int argc, char *argv[] ) {
   fLogMemory = 1;   // Turn logging on
   if( argc > 1 )
      for( int i = 0; i < atoi( argv[1] ); ++i ) {
         char *pMem = new char[10];
         delete[] pMem;
      }
   fLogMemory = 0;  // Turn logging off.
   return cBlocksAllocated;
}
```
上述代码可以用来检测内存泄漏，重新定义全局操作符 **`new`** 和 **`delete`** 来计算内存的分配和释放次数。

编译器支持在类中声明成员数组 **`new`** 和 **`delete`** 操作符：

```cpp
class X  {
public:
   void * operator new[] (size_t) {
      return 0;
   }
   void operator delete[] (void*) {}
};

void f() {
   X *pX = new X[5];
   delete [] pX;
}
```
