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
        /// Normalisiert eine Waveform in den Bereich zwischen 0 und 1.
        /// </summary>
        /// <param name="waveform">Die originale Waveform.</param>
        /// <returns>Die normalisierte Waveform.</returns>
        public static Waveform Normalize(Waveform waveform)
        {
            List<Waveform> waveforms = new List<Waveform>();
            waveforms.Add(waveform);
            NormalizeMultiple(ref waveforms);
            return waveforms[0];
        }

        /// <summary>
        /// Normalisiert mehrere Waveforms in den Bereich zwischen 0 und 1. Die Referenzen (Minimum und Maximum) werden dabei über alle Waveforms hinweg gesucht.
        /// </summary>
        /// <param name="waveforms">Die zu normalisierenden Waveforms. Die Liste wird durch die normalisierten Waveforms ersetzt.</param>
        public static void NormalizeMultiple(ref List<Waveform> waveforms)
        {
            if (waveforms.Count < 1) return;

            double min = waveforms[0][waveforms[0].First];
            double max = waveforms[0][waveforms[0].First];

            // kleinsten und größten Wert suchen
            foreach (Waveform wf in waveforms)
            {
                for (int i = wf.First; i <= wf.Last; i++)
                {
                    if (wf[i] < min) min = wf[i];
                    if (wf[i] > max) max = wf[i];
                }
            }

            // alle Waveforms normalisieren
            if (max - min > 0)
            {
                for (int j = 0; j < waveforms.Count; j++)
                {
                    Waveform wf = waveforms[j];
                    Waveform normalizedWaveform = new Waveform(0, wf.Rate);
                    for (int i = wf.First; i <= wf.Last; i++)
                    {
                        // Wert normalisieren
                        normalizedWaveform.Add((wf[i] - min) / (max - min));
                    }
                    waveforms[j] = normalizedWaveform;
                }
            }
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
