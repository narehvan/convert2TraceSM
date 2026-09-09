using System.Text;
using WPLogs.core.Services;
using WPLogs.core.Workplace.Windows;

namespace convert2TraceSM
{
    class Program
    {
        private static readonly string PRODUCT_NAME = "convert2TraceSM";
        private static readonly string PRODUCT_VERSION = "2.0.1.2";
        private static readonly string PRODUCT_AUTHOR = "Arbi Arzouman";

        private static readonly int APP_RETURN_SUCCESS = 1;
        private static readonly int APP_RETURN_FAIL = -1;

//        private static readonly string[] supportedApplicationTypes = { "windows", "macos", "ios", "android"};
        private static readonly string[] supportedApplicationTypes = { "windows" };

        /// <summary>
        /// Return values:
        /// 0  -> the app did nothing
        /// 1  -> the conversion was successful
        /// -1 -> something failed with the conversion
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        static int Main(string[] args)
        {
            int appSuccessReturnValue = 0;
            int appReturnValue;
            string appType;
            string inputFile;
            string outputFile;
            AppArgumentsManager argManager = new(args);

            if (args.Length == 0)
            {
                PrintComment("No arguments provided. Please use -h option to learn how to use this program.");
                return appSuccessReturnValue;
            }

            string versionOption = "-v";
            if (argManager.DoesOptionExistInArgumentList(versionOption))
            {
                PrintVersion();
                return appSuccessReturnValue;
            }

            string helpOption = "-h";
            if (argManager.DoesOptionExistInArgumentList(helpOption))
            {
                PrintHelp();
                return appSuccessReturnValue;
            }

            string applicationTypeOption = "-a";
            if (argManager.DoesOptionExistInArgumentList(applicationTypeOption))
            {
                appType = argManager.ExtractOptionWithParameter(applicationTypeOption);
            }
            else
            {
                PrintComment("No application type found. Please use -h option to learn how to use this program.");
                return appSuccessReturnValue;
            }

            string inputFileOption = "-i";
            if (argManager.DoesOptionExistInArgumentList(inputFileOption))
            {
                inputFile = argManager.ExtractOptionWithParameter(inputFileOption);
            }
            else
            {
                PrintComment("No input file found. Please use -h option to learn how to use this program.");
                return appSuccessReturnValue;
            }

            string outputFileOption = "-o";
            if (argManager.DoesOptionExistInArgumentList(outputFileOption))
            {
                outputFile = argManager.ExtractOptionWithParameter(outputFileOption);
            }
            else
            {
                PrintComment("No output file found. Please use -h option to learn how to use this program.");
                return appSuccessReturnValue;
            }

            // if any of the mandatory parmeters are empty, then fail. can't continue
            if (String.IsNullOrWhiteSpace(appType) || String.IsNullOrWhiteSpace(inputFile) || String.IsNullOrWhiteSpace(outputFile))
            {
                PrintComment("Invalid arguments. Unable to convert. Please use -h option to learn how to use this program.");
                return appSuccessReturnValue;
            }

            // normalize and null-check appType, then do a case-insensitive membership test
            if (IsSupportedApplicationType(appType))
            {
                appReturnValue = ConvertFile(appType, inputFile, outputFile);
            }
            else
            {
                PrintComment("Invalid application type. select one of the supported application types. Please use -h option to learn how to use this program.");
                PrintComment("Valid options:");
                PrintComment(string.Join(", ", supportedApplicationTypes));
                return -1;
            }

            return appReturnValue;
        }

        private static int ConvertFile(string appType, string inputFile, string outputFile)
        {
            Console.WriteLine($"selected application: {appType}");

            IFileReader fileReader = FileService.CreateFileReaderService(inputFile);
            string? outputDir = Path.GetDirectoryName(outputFile);
            ISingleFileWriter fileWriter = FileService.CreateSingleFileWriterService(
                PRODUCT_NAME,
                outputDir ?? string.Empty,
                Path.GetFileName(outputFile)
            );

            // this check is done earlier. this is just in case. 
            // if it's not in the supported list, then exit
            if (!IsSupportedApplicationType(appType))
            {
                PrintComment($"unexpected application: {appType}");
                return APP_RETURN_FAIL;
            }

            if (fileReader.FileExists() == false)
            {
                PrintComment("Input file not found: " + inputFile);
                return APP_RETURN_FAIL;
            }

            if (fileWriter.FileExists() == true)
            {
                PrintComment("NOTE! Output file exists. It will be overwritten: " + outputFile);
            }

            StringBuilder sipItems = new();

            try
            {
                switch (appType)
                {
                    case "windows":
                        WindowsClientLogProcessor logWin = new(fileReader);
                        logWin.Parse();
                        sipItems = logWin.ConvertToTraceSMSIP();
                        break;
                    case "macos":
                        Console.WriteLine("not yet implemented");
                        break;
                    case "ios":
                        Console.WriteLine("not yet implemented");
                        break;
                    case "android":
                        Console.WriteLine("not yet implemented");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return APP_RETURN_FAIL;
            }

            // write to the output file
            try
            {
                fileWriter.StartWriting();
                fileWriter.NewLineCharacterSet = "\n";
                fileWriter.WriteToFile(sipItems.ToString());
                fileWriter.StopWriting();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return APP_RETURN_FAIL;
            }

            ConsoleColor bgColour = Console.BackgroundColor;
            ConsoleColor fgColour = Console.ForegroundColor;

            if (sipItems.Length > 0)
            {
                string fileName = Path.GetFileName(outputFile);

                Console.BackgroundColor = ConsoleColor.Blue;
                Console.ForegroundColor = ConsoleColor.White;

                Console.WriteLine("Conversion was successful.");
                Console.WriteLine("Transfer the file: [ " + outputFile + " ] to a Session Manager CLI and run [ traceSM " + fileName + " ]");

                Console.BackgroundColor = bgColour;
                Console.ForegroundColor = fgColour;
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("Conversion completed. But nothing was converted. Check the input file if it's a valid log file.");
                Console.BackgroundColor = bgColour;
                Console.ForegroundColor = fgColour;
            }

            return APP_RETURN_SUCCESS;
        }

        private static void PrintVersion()
        {
            string productVersion = PRODUCT_VERSION;
            string developer = PRODUCT_AUTHOR;
            string productName = PRODUCT_NAME;

            Console.WriteLine("");
            Console.WriteLine("Extract SIP messages and convert them to Session Manager traceSM format");
            Console.WriteLine("Application name:\t[ " + productName + " ]");
            Console.WriteLine("Application version:\t[ " + productVersion + " ]");
            Console.WriteLine("Developed By:\t\t[ " + developer + " ]");
            Console.WriteLine("------------------------------------");
        }

        private static void PrintHelp()
        {
            PrintVersion();

            Console.WriteLine("Usage:");
            Console.WriteLine("\t -a\t Client type the debug logs belong to. [Mandatory!] Options are:");
            Console.WriteLine("\t \t\twindows - for Workplace for Microsoft Windows");
//            Console.WriteLine("\t \t\tmacos   - for Workplace for macOS");
//            Console.WriteLine("\t \t\tios     - for Workplace for iOS");
//            Console.WriteLine("\t \t\tandroid - for Workplace for Android");

            Console.WriteLine("");
            Console.WriteLine("\t -i\t input log file name. [Mandatory!]");
            Console.WriteLine("\t \t For Windows Client: the filename is UccLog.log- e.g., UccLog.log, or UccLog.2019-02-19.2.log");
            Console.WriteLine("\t \t For iOS Client: the filename is Workplacexxx.log- e.g., Workplace 2019-02-22 23-25.log");
            Console.WriteLine("\t \t If filename contains spaces, use \"\" - see example below");

            Console.WriteLine("");
            Console.WriteLine("\t -o\t output traceSM filename. [Mandatory!]");
            Console.WriteLine("\t \t If filename contains spaces, use \"\" - see example below");

            Console.WriteLine("");
            Console.WriteLine("\t -h\t print help information. Cannot be used with any other option");
            Console.WriteLine("\t -v\t print version information. Cannot be used with any other option");
            Console.WriteLine("");
            Console.WriteLine("Examples:");
            Console.WriteLine("\t" + PRODUCT_NAME + " -a windows -i UccLog.log -o ucclogtraceSM");
            Console.WriteLine("\t" + PRODUCT_NAME + " -a windows -i \"UccLog Workplace.log\" -o \"ucclogtraceSM.1\"");
            Console.WriteLine("\t" + PRODUCT_NAME + " -a windows -i \"c:\\temp\\UccLog.log\" -o \"c:\\temp\\ucclogtraceSM\"");
        }

        private static void PrintComment(string comment)
        {
            if (!String.IsNullOrEmpty(comment))
            {
                Console.WriteLine(comment);
            }
        }
        private static bool IsSupportedApplicationType(string appType)
        {
            appType = appType?.Trim() ?? string.Empty;
            return !string.IsNullOrEmpty(appType) && supportedApplicationTypes.Contains(appType, StringComparer.OrdinalIgnoreCase);
        }
    }
}
