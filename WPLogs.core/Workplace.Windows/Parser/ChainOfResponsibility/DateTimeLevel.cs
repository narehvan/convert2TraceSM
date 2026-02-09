using System.Text.RegularExpressions;

namespace WPLogs.core.Workplace.Windows.Parser.ChainOfResponsibility
{
    public partial class DateTimeLevel : SingleLineParser
    {
        const string pattern = @"^(?<year>\d{4})-(?<month>\d{2})-(?<day>\d{2})\s(?<hour>\d{2}):(?<minute>\d{2}):(?<second>\d{2}).(?<milisecond>\d{3})\s(?<level>\w{1})\s+(?<body>.+)";

        public override WindowsClientLogItem ParseLine(string line)
        {
            WindowsClientLogItem? result = null;

            Match match = MyRegex().Match(line);
            if (match.Success)
            {
                result = SaveMatchedData(match);
            }
            else
            {
                if (_successor != null)
                {
                    result = _successor.ParseLine(line);
                }
                else
                {
                    throw new FormatException("Unable to parse exception");
                }
            }

            return result;
        }

        [GeneratedRegex(pattern)]
        private static partial Regex MyRegex();
    }
}
