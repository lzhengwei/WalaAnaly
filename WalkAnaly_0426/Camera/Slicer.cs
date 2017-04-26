using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Windows.Forms;

namespace WalkAnaly
{
     class Slicer 
    {
        public Byte[] jpegData;
        
        /** padding data container */
        public Byte[] paddingData;

        Stream InputStream;
        HttpWebRequest imageRequest;

         public  class Payload {
        /** jpeg data container */
        public byte[] jpegData;

        /** padding data container */
        public byte[] paddingData;

        /**
         * Constructor
         */
        public Payload(byte[] jpeg, byte[] padding) {
            this.jpegData = jpeg;
            this.paddingData = padding;
        }
    }
     

        public void open(String liveviewUrl) {
          
                imageRequest = (HttpWebRequest)WebRequest.Create("http://192.168.122.1:60152/liveview.JPG?%211234%21http%2dget%3a%2a%3aimage%2fjpeg%3a%2a%21%21%21%21%21");
                // imageRequest.AllowWriteStreamBuffering = true;
                imageRequest.Method = "GET";
                imageRequest.Accept = "image/jpeg";
                imageRequest.ProtocolVersion = HttpVersion.Version10;
                var cameraResp = (HttpWebResponse)imageRequest.GetResponse();
                InputStream = cameraResp.GetResponseStream();
            

        }
        public void close()
        {
            imageRequest.Abort();
            InputStream.Close();
        }
        public Payload nextPayload()
        {
            Payload payload = null;
            while (InputStream != null && payload == null)
            {
                int readLength = 1 + 1 + 2 + 4;
                byte[] commonHeader = readBytes(InputStream, readLength);
                if (commonHeader == null || commonHeader.Length != readLength)
                {
                    MessageBox.Show("Cannot read stream for common header.");
                }
                if (commonHeader[0] != (byte)0xFF)
                {
                    MessageBox.Show("Unexpected data format. (Start byte)");
                }
                switch (commonHeader[1])
                {
                    case (byte)0x12:
                        // This is information header for streaming.
                        // skip this packet.
                        readLength = 4 + 3 + 1 + 2 + 118 + 4 + 4 + 24;
                        commonHeader = null;
                        readBytes(InputStream, readLength);
                        imageRequest.Abort();

                        break;
                    case (byte)0x01:
                    case (byte)0x11:
                        payload = readPayload();
                        imageRequest.Abort();

                        break;
                    default:
                        break;
                }
            }

            imageRequest.Abort();
            InputStream.Close();
            return payload;

        }
        public Payload readPayload()
        {
            if (InputStream != null)
            {
                int readLength = 4 + 3 + 1 + 4 + 1 + 115;
                byte[] payloadHeader = readBytes(InputStream, readLength);
                if (payloadHeader == null || payloadHeader.Length != readLength)
                {
                    MessageBox.Show("Cannot read stream for payload header.");
                }
                if (payloadHeader[0] != (byte)0x24 || payloadHeader[1] != (byte)0x35
                    || payloadHeader[2] != (byte)0x68
                    || payloadHeader[3] != (byte)0x79)
                {
                    MessageBox.Show("Unexpected data format. (Start code)");
                }
                int jpegSize = bytesToInt(payloadHeader, 4, 3);
                int paddingSize = bytesToInt(payloadHeader, 7, 1);

                byte[] jpegData = readBytes(InputStream, jpegSize);
                byte[] paddingData = readBytes(InputStream, paddingSize);

                return new Payload(jpegData, paddingData);
            }
            return null;
        }
        private static int bytesToInt(byte[] byteData, int startIndex, int count)
        {
            int ret = 0;
            for (int i = startIndex; i < startIndex + count; i++)
            {
                ret = (ret << 8) | (byteData[i] & 0xff);
            }
            return ret;
        }
        private static byte[] readBytes(Stream input, int length) {          
            MemoryStream tmpByteArray = new MemoryStream();
            byte[] buffer = new byte[1024];
            while (true )
            {
                int trialReadlen = Math.Min((int)buffer.Length, length - (int)tmpByteArray.Length);
                int readlen = input.Read(buffer, 0, trialReadlen);

                if (readlen < 0)
                {
                    break;
                }
                //StreamWriter writer = new StreamWriter(tmpByteArray);
                BinaryWriter writer = new BinaryWriter(tmpByteArray);
                writer.Write(buffer, 0, readlen);
                if (length <= tmpByteArray.Length)
                {
                    break;
                }

            }
            byte[] ret = tmpByteArray.ToArray();
            tmpByteArray.Close();
            return ret;

        }
    }
}
