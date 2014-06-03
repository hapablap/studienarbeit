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

        public Waveform Waveform { get; private set; }
        double sampleRate;

        public Dictionary<EEGBand, Waveform> Activity;

        public EEGAnalyzer(Waveform Waveform)
        {
            this.Waveform = Waveform;
            this.sampleRate = Waveform.Rate;
            CalculateActivity();
        }

        /// <summary>
        /// Wendet einen Band-Pass-Filter auf die Waveform an.
        /// </summary>
        /// <param name="eegBand">Das Frequenzband, auf das die Waveform beschränkt werden soll.</param>
        /// <returns>Die gefilterte Waveform.</returns>
        public Waveform FilterBand(EEGBand eegBand)
        {
            LTISystem filterSystem = Filter.NRBandPass(eegBand.GetMinFreq() / sampleRate, eegBand.GetMaxFreq() / sampleRate, 10);
            Waveform wf = this.Waveform;
            return filterSystem.Filter(ref wf);
        }
        

        /// <summary>
        /// Speichert die Waveform in einer WAV-Datei.
        /// </summary>
        /// <param name="filename">Der gewünschte vollständige Dateiname mit Pfad.</param>
        public void SaveWavFile(String filename)
        {
            Waveform.Quantise().SaveWaveFile(filename);
        }


        /// <summary>
        /// Ermittelt das Spektrum eines Signalausschnitts.
        /// </summary>
        /// <param name="beginSample">Beginn des Signalausschnitts (Index)</param>
        /// <param name="length">Länge des Signalausschnitts (Zahl der Indizes)</param>
        /// <returns>Das komplexwertige Spektrum. Seine Länge ist length / 2.</returns>
        public Spectrum GetSpectrum(int beginSample, int length)
        {
            // 1 Sekunde langes Stück herausschneiden
            Waveform cutWaveform = Waveform.Cut(beginSample, length);

            // Fensterfunktion anwenden, um Leck-Effekte an den Signalrändern zu vermeiden
            Waveform windowedCutWaveform = Window.Hamming(cutWaveform);

            // FFT anwenden und nur die untere Hälfte des Spektrums zurückgeben (bis Nyquist-Frequenz)
            ComplexWaveform cutComplexWaveform = windowedCutWaveform.Complex();
            return DFT.ComplexFFT(ref cutComplexWaveform).Half();
        }


        /// <summary>
        /// Ermittelt das Amplitudenspektrum eines Singalausschnitts.
        /// </summary>
        /// <param name="beginSample">Beginn des Signalausschnitts (Index)</param>
        /// <param name="length">Länge des Signalausschnitts (Zahl der Indizes)</param>
        /// <returns>Das Amplitudenspektrum. Seine Länge ist length / 2.</returns>
        public Waveform GetAmplitudeSpectrum(int beginSample, int length) {

            Waveform magnitudeWaveform = GetSpectrum(beginSample, length).Mag();
            /*for (int i = magnitudeWaveform.First; i <= magnitudeWaveform.Last; i++)
            {
                magnitudeWaveform[i] = Math.Log10(magnitudeWaveform[i]);
            }*/

            return magnitudeWaveform;
        }

        /// <summary>
        /// Generiert eine Waveform, die die sekündliche Aktivität eines EEG-Frequenzbandes wiederspiegelt und speichert sie zwischen. Die Samplerate ist 1 Hz.
        /// </summary>
        private void CalculateActivity()
        {
            this.Activity = new Dictionary<EEGBand, Waveform>();
            foreach (EEGBand eegBand in (EEGBand[])Enum.GetValues(typeof(EEGBand)))
            {
                double minFreq = eegBand.GetMinFreq();
                double maxFreq = eegBand.GetMaxFreq();

                // wir nehmen das Spektrum der letzen 2 Sekunden
                int retrospectionCount = (int)sampleRate * 2;

                // die Samplerate der Activity-Waveform soll 1 Hz sein.
                int activitySampleRate = 1;

                // finde die Indizes im diskreten Spektrum, die Ober- und Unterkante des Frequenzbands darstellen
                int minFreqIndex = EEGUtils.FindSpectrumIndexForFreq(minFreq, retrospectionCount / 2, (int)sampleRate);
                int maxFreqIndex = EEGUtils.FindSpectrumIndexForFreq(maxFreq, retrospectionCount / 2, (int)sampleRate);

                // wir benötigen eine leere Waveform
                Waveform activityWaveform = new Waveform(0, activitySampleRate);

                // Ermittle das Frequenzspektrum in regelmäßigen Abständen  und berechne die Aktivität im Frequenzband
                for (int i = Waveform.First; i <= Waveform.Last - retrospectionCount; i = i + ((int)sampleRate / activitySampleRate))
                {
                    Waveform amplitudeSpectrum = GetAmplitudeSpectrum(i, retrospectionCount);
                    double activityValue = 0;
                    for (int j = minFreqIndex; j <= maxFreqIndex; j++)
                    {
                        activityValue = activityValue + amplitudeSpectrum[j];
                    }
                    activityWaveform.Add(activityValue / (maxFreqIndex - minFreqIndex + 1));
                }

                // Die Waveform ist etwas zu kurz und die Werte beziehen sich jeweils auf zwei Sekunden später
                // Kompensieren, indem die Welle nach hinten verschoben wird und die ersten Werte denselben Wert annehmen
                Waveform compensationWaveform = new Waveform(0, activitySampleRate);
                for (int i = 0; i <= retrospectionCount; i = i + ((int)sampleRate / activitySampleRate))
                {
                    compensationWaveform.Add(activityWaveform[activityWaveform.First]);
                }
                this.Activity.Add(eegBand, EEGUtils.Concatenate(compensationWaveform, activityWaveform));
            }

           
        }

    }
}


