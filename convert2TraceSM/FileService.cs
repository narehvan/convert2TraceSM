using System.IO.Pipelines;
using WPLogs.core.Services;

namespace convert2TraceSM
{
    internal class FileService
    {
        internal static IFileReader CreateFileReaderService(string inputFilename)
        {
            return new FileReader(inputFilename);
        }

        internal static ISingleFileWriter CreateSingleFileWriterService(string prefix, string destinationpath, string outputFilename)
        {
            string filename = prefix + "_" + outputFilename;
            string newfilepathandname = Path.Combine(destinationpath, filename);
            SingleFileWriter writer = new()
            {
                FileName = newfilepathandname,
            };
            return writer;
        }
    }
}
