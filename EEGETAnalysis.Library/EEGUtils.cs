using BasicDSP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EEGETAnalysis.Library
{
    public class EEGUtils
    {

        /// <summary>
        /// Quantisiert eine Waveform in den Bereich zwischen 0 und 1.
        /// </summary>
        /// <param name="waveform">Die originale Waveform.</param>
        /// <returns>Die quantisierte Waveform.</returns>
        public static Waveform Quantize(Waveform waveform)
        {
            double min = 0;
            double max = 0;

            // höchsten und niedrigsten Wert finden
            for (int i = waveform.First; i <= waveform.Last; i++)
            {
                if (waveform[i] < min) min = waveform[i];
                if (waveform[i] > max) max = waveform[i];
            }

            // neue Waveform generieren, welche die quantisierten Werte enthält
            Waveform quantizedWaveform = new Waveform(0, waveform.Rate);
            for (int i = waveform.First; i <= waveform.Last; i++)
            {
                quantizedWaveform.Add((waveform[i] - min) / (max - min));
            }

            return quantizedWaveform;
        }

        /// <summary>
        /// Findet zu einer Frequenz den nächstkleineren Index im diskreten Spektrum.
        /// </summary>
        /// <param name="freq">Frequenz</param>
        /// <returns>Index im diskreten Spektrum</returns>
        public static int FindSpectrumIndexForFreq(double freq, int spectrumLength, int sampleRate)
        {
            return (int)Math.Floor(freq / (sampleRate / 2) * spectrumLength);
        }


        /// <summary>
        /// Bildet den Durschnitt aus einer Liste aus Waveforms. Die Waveforms müssen gleich lang sein.
        /// </summary>
        /// <param name="waveforms">Die Liste aus Waveforms.</param>
        /// <returns>Die Durchschnitts-Waveform.</returns>
        public static Waveform AverageWaveform(List<Waveform> waveforms)
        {
            int noOfWaveforms = waveforms.Count;

            if (noOfWaveforms < 1) return null;
            else if (noOfWaveforms < 2) return waveforms[0];


            double average;
            Waveform averageWaveform = new Waveform(0, waveforms[0].Rate);
            for (int i = waveforms[0].First; i <= waveforms[0].Last; i++)
            {
                average = 0;
                foreach (Waveform waveform in waveforms)
                {
                    average = average + waveform[i];
                }
                average = average / noOfWaveforms;
                averageWaveform.Add(average);
            }

            return averageWaveform;
        }

        /// <summary>
        /// Hängt zwei Waveforms aneinander an.
        /// </summary>
        /// <param name="a">Die erste Waveform.</param>
        /// <param name="b">Die zweite Waveform.</param>
        /// <returns>Die verbundene Waveform (a+b).</returns>
        public static Waveform Concatenate(Waveform a, Waveform b)
        {
            for (int i = b.First; i <= b.Last; i++)
            {
                a.Add(b[i]);
            }
            return a;
        }
    }
}
