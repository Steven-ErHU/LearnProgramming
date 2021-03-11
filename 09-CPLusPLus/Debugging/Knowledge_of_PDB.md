## 关于 PDB 文件你需要知道什么？

## 引言

大多数人知道 PDB 文件是用来帮助我们 debug 的，但也仅此而已。如果你不知道更多关于 PDB 文件的知识也不要紧，因为大多数相关文档比较分散且主要面向编译器和调试器的开发人员。尽管编写编译器和调试器的工作很酷很有趣，但这很少会是我们的工作。

本文主要介绍当你遇到 PDB 文件时（windows 开发中），你必须要知道的内容。

## 重要的事情说三遍

**PDB 文件和源代码一样重要！！!** 

**PDB 文件和源代码一样重要！！!** 
 
**PDB 文件和源代码一样重要！！!** 

## 开始之前    

首先定义两个概念：
- **本地编译**：在你本机开发环境中的编译。
- **官方编译**：在编译服务器上的编译。

这两种编译的区分很重要，因为调试本地编译往往很简单，但是问题往往出现在官方编译中。

官方编译至少需要有一个地方（Symbol Server）来存放编译出来的二进制及 PDB 文件。这样当某个版本发现任何问题，我们可以获取到对应的 PDB 文件进行调试。
没有匹配的 PDB 文件，调试器几乎不可能完成调试任务，或者你将付出高昂的代价才能解决问题。

> 更多关于 Symbol Server 的内容，参考 https://docs.microsoft.com/en-us/archive/msdn-magazine/2002/june/bugslayer-symbols-and-crash-dumps

Visual Studio 和 WinDBG 知道如何访问 Symbol Server 并且如果二进制文件来自官方编译，那么调试器能够自动加载匹配的 PDF 文件。

在将 PDB 文件放在 Symbol Server 上之前，还需要做一件事：在官方编译出来的 PDB 文件上，通过 Source Server 工具，进行源文件索引（source indexing）。索引过程会嵌入版本控制命令用于拉取当前版本编译的源文件。这样，当你调试当前版本时，你就不用担心找不到版本的源文件。

> 更多关于 Source Server 的内容，参考 https://docs.microsoft.com/en-us/archive/msdn-magazine/2006/august/source-server-helps-you-kill-bugs-dead-in-visual-studio-2005

## 什么是 PDB 文件？

现在，我们可以开始介绍什么是 PDB 文件，以及调试器是如何查找 PDB 文件了。
实际的 PDB 文件格式是加密的，但是微软提供了 API，为调试器提供 PDB 文件的数据。

非托管的 C++ PDB 文件包含了以下信息：
- 公有函数，私有函数和静态函数的地址
- 全局变量的名称和地址
- 参数和局部变量的名称以及栈上的偏移量
- 类，结构体以及数据定义的类型信息
- FPO（Frame Pointer Omission） 数据
- 源文件的文件名及行信息。

而 .NET 的 PDB 文件只包含了两项内容：
- 源文件的文件名及行信息
- 局部变量的名称

其他所有信息已经存放在 .NET 元数据中，所以没有必要再 PDB 文件中冗余。

## PDB 文件的加载

当模块被加载到进程的地址空间后，调试器会使用两种信息找到匹配的 PDB 文件。首先，当然是文件名。如果你加载 ZZZ.DLL，那么调试器就会查找 ZZZ.PDB。
更重要的是，调试器如何知道这就是匹配的 PDB 文件？这是通过比对内嵌于 PDB 文件和二进制文件中的 GUID 来确认的。

负责将 GUID 嵌入二进制和 PDB 文件的是编译器（.NET）或者链接器（C++）。想想，历史编译的版本如果没有保存 PDB 文件，你还能调试吗？答案是否定的，哪怕你没有修改源文件！你可能会想是否可以修改 PDB 文件的 GUID？很遗憾，答案也是否定的。

你可以查看二进制文件中的 GUID。使用 Visual Studio -> DUMPBIN 的命令行工具，你可以列出所有的 PE（Portable Executable） 文件内容。可以在 Visual Studio 的命令行工具中调用 DUMPBIN。

> 更多关于 DUMPBIN 的内容参考：https://docs.microsoft.com/en-us/archive/msdn-magazine/2002/february/inside-windows-win32-portable-executable-file-format-in-detail 和 https://docs.microsoft.com/en-us/archive/msdn-magazine/2002/march/inside-windows-an-in-depth-look-into-the-win32-portable-executable-file-format-part-2

DUMPBIN 有很多命令行指令，其中显示 GUID 的指令是 /HEADERS。在输出内容中，对我们来说重要的是 Debug Directories 部分的内容：

```
Debug Directories

        Time Type        Size      RVA  Pointer
    -------- ------- -------- -------- --------
    6045C20E cv            60 00541AC8   5408C8    Format: RSDS, {DC80D058-127B-4379-B859-3F9F6978A4DB}, 1, C:ZZZ.pdb
```

知道了调试器如何确定匹配的 PDB 文件，下一步我们讨论调试器从哪里查找 PDB 文件。首先，调试器会在加载二进制文件的目录查找对应的 PDB 文件，如果没有找到，那么就查找PE 文件中 Debug Directories 内容里硬编码的 PDB 文件路径，在上面的输出示例中是 "C:ZZZ.pdb"（.NET 应用编译工具 MSBUILD会将 PDB 文件编译到 OBJ<Debug/Relase/...> 目录下，如果编译成功，再拷贝到 DEBUG 或者 RELEASE 目录）。如果在上述两个位置都没有找到，但是建立了 Symbol Server，那么调试器会在 Symbol Server 的缓存目录里继续查找。这种查找顺序也保证了本地编译和官方编译不会有冲突。

在 Visual Studio 中调试的时候，你可以在窗口 Modules 中的列 Symbol File 里看到 PDB 文件的位置。 

对大多数应用来讲这种加载方式都没有问题。但是对于需要将程序集放入 GAC（Global Assembly Cache）的 .NET 应用，PDB 的加载就会变得有趣了。对于本地编译，调试器会在编译目录找到 PDB 文件，所以没什么问题。问题来源于当你想要在其他机器上调试本地编译版本。

在其他机器上调式，很多人会用 Gacutil.exe 将程序集放入 GAC，然后打开命令行在 "C:WINDOWSASSEMBLY" 下查找程序集的物理位置。但是基于 Any CPU 编译的程序集实际上会放入类似 "C:WindowsassemblyGAC_MSILExample1.0.0.0__682bc775ff82796a" 的路径。

上述路径中，Example 是程序集名称，1.0.0.0 是版本号，682bc775ff82796a 是公有秘钥令牌值（public key token value）。当你推断出这个路径后，你可以将 PDB 文件拷入这个目录然后调试器会加载它。

## PDB 文件的内容

对于官方编译，因为有源文件索引工具，所以 PDB 文件中会存储版本控制命令，用于将源文件放入你配置的源文件缓存池。对于本地编译，PDB 文件中存储的是二进制文件对应的源文件的完整路径。换句话说，如果你使用 C:FOO 中的源文件 MYCODE.CPP，那么 PDB 文件中存储的就是 C:FOOMYCODE.CPP。

理论上，所有的官方编译会自动立马进行源文件索引，并将内容存储于 Symbol Server，以至于你都不用考虑源文件在哪。然而，有些开发团队在测试及其他环节中会考量编译结果是否满足使用的要求，在此之前，不会对 PDB 文件进行源文件索引。如果你确实需要调试未索引的版本，最好将源代码下载到本地时保证和编译服务器相同的磁盘和目录，否则，你可能会在调试时遇到麻烦。尽管 Visual Studio 调试器和 WinDBG 有配置源文件搜索路径的选项，但要配置正确并不容易。

## 引用
https://www.wintellect.com/pdb-files-what-every-developer-must-know/
