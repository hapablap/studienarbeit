using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EEGETAnalysis.Library
{
    public class Sampler
    {
        List<List<String>> csvData;
        int sampleRate;

        int timestampPosition = 0;
        int t7ColumnNo = 0;
        int t8ColumnNo = 0;
        int eyeXColumnNo = 0;
        int eyeYColumnNo = 0;

        public Sampler(List<List<String>> csvData, int sampleRate)
        {
            this.csvData = csvData;
            this.sampleRate = sampleRate;
            
            // Find column numbers of relevant data
            foreach (List<String> row in csvData)
            {
                int i = 0;
                foreach (String content in row)
                {
                    if (content == "Time") { timestampPosition = i; }
                    else if (content == "EEG_RAW_T7") { t7ColumnNo = i; }
                    else if (content == "EEG_RAW_T8") { t8ColumnNo = i; }
                    else if (content == "L POR X [px]") { eyeXColumnNo = i; }
                    else if (content == "L POR Y [px]") { eyeYColumnNo = i; }
                    i++;
                }
                if (i > 0) break;
            }
        }

        public Sample findNextGoodSample(int i)
        {
            Sample thisSample = getSample(i);

            Sample bestSample = null;
            long difference = thisSample.timestamp;

            for (int j = 1; j < 4; j++)
            {
                Sample anotherSample = getSample(i + j);
                long anotherDifference = Math.Abs(anotherSample.timestamp - thisSample.timestamp - (1 / sampleRate * 1000000));
                if (anotherDifference < difference)
                {
                    bestSample = anotherSample;
                    difference = anotherDifference;
                }
            }

            return bestSample;
        }

        public Sample getSample(int i)
        {
            List<String> row = csvData[i];
            Sample sample = new Sample();
            sample.timestamp = Convert.ToInt64(row[timestampPosition]);
            sample.T7 = Convert.ToInt64(row[t7ColumnNo]);
            sample.T8 = Convert.ToInt64(row[t8ColumnNo]);
            sample.eyeX = Convert.ToDouble(row[eyeXColumnNo]);
            sample.eyeY = Convert.ToDouble(row[eyeYColumnNo]);

            return sample;
        }
    }
}
