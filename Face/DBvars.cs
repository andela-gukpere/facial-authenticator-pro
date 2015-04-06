using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Face
{
    public class DBvars
    {
        private static object[] DB_vars = null;
        public static object[] db_vars
        {
            get { return DB_vars; }
            set { DB_vars = value;}
        }
        public static string[] FSDK_KEY = {
                                              
"NjFJS4VQUZ+2zvJ5T/qe37jwRbH+yieWBQhUUeXV5YLRDIpwk/dK+ozAFj3PHgoZRzQtD4PfMUk1fSszVKZqOX2C1Rg1W1z3QISdHzgihwW0xldUnMLHGJbqZScf/HXQ4CMblB2gBn3WvtW7BgyfpvysfDlwv2tgVg6U3Q7ti44=",
"lrXRvdKVpuK3rp0Km7htgxXSEr1vv9gRNDskTcyeDGQvKAclnvfhNyAyZHMMPmSGV0ruj3+ENvEMe5zOCVqjOERIyNZr/wD3I6I58vwJmbVs8CRHYUYBlWIrLHzOhML93hp0Kk3R97QEBvna2CS6qp81rGoG4Kesk3pHYXJFoe0=",
"Z9wVY0NbyLIRgyATzlnERnWKKwkBmDASUFZuE3ziey+cwBkiycLufAauVroXLDmhHWgh5tkZo3l88r+X1My7wqm5NL/N27wLANFeBDeVO0nxRBTnr6XJmk1S1NsiOs5BOh7EMq7s1vpZ91q5bMxPcssb/MXhiV579QAfnqjI2HQ="};
    }
}

/*
           try
           {
               faceTemplates.Clear();
               XmlDocument xdoc = new XmlDocument();
               //XmlReader xmlr = XmlReader.Create("db.xml");
               xdoc.Load("db.xml");
               for (int i = 0; i < xdoc.GetElementsByTagName("temp").Count; i++)
               {
                   XmlNode tnode = xdoc.GetElementsByTagName("temp")[i];
                   string ttemp = tnode.FirstChild.Value;
                   byte[] fe = System.Text.Encoding.ASCII.GetBytes(ttemp);
                   faceTemplates.Add(fe);
                   names.Add(tnode.PreviousSibling.FirstChild.Value);
                   emails.Add(tnode.ParentNode.ChildNodes[2].FirstChild.Value);
                   date.Add(tnode.ParentNode.ChildNodes[3].FirstChild.Value);
                   Application.DoEvents();
               }
           }
           catch (NullReferenceException nullref)
           {
               Application.DoEvents();
               Console.WriteLine(nullref.ToString());
           }
        //   xmlr.Close();
            * */
/* public void recFace(object pppp)
       {
           Bitmap ppp = (Bitmap)pppp;
           if (button2.InvokeRequired)
           {
               button2.Invoke(new System.Threading.ParameterizedThreadStart(recFace), new object[] { pppp });
               return;
           }
           try
           {
               FaceTemplate template = new FaceTemplate();
               template.templateData = new byte[FSDK.TemplateSize];

               IntPtr ff = ppp.GetHbitmap();
               FSDK.LoadImageFromHBitmap(ref cimg, ff);
               FSDK.SetFaceDetectionParameters(false, false, 300);
               FSDK.SetFaceDetectionThreshold(2);

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
                  // temp = System.Text.Encoding.ASCII.GetString(tempd);
                   if (check_reg(tempd))
                   {
                       StringFormat format = new StringFormat();
                       format.Alignment = StringAlignment.Center;
                       gr.DrawString(_name + " | " + _email + " | " + _date, new System.Drawing.Font("Candara", 18),
                           new System.Drawing.SolidBrush(System.Drawing.Color.LightBlue),
                           facePosition.xc, facePosition.yc + facePosition.w * 0.55f, format);
                   }
                   button2.Enabled = true;
               }
               else
               {
                   button2.Enabled = false;
               }
               if (!held)
               {
                   pictureBox1.Height = ccimg.Height;
                   pictureBox1.Width = ccimg.Width;
                   pictureBox1.Image = ccimg;

               }
               FSDK.FreeImage(cimg);
               DeleteObject(hbitmapHandle);
               GC.Collect();
               Application.DoEvents();
               new System.Threading.Thread(new System.Threading.ThreadStart(getStreams)).Start();
           }
           catch (NullReferenceException nulr)
           {
               timer1.Enabled = false;
               MessageBox.Show(nulr.Message);
               canClose = true;
               Application.Exit();
               //Application.Restart();
           }
           catch (AccessViolationException acs)
           {

           }
           catch (ExternalException exrer)
           {
           }
       }
       * */
//under webcam function
/* FSDKCam.InitializeCapturing();


            
            MessageBox.Show(cameraList[0]);
            
            
            this.Width = formatList[0].Width + 48;
            this.Height = formatList[0].Height + 116;
                       
            
            cameraName = cameraList[0];

            int cameraHandle = 0;
            
         
            int r = FSDKCam.OpenVideoCamera(ref cameraName, ref cameraHandle);
            if (r != FSDK.FSDKE_OK)
            {
                MessageBox.Show("Error opening the first camera", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }*/
//  string[] cameraList;
// int count;
// FSDKCam.GetCameraList(out cameraList, out count);

//            if (0 == count)
//          {
//            MessageBox.Show("Please attach a camera", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//          Application.Exit();
//    }