namespace cw_win_4
{
    partial class Cw_winForm
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Cw_winForm));
            this.button_About = new System.Windows.Forms.Button();
            this.label_OutText = new System.Windows.Forms.Label();
            this.button_StartStop = new System.Windows.Forms.Button();
            this.button_Help = new System.Windows.Forms.Button();
            this.button_Exit = new System.Windows.Forms.Button();
            this.button_Config = new System.Windows.Forms.Button();
            this.button_OpenFile = new System.Windows.Forms.Button();
            this.button_SafeFile = new System.Windows.Forms.Button();
            this.button_ToMp3 = new System.Windows.Forms.Button();
            this.trackBar_N = new System.Windows.Forms.TrackBar();
            this.trackBar_Speed = new System.Windows.Forms.TrackBar();
            this.trackBar_Pause = new System.Windows.Forms.TrackBar();
            this.trackBar_Tone = new System.Windows.Forms.TrackBar();
            this.checkBox_StartPause = new System.Windows.Forms.CheckBox();
            this.label_N = new System.Windows.Forms.Label();
            this.label_Speed = new System.Windows.Forms.Label();
            this.label_Pause = new System.Windows.Forms.Label();
            this.label_Tone = new System.Windows.Forms.Label();
            this.groupBox_regul = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label_Show_Pause = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label_Show_Speed = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label_Show_Tone = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label_Show_N = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.label_FileName = new System.Windows.Forms.Label();
            this.label_CopyRight = new System.Windows.Forms.Label();
            this.groupBox_StartPause = new System.Windows.Forms.GroupBox();
            this.checkBox_infinity = new System.Windows.Forms.CheckBox();
            this.label_StartPause = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_N)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Speed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Pause)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Tone)).BeginInit();
            this.groupBox_regul.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox_StartPause.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_About
            // 
            resources.ApplyResources(this.button_About, "button_About");
            this.button_About.Name = "button_About";
            this.button_About.UseVisualStyleBackColor = true;
            this.button_About.Click += new System.EventHandler(this.button_About_Click);
            // 
            // label_OutText
            // 
            this.label_OutText.BackColor = System.Drawing.Color.DarkCyan;
            this.label_OutText.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.label_OutText, "label_OutText");
            this.label_OutText.ForeColor = System.Drawing.Color.White;
            this.label_OutText.Name = "label_OutText";
            // 
            // button_StartStop
            // 
            resources.ApplyResources(this.button_StartStop, "button_StartStop");
            this.button_StartStop.ForeColor = System.Drawing.Color.Yellow;
            this.button_StartStop.Name = "button_StartStop";
            this.button_StartStop.Tag = "";
            this.button_StartStop.UseVisualStyleBackColor = true;
            this.button_StartStop.Click += new System.EventHandler(this.button_StartStop_Click);
            // 
            // button_Help
            // 
            resources.ApplyResources(this.button_Help, "button_Help");
            this.button_Help.Name = "button_Help";
            this.button_Help.UseVisualStyleBackColor = true;
            this.button_Help.Click += new System.EventHandler(this.button_Help_Click);
            // 
            // button_Exit
            // 
            resources.ApplyResources(this.button_Exit, "button_Exit");
            this.button_Exit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.button_Exit.Name = "button_Exit";
            this.button_Exit.UseVisualStyleBackColor = true;
            this.button_Exit.Click += new System.EventHandler(this.button_Exit_Click);
            // 
            // button_Config
            // 
            resources.ApplyResources(this.button_Config, "button_Config");
            this.button_Config.Name = "button_Config";
            this.button_Config.UseVisualStyleBackColor = true;
            this.button_Config.Click += new System.EventHandler(this.button_Config_Click);
            // 
            // button_OpenFile
            // 
            resources.ApplyResources(this.button_OpenFile, "button_OpenFile");
            this.button_OpenFile.Name = "button_OpenFile";
            this.button_OpenFile.UseVisualStyleBackColor = true;
            this.button_OpenFile.Click += new System.EventHandler(this.button_OpenFile_Click);
            // 
            // button_SafeFile
            // 
            resources.ApplyResources(this.button_SafeFile, "button_SafeFile");
            this.button_SafeFile.Name = "button_SafeFile";
            this.button_SafeFile.UseVisualStyleBackColor = true;
            this.button_SafeFile.Click += new System.EventHandler(this.button_SafeFile_Click);
            // 
            // button_ToMp3
            // 
            resources.ApplyResources(this.button_ToMp3, "button_ToMp3");
            this.button_ToMp3.Name = "button_ToMp3";
            this.button_ToMp3.UseVisualStyleBackColor = true;
            this.button_ToMp3.Click += new System.EventHandler(this.button_ToMp3_Click);
            // 
            // trackBar_N
            // 
            this.trackBar_N.BackColor = System.Drawing.Color.Teal;
            this.trackBar_N.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.trackBar_N.LargeChange = 10;
            resources.ApplyResources(this.trackBar_N, "trackBar_N");
            this.trackBar_N.Maximum = 1000;
            this.trackBar_N.Name = "trackBar_N";
            this.trackBar_N.TickFrequency = 100;
            this.trackBar_N.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trackBar_N.Scroll += new System.EventHandler(this.trackBar_N_Scroll);
            // 
            // trackBar_Speed
            // 
            this.trackBar_Speed.BackColor = System.Drawing.Color.Teal;
            this.trackBar_Speed.Cursor = System.Windows.Forms.Cursors.SizeWE;
            resources.ApplyResources(this.trackBar_Speed, "trackBar_Speed");
            this.trackBar_Speed.Maximum = 250;
            this.trackBar_Speed.Minimum = 50;
            this.trackBar_Speed.Name = "trackBar_Speed";
            this.trackBar_Speed.SmallChange = 5;
            this.trackBar_Speed.TickFrequency = 10;
            this.trackBar_Speed.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trackBar_Speed.Value = 50;
            this.trackBar_Speed.Scroll += new System.EventHandler(this.trackBar_Speed_Scroll);
            // 
            // trackBar_Pause
            // 
            this.trackBar_Pause.BackColor = System.Drawing.Color.Teal;
            this.trackBar_Pause.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.trackBar_Pause.LargeChange = 1;
            resources.ApplyResources(this.trackBar_Pause, "trackBar_Pause");
            this.trackBar_Pause.Maximum = 20;
            this.trackBar_Pause.Minimum = 1;
            this.trackBar_Pause.Name = "trackBar_Pause";
            this.trackBar_Pause.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trackBar_Pause.Value = 1;
            this.trackBar_Pause.Scroll += new System.EventHandler(this.trackBar_Pause_Scroll);
            // 
            // trackBar_Tone
            // 
            this.trackBar_Tone.BackColor = System.Drawing.Color.Teal;
            this.trackBar_Tone.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.trackBar_Tone.LargeChange = 50;
            resources.ApplyResources(this.trackBar_Tone, "trackBar_Tone");
            this.trackBar_Tone.Maximum = 2000;
            this.trackBar_Tone.Minimum = 300;
            this.trackBar_Tone.Name = "trackBar_Tone";
            this.trackBar_Tone.SmallChange = 10;
            this.trackBar_Tone.TickFrequency = 100;
            this.trackBar_Tone.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trackBar_Tone.Value = 300;
            this.trackBar_Tone.Scroll += new System.EventHandler(this.trackBar_Tone_Scroll);
            // 
            // checkBox_StartPause
            // 
            resources.ApplyResources(this.checkBox_StartPause, "checkBox_StartPause");
            this.checkBox_StartPause.ForeColor = System.Drawing.Color.White;
            this.checkBox_StartPause.Name = "checkBox_StartPause";
            this.checkBox_StartPause.UseVisualStyleBackColor = true;
            // 
            // label_N
            // 
            resources.ApplyResources(this.label_N, "label_N");
            this.label_N.Name = "label_N";
            // 
            // label_Speed
            // 
            resources.ApplyResources(this.label_Speed, "label_Speed");
            this.label_Speed.Name = "label_Speed";
            // 
            // label_Pause
            // 
            resources.ApplyResources(this.label_Pause, "label_Pause");
            this.label_Pause.Name = "label_Pause";
            // 
            // label_Tone
            // 
            resources.ApplyResources(this.label_Tone, "label_Tone");
            this.label_Tone.Name = "label_Tone";
            // 
            // groupBox_regul
            // 
            this.groupBox_regul.Controls.Add(this.groupBox4);
            this.groupBox_regul.Controls.Add(this.groupBox3);
            this.groupBox_regul.Controls.Add(this.groupBox2);
            resources.ApplyResources(this.groupBox_regul, "groupBox_regul");
            this.groupBox_regul.Name = "groupBox_regul";
            this.groupBox_regul.TabStop = false;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label_Show_Pause);
            this.groupBox4.Controls.Add(this.label_Pause);
            this.groupBox4.Controls.Add(this.trackBar_Pause);
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // label_Show_Pause
            // 
            this.label_Show_Pause.BackColor = System.Drawing.Color.DarkSlateGray;
            this.label_Show_Pause.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.label_Show_Pause, "label_Show_Pause");
            this.label_Show_Pause.ForeColor = System.Drawing.Color.Yellow;
            this.label_Show_Pause.Name = "label_Show_Pause";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label_Show_Speed);
            this.groupBox3.Controls.Add(this.trackBar_Speed);
            this.groupBox3.Controls.Add(this.label_Speed);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // label_Show_Speed
            // 
            this.label_Show_Speed.BackColor = System.Drawing.Color.DarkSlateGray;
            this.label_Show_Speed.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.label_Show_Speed, "label_Show_Speed");
            this.label_Show_Speed.ForeColor = System.Drawing.Color.Yellow;
            this.label_Show_Speed.Name = "label_Show_Speed";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label_Tone);
            this.groupBox2.Controls.Add(this.label_Show_Tone);
            this.groupBox2.Controls.Add(this.trackBar_Tone);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // label_Show_Tone
            // 
            this.label_Show_Tone.BackColor = System.Drawing.Color.DarkSlateGray;
            this.label_Show_Tone.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.label_Show_Tone, "label_Show_Tone");
            this.label_Show_Tone.ForeColor = System.Drawing.Color.Yellow;
            this.label_Show_Tone.Name = "label_Show_Tone";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label_Show_N);
            this.groupBox1.Controls.Add(this.label_N);
            this.groupBox1.Controls.Add(this.trackBar_N);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label_Show_N
            // 
            this.label_Show_N.BackColor = System.Drawing.Color.DarkSlateGray;
            this.label_Show_N.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.label_Show_N, "label_Show_N");
            this.label_Show_N.ForeColor = System.Drawing.Color.Yellow;
            this.label_Show_N.Name = "label_Show_N";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "txt";
            resources.ApplyResources(this.openFileDialog1, "openFileDialog1");
            this.openFileDialog1.SupportMultiDottedExtensions = true;
            // 
            // label_FileName
            // 
            resources.ApplyResources(this.label_FileName, "label_FileName");
            this.label_FileName.ForeColor = System.Drawing.Color.Lime;
            this.label_FileName.Name = "label_FileName";
            // 
            // label_CopyRight
            // 
            resources.ApplyResources(this.label_CopyRight, "label_CopyRight");
            this.label_CopyRight.Name = "label_CopyRight";
            // 
            // groupBox_StartPause
            // 
            this.groupBox_StartPause.Controls.Add(this.checkBox_StartPause);
            this.groupBox_StartPause.Controls.Add(this.checkBox_infinity);
            this.groupBox_StartPause.Controls.Add(this.label_StartPause);
            resources.ApplyResources(this.groupBox_StartPause, "groupBox_StartPause");
            this.groupBox_StartPause.Name = "groupBox_StartPause";
            this.groupBox_StartPause.TabStop = false;
            // 
            // checkBox_infinity
            // 
            resources.ApplyResources(this.checkBox_infinity, "checkBox_infinity");
            this.checkBox_infinity.ForeColor = System.Drawing.Color.White;
            this.checkBox_infinity.Name = "checkBox_infinity";
            this.checkBox_infinity.UseVisualStyleBackColor = true;
            this.checkBox_infinity.CheckedChanged += new System.EventHandler(this.checkBox_infinity_CheckedChanged);
            // 
            // label_StartPause
            // 
            this.label_StartPause.BackColor = System.Drawing.Color.DarkSlateGray;
            this.label_StartPause.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            resources.ApplyResources(this.label_StartPause, "label_StartPause");
            this.label_StartPause.ForeColor = System.Drawing.Color.Yellow;
            this.label_StartPause.Name = "label_StartPause";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label_FileName);
            resources.ApplyResources(this.groupBox5, "groupBox5");
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.TabStop = false;
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.CheckPathExists = false;
            resources.ApplyResources(this.saveFileDialog1, "saveFileDialog1");
            // 
            // Cw_winForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Teal;
            this.Controls.Add(this.groupBox_StartPause);
            this.Controls.Add(this.label_CopyRight);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button_ToMp3);
            this.Controls.Add(this.button_SafeFile);
            this.Controls.Add(this.button_OpenFile);
            this.Controls.Add(this.button_Config);
            this.Controls.Add(this.button_Exit);
            this.Controls.Add(this.button_Help);
            this.Controls.Add(this.button_StartStop);
            this.Controls.Add(this.label_OutText);
            this.Controls.Add(this.button_About);
            this.Controls.Add(this.groupBox_regul);
            this.Controls.Add(this.groupBox5);
            this.ForeColor = System.Drawing.Color.White;
            this.KeyPreview = true;
            this.Name = "Cw_winForm";
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_N)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Speed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Pause)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Tone)).EndInit();
            this.groupBox_regul.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox_StartPause.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_About;
        private System.Windows.Forms.Label label_OutText;
        private System.Windows.Forms.Button button_StartStop;
        private System.Windows.Forms.Button button_Help;
        private System.Windows.Forms.Button button_Exit;
        private System.Windows.Forms.Button button_Config;
        private System.Windows.Forms.Button button_OpenFile;
        private System.Windows.Forms.Button button_SafeFile;
        private System.Windows.Forms.Button button_ToMp3;
        private System.Windows.Forms.TrackBar trackBar_N;
        private System.Windows.Forms.TrackBar trackBar_Speed;
        private System.Windows.Forms.TrackBar trackBar_Pause;
        private System.Windows.Forms.TrackBar trackBar_Tone;
        private System.Windows.Forms.CheckBox checkBox_StartPause;
        private System.Windows.Forms.Label label_N;
        private System.Windows.Forms.Label label_Speed;
        private System.Windows.Forms.Label label_Pause;
        private System.Windows.Forms.Label label_Tone;
        private System.Windows.Forms.GroupBox groupBox_regul;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label label_FileName;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label_Show_N;
        private System.Windows.Forms.Label label_Show_Tone;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label_Show_Pause;
        private System.Windows.Forms.Label label_Show_Speed;
        private System.Windows.Forms.Label label_CopyRight;
        private System.Windows.Forms.GroupBox groupBox_StartPause;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Label label_StartPause;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.CheckBox checkBox_infinity;
    }
}

