using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace DataRelational.WpfTestApp.Helpers
{
    public static class ObservableCollectionExtension
    {
        public static void ForEach<T>(this ObservableCollection<T> list, Action<T> Action)
        {
            list.ToList().ForEach(Action);
        }

        public static void AddRange<T>(this ObservableCollection<T> list, IEnumerable<T> range)
        {
            foreach (T item in range)
            {
                list.Add(item);
            }
        }
    }
}
