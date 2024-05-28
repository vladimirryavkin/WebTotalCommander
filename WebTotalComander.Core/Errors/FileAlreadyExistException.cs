namespace WebTotalComander.Core.Errors
{
    [Serializable]
    public class FileAlreadyExistException : Exception
    {
        public FileAlreadyExistException() { }
        public FileAlreadyExistException(string message) : base(message) { }
        public FileAlreadyExistException(string message, Exception inner) : base(message, inner) { }
    }
}
