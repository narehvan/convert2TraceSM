using System.Text;
using UnitTestProject.Mockers;
using WPLogs.core.Workplace.Windows;

namespace UnitTestProject.WPLogs.core_Tests
{
    public class UnitTest_SIP_Messages
    {
        [Fact]
        public void TestMethod_REGISTER_SENDING_IPv4_Success()
        {
            string inputFilename = @"WPLogs.core_Tests.SampleData.SingleSIPMessage_REGISTER.txt";
            string outputFilename = @"WPLogs.core_Tests.SampleData.MockFile.txt";
            string expetedFilename = @"WPLogs.core_Tests.SampleData.SingleSIPMessage_REGISTER_converted.txt";
            
            MockFileReader reader = new(inputFilename);
            WindowsClientLogProcessor logWin = new(reader);
            logWin.Parse();
            StringBuilder sipItems = logWin.ConvertToTraceSMSIP();

            MockSingleFileWriter writer = new(outputFilename);
            writer.StartWriting();
            writer.NewLineCharacterSet = "\n";
            writer.WriteToFile(sipItems.ToString());
            writer.StopWriting();

            MockFileReader expeted = new(expetedFilename);

            Assert.Equal(expeted.GetContent(), writer.GetContent());
        }

        [Fact]
        public void TestMethod_200OK_RECEIVED_IPv4_Success()
        {
            string inputFilename = @"WPLogs.core_Tests.SampleData.SingleSIPMessage_200OK.txt";
            string outputFilename = @"WPLogs.core_Tests.SampleData.MockFile.txt";
            string expetedFilename = @"WPLogs.core_Tests.SampleData.SingleSIPMessage_200OK_converted.txt";

            MockFileReader reader = new(inputFilename);
            WindowsClientLogProcessor logWin = new(reader);
            logWin.Parse();
            StringBuilder sipItems = logWin.ConvertToTraceSMSIP();

            MockSingleFileWriter writer = new(outputFilename);
            writer.StartWriting();
            writer.NewLineCharacterSet = "\n";
            writer.WriteToFile(sipItems.ToString());
            writer.StopWriting();

            MockFileReader expeted = new(expetedFilename);

            Assert.Equal(expeted.GetContent(), writer.GetContent());
        }

        [Fact]
        public void TestMethod_REGISTER_SENDING_With_logLines_IPv4_Success()
        {
            string inputFilename = @"WPLogs.core_Tests.SampleData.SingleSIPMessage_with_LogLines.txt";
            string outputFilename = @"WPLogs.core_Tests.SampleData.MockFile.txt";
            string expetedFilename = @"WPLogs.core_Tests.SampleData.SingleSIPMessage_REGISTER_converted.txt";

            MockFileReader reader = new(inputFilename);
            WindowsClientLogProcessor logWin = new(reader);
            logWin.Parse();
            StringBuilder sipItems = logWin.ConvertToTraceSMSIP();

            MockSingleFileWriter writer = new(outputFilename);
            writer.StartWriting();
            writer.NewLineCharacterSet = "\n";
            writer.WriteToFile(sipItems.ToString());
            writer.StopWriting();

            MockFileReader expeted = new(expetedFilename);

            Assert.Equal(expeted.GetContent(), writer.GetContent());
        }
    }
}
