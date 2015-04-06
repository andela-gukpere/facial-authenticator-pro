using System;
using System.Windows.Forms;

namespace Face
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        [STAThread]
        static void Main(string[]args)
        {
           Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            bool _continue = false;
            foreach (string arg in args)
            {
                
                if (arg.IndexOf("reg") > -1)
                {
                    new Home().start();
                    Application.Run(new Form1());
                    _continue = false;
                    break;
                }
                else if (arg.IndexOf("surv") > -1)
                {
                    new Home().start();
                    Application.Run(new surv());
                    _continue = false;
                    break;
                }
                else if (arg.IndexOf("att") > -1)
                {
                    MessageBox.Show("Ka Kuro");
                    Application.Exit();
                    break;
                }
                else
                {
                    _continue = true;
                    break;
                }
            }

            if (_continue || args.Length == 0)
            {
                Application.Run(new Home());
            }
           
        }
        
    }
}
