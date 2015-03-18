using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using MvvmFoundation.Wpf;
using DataRelational;


namespace DataRelational.WpfTestApp.ViewModels
{
    public class LoggingViewModel : Base 
    {
        private ObservableCollection<string> _LogEntries;

        public LoggingViewModel()
        {
            _LogEntries = new ObservableCollection<string>();
            Logging.AddLoggingAction(log => _LogEntries.Add(log));

            if (IsDesignMode)
            {
                _LogEntries.Add("Warning: blah blah blah");
                _LogEntries.Add("Error: blah blah blah 2");
            }
        }

        public ObservableCollection<string> LogEntries
        {
            get { return _LogEntries; }
        }
    }
}
