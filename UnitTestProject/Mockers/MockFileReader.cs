using System.Reflection;
using System.Reflection.PortableExecutable;
using WPLogs.core.Services;

namespace UnitTestProject.Mockers
{
    internal class MockFileReader : IFileReader
    {
        public string FileName
        {
            get => _fileName;
            set => _fileName = value;
        }
        private string _fileName = string.Empty;
        private List<string> _lines = [];
        private bool _isFileOpenAlready = false;
        private int _lineReadPosition = -1;
        private bool _disposed = false;
        
        public MockFileReader(string filename)
        {
            FileName = filename;
        }
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public bool FileExists()
        {
            return true;
        }

        public long FileSizeInBytes()
        {
            long length = -1;
            if (FileName == null)
            {
                throw new FileNotFoundException("Speccify a file. no file specified");
            }

            if (!FileExists())
            {
                throw new FileNotFoundException("Could not find the file");
            }

            length = GetTestFileContentsByteSize(FileName);

            return length;
        }

        public string? ReadLine()
        {
            if (!_isFileOpenAlready)
            {
                OpenFile();
            }
            return NextLine()!;
        }

        public string GetContent()
        {
            string fileContents = string.Empty;

            var assembly = Assembly.GetCallingAssembly();
            var uri = String.Format("{0}.{1}", assembly.GetName().Name, FileName);
            if (assembly is not null)
            {
                using (Stream inputStream = assembly.GetManifestResourceStream(uri)!)
                using (var streamReader = new StreamReader(inputStream))
                {
                    if (streamReader is not null)
                    {
                        fileContents = streamReader.ReadToEnd();
                    }
                }
            }

            return fileContents;
        }

        private void OpenFile()
        {
            _lines = GetTestFileContentsAsList(_fileName);
            _isFileOpenAlready = true;
        }

        private string? NextLine()
        {
            _lineReadPosition++;
            if (_lineReadPosition >= _lines.Count)
            {
                return null;
            }

            return _lines[_lineReadPosition];
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
        }

        internal static List<string> GetTestFileContentsAsList(string filename)
        {
            string relativeFile = filename;
            List<string> lines = [];
            var assembly = Assembly.GetCallingAssembly();
            var uri = String.Format("{0}.{1}", assembly.GetName().Name, relativeFile);
            if (assembly is not null)
            {
                using (Stream inputStream = assembly.GetManifestResourceStream(uri)!)
                using (var streamReader = new StreamReader(inputStream))
                {
                    string line;
                    if (streamReader is not null)
                    {
                        while ((line = streamReader.ReadLine()!) != null)
                        {
                            lines.Add(line);
                        }
                    }
                }
            }
            return lines;
        }

        internal static long GetTestFileContentsByteSize(string filename)
        {
            string relativeFile = filename;
            List<string> lines = [];
            var assembly = Assembly.GetCallingAssembly();
            var uri = String.Format("{0}.{1}", assembly.GetName().Name, relativeFile);
            long bytecounter = 0;
            if (assembly is not null)
            {
                using (Stream inputStream = assembly.GetManifestResourceStream(uri)!)
                using (var streamReader = new StreamReader(inputStream))
                {
                    string line;
                    if (streamReader is not null)
                    {
                        while ((line = streamReader.ReadLine()!) != null)
                        {
                            bytecounter += line.Length;
                        }
                    }
                }
            }
            return bytecounter;
        }
    }
}
