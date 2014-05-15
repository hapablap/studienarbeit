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

            csvData.RemoveRange(0, 2);
            csvData.RemoveAt(csvData.Count - 1);
        }

        public Sample FindNextGoodSample(int i)
        {

            if (i > csvData.Count - 4) return null;

            Sample thisSample = GetSample(i);

            Sample bestSample = null;
            long difference = thisSample.timestamp;

            for (int j = 1; j < 4; j++)
            {
                Sample anotherSample = GetSample(i + j);
                long anotherDifference = Math.Abs(anotherSample.timestamp - thisSample.timestamp - (1 / sampleRate * 1000000));
                if (anotherDifference < difference)
                {
                    bestSample = anotherSample;
                    difference = anotherDifference;
                }
            }

            return bestSample;
        }

        public Sample GetSample(int i)
        {
            List<String> row = csvData[i];
            Sample sample = new Sample();
            sample.timestamp = Convert.ToInt64(row[timestampPosition]);
            sample.T7 = Convert.ToDouble(row[t7ColumnNo], System.Globalization.CultureInfo.InvariantCulture);
            sample.T8 = Convert.ToDouble(row[t8ColumnNo], System.Globalization.CultureInfo.InvariantCulture);
            sample.eyeX = Convert.ToDouble(row[eyeXColumnNo], System.Globalization.CultureInfo.InvariantCulture);
            sample.eyeY = Convert.ToDouble(row[eyeYColumnNo], System.Globalization.CultureInfo.InvariantCulture);

            return sample;
        }

        public List<Sample> FindAllGoodSamples()
        {
            List<Sample> sampleList = new List<Sample>();
            sampleList.Add(GetSample(0));
            int i = 0;
            Sample sample;
            while ((sample = FindNextGoodSample(i)) != null)
            {
                sampleList.Add(sample);
                i++;
            }
            return sampleList;
        }

    }
}
