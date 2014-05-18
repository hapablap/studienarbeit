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

        
        public Sample FindNextGoodSample(Sample startSample)
        {

            int i = startSample.index;

            long timestampWanted = (long) (startSample.timestamp + (1000000 / sampleRate));
            long bestDifferenceToWantedTimestamp = startSample.timestamp;
            Sample bestSample = null;

            // Manchmal häufen sich Timestamps mit demselben Wert in der Datei. Diese überspringen
            Sample nextSample = GetSample(i + 1);
            while (nextSample != null && nextSample.timestamp == startSample.timestamp)
            {
                i++;
                nextSample = GetSample(i + 1);
            }

            for (int j = i + 1; j < (i + 5); j++)
            {
                nextSample = GetSample(j);
                if (nextSample == null) return null;
                long differenceToWantedTimestamp = Math.Abs(timestampWanted - nextSample.timestamp);

                if (differenceToWantedTimestamp < bestDifferenceToWantedTimestamp)
                {
                    bestDifferenceToWantedTimestamp = differenceToWantedTimestamp;
                    bestSample = nextSample;
                }
            }

            Console.WriteLine(nextSample.timestamp);

            return bestSample;

        }

        public Sample GetSample(int i)
        {
            if (i >= csvData.Count) return null;
            List<String> row = csvData[i];
            Sample sample = new Sample();
            sample.index = i;
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
            Sample sample = GetSample(0);
            sampleList.Add(sample);
            while ((sample = FindNextGoodSample(sample)) != null)
            {
                sampleList.Add(sample);
            }
            return sampleList;
        }

    }
}
