namespace WPLogs.core.Services
{
    public class FileReader : IFileReader
    {
        public string FileName { get; set; } = string.Empty;

        private bool _disposed;
        private StreamReader? _streamReader;
        private FileStream? _fileStream;
        private bool _isFileOpenAlready;

        public FileReader()
        {
            SetValues(string.Empty);
        }

        public FileReader(string filename)
        {
            SetValues(filename);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the size of the specified file in bytes.
        /// </summary>
        /// <returns>The size of the file, in bytes.</returns>
        /// <exception cref="FileNotFoundException">Thrown if no file is specified or if the specified file does not exist.</exception>
        public long FileSizeInBytes()
        {
            if (FileName == null)
            {
                throw new FileNotFoundException("Speccify a file. no file specified");
            }

            if (!FileExists())
            {
                throw new FileNotFoundException("Could not find the file");
            }

            return new FileInfo(FileName).Length;
        }

        /// <summary>
        /// Reads a single line
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="OutOfMemoryException"></exception>
        /// <exception cref="IOException"></exception>
        /// <exception cref="Exception"></exception>
        public string? ReadLine()
        {
            if (!_isFileOpenAlready)
            {
                OpenFile();
            }

            string? line = null;
            try
            {
                /*
                 * A line is defined as a sequence of characters followed by a line feed ("\n"), a carriage return ("\r"), 
                 * or a carriage return immediately followed by a 
                 * line feed ("\r\n"). The string that is returned does not contain the terminating carriage return or line feed. 
                 * The returned value is null if the end of the input stream is reached.
                 * https://learn.microsoft.com/en-us/dotnet/api/system.io.streamreader.readline?view=net-8.0&redirectedfrom=MSDN#System_IO_StreamReader_ReadLine
                 */
                if (_streamReader is not null)
                {
                    line = _streamReader.ReadLine()!;
                }
                else
                {
                    throw new InvalidOperationException("Unable to create a file stream. filestream is null");
                }
            }
            catch (OutOfMemoryException eOutOfMemory)
            {
                CloseFile();
                throw new OutOfMemoryException(eOutOfMemory.ToString());
            }
            catch (IOException eIOException)
            {
                CloseFile();
                throw new IOException(eIOException.ToString());
            }
            catch (Exception e)
            {
                CloseFile();
                throw new Exception(e.ToString());
            }

            if (line == null)
            {
                CloseFile();
            }
            return line;
        }

        private void SetValues(string filename)
        {
            FileName = filename;
            _isFileOpenAlready = false;
            _fileStream = null;
            _streamReader = null;
            _disposed = false;
        }

        public bool FileExists()
        {
            if (FileName is not null)
                return File.Exists(FileName);
            return false;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                CloseFile();
            }
            _disposed = true;
        }

        /// <summary>
        /// Opens a file for reading
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="FileNotFoundException"></exception>
        private void OpenFile()
        {
            if (FileExists())
            {
                try
                {
                    _fileStream = new(FileName!, FileMode.Open, FileAccess.Read, FileShare.Read);
                    _streamReader = new(_fileStream);
                    _isFileOpenAlready = true;
                }
                catch (Exception e)
                {
                    _fileStream?.Dispose();
                    _isFileOpenAlready = false;
                    throw new InvalidOperationException(e.ToString());
                }
            }
            else
            {
                throw new FileNotFoundException("Unable to find the file. either file not found, or filename is null");
            }
        }

        private void CloseFile()
        {
            if (_streamReader != null)
            {
                _streamReader.Dispose();
                _streamReader = null;
            }

            if (_fileStream != null)
            {
                _fileStream.Dispose();
                _fileStream = null;
            }

            _isFileOpenAlready = false;
        }
    }
}
