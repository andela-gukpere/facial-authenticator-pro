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
    public partial class manageUsers : Form
    {
        public manageUsers()
        {
            InitializeComponent();
            String[] fnames = ((List<string>)DBvars.db_vars[0]).ToArray();
            listBox1.Items.Clear();
            List<string> already_added = new List<string>();

            for (int i = 0; i < fnames.Length; i++)
            {
                if (!already_added.Contains(fnames[i]))
                {
                    already_added.Add(fnames[i]);
                    listBox1.Items.Add(fnames[i]);
                }
                else
                {
                    continue;
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string val = listBox1.SelectedItem.ToString();
            if (Fitems.deleteUser(val))
            {
                new System.Threading.Thread(new System.Threading.ThreadStart(getDBvars)).Start();
                listBox1.Items.RemoveAt(listBox1.SelectedIndex);
                MessageBox.Show("'" + val + "' has been deleted");
            }
            else
            {
                MessageBox.Show("Error deleteing '" + val + "'");
            }
        }
        private void getDBvars()
        {
            DBvars.db_vars = Fitems.get_db_vars();
        }
    }
}
