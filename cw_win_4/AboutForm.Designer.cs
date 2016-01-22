namespace cw_win_4
{
    partial class AboutForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutForm));
            this.label_About = new System.Windows.Forms.Label();
            this.label_About2 = new System.Windows.Forms.Label();
            this.button_AboutOk = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label_version = new System.Windows.Forms.Label();
            this.label_version_value = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label_About
            // 
            resources.ApplyResources(this.label_About, "label_About");
            this.label_About.Name = "label_About";
            // 
            // label_About2
            // 
            resources.ApplyResources(this.label_About2, "label_About2");
            this.label_About2.Name = "label_About2";
            // 
            // button_AboutOk
            // 
            resources.ApplyResources(this.button_AboutOk, "button_AboutOk");
            this.button_AboutOk.ForeColor = System.Drawing.Color.Yellow;
            this.button_AboutOk.Name = "button_AboutOk";
            this.button_AboutOk.UseVisualStyleBackColor = true;
            this.button_AboutOk.Click += new System.EventHandler(this.button_AboutOk_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::cw_win_4.Properties.Resources.ham_ezhik_2;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // label_version
            // 
            resources.ApplyResources(this.label_version, "label_version");
            this.label_version.Name = "label_version";
            // 
            // label_version_value
            // 
            resources.ApplyResources(this.label_version_value, "label_version_value");
            this.label_version_value.Name = "label_version_value";
            // 
            // AboutForm
            // 
            this.AcceptButton = this.button_AboutOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Teal;
            this.ControlBox = false;
            this.Controls.Add(this.label_version_value);
            this.Controls.Add(this.label_version);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.button_AboutOk);
            this.Controls.Add(this.label_About2);
            this.Controls.Add(this.label_About);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutForm";
            this.ShowInTaskbar = false;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label_About;
        private System.Windows.Forms.Label label_About2;
        private System.Windows.Forms.Button button_AboutOk;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label_version;
        private System.Windows.Forms.Label label_version_value;

    }
}