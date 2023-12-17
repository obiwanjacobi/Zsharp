using System;

namespace Maja.Compiler;

public class MajaException : Exception
{
    public MajaException() { }
    public MajaException(string message) : base(message) { }
    public MajaException(string message, Exception inner) : base(message, inner) { }
}