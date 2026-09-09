using System.Text;

namespace WPLogs.core.Services
{
    public class SingleFileWriter : ISingleFileWriter
    {
        public string FileName { get; set; } = string.Empty;
        public string NewLineCharacterSet { get; set; } = "\n";

        private StreamWriter? _streamWriter;
        private bool _isFileOpenAlready;

        public void DeleteFile()
        {
            if (FileName == null) throw new ArgumentException(null, nameof(FileName));
            if (FileExists())
                File.Delete(FileName);
        }

        public bool FileExists()
        {
            if (FileName == null) throw new ArgumentException(null, nameof(FileName));
            return File.Exists(FileName);
        }

        public long FileSizeInBytes()
        {
            long length = -1;
            if (FileExists())
            {
                length = new FileInfo(FileName).Length;
            }
            return length;
        }

        public void StartWriting()
        {
            _isFileOpenAlready = false;
            bool appendToFile = true;
            try
            {
                _streamWriter = new StreamWriter(FileName, appendToFile, Encoding.UTF8);
            }
            catch (Exception)
            {
                throw;
            }
            _isFileOpenAlready = true;
        }

        public void StopWriting()
        {
            try
            {
                if (_streamWriter is not null)
                {
                    _streamWriter.Close();
                    _streamWriter.Dispose();
                }
            }
            catch
            {
                _isFileOpenAlready = false;
                throw new Exception("Unable to close file");
            }

            _isFileOpenAlready = false;
        }

        public void WriteToFile(string text)
        {
            if (_streamWriter is null)
            {
                throw new InvalidOperationException("File not open yet. open file first");
            }

            try
            {
                if (_isFileOpenAlready == false)
                    StartWriting();

                // do a dos2unix conversion to ensure that the new line character is consistent across platforms
                text = text.Replace("\r\n", "\n").Replace("\r", "\n");
                _streamWriter.NewLine = NewLineCharacterSet;
                _streamWriter!.WriteLine(text);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
