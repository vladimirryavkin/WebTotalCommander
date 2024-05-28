using System.Runtime.Serialization;

namespace WebTotalComander.Core.Errors
{
    [Serializable]
    public class NoFileException : Exception
    {
        public NoFileException() { }
        public NoFileException(string message) : base(message) { }
        public NoFileException(string message, Exception inner) : base(message, inner) { }
        protected NoFileException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
