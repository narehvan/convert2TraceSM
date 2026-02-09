using WPLogs.core.SIP;

namespace UnitTestProject.WPLogs.core_Tests
{
    public class UnitTest_SIP_Log_header
    {
        [Fact]
        public void TestMethod_GetFarEndIP_RECEIVED_IPv6_Success()
        {
            string logEntry = @"2024-10-24 18:39:57.630 D [ 6872] [CSDK::SIP] RECEIVED 609 bytes from 2aaa:2bbb:2ccc:161::44:5061 {";
            string farendIP = SIPMessageLogEntry.GetFarEndIP(logEntry);
            string expectedFarendIP = "2aaa:2bbb:2ccc:161::44";
            Assert.Equal(expectedFarendIP, farendIP);
        }

        [Fact]
        public void TestMethod_GetFarEndIP_RECEIVED_IPv4_Success()
        {
            string logEntry = @"2024-11-07 11:04:21.653 D [44492] [CSDK::SIP] RECEIVED 1870 bytes from 10.11.122.123:5061 {";
            string farendIP = SIPMessageLogEntry.GetFarEndIP(logEntry);
            string expectedFarendIP = "10.11.122.123";
            Assert.Equal(expectedFarendIP, farendIP);
        }

        [Fact]
        public void TestMethod_GetFarEndIP_SENDING_IPv6_Success()
        {
            string logEntry = @"2024-10-24 18:39:57.622 D [ 6872] [CSDK::SIP] SENDING 868 bytes to 2aaa:2bbb:2ccc:161::44:5061 { ";
            string farendIP = SIPMessageLogEntry.GetFarEndIP(logEntry);
            string expectedFarendIP = "2aaa:2bbb:2ccc:161::44";
            Assert.Equal(expectedFarendIP, farendIP);
        }

        [Fact]
        public void TestMethod_GetFarEndIP_SENDING_IPv4_Success()
        {
            string logEntry = @"2024-11-07 11:04:21.653 D [44492] [CSDK::SIP] SENDING 581 bytes to 10.11.122.123:5061 {";
            string farendIP = SIPMessageLogEntry.GetFarEndIP(logEntry);
            string expectedFarendIP = "10.11.122.123";
            Assert.Equal(expectedFarendIP, farendIP);
        }

        [Fact]
        public void TestMethod_GetFarEndIP_RECEIVED_Fail_1_missing_open_braces()
        {
            string logEntry = @"2024-10-24 18:39:57.630 D [ 6872] [CSDK::SIP] RECEIVED 609 bytes from 2aaa:2bbb:2ccc:161::44:5061";
            string farendIP = SIPMessageLogEntry.GetFarEndIP(logEntry);
            string expectedFarendIP = "0.0.0.0";
            Assert.Equal(expectedFarendIP, farendIP);
        }

        [Fact]
        public void TestMethod_GetFarEndIP_RECEIVED_Fail_2_Midding_bytes_from_keyword()
        {
            string logEntry = @"2024-10-24 18:39:57.630 D [ 6872] [CSDK::SIP] RECEIVED 609 2aaa:2bbb:2ccc:161::44:5061 {";
            string farendIP = SIPMessageLogEntry.GetFarEndIP(logEntry);
            string expectedFarendIP = "0.0.0.0";
            Assert.Equal(expectedFarendIP, farendIP);
        }

        [Fact]
        public void TestMethod_GetFarEndIP_RECEIVED_Fail_3_Invalid_IPv6()
        {
            string logEntry = @"2024-10-24 18:39:57.630 D [ 6872] [CSDK::SIP] RECEIVED 609 bytes from af85:161:44:5061 {";
            string farendIP = SIPMessageLogEntry.GetFarEndIP(logEntry);
            string expectedFarendIP = "0.0.0.0";
            Assert.Equal(expectedFarendIP, farendIP);
        }

        [Fact]
        public void TestMethod_GetFarEndIP_RECEIVED_Fail_4_Invalid_IPv4()
        {
            string logEntry = @"2024-10-24 18:39:57.630 D [ 6872] [CSDK::SIP] RECEIVED 609 bytes from 192.267.122.21:5061 {";
            string farendIP = SIPMessageLogEntry.GetFarEndIP(logEntry);
            string expectedFarendIP = "0.0.0.0";
            Assert.Equal(expectedFarendIP, farendIP);
        }

        [Fact]
        public void TestMethod_GetFarEndPort_RECEIVED_IPv6_Success()
        {
            string logEntry = @"2024-10-24 18:39:57.630 D [ 6872] [CSDK::SIP] RECEIVED 609 bytes from 2aaa:2bbb:2ccc:161::44:5061 {";
            string farendPort = SIPMessageLogEntry.GetFarEndPort(logEntry);
            string expectedFarendPort = "5061";
            Assert.Equal(expectedFarendPort, farendPort);
        }

        [Fact]
        public void TestMethod_GetFarEndPort_RECEIVED_IPv4_Success()
        {
            string logEntry = @"2024-11-07 11:04:21.653 D [44492] [CSDK::SIP] RECEIVED 1870 bytes from 10.11.122.123:5062 {";
            string farendPort = SIPMessageLogEntry.GetFarEndPort(logEntry);
            string expectedFarendPort = "5062";
            Assert.Equal(expectedFarendPort, farendPort);
        }

        [Fact]
        public void TestMethod_GetFarEndPort_SENDING_IPv6_Success()
        {
            string logEntry = @"2024-10-24 18:39:57.622 D [ 6872] [CSDK::SIP] SENDING 868 bytes to 2aaa:2bbb:2ccc:161::44:5061 { ";
            string farendPort = SIPMessageLogEntry.GetFarEndPort(logEntry);
            string expectedFarendPort = "5061";
            Assert.Equal(expectedFarendPort, farendPort);
        }

        [Fact]
        public void TestMethod_GetFarEndPort_SENDING_IPv4_Success()
        {
            string logEntry = @"2024-11-07 11:04:21.653 D [44492] [CSDK::SIP] SENDING 581 bytes to 10.11.122.123:5062 {";
            string farendPort = SIPMessageLogEntry.GetFarEndPort(logEntry);
            string expectedFarendPort = "5062";
            Assert.Equal(expectedFarendPort, farendPort);
        }

        [Fact]
        public void TestMethod_GetFarEndPort_RECEIVED_Fail_1_IPv6_Missing_port()
        {
            string logEntry = @"2024-10-24 18:39:57.630 D [ 6872] [CSDK::SIP] RECEIVED 609 bytes from 2aaa:2bbb:2ccc:161::44: {";
            string farendPort = SIPMessageLogEntry.GetFarEndPort(logEntry);
            string expectedFarendPort = "9999";
            Assert.Equal(expectedFarendPort, farendPort);
        }

        [Fact]
        public void TestMethod_GetFarEndPort_RECEIVED_Fail_2_IPv6_Not_a_number()
        {
            string logEntry = @"2024-10-24 18:39:57.630 D [ 6872] [CSDK::SIP] RECEIVED 609 bytes from 2aaa:2bbb:2ccc:161::44:19KLA {";
            string farendPort = SIPMessageLogEntry.GetFarEndPort(logEntry);
            string expectedFarendPort = "9999";
            Assert.Equal(expectedFarendPort, farendPort);
        }

        [Fact]
        public void TestMethod_GetFarEndPort_RECEIVED_Fail_3_IPv6_missing_bytes_from_keyword()
        {
            string logEntry = @"2024-10-24 18:39:57.630 D [ 6872] [CSDK::SIP] RECEIVED 609 2aaa:2bbb:2ccc:161::44:5062 {";
            string farendPort = SIPMessageLogEntry.GetFarEndPort(logEntry);
            string expectedFarendPort = "9999";
            Assert.Equal(expectedFarendPort, farendPort);
        }

        [Fact]
        public void TestMethod_GetFarEndPort_SENDING_Fail_4_IPv6_missing_bytes_to_keyword()
        {
            string logEntry = @"2024-11-07 11:04:21.653 D [44492] [CSDK::SIP] SENDING 581 10.11.122.123:5061 {";
            string farendPort = SIPMessageLogEntry.GetFarEndPort(logEntry);
            string expectedFarendPort = "9999";
            Assert.Equal(expectedFarendPort, farendPort);
        }
    }
}
