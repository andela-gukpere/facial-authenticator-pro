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
    public partial class userlog : Form
    {
        public userlog()
        {
            InitializeComponent();
            listBox1.Items.Clear();
            List<string> user_log = Fitems.get_log_vars();
            listBox1.Items.AddRange(user_log.ToArray());
            listBox1.SetSelected(listBox1.Items.Count - 1, true);
        }
    }
}
