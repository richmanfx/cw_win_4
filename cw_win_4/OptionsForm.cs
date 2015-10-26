using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using INI;
using System.Threading;
using System.Globalization;

namespace cw_win_4
{
    public partial class ConfigForm : Form
    {
        public ConfigForm()
        {
            // Установка UI culture в Английский).
            if (Cw_winForm.english_flag)
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");
            else
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru");

            InitializeComponent();
            
            // Заполняем текстбоксы значениями параметров из главной формы
            checkBox_Config_RusPunktuation.Checked = Cw_winForm.russianpunktuation_flag;
            checkBox_Config_EngInterface.Checked = Cw_winForm.english_flag;
            checkBox_Config_mp3.Checked = Cw_winForm.alternative_mp3_flag;
            checkBox_Config_non_random.Checked = Cw_winForm.non_random_flag;

            textBox_Config_StartPause.Text = Cw_winForm.startpause.ToString();
            textBox_Config_Calibr.Text = Cw_winForm.speed_calibr.ToString();

            textBox_Config_N_Word.Text = Cw_winForm.numberofwords.ToString();
            textBox_Config_Speed.Text = Cw_winForm.speed.ToString();
            textBox_Config_Tone.Text = Cw_winForm.tone.ToString();
            textBox_Config_Interval.Text = Cw_winForm.interval.ToString();

        }

        private void button_ExitConfig_Click(object sender, EventArgs e)
        {
            Close();    // Закрываем форму
        }

        private void button_Config_Save_Click(object sender, EventArgs e)
        {
            // Путь к папке файлов приложений пользователя
            string user_app_path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            // Проверить существование папки
            DirectoryInfo folder = new DirectoryInfo(Path.Combine(user_app_path, 
                                                     Cw_winForm.cw_win_user_folder));
            if (!folder.Exists)
            {
                // Создаём папку
                folder.Create();
            }

            // Существует ли файл?
            string full_path_and_name = Path.Combine(user_app_path, Cw_winForm.cw_win_user_folder,
                                                                    Cw_winForm.ini_file_name);
            if (!File.Exists(full_path_and_name))
            {
                // Создаём INI-файл
                //File.Create(full_path_and_name);
                FileStream fs = File.Create(full_path_and_name);
                fs.Close();
            }

            // Открываем и считываем настройки из INI-файла
             ini_parser cw_win_ini_file = new ini_parser(full_path_and_name);
            

            /// Параметры для записи в файл
            
               // Использовать русские знаки пунктуации
             if (checkBox_Config_RusPunktuation.Checked)
             {
                 cw_win_ini_file.AddSetting("CW_WIN", "RUSSIANPUNKTUATION", "YES");
                 Cw_winForm.russianpunktuation_flag = true;
             }
             else
             {
                 cw_win_ini_file.AddSetting("CW_WIN", "RUSSIANPUNKTUATION", "NO");
                 Cw_winForm.russianpunktuation_flag = false;
             }

             // Использовать английский интерфейс
             if (checkBox_Config_EngInterface.Checked)
             {
                 cw_win_ini_file.AddSetting("CW_WIN", "ENGLISH", "YES");
                 Cw_winForm.english_flag = true;
             }
             else
             {
                 cw_win_ini_file.AddSetting("CW_WIN", "ENGLISH", "NO");
                 Cw_winForm.english_flag = false;
             }

             // Использовать альтернативные пути для звуковых файлов
             if (checkBox_Config_mp3.Checked)
             {
                 cw_win_ini_file.AddSetting("CW_WIN", "ALTERNATIVEMP3", "YES");
                 Cw_winForm.alternative_mp3_flag = true;
             }
             else
             {
                 cw_win_ini_file.AddSetting("CW_WIN", "ALTERNATIVEMP3", "NO");
                 Cw_winForm.alternative_mp3_flag = false;
             }
            
             // Отключить случайный вывод слов
             if (checkBox_Config_non_random.Checked)
             {
                 cw_win_ini_file.AddSetting("CW_WIN", "NONRANDOM", "YES");
                 Cw_winForm.non_random_flag = true;
             }
             else
             {
                 cw_win_ini_file.AddSetting("CW_WIN", "NONRANDOM", "NO");
                 Cw_winForm.non_random_flag = false;
             }


               // Пауза перед стартом
             cw_win_ini_file.AddSetting("CW_WIN", "STARTPAUSE", textBox_Config_StartPause.Text);
             Cw_winForm.startpause = int.Parse(textBox_Config_StartPause.Text);
              

               // Значение калибровки скорости
             cw_win_ini_file.AddSetting("CW_WIN", "SPEEDCALIBR", textBox_Config_Calibr.Text);
             Cw_winForm.speed_calibr = int.Parse(textBox_Config_Calibr.Text);


               // Количество слов
             cw_win_ini_file.AddSetting("cw_win", "NUMBEROFWORDS", textBox_Config_N_Word.Text);
            // Form1.numberofwords = int.Parse(textBox_Config_N_Word.Text);
               
               // Скорость
             cw_win_ini_file.AddSetting("cw_win", "SPEED", textBox_Config_Speed.Text);
            // Form1.speed = int.Parse(textBox_Config_Speed.Text);
             
               // Тон
             cw_win_ini_file.AddSetting("cw_win", "TONE", textBox_Config_Tone.Text);
            // Form1.tone = int.Parse(textBox_Config_Tone.Text);
             
               // Интервал между словами
             cw_win_ini_file.AddSetting("cw_win", "INTERVAL", textBox_Config_Interval.Text);
             //Form1.interval = int.Parse(textBox_Config_Interval.Text);

            // Сохранить настройки и переписать INI-файл 
            cw_win_ini_file.SaveSettings();
            // label_SaveMessage.Text = "Настройки сохранены";

            

            if (Cw_winForm.english_flag)
                label_SaveMessage.Text = "The settings are saved.\nRestart the program.";
            else
                label_SaveMessage.Text = "Настройки сохранены.\nПерезапустите программу.";
            return;
        }
    }
}
