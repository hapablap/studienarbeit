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

        public EEGEmotionizer(Sampler sampler)
        {
            this.sampler = sampler;

            Dictionary<Electrode, Waveform> eegWaveforms = sampler.GetEEGWaveforms();

            Analyzers = new Dictionary<Electrode, EEGAnalyzer>();
            foreach (KeyValuePair<Electrode, Waveform> eegWaveform in eegWaveforms)
            {
                Analyzers.Add(eegWaveform.Key, new EEGAnalyzer(eegWaveform.Value));
            }
        }

        /// <summary>
        /// Bildet den Durschnitt der Frequenzbandaktivität aus allen verfügbaren Elektroden.
        /// </summary>
        /// <param name="eegBand">Das Frequenzband, für welches der Durchschnitt berechnet werden soll.</param>
        /// <returns>Eine Waveform mit dem zeitlichen Verlauf der Aktivität im angegbenen Frequenzband.</returns>
        public Waveform GetAverageActivity(EEGBand eegBand)
        {
            List<Waveform> waveforms = new List<Waveform>();
            foreach (KeyValuePair<Electrode, EEGAnalyzer> analyzer in Analyzers)
            {
                waveforms.Add(analyzer.Value.GetActivity(eegBand));
            }
            return EEGUtils.AverageWaveform(waveforms);
        }


        /// <summary>
        /// Berechnet aus der Aktivität der Frequenzbänder die Ausprägung der Basisemotionen im zeitlichen Verlauf.
        /// </summary>
        /// <returns>Dicitonary, das den Emotionen eine Waveform zuordnet. Die Waveforms stellen den zeitlichen Verlauf der Emotionen dar.</returns>
        public Dictionary<Emotion, Waveform> GetEmotions()
        {
            Waveform alphaAcitvity = EEGUtils.Quantize(GetAverageActivity(EEGBand.ALPHA));
            Waveform betaActivity = EEGUtils.Quantize(GetAverageActivity(EEGBand.BETA));
            Waveform thetaActivity = EEGUtils.Quantize(GetAverageActivity(EEGBand.THETA));
            Waveform deltaActivity = EEGUtils.Quantize(GetAverageActivity(EEGBand.DELTA));

            Dictionary<Emotion, Waveform> emotions = new Dictionary<Emotion, Waveform>();
            emotions.Add(Emotion.FEAR, new Waveform(0, 1));
            emotions.Add(Emotion.JOY, new Waveform(0, 1));
            emotions.Add(Emotion.RAGE, new Waveform(0, 1));
            emotions.Add(Emotion.SORROW, new Waveform(0, 1));

            double alpha, beta, theta, delta, fear, joy, rage, sorrow;

            for (int i = alphaAcitvity.First; i <= alphaAcitvity.Last; i++)
            {
                alpha = alphaAcitvity[i];
                beta = betaActivity[i];
                theta = thetaActivity[i];
                delta = deltaActivity[i];

                if (alpha == 0 && beta == 0 && theta == 0 && delta == 0)
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

            emotions[Emotion.FEAR] = EEGUtils.Quantize(emotions[Emotion.FEAR]);
            emotions[Emotion.JOY] = EEGUtils.Quantize(emotions[Emotion.JOY]);
            emotions[Emotion.RAGE] = EEGUtils.Quantize(emotions[Emotion.RAGE]);
            emotions[Emotion.SORROW] = EEGUtils.Quantize(emotions[Emotion.SORROW]);

            return emotions;

        }

    }
}
