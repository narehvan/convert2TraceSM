using System.Collections.ObjectModel;
using WPLogs.core.Services;

namespace WPLogs.core
{
    public abstract class LogFileParser
    {
        public Collection<String> UnparsedLines { get; } = [];

        public required IFileReader InputFileReader { get; set; }

        abstract public void Parse();

        protected void AddToUnparsed(string line)
        {
            UnparsedLines.Add(line);
        }
    }
}
