https://docs.microsoft.com/en-us/cpp/cpp/errors-and-exception-handling-modern-cpp?view=msvc-160

C++ doesn't provide or require a finally block to make sure all resources are released if an exception is thrown. The resource acquisition is initialization (RAII) idiom, which uses smart pointers, provides the required functionality for resource cleanup. For more information, see How to: Design for exception safety. For information about the C++ stack-unwinding mechanism, see Exceptions and stack unwinding.


