namespace Zsharp
{
    [System.Serializable]
    public class ZsharpException : System.Exception
    {
        public ZsharpException() { }
        public ZsharpException(string message) : base(message) { }
        public ZsharpException(string message, System.Exception inner) : base(message, inner) { }
        protected ZsharpException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}