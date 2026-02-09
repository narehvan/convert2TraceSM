using System.Collections.ObjectModel;
using System.Text;
using WPLogs.core.Exceptions;
using WPLogs.core.Services;
using WPLogs.core.SIP;
using WPLogs.core.Workplace.Windows.Parser.ChainOfResponsibility;

namespace WPLogs.core.Workplace.Windows
{
    public class WindowsClientLogProcessor : LogFileParser
    {
        private static readonly string RANDOMSTRING_PLACEHOLDER = (Guid.NewGuid()).ToString();

        private static readonly string SIP_MESSAGE_FILTER = @"CSDK::SIP";

        /// <summary>
        /// Collection that contains all the log enteries from this type of log. Each entry is a single log line
        /// if a log line contains multiple lines, then the multiple lines will be saved under one single log file 
        /// for example all the lines of a SIP message will be saved under one single log line
        /// </summary>
        internal Collection<WindowsClientLogItem> Enteries { get; } = [];

        /// <summary>
        /// The inputFileReader must contains an absolute path to a file
        /// </summary>
        /// <param name="inputFileReader"></param>
        [System.Diagnostics.CodeAnalysis.SetsRequiredMembersAttribute]
        public WindowsClientLogProcessor(IFileReader inputFileReader)
        {
            base.InputFileReader = inputFileReader;
        }

        /// <summary>
        /// Throws Exceptions:
        /// NoFileToParseException("There is no file specified. Unable to parse an empty filename")
        /// FileNotFoundException("Unable to find the specified file")
        /// </summary>
        public override void Parse()
        {
            if (base.InputFileReader == null)
            {
                throw new NoFileToParseException("There is no file specified. Unable to parse an empty filename");
            }

            if (base.InputFileReader.FileExists() == false)
            {
                throw new FileNotFoundException("Unable to find the specified file");
            }

            // setup chain of responsibility
            SingleLineParser chainOfResponsibility = CreaterChanOfResponsibility();

            int index = 1;
            WindowsClientLogItem logData;

            string? line;
            while ((line = InputFileReader!.ReadLine()) != null)
            {
                try
                {
                    logData = chainOfResponsibility.ParseLine(line);
                    logData.Index = index;
                    logData.Header = logData.Body;
                    AddToEnteries(logData);
                    index++;
                }
                catch (FormatException)
                {
                    if (index > 0)
                    {
                        if (Enteries.Count == 0)
                            UpdateEntry(0, line);
                        else
                            UpdateEntry(Enteries.Count - 1, line);
                    }
                    else
                    {
                        base.AddToUnparsed(line);
                    }
                }
            }
        }

        /// <summary>
        /// Returns a list of all the log enteries that are of type SIP message
        /// This method does not modify any data. 
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        private Collection<WindowsClientLogItem> GetAllSIPMessages()
        {
            Collection<WindowsClientLogItem> sipItems = [];
            foreach (WindowsClientLogItem item in Enteries)
            {
                if (item.SubProcess == SIP_MESSAGE_FILTER)
                {
                    sipItems.Add(item);
                }
            }
            return sipItems;
        }

        /// <summary>
        /// Extracrs all the SIP messages and converts them to data that can be viewed in traceSM
        /// modification to the format of the data is important to make it readable by traceSM
        /// The actual data itself (i.e., the contents of the SIP message) is not modified
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public StringBuilder ConvertToTraceSMSIP()
        {
            StringBuilder returnStringBuilder = new();

            /*
             * ### traceSM name_ip: 1.2.3.4->CM;1.2.3.5->AMM;1.2.3.6->SM100;
             * --------------------
             * com.avaya.asm  SIPMSGT
             * --------------------
             * 02/01/2019 13:21:18.856 <--
             * octets: , Body Length:
             * ingress: [NO TARGET]
             * egress: { L10.116.68.20:5061/R10.116.67.37:49591/TLS/0xa }
             * --------------------
             * SIP/2.0 100 Trying
             * --------------------
             * 
             * Format of the month is day/month/year
             * 
             * SENDING 1800 bytes to 192.152.66.100:5061 { 
             * RECEIVED 412 bytes from 192.152.66.100:5061 {
             */

            string tabChar = "\t";
            string messageBorder = "--------------------";
            string messageBorderText = "com.avaya.asm  SIPMSGT "; // the space at the end is very important for traceSM to detect the file as asset file
            string incomingMessageSign = "--> "; // the space at the end is very important for traceSM to detect the file as asset file
            string outgoingMessageSign = "<-- "; // the space at the end is very important for traceSM to detect the file as asset file
            string bodyLenthString = tabChar + "octets: , Body Length:";
            string noTargetMessage = "[NO TARGET]";
            string myIP = "0.0.0.42"; // this is the IP of the device where Workplace was runing when logs were captured. at the moment, I stick to a dummy IP
            string myPort = "9999"; // and a dummy port. this info can be seen in the SIP trace itself

            Collection<string> listOfFarEndIPs = [];
            Collection<WindowsClientLogItem> allSIPmessages = GetAllSIPMessages();
            SIPCallsDictionary sipDictionary = SIPMessageLogEntry.BuildDictionary(allSIPmessages);

            if (allSIPmessages.Count > 0)
            {
                returnStringBuilder.AppendLine(RANDOMSTRING_PLACEHOLDER);
                foreach (WindowsClientLogItem? item in allSIPmessages)
                {
                    string dateTimeDirectionMessage;
                    string messageBody = GetMessageBody(item.Body);
                    string logEntryHeader = item.Header ?? string.Empty;
                    string ingressMessage = "ingress: ";
                    string egressMessage = "egress: ";
                    string messageSign;

                    string dateString = item.DateAndTime.Day.ToString("00") + "/" + item.DateAndTime.Month.ToString("00") + "/" + item.DateAndTime.Year;
                    string timeString = item.Time;
                    string farendIP = SIPMessageLogEntry.GetFarEndIP(logEntryHeader);
                    string farendIPAndPort = farendIP + ':' + SIPMessageLogEntry.GetFarEndPort(logEntryHeader);


                    if (item.Body.Contains("RECEIVED"))
                    {
                        // incoming message
                        messageSign = incomingMessageSign;
                        egressMessage = tabChar + egressMessage + noTargetMessage;
                        ingressMessage = tabChar + ingressMessage + "{ L" + myIP + ":" + myPort + "/R" + farendIPAndPort + "/TLS/0xa }";
                    }
                    else if (item.Body.Contains("SENDING"))
                    {
                        // outgoing message
                        messageSign = outgoingMessageSign;
                        ingressMessage = tabChar + ingressMessage + noTargetMessage;
                        egressMessage = tabChar + egressMessage + "{ L" + myIP + ":" + myPort + "/R" + farendIPAndPort + "/TLS/0xa }";
                    }
                    else
                    {
                        continue; // skip as this is neither a SENDING, nor a RECEIVED message. makes no sense to continue
                    }

                    dateTimeDirectionMessage = dateString + " " + timeString + " " + messageSign;

                    bool foundIP = false;
                    foreach (string ip in listOfFarEndIPs)
                    {
                        if (ip == farendIP)
                        {
                            foundIP = true;
                            break;
                        }
                    }
                    if (!foundIP)
                        listOfFarEndIPs.Add(farendIP);


                    returnStringBuilder.AppendLine(messageBorderText);
                    returnStringBuilder.AppendLine(messageBorder);
                    returnStringBuilder.AppendLine(dateTimeDirectionMessage);
                    returnStringBuilder.AppendLine(bodyLenthString);
                    returnStringBuilder.AppendLine(ingressMessage);
                    returnStringBuilder.AppendLine(egressMessage);
                    returnStringBuilder.AppendLine(messageBorder);

                    // delete the empty lines before the SIP message because traceSM doesn't like it
                    string[] bodies = messageBody.Split('\n');
                    bool firstTextFound = false;
                    foreach (string l in bodies)
                    {
                        // ignore the lines in the beginning of the message that are empty
                        if (firstTextFound == false)
                        {
                            if (!String.IsNullOrWhiteSpace(l))
                            {
                                firstTextFound = true;
                                returnStringBuilder.AppendLine(l);
                            }
                        }
                        else
                        {
                            returnStringBuilder.AppendLine(l);
                        }
                    }

                    returnStringBuilder.AppendLine(item.Header);
                    returnStringBuilder.AppendLine(messageBorder);

                }

                // build the traceSM name/IP line at the beginning of a traceSM file, something like the one below
                // ### traceSM name_ip: 1.2.3.4->CM;1.2.3.5->AMM;1.2.3.6->SM100;
                string iPList = BuildTraceSMIPNameList(listOfFarEndIPs, myIP);
                returnStringBuilder.Replace(RANDOMSTRING_PLACEHOLDER, iPList);
            }

            return returnStringBuilder;
        }

        private static string GetMessageBody(string body)
        {
            //some traces don't have the } closing. it has :"too large log size" instead because the message is too long to fit and it gets cut in the logs

            try
            {
                if (body.Contains("too large log size"))
                {
                    return body.Substring(body.IndexOf('{') + 1, body.Length);
                }
                else
                {
                    return body.Substring(body.IndexOf('{') + 1, (body.LastIndexOf('}') - body.IndexOf('{')) - 1);
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private void AddToEnteries(WindowsClientLogItem item)
        {
            Enteries.Add(item);
        }

        /// <summary>
        /// During parsing, when the line reader encounters a new line which is part of a collection of lines belonging to a single
        /// log line entery (for example a SIP message), then the line is appended to the last log line to make it part of that log entry
        /// </summary>
        /// <param name="index"></param>
        /// <param name="line"></param>
        private void UpdateEntry(int index, string line)
        {
            // if the list is empty, don't do anything
//            if (index > Enteries.Count || index < Enteries.Count || index == 0)
//                return;

            if (Enteries.Count == 0)
                return;

            string? body = Enteries[index].Body;
            body = body + "\n" + line;
            Enteries[index].Body = body;
        }

        private static SingleLineParser CreaterChanOfResponsibility()
        {
            // setup chain of responsibility
            SingleLineParser dateTimeLevelProcessSubprocess = new DateTimeLevelProcessSubprocess();
            SingleLineParser dateTimeLevelProcess = new DateTimeLevelProcess();
            SingleLineParser dateTimeLevel = new DateTimeLevel();

            dateTimeLevelProcessSubprocess.SetSucessor(dateTimeLevelProcess);
            dateTimeLevelProcess.SetSucessor(dateTimeLevel);

            return dateTimeLevelProcessSubprocess;
        }

        /// <summary>
        /// Builds a string line of the type: ### traceSM name_ip: 1.2.3.4->CM;1.2.3.5->AMM;1.2.3.6->SM100;
        /// This line is then placed at the top of the file to be fed into traceSM
        /// </summary>
        /// <param name="listOfIPs"></param>
        /// <param name="myIP"></param>
        /// <returns></returns>
        private static string BuildTraceSMIPNameList(Collection<string> listOfIPs, string myIP)
        {
            string IPList = "### traceSM name_ip: " + myIP + "->Client;";
            string destString = "->destination_";
            string list = string.Empty;
            int index = 1;
            foreach (string ip in listOfIPs)
            {
                list = list + ip + destString + index.ToString() + ";";
                index++;
            }
            IPList += list;

            return IPList;
        }
    }
}
