using WPLogs.core.Workplace.Windows;
using System.Collections.ObjectModel;
using System.Net;

namespace WPLogs.core.SIP
{
    internal static class SIPMessageLogEntry
    {
        private static readonly string DEFAULT_IP = "0.0.0.0";
        private static readonly string DEFAULT_PORT = "9999";
        private static readonly string NewLineCharacterSet = "\n";

        #region Internal methods

        internal static string GetFarEndIP(string logEntry)
        {
            string farendIPPort = string.Empty;

            if (string.IsNullOrEmpty(logEntry))
                return farendIPPort;
            if (logEntry.Contains("RECEIVED"))
                farendIPPort = GetFarendIPAndPortReceived(logEntry);
            else if (logEntry.Contains("SENDING"))
                farendIPPort = GetFarendIPAndPortSending(logEntry);
            if (farendIPPort == string.Empty)
                return DEFAULT_IP;

            return GetIPFromIPAndPort(farendIPPort);
        }

        internal static string GetFarEndPort(string logEntry)
        {
            string farendPort = string.Empty;

            if (string.IsNullOrEmpty(logEntry))
                return farendPort;
            if (logEntry.Contains("RECEIVED"))
                farendPort = GetFarendIPAndPortReceived(logEntry);
            else if (logEntry.Contains("SENDING"))
                farendPort = GetFarendIPAndPortSending(logEntry);
            if (farendPort == string.Empty)
                return DEFAULT_PORT;

            return GetPortFromIPAndPort(farendPort);
        }

        internal static string GetSIPCallID(string logEntry)
        {
            string callID;
            string keyword = "Call-ID";
            string extraTextAfterKeyword = ": ";

            if (string.IsNullOrEmpty(logEntry))
                return string.Empty;

            if (!logEntry.Contains(keyword))
                return string.Empty;

            int keywordIndex = logEntry.IndexOf(keyword);
            int indexOfFirstEndOfLine = logEntry.IndexOf(NewLineCharacterSet, keywordIndex);
            int startingPosition = keywordIndex + keyword.Length + extraTextAfterKeyword.Length;

            try
            {
                callID = logEntry.Substring(startingPosition, indexOfFirstEndOfLine - startingPosition);
            }
            catch (Exception)
            {
                return string.Empty;
            }

            return callID;
        }

        internal static SIPCallsDictionary BuildDictionary(Collection<WindowsClientLogItem> items)
        {
            SIPCallsDictionary sipDictionary = new();

            if (items != null)
            {
                foreach (WindowsClientLogItem item in items)
                {
                    string callID = GetSIPCallID(item.Body);
                    sipDictionary.Add(callID, item);
                }
            }

            return sipDictionary;
        }

        #endregion Internal Methods

        #region Private methods
        private static string GetIPFromIPAndPort(string ipAndPort)
        {
            string ip = DEFAULT_IP;

            if (!string.IsNullOrEmpty(ipAndPort))
            {
                try
                {
                    int lastIndexOfColon = ipAndPort.LastIndexOf(':');
                    if (lastIndexOfColon > 0)
                    {
                        ip = ipAndPort.Substring(0, lastIndexOfColon);
                    }
                }
                catch (Exception)
                {
                    ip = DEFAULT_IP;
                }

                if (!IPAddress.TryParse(ip, out var _))
                {
                    ip = DEFAULT_IP;
                }
            }
            return ip;
        }

        private static string GetPortFromIPAndPort(string ipAndPort)
        {
            string port = DEFAULT_PORT;

            if (!string.IsNullOrEmpty(ipAndPort))
            {
                try
                {
                    int lastIndexOfColon = ipAndPort.LastIndexOf(':');
                    if (lastIndexOfColon > 0)
                    {
                        port = ipAndPort.Substring(lastIndexOfColon + 1, ipAndPort.Length - lastIndexOfColon - 1);
                    }
                }
                catch (Exception)
                {
                    port = DEFAULT_PORT;
                }

                if (!int.TryParse(port, out var _))
                {
                    port = DEFAULT_PORT;
                }
            }
            return port;
        }

        private static string GetFarendIPAndPortReceived(string logEntry)
        {
            // 2024-10-24 18:39:57.630 D [ 6872] [CSDK::SIP] RECEIVED 609 bytes from 2ccc:2aaa:2bbb:161::44:5061 {
            // 2024-11-07 11:04:21.653 D [44492] [CSDK::SIP] RECEIVED 1870 bytes from 10.11.122.123:5061 {
            string farendIPPort = string.Empty;
            string keyword = "bytes from ";
            try
            {
                if (logEntry.Contains(keyword))
                {
                    int keywordIndex = logEntry.IndexOf(keyword);
                    return logEntry.Substring(keywordIndex + 11, (logEntry.IndexOf('{') - keywordIndex) - 12);
                }
                else
                {
                    return farendIPPort;
                }
            }
            catch (Exception)
            {
                return farendIPPort;
            }
        }

        private static string GetFarendIPAndPortSending(string logEntry)
        {
            // 2024-10-24 18:39:57.622 D [ 6872] [CSDK::SIP] SENDING 868 bytes to 2ccc:2aaa:2bbb:161::44:5061 { 
            // 2024-11-07 11:04:21.653 D [44492] [CSDK::SIP] SENDING 581 bytes to 10.11.122.123:5061 {
            string farendIPPort = string.Empty;
            string keyword = "bytes to ";
            try
            {
                if (logEntry.Contains(keyword))
                {
                    int keywordIndex = logEntry.IndexOf(keyword);
                    return logEntry.Substring(keywordIndex + 9, (logEntry.IndexOf('{') - keywordIndex) - 10);
                }
                else
                {
                    return farendIPPort;
                }
            }
            catch (Exception)
            {
                return farendIPPort;
            }
        }

        private static string GetMessageBody(string logEntry)
        {
            //some traces don't have the } closing. it has :"too large log size" instead because the message is too long to fit and it gets cut in the logs

            if (string.IsNullOrEmpty(logEntry))
                return string.Empty;

            try
            {
                if (logEntry.Contains("too large log size"))
                {
                    return logEntry.Substring(logEntry.IndexOf('{') + 1, logEntry.Length);
                }
                else
                {
                    return logEntry.Substring(logEntry.IndexOf('{') + 1, (logEntry.LastIndexOf('}') - logEntry.IndexOf('{')) - 1);
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        #endregion Private methods
    }
}
