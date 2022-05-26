using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundMonitor4Dogs
{
    public class MicrophoneLevel
    {
        private NAudio.Wave.WaveInEvent waveIn;   // install-package NAudio

        public event EventHandler<SoundLevelArgs> MicLevelReceived;

        public MicrophoneLevel(int micNumber = 0, bool isMono = true)
        {
            waveIn = new NAudio.Wave.WaveInEvent
            {
                DeviceNumber = micNumber, // customize this to select your microphone device
                WaveFormat = new NAudio.Wave.WaveFormat(rate: 44100, bits: 16, channels: 1),
                BufferMilliseconds = 50
            };

            waveIn.DataAvailable += ShowPeakMono;
            waveIn.StartRecording();
        }

        /// <summary>
        /// Called when the incoming Mono audio buffer is filled
        /// </summary>
        private void ShowPeakMono(object sender, NAudio.Wave.WaveInEventArgs args)
        {
            

            int maxValue = 32767;
            int peakValue = 0;
            int bytesPerSample = 2;
            for (int index = 0; index < args.BytesRecorded; index += bytesPerSample)
            {
                int value = BitConverter.ToInt16(args.Buffer, index);
                peakValue = Math.Max(peakValue, value);
            }

            // Calls the PrintLabel in Main Thread (to avoid cross-threading error)
            if (MicLevelReceived != null)
                MicLevelReceived(this, new SoundLevelArgs(peakValue, -1, maxValue));
        }

        /// <summary>
        /// Called when the incoming Stereo audio buffer is filled
        /// </summary>
        private void ShowPeakStereo(object sender, NAudio.Wave.WaveInEventArgs args)
        {
            float maxValue = 32767;
            int peakL = 0;
            int peakR = 0;
            int bytesPerSample = 4;
            for (int index = 0; index < args.BytesRecorded; index += bytesPerSample)
            {
                int valueL = BitConverter.ToInt16(args.Buffer, index);
                peakL = Math.Max(peakL, valueL);
                int valueR = BitConverter.ToInt16(args.Buffer, index + 2);
                peakR = Math.Max(peakR, valueR);
            }

            // peak are values for channels, and max is the maximum
            string vals = peakL.ToString() + peakR.ToString() + maxValue.ToString();
        }

    }

    public class SoundLevelArgs : EventArgs
    {
        int _levelL, _levelR, _maximum;

        public int LevelL { get => _levelL; }
        public int LevelR { get => _levelR; }
        public int Maximum { get => _maximum; }

        public SoundLevelArgs(int levelL, int levelR, int maximum)
        {
            _levelL = levelL;
            _levelR = levelR;
            _maximum = maximum;
        }
    }
}
