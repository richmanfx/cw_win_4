/*	
	Классы для использования библиотеки NAudio.
	Требуется подключить в проекте dll-ку NAudio.dll в
	"Обозревателе решений" -> "Добавить ссылку..." ->
	закладка "Обзор".
	

	Пример использования:

	using NAudio.Wave;

	в Main:	
	    WaveOut waveOut;
            
            Console.ReadKey();

            var sineWaveProvider = new SineWaveProvider32();

            sineWaveProvider.SetWaveFormat(16000, 1); // 16kHz mono
            sineWaveProvider.Frequency = 700;
            sineWaveProvider.Amplitude = 0.25f;
            waveOut = new WaveOut();
            waveOut.Init(sineWaveProvider);

            int speed = 50;
            
            // R
            waveOut.Play(); Thread.Sleep(speed); waveOut.Stop(); Thread.Sleep(speed);
            waveOut.Play(); Thread.Sleep(3*speed); waveOut.Stop(); Thread.Sleep(speed);
            waveOut.Play(); Thread.Sleep(speed); waveOut.Stop();
            Thread.Sleep(speed * 3);


            Console.ReadKey();
            //waveOut.Stop();
            waveOut.Dispose();
            waveOut = null;

*/
using NAudio.Wave;
using System.Threading;
using System;


namespace ZoerNaudio
{

    public abstract class WaveProvider32 : IWaveProvider
    {
        private WaveFormat waveFormat;

        public WaveProvider32()
            : this(44100, 1)
        {
        }

        public WaveProvider32(int sampleRate, int channels)
        {
            SetWaveFormat(sampleRate, channels);
        }

        public void SetWaveFormat(int sampleRate, int channels)
        {
            this.waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels);
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            WaveBuffer waveBuffer = new WaveBuffer(buffer);
            int samplesRequired = count / 4;
            int samplesRead = Read(waveBuffer.FloatBuffer, offset / 4, samplesRequired);
            return samplesRead * 4;
        }

        public abstract int Read(float[] buffer, int offset, int sampleCount);

        public WaveFormat WaveFormat
        {
            get { return waveFormat; }
        }
    }

    public class SineWaveProvider32 : WaveProvider32
    {
        int sample;

        public SineWaveProvider32()
        {
            Frequency = 1000;
            Amplitude = 0.25f; // let's not hurt our ears            
        }

        public float Frequency { get; set; }
        public float Amplitude { get; set; }

        public override int Read(float[] buffer, int offset, int sampleCount)
        {
            int sampleRate = WaveFormat.SampleRate;
            for (int n = 0; n < sampleCount; n++)
            {
                buffer[n + offset] = (float)(Amplitude * Math.Sin((2 * Math.PI * sample * Frequency) / sampleRate));
                sample++;
                if (sample >= sampleRate) sample = 0;
            }
            return sampleCount;
        }
    }
}