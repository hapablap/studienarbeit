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

        Waveform waveform;
        int sampleRate;

        public EEGAnalyzer(Waveform Waveform, int sampleRate)
        {
            this.waveform = Waveform;
            this.sampleRate = sampleRate;
        }
        
        public Waveform FilterBeta()
        {
            LTISystem filterSystem = Filter.NRBandPass(12.5 / sampleRate, 30 / sampleRate, 10);
            return filterSystem.Filter(ref this.waveform);
        }

        public Waveform FilterAlpha()
        {
            LTISystem filterSystem = Filter.NRBandPass(7.5 / sampleRate, 12.5 / sampleRate, 10);
            return filterSystem.Filter(ref this.waveform);
        }

        public Waveform FilterTheta()
        {
            LTISystem filterSystem = Filter.NRBandPass(3.5 / sampleRate, 7.5 / sampleRate, 10);
            return filterSystem.Filter(ref this.waveform);
        }

        public Waveform FilterDelta()
        {
            LTISystem filterSystem = Filter.NRBandPass(0.5 / sampleRate, 3.5 / sampleRate, 10);
            return filterSystem.Filter(ref this.waveform);
        }


        public void SaveWavFile(String filename)
        {
            waveform.Quantise().SaveWaveFile(filename);
        }


        // Komplexwertiges Spektrum zurückgeben
        // Anzeige des Amplitudenspektrums erfolgt mit Graph.PlotDbSpectrum
        public Spectrum GetSpectrum(int beginSample, int length)
        {
            // 1 Sekunde langes Stück herausschneiden
            Waveform cutWaveform = waveform.Cut(beginSample, length);

            // Fensterfunktion anwenden, um Leck-Effekte an den Signalrändern zu vermeiden
            Waveform windowedCutWaveform = Window.Hamming(cutWaveform);

            // FFT anwenden und nur die untere Hälfte des Spektrums zurückgeben (bis Nyquist-Frequenz)
            ComplexWaveform cutComplexWaveform = windowedCutWaveform.Complex();
            return DFT.ComplexFFT(ref cutComplexWaveform).Half();
        }


        // Amplitudenspektrum wird im Bereich 0 bis 1 zurückgegeben. 1 entspricht sampleRate / 2 Hz.
        public Waveform GetAmplitudeSpectrum(int beginSample, int length) {

            Waveform magnitudeWaveform = GetSpectrum(beginSample, length).Mag();
            for (int i = 0; i < magnitudeWaveform.Count; i++)
            {
                magnitudeWaveform[i] = Math.Pow(magnitudeWaveform[i], 1.0 / 10);
            }

            return magnitudeWaveform;
        }


        
        public Waveform GetAlphaActivity()
        {

            // wir nehmen das Spektrum der letzen 32 Samples
            int retrospectionCount = 32;

            // wir benötigen eine leere Waveform
            Waveform activityWaveform = new Waveform(0, sampleRate);

            // zu Beginn können wir noch keine Aktivität berechnen, setze ersten Werte auf 0
            for (int i = 0; i < 32; i++)
            {
                activityWaveform.Add(0);
            }


            for (int i = 0; i < waveform.Count - retrospectionCount; i++)
            {
                Waveform phaseSpectrum = GetAmplitudeSpectrum(i, retrospectionCount);
                //TODO continue
            }

            return null;
        }
        

    }
}


