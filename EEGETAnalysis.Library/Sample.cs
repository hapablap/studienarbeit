using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EEGETAnalysis.Library
{
    public class EEGSample
    {
        public int index;
        public long timestamp;
        public double eyeX;
        public double eyeY;
        public Dictionary<Electrode, double> eegValues = new Dictionary<Electrode,double>();
    }
}
