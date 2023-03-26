using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaDevices;

namespace LLVC
{
    public class MediaController
    {
        public SortedDictionary<string, MediaDevice> Devices { get; set; } = new SortedDictionary<string, MediaDevice>();

        public void ConnectToDevice(MediaDevice device)
        {
            var key = device.DeviceId;

            if (device.IsConnected)
            {
                Console.WriteLine("Device " + device.FriendlyName + " is already connected.");
                if (!Devices.ContainsKey(key))
                    Devices.Add(key, device);
                return;
            }
            try
            {
                device.Connect();
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Connection failed.");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(e.Message);
            }
            finally
            {
                Console.WriteLine("Connected to " + device.FriendlyName + ", " + device.Manufacturer + ".");
                if (!Devices.ContainsKey(key))
                    Devices.Add(key, device);
            }
        }
        public void DisconnectFromDevice(MediaDevice device)
        {
            if (!device.IsConnected)
            {
                Console.WriteLine("Device " + device.FriendlyName + " is already disconnected.");
                if(Devices.ContainsKey(device.DeviceId))
                    Devices.Remove(device.DeviceId);
                return;
            }
            try
            {
                device.Disconnect();
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Disconnection failed.");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(e.Message);
            }
            finally
            {
                Console.WriteLine("Disconnected from " + device.FriendlyName + ", " + device.Manufacturer + ".");
                if (Devices.ContainsKey(device.DeviceId))
                    Devices.Remove(device.DeviceId);
            }
        }
    }
}
