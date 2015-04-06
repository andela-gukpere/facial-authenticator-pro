using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using Luxand;
using System.Runtime.InteropServices;
using System.Net;
using System.Windows.Forms;

namespace Face
{
    public partial class attendance : Form
    {
        public attendance()
        {
            InitializeComponent();
        }

        private void attendance_Load(object sender, EventArgs e)
        {
            //timer1.Enabled = true;
           // label2.Text = "Current Camera Location" + new Home().location;
            FSDK.ActivateLibrary("7E92A165BF635F1767BF514733B46E3A4640821E9BD40FE7DCB716BBC33F219B9C8C6B346A8A58DF103DA9C29DE5E6F4E0DADFEDED0A026229DBDA7B6CA5F455");
            FSDK.InitializeLibrary();
            names = new List<string>();
            rooms = new List<string>();
            matrics = new List<string>();
            Allstudents = new List<string[]>();
            pres = new List<string>();
            faceTemplates = new List<byte[]>();
            new System.Threading.Thread(new System.Threading.ThreadStart(load_XML)).Start();
            new System.Threading.Thread(new System.Threading.ThreadStart(load_db)).Start();
          //  getStreams();
        }
        public int fsbmp = 0;
        public int cimg = 0;
       
        /*void getStreams()
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
        }*/
        public string temp = "";
        public void recFace(object pppp)
        {
            Bitmap ppp = (Bitmap)pppp;
            if (label1.InvokeRequired)
            {
                label1.Invoke(new System.Threading.ParameterizedThreadStart(recFace),new object[]{pppp});
                return;
            }
            bool error = false;
            try
            {
                IntPtr ff = ppp.GetHbitmap();
                FSDK.LoadImageFromHBitmap(ref cimg, ff);
                FSDK.SetFaceDetectionParameters(false, false, 500);
                FSDK.SetFaceDetectionThreshold(1);

                FSDK.TFacePosition facePosition = new FSDK.TFacePosition();

                IntPtr hbitmapHandle = IntPtr.Zero;

                FSDK.SaveImageToHBitmap(cimg, ref hbitmapHandle);
                Image ccimg = Image.FromHbitmap(hbitmapHandle);
                Graphics gr = Graphics.FromImage(ccimg);

                if (FSDK.FSDKE_OK == FSDK.DetectFace(cimg, ref facePosition))
                {
                    gr.DrawRectangle(Pens.LightBlue, facePosition.xc - facePosition.w / 2, facePosition.yc - facePosition.w / 2,
                    facePosition.w, facePosition.w);
                    byte[] tempd = new byte[FSDK.TemplateSize];
                    FSDK.GetFaceTemplateInRegion(cimg, ref facePosition, out tempd);
                    temp = System.Text.Encoding.ASCII.GetString(tempd);
                    if (check_reg(tempd))
                    {
                        timer2.Enabled = false;
                        StringFormat format = new StringFormat();
                        format.Alignment = StringAlignment.Center;
                        gr.DrawString(_name + " | " + _matric + " | " + _room, new System.Drawing.Font("Candara", 18),
                            new System.Drawing.SolidBrush(System.Drawing.Color.LightBlue),
                            facePosition.xc, facePosition.yc + facePosition.w * 0.55f, format);
                    }
                    else
                    {
                        timer2.Enabled = true;

                    }
                }
                else
                {
                    timer2.Enabled = false;
                    label1.Text = "Please Focus on the camera";
                }
                pictureBox1.Height = ccimg.Height;
                pictureBox1.Width = ccimg.Width;
                pictureBox1.Image = ccimg;
                FSDK.FreeImage(cimg);
                DeleteObject(hbitmapHandle);
                GC.Collect();
                Application.DoEvents();
            }
            catch (NullReferenceException nul)
            {
                timer1.Enabled = false;
                timer1.Dispose();
                MessageBox.Show(nul.ToString());
                error = true;
            }
            catch (ExternalException exer)
            {
            }
            catch (AccessViolationException acs)
            {

            }
            finally
            {
                if (error)
                {
                    canClose = true;
                    Application.Exit();
                    Application.Restart();
                }
            }
        }
        [DllImport("gdi32.dll")]
        static extern bool DeleteObject(IntPtr hObject);
        public void load_XML()
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load("db.xml");


            for (int i = 0; i < xdoc.GetElementsByTagName("temp").Count; i++)
            {
                XmlNode tnode = xdoc.GetElementsByTagName("temp")[i];
                string ttemp = tnode.FirstChild.Value;
                byte[] fe = System.Text.Encoding.ASCII.GetBytes(ttemp);
                faceTemplates.Add(fe);
                names.Add(tnode.PreviousSibling.FirstChild.Value);
                matrics.Add(tnode.ParentNode.ChildNodes[2].FirstChild.Value);
                rooms.Add(tnode.ParentNode.ChildNodes[3].FirstChild.Value);
            }
        }

        public List<byte[]> faceTemplates; 
        public List<String> names;
        public List<String> matrics;
        public List<string[]> Allstudents;
        public List<String> rooms;
        public List<String> pres;
        public bool check_reg(byte[] ttp)
        {
            bool val = false;
            foreach (byte[] t in faceTemplates)
            {
                float similarity = 0.0f;
                byte[] t1 = t;
                FSDK.MatchFaces(ref ttp, ref t1, ref similarity);
                float threshold = 0.0f;
                FSDK.GetMatchingThresholdAtFAR(0.01f, ref threshold); 
                if (similarity > threshold)
                {
                    val = true;
                    setV(t1);
                    break;
                }
            }

            return val;
        }

        public void setV(byte[] t)
        {
            for (int i = 0; i < faceTemplates.Count; i++)
            {
                if (faceTemplates[i] == t)
                {
                    _name = names[i];
                    _room = rooms[i];
                    _matric = matrics[i];
                    mark_student();
                    break;
                }
            }
        }

        public string _name = "";
        public string _matric = "";
        public string _room = "";
        bool Today = false;
        int nodeN = 0;
        public void load_db()
        {
            XmlDocument xmdoc = new XmlDocument();
            xmdoc.Load("dba.xml");
            XmlNodeList p =  xmdoc.GetElementsByTagName("cdt");
            for (int i = 0; i < p.Count; i++)
            {
                
                if (p[i].FirstChild.Value == DateTime.Now.ToShortDateString())
                {
                    Today = true;
                    nodeN = i;
                    List<string> sq = new List<string>();
                    foreach (XmlNode min in p[i].ParentNode.ChildNodes[1].ChildNodes)
                    {
                        sq.Add(min.FirstChild.Value);
                    }
                    Allstudents.Add(sq.ToArray<string>());
                }
                else
                {
                    Today = false;
                    nodeN = 0;
                }
            }
        }
        public void mark_student()
        {
            load_db();
            string dateString = DateTime.Now.ToShortDateString();
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load("dba.xml");
            bool doneT = false;
            foreach (var stl in Allstudents)
            {
                if (stl.Contains<string>(_matric))
                {
                    doneT = true;
                    break;
                }
                else
                {
                    doneT = false;
                }
            }
            if (!Today && nodeN == 0)
            {
                XmlElement date = xdoc.CreateElement("date");
                XmlElement dat_att = xdoc.CreateElement("cdt");
                dat_att.AppendChild(xdoc.CreateTextNode(dateString));
                date.AppendChild(dat_att);
                XmlElement matric = xdoc.CreateElement("matric");
                XmlElement students = xdoc.CreateElement("students");
                matric.AppendChild(xdoc.CreateTextNode(_matric));
                students.AppendChild(matric);
                date.AppendChild(students);
                
                if (!doneT && !pres.Contains(_matric))
                {

                    xdoc.GetElementsByTagName("attendance")[0].AppendChild(date);
                    pres.Add(_matric);
                    label1.Text = "";
                    label3.Text = "NAME:" + _name + "\n MATRIC:" + _matric + "\nhas been \nauthenticated";
                }
                else
                {
                    label1.Text = "";
                    label3.Text = "SORRY\nNAME:" + _name + "\nMATRIC:" + _matric + "\nhas already been \nauthenticated today";
                }                
            }
            else
            {
                XmlNode cdt = xdoc.GetElementsByTagName("cdt")[nodeN];
                XmlNode date = cdt.ParentNode;
                XmlElement matric = xdoc.CreateElement("matric");
                matric.AppendChild(xdoc.CreateTextNode(_matric));
                if (!doneT)
                {
                    date.ChildNodes[1].AppendChild(matric);
                    pres.Add(_matric);
                    label1.Text = "";
                    label3.Text = "NAME:" + _name + "\n MATRIC:" + _matric + "\nhas been \nauthenticated";
                }
                else
                {
                    label1.Text = "";
                    label3.Text = "SORRY\nNAME:" + _name + "\nMATRIC:" + _matric + "\nhas already been \nauthenticated today";
                }      
            }
            StreamWriter dtr = new StreamWriter("dba.xml");
            xdoc.Save(dtr.BaseStream);
            dtr.Flush();
            dtr.Close();
            dtr.Dispose();
        }
        bool _no_error = false;
        private void timer1_Tick(object sender, EventArgs e)
        {
            /*Bitmap pp = getStreams();
            if (pp == null)
            {
                canClose = true;
                Application.Exit();
                Application.Restart();
            }
            else
            {
                recFace(pp);
            }*/
          //  System.Threading.Thread aa = new System.Threading.Thread(new System.Threading.ThreadStart(getStreams));
           // aa.Start();
            //aa.Priority = System.Threading.ThreadPriority.Highest;
          //  getStreams();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Home ds = new Home();
            this.AddOwnedForm(ds);
            this.Hide();
            this.Enabled = false;
            timer1.Enabled = false;
            timer2.Enabled = false;
            ds.Show();
        }
        bool canClose = false;
        private void attendance_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.Visible == true && !canClose)
            {
                e.Cancel = true;
            }
            else
            {
                e.Cancel = false;
            }
            timer2.Enabled = false;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            Console.Beep(1000, 2000);
            label1.Text = "Unrecognized Individual";
            Application.DoEvents();
        }
    }
}
