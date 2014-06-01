using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EEGETAnalysis.Library
{
    public enum EEGBand
    {
        ALPHA,
        BETA,
        THETA,
        DELTA
    }

    public static class EEGBandMethods
    {
        public static double GetMinFreq(this EEGBand eegBand)
        {
            switch (eegBand)
            {
                case EEGBand.ALPHA:
                    return 7.5;
                case EEGBand.BETA:
                    return 12.5;
                case EEGBand.DELTA:
                    return 0.5;
                case EEGBand.THETA:
                    return 3.5;
                default:
                    return 0;
            }
        }

        public static double GetMaxFreq(this EEGBand eegBand)
        {
            switch (eegBand)
            {
                case EEGBand.ALPHA:
                    return 12.5;
                case EEGBand.BETA:
                    return 30;
                case EEGBand.DELTA:
                    return 3.5;
                case EEGBand.THETA:
                    return 7.5;
                default:
                    return 0;
            }
        }
    }
}
