using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WalkAnaly
{
    class Synchronization
    {
        public string openfile;
        public string filepath;
        public void synpreim(int x, int y, int width, int height)
        {
            string Opath = filepath + "\\images\\";
            Image bmp;
            bool imcolor = false;

            string[] fname = Directory.GetFiles(Opath);
            int[] fstim = new int[460800];
            int[] stdim = new int[460800];
            int[] thdim = new int[460800];
            for (int j = 0; j < fname.Length; j++)
            {
                bmp = new Bitmap(fname[j]);

                Rectangle CutAera = new Rectangle(0, 0, width, height);
                Bitmap newbmp = new Bitmap(width, height); //這個函數在後面有定義  
                Graphics tmpGraph = Graphics.FromImage(newbmp);
                tmpGraph.DrawImage(bmp, CutAera, new Rectangle(x, y, width, height), GraphicsUnit.Pixel);
                BitmapData dstBmData = newbmp.LockBits(CutAera, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                IntPtr dstPtr = dstBmData.Scan0;

                int dst_bytes = dstBmData.Stride * height;
                byte[] dstValues = new byte[dst_bytes];

                System.Runtime.InteropServices.Marshal.Copy(dstPtr, dstValues, 0, dst_bytes);
                //newbmp.Save("C:\\im\\" + "\\" + "test-" + j + ".jpg", ImageFormat.Jpeg);

                int index = 0;
                for (int i = 0; i < dstValues.Length; i += 3)
                {
                    int a = fstim[(dstValues.Length / 3) - 1];
                    if (fstim[(dstValues.Length / 3) - 1] == 0)
                    {
                        fstim[index] = dstValues[i + 1];       //[i + 1]綠
                        index++;
                    }
                    else
                    {
                        stdim[index] = fstim[index];
                        fstim[index] = dstValues[i + 1];
                        thdim[index] = fstim[index] - stdim[index];
                        if (thdim[index] >= 100 && dstValues[i + 1] >= 200 && dstValues[i + 1] >= dstValues[i] && dstValues[i + 1] >= dstValues[i + 2])
                        {
                            imcolor = true;
                            break;
                        }
                        index++;
                    }
                }                 

                System.Runtime.InteropServices.Marshal.Copy(dstValues​​, 0, dstPtr, dst_bytes);
                newbmp.UnlockBits(dstBmData);
                //newbmp.Save("C:\\im\\" + "\\" + "test-" + j + ".jpg", ImageFormat.Jpeg);

                bmp.Dispose();
                newbmp.Dispose();
                tmpGraph.Dispose();

                if (imcolor == false)
                    File.Delete(fname[j]);              
                else
                    break;
            }
        }

        public void synbackim(int x, int y, int width, int height)
        {
            string Opath = filepath + "\\images\\";
            Image bmp;
            bool imcolor = false;

            string[] fname = Directory.GetFiles(Opath);
            int[] fstim = new int[172800];
            int[] stdim = new int[172800];
            int[] thdim = new int[172800];
            for (int j = fname.Length - 1; j >= 0; j--)
            {
                bmp = new Bitmap(fname[j]);

                Rectangle CutAera = new Rectangle(0, 0, width, height);
                Bitmap newbmp = new Bitmap(width, height); //這個函數在後面有定義  
                Graphics tmpGraph = Graphics.FromImage(newbmp);
                tmpGraph.DrawImage(bmp, CutAera, new Rectangle(x, y, width, height), GraphicsUnit.Pixel);
                BitmapData dstBmData = newbmp.LockBits(CutAera, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                IntPtr dstPtr = dstBmData.Scan0;

                int dst_bytes = dstBmData.Stride * height;
                byte[] dstValues = new byte[dst_bytes];

                System.Runtime.InteropServices.Marshal.Copy(dstPtr, dstValues, 0, dst_bytes);
                //newbmp.Save("C:\\im\\" + "\\" + "test-" + j + ".jpg", ImageFormat.Jpeg);

                int index = 0;
                for (int i = 0; i < dstValues.Length; i += 3)
                {
                    int a = fstim[(dstValues.Length / 3) - 1];
                    if (fstim[(dstValues.Length / 3) - 1] == 0)
                    {
                        fstim[index] = dstValues[i + 1];       //[i + 1]綠
                        index++;
                    }
                    else
                    {
                        stdim[index] = fstim[index];
                        fstim[index] = dstValues[i + 1];
                        thdim[index] = fstim[index] - stdim[index];
                        if (thdim[index] >= 100)
                        {
                            imcolor = true;
                            break;
                        }
                        index++;
                    }
                }

                System.Runtime.InteropServices.Marshal.Copy(dstValues​​, 0, dstPtr, dst_bytes);
                newbmp.UnlockBits(dstBmData);
                //newbmp.Save("C:\\im\\" + "\\" + "test-" + j + ".jpg", ImageFormat.Jpeg);

                bmp.Dispose();
                newbmp.Dispose();
                tmpGraph.Dispose();

                if (imcolor == false)
                    File.Delete(fname[j]);
                else
                    break;
            }
        }
    }
}
