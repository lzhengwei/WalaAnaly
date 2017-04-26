using System;
using System.Diagnostics;//引用執行緒類別
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

using MathWorks.MATLAB.NET.Arrays;
using dlltest;
using System.Windows.Forms.DataVisualization.Charting;
using NativeWifi;
using System.Data;

namespace WalkAnaly
{
    public partial class Form1 : Form
    {
        Synchronization syn = new Synchronization();
        Decode decode = new Decode();
        string username = "",selectfoldername="";
        string Datasavepath ;
        string newrecordname;
        int testTime = 0;
        int[] Photoarray = new int[200];
        int photoindex = 0,photoarraylength=0,BTdelayindex=60;

        public Form1()
        {                       
            InitializeComponent();
            string MyDocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            Datasavepath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)+@"\WalkAnaly";
            DirectoryInfo dir = new DirectoryInfo(Datasavepath);
            if (!dir.Exists)
                Directory.CreateDirectory(Datasavepath);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Thread cameraWifithread = new Thread(cameraWificonnect);
            updateUI("Bluetoothstate", 9); 
            updateUI("Bluetoothstate", 5); 
            updateUI("Camerawififalse", 3);
            buttonstop.Enabled = false;
            if (!File.Exists(Datasavepath + @"\BTdelay.txt"))
            {
                File.WriteAllText(Datasavepath + @"\BTdelay.txt", "60");               
               
            }
            StreamReader BTdelayFile = new StreamReader(Datasavepath + @"\BTdelay.txt", Encoding.Default);
                BTdelayindex = Int32.Parse(BTdelayFile.ReadLine());            
            BTdelayFile.Close();
            //TimerCallback callback = new TimerCallback(TimerTask);
            //Phototimer = new System.Threading.Timer(callback, null, 0,1000);
          //  cameraWifithread.Start();
        }
        private void TimerTask(object sender,MicroLibrary.MicroTimerEventArgs timerEventArgs)
        {
            photoindex++;
            if (!BluetoothOK && photoindex%600==0)
            {

                Thread btconnect = new Thread(bttest);
                btconnect.Start();
            }
 
          
            //if (photoindex %20==0)

        }
        #region Video to picture & decode
        //影片轉圖片
        bool gifstart = false;
        WalkAnaly.Camera.CameraUSB Usbcontrol = new WalkAnaly.Camera.CameraUSB();
        private void button1_Click(object sender, EventArgs e)
        {
            if (Usbcontrol.CameraUSBcheck())
            {
                if (testTime==1)
                {
                    Usbcontrol.Mp4fileCopy(newrecordname, false);

                    updateUI(" ", 6);
                }
                else if (testTime > 1)
                {
                    //ShowMyPictureBox();
                    DirectoryInfo Namelistdir = new DirectoryInfo(Datasavepath + "\\" +username);
                    Namelist.Clear();
                    listname(Namelistdir);
                    for (int i = Namelist.Count - 1; i >= Namelist.Count-Usbcontrol.numofMp4file; i--)
                    {
                        Usbcontrol.Mp4fileCopy(Datasavepath + "\\" + username+"\\"+Namelist[i]+"\\", true);
                    }
                    DirectoryInfo dir = new DirectoryInfo(Datasavepath );
                    Namelist.Clear();
                    listname(dir);
                    namelistBox.DataSource = null;
                    namelistBox.DataSource = Namelist;
                   // updateUI(" ", 7);
                }
            }
            else
            {
                MessageBox.Show("沒有可讀取的相機");
            }
        }
        #endregion

        #region informations & pictures
        string line;
        string[] result;
        PointF[] rightarray;
        double computing = 0;
        double datarate = 30;
        private void button3_Click(object sender, EventArgs e)
        {
            DirectoryInfo dir = new DirectoryInfo(Datasavepath);
            datanamelist.Clear();          
            datafolderlist.Clear();
            listfile(dir);
            new Thread(new ThreadStart(delegate
            {
                showListbox(1);
            })).Start();                  
        }
        private void hScrollBar1_ValueChanged(object sender, EventArgs e)
        {
           // createchart(true);
            Analychart.ChartAreas[0].AxisX.StripLines.Clear();
            addLateline(Analychart, hScrollBar1.Value);
            ScrollShowimage();
        }
        #endregion

        private double[] photoRatio;
        private int[] photoSum ;
        private string[] fname;
        #region ChartView function
        public void ScrollShowimage()
        {
            string Opath = newrecordname + "images\\";
            double resultcomputing;
       
            string lastnumber = fname[0].Substring(fname[0].Length - 8, 4);
            //resultcomputing = (int)(hScrollBar1.Value * computing) + Int32.Parse(lastnumber);

            //resultcomputing = photoSum[(int)(hScrollBar1.Value / 120)] + (int)((hScrollBar1.Value % 120) * photoRatio[(int)(hScrollBar1.Value / 120)]) + Int32.Parse(lastnumber);
            resultcomputing = photoSum[(int)(hScrollBar1.Value / datarate)] + (int)((hScrollBar1.Value % datarate) * photoRatio[(int)(hScrollBar1.Value / datarate)])+photogap;
            string lastfilenumber = fname[fname.Length - 1].Substring(fname[fname.Length - 1].Length - 8, 4);
            Transferbutton.Text = resultcomputing.ToString() + " " + hScrollBar1.Value.ToString();
            if (resultcomputing > 0)
            {
                if (File.Exists(fname[(int)resultcomputing]))
                    AnalypictureBox.Image = Image.FromFile(fname[(int)resultcomputing]);
            }
          
        }
        private void createchart(bool clearornot)
        {
            if (clearornot)
            {
                Analychart.ChartAreas.Clear();
                Analychart.Legends.Clear();
                Analychart.Series.Clear();
                Analychart.ChartAreas.Add("ChartArea1"); //圖表區域集合
                Analychart.ChartAreas["ChartArea1"].AxisX.Interval = 100;
                Analychart.ChartAreas["ChartArea1"].AxisX.Minimum = 0;
                Analychart.ChartAreas["ChartArea1"].AxisX.Maximum = result.Length - 1 + 20;
                Analychart.ChartAreas["ChartArea1"].AxisY.MajorGrid.LineColor = Color.FromArgb(150, 150, 150);
            }


            Series series1 = new Series();
            series1.ChartType = SeriesChartType.Line;
            for (int index = 0; index < result.Length; index++)
            {
                series1.Points.AddXY(index, result[index]);
            }
            Analychart.Series.Add(series1);
        }

        private void addLateline(Chart chart, int strLate)
        {
            StripLine stripline = new StripLine();
            stripline.Interval = 0;
            stripline.IntervalOffset = strLate * 1;
            stripline.StripWidth = 1;
            stripline.BackColor = Color.Red;
            stripline.BorderDashStyle = ChartDashStyle.Dash;
            chart.ChartAreas[0].AxisX.StripLines.Add(stripline);
            
        }

        #endregion

        #region Connect
        Bluetooth bluetooth1,bluetooth2;
        Liveview liveview;
        Thread LiveviewThread;
        Boolean Liveviewrunning = false,BluetoothOK=false,WifiOK=false;
        private void button5_Click_1(object sender, EventArgs e)
        {
            microTimer = new MicroLibrary.MicroTimer();
            microTimer.Interval = 8333; // Call micro timer every 1000µs (1ms)
            microTimer.MicroTimerElapsed +=
            new MicroLibrary.MicroTimer.MicroTimerElapsedEventHandler(TimerTask);
            microTimer.Enabled = true;
            WifiOK = false;
            BluetoothOK = false;
            //====================================== Camera ================================================================


            //====================================== Blue tooth ============================================================
            bluetooth1 = new Bluetooth("20:16:04:11:03:00"); //right BT
            bluetooth2 = new Bluetooth("20:16:04:11:12:99"); //left BT
            bluetooth1.pairDevice();
            bluetooth2.pairDevice();
            try
            {

                bluetooth1.ClientConnectThread();
                bluetooth2.ClientConnectThread();
                
                connectrun = true;
                client=new WlanClient();
                if (client.Interfaces.Count() == 0)
                {
                    MessageBox.Show("無法讀取無線網路介面卡\nCan't find Wlan Interfaces");
                }
                else
                {
                    wlanIface = client.Interfaces[0];
                    Thread Wificonnect = new Thread(cameraWificonnect);
                    Wificonnect.Start();
                }
            }
            catch (IOException x)
            {
                updateUI("Pair failed", 0);
                

            }
            buttonConnect.Enabled = false;
        }
        CameraControl cameracontrol = new CameraControl();
        byte[] message;
        string d;
        int mode = 0;
        float time_camera, time_bt;
        String stoptime = "", receiveString = "";
        Thread BTreceiveThread_a;
        System.Threading.Timer Phototimer;
        MicroLibrary.MicroTimer microTimer ;
        private void button7_Click_1(object sender, EventArgs e)
        {
            //timer1.Start();
            showListbox(0);
            Photoarray = new int[200];
            photoindex = 0; photoarraylength = 0;
            bttest();
            if ( Itemselect && BluetoothOK && WifiOK)
            {
                buttonstop.Enabled = true;
                buttonstart.Enabled = false;

                testTime++;
                photoindex = 0;

                mode = 0;
                Thread Bluetooth_start = new Thread(Bluetoothstarttask);
                Thread Camera_start = new Thread(Camerastarttask);


                Bluetooth_start.Start();
                Camera_start.Start();
                selectfoldername = "";
            }
        }
        private void button8_Click_1(object sender, EventArgs e)
        {
            buttonstart.Enabled = true;
            buttonstop.Enabled = false;
            mode = 1;
            Thread Bluetooth_start = new Thread(Bluetoothstarttask);
            Bluetooth_start.Start();
            Thread Camera_start = new Thread(Camerastarttask);
            Camera_start.Start();
            String photostring="";
            for (int i = 0; i < photoarraylength; i++)
            {
                photostring +=Photoarray[i]+"\n";
            }
            System.IO.StreamWriter file =
                new System.IO.StreamWriter(Datasavepath +"\\photoarray.txt");
            file.WriteLine(photostring);
            file.Close();


        }
        #endregion

        #region Bluetooth & Wifi function 
       
        private void Liveviewtask()
        {
            try
            {
                while (liveview.Imagequeue.Count <= 30 && connectrun)
                {
                        LiveviewpictureBox.Image = liveview.getImage();
                        if (LiveviewpictureBox.Image == null)
                        {
                            Thread Wificonnect = new Thread(checkWifistate);
                           // Wificonnect.Start(); 
                            Thread.Sleep(2000); 
                            break;
                        }
                        else
                            Thread.Sleep(200);
                }
                if (connectrun)
                {
                    liveview = new Liveview(LiveviewpictureBox.Width, LiveviewpictureBox.Height);
                    liveview.Start("d");
                    LiveviewThread = new Thread(Liveviewtask);                  
                    LiveviewThread.Start();
                }
            }
            catch (Exception xx)
            {
                liveview = new Liveview(LiveviewpictureBox.Width, LiveviewpictureBox.Height);
                liveview.Start("d");
                LiveviewThread = new Thread(Liveviewtask);
                LiveviewThread.Start();
                Thread Wificonnect = new Thread(checkWifistate);
                Wificonnect.Start(); 
            }
        }

        private void Bluetoothstarttask()
        {
            switch (mode)
            {
                case 0:
                    d = "a";
                    while (!cameracontrol.issendMessage) ;
                    Thread.Sleep(500);
                    BTreceiveThread_a = new Thread(BTreceivetask);
                    BTreceiveThread_a.Start();
                    Thread BTreceiveThread_b = new Thread(BTreceivetask2);
                    BTreceiveThread_b.Start();
                    
                    if (bluetooth1.stream != null)
                    {
                        message = Encoding.ASCII.GetBytes("a");
                        bluetooth1.stream.Write(message, 0, message.Length);

                    }
                    int compare = photoindex;
                    while (Math.Abs(compare - photoindex) <= BTdelayindex) ;
                    if (bluetooth2.stream != null)
                    {
                        message = Encoding.ASCII.GetBytes("a");
                        bluetooth2.stream.Write(message, 0, message.Length);
                    }

                    break;
                case 1:
                    d = "o";
                    message = Encoding.ASCII.GetBytes(d);
                    
                    if (bluetooth1.stream!=null)
                        bluetooth1.stream.Write(message, 0, message.Length);
                    if (bluetooth2.stream != null)
                        bluetooth2.stream.Write(message, 0, message.Length);
                    break;
            }
        }
        private void Camerastarttask()
        {
            switch (mode)
            {
                case 0:
                    cameracontrol.ControlCamera("");
                    String startcamera = cameracontrol.StartMovieRec();
                    time_camera = cameracontrol.time_camera;
                    break;
                case 1:
                    String stopcamera = cameracontrol.StopMovieRec();

                    stoptime += " " + cameracontrol.time_camera.ToString() + "cc";               
                    break;
            }
          //  updateUI("\n" + " bt " + time_bt.ToString() + " ca " + time_camera.ToString(), 1);
        }
        WlanClient client;
        WlanClient.WlanInterface wlanIface;
        private void cameraWificonnect()
        {
            //Timer clock;
            try
            {


                   // string profileName = "DIRECT-AFH4:HDR-AS100V"; // this is also the SSID
                   // string key = "BK8Brwei";
                    string profileName = "DIRECT-yJH4:HDR-AS100V"; // this is also the SSID
                    string key = "zpp1FHcR";
                    string profileXml = string.Format("<?xml version=\"1.0\" encoding=\"US-ASCII\"?><WLANProfile xmlns=\"http://www.microsoft.com/networking/WLAN/profile/v1\"><name>{0}</name><SSIDConfig><SSID><name>{0}</name></SSID></SSIDConfig><connectionType>ESS</connectionType><connectionMode>auto</connectionMode><autoSwitch>false</autoSwitch><MSM><security><authEncryption><authentication>WPA2PSK</authentication><encryption>AES</encryption><useOneX>false</useOneX></authEncryption><sharedKey><keyType>passPhrase</keyType><protected>false</protected><keyMaterial>{1}</keyMaterial></sharedKey></security></MSM></WLANProfile>", profileName, key);

                    Wlan.WlanAvailableNetwork[] netwroks = wlanIface.GetAvailableNetworkList(0);

                    bool isconnect = false;
                    foreach (Wlan.WlanAvailableNetwork network in netwroks)
                    {
                        if (network.profileName == profileName)
                        {
                            if (wlanIface.InterfaceState.ToString()[0] == 'C')
                            {
                                if (wlanIface.CurrentConnection.profileName == profileName)
                                {

                                    isconnect = true;
                                    
                                }
                            }
                        }
                    }
                    if (!isconnect)
                    {
                        wlanIface.SetProfile(Wlan.WlanProfileFlags.AllUser, profileXml, true);
                        wlanIface.Connect(Wlan.WlanConnectionMode.Profile, Wlan.Dot11BssType.Any, profileName);
                    }
                
            }
            catch { 
            }
            Thread.Sleep(3000);
            if (tabSelectedIndex == 0)
            {
                Thread Wificonnect = new Thread(checkWifistate);
               // Wificonnect.Start();   
                if (Liveviewrunning == false)
                {
                    
                    liveview = new Liveview(LiveviewpictureBox.Width, LiveviewpictureBox.Height);
                    liveview.Start("d");
                    LiveviewThread = new Thread(Liveviewtask);
                    Liveviewrunning = true;
                    LiveviewThread.Start();
                    checkWifistate();

                }      
            }
        }
        bool connectrun=true;
        
        private void checkWifistate()
        {
            bool wifiisconnect;
            //string profileName = "DIRECT-AFH4:HDR-AS100V"; // this is also the SSID
            string profileName = "DIRECT-yJH4:HDR-AS100V"; // this is also the SSID
           
                          
                //Timer clock;
                wifiisconnect = true;
                try
                {
                    

                        if (wlanIface.InterfaceState.ToString()[0] != 'C')
                        {
                            updateUI("Camerawififalse", 3);
                            wifiisconnect = false;
                            cameraWificonnect();

                            //break;
                        }
                        else
                        {
                            if (wlanIface.CurrentConnection.profileName != profileName)
                            {
                                updateUI("Camerawififalse", 3);
                                wifiisconnect = false;
                                cameraWificonnect();

                                //break;
                            }
                        }
                    
                }
                catch 
                {
                    
                }
                    if (wifiisconnect)
                    {
                       
                        updateUI("Camerawifitrue", 2);
                        WifiOK = true;
                        if (Liveviewrunning == false)
                        {
                            liveview = new Liveview(LiveviewpictureBox.Width, LiveviewpictureBox.Height);
                            liveview.Start("d");
                            LiveviewThread = new Thread(Liveviewtask);
                            Liveviewrunning = true;
                            LiveviewThread.Start();
                            
                        }      

                    }
                                                                 
                Thread.Sleep(3000);
             
        }
        private void bttest()
        {
            try
            {
                message = Encoding.ASCII.GetBytes("c");
                bluetooth1.stream.Write(message, 0, message.Length);
                updateUI("Bluetoothstate", 4);
                bluetooth1.BTconnected = true;
                if (photoindex >= Int32.MaxValue - 10)
                    photoindex = 0;
            }
            catch (Exception x)
            {
                bluetooth1.BTconnected = false;
                updateUI("Bluetoothstate", 5);
                //MessageBox.Show("藍芽連接錯誤，請重新點選連接按鈕");
                bluetooth1.ClientConnectThread();
                //break;
            }
           try
            {
                message = Encoding.ASCII.GetBytes("c");
                bluetooth2.stream.Write(message, 0, message.Length);
                updateUI("Bluetooth2connect", 8);
                bluetooth2.BTconnected = true;
            }
            catch (Exception x)
            {
                bluetooth2.BTconnected = false;
                updateUI("Bluetooth2Disconnect", 9);
                //MessageBox.Show("藍芽連接錯誤，請重新點選連接按鈕");
                bluetooth2.ClientConnectThread();
                //break;
            }
           if (!bluetooth2.BTconnected || !bluetooth1.BTconnected)
               BluetoothOK = false;
           else if(bluetooth1.BTconnected && bluetooth1.BTconnected)
               BluetoothOK = true;

        }
        public void BTreceivetask()
        {
            String a;
            byte[] result=new byte[1024];
            bool notenda = true;
            
            int[] bt2photoarray = new int[100];
            
           // MemoryStream receivestream = new MemoryStream();
            while (notenda && bluetooth1.stream != null)
            {
                try
                {
                    //handle server connection
                    using (var receivestream = new MemoryStream())
                    {
                        byte[] received = new byte[1024];
                        int readlength = bluetooth1.stream.Read(received, 0, received.Length);
                        if (readlength > 0)
                        {
                            receivestream.Write(received, 0, readlength);
                            result = receivestream.ToArray();
                        }
                    }
                    //bluetooth.stream.Read(received, 0, readlength);
                    a = Encoding.ASCII.GetString(result);

                    if (a.Contains("a"))
                    {
                        Photoarray[photoarraylength] = photoindex;
                        photoarraylength++;
                        time_bt = float.Parse(DateTime.Now.ToString("ss.fff"));
                        updateUI("開始行走", 1);
                    }
                    else if (a.Contains("o"))
                    {
                        notenda = false;
                        stoptime += " " + cameracontrol.time_camera.ToString() + "cc";
                    }
                    else if (notenda)
                    {

                        if (a.Contains("p"))
                        {
                            Photoarray[photoarraylength] = photoindex;
                            photoarraylength++;

                            String removeString = a;
                            while (removeString.Contains("p"))
                            {
                                for (int i = 0; i < removeString.Length; i++)
                                {
                                    if (removeString[i] == 'p')
                                    {
                                        removeString = removeString.Remove(i, 1);
                                        break;
                                    }
                                }
                            }
                            receiveString += removeString;
                        }
                        else
                            receiveString += a;
                    }
                }
                catch (IOException exception)
                {
                }
            }
            String dataname = DateTime.Now.ToString("MMdd_HHmm");
            updateUI("測試結束", 1);
            newrecordname = Datasavepath + "\\" + username + "\\" + dataname+"\\";
            if (!Directory.Exists(newrecordname))
                Directory.CreateDirectory(newrecordname);
            else
            {
                dataname+=testTime.ToString();
                newrecordname = Datasavepath + "\\" + username + "\\" + dataname + "\\";
                Directory.CreateDirectory(newrecordname);
            }
            selectfoldername = dataname;
            File.Create(newrecordname+dataname+ ".wan");
            System.IO.StreamWriter file =
            new System.IO.StreamWriter(newrecordname   + dataname + ".txt");
            file.WriteLine(receiveString);
            file.Close();
            receiveString = "";
            decode.filepath = newrecordname+dataname;
            //double[] Photosignalratio = new double[photoarraylength];
            for (int i = 0; i < photoarraylength - 1; i++)
            {
                receiveString+= (Photoarray[i + 1] - Photoarray[i]) +"\n";
            }
            System.IO.StreamWriter ratiofile =
            new System.IO.StreamWriter(newrecordname + dataname + "_syn.txt");
            ratiofile.WriteLine(receiveString);
            ratiofile.Close();
            receiveString = "";
            for (int i = 0; i < photoarraylength - 1; i++)
            {
                receiveString += Photoarray[i] + "\n";
            }
            System.IO.StreamWriter photofile =
            new System.IO.StreamWriter(newrecordname + dataname + "_photo.txt");
            photofile.WriteLine(receiveString);
            photofile.Close();
            receiveString = "";
        }
        public void BTreceivetask2()
        {
            String a, b, receivestringb = "";
            byte[] result = new byte[1024];
            bool notenda = true, notendb = true;
            int bt2photolength = 0;
            int[] bt2photoarray = new int[100];
            
            // MemoryStream receivestream = new MemoryStream();
            while (notendb && bluetooth2.stream != null)
            {               
                try
                {
                    using (var receivestream = new MemoryStream())
                    {
                        byte[] received = new byte[1024];
                        int readlength = bluetooth2.stream.Read(received, 0, received.Length);
                        if (readlength > 0)
                        {
                            receivestream.Write(received, 0, readlength);
                            result = receivestream.ToArray();
                        }
                    }
                    //bluetooth.stream.Read(received, 0, readlength);
                    b = Encoding.ASCII.GetString(result);

                    if (b.Contains("a"))
                    {
                        bt2photoarray[bt2photolength] = photoindex;
                        bt2photolength++;
                        time_bt = float.Parse(DateTime.Now.ToString("ss.fff"));
                        //updateUI("開始行走", 1);
                    }
                    else if (b.Contains("o"))
                    {
                        notendb = false;
                        stoptime += " " + cameracontrol.time_camera.ToString() + "cc";
                    }
                    else if (notendb)
                    {

                        if (b.Contains("p"))
                        {
                            //Photoarray[photoarraylength] = photoindex;
                            // photoarraylength++;
                            bt2photoarray[bt2photolength] = photoindex;
                            bt2photolength++;
                            String removeString = b;
                            while (removeString.Contains("p"))
                            {
                                for (int i = 0; i < b.Length; i++)
                                {
                                    if (removeString[i] == 'p')
                                    {
                                        removeString = removeString.Remove(i, 1);
                                        break;
                                    }
                                }
                            }

                            receivestringb += removeString;
                        }
                        else
                            receivestringb += b;
                    }
                }
                catch
                {
                    
                }
            }
            while (selectfoldername == "") ;
            System.IO.StreamWriter fileb =
            new System.IO.StreamWriter(newrecordname + selectfoldername + "b.txt");
            fileb.WriteLine(receivestringb);
            fileb.Close();
            receivestringb = "";

            //double[] Photosignalratio = new double[photoarraylength];
            for (int i = 0; i < bt2photolength - 1; i++)
            {
                receivestringb += (bt2photoarray[i + 1] - bt2photoarray[i]) + "\n";
            }
            System.IO.StreamWriter ratiofileb =
            new System.IO.StreamWriter(newrecordname + selectfoldername + "b_syn.txt");
            ratiofileb.WriteLine(receivestringb);
            ratiofileb.Close();
            receivestringb = "";
            for (int i = 0; i < bt2photolength - 1; i++)
            {
                receivestringb += bt2photoarray[i] + "\n";
            }
            System.IO.StreamWriter photofileb =
            new System.IO.StreamWriter(newrecordname + selectfoldername + "b_photo.txt");
            photofileb.WriteLine(receivestringb);
            photofileb.Close();
        }
        #endregion

        #region Update UI

        public void updateUI(string message, int mode)
        {
            Thread p = new Thread(VideotoPicture);
            Func<int> del;
            switch (mode)
            {
                case 0:
                    del = delegate()
                    {
                        return 0;
                    };
                    Invoke(del);
                    break;
                case 1:
                    del = delegate()
                    {
                        if (message.Equals("測試結束"))
                            label1.ForeColor = Color.Red;
                        else
                            label1.ForeColor = Color.Lime;
                        label1.Text = message;
                        return 0;
                    };
                    Invoke(del);
                    break;
                case 2:
                    del = delegate()
                    {
                        wifi_pictureBox.Image = Image.FromFile(Application.StartupPath + @"\..\..\resource\connectWIFI.png");
                        return 0;
                    };
                    Invoke(del);
                    break;
                case 3:
                    del = delegate()
                    {
                        wifi_pictureBox.Image = Image.FromFile(Application.StartupPath + @"\..\..\resource\disconnectWIFI.png");
                        return 0;
                    };
                    Invoke(del);
                    break;
                case 4:
                    del = delegate()
                    {
                        BT1_pictureBox.Image = Image.FromFile(Application.StartupPath + @"\..\..\resource\connectBT.png");
                        return 0;
                    };
                    Invoke(del);
                    break;
                case 5:
                    del = delegate()
                    {
                        BT1_pictureBox.Image = Image.FromFile(Application.StartupPath + @"\..\..\resource\disconnectBT.png");
                        return 0;
                    };
                    Invoke(del);
                    break;
                case 6:
                    del = delegate()
                    {
                        // Read the file and display it line by line.
                        DirectoryInfo dir = new DirectoryInfo(newrecordname);
                        string txtname=listfile(dir);
                        string Opath = newrecordname + "images\\";
                        
                        if (!Directory.Exists(Opath))
                        {
                            p.Start();

                            ShowMyPictureBox();
                            Decodetxt(newrecordname + txtname);
                        }
                        if (File.Exists(newrecordname + txtname + "butterworth.txt"))
                        {
                            ChartPrepare(newrecordname + txtname,true);                           
 
                        }
                        else
                        {
                            if (File.Exists(newrecordname + txtname + ".txt"))
                                Decodetxt(newrecordname + txtname);
                            updateUI("", 6);
                            return 0;
                        }
                        if (!File.Exists(newrecordname + txtname + "bbutterworth.txt"))
                        {
                            if (File.Exists(newrecordname + txtname + "b.txt"))
                                Decodetxt(newrecordname + txtname + "b");
                        }


                         fname = Directory.GetFiles(Opath);
                         hScrollBar1.Value = 1;
                        ScrollShowimage();
                        return 0;
                    };
                    Invoke(del);
                    break;
                case 7:
                    del = delegate()
                    {
                        waitform.Close();
                        return 0;
                    };
                    Invoke(del);
                    break;
                case 8:
                    del = delegate()
                    {
                        BT2_pictureBox.Image = Image.FromFile(Application.StartupPath + @"\..\..\resource\connectBT.png");
                        return 0;
                    };
                    Invoke(del);
                    break;
                case 9:
                    del = delegate()
                    {
                        BT2_pictureBox.Image = Image.FromFile(Application.StartupPath + @"\..\..\resource\disconnectBT.png");
                        return 0;
                    };
                    Invoke(del);
                    break;
            }
        }
        private void Decodetxt(String dir)
        {
            decode.filepath = dir;
            decode.decodetxt();
            line = "";
            double law = 0, high = 15.0 / (120 / 2);
            ButterWorth t = new ButterWorth();
            MWArray infilename =dir + "-1.txt";
            MWArray outfilename = dir + "butterworth.txt";
            MWArray filterOrder = 8, lawpass = law, highpass = high;
            MWArray[] test;
            t.myfunc(2, infilename, outfilename, filterOrder, lawpass, highpass);
        }
        int photogap = 0;
        private void ChartPrepare(String dir,bool clearornot)
        {
            String read = "";
            if (File.Exists(dir + "_photo.txt"))
            {
                StreamReader synfile = new StreamReader(dir + "_photo.txt", Encoding.Default);
                
                photoarraylength = 0;
                Photoarray = new int[200];
                while (!synfile.EndOfStream)
                {
                    read = synfile.ReadLine();
                    if (read != "")
                    {
                        Photoarray[photoarraylength] = Int32.Parse(read);
                        photoarraylength++;
                    }
                }
                synfile.Close();
                photoSum = new int[photoarraylength + 1];
                photoRatio = new double[photoarraylength + 1];
                for (int i = 0; i < photoarraylength; i++)
                {
                    photoSum[i] = Photoarray[i] - Photoarray[0];
                    photoRatio[i] = (Photoarray[i + 1] - Photoarray[i]) / datarate;
                }

            }
            photogap = 0;
            if (dir[dir.Length - 1] == 'b')
            {
                StreamReader synfile = new StreamReader(dir.Remove(dir.Length-1,1) + "_photo.txt", Encoding.Default);
                int photofirst = 0;
                while (!synfile.EndOfStream)
                {
                    read = synfile.ReadLine();
                    if (read != "")
                    {
                        photofirst = Int32.Parse(read);
                        break;
                    }
                }
                photogap= Photoarray[0]-photofirst;
            }
            StreamReader file = new StreamReader(dir + "butterworth.txt", Encoding.Default);
            while (!file.EndOfStream)
            {
                line = file.ReadLine();
            }
            file.Close();
            result = line.Split(' ');
            rightarray = new PointF[result.Length - 1];

            createchart(clearornot);

            Analypanel.Height = Analychart.Height + hScrollBar1.Height + 30;
            Analychart.Width = result.Length - 1;
            hScrollBar1.Width = result.Length - 1;
            hScrollBar1.Maximum = result.Length - 1 + 10;
            panel2.Visible = true;

            hScrollBar1.Value = 1;
           
        }

       #endregion

        #region ListboxForm

        ListBox listboxm = new ListBox();
        List<String> datanamelist = new List<String>(100);
        List<String> datafolderlist = new List<string>(100);            
        List<String> Namelist = new List<String>(100);
        bool Itemselect = false;
        private DialogResult showListbox(int mode)
        {
            DirectoryInfo dir = new DirectoryInfo(Datasavepath);
            ListBox listbox = new ListBox();
            Form form;
            form = new Form();
            Itemselect = false;
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonAdd = new Button();
            int click = 0;

           // label.SetBounds(9, 20, 372, 13);
            listbox.SetBounds(10, 20, 180, 150);
            buttonAdd.SetBounds(130, 170,60, 23);
            textBox.SetBounds(10, 170, 120, 23);
            label.AutoSize = true;

            listbox.Font = new Font(FontFamily.GenericSansSerif,
            12.0F, FontStyle.Bold);

            Namelist.Clear();
            listname(dir);
            listbox.DataSource = Namelist;

            listbox.Anchor = listbox.Anchor | AnchorStyles.Right;
            listbox.MouseDoubleClick += (sender, Args) =>
                   {
                       int index = listbox.IndexFromPoint(Args.Location);
                       switch (mode)
                       {
                           case 0:
                             username = Namelist[index];
                             String userfolder = Datasavepath+"\\" + username;
                             if (!Directory.Exists(userfolder))
                                  Directory.CreateDirectory(userfolder);
                             form.Close();
                             Itemselect = true;
                               break;
                       }
                   };
            listbox.BackColor = Color.FromArgb(224, 224, 224);
            //buttonAdd.Click += new EventHandler(ListnameButtonAdd_click);
            buttonAdd.Click += (sender, args) =>
                       {
                           switch (mode)
                           {
                               case 0:
                                   Directory.CreateDirectory(Datasavepath + "\\" + textBox.Text);
                               Namelist.Clear();
                               listname(dir);
                               listbox.DataSource = null;
                               listbox.DataSource = Namelist;
                                   break;
                               case 1:
                                   try
                                   {
                                       if (listclick == 0)
                                       {
                                           AnalypictureBox.Image = null;
                                           DirectoryInfo Namelistdir = new DirectoryInfo(Datasavepath );
                                           Namelist.Clear();
                                           listname(Namelistdir);
                                           if (Directory.Exists(Datasavepath + "\\" + Namelist[namelistBox.SelectedIndex]))
                                               Directory.Move(Datasavepath + "\\" + Namelist[namelistBox.SelectedIndex], Datasavepath + "\\"  + textBox.Text);
                                           Namelist.Clear();
                                           listname(Namelistdir);
                                           namelistBox.DataSource = null;
                                           namelistBox.DataSource = Namelist;
                                           form.Close();
                                       }
                                       else
                                       {
                                           AnalypictureBox.Image = null;
                                           DirectoryInfo Namelistdir = new DirectoryInfo(Datasavepath + "\\" + username);
                                           Namelist.Clear();
                                           listname(Namelistdir);
                                           if (Directory.Exists(Datasavepath + "\\" + username + "\\" + Namelist[namelistBox.SelectedIndex]))
                                               Directory.Move(Datasavepath + "\\" + username + "\\" + Namelist[namelistBox.SelectedIndex], Datasavepath + "\\" + username + "\\" + textBox.Text);
                                           Namelist.Clear();
                                           listname(Namelistdir);
                                           namelistBox.DataSource = null;
                                           namelistBox.DataSource = Namelist;
                                           form.Close();
                                       }

                                   }
                                   catch(Exception xx) {
                                       MessageBox.Show("輸入檔名錯誤\n"+xx.ToString());
                                   }
                                   break;
                           }
                          
                       };
           // buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            //buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(200, 200);
            switch (mode)
            {
                case 0:
                    form.Text = "使用者選單";
                    form.Font = new Font(FontFamily.GenericSansSerif,
                    12.0F, FontStyle.Bold);
                    buttonAdd.Text = "新增";
                    buttonAdd.Font =new Font(FontFamily.GenericSansSerif,
                    10.0F, FontStyle.Regular);
                    form.Controls.AddRange(new Control[] { listbox, textBox, buttonAdd });
                    form.StartPosition = FormStartPosition.CenterScreen;
                    break;
                case 1:
                    buttonAdd.SetBounds(50, 120,100, 40);
                    textBox.SetBounds(10, 50, 170, 23);
                    form.Text = "更改名稱";
                    form.Font = new Font(FontFamily.GenericSansSerif,
                    12.0F, FontStyle.Bold);
                    buttonAdd.Text = "確定更改";
                    buttonAdd.Font =new Font(FontFamily.GenericSansSerif,
                    10.0F, FontStyle.Regular);
                    form.Controls.AddRange(new Control[] { textBox, buttonAdd });
                    form.StartPosition= FormStartPosition.Manual;
                    form.Location = new Point(this.Location.X + this.Width - 10, this.Location.Y);
                    break;
            } 
            form.ClientSize = new Size(200, form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            //
            form.MinimizeBox = false;
            form.MaximizeBox = false;

            return form.ShowDialog();
        }
        #endregion 

        #region Resize

        private bool isminized=false;
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                isminized = true ;
            }
        }
        int oriFormHeight = 772, oriFormWidth = 1100;
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
               
            if (!isminized)
            {
                int FormHeight = this.Height,FormWidth = this.Width;

                LiveviewpictureBox.Height += FormHeight - oriFormHeight;
                LiveviewpictureBox.Width += FormWidth - oriFormWidth;

               // panel1.Location = new Point(LiveviewpictureBox.Location.X + LiveviewpictureBox.Width, panel1.Location.Y );
               /* buttonConnect.Location = new Point(buttonConnect.Location.X, LiveviewpictureBox.Location.Y + LiveviewpictureBox.Height+25);

                buttonstart.Location = new Point(buttonstart.Location.X, LiveviewpictureBox.Location.Y + LiveviewpictureBox.Height + 25);

                buttonstop.Location = new Point(buttonstop.Location.X, LiveviewpictureBox.Location.Y + LiveviewpictureBox.Height + 25);*/

 
               /* AnalypictureBox.Height += FormHeight - oriFormHeight;
                AnalypictureBox.Width += FormWidth - oriFormWidth;*/

                //panel2.Height += FormHeight - oriFormHeight;
               // panel2.Width += FormHeight - oriFormHeight;

                /*Analypanel.Height += FormHeight - oriFormHeight;
                Analypanel.Width += FormWidth - oriFormWidth;
                Analypanel.Location = new Point(AnalypictureBox.Location.X, AnalypictureBox.Location.Y + AnalypictureBox.Height + 5);*/
               // Analychart.Height += FormHeight - oriFormHeight;
                //Analychart.Width += FormWidth - oriFormWidth;

              /*  OriginSignalbutton.Location = new Point(OriginSignalbutton.Location.X, Analypanel.Location.Y + Analypanel.Height + 20);

                FilterSignalbutton.Location = new Point(FilterSignalbutton.Location.X, Analypanel.Location.Y + Analypanel.Height + 20);

                Transferbutton.Location = new Point(Transferbutton.Location.X, Analypanel.Location.Y + Analypanel.Height + 20);*/

                oriFormHeight = FormHeight;
                oriFormWidth = FormWidth;
            }
            else
            {
                isminized = false;
            }
        }
        #endregion

        #region File list
        private String listfile(FileSystemInfo folderinfo)
        {
            DirectoryInfo folder = folderinfo as DirectoryInfo;
            if (folder == null) return null;

            FileSystemInfo[] files = folder.GetFileSystemInfos();

            for (int i = 0; i < files.Length; i++)
            {
                FileInfo file = files[i] as FileInfo;
                //是文件 
                if (file != null && file.Extension == ".wan")
                {
                    return Path.GetFileNameWithoutExtension(file.Name);
                    // MessageBox.Show(file.FullName);
                 
                  
                    //對於子目錄，進行遞歸調用 
                }

            }
            return null;
        }
        private void listname(FileSystemInfo folderinfo)
        {
            DirectoryInfo folder = folderinfo as DirectoryInfo;
            if (folder == null) return;
            DirectoryInfo[] dir = folder.GetDirectories();
          

            for (int i = 0; i < dir.Length; i++)
            {
                Namelist.Add(dir[i].Name);              
            }
        }
        #endregion

        #region Image Transfer

        private void VideotoPicture()
        {
            string Opath = newrecordname + "images\\";
            syn.filepath = newrecordname;
            if (!Directory.Exists(Opath))
                Directory.CreateDirectory(Opath);
            else
            {
                Directory.Delete(Opath, true);
                Directory.CreateDirectory(Opath);
            }
            try
            {
                Process ffmpeg = new Process();// 產生執行緒
                //string video = Application.StartupPath + @"\..\v1.mp4"; // 設定輸入影片路徑與檔名
                string video = newrecordname + Usbcontrol.getFolderMp4name(newrecordname); // 設定輸入影片路徑與檔名
                string outimg = Opath + "%4d.png"; // 設定輸出路徑與檔名
                string fileargs = " -i ";
                fileargs += "\"" + video + "\"";
                fileargs += " -s 1280x720 -aspect 16:9 -r 120 -y \"" + outimg + "\"";//命令字串

                ffmpeg.StartInfo.Arguments = fileargs;
                ffmpeg.StartInfo.FileName = Application.StartupPath + @"\..\ffmpeg.exe";//要執行檔案的位置
                ffmpeg.StartInfo.UseShellExecute = false;
                ffmpeg.StartInfo.RedirectStandardOutput = false;
                ffmpeg.StartInfo.CreateNoWindow = true;
                //ffmpeg.StartInfo.CreateNoWindow = false;
                ffmpeg.Start(); // 執行 !

                ffmpeg.WaitForExit();
                ffmpeg.Close();
                ffmpeg.Dispose();
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                MessageBox.Show(error);
            }

            syn.synpreim(0, 0, 640, 720);
            //syn.synbackim(1040, 0, 240, 720);
            //Thread.Sleep(5000);
            gifstart = true;
            if (gifstart == true)
                updateUI(" ", 7);
        }
        #endregion

        Form waitform;
        public void ShowMyPictureBox()
        {
            waitform = new Form();
            PictureBox pic = new PictureBox();

            pic.SetBounds(0, 0, 480, 320);

            pic.Image = Image.FromFile(Application.StartupPath + @"\..\..\resource\picwait.gif");

            waitform.ClientSize = new Size(480, 320);
            waitform.Controls.AddRange(new Control[] { pic });
            //waitform.FormBorderStyle = FormBorderStyle.FixedSingle;
            waitform.FormBorderStyle = FormBorderStyle.FixedDialog;
            waitform.StartPosition = FormStartPosition.CenterScreen;
            waitform.ControlBox = false;
            DialogResult dialogResult = waitform.ShowDialog();
        }
        int tabSelectedIndex = 0;
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            tabSelectedIndex = tabControl1.SelectedIndex;
            switch (tabControl1.SelectedIndex)
            {
                case 0:
                    Liveviewrunning = false;
                    panel2.Visible = false;
                    buttonConnect.Enabled = true;
                    testTime = 0;                  
                    break;
                case 1:
                    listclick = 0;                    
                    if (microTimer!=null)
                    microTimer.Enabled = false;
                    updateUI("Bluetoothstate", 5);updateUI("Camerawififalse", 3);updateUI("Camerawififalse", 9);
                    LiveviewpictureBox.Image = Image.FromFile(Application.StartupPath + @"\..\..\resource\wait.gif");
                    connectrun = false;
                    DirectoryInfo dir = new DirectoryInfo(Datasavepath);
                    Namelist.Clear();
                    namelistBox.DataSource = null;
                    listname(dir);
                    namelistBox.DataSource = Namelist;
                    if (bluetooth1 != null)
                    {
                        bluetooth1.BTconnected = false;
                        bluetooth1.client.Close();
                    }
                    if (bluetooth2 != null)
                    {
                        bluetooth2.BTconnected = false;
                        bluetooth2.client.Close();
                    }
                    break;
            }
        }
        int listclick = 0;
        private void namelistBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = namelistBox.IndexFromPoint(e.Location);
            if (listclick == 0 && index >= 0)
            {
                username = Namelist[index];
                namelistBox.DataSource = null;
                DirectoryInfo Namelistdir = new DirectoryInfo(Datasavepath + "\\" + Namelist[index]);
                Namelist.Clear();
                listname(Namelistdir);
                namelistBox.DataSource = Namelist;
                listclick = 1;
            }
            else if (index>=0)
            {
                selectfoldername = Namelist[index];
                newrecordname = Datasavepath + "\\" + username + "\\" + Namelist[index] + "\\";
                updateUI("", 6);
            }
        }

        private void namelistBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {

                if (namelistBox.SelectedItems.Count <= 1)
                {
                    namelistBox.SelectedItems.Clear();
                    contextMenuStrip1.Items.Clear();
                    contextMenuStrip1.Items.Add("顯示測資");
                    contextMenuStrip1.Items.Add("刪除測資");
                    contextMenuStrip1.Items.Add("更改名稱");
                }
                else
                {
                    contextMenuStrip1.Items.Clear();
                    contextMenuStrip1.Items.Add("刪除測資");
                }

                if (namelistBox.IndexFromPoint(e.X, e.Y)!=-1)
                {
                    namelistBox.SelectedIndex = namelistBox.IndexFromPoint(e.X, e.Y);
                    contextMenuStrip1.Show(MousePosition);
                }
            }
            else
                contextMenuStrip1.Close();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            DirectoryInfo dir = new DirectoryInfo(Datasavepath);
            Namelist.Clear();
            listname(dir);
            namelistBox.DataSource = null;
            namelistBox.DataSource = Namelist;
            listclick = 0;
            
        }

        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            int x = e.ClickedItem.ImageIndex;
            switch (e.ClickedItem.Text)
            {
                case "顯示測資":
                    if (namelistBox.SelectedItems.Count <= 1)
                    {
                        if (listclick == 0)
                        {
                            username = Namelist[namelistBox.SelectedIndex];

                            DirectoryInfo Namelistdir = new DirectoryInfo(Datasavepath + "\\" + Namelist[namelistBox.SelectedIndex]);
                            Namelist.Clear();
                            namelistBox.DataSource = null;
                            listname(Namelistdir);
                            namelistBox.DataSource = Namelist;
                            listclick = 1;
                        }
                        else
                        {
                            selectfoldername = Namelist[namelistBox.SelectedIndex];
                            newrecordname = Datasavepath + "\\" + username + "\\" + Namelist[namelistBox.SelectedIndex] + "\\";
                            updateUI("", 6);
                        }
                    }
                    break;
                case "刪除測資":
                    if (listclick == 0)
                    {
                        for (int i = 0; i < namelistBox.SelectedItems.Count; i++)
                        {
                            if (Directory.Exists(Datasavepath + "\\" +  namelistBox.SelectedItems[i].ToString()))
                                Directory.Delete(Datasavepath + "\\" + namelistBox.SelectedItems[i].ToString(), true);
                        }                        
                        
                        DirectoryInfo Namelistdir = new DirectoryInfo(Datasavepath );
                        Namelist.Clear();
                        namelistBox.DataSource = null;
                        listname(Namelistdir);
                        namelistBox.DataSource = Namelist;
                    }
                    else
                    {
                        AnalypictureBox.Image = null;
                        for (int i = 0; i < namelistBox.SelectedItems.Count; i++)
                        {
                            if (Directory.Exists(Datasavepath + "\\" + username + "\\" + namelistBox.SelectedItems[i].ToString()))
                                Directory.Delete(Datasavepath + "\\" + username + "\\" + namelistBox.SelectedItems[i].ToString(), true); 
                        }                                                    
                          DirectoryInfo Namelistdir = new DirectoryInfo(Datasavepath+"\\"+username);
                          Namelist.Clear();
                          namelistBox.DataSource = null;
                          listname(Namelistdir);
                          namelistBox.DataSource = Namelist;
                    }
                    break;
                case "更改名稱":
                    if (namelistBox.SelectedItems.Count<=1)
                    showListbox(1);
                    /*if (Directory.Exists(Datasavepath + "\\" + username + "\\" + Namelist[namelistBox.SelectedIndex]))
                        Directory.Move(Datasavepath + "\\" + username + "\\" + Namelist[namelistBox.SelectedIndex],Datasavepath + "\\" + username + "\\" + "123");*/
                       
                    break;
            }
        }

        private void showsignal_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (showsignal_comboBox.SelectedIndex)
            {
                case 0:
                Analychart.ChartAreas.Clear();
                Analychart.Legends.Clear();
                Analychart.Series.Clear();
                Analychart.ChartAreas.Add("ChartArea1"); //圖表區域集合
                Analychart.ChartAreas["ChartArea1"].AxisX.Interval = 100;
                Analychart.ChartAreas["ChartArea1"].AxisX.Minimum = 0;
                Analychart.ChartAreas["ChartArea1"].AxisX.Maximum = result.Length - 1 + 20;
                Analychart.ChartAreas["ChartArea1"].AxisY.MajorGrid.LineColor = Color.FromArgb(150, 150, 150);
                ChartPrepare(newrecordname + selectfoldername + "b", false);
                ChartPrepare(newrecordname + selectfoldername , false);

                    break;
                case 1:
                    if (File.Exists(newrecordname + selectfoldername + "bbutterworth.txt"))
                    {
                        ChartPrepare(newrecordname + selectfoldername+"b",true);

                    }
                    break;
                case 2:
                    if (File.Exists(newrecordname + selectfoldername + "butterworth.txt"))
                    {
                        ChartPrepare(newrecordname + selectfoldername,true);

                    }
                    break;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (bluetooth1 != null)
            {
                bluetooth1.BTconnected = false;
                bluetooth1.client.Close();
            }
            if (bluetooth2 != null)
            {
                bluetooth2.BTconnected = false;
                bluetooth2.client.Close();
            }
        }
    }
}