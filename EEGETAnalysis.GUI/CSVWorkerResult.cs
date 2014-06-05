using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EEGETAnalysis.GUI
{
    public class CSVWorkerResult
    {
        public string ErrorMessage { get; set; }
        public bool Success { get; set; }
        public EEGETAnalysis.Library.Sampler Sampler { get; set; }
        public EEGETAnalysis.Library.EEGEmotionizer Emotionizer { get; set; }
    }
}
