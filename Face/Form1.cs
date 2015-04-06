using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using Luxand;
using System.Text;
//using System.Net;
using System.IO;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Windows.Forms;
//using System.Xml;
using System.Threading;

namespace Face
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public byte[] tempFace = null;
        string cameraName,fileName;
        bool isFace = false;
        bool needClose = false;
        bool reg = true;
        void drawEyes(object objc)
        {
            try
            {
                object[] obj = (object[])objc;
                int imageHandle = (int)obj[0];
                FSDK.TFacePosition facePosition = (FSDK.TFacePosition)obj[1];
                Graphics gr = (Graphics)obj[2];
                FSDK.TPoint[] eye = new FSDK.CImage(imageHandle).DetectEyesInRegion(ref facePosition);
                byte[] eyes = new byte[0];
                FSDK.GetFaceTemplateUsingEyes(imageHandle, ref eye, out eyes);
                gr.DrawRectangle(Pens.Pink, eye[0].x, eye[0].y, 20f, 10f);
                gr.DrawRectangle(Pens.Pink, eye[1].x, eye[1].y, 20f, 10f);
            }
            catch (Exception)
            {
                //who sai
            }

        }
        void webCam()
        {
            try
            {
                if (pictureBox1.InvokeRequired)
                {
                    pictureBox1.Invoke(new ThreadStart(webCam));
                    return;
                }
            }
            catch (Exception)
            {
          //      webCam();
            }

            try
            {

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
                        gr.DrawRectangle(Pens.Orange, facePosition.xc - facePosition.w / 2, facePosition.yc - facePosition.w / 2,
                        facePosition.w, facePosition.w);
                        byte[] tempd = new byte[FSDK.TemplateSize];
                        FSDK.GetFaceTemplateInRegion(imageHandle, ref facePosition, out tempd);
                        drawEyes(new object[] { imageHandle, facePosition,gr });
                        tempFace = tempd;

                        isFace = true;
                        if (reg)
                        {
                            try
                            {
                                button2.Enabled = true;
                            }
                            catch (InvalidOperationException inv)
                            {

                            }
                            if (check_reg(tempd))
                            {
                                StringFormat format = new StringFormat();
                                format.Alignment = StringAlignment.Center;
                                gr.DrawString(_name + " | " + _email, new System.Drawing.Font("Candara", 18),
                                    new System.Drawing.SolidBrush(System.Drawing.Color.Orange),
                                    facePosition.xc, facePosition.yc + facePosition.w * 0.55f, format);
                                fileName += _name;
                            }
                        }

                    }
                    else
                    {
                        isFace = false;
                        try
                        {
                            button2.Enabled = false;
                        }
                        catch (InvalidOperationException inv)
                        {

                        }

                    }

                    //recFace(ppp);
                    pictureBox1.Image = frameImage;

                    FSDK.FreeImage(imageHandle); // delete the FSDK image handle
                    DeleteObject(hbitmapHandle); // delete the HBITMAP object
                    GC.Collect(); // collect the garbage after the deletion

                    // make UI controls accessible
                    Application.DoEvents();
                }
            }
            catch (Exception)
            {
               
            }
        }


       
        private void Form1_Load(object sender, EventArgs e)
        {
            
            names = new List<string>();
            date = new List<long>();
            emails = new List<string>();
            userid = new List<int>();
            faceTemplates = new List<byte[]>();
            new System.Threading.Thread(new System.Threading.ThreadStart(load_XML)).Start();
            Thread t =  new System.Threading.Thread(new System.Threading.ThreadStart(webCam));
            //t.SetApartmentState(ApartmentState.MTA);
            t.Start();
            button2.Enabled = false;
         }
        
      
        [DllImport("gdi32.dll")]
        static extern bool DeleteObject(IntPtr hObject);
       // public int fsbmp = 0;
      //  public int cimg = 0;
       
       
    /*   void getStreams()
       {
           try
           {
               HttpWebRequest cam = (HttpWebRequest)WebRequest.Create(new Home().location);
               WebResponse resp = cam.GetResponse();
               Stream resps = resp.GetResponseStream();
               //AsyncCallback ds = new AsyncCallback(callb);
               //int read, total = 0;
               //byte[] buff = new byte[100000];
               //object state = new char[32];
               //IAsyncResult bmpdl = resps.BeginRead(buff,0,3000,ds,state);
               //while ((read = resps.Read(buff, total, 1000)) != 0)
               //{
               //total += read;
               //}

               Bitmap bmp = (Bitmap)(Bitmap.FromStream(resps));
               //(Bitmap)Bitmap.FromStream(new MemoryStream(buff, 0, total, false));
               //new MemoryStream(buff, 0, total));


               resps.Close();

               recFace(bmp);
           }
           catch (HttpListenerException ht)
           {
               timer1.Enabled = false;
               timer1.Dispose();
               MessageBox.Show(ht.Message);
               _no_error = true;
           }
           catch (ArgumentException ar)
           {
               timer1.Enabled = false;
               timer1.Dispose();
               MessageBox.Show(ar.Message);
               _no_error = true;
           }
           catch (IOException ioe)
           {
               timer1.Enabled = false;
               timer1.Dispose();
               MessageBox.Show(ioe.Message);
               _no_error = true;
           }
           catch (WebException webex)
           {
               timer1.Enabled = false;
               timer1.Dispose();
               MessageBox.Show(webex.Message);
               _no_error = true;
           }
           if (_no_error)
           {
               canClose = true;
               Application.Exit();
               Application.Restart();
           }
       }
     * */
       bool _no_error = false;
        bool canClose = false;
        private void timer1_Tick(object sender, EventArgs e)
        {
            //System.Threading.Thread aa = new System.Threading.Thread(new System.Threading.ThreadStart(getStreams));
            //aa.Start();
            //aa.Priority = System.Threading.ThreadPriority.Highest;
            //getStreams();
        }
       
        public bool held = false;
        private void button3_Click(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            progressBar1.Value = 0;
            //timer3.Stop();
            reg = false;
            timer3.Enabled = true;
        /*    while (true)
            {
                time++;
                if (textBox1.Text.Length < 1 || textBox2.Text.Length < 1 || textBox3.Text.Length < 1)
                {
                   
                    time = 0;
                    button2.Enabled = true;
                    label5.Text = "more info needs to be provided";
                    reg = true;
                    return;
                }
                add_user();
                
                //new Thread(new ThreadStart(add_user)).Start();
                if (time == 10)
                {
                    reg = true;
                    //    timer3.Stop();
                    //    timer3.Enabled = false;
                    time = 0;
                   
                    label5.Text = textBox1.Text + " \n" + textBox2.Text + textBox3.Text + "\n" + "saved";
                    load_XML();
                    button2.Enabled = true;
                    break;
                    //   Application.Restart();
                }
                //timer3.Enabled = true;
                
                
                Application.DoEvents();
            }
         */
        }
    /*    public void xml_save()
        {
            StreamWriter dtr = new StreamWriter("db.xml");
            XmlWriter xmlw = XmlWriter.Create(dtr.BaseStream);
            xmlw.WriteStartDocument();
            xmlw.WriteStartElement("students");
            xmlw.WriteEndElement();
            xmlw.WriteEndDocument();
            xmlw.Close();
            dtr.Close();
        }
        */
        public void add_user()
        {
           
       /*     if (textBox1.InvokeRequired)
            {
                 textBox1.Invoke(new ThreadStart(add_user));
            }
            if (textBox2.InvokeRequired)
            {
                textBox2.Invoke(new ThreadStart(add_user));
            }
            if (textBox3.InvokeRequired)
            {
                textBox3.Invoke(new ThreadStart(add_user));
            }
        * */
            
            reg = false;
            
            bool appen_user = Fitems.insert_vals(textBox1.Text, textBox2.Text, tempFace);
            Application.DoEvents();
       /*     XmlDocument xdoc = new XmlDocument();
            xdoc.Load("db.xml");

            

            XmlElement name = xdoc.CreateElement("name");
            
            XmlElement student = xdoc.CreateElement("student");
            XmlElement tempa = xdoc.CreateElement("temp");
            XmlElement matric = xdoc.CreateElement("matric");
            XmlElement room = xdoc.CreateElement("room");
            
            tempa.AppendChild(xdoc.CreateTextNode(temp));
            name.AppendChild(xdoc.CreateTextNode(textBox1.Text));
            matric.AppendChild(xdoc.CreateTextNode(textBox2.Text));
            room.AppendChild(xdoc.CreateTextNode(textBox3.Text));

            student.AppendChild(name);
            student.AppendChild(tempa);
            student.AppendChild(matric);
            student.AppendChild(room);
            xdoc.GetElementsByTagName("students")[0].AppendChild(student);
            try
            {
                StreamWriter dtr = new StreamWriter("db.xml");
               // dtr.AutoFlush = true;
                xdoc.Save(dtr.BaseStream);
                dtr.Flush();
                dtr.Close();
                dtr.Dispose();
               // dtr = null;
               // xdoc = null;
                
            }
            catch (IOException ioexc)
            {
                Console.WriteLine(ioexc.StackTrace);
               // Application.DoEvents();
            }
          //  dtr.Dispose();
          //  load_XML();
        * */
        }
         
        public struct FaceTemplate
        { // single template
            public byte[] templateData;
        }
        private List<byte[]> faceTemplates; // set of face templates (we store 10)
        private List<string> names;
        private List<string> emails;
        private List<long> date;
        private List<int> userid;

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
                    FSDK.GetMatchingThresholdAtFAR(0.001f, ref threshold); // set FAR to 1% now at 0.1%
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
                    _email = emails[i];
                   
                    break;
                }
            }
        } 
       
        public void load_XML()
        {
            object[] obj = DBvars.db_vars;
            if (obj == null)
            {
                MessageBox.Show("Database NULL");
                return;
            }
            else
            {
                faceTemplates.Clear();
                names.Clear();
                userid.Clear();
                emails.Clear();
                date.Clear();
             //   MessageBox.Show("Object array DB Loaded");
                //Clear array contents sharply//

                faceTemplates = (List<byte[]>)obj[2];
                names = (List<string>)obj[0];
                emails = (List<string>)obj[1];
                date = (List<long>)obj[3];
                userid = (List<int>)obj[4];
            }
           
        }
        
        public string _name = "";
        public string _email = "";
        public long _date =0L;
       
        private void timer2_Tick(object sender, EventArgs e)
        {

        }
        private void button4_Click(object sender, EventArgs e)
        {
               
        }

        private void ce_fini()
        {
            Home ds = new Home();
            this.AddOwnedForm(ds);
            ds.Show();
            this.Hide();
            held = true;
            timer1.Enabled = false;
            timer2.Enabled = false;
            this.Enabled = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            needClose = true;
            if (this.Visible == true && !canClose)
            {
                e.Cancel = true;
            }
            else
            {
                e.Cancel = false;
            }
        }
        int time = 0;
        private void timer3_Tick(object sender, EventArgs e)
        {
            if (!isFace)
            {                
                label5.Text = "Please focus on camera";
                return;
            }
            
            if (textBox1.Text.Length < 2 || textBox2.Text.Length < 2)// || textBox3.Text.Length < 2)
            {
                timer3.Stop();
                timer3.Enabled = false;
                time = 0;
                button2.Enabled = true;
                label5.Text = "more info needs to be provided";
                reg = true;
                return;
            }
            try
            {
                progressBar1.Value = time * 10;
            }
            catch (ArgumentOutOfRangeException argex)
            {
                
            }
            time++;
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            label5.Text = "Saving : "+time.ToString() + " of 10";
            new Thread(new ThreadStart(add_user)).Start();
            Application.DoEvents();
            if (time > 10 || time == 10)
            {
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                reg = true;
                timer3.Stop();
                timer3.Enabled = false;
                time = 0;
                label5.Text = "Name: "+textBox1.Text + " \nEmail: " + textBox2.Text + "\n" + "saved";
                button2.Enabled = true;
                DBvars.db_vars = Fitems.get_db_vars();
                Thread t = new Thread(new ThreadStart(load_XML));
                t.Priority = ThreadPriority.BelowNormal;
                t.IsBackground = true;
                t.Start();
                progressBar1.Value = 100;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            ((Button)sender).Enabled = false;
            webCam();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void homeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ce_fini();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            settings settings = new settings();            
            this.AddOwnedForm(settings);
            settings.Show();
        }

        private void manageTemplatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new manageUsers().ShowDialog();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox1().ShowDialog();
        }

        private void userLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new userlog().ShowDialog();
        }

    }
}
