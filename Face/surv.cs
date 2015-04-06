using System;
using Luxand;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Text;
using System.IO;
using SpeechLib;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Face
{
    public partial class surv : Form
    {
        public surv()
        {

            
            InitializeComponent();
           
        }
        
        
        void callb(IAsyncResult a)
        {
            if (_no_error)
            {
                canClose = true;
                Application.Restart();
                Application.Exit();
            }
            else
            {
                //pictureBox1.Image = bmm;
            }
        }
        bool _no_error = false;
        
    /*    void getStreams()
        {
                       
            try
            {
                HttpWebRequest cam = (HttpWebRequest)WebRequest.Create(new Home().location);
                WebResponse resp = cam.GetResponse();
                Stream resps = resp.GetResponseStream();
                
               // AsyncCallback ds = new AsyncCallback(callb);
                int read = 0, total = 0;
                byte[] buff = new byte[7000000];
                //object state = new char[32];
                //IAsyncResult bmpdl = resps.BeginRead(buff,0,3000,ds,state);
                while ((read = resps.Read(buff, total, 1000)) != 0)
                {
                    total += read;
                }
                Bitmap bmp = (Bitmap)Bitmap.FromStream(new MemoryStream(buff, 0, total, false));
                //(Bitmap)(Bitmap.FromStream(resps));
                //(Bitmap)Bitmap.FromStream(new MemoryStream(buff, 0, total, false));
                //new MemoryStream(buff, 0, total));
                resps.Flush();
                recFace(bmp);
                resp.Close();
                bmp.Dispose();            
                resps.Close();
                resps.Dispose();
            }
            catch (HttpListenerException ht)
            {
                held = true;
                MessageBox.Show(ht.Message + " listener exception");
                Application.Exit();
                _no_error = true;
            }
            catch (ArgumentException ar)
            {
                held = true;
                MessageBox.Show(ar.TargetSite.ToString() + " arg except");
                _no_error = true;
            }
            catch (IOException ioe)
            {
                held = true;
                MessageBox.Show(ioe.Message + "ioexception");
                _no_error = true;
            }
            catch (WebException webex)
            {
                held = true;
                MessageBox.Show(webex.Message + "web exception");
                _no_error = true;
            }
            if (_no_error)
            {
                canClose = true;
                Application.Exit();
                Application.Restart();
            }
        }*/
        public void recFace(Bitmap ppp)
        {
            try
            {
               // IntPtr ff = IntPtr.Zero;//ppp.GetHbitmap();
               // FSDK.SaveImageToHBitmap(cimg, ref ff);
                //FSDK.LoadImageFromHBitmap(ref cimg, ff);
                //FSDK.SetFaceDetectionParameters(false, false, 500);
                //FSDK.SetFaceDetectionThreshold(2);

                FSDK.TFacePosition facePosition = new FSDK.TFacePosition();

                IntPtr hbitmapHandle = IntPtr.Zero;

                FSDK.SaveImageToHBitmap(cimg, ref hbitmapHandle);
                Image ccimg = Image.FromHbitmap(hbitmapHandle);
                Graphics gr = Graphics.FromImage(ccimg);
                fileName = (DateTime.Now.ToLongTimeString() + "__" + DateTime.Now.ToShortDateString()).Replace("/", "-").Replace(" ", "_").Replace(":", "-") + (new Random().Next(new Random().Next(100), new Random().Next(100, 1000))).ToString()+"__";
                if (FSDK.FSDKE_OK == FSDK.DetectFace(cimg, ref facePosition))
                {
                    gr.DrawRectangle(Pens.LightBlue, facePosition.xc - facePosition.w / 2, facePosition.yc - facePosition.w / 2,
                    facePosition.w, facePosition.w);
                    byte[] tempd = new byte[FSDK.TemplateSize];
                    FSDK.GetFaceTemplateInRegion(cimg, ref facePosition, out tempd);
                    temp = System.Text.Encoding.ASCII.GetString(tempd);
                    if (check_reg(tempd))
                    {
                        StringFormat format = new StringFormat();
                        format.Alignment = StringAlignment.Center;
                        gr.DrawString(_name + " | " + _email , new System.Drawing.Font("Candara", 18),
                            new System.Drawing.SolidBrush(System.Drawing.Color.LightBlue),
                            facePosition.xc, facePosition.yc + facePosition.w * 0.55f, format);
                        fileName += _name;
                    }

                    //gp = ppp;
                    //                    timer3.Enabled = true;
                    //res(ppp);
                    _first_tick++;
                }
                else
                {
                    if (_first_tick < 20 && _first_tick != 0)
                    {
                        _first_tick++;
                        if (_first_tick == 19)
                        {
                            _first_tick = 0;
                        }
                        res(ppp);
                    }
                }
                if (!held)
                {
                  //  pictureBox1.Height = ccimg.Height;
                 //   pictureBox1.Width = ccimg.Width;
                    pictureBox1.Image = ccimg;
                }
                
                FSDK.FreeImage(cimg);
                DeleteObject(hbitmapHandle);
                GC.Collect();
                Application.DoEvents();
              //  new System.Threading.Thread(new System.Threading.ThreadStart(getStreams)).Start();
            }
            catch (NullReferenceException nulr)
            {
                MessageBox.Show(nulr.Message);
                canClose = true;
                Application.Exit();
                Application.Restart();
            }
            catch (ExternalException exp)
            {

            }
            catch (AccessViolationException acs)
            {

            }

        }

        [DllImport("gdi32.dll")]
        static extern bool DeleteObject(IntPtr hObject);
        bool canClose = true;
        private void button1_Click(object sender, EventArgs e)
        {
            
        }
        bool needClose = false;
        void webCam()
        {
            if (pictureBox1.InvokeRequired)
            {
                pictureBox1.Invoke(new MethodInvoker(webCam));//new ThreadStart(webCam));
                return;
            }
            pictureBox1.Width = Fitems.CamX;
            pictureBox1.Height = Fitems.CamY;
            int cameraHandle = Fitems.cameraN;
            FSDK.TFacePosition facePosition = new FSDK.TFacePosition();
            while (!needClose) 
            {
                
                Int32 imageHandle = 0;
                if (FSDK.FSDKE_OK != FSDKCam.GrabFrame(cameraHandle, ref imageHandle)) // grab the current frame from the camera
                {
                    Application.DoEvents();
                    continue;
                }
                IntPtr hbitmapHandle = IntPtr.Zero; // to store the HBITMAP handle
                FSDK.SaveImageToHBitmap(imageHandle, ref hbitmapHandle);

                Image frameImage = Image.FromHbitmap(hbitmapHandle);
                Graphics gr = Graphics.FromImage(frameImage);
                               
                fileName = (DateTime.Now.ToLongTimeString() + "__" + DateTime.Now.ToShortDateString()).Replace("/", "-").Replace(" ", "_").Replace(":", "-") + (new Random().Next(new Random().Next(100), new Random().Next(100, 1000))).ToString();
                if (FSDK.FSDKE_OK == FSDK.DetectFace(imageHandle, ref facePosition))
                {
                    gr.DrawRectangle(Pens.Purple, facePosition.xc - facePosition.w / 2, facePosition.yc - facePosition.w / 2,
                    facePosition.w, facePosition.w);
                    byte[] tempd = new byte[FSDK.TemplateSize];
                    FSDK.GetFaceTemplateInRegion(imageHandle, ref facePosition, out tempd);
                    
               //     temp = System.Text.Encoding.ASCII.GetString(tempd);
                    timer1.Enabled = true;
                   
                    Fitems.intruder_img = frameImage;
                    if (check_reg(tempd))
                    {
                        timer1.Enabled = false;
                        timer2.Enabled = false;
                        sp.Stop();
                   //     if (Fitems.intruder_img != null) Fitems.intruder_img.Dispose();
                        alarm_int = 0;
                        StringFormat format = new StringFormat();
                        format.Alignment = StringAlignment.Center;
                        gr.DrawString(_name + " | " + _email , new System.Drawing.Font("Candara", 18),
                            new System.Drawing.SolidBrush(System.Drawing.Color.Purple),
                            facePosition.xc, facePosition.yc + facePosition.w * 0.55f, format);
                        fileName += _name;
                        Fitems.user_name = _name;
                        Fitems.user_id = _userid;
                        res((Bitmap)frameImage);
                        Fitems.uimg = frameImage;
                                               
                        
                        Form sForm = new successForm();
                        needClose = true;
                        this.AddOwnedForm(sForm);
                        sForm.Show();
                        this.Enabled = false;

                        sp.Stop();
                        this.Hide();
                        //return;
                    }

                    //gp = ppp;
                    //                    timer3.Enabled = true;
                    //Bitmap ppp = (Bitmap)frameImage;
                    //this.Invoke(new System.Threading.ParameterizedThreadStart(res), new object[] { ppp });
                    //new System.Threading.Thread(new System.Threading.ParameterizedThreadStart());
                    //res(ppp);
                    //System.Threading.Thread tr = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(res(ppp)),1);
              //      _first_tick++;
                }
                else
                {
                    timer1.Enabled = false;
                    alarm_int = 0;
                    timer2.Enabled = false;
                    sp.Stop();
                }

                try
                {
                    pictureBox1.Image = frameImage;
                }
                catch (ArgumentException argex)
                {
                  //  MessageBox.Show(argex.Message + "\n" + argex.StackTrace);
                }
                FSDK.FreeImage(imageHandle); // delete the FSDK image handle
                DeleteObject(hbitmapHandle); // delete the HBITMAP object
                GC.Collect(); // collect the garbage after the deletion

                // make UI controls accessible
                Application.DoEvents();
            }
           // FSDKCam.CloseVideoCamera(cameraHandle);
           // FSDKCam.FinalizeCapturing(); 
        }

        
        String cameraName;
        private void surv_Load(object sender, EventArgs e)
        {   
           names = new List<string>();
           emails = new List<string>();
           date = new List<long>();
           userid = new List<int>();
           faceTemplates = new List<byte[]>();
           Thread db_thread = new System.Threading.Thread(new System.Threading.ThreadStart(load_XML));
           db_thread.Priority = ThreadPriority.Lowest;
           db_thread.Start();
           new System.Threading.Thread(new System.Threading.ThreadStart(webCam)).Start();
           sp = new System.Media.SoundPlayer("ringout.wav"); 
        }

        public void load_XML()
        {
            object[] obj = DBvars.db_vars;
            if (obj == null)
            {
                MessageBox.Show("Object array contains null explitus");
                return;
            }
            else
            {
                faceTemplates.Clear();
                names.Clear();
                emails.Clear();
                date.Clear();
                userid.Clear();
                //Clear array contents sharply//

                faceTemplates = (List<byte[]>)obj[2];
                names = (List<string>)obj[0];
                emails = (List<string>)obj[1];
                date = (List<long>)obj[3];
                userid = (List<int>)obj[4];
            }
            /*
            try
            {
                faceTemplates.Clear();
                XmlDocument xdoc = new XmlDocument();
                xdoc.Load("db.xml");
                for (int i = 0; i < xdoc.GetElementsByTagName("temp").Count; i++)
                {
                    XmlNode tnode = xdoc.GetElementsByTagName("temp")[i];
                    string ttemp = tnode.FirstChild.Value;
                    //byte[] fe = ;
                    faceTemplates.Add(System.Text.Encoding.ASCII.GetBytes(ttemp));
                    names.Add(tnode.PreviousSibling.FirstChild.Value);
                    emails.Add(tnode.ParentNode.ChildNodes[2].FirstChild.Value);
                 //   rooms.Add(tnode.ParentNode.ChildNodes[3].FirstChild.Value);
                    Application.DoEvents();
                }
                
                //new System.Threading.Thread(new System.Threading.ThreadStart(load_XML)).Start();               
            }
            catch (OutOfMemoryException outEx)
            {
                Console.WriteLine(outEx.Message);
             //   load_XML();
            }
             
             */
        }

        public List<byte[]> faceTemplates; // set of face templates (we store 10)
        public List<String> names;
        public List<String> emails;
        public List<long> date;
        public List<int> userid;
       // public List<String> rooms;
        
       
        int cimg;
        private string temp = "";
        private string _name = "";
        private string _email = "";
        private int _userid = 0;
        private long _date = 0L;
        public Bitmap gp
        {
            get { return gpp; }
            set { gpp = value; }
        }
        //int incr = 0;
        public Bitmap gpp;
        
        //bool _toContinue = true;
        public bool check_reg(byte[] ttp)
        {
            try
            {
                bool val = false;
                foreach (byte[] t in faceTemplates)
                {
                    float similarity = 0.0f;
                    byte[] t1 = t;
                    FSDK.MatchFaces(ref ttp, ref t1, ref similarity);
                    float threshold = 0.0f;
                    FSDK.GetMatchingThresholdAtFAR(0.001f, ref threshold); // set FAR to 1%--0.1%
                    if (similarity > threshold)
                    {
                        val = true;
                        setV(t1);
                        break;
                    }
                    Application.DoEvents();
                }
                return val;
            }
            catch (InvalidOperationException inov)
            {
                return false;
            }
        }
        public void setV(byte[] t)
        {
            for (int i = 0; i < faceTemplates.Count; i++)
            {
                if (faceTemplates[i] == t)
                {
                    _name = names[i];
                    _date = date[i];
                    _userid = userid[i];
                    _email = emails[i];
                    break;
                }
            }
        }
        string fileName = "";
        
      
        public void res(Bitmap bmp)
        {
            try
            {
                Image img = Image.FromHbitmap(bmp.GetHbitmap());
                Rectangle rect = new Rectangle(0, 0, img.Width / 2, img.Height / 2);
                Bitmap out_img = new Bitmap(rect.Width, rect.Height);
                Graphics g = Graphics.FromImage(out_img);
                g.FillRectangle(Brushes.Black, rect);
                g.DrawImage(img, rect);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
                string folder = DateTime.Now.ToShortDateString().Replace("/", "-");
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                out_img.Save("./"+folder+"/"+fileName+ ".jpg",System.Drawing.Imaging.ImageFormat.Jpeg);
               // img.Dispose();
            }
            catch (Exception)
            {

         //     held = true;
              //  MessageBox.Show(nule.Message);
          //      canClose = true;
         //       needClose = true;
           //     Application.Exit();
              //  Application.Restart();
            }
           
        }

        
        void go_back()
        {
            needClose = true;
            Home ds = new Home();
            this.AddOwnedForm(ds);
            this.Hide();
            this.Enabled = false;
            ds.Show();
        }
        private void surv_FormClosing(object sender, FormClosingEventArgs e)
        {
            needClose = true;
            try
            {
                sp.Stop();
            }
            catch (NullReferenceException nre)
            {

            }
            if (this.Visible)// && !canClose)
            {                
                Home ds = new Home();
                this.AddOwnedForm(ds);
                this.Hide();
                this.Enabled = false;
                ds.Show();
                e.Cancel = true;
            }
            else
            {
                e.Cancel = false;
            }
        }
        bool held = false;
        private void button2_Click(object sender, EventArgs e)
        {
            
                        
           
        }
        int _first_tick = 0;
        int alarm_int = 0;
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            alarm_int++;
            if (alarm_int == 2)
            {
                try
                {
                    vox_alert();
                    alert();
                }
                catch (NullReferenceException nulexp)
                {
                    MessageBox.Show(nulexp.Message + "\n" + nulexp.StackTrace);
                }
                timer1.Enabled = false;
            }
        }

        private void vox_alert()
        {
            if (vox == null) return;
            vox.Rate = 1;
            vox.Volume = 100;
            timer2.Enabled = true;
        }
        SpVoice vox = new SpVoice();
        private System.Media.SoundPlayer sp = null;
        void alert()
        {
            string intruder_Folder = "intruder_"+DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
            if (!Directory.Exists(intruder_Folder))
            {
                Directory.CreateDirectory(intruder_Folder);
            }
            string date =  DateTime.Now.Hour + "_" + DateTime.Now.Minute+"_"+DateTime.Now.Millisecond;
            Bitmap b = (Bitmap)Fitems.intruder_img;
            Stream str = new FileStream(intruder_Folder+"/"+date+".jpg", FileMode.Create);
            
            b.Save(str,System.Drawing.Imaging.ImageFormat.Jpeg);

            Fitems.intruder_img.Dispose();
            sp.LoadAsync();
            sp.PlayLooping();
            sp.Dispose();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (vox == null) return;
            vox.Speak("Intruder!!!", SpeechVoiceSpeakFlags.SVSFlagsAsync);
        }

        private void homeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            needClose = true;
            Home ds = new Home();
            sp.Stop();
            vox.Pause();
            vox = null;
            this.AddOwnedForm(ds);
            this.Hide();
            this.Enabled = false;


            ds.Show();
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!held)
            {
                held = true;
             //   button2.Text = "Pause Capture";
                needClose = true;
            }
            else
            {
                needClose = false;
                System.Threading.Thread t = new Thread(new System.Threading.ThreadStart(webCam));
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
               // button2.Text = "Continue Capturing";
                held = false;
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            settings settings = new settings();

            this.AddOwnedForm(settings);
            settings.Show();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox1();
        }
    }
}
