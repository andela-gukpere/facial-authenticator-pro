using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using SpeechLib;
using System.Windows.Forms;

namespace Face
{
    public partial class successForm : Form
    {
        public successForm()
        {
            InitializeComponent();
        }
        SpVoice vox = new SpVoice();
        private void Form2_Load(object sender, EventArgs e)
        {
            conBttn.Focus();
            pictureBox1.Image = Fitems.uimg;
            label2.Text = Fitems.user_name;
            timer1.Enabled = true; 
            timer2.Enabled = true;            
            this.FindForm().Text = "Welcome | " + Fitems.user_name;
            new System.Threading.Thread(new System.Threading.ThreadStart(log_user)).Start();
        }
        private void log_user()
        {
            Fitems.insert_log(Fitems.user_id);
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            END();
        }

        private void Form2_Click(object sender, EventArgs e)
        {
            END();
        }
        void END()
        {
            surv ds = new surv();
            this.AddOwnedForm(ds);
            ds.Show();
            this.Hide();
            timer1.Enabled = false;
            timer2.Enabled = false;
            this.Enabled = false;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            vox.Rate = 1;
            vox.Volume = 100;
            vox.Speak("Welcome, " + Fitems.user_name, SpeechVoiceSpeakFlags.SVSFlagsAsync);
            timer2.Enabled = false;
        }

        private void conBttn_Click(object sender, EventArgs e)
        {
            END();
        }

    }
}
