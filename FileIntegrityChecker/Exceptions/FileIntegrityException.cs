using System;

namespace FileIntegrityChecker.Exceptions;

/// <summary>
/// Base exception for all File Integrity Checker errors.
/// </summary>
// OOP: Custom Exception (Inherits from Exception - demonstrates Inheritance)
// OOP: Encapsulation (exposes only relevant error info)
public class FileIntegrityException : Exception
{
    // OOP: Auto-property with private setter
    public string ErrorCode { get; private set; }

    // OOP: Parameterized Constructor
    public FileIntegrityException(string message, string errorCode = "FIC_ERR")
        : base(message)
    {
        ErrorCode = errorCode;
    }

    // OOP: Constructor chaining with inner exception
    public FileIntegrityException(string message, Exception innerException, string errorCode = "FIC_ERR")
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}
