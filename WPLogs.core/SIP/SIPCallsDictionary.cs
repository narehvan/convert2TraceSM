using WPLogs.core.Workplace.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPLogs.core.SIP
{
    internal class SIPCallsDictionary
    {
        private readonly Dictionary<string, SIPCallMessageCollection> _dictionary = [];

        internal SIPCallsDictionary()
        {

        }

        internal void Add(string sipCallID, WindowsClientLogItem sipMessage)
        {
            if (_dictionary.TryGetValue(sipCallID, out SIPCallMessageCollection? value))
            {
                value.Add(sipMessage);
            }
            else
            {
                _dictionary.Add(sipCallID, new SIPCallMessageCollection(sipMessage));
            }
        }
    }
}
