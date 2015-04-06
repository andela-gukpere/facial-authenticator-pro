using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using Luxand;
using System.Windows.Forms;

namespace Face
{
    public partial class settings : Form
    {
        public settings()
        {
            InitializeComponent();
        }

        private void settings_Load(object sender, EventArgs e)
        {
            trackBar1.Value = Fitems.facethreshold;
            checkBox2.Checked = Fitems.faceAngle;
            trackBar2.Value = Fitems.faceWidth;
            checkBox1.Checked = Fitems.facerotationB;
        }
        private void trackBar1_Scroll(object sender, EventArgs e)
        {

            Fitems.facethreshold = trackBar1.Value;
            label5.Text = "Face Detection Threshold =>" + Fitems.facethreshold + "\n Internal Resize Width =>" + Fitems.faceWidth;
            FSDK.SetFaceDetectionThreshold(Fitems.facethreshold);
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {

            Fitems.faceWidth = trackBar2.Value;
            FSDK.SetFaceDetectionParameters(Fitems.facerotationB, Fitems.faceAngle, Fitems.faceWidth);
            label5.Text = "Face Detection Threshold =>" + Fitems.facethreshold + "\n Internal Resize Width =>" + Fitems.faceWidth;
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Fitems.facerotationB = checkBox1.Checked;
            FSDK.SetFaceDetectionParameters(Fitems.facerotationB, Fitems.faceAngle, Fitems.faceWidth);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Fitems.faceAngle = checkBox2.Checked;
            FSDK.SetFaceDetectionParameters(Fitems.facerotationB, Fitems.faceAngle, Fitems.faceWidth);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
