using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using NAudio.Wave;
using ZoerNaudio;
using System.Threading;
using INI;
using System.Globalization;
using System.Diagnostics;

namespace cw_win_4
{
    public partial class Cw_winForm : Form
    {
        string fileNameHlp = Path.ChangeExtension("cw_win_4", "chm");   // Файл win-помощи
        public static string cw_win_user_folder = "cw_win";             // Папка для INI-файла
        public static string ini_file_name = "cw_win.ini";              // Имя INI-файла
        public static string originalAudioFileName = "\\cw_audio";      // Начало имени звуковых файлов
        string[] AllWords;                                              // Массив для слов из файла

        // Значения по умолчанию
       // public static string default_cw_filename = "cw.txt";     // Файл со словами по умолчанию - заготовка в INI хранить!!!
        
        public static int numberofwords = 10;		            // количество слов
        public static int interval = 1; 			            // интервал между словами
        public static int speed = 100;				            // скорость в знаках в минуту
        public static int tone = 700; 				            // высота тона
        public static int startpause = 3;                       // Пауза перед стартом передачи, секунды
        public static int speed_calibr = 5800;                  // Коэффициент для расчёта длительностей
        public static bool startpause_flag = false;             // Признак паузы перед стартом
        public static bool infinity_work_flag = false;          // Признак бесконечной работы
        public static bool russianpunktuation_flag = true;      // Признак русских знаков пунктуации
        public static bool alternative_mp3_flag = false;        // Альтернативное размещение звуковых файлов
        public static bool english_flag = false;                // Признак английского интерфейса
        bool working_flag = false;                              // Признак работы режима воспроизведения
        public static bool exist_ini_file = false;              // Признак существования INI-файла
        public static bool non_random_flag = false;             // Признак отмены случайного выбора слов

        public Cw_winForm()
        {
            // Существует ли файл INI
            exist_ini_file = ExistIniFile(cw_win_user_folder, ini_file_name);

            // Если INI-файл существует, то читаем из него параметры
            if (exist_ini_file)
            {
                ReadIniFile(cw_win_user_folder, ini_file_name);
            }

            // Установка "UI culture" в Английский или Русский).
            if(english_flag)
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
            else
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru");
            
            // Инициализируем компоненты на форме
            InitializeComponent();

            // Перерисовать индикаторы и движки регуляторов
            LabelAndTrackbarShow();
        }

        // Выход из программы по кнопке "Exit"
        private void button_Exit_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        // Покажем форму "О программе" по кнопке "About"
        private void button_About_Click(object sender, EventArgs e)
        {
            AboutForm form_about = new AboutForm();
            form_about.ShowDialog();
        }

        // Вызов Win-Help по кнопке "Help"
        private void button_Help_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, fileNameHlp, HelpNavigator.TableOfContents);
        }

        // Открываем файл со словами
        private void button_OpenFile_Click(object sender, EventArgs e)
        {
            DialogFileOpen();
        }


        /// <summary>
        /// Открытие файла с CW-словами
        /// </summary>
        private void DialogFileOpen()
        {
            openFileDialog1.Multiselect = false;    // Несколько файлов в окне не выбирать

            if (openFileDialog1.ShowDialog() ==
                System.Windows.Forms.DialogResult.OK &&
                openFileDialog1.FileName.Length > 0)
            {
                // Читаем отдельные слова  в массив, устраняем пробелы. 
                // Кодировка Default - операционной системы
                AllWords = File.ReadAllText(openFileDialog1.FileName, Encoding.Default).
                                            Split((char[])null,
                                            StringSplitOptions.RemoveEmptyEntries);
                
                // Выводим имя файла на форму
                label_FileName.Text = openFileDialog1.SafeFileName;
                
                // Очистим поле вывода слов
                label_OutText.Text = "";
            }
        }

        // Обработка кнопки "Start/Stop"
        private void button_StartStop_Click(object sender, EventArgs e)
        {
            if (!working_flag)
             {
                button_StartStop.ForeColor = System.Drawing.Color.Red;
                button_StartStop.Text = "Стоп";
                working_flag = true;                // Запускаем процесс воспроизведения
                StartPlay();
             }
            else
             {
                working_flag = false;               // Останавливаем процесс воспроизведения
             }
        }

        // Процесс воспроизведения 
        private void StartPlay()
        {
            // Если INI-файл существует, то читаем из него параметры
            if (exist_ini_file)
            {
                ReadIniFile(cw_win_user_folder, ini_file_name);
            }

            // Проверяем открыт ли какой-нибудь файл
            if (openFileDialog1.FileName.Length < 1)
            {
                if (english_flag)
                    label_OutText.Text = "No file is selected with CW-words.";
                else
                    label_OutText.Text = "Не выбран файл с CW-словами.";

                // Восстанавливаем вид кнопки
                button_StartStop.ForeColor = System.Drawing.Color.Yellow;
                button_StartStop.Text = "Старт";
                working_flag = false;               // Останавливаем процесс воспроизведения
                return;
            }
            
            // Выводим имя файла на форму
            label_FileName.Text = openFileDialog1.SafeFileName;
            label_FileName.Update();

            // Изменяем доступность кнопок
            if (english_flag)
                button_StartStop.Text = "Stop";
            else
                button_StartStop.Text = "Стоп";
            button_StartStop.Update();

            button_Exit.Enabled = false;
            button_Config.Enabled = false;
            button_OpenFile.Enabled = false;
            button_SafeFile.Enabled = false;
            button_ToMp3.Enabled = false;
            button_Help.Enabled = false;
            button_About.Enabled = false;

            // Очистим поле вывода слов
            label_OutText.Text = "";
            label_OutText.Update();

            // Выставляем флаг работы режима воспроизведения
            working_flag = true;

            // Запускаем воспроизведение слов
            SoundWord();

            // Убираем флаг работы режима воспроизведения
            working_flag = false;

            // Возвращение доступности кнопок
            button_StartStop.ForeColor = System.Drawing.Color.Yellow;   // Жёлтая надпись
            if (english_flag)
                button_StartStop.Text = "Start";
            else
                button_StartStop.Text = "Старт";

            button_Exit.Enabled = true;
            button_Config.Enabled = true;
            button_OpenFile.Enabled = true;
            button_SafeFile.Enabled = true;
            button_ToMp3.Enabled = true;
            button_Help.Enabled = true;
            button_About.Enabled = true;

            // Проверить, может ли кнопка "Старт" получить фокус,
            // если да, передать ей фокус.
            if (button_StartStop.CanFocus)
                button_StartStop.Focus();
        }

        private void SoundWord()
        {
            // Считываем параметры с движков регуляторов

             // Беконечная работа - если слово условно звучит одну секунду, то это на 1 год
             if (infinity_work_flag) 
                 numberofwords = 31536000;
             else 
                 numberofwords = trackBar_N.Value;
            
             interval = trackBar_Pause.Value;
             speed = trackBar_Speed.Value;
             tone = trackBar_Tone.Value;


            // Инициализация звуковой карты (NAudio library)
            WaveOut MyWaveOut = new WaveOut();
            var sineWaveProvider = new SineWaveProvider32();
            sineWaveProvider.SetWaveFormat(16000, 1); // 16кГц, моно
            sineWaveProvider.Frequency = tone;          // Тон посылки
            sineWaveProvider.Amplitude = 0.5f;          // Амплитуда посылки
            MyWaveOut.Init(sineWaveProvider);

            // Воспроизведение слова
            int dot = speed_calibr / speed;    // Длительность точки 
            int dash = 3 * dot;                // Длительность тире
            int dash_conjoint = dash;          // Промежуток между слитными буквами типа <KN>

            // Если стоит галка "Пауза при старте", то использовать паузу перед началом
            if (checkBox_StartPause.Checked)
            {
                startpause_flag = true;                // Использовать паузу
                Thread.Sleep(startpause * 1000);	   // Задержка в секундах перед началом передачи
            }

            // Выводим случайные слова из массива
            
            Random rnd = new Random();
                       
            if(non_random_flag)
            {
                numberofwords = AllWords.Length;        // При отмене случайного выбора слов выводим все слова
            }                                           // один раз
              

            for (int i = 1; i <= numberofwords; i++)
            {
             int num;

             // Обрабатываем кнопку "СТОП"
             Application.DoEvents();
             if (!working_flag) 
                 break;          //останавливаем воспроизведение слов

             if (!non_random_flag)
             {
                 num = rnd.Next(0, AllWords.Length);	// Номер случайного слова
             }
             else
             {
                 num = i-1;    
             }

            
             string word = AllWords[num].ToUpper();     // Получаем случайное слово в верхнем 
                                                        // регистре
             for (int j = 0; j < word.Length; j++)      // Перебираем буквы в слове
             {

                 // Обрабатываем кнопку "СТОП"
                 Application.DoEvents();
                 if (!working_flag)
                     break;          //останавливаем воспроизведение
                 
                // Воспроизводим букву
                char symbol = word[j];

                // Слитные буквы для <AS> и т.п.
                if (symbol == '<') 
                    dash_conjoint = dot;	            // "<" - слитное слово
                
                 if (symbol == '>')
                {
                    dash_conjoint = dash;			    // ">" - раздельное слово
                    Thread.Sleep(dash_conjoint);
                }

                    // Цифры
                    if(symbol == '0')
                    {
                        MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                        MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                        MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                        MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                        MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop();
                        Thread.Sleep(dash_conjoint);
                    }

                     if(symbol == '1')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dot);  MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop();
                         Thread.Sleep(dash_conjoint);
                     }

                     if(symbol == '2')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dot);  MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot);  MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop();
                         Thread.Sleep(dash_conjoint);
                     }

                     if(symbol == '3')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dot);  MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot);  MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot);  MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop();
                         Thread.Sleep(dash_conjoint);
                     }

                     if(symbol == '4')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dot);  MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot);  MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot);  MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot);  MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop();
                         Thread.Sleep(dash_conjoint);
                     }

                     if(symbol == '5')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop();
                         Thread.Sleep(dash_conjoint);
                     }

                     if(symbol == '6')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop();
                         Thread.Sleep(dash_conjoint);
                     }

                     if(symbol == '7')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop();
                         Thread.Sleep(dash_conjoint);
                     }

                     if(symbol == '8')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop();
                         Thread.Sleep(dash_conjoint);
                     }

                     if(symbol == '9')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop();
                         Thread.Sleep(dash_conjoint);
                     }

                     // Буквы
                     if (symbol == 'A' || symbol == 'А')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop();
                         Thread.Sleep(dash_conjoint);
                     }
                     
                     if (symbol == 'B' || symbol == 'Б')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop();
                         Thread.Sleep(dash_conjoint);
                     }

                     if (symbol == 'C' || symbol == 'Ц')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop();
                         Thread.Sleep(dash_conjoint);
                     }

                     if (symbol == 'D' || symbol == 'Д')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop();
                         Thread.Sleep(dash_conjoint);
                     }

                     if (symbol == 'E' || symbol == 'Е' || symbol == 'Ё')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop();
                         Thread.Sleep(dash_conjoint);
                     }

                     if (symbol == 'F' || symbol == 'Ф')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop();
                         Thread.Sleep(dash_conjoint);
                     }

                     if (symbol == 'G' || symbol == 'Г')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop();
                         Thread.Sleep(dash_conjoint);
                     }

                     if (symbol == 'H' || symbol == 'Х')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop();
                         Thread.Sleep(dash_conjoint);
                     }

                     if (symbol == 'I' || symbol == 'И')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop();
                         Thread.Sleep(dash_conjoint);
                     }

                     if (symbol == 'J' || symbol == 'Й')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop();
                         Thread.Sleep(dash_conjoint);
                     }

                     if (symbol == 'K' || symbol == 'К')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop();
                         Thread.Sleep(dash_conjoint);
                     }

                     if (symbol == 'L' || symbol == 'Л')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop();
                         Thread.Sleep(dash_conjoint);
                     }

                     if (symbol == 'M' || symbol == 'М')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop();
                         Thread.Sleep(dash_conjoint);
                     }

                     if (symbol == 'N' || symbol == 'Н')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop();
                         Thread.Sleep(dash_conjoint);
                     }

                     if (symbol == 'O' || symbol == 'О')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop();
                         Thread.Sleep(dash_conjoint);
                     }

                     if (symbol == 'P' || symbol == 'П')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop();
                         Thread.Sleep(dash_conjoint);
                     }

                     if (symbol == 'Q' || symbol == 'Щ')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop();
                         Thread.Sleep(dash_conjoint);
                     }

                     if (symbol == 'R' || symbol == 'Р')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop();
                         Thread.Sleep(dash_conjoint);
                     }

                     if (symbol == 'S' || symbol == 'С')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop();
                         Thread.Sleep(dash_conjoint);
                     }

                     if (symbol == 'T' || symbol == 'Т')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop();
                         Thread.Sleep(dash_conjoint);
                     }

                     if (symbol == 'U' || symbol == 'У')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop();
                         Thread.Sleep(dash_conjoint);
                     }

                     if (symbol == 'V' || symbol == 'Ж')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop();
                         Thread.Sleep(dash_conjoint);
                     }

                     if (symbol == 'W' || symbol == 'В')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop();
                         Thread.Sleep(dash_conjoint);
                     }

                     if (symbol == 'X' || symbol == 'Ь')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop();
                         Thread.Sleep(dash_conjoint);
                     }

                     if (symbol == 'Y' || symbol == 'Ы')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop();
                         Thread.Sleep(dash_conjoint);
                     }

                     if (symbol == 'Z' || symbol == 'З')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop();
                         Thread.Sleep(dash_conjoint);
                     }

                     if (symbol == 'Ч')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop();
                         Thread.Sleep(dash_conjoint);
                     }

                     if (symbol == 'Ш')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop();
                         Thread.Sleep(dash_conjoint);
                     }

                     if (symbol == 'Э')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop();
                         Thread.Sleep(dash_conjoint);
                     }

                     if (symbol == 'Ю')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop();
                         Thread.Sleep(dash_conjoint);
                     }

                     if (symbol == 'Я')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop();
                         Thread.Sleep(dash_conjoint);
                     }

                     // Символы
                     if (symbol == '=')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot);  MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot);  MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot);  MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop();
                         Thread.Sleep(dash);
                     }

                     if (symbol == '?')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dot);  MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot);  MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot);  MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot);  MyWaveOut.Stop();
                         Thread.Sleep(dash);
                     }

                     if (symbol == '/')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot);  MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot);  MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot);  MyWaveOut.Stop();
                         Thread.Sleep(dash);
                     }

                     if (symbol == '@')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dot);  MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot);  MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot);  MyWaveOut.Stop();
                         Thread.Sleep(dash);
                     }

                     if (symbol == '.' && russianpunktuation_flag)
                     {
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop();
                         Thread.Sleep(dash);
                     }

                     if (symbol == '.' && !russianpunktuation_flag)
                     {
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop();
                         Thread.Sleep(dash);
                     }

                     if (symbol == ',' && russianpunktuation_flag)
                     {
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop();
                         Thread.Sleep(dash);
                     }

                     if (symbol == ',' && !russianpunktuation_flag)
                     {
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop();
                         Thread.Sleep(dash);
                     }

                     if (symbol == ';')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop();
                         Thread.Sleep(dash);
                     }

                     if (symbol == ':')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop();
                         Thread.Sleep(dash);
                     }

                     if (symbol == '!' && russianpunktuation_flag)
                     {
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop();
                         Thread.Sleep(dash);
                     }

                     if (symbol == '!' && !russianpunktuation_flag)
                     {
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop();
                         Thread.Sleep(dash);
                     }

                     if (symbol == '-')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop();
                         Thread.Sleep(dash);
                     }

                     if (symbol == '\'')
                     {
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dash); MyWaveOut.Stop(); Thread.Sleep(dot);
                         MyWaveOut.Play(); Thread.Sleep(dot); MyWaveOut.Stop();
                         Thread.Sleep(dash);
                     }

             } // End For j

             if (non_random_flag)
             {
                 screen_Out(word);     // вывод на label непрерывно
             }
             else
             {
                 screen_Out_By_10(i, word);     // вывод на label по 10 слов
             }

           
             Thread.Sleep(interval * dash);                // Пауза между словами

            } // End for(i)

            // Звершить работу со звуковой картой
            MyWaveOut.Dispose();
            MyWaveOut = null;
        }


        /* Вывод на экран
         * Эмулируем "\t" при выводе на label
         * Разбиваем на 10 столбцов
         */
        private void screen_Out_By_10(int i, string word)
        {
            string print_buffer = "";

            label_OutText.Text += word;
            label_OutText.Update();
            
            if (Convert.ToBoolean(i % 10))			    // Разбиваем вывод слов на 10 столбцов
            {
                int addition = 10 - word.Length;        // Добиваем пробелами до 10 символов
                for (int k = 1; k <= addition; k++)
                    print_buffer += " ";
                label_OutText.Text += print_buffer;
            }
            else
                label_OutText.Text += "\n";
        }


        /* Вывод на экран смыслового текста без случайных слов
         * 
         */
        private void screen_Out(string word)
        {
            string print_buffer = "";

            label_OutText.Text += word;
            label_OutText.Update();
            print_buffer += " ";
            label_OutText.Text += print_buffer;
        }


        private void trackBar_Tone_Scroll(object sender, EventArgs e)
        {
            label_Show_Tone.Text = trackBar_Tone.Value.ToString();
        }

        private void trackBar_N_Scroll(object sender, EventArgs e)
        {
            label_Show_N.Text = trackBar_N.Value.ToString();
        }

        private void trackBar_Speed_Scroll(object sender, EventArgs e)
        {
            label_Show_Speed.Text = trackBar_Speed.Value.ToString();
        }

        private void trackBar_Pause_Scroll(object sender, EventArgs e)
        {
            label_Show_Pause.Text = trackBar_Pause.Value.ToString();
        }

        private void button_SafeFile_Click(object sender, EventArgs e)
        {
            DialogSaveOpen();
        }

        // Сохранияем выведенные слова в текстовый файл
        private void DialogSaveOpen()
        {
         saveFileDialog1.Filter = "Text file | *.txt";
         saveFileDialog1.FileName = "my_cw.txt";
         saveFileDialog1.DefaultExt = "txt";
         
         DialogResult SaveDialogResult = saveFileDialog1.ShowDialog();
         string ReadResFilePlace = saveFileDialog1.FileName;    // Путь к файлу 


         if (ReadResFilePlace != String.Empty)  // Если путь к файлу не пустой
         {  
            
             if (SaveDialogResult == DialogResult.OK)
             {
                 // Используем в файле кодировку ОС (можно windows-1251), 
                 // false - файл перезаписываем
                 using (StreamWriter sw = new StreamWriter(ReadResFilePlace,
                                                           false,
                                                           Encoding.Default))
                                                        // Encoding.GetEncoding("windows-1251")))

                 // Читаем из поля вывода слов сгенерированные слова и пишем в файл 
                 sw.WriteLine(label_OutText.Text);

                 if (english_flag)
                     label_FileName.Text = "The file is saved.";
                 else
                     label_FileName.Text = "Файл сохранён.";
             }
             return;
         }
        }

        // Обработка функциональных клавиш
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
         // На форме нужно выставить свойство "keyPreview" в "true"!!!
         string key = Convert.ToString(e.KeyData);
         
         if (key == "F1")
         {
             Help.ShowHelp(this, fileNameHlp, HelpNavigator.TableOfContents);
         }

         if (key == "F2" || key == "S, Control")
         {
             DialogSaveOpen();
         }

         if (key == "F3" || key == "O, Control")
         {
             DialogFileOpen();
         }

         if (key == "F5")
         {
             StartPlay();
         }

         if (key == "F9")
         {
             ConfigForm form_config = new ConfigForm();
             form_config.ShowDialog();
         }

         if (key == "F10" || key == "Q, Control")
         {
             Environment.Exit(0);
         }

        }

        // Вывод формы настроек по кнопке "Config" 
        private void button_Config_Click(object sender, EventArgs e)
        {
            ConfigForm form_config = new ConfigForm();
            form_config.ShowDialog();
        }

        // Проверяем существование INI-файла
        private bool ExistIniFile(string cw_win_user_folder, string ini_file_name)
        {
          // Путь к папке файлов приложений пользователя
          string user_app_path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

          // Проверить существование папки
          DirectoryInfo folder = new DirectoryInfo(Path.Combine(user_app_path, cw_win_user_folder));
          if (!folder.Exists)
          {
            return false;       // INI-файла нет, так как нет папки для него
          }
          else
          {
            // Существует ли файл?
            FileInfo ini_file = new FileInfo(Path.Combine(user_app_path, 
                                                          cw_win_user_folder,
                                                          ini_file_name));
            if (!ini_file.Exists)
            {
                return false;       // INI-файла нет
            }
          }
          return true;       // INI-файл уже cуществует
        }

        // Читаем параметры из INI-файла
        private void ReadIniFile(string cw_win_user_folder, string ini_file_name)
        {
            // Путь к папке файлов приложений пользователя
            string user_app_path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            // Полный путь к файлу (вместе с именем файла)
            string full_path_and_name = Path.Combine(user_app_path, Cw_winForm.cw_win_user_folder,
                                                                    Cw_winForm.ini_file_name);
            // Открываем и считываем настройки из INI-файла
            ini_parser cw_win_ini_file = new ini_parser(full_path_and_name);
            try
            {
                if (cw_win_ini_file.GetSetting("CW_WIN", "RUSSIANPUNKTUATION").ToUpper() == "YES")
                    russianpunktuation_flag = true;
                else
                {
                    if (cw_win_ini_file.GetSetting("CW_WIN", "RUSSIANPUNKTUATION").ToUpper() == "NO")
                        russianpunktuation_flag = false;
                }

                if (cw_win_ini_file.GetSetting("CW_WIN", "ALTERNATIVEMP3").ToUpper() == "YES")
                    alternative_mp3_flag = true;
                else
                {
                    if (cw_win_ini_file.GetSetting("CW_WIN", "ALTERNATIVEMP3").ToUpper() == "NO")
                        alternative_mp3_flag = false;
                }

                if (cw_win_ini_file.GetSetting("CW_WIN", "NONRANDOM").ToUpper() == "YES")
                    non_random_flag = true;
                else
                {
                    if (cw_win_ini_file.GetSetting("CW_WIN", "NONRANDOM").ToUpper() == "NO")
                        non_random_flag = false;
                }


                if (cw_win_ini_file.GetSetting("CW_WIN", "ENGLISH").ToUpper() == "YES")
                    english_flag = true;
                else
                {
                    if (cw_win_ini_file.GetSetting("CW_WIN", "ENGLISH").ToUpper() == "NO")
                        english_flag = false;
                }

                if (int.Parse(cw_win_ini_file.GetSetting("CW_WIN", "STARTPAUSE")) > 0)
                {
                    startpause = int.Parse(cw_win_ini_file.GetSetting("CW_WIN", "STARTPAUSE"));
                    startpause_flag = true;
                }
                else
                {
                    if (int.Parse(cw_win_ini_file.GetSetting("CW_WIN", "STARTPAUSE")) == 0)
                    {
                        startpause = int.Parse(cw_win_ini_file.GetSetting("CW_WIN", "STARTPAUSE"));
                        startpause_flag = false;
                    }
                }

                speed_calibr = int.Parse(cw_win_ini_file.GetSetting("CW_WIN", "SPEEDCALIBR"));
                numberofwords = int.Parse(cw_win_ini_file.GetSetting("CW_WIN", "NUMBEROFWORDS"));
                speed = int.Parse(cw_win_ini_file.GetSetting("CW_WIN", "SPEED"));
                tone = int.Parse(cw_win_ini_file.GetSetting("CW_WIN", "TONE"));
                interval = int.Parse(cw_win_ini_file.GetSetting("CW_WIN", "INTERVAL"));
                return;
            }
            catch (Exception)
            {
                if (english_flag)
                    MessageBox.Show("Error read parametrs from INI-file. Save options again.");
                else
                    MessageBox.Show("Ошибка чтения параметров из INI-файла. Cохраните снова настройки.");
            }
        } 

        public void LabelAndTrackbarShow()
            {
                label_Show_N.Text = numberofwords.ToString();   // Вывести на индикатор
                trackBar_N.Value = numberofwords;               // Выставить движок

                label_Show_Speed.Text = speed.ToString();
                trackBar_Speed.Value = speed;

                label_Show_Tone.Text = tone.ToString();     
                trackBar_Tone.Value = tone;                 

                label_Show_Pause.Text = interval.ToString();
                trackBar_Pause.Value = interval;

                label_StartPause.Text = startpause.ToString();
                if (startpause_flag)
                    checkBox_StartPause.Checked = true;
                else
                    checkBox_StartPause.Checked = false;
            }

        private void button_ToMp3_Click(object sender, EventArgs e)
        {
            StartWriteMP3();
        }

        // Создание звуковых файлов
        private void StartWriteMP3()
        {
            // Если INI-файл существует, то читаем из него параметры
            if (exist_ini_file)
            {
                ReadIniFile(cw_win_user_folder, ini_file_name);
            }

            // Проверяем открыт ли какой-нибудь файл
            if (openFileDialog1.FileName.Length < 1)
            {
                if (english_flag)
                    label_OutText.Text = "No file is selected with CW-words.";
                else
                    label_OutText.Text = "Не выбран файл с CW-словами.";
                return;
            }
            
            // Выводим имя файла на форму
            label_FileName.Text = openFileDialog1.SafeFileName;
            label_FileName.Update();

            // Изменяем доступность кнопок
            button_StartStop.Enabled = false;
            button_Exit.Enabled = false;
            button_Config.Enabled = false;
            button_OpenFile.Enabled = false;
            button_SafeFile.Enabled = false;
            button_ToMp3.Enabled = false;
            button_Help.Enabled = false;
            button_About.Enabled = false;

            // Очистим поле вывода слов
            label_OutText.Text = "";
            label_OutText.Update();

            // Запускаем синтез WAV-файла из CW-слов
            WordToWAV();

            // Возвращение доступности кнопок
            button_StartStop.Enabled = true;
            button_Exit.Enabled = true;
            button_Config.Enabled = true;
            button_OpenFile.Enabled = true;
            button_SafeFile.Enabled = true;
            button_ToMp3.Enabled = true;
            button_Help.Enabled = true;
            button_About.Enabled = true;

            // Проверить, может ли кнопка "Старт" получить фокус,
            // если да, передать ей фокус.
            if (button_StartStop.CanFocus)
                button_StartStop.Focus();
        }

        // Генерация WAV-файла
        public void WordToWAV()
        { 
            // Считываем параметры с движков регуляторов
            numberofwords = trackBar_N.Value;
            interval = trackBar_Pause.Value;
            speed = trackBar_Speed.Value;
            tone = trackBar_Tone.Value;

            // Выбираем папку для звуковых файлов
            
            string fileName = GetAudioFolderName();
            string fileNameOld = fileName;      // Сохраним для переноса файлов из корня (в WinXP)
            

            if (fileName == "cancel")   // Если папка не выбрана, то выходим
                return;

            if (alternative_mp3_flag)    // В Windows-XP сохраняем в корень диска
            {

                fileName = fileName.Remove(fileName.IndexOf(":")+1);
            }

            // Текущая дата
            string currentDate = DateTime.Now.Year.ToString() + 
                                 DateTime.Now.Month.ToString() +
                                 DateTime.Now.Day.ToString();
            
            string currentTime = DateTime.Now.Hour.ToString() + 
                                 DateTime.Now.Minute.ToString() +
                                 DateTime.Now.Second.ToString();
            
            // Добавляем дату и время к именам звуковых файлов
            string audioFileName = originalAudioFileName + "-" + currentDate + currentTime;

            // Добавляем к папке имя звуковых файлов
            fileName = fileName + audioFileName;
            fileNameOld = fileNameOld + audioFileName; 

            // Выставить параметры WAV-файла
            WaveFormat waveFormat = new WaveFormat(8000, 16, 2);
            int one_milli_second = 2 * waveFormat.SampleRate / 1000;
            float amplitude = 0.25f;
            float frequency = tone;

            using (WaveFileWriter writer = new WaveFileWriter(fileName + ".wav", waveFormat))
            {
                // Воспроизведение слова
                int dot = speed_calibr * one_milli_second / speed;    // Длительность точки 
                
                int dash = 3 * dot;                // Длительность тире
                int dash_conjoint = dash;          // Промежуток между слитными буквами типа <KN>

                // Если стоит "галка", то использовать паузу перед началом
                if (checkBox_StartPause.Checked)
                {
                    startpause_flag = true;                 // Использовать паузу
                    WavPause(writer, startpause * 1000);    // Задержка перед началом передачи
                }

                // Выводим случайные слова из массива
                Random rnd = new Random();

                if (non_random_flag)
                {
                    numberofwords = AllWords.Length;        // При отмене случайного выбора слов выводим все слова
                }                                           // один раз

                for (int i = 1; i <= numberofwords; i++)
                {
                    int num;

                    if (!non_random_flag)
                    {
                        num = rnd.Next(0, AllWords.Length);     // Номер случайного слова
                    }
                    else
                    {
                        num = i - 1;
                    }

                    string word = AllWords[num].ToUpper();      // Получаем случайное слово в верхнем 
                    // регистре
                    for (int j = 0; j < word.Length; j++)       // Перебираем буквы в слове
                    {
                        // Воспроизводим букву
                        char symbol = word[j];

                        // Слитные буквы для <AS> и т.п.
                        if (symbol == '<') 
                            dash_conjoint = dot;                // "<" - слитное слово
                        if (symbol == '>')
                        {
                            dash_conjoint = dash;			    // ">" - раздельное слово
                            WavPause(writer, dash_conjoint);
                        }

                        // Цифры
                        if (symbol == '0')
                        {
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency);
                            WavPause(writer, dash_conjoint);
                        }

                        if (symbol == '1')
                        {
                            WavPlay(writer, dot, amplitude, frequency);  WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency);
                            WavPause(writer, dash_conjoint);
                        }

                        if (symbol == '2')
                        {
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency);
                            WavPause(writer, dash_conjoint);
                        }

                        if (symbol == '3')
                        {
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency);
                            WavPause(writer, dash_conjoint);
                        }

                        if (symbol == '4')
                        {
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency);
                            WavPause(writer, dash_conjoint);
                        }

                        if (symbol == '5')
                        {
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency);
                            WavPause(writer, dash_conjoint);
                        }

                        if (symbol == '6')
                        {
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency);
                            WavPause(writer, dash_conjoint);
                        }

                        if (symbol == '7')
                        {
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency);
                            WavPause(writer, dash_conjoint);
                        }

                        if (symbol == '8')
                        {
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency);
                            WavPause(writer, dash_conjoint);
                        }

                        if (symbol == '9')
                        {
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency);
                            WavPause(writer, dash_conjoint);
                        }

                        // Буквы
                        if (symbol == 'A' || symbol == 'А')
                        {
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency);
                            WavPause(writer, dash_conjoint);
                        }

                        if (symbol == 'B' || symbol == 'Б')
                        {
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency);
                            WavPause(writer, dash_conjoint);
                        }

                        if (symbol == 'C' || symbol == 'Ц')
                        {
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency);
                            WavPause(writer, dash_conjoint);
                        }

                        if (symbol == 'D' || symbol == 'Д')
                        {
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency);
                            WavPause(writer, dash_conjoint);
                        }

                        if (symbol == 'E' || symbol == 'Е' || symbol == 'Ё')
                        {
                            WavPlay(writer, dot, amplitude, frequency);
                            WavPause(writer, dash_conjoint);
                        }

                        if (symbol == 'F' || symbol == 'Ф')
                        {
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency);
                            WavPause(writer, dash_conjoint);
                        }

                        if (symbol == 'G' || symbol == 'Г')
                        {
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency);
                            WavPause(writer, dash_conjoint);
                        }

                        if (symbol == 'H' || symbol == 'Х')
                        {
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency);
                            WavPause(writer, dash_conjoint);
                        }

                        if (symbol == 'I' || symbol == 'И')
                        {
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency);
                            WavPause(writer, dash_conjoint);
                        }

                        if (symbol == 'J' || symbol == 'Й')
                        {
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency);
                            WavPause(writer, dash_conjoint);
                        }

                        if (symbol == 'K' || symbol == 'К')
                        {
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency);
                            WavPause(writer, dash_conjoint);
                        }

                        if (symbol == 'L' || symbol == 'Л')
                        {
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency);
                            WavPause(writer, dash_conjoint);
                        }

                        if (symbol == 'M' || symbol == 'М')
                        {
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency);
                            WavPause(writer, dash_conjoint);
                        }

                        if (symbol == 'N' || symbol == 'Н')
                        {
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency);
                            WavPause(writer, dash_conjoint);
                        }

                        if (symbol == 'O' || symbol == 'О')
                        {
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency);
                            WavPause(writer, dash_conjoint);
                        }

                        if (symbol == 'P' || symbol == 'П')
                        {
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency);
                            WavPause(writer, dash_conjoint);
                        }

                        if (symbol == 'Q' || symbol == 'Щ')
                        {
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency);
                            WavPause(writer, dash_conjoint);
                        }

                        if (symbol == 'R' || symbol == 'Р')
                        {
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency);
                            WavPause(writer, dash_conjoint);
                        }

                        if (symbol == 'S' || symbol == 'С')
                        {
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency);
                            WavPause(writer, dash_conjoint);
                        }

                        if (symbol == 'T' || symbol == 'Т')
                        {
                            WavPlay(writer, dash, amplitude, frequency);
                            WavPause(writer, dash_conjoint);
                        }

                        if (symbol == 'U' || symbol == 'У')
                        {
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency);
                            WavPause(writer, dash_conjoint);
                        }

                        if (symbol == 'V' || symbol == 'Ж')
                        {
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency);
                            WavPause(writer, dash_conjoint);
                        }

                        if (symbol == 'W' || symbol == 'В')
                        {
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency);
                            WavPause(writer, dash_conjoint);
                        }

                        if (symbol == 'X' || symbol == 'Ь')
                        {
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency);
                            WavPause(writer, dash_conjoint);
                        }

                        if (symbol == 'Y' || symbol == 'Ы')
                        {
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency);
                            WavPause(writer, dash_conjoint);
                        }

                        if (symbol == 'Z' || symbol == 'З')
                        {
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency);
                            WavPause(writer, dash_conjoint);
                        }

                        if (symbol == 'Ч')
                        {
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency);
                            WavPause(writer, dash_conjoint);
                        }

                        if (symbol == 'Ш')
                        {
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency);
                            WavPause(writer, dash_conjoint);
                        }

                        if (symbol == 'Э')
                        {
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency);
                            WavPause(writer, dash_conjoint);
                        }

                        if (symbol == 'Ю')
                        {
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency);
                            WavPause(writer, dash_conjoint);
                        }

                        if (symbol == 'Я')
                        {
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency);
                            WavPause(writer, dash_conjoint);
                        }

                        // Символы
                        if (symbol == '=')
                        {
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency);
                            Thread.Sleep(dash);
                        }

                        if (symbol == '?')
                        {
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency);
                            Thread.Sleep(dash);
                        }

                        if (symbol == '/')
                        {
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency);
                            Thread.Sleep(dash);
                        }

                        if (symbol == '@')
                        {
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency);
                            Thread.Sleep(dash);
                        }

                        if (symbol == '.' && russianpunktuation_flag == true)
                        {
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency);
                            Thread.Sleep(dash);
                        }

                        if (symbol == '.' && russianpunktuation_flag == false)
                        {
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency);
                            Thread.Sleep(dash);
                        }

                        if (symbol == ',' && russianpunktuation_flag == true)
                        {
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency);
                            Thread.Sleep(dash);
                        }

                        if (symbol == ',' && russianpunktuation_flag == false)
                        {
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency);
                            Thread.Sleep(dash);
                        }

                        if (symbol == ';')
                        {
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency);
                            Thread.Sleep(dash);
                        }

                        if (symbol == ':')
                        {
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency);
                            Thread.Sleep(dash);
                        }

                        if (symbol == '!' && russianpunktuation_flag == true)
                        {
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency);
                            Thread.Sleep(dash);
                        }

                        if (symbol == '!' && russianpunktuation_flag == false)
                        {
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency);
                            Thread.Sleep(dash);
                        }

                        if (symbol == '-')
                        {
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency);
                            Thread.Sleep(dash);
                        }

                        if (symbol == '\'')
                        {
                            WavPlay(writer, dot, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dash, amplitude, frequency); WavPause(writer, dot);
                            WavPlay(writer, dot, amplitude, frequency);
                            Thread.Sleep(dash);
                        }

                    } // End For j
  
                    
                    if (non_random_flag)
                    {
                        screen_Out(word);               // вывод на label непрерывно
                    }
                    else
                    {
                        screen_Out_By_10(i, word);      // вывод на label по 10 слов
                    }
                    

                    WavPause(writer, interval * dash);  // Пауза между словами

                } // End for(i)

                writer.Close();
                writer.Dispose();

            } // end using


            // Конвертим WAV-файл в MP3-файл
            if (File.Exists(fileName + ".wav"))
                ConvertWavToMP3(fileName);
            else
            {
                if (english_flag)
                    MessageBox.Show("No exist WAV-file to conver in MP3.");
                else
                    MessageBox.Show(fileName + ".wav ", "Нет WAV-файла для конвертации в MP3.");                
            }
            // Переместить звуковые файлы
            if (alternative_mp3_flag)
            {
                try
                {
                    // MessageBox.Show("Конвертим с задержкой MP3.");
                    // Не успевает LAME отрабоать - заддерка нужна
                    Thread.Sleep(5000);     // 5 секунд

                    if (File.Exists(fileName + ".mp3"))
                    {
                        // Удаляем старые файлы
                        if (File.Exists(fileNameOld + ".mp3 "))
                            File.Delete(fileNameOld + ".mp3 ");

                        //Переносим файлы из корня диска в папку, указанную пользователем
                        File.Move(fileName + ".mp3 ", fileNameOld + ".mp3");
                    }
                    else
                        MessageBox.Show(fileName + ".mp3 ", "Нет MP3-файла!");

                    if (File.Exists(fileName + ".wav"))
                    {
                        //MessageBox.Show(fileName + ".wav ", "Старый файл WAV");
                        //MessageBox.Show(fileNameOld + ".wav ", "Новый файл WAV");
                       // MessageBox.Show(fileName + ".mp3 ", "Старый файл MP3");
                       // MessageBox.Show(fileNameOld + ".mp3 ", "Новый файл MP3");
                        
                        // Удаляем старые файлы
                        if (File.Exists(fileNameOld + ".wav "))
                            File.Delete(fileNameOld + ".wav ");
                        
                        //Переносим файлы из корня диска в папку, указанную пользователем
                        File.Move(fileName + ".wav ", fileNameOld + ".wav");
                    }

                   
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString(), "The process move a audiofiles failed!");
                }
            }
        }

        // Сгенерируем Сэмпл определённой длительности, амплитуды и частоты и запишем в звуковой файл
        private void WavPlay(WaveFileWriter writer, int millisec, float amplitude, float frequency)
        {
            for (int n = 0; n < millisec; n++)
            {
               float sample = (float)(amplitude * Math.Sin((Math.PI * n * frequency) / writer.WaveFormat.SampleRate));
               writer.WriteSample(sample);
            }
            return;
        }

        // Сгенерируем Сэмпл определённой длительности, но с нулевой амплитудой и частотой (пауза) и
        // запишем в звуковой файл
        private void WavPause(WaveFileWriter writer, int millisec)
        {
            float amplitude = 0;
            float frequency = 0;

            for (int n = 0; n < millisec; n++)
            {
                float sample = (float)(amplitude * Math.Sin((Math.PI * n * frequency) / writer.WaveFormat.SampleRate));
                writer.WriteSample(sample);
            }
            return;
        }

        // Сконвертируем WAV-файл в MP3-файл с помощью LAME.EXE с параметрами
        private void ConvertWavToMP3(string filename)
        {
            // Проверим есть ли LAME.EXE в текущей папке
            // Текущая папка программы
            string lame_file_name = "lame.exe";
            string current_app_path = Environment.CurrentDirectory;

            // Существует ли файл LAME.EXE?
            FileInfo file = new FileInfo(Path.Combine(current_app_path, lame_file_name));
            if (!file.Exists)
            {
                if (english_flag)
                    MessageBox.Show("No LAME.EXE file in the folder with the program.");
                else
                    MessageBox.Show("Нет файла LAME.EXE в папке с программой.");
                return;
            }

            //MessageBox.Show(Path.Combine(current_app_path, lame_file_name), "Путь и имя ЛАМЕ");

                Process pr = new Process();
            /*
                pr.StartInfo.FileName = "cmd.exe ";
                pr.StartInfo.Arguments = "/k " + lame_file_name + " -m m -V6 --ta R5AM --tt R5AM c:\\cw_audio.wav c:\\cw_audio.mp3";
                // В спрятанном виде сонсольное приложение запустить
                //   pr.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            */
                pr.StartInfo.FileName = Path.Combine(current_app_path, lame_file_name) + " ";
                pr.StartInfo.Arguments = "-m m -V6 --ta R5AM --tt R5AM --silent " +
                                         filename + ".wav " +
                                         filename + ".mp3";
                // В спрятанном виде консольное прилоение запустить
                pr.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                pr.Start();
        }

        // Получаем имя директории для звуковых файлов
        private string GetAudioFolderName()
        {
            // Путь к файлу по умолчанию "Моя музыка" 
            folderBrowserDialog1.RootFolder = Environment.SpecialFolder.MyMusic;

            if (english_flag)
                folderBrowserDialog1.Description =
                    "Select the directory that you want to use for audio files.";
            else
                folderBrowserDialog1.Description =
                    "Выберите папку для сохранения звуковых файлов.";
            
            DialogResult FolderDialogResult = folderBrowserDialog1.ShowDialog();
            
            string DirectoriaFilePlace = folderBrowserDialog1.SelectedPath;

           // MessageBox.Show(DirectoriaFilePlace);

            if (DirectoriaFilePlace != String.Empty)  // Если путь к файлу не пустой
            {
                if (FolderDialogResult == DialogResult.OK)
                {
                    return DirectoriaFilePlace;
                }
                else
                {
                    if (english_flag) 
                        label_OutText.Text = "Path not selected.";
                    else
                        label_OutText.Text = "Путь не выбран.";

                    return "cancel";
                }
            }
            else
                return "cancel";
        }

        // Если поставили "Инфинити-галку", то работаем "бесконечно" - год
        private void checkBox_infinity_CheckedChanged(object sender, EventArgs e)
        {
            if(infinity_work_flag)
                infinity_work_flag = false;
            else
                infinity_work_flag = true;
        }
   }
}
