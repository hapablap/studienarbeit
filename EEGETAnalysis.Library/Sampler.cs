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

        Dictionary<Electrode, int> columnNumbers = new Dictionary<Electrode, int>();
        int eyeXColumnNo = 0;
        int eyeYColumnNo = 0;

        List<EEGSample> goodSamples;

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
                    else if (content == "L POR X [px]") { eyeXColumnNo = i; }
                    else if (content == "L POR Y [px]") { eyeYColumnNo = i; }
                    else
                    {
                        foreach (Electrode electrode in (Electrode[])Enum.GetValues(typeof(Electrode)))
                        {
                            if (content == "EEG_RAW_" + electrode)
                            {
                                columnNumbers.Add(electrode, i);
                            }
                        }
                    }
                    i++;
                }
                if (i > 0) break;
            }

            csvData.RemoveRange(0, 2);
            csvData.RemoveAt(csvData.Count - 1);

            FindAllGoodSamples();
        }
       

        public List<EEGSample> GetAllGoodSamples()
        {
            return goodSamples;
        }



        public Waveform GetEEGWaveform(Electrode electrode)
        {
            Waveform Waveform = new Waveform(0, sampleRate);
            foreach (EEGSample sample in goodSamples)
            {
                Waveform.Add(sample.eegValues[electrode]);
            }

            return Waveform;
        }


        public Dictionary<Electrode, Waveform> GetEEGWaveforms()
        {
            Dictionary<Electrode, Waveform> waveforms = new Dictionary<Electrode, Waveform>();
            foreach (KeyValuePair<Electrode, int> availableElectrode in columnNumbers)
            {
                waveforms.Add(availableElectrode.Key, GetEEGWaveform(availableElectrode.Key));
            }
            return waveforms;
        }



        private EEGSample GetSample(int i)
        {
            if (i >= csvData.Count) return null;
            List<String> row = csvData[i];
            EEGSample sample = new EEGSample();
            sample.index = i;
            sample.timestamp = Convert.ToInt64(row[timestampPosition]);

            Electrode electrode;
            int columnNo;
            foreach (KeyValuePair<Electrode, int> column in columnNumbers)
            {
                electrode = column.Key;
                columnNo = column.Value;
                sample.eegValues.Add(electrode, Convert.ToDouble(row[columnNo], System.Globalization.CultureInfo.InvariantCulture));
            }

            sample.eyeX = Convert.ToDouble(row[eyeXColumnNo], System.Globalization.CultureInfo.InvariantCulture);
            sample.eyeY = Convert.ToDouble(row[eyeYColumnNo], System.Globalization.CultureInfo.InvariantCulture);

            return sample;
        }

        private EEGSample FindNextGoodSample(EEGSample startSample)
        {

            int i = startSample.index;

            long timestampWanted = (long)(startSample.timestamp + (1000000 / sampleRate));
            long bestDifferenceToWantedTimestamp = startSample.timestamp;
            EEGSample bestSample = null;

            // Überspringe aufeinanderfolgende Timestamps mit demselben Wert. Manchmal häufen die sich.
            EEGSample nextSample = GetSample(i + 1);
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

        private void FindAllGoodSamples()
        {
            goodSamples = new List<EEGSample>();

            EEGSample sample = GetSample(0);
            goodSamples.Add(sample);
            while ((sample = FindNextGoodSample(sample)) != null)
            {
                goodSamples.Add(sample);
            }
        }


        public int GetSampleRate()
        {
            return sampleRate;
        }

    }

}