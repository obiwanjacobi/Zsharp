using System;

namespace Zsharp.AST
{
    [Serializable]
    public class AstException : ZsharpException
    {
        public AstException() { }
        public AstException(string message) : base(message) { }
        public AstException(string message, Exception inner) : base(message, inner) { }
        protected AstException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }


    [Serializable]
    public class InternalErrorException : AstException
    {
        public InternalErrorException() { }
        public InternalErrorException(string message) : base(message) { }
        public InternalErrorException(string message, Exception inner) : base(message, inner) { }
        protected InternalErrorException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
