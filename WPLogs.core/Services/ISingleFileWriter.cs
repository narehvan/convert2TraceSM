namespace WPLogs.core.Services
{
    public interface ISingleFileWriter
    {
        public string NewLineCharacterSet { get; set; }
        public string FileName { get; set; }

        public void StartWriting();
        public void WriteToFile(string text);
        public void StopWriting();

        public void DeleteFile();
        public bool FileExists();
        public long FileSizeInBytes();
    }
}
