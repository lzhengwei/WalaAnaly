using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WalkAnaly
{
    class Decode
    {
        public string openline;
        public string openfile;
        public int quotient;
        public int newline;
        public string filepath;

        public void decodetxt()
        {
            StreamReader sr = new StreamReader(filepath  + ".txt");
           
            while (!sr.EndOfStream)  // 每次讀取一行，直到檔尾
            {
                openline = sr.ReadLine();            // 讀取文字到 line 變數
            }
            sr.Close();
            quotient = openline.Length / 12;
            newline = quotient * 12;
            string xS = "", yS = "", zS = "";
            double dataSol = 1280.0;//2g=16384.0 / 4g=8192.0 / 8g=4096.0 / 16g=1280.0
            int x = 0;
            int num;
            double dnum;

            while (x < newline - 1)
            {

                string a = "", b = "", c = "";
                a = a + openline[x];
                x += 1;
                a = a + openline[x];
                x += 1;
                a = a + openline[x];
                x += 1;
                a = a + openline[x];
                x += 1;

                num = Int32.Parse(a, System.Globalization.NumberStyles.HexNumber);
                Console.WriteLine(num);
                if (num > 32768)
                {
                    num = num - 32768;
                    num = 32767 - num + 1;
                    num = num * -1;
                }
                dnum = (num * 9.8) / dataSol;
                xS = xS + dnum.ToString() + " ";

                b = b + openline[x];
                x += 1;
                b = b + openline[x];
                x += 1;
                b = b + openline[x];
                x += 1;
                b = b + openline[x];
                x += 1;

                num = Int32.Parse(b, System.Globalization.NumberStyles.HexNumber);
                Console.WriteLine(num);
                if (num > 32768)
                {
                    num = num - 32768;
                    num = 32767 - num + 1;
                    num = num * -1;
                }
                dnum = (num * 9.8) / dataSol;
                yS = yS + dnum.ToString() + " ";

                c = c + openline[x];
                x += 1;
                c = c + openline[x];
                x += 1;
                c = c + openline[x];
                x += 1;
                c = c + openline[x];
                x += 1;

                num = Int32.Parse(c, System.Globalization.NumberStyles.HexNumber);
                Console.WriteLine(num);
                if (num > 32768)
                {
                    num = num - 32768;
                    num = 32767 - num + 1;
                    num = num * -1;
                }
                dnum = (num * 9.8) / dataSol;//8192,
                zS = zS + dnum.ToString() + " ";
            }

            string dirPath = @"C:\accData";
            if (Directory.Exists(dirPath))
            {
                Console.WriteLine("The directory {0} already exists.", dirPath);
            }
            else
            {
                Directory.CreateDirectory(dirPath);
                Console.WriteLine("The directory {0} was created.", dirPath);
            }

            string xx = filepath + "-1.txt";
            string yy = filepath + "-2.txt";
            string zz = filepath + "-3.txt";
            StreamWriter sw = new StreamWriter(xx);
            sw.WriteLine(xS);            // 寫入文字
            sw.Close();
            StreamWriter aq = new StreamWriter(yy);
            aq.WriteLine(yS);            // 寫入文字
            aq.Close();
            StreamWriter af = new StreamWriter(zz);
            af.WriteLine(zS);            // 寫入文字
            af.Close();
        }
    }
}
