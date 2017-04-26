using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Forms;
using Newtonsoft.Json;


namespace WalkAnaly
{
    class CameraControl
    {
        private string cameraURL;
        public bool recModeActive, issendMessage = false, issendMessage2=false;
        public float time_camera = 0, time_camera2 = 0, time_camera3 = 0;
        public void ControlCamera(string cameraResp)
        {
            cameraURL = string.Format("{0}/{1}", "http://192.168.122.1:10000/sony", "/camera");
        }

        #region Camera Details
        private string CameraRequest(string _cameraUrl, string _cameraRequest)
        {
            //Uri urlURI = new Uri(cameraURL);
            issendMessage = false;
            issendMessage2 = false;
            try
            {

                HttpWebRequest cameraReq = (HttpWebRequest)WebRequest.Create(cameraURL);
                cameraReq.Method = "POST";
                cameraReq.AllowWriteStreamBuffering = false;
                cameraReq.ContentType = "application/json; charset=utf-8";
                cameraReq.Accept = "Accept-application/json";
                cameraReq.ContentLength = _cameraRequest.Length;
                using (var cameraWrite = new StreamWriter(cameraReq.GetRequestStream()))
                {

                    cameraWrite.Write(_cameraRequest);
                    time_camera = float.Parse(DateTime.Now.ToString("ss.fff"));
                    
                }

                var cameraResp = (HttpWebResponse)cameraReq.GetResponse();

                time_camera3 = float.Parse(DateTime.Now.ToString("ss.fff"));
                Stream cameraStream = cameraResp.GetResponseStream();

                time_camera2 = float.Parse(DateTime.Now.ToString("ss.fff"));
                StreamReader cameraRead = new StreamReader(cameraStream);
                string readCamera = cameraRead.ReadToEnd();
               
                issendMessage = true;
                return readCamera;
            }
            catch
            {
                return "Post_error";
            }
        }

        public string GetCameraURL(string cameraResp)
        {
            string[] cameraXML = cameraResp.Split('\n');
            string cameraURL = "";
            foreach (string cameraString in cameraXML)
            {
                string getCameraURL = "";
                if (cameraString.Contains("<av:X_ScalarWebAPI_ActionList_URL>"))
                {

                    int ipindex = cameraString.IndexOf("av:X_ScalarWebAPI_ActionList_URL");
                    cameraURL = cameraString.Substring(ipindex + 33, 31);
                   // getCameraURL = cameraString.Substring(cameraString.IndexOf('>') + 1);
                    //cameraURL = getCameraURL.Substring(0, getCameraURL.IndexOf('<'));

                }
            }
            return cameraURL;
        }

        public string GetActionType(string cameraResp)
        {
            string[] cameraXML = cameraResp.Split('\n');
            string actionType = "";
            foreach (string cameraString in cameraXML)
            {
                string getType = "";
                if (cameraString.Contains("<av:X_ScalarWebAPI_ServiceType>"))
                {
                    getType = cameraString.Substring(cameraString.IndexOf('>') + 1);
                    actionType = getType.Substring(0, getType.IndexOf('<'));
                    if (actionType == "camera")
                    {
                        break;
                    }
                }
            }
            return "camera";
        }
        #endregion

        #region Record Mode
        public string StartMovieRec()
        {
            string startRecMode = JsonConvert.SerializeObject(new CameraSetup
            {
                //method = "startRecMode",
                method = "startMovieRec",
                //method="startLiveview",
                @params = new List<string> { },
                id = 1,
                version = "1.0"
            });

            recModeActive = true;

            return CameraRequest(cameraURL, startRecMode);
        }
        public string StopMovieRec()
        {
            string stopRecMode = JsonConvert.SerializeObject(new CameraSetup
            {
                //method = "startRecMode",
                method = "stopMovieRec",
                //method = "stopLiveview",
                @params = new List<string> { },
                id = 1,
                version = "1.0"
            });

            recModeActive = false;

            return CameraRequest(cameraURL, stopRecMode);
        }
        public bool IsRecModeActive()
        {
            return recModeActive;
        }

        public string StopRecMode()
        {
            string stopRecMode = JsonConvert.SerializeObject(new CameraSetup
            {
               // method = "stopRecMode",
                //method = "stopMovieRec",
                method = "stopLiveview",
                @params = new List<string> { },
                id = 1,
                version = "1.0"
            });

            recModeActive = false;

            return CameraRequest(cameraURL, stopRecMode);
        }
        #endregion

        #region Get Event
        public string GetEvent()
        {
            string _getEvent = JsonConvert.SerializeObject(new EventNotification
            {
                method = "getEvent",
                @params = new List<bool> { true },
                id = 1,
                version = "1.0"
            });

            recModeActive = true;

            return CameraRequest(cameraURL, _getEvent);
        }
        #endregion

    }
    class CameraSetup
    {
        public string method { get; set; }
        public List<string> @params { get; set; }
        public int id { get; set; }
        public string version { get; set; }
    }

    class EventNotification
    {
        public string method { get; set; }
        public List<bool> @params { get; set; }
        public int id { get; set; }
        public string version { get; set; }
    }

    class MovieRecording
    {
        public string method { get; set; }
        public List<string> @params { get; set; }
        public int id { get; set; }
        public string version { get; set; }
    }
}
