namespace WPLogs.core.Services
{
    public interface IFileReader : IDisposable
    {
        public string FileName { get; set; }
        public string? ReadLine();
        public long FileSizeInBytes();
        public bool FileExists();
    }
}
