using BasicDSP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EEGETAnalysis.Library
{
    public class EEGAnalyzer
    {

        Waveform waveform;

        public EEGAnalyzer(Waveform waveform)
        {
            this.waveform = waveform;
        }


        public Waveform filterBeta()
        {
            LTISystem filterSystem = Filter.NRBandPass(12.5, 30, 30);
            return filterSystem.Filter(ref this.waveform);
        }

        public Waveform filterAlpha()
        {
            LTISystem filterSystem = Filter.NRBandPass(7.5, 12.5, 30);
            return filterSystem.Filter(ref this.waveform);
        }

        public Waveform filterTheta()
        {
            LTISystem filterSystem = Filter.NRBandPass(3.5, 7.5, 30);
            return filterSystem.Filter(ref this.waveform);
        }

        public Waveform filterDelta()
        {
            LTISystem filterSystem = Filter.NRBandPass(0.5, 3.5, 30);
            return filterSystem.Filter(ref this.waveform);
        }

    }
}
