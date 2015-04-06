using System;
using Luxand;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Face
{
    public partial class Home : Form
    {
        ListBox camera = null;
        public Home()
        {
            InitializeComponent();
           
            
            camera = new ListBox();
            this.camera.FormattingEnabled = true;
            this.camera.Name = "Camera List";
            this.camera.Size = new System.Drawing.Size(175, 50);
            this.camera.Location = new Point(46, 250);
            this.camera.TabIndex = 10;
            this.Controls.Add(camera);
            this.camera.SelectionMode = SelectionMode.One;
            this.camera.SelectedIndexChanged += new System.EventHandler(this.camera_select);
            
            
        }
        private void camera_select(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            int s = camera.SelectedIndex;
            if (s != Fitems.cameraN) Fitems.init = false;
            if ((Fitems.cameraN == s) && Fitems.init == true) { doneLoadingCamera(); return; }

         //   System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(fsdk_async_funcs));
          //  t.IsBackground = true;
          //  t.SetApartmentState(System.Threading.ApartmentState.STA);
           // t.Priority = System.Threading.ThreadPriority.BelowNormal;
          //  t.Start();
            backgroundWorker1.RunWorkerAsync();
                        
            string cameraName = (string)camera.SelectedItem;


            int r = FSDKCam.OpenVideoCamera(ref cameraName, ref s);
            if (r != FSDK.FSDKE_OK)
            {
                MessageBox.Show("Error opening camera\nCamera Busy", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();

            }
            int count;
            String[] cameraList;
            FSDKCam.GetCameraList(out cameraList, out count);

            FSDK.SetFaceDetectionParameters(Fitems.facerotationB, Fitems.faceAngle, Fitems.faceWidth);
            FSDK.SetFaceDetectionThreshold(Fitems.facethreshold);

            FSDKCam.VideoFormatInfo[] formatList;

            FSDKCam.GetVideoFormatList(ref cameraName, out formatList, out count);
            Fitems.CamX = formatList[0].Width;
            Fitems.CamY = formatList[0].Height;
            Fitems.cameraN = s;
            Application.DoEvents();
            Fitems.init = true;
        }
        private bool Init = false;
        void fsdk_async_funcs()
        {
            try
            {
            //    if (Init)
             //   {
                    FSDKCam.CloseVideoCamera(Fitems.cameraN);
                    FSDKCam.FinalizeCapturing();
              //  }
                Application.DoEvents();
                FSDKCam.InitializeCapturing();
                Init = true;
            }
            catch (AccessViolationException)
            {
                MessageBox.Show("AccessViolationException thrown\nIf you can see this error please contact visit http://godson.com.ng");
                Application.Exit();
                return;
            }
        }
       
        private void button3_Click(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 ds = new Form1();
            AddOwnedForm(ds);
            ds.Show();
            //ds.timer1.Enabled = true;
            //ds.timer2.Enabled = true;
            this.Hide();
            
         //   this.Close();
          //  this.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            attendance ds = new attendance();
            AddOwnedForm(ds);
            ds.Show();
            
            //ds.timer1.Enabled = true;
            this.Hide();
           // this.Close();
           // this.Dispose();
            
        }
     
       
        
      /*  public string location
        {
            get { return loc; }
            set { loc = value; }
        }
        public string loc = "http://192.168.1.9/axis-cgi/jpg/image.cgi";
        public string loc = "http://localhost/projects/k.jpg";*/
               
        private void button5_Click(object sender, EventArgs e)
        {
            surv ds = new surv();
            this.AddOwnedForm(ds);
            ds.Show();                                 
            this.Hide();
        }

     

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Home_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        
        private void button3_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }
        settings settings = null;
        private void Home_Load(object sender, EventArgs e)
        {
            settings = new settings();
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(start));
            t.SetApartmentState(System.Threading.ApartmentState.STA);
            t.Priority = System.Threading.ThreadPriority.Normal;
            t.Start();
            this.camera.Enabled = false;
            this.camera.Items.Add("Loading Cameras....");

            progressBar1.Value = 0;
           
            System.Threading.Thread tt = new System.Threading.Thread(new System.Threading.ThreadStart(loadSQLiteDBvars));
            tt.Priority = System.Threading.ThreadPriority.BelowNormal;    
            tt.Start();

            settings.trackBar1.Value = Fitems.facethreshold;
            settings.trackBar2.Value = Fitems.faceWidth;
            settings.checkBox1.Checked = Fitems.facerotationB;
            settings.checkBox2.Checked = Fitems.faceAngle;
           /* while (t.isAlive)
            {
                progressBar1.Increment(10);
                if (progressBar1.Value == 100) { break; }
            }*/
            
        }
        void loadSQLiteDBvars()
        {
            DBvars.db_vars = Fitems.get_db_vars();
        }
        
        public void start()
        {
            //if (Fitems.init) MessageBox.Show(Fitems.cameraN.ToString());
            if (camera.InvokeRequired)
            {
                camera.Invoke(new System.Threading.ThreadStart(start), new object[] { });
                Application.DoEvents();
                return;
            }
            //
            //            

            FSDK.ActivateLibrary(DBvars.FSDK_KEY[DBvars.FSDK_KEY.Length - 1]);
            FSDK.InitializeLibrary();
            string[] cameraList;
            int count;
            FSDKCam.GetCameraList(out cameraList, out count);
            if (0 == count)
            {
                MessageBox.Show("Please attach a camera", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            bool camSel = false;
            camera.Items.Clear();
            
            for (int i = 0; i < count; i++)
            {
                //MessageBox.Show(cameraList[i] + " : " + i.ToString());
                camera.Items.Add(cameraList[i]);
                if (cameraList[i].Contains("USB 2.0 Camera"))
                {
                 //   Fitems.cameraN = i;
                  //  camSel = true;
                 //   camera.SetSelected(i, true);
                }
                if (i == 1)
                {
      
                }
                Application.DoEvents();
            }
            if(!camSel)camera.SetSelected(Fitems.cameraN, true);
          
        }

      

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void settingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (settings == null) return;
            this.AddOwnedForm(settings);
            settings.Show();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox1().ShowDialog();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
       //     if (!backgroundWorker1.IsBusy) backgroundWorker1.CancelAsync(); 
            fsdk_async_funcs();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            doneLoadingCamera(); 
        }
        void doneLoadingCamera()
        {
            progressBar1.Value = 100;
            this.camera.Enabled = true;
        }
    }
}
