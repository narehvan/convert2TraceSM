namespace WPLogs.core.Exceptions
{
    [Serializable]
    public class NoFileToParseException : Exception
    {
        public NoFileToParseException()
        {
        }

        public NoFileToParseException(string message)
            : base(message)
        {
        }

        public NoFileToParseException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
