using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace WalkAnaly.Camera
{
    class CameraUSB
    {
        List<FileInfo> Mp4file = new List<FileInfo>(100);
        List<DateTime> Mp4fileDate = new List<DateTime>(100);
        public bool Devicefind = false;
        public bool Mutifile = false;
        public int numofMp4file = 0;
        public string devicename = "",deviceANVname="";
        public Boolean CameraUSBcheck()
        {

            foreach (DriveInfo di in DriveInfo.GetDrives())
            {
                //取得磁碟的資訊，並逐一列出
                if (di.IsReady)
                //表示有東西，若不是可能是光碟、軟碟機
                {
                    DirectoryInfo diexit = new DirectoryInfo(di.Name + "MP_ROOT");
                    if (diexit.Exists)
                    {
                        DirectoryInfo[] ANV = diexit.GetDirectories();
                        if (ANV[0].Exists)
                            deviceANVname = ANV[0].Name;
                        devicename = di.Name;
                        Mp4file.Clear();
                        DirectoryInfo info = new DirectoryInfo(devicename + "MP_ROOT\\" + deviceANVname);
                        FileSystemInfo[] files = info.GetFileSystemInfos();
                        //---------------------------------------------
                        for (int i = 0; i < files.Length; i++)
                        {
                            FileInfo file = files[i] as FileInfo;
                            //是文件 
                            if (file != null && file.Extension == ".MP4")
                            {
                                Mp4file.Add(file);
                                
                            }
                        }
                        if (Mp4file.Count > 1)
                            Mutifile = true;
                        numofMp4file = Mp4file.Count;
                        Devicefind = true;
                        return true;
                    }
                    
                }

                //印出資訊
            }
            return false;
        }
        public void Mp4fileCopy(String dirpath,Boolean removelast)
        {
            DirectoryInfo info = new DirectoryInfo(devicename + "MP_ROOT\\" + deviceANVname);


            if (Devicefind && Mp4file.Count > 0)
                File.Copy(Mp4file.Last().FullName, dirpath + Mp4file.Last().Name, true);

            if (removelast)
            {
                Mp4file.Remove(Mp4file.Last());
                if(Mp4file.Count==0)
                    info.Delete(true);
            }
            else
            {
                if (info.Exists)
                    info.Delete(true);
            }
        }

        public String getLastMp4name()
        {
            if (Devicefind && Mp4file.Count > 0)
                return Mp4file.Last().Name;
            else
                return "No Mp4 file at device";
        }
        public String getAdapterMp4name(DateTime maintime)
        {

            
            String filename="";
            long minticks=long.MaxValue;
            TimeSpan Max = TimeSpan.MaxValue;
            TimeSpan min = Max;
            foreach (FileInfo date in Mp4file)
            {
                if (date.CreationTime.Day == maintime.Day && date.CreationTime.Month == maintime.Month)
                {
                    if (Math.Abs(date.CreationTime.TimeOfDay.Ticks - maintime.TimeOfDay.Ticks) < minticks)
                    {
                        minticks = date.CreationTime.TimeOfDay.Ticks;
                        filename = date.Name;
                    }
                }
                
                /*if (Math.Abs(date.CreationTime.Ticks - maintime.Ticks) < min)
                {
                    min = date.CreationTime.Ticks - maintime.Ticks;
                    filename = date.Name;
                }*/

            }
            return filename;
        }
        public String getFolderMp4name(String dir)
        {
            DirectoryInfo info = new DirectoryInfo(dir);
            FileSystemInfo[] files = info.GetFileSystemInfos();
            //---------------------------------------------
            for (int i = 0; i < files.Length; i++)
            {
                FileInfo file = files[i] as FileInfo;
                //是文件 
                if (file != null && file.Extension == ".MP4")
                {
                    return file.Name;

                }
            }
            return null;
        }
        private DateTime StringtoDatetime(String date)
        {
           
            IFormatProvider culture = new System.Globalization.CultureInfo("zh-TW", true);
            return DateTime.Parse(date, culture, System.Globalization.DateTimeStyles.AssumeLocal);
  
        }
    }
}
