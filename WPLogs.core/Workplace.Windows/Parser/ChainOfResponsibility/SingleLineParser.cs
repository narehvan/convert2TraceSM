using System.Text.RegularExpressions;

namespace WPLogs.core.Workplace.Windows.Parser.ChainOfResponsibility
{
    public abstract class SingleLineParser
    {
        protected SingleLineParser? _successor;
        public void SetSucessor(SingleLineParser successor)
        {
            this._successor = successor;
        }

        public static WindowsClientLogItem SaveMatchedData(Match match)
        {
            WindowsClientLogItem result = new();

            string date = string.Format("{0}-{1}-{2}",
                       match.Groups["year"].Value,
                       match.Groups["month"].Value,
                       match.Groups["day"].Value);
            result.Date = date;

            string time = string.Format("{0}:{1}:{2}.{3}",
                       match.Groups["hour"].Value,
                       match.Groups["minute"].Value,
                       match.Groups["second"].Value,
                       match.Groups["milisecond"].Value);
            result.Time = time;

            result.DateAndTime = new DateTime(Int32.Parse(match.Groups["year"].Value),
                Int32.Parse(match.Groups["month"].Value),
                Int32.Parse(match.Groups["day"].Value),
                Int32.Parse(match.Groups["hour"].Value),
                Int32.Parse(match.Groups["minute"].Value),
                Int32.Parse(match.Groups["second"].Value),
                Int32.Parse(match.Groups["milisecond"].Value));

            result.Level = match.Groups["level"].Value;
            result.Process = match.Groups["process"].Value;
            result.SubProcess = match.Groups["subprocess"].Value;
            result.Body = match.Groups["body"].Value;

            return result;
        }

        public abstract WindowsClientLogItem ParseLine(string line);
    }
}
