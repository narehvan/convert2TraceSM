using System.Text;
using WPLogs.core.Services;

namespace UnitTestProject.Mockers
{
    internal class MockSingleFileWriter : ISingleFileWriter
    {
        public string NewLineCharacterSet { get; set; } = "\n";
        public string FileName { get; set; } = "MockFile.txt";

        private StringBuilder _fileContent = new StringBuilder();
        
        public MockSingleFileWriter(string filename)
        {
            FileName = filename;
        }

        public void DeleteFile()
        {
            return;
        }

        public bool FileExists()
        {
            return true;
        }

        public long FileSizeInBytes()
        {
            return _fileContent.Length;
        }

        public void StartWriting()
        {
            return;
        }

        public void StopWriting()
        {
            return;
        }

        public void WriteToFile(string text)
        {
            _fileContent.Append(text);
        }

        public string GetContent()
        {
            return _fileContent.ToString();
        }
    }
}
