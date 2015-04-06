using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Face
{
    public partial class login : Form
    {
        public login()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            if (_isValidUser())
            {
                Home home = new Home();
                this.AddOwnedForm(home);
                this.Hide();
                home.Show();
            }
            else
            {
                MessageBox.Show("Invalid Login Information provided '" + textBox1.Text +"' is not a valid Administrative account","Facesometrics | Login",MessageBoxButtons.OK,MessageBoxIcon.Exclamation,MessageBoxDefaultButton.Button1,MessageBoxOptions.ServiceNotification);
                //textBox1.Text = "";
                textBox2.Text = "";
                this.Focus();
                textBox2.Focus();
            }
        }
        bool _isValidUser()
        {
            if (textBox1.Text.GetHashCode() == "Admin".GetHashCode() && textBox2.Text.GetHashCode() == "facelogin".GetHashCode())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void login_Load(object sender, EventArgs e)
        {
            textBox1.Focus();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
