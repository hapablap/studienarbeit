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
        int sampleRate;

        public EEGAnalyzer(Waveform waveform, int sampleRate)
        {
            this.waveform = waveform;
            this.sampleRate = sampleRate;
        }


        public Waveform filterBeta()
        {
            LTISystem filterSystem = Filter.NRBandPass(12.5 / sampleRate, 30 / sampleRate, 3);
            return filterSystem.Filter(ref this.waveform);
        }

        public Waveform filterAlpha()
        {
            LTISystem filterSystem = Filter.NRBandPass(7.5 / sampleRate, 12.5 / sampleRate, 3);
            return filterSystem.Filter(ref this.waveform);
        }

        public Waveform filterTheta()
        {
            LTISystem filterSystem = Filter.NRBandPass(3.5 / sampleRate, 7.5 / sampleRate, 3);
            return filterSystem.Filter(ref this.waveform);
        }

        public Waveform filterDelta()
        {
            LTISystem filterSystem = Filter.NRBandPass(0.5 / sampleRate, 3.5 / sampleRate, 3);
            return filterSystem.Filter(ref this.waveform);
        }

    }
}
