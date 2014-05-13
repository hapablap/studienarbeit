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
        public ObservableCollection<KeyValuePair<long, long>> ValueList { get; private set; }

        public ChartInput()
        {
            this.ValueList = new ObservableCollection<KeyValuePair<long, long>>();
        }

        public void Add(KeyValuePair<long, long> data)
        {
            ValueList.Add(data);
        }
    }
}
