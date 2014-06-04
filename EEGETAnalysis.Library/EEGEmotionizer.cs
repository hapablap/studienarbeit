using BasicDSP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EEGETAnalysis.Library
{
    public class EEGEmotionizer
    {

        Sampler sampler;

        public Dictionary<Electrode, EEGAnalyzer> Analyzers { get; private set; }

        public Dictionary<EEGBand, Waveform> AverageActivity { get; private set; }

        public Dictionary<Emotion, Waveform> Emotions { get; private set; }


        public EEGEmotionizer(Sampler sampler)
        {
            this.sampler = sampler;

            Dictionary<Electrode, Waveform> eegWaveforms = sampler.GetEEGWaveforms();

            Analyzers = new Dictionary<Electrode, EEGAnalyzer>();
            foreach (KeyValuePair<Electrode, Waveform> eegWaveform in eegWaveforms)
            {
                Analyzers.Add(eegWaveform.Key, new EEGAnalyzer(eegWaveform.Value));
            }

            CalculateAverageActivity();
            CalculateEmotions();
        }


        /// <summary>
        /// Bildet den Durchschnitt der Frequenzbandaktivität aus allen verfügbaren Elektroden und speichert ihn zwischen.
        /// </summary>
        private void CalculateAverageActivity()
        {
            AverageActivity = new Dictionary<EEGBand, Waveform>();
            foreach (EEGBand eegBand in (EEGBand[])Enum.GetValues(typeof(EEGBand)))
            {
                List<Waveform> waveforms = new List<Waveform>();
                foreach (KeyValuePair<Electrode, EEGAnalyzer> analyzer in Analyzers)
                {
                    waveforms.Add(analyzer.Value.Activity[eegBand]);
                }
                AverageActivity.Add(eegBand, EEGUtils.AverageWaveform(waveforms));
            }
            
        }
  

        /// <summary>
        /// Berechnet aus der Aktivität der Frequenzbänder die Ausprägung der Basisemotionen im zeitlichen Verlauf und speichert ihn zwischen.
        /// </summary>
        private void CalculateEmotions()
        {

            Waveform alphaActivity = EEGUtils.Normalize(AverageActivity[EEGBand.ALPHA]);
            Waveform betaActivity = EEGUtils.Normalize(AverageActivity[EEGBand.BETA]);
            Waveform thetaActivity = EEGUtils.Normalize(AverageActivity[EEGBand.THETA]);

            Dictionary<Emotion, Waveform> emotions = new Dictionary<Emotion, Waveform>();
            emotions.Add(Emotion.FEAR, new Waveform(0, 1));
            emotions.Add(Emotion.JOY, new Waveform(0, 1));
            emotions.Add(Emotion.RAGE, new Waveform(0, 1));
            emotions.Add(Emotion.SORROW, new Waveform(0, 1));

            double alpha, beta, theta, fear, joy, rage, sorrow;

            for (int i = alphaActivity.First; i <= alphaActivity.Last; i++)
            {
                alpha = alphaActivity[i];
                beta = betaActivity[i];
                theta = thetaActivity[i];

                if (alpha == 0 && beta == 0 && theta == 0)
                {
                    fear = 0;
                    joy = 0;
                    rage = 0;
                    sorrow = 0;
                }
                else
                {
                    fear = ((beta + theta) / 2) - (alpha * ((beta + theta) / 2));
                    joy = alpha - (((theta + beta) / 2) * alpha);
                    rage = (alpha + beta + theta) / 3;
                    sorrow = 1 - ((alpha + beta + theta) / 3);
                }

                emotions[Emotion.FEAR].Add(fear);
                emotions[Emotion.JOY].Add(joy);
                emotions[Emotion.RAGE].Add(rage);
                emotions[Emotion.SORROW].Add(sorrow);
            }
            
            this.Emotions = emotions;

        }

    }
}
