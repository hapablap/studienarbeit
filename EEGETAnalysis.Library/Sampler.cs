using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BasicDSP;
using System.IO;

namespace EEGETAnalysis.Library
{
    public class Sampler
    {
        List<List<String>> csvData;
        int sampleRate;

        int timestampPosition = 0;
        int af3ColumnNo = 0;
        int f7ColumnNo = 0;
        int f3ColumnNo = 0;
        int fc5ColumnNo = 0;
        int t7ColumnNo = 0;
        int p7ColumnNo = 0;
        int o1ColumnNo = 0;
        int o2ColumnNo = 0;
        int p8ColumnNo = 0;
        int t8ColumnNo = 0;
        int fc6ColumnNo = 0;
        int f4ColumnNo = 0;
        int f8ColumnNo = 0;
        int af4ColumnNo = 0;
        int eyeXColumnNo = 0;
        int eyeYColumnNo = 0;

        List<Sample> sampleList;

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
                    else if (content == "EEG_RAW_AF3") { af3ColumnNo = i; }
                    else if (content == "EEG_RAW_F7") { f7ColumnNo = i; }
                    else if (content == "EEG_RAW_F3") { f3ColumnNo = i; }
                    else if (content == "EEG_RAW_FC5") { fc5ColumnNo = i; }
                    else if (content == "EEG_RAW_T7") { t7ColumnNo = i; }
                    else if (content == "EEG_RAW_P7") { p7ColumnNo = i; }
                    else if (content == "EEG_RAW_O1") { o1ColumnNo = i; }
                    else if (content == "EEG_RAW_O2") { o2ColumnNo = i; }
                    else if (content == "EEG_RAW_P8") { p8ColumnNo = i; }
                    else if (content == "EEG_RAW_T8") { t8ColumnNo = i; }
                    else if (content == "EEG_RAW_FC6") { fc6ColumnNo = i; }
                    else if (content == "EEG_RAW_F4") { f4ColumnNo = i; }
                    else if (content == "EEG_RAW_F8") { f8ColumnNo = i; }
                    else if (content == "EEG_RAW_AF4") { af4ColumnNo = i; }
                    else if (content == "L POR X [px]") { eyeXColumnNo = i; }
                    else if (content == "L POR Y [px]") { eyeYColumnNo = i; }
                    i++;
                }
                if (i > 0) break;
            }

            csvData.RemoveRange(0, 2);
            csvData.RemoveAt(csvData.Count - 1);

            findAllGoodSamples();
        }

        
        public Sample FindNextGoodSample(Sample startSample)
        {

            int i = startSample.index;

            long timestampWanted = (long) (startSample.timestamp + (1000000 / sampleRate));
            long bestDifferenceToWantedTimestamp = startSample.timestamp;
            Sample bestSample = null;

            // Überspringe aufeinanderfolgende Timestamps mit demselben Wert. Manchmal häufen die sich.
            Sample nextSample = GetSample(i + 1);
            while (nextSample != null && nextSample.timestamp == startSample.timestamp)
            {
                i++;
                nextSample = GetSample(i + 1);
            }

            // Suche den nächsten Timestamp, der (entsprechend der angegebenen Sample-Rate) dem erwarteten Wert am nächsten kommt
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

            return bestSample;

        }

        public Sample GetSample(int i)
        {
            if (i >= csvData.Count) return null;
            List<String> row = csvData[i];
            Sample sample = new Sample();
            sample.index = i;
            sample.timestamp = Convert.ToInt64(row[timestampPosition]);
            sample.AF3 = Convert.ToDouble(row[af3ColumnNo], System.Globalization.CultureInfo.InvariantCulture);
            sample.F7 = Convert.ToDouble(row[f7ColumnNo], System.Globalization.CultureInfo.InvariantCulture);
            sample.F3 = Convert.ToDouble(row[f3ColumnNo], System.Globalization.CultureInfo.InvariantCulture);
            sample.FC5 = Convert.ToDouble(row[fc5ColumnNo], System.Globalization.CultureInfo.InvariantCulture);
            sample.T7 = Convert.ToDouble(row[t7ColumnNo], System.Globalization.CultureInfo.InvariantCulture);
            sample.P7 = Convert.ToDouble(row[p7ColumnNo], System.Globalization.CultureInfo.InvariantCulture);
            sample.O1 = Convert.ToDouble(row[o1ColumnNo], System.Globalization.CultureInfo.InvariantCulture);
            sample.O2 = Convert.ToDouble(row[o2ColumnNo], System.Globalization.CultureInfo.InvariantCulture);
            sample.P8 = Convert.ToDouble(row[p8ColumnNo], System.Globalization.CultureInfo.InvariantCulture);
            sample.T8 = Convert.ToDouble(row[t8ColumnNo], System.Globalization.CultureInfo.InvariantCulture);
            sample.FC6 = Convert.ToDouble(row[fc6ColumnNo], System.Globalization.CultureInfo.InvariantCulture);
            sample.F4 = Convert.ToDouble(row[f4ColumnNo], System.Globalization.CultureInfo.InvariantCulture);
            sample.F8 = Convert.ToDouble(row[f8ColumnNo], System.Globalization.CultureInfo.InvariantCulture);
            sample.AF4 = Convert.ToDouble(row[af4ColumnNo], System.Globalization.CultureInfo.InvariantCulture);
            sample.eyeX = Convert.ToDouble(row[eyeXColumnNo], System.Globalization.CultureInfo.InvariantCulture);
            sample.eyeY = Convert.ToDouble(row[eyeYColumnNo], System.Globalization.CultureInfo.InvariantCulture);

            return sample;
        }

        public List<Sample> GetAllGoodSamples()
        {
            return sampleList;
        }

        private void findAllGoodSamples()
        {
            sampleList = new List<Sample>();
            Sample sample = GetSample(0);
            sampleList.Add(sample);
            while ((sample = FindNextGoodSample(sample)) != null)
            {
                sampleList.Add(sample);
            }
        }

        //public Waveform getEEGWaveform(int mode)
        //{
        //    Waveform Waveform = new Waveform(0, sampleRate);
        //    foreach (Sample sample in sampleList)
        //    {
        //        if (mode == 1)
        //        {
        //            Waveform.Add(sample.T7);
        //        }
        //        else
        //        {
        //            Waveform.Add(sample.T8);
        //        }
        //    }

        //    return Waveform;
        //}

        public Waveform GetEEGWaveformAF3()
        {
            Waveform Waveform = new Waveform(0, sampleRate);
            foreach (Sample sample in sampleList)
            {
                Waveform.Add(sample.AF3);
            }

            return Waveform;
        }

        public Waveform GetEEGWaveformF7()
        {
            Waveform Waveform = new Waveform(0, sampleRate);
            foreach (Sample sample in sampleList)
            {
                Waveform.Add(sample.F7);
            }

            return Waveform;
        }

        public Waveform GetEEGWaveformF3()
        {
            Waveform Waveform = new Waveform(0, sampleRate);
            foreach (Sample sample in sampleList)
            {
                Waveform.Add(sample.F3);
            }

            return Waveform;
        }

        public Waveform GetEEGWaveformFC5()
        {
            Waveform Waveform = new Waveform(0, sampleRate);
            foreach (Sample sample in sampleList)
            {
                Waveform.Add(sample.FC5);
            }

            return Waveform;
        }

        public Waveform GetEEGWaveformT7()
        {
            Waveform Waveform = new Waveform(0, sampleRate);
            foreach (Sample sample in sampleList)
            {
                Waveform.Add(sample.T7);
            }

            return Waveform;
        }

        public Waveform GetEEGWaveformP7()
        {
            Waveform Waveform = new Waveform(0, sampleRate);
            foreach (Sample sample in sampleList)
            {
                Waveform.Add(sample.P7);
            }

            return Waveform;
        }

        public Waveform GetEEGWaveformO1()
        {
            Waveform Waveform = new Waveform(0, sampleRate);
            foreach (Sample sample in sampleList)
            {
                Waveform.Add(sample.O1);
            }

            return Waveform;
        }

        public Waveform GetEEGWaveformO2()
        {
            Waveform Waveform = new Waveform(0, sampleRate);
            foreach (Sample sample in sampleList)
            {
                Waveform.Add(sample.O2);
            }

            return Waveform;
        }

        public Waveform GetEEGWaveformP8()
        {
            Waveform Waveform = new Waveform(0, sampleRate);
            foreach (Sample sample in sampleList)
            {
                Waveform.Add(sample.P8);
            }

            return Waveform;
        }

        public Waveform GetEEGWaveformT8()
        {
            Waveform Waveform = new Waveform(0, sampleRate);
            foreach (Sample sample in sampleList)
            {
                Waveform.Add(sample.T8);
            }

            return Waveform;
        }

        public Waveform GetEEGWaveformFC6()
        {
            Waveform Waveform = new Waveform(0, sampleRate);
            foreach (Sample sample in sampleList)
            {
                Waveform.Add(sample.FC6);
            }

            return Waveform;
        }

        public Waveform GetEEGWaveformF4()
        {
            Waveform Waveform = new Waveform(0, sampleRate);
            foreach (Sample sample in sampleList)
            {
                Waveform.Add(sample.F4);
            }

            return Waveform;
        }

        public Waveform GetEEGWaveformF8()
        {
            Waveform Waveform = new Waveform(0, sampleRate);
            foreach (Sample sample in sampleList)
            {
                Waveform.Add(sample.F8);
            }

            return Waveform;
        }

        public Waveform GetEEGWaveformAF4()
        {
            Waveform Waveform = new Waveform(0, sampleRate);
            foreach (Sample sample in sampleList)
            {
                Waveform.Add(sample.AF4);
            }

            return Waveform;
        }
    }

}