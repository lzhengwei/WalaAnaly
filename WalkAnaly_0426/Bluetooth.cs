using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using InTheHand;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Ports;
using InTheHand.Net.Sockets;
using System.IO;
using System.Linq;
using InTheHand.Net;

namespace WalkAnaly
{
    class Bluetooth
    {
        Guid mUUID = new Guid("00001101-0000-1000-8000-00805F9B34FB");
        public Stream stream;
        public Thread bluetoothClientThread;
        //BluetoothAddress Address = BluetoothAddress.Parse("20:15:09:01:89:93");
        BluetoothAddress Address;
        BluetoothDeviceInfo deviceInfo;
        public BluetoothClient client;
        string myPin = "1234";
        public Boolean BTconnected = false;
        public Bluetooth(String address)
        {
            Address = BluetoothAddress.Parse(address);
        }
        public bool pairDevice()
        {
            deviceInfo = new BluetoothDeviceInfo(Address);
            if (!deviceInfo.Authenticated)
            {
                if (!BluetoothSecurity.PairRequest(Address, myPin))
                {
                    return false;
                    
                }
            }
            return true;
        }

        public void ClientConnectThread()
        {
            
            client = new BluetoothClient();

            //while (!client.Connected)
            {
                if (BTconnected == false)
                {
                    client.BeginConnect(deviceInfo.DeviceAddress, mUUID, this.bluetoothClientConnectCallback, client);
                    BTconnected = true;
                    
                }
                //Thread.Sleep(5000);
            }
            
           
        }

        public void bluetoothClientConnectCallback(IAsyncResult result)
        {
            try
            {
                BluetoothClient client = (BluetoothClient)result.AsyncState;
                client.EndConnect(result);

                stream = client.GetStream();
                stream.ReadTimeout = 1000;
                bool b = client.Authenticate;
            }
            catch (Exception x)
            {
                BTconnected = false;
               // MessageBox.Show("Bluetooth connect error");
            }
        }
    }
}
