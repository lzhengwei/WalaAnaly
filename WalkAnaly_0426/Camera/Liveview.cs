using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Concurrent;
using System.Drawing;
using System.IO;

namespace WalkAnaly
{
    class Liveview
    {   
        private bool mWhileFetching;
        private int width, height;
        private String liveviewURI = "";
        Queue<Image> testqueue=new Queue<Image>(100);
       public BlockingCollection<Image> Imagequeue = new BlockingCollection<Image>(1000);
        BlockingCollection<Byte[]> JpegQueue = new BlockingCollection<Byte[]>(2);
        Image frameBitmap = null,returnimage;
        Thread Fetching, Drawing;
        Slicer slicer = null;
        public bool isruning = false;
        public int count = 0;

        public  Liveview(int W,int H)
        {
            width = W;
            height =H;
           
        }
        public void Start(String URI)
        {
            liveviewURI = URI;
            mWhileFetching = true;
            //Imagequeue.Add(Image.FromFile(Application.StartupPath + @"\..\..\resource\wait.gif"));
           // Fetching = new Thread(Fetchingtask);
           // Drawing = new Thread(Draingtask);
            //Fetching.Start();
           // Drawing.Start();
        }
        public void Close()
        {
            slicer.close();
            Imagequeue.Dispose();
            JpegQueue.Dispose();
        }
        private bool Fetchingtask()
        {
           
            
               
                slicer = null;
                slicer = new Slicer();
                
                try
                {
                    slicer.open(liveviewURI);
                    Slicer.Payload payload = slicer.nextPayload();
                    if (payload == null)
                    { // never occurs
                        MessageBox.Show("Liveview Payload is null.");
                        
                    }

                   JpegQueue.Add(payload.jpegData);
                    if (JpegQueue.Count >= 1)
                    {
                        byte[] jpegData = JpegQueue.Take();
                        var ms = new MemoryStream(jpegData);
                        Imagequeue.Add(Bitmap.FromStream(ms));
                        
                        //frameBitmap = Bitmap.FromStream(ms);
                        //testqueue.Enqueue(Bitmap.FromStream(ms));
                        
                        ms.Close();
                    }
                    count++;
                    slicer.close();
                    return true;
                }
                catch (Exception xx)
                {
                    return false;
                }
            
        }
        private void Draingtask()
        {
            while (true)
            {            
                //Graphics Graphicframe = Graphics.FromImage(frameBitmap);

                
            }
        }
        public Image getImage()
        {
            isruning = true;
            if (!Fetchingtask())
                return null;
            //Fetchingtask();

            isruning=false;
            return Imagequeue.Last();

            
        }

    }
}
