using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EEGETAnalysis.GUI
{
    public class ChartInput
    {
        public ObservableCollection<KeyValuePair<double, double>> ValueList { get; private set; }

        public ChartInput()
        {
            this.ValueList = new ObservableCollection<KeyValuePair<double, double>>();
        }

        public void Add(KeyValuePair<double, double> data)
        {
            ValueList.Add(data);
        }

        public void Clear()
        {
            ValueList.Clear();
        }
    }
}
