using System;

namespace Maja.Compiler;

[Serializable]
public class MajaException : Exception
{
    public MajaException() { }
    public MajaException(string message) : base(message) { }
    public MajaException(string message, Exception inner) : base(message, inner) { }
    protected MajaException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}