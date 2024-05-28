using System.Runtime.Serialization;

namespace WebTotalComander.Core.Errors
{
    [Serializable]
    public class RequestParametrsInvalidExeption : Exception
    {
        public RequestParametrsInvalidExeption() { }
        public RequestParametrsInvalidExeption(string message) : base(message) { }
        public RequestParametrsInvalidExeption(string message, Exception inner) : base(message, inner) { }
        protected RequestParametrsInvalidExeption(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
