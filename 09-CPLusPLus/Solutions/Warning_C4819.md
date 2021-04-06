# 关于编译告警 C4819 的完整解决方案

## 引言

今天迁移开发环境的时候遇到一个问题，同样的操作系统和 Visual Studio 版本，原始开发环境一切正常，但是迁移后 VS 出现了 C4819 告警，上网查了中文的一些博客，大部分涵盖几种解决方案：
- 修改工程文件或者源文件，禁用该告警
- 修改文件内容，避免使用非法字符
- 将文件重新以 Unicode 格式保存

这些方案都不适用于我的问题，我并**不想修改这些产生告警的头文件**，因为这并不是我负责的模块，同时也并一定就是源文件的问题。

## 问题调研

不得已还是搜索了微软的官方文档，找到关于 C4819 告警的定义如下：

### Compiler Warning (level 1) C4819

> The file contains a character that cannot be represented in the current code page (number). Save the file in Unicode format to prevent data loss.

### 出现 C4819 的原因
当编译 ANSI 源文件时，系统使用了无法显示所有字符的 codepage。

### 消除该告警的完整解决方案：
- 如果你不需要的话，可以移除非法字符，比如注释
- 使用 Unicode escape sequences 来创建只包含基本 ANSI 字符的文件
- 将文件重新以 Unicode 格式保存
- **在控制面板中设置 codepage 以支持源文件中非法的字符集**

关于什么是 Unicode escape sequences 可以参考 https://dencode.com/en/string/unicode-escape

关于如何用 Unicode 格式保存文件可以参考原文，链接在文末。

## 问题解决

对于我不想修改源文件的情况，只能通过修改系统的 codepage 来解决，以英文操作系统为例，中文系统可以对照修改：
- 打开 Control Panel
- 选择 Region (and Language)
- 选择 Administrative 页签
- 在 Language for non-Unicode programs 区域点击 "Change System Locale" 按钮
- 设置 Current system locale，比如 English (United States)。
- 点击 "OK" 保存。

重新编译代码，确认问题已经解决。

## 引用
- https://docs.microsoft.com/en-us/cpp/error-messages/compiler-warnings/compiler-warning-level-1-c4819?view=msvc-160
- https://knowledgebase.progress.com/articles/Article/4677