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
            // do a dos2unix conversion to ensure that the new line character is consistent across platforms
            text = text.Replace("\r\n", "\n").Replace("\r", "\n");
            _fileContent.Append(text);
        }

        public string GetContent()
        {
            return _fileContent.ToString();
        }
    }
}
