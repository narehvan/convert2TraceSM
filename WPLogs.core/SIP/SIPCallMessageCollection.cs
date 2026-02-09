using WPLogs.core.Workplace.Windows;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Transactions;

namespace WPLogs.core.SIP
{
    internal class SIPCallMessageCollection
    {
        private readonly Collection<WindowsClientLogItem> _collection = [];

        internal SIPCallMessageCollection() 
        { 
        }

        internal SIPCallMessageCollection(WindowsClientLogItem sipMessage)
        {
            Add(sipMessage);
        }

        internal void Add(WindowsClientLogItem sipMessage)
        {
            _collection.Add(sipMessage);
        }

    }
}