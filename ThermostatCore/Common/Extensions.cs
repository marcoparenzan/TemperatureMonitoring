using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace ThermostatCore.Common
{
    public static class Extensions
    {
        public static void Sort<T>(this ObservableCollection<T> collection, Func<T, T, int> comparer
            )
        {
            int i, j;
            T index;

            for (i = 1; i < collection.Count; i++)
            {
                index = collection[i]; //If you can't read it, it should be index = this[x], where x is i :-)
                j = i;

                while ((j > 0) && (comparer(collection[j - 1], index) == 1))
                {
                    collection[j] = collection[j - 1];

                    j = j - 1;
                }

                collection[j] = index;
            }
        }
    }
}
