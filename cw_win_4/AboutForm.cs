using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace cw_win_4
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            label_version_value.Text = Cw_winForm.program_version;
        }

        private void button_AboutOk_Click(object sender, EventArgs e)
        {
            Close();    // Закрываем форму
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Close();    // Закрываем форму
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
