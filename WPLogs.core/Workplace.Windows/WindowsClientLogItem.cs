namespace WPLogs.core.Workplace.Windows
{
    public class WindowsClientLogItem : IDisposable
    {
        public int? Index { get; set; }
        public string? Date { get; set; }
        public string Time { get; set; }
        public DateTime DateAndTime { get; set; }
        public string? Level { get; set; }
        public string? ProcessID { get; set; }
        public string? Process { get; set; }
        public string? SubProcess { get; set; }
        public string Body { get; set; }
        public string? Header { get; set; }

        public WindowsClientLogItem()
        {
            Time = string.Empty;
            Body = string.Empty;
            /* The visual studio does not detect that Reset() sets the value to string.empty() and hence it does not supress the warning about being null */
            /* so I have to set them to string.empty here and then call Reset() again */
            Reset();
        }

        public WindowsClientLogItem(
            int index, 
            string date, 
            string time, 
            DateTime dateAndTime, 
            string level, 
            string processID, 
            string process, 
            string subprocess, 
            string body, 
            string header)
        {
            Index = index;
            Date = date;
            Time = time;
            DateAndTime = dateAndTime;
            Level = level;
            ProcessID = processID;
            Process = process;
            SubProcess = subprocess;
            Body = body;
            Header = header;
        }
        public override string ToString()
        {
            if (Body == null) 
                return string.Empty;
            return Body;
        }

        private void Reset()
        {
            Index = 0;
            Date = string.Empty;
            Time = string.Empty;
            DateAndTime = new DateTime();
            Level = string.Empty;
            ProcessID = string.Empty;
            Process = string.Empty;
            SubProcess = string.Empty;
            Body = string.Empty;
            Header = string.Empty;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Reset();
            }
        }
    }
}
