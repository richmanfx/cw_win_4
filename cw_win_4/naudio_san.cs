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
using System.IO;


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
        //int sample;
        
        public SineWaveProvider32()
        {
            Frequency = 1000;   // Тон и амплитуда посылки по умолчанию - перебиваются сеттерами
            Amplitude = 0.25f;         
        }

        public float Frequency { get; set; }
        public float Amplitude { get; set; }

        public override int Read(float[] buffer, int offset, int sampleCount)
        {
            int sampleRate = WaveFormat.SampleRate;
            
            // new
            int ramp = 10;               // скат посылки (крутизна?) - передавать бы нужно из конфига
            int rampSamples = (int)Math.Round(ramp * sampleRate / 1000.0);      // Количество выборок на фронт/спад
            double deltaAmplitude = 1.0 / rampSamples;

            //for (int n = 0; n < sampleCount; n++)
            //{
                // Синтез посылки 
            //    buffer[n + offset] = (float)(Amplitude * Math.Sin((2 * Math.PI * sample * Frequency) / sampleRate));

            
                // Фронт
                for (int i = 0; i < rampSamples; i++)
                {
                    buffer[i + offset] = (float)(Amplitude *
                            (Math.Sin(2.0 * Math.PI * Frequency * i / sampleRate)) *
                            i * deltaAmplitude);
                    if (i >= sampleRate)
                        buffer[i + offset] = 0;
                                      
                }
            
                // Середина посылки
                for (int i = rampSamples; i < (sampleCount - rampSamples); i++)
              //  for (int i = 0; i < sampleCount; i++)
                {
                  buffer[i + offset] = (float)(Amplitude * Math.Sin(2.0 * Math.PI * i * Frequency / sampleRate));
                  if (i >= sampleRate)
                      buffer[i + offset] = 0;
                }

            
                // Спад
                for (int i = (sampleCount - rampSamples); i < sampleCount; i++)
                {
                    buffer[i + offset] = (float)(Amplitude *
                                                (Math.Sin(2 * Math.PI * Frequency * i / sampleRate)) *
                                                ((sampleCount - i) * deltaAmplitude));
                    if (i >= sampleRate)
                        buffer[i + offset] = 0;
                }
            

            //*********************************************************************
            // Записать семпл в файл   --- для визуального анализа данных в gnuplot
            //*********************************************************************
            /*
            FileStream fileStream = new FileStream("sample1.txt", FileMode.Create);
            StreamWriter streamWriter = new StreamWriter(fileStream);
            streamWriter.BaseStream.Seek(fileStream.Length, SeekOrigin.End);//запись в конец файла
            for (int i = 0; i < sampleCount; i++)
            {
                streamWriter.Write("{0}\t{1}\n", i, buffer[i]);
            }
            streamWriter.Close();
            fileStream.Close();
            */

                // sample++;
                

               // if (sample >= sampleRate) 
                 //   sample = 0;

            //} // End for(n)
            return sampleCount;
        }
    }
}