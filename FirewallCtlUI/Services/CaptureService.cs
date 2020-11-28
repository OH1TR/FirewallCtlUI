using FirewallCtlUI.DTO;
using SharpPcap;
using SharpPcap.LibPcap;
using SharpPcap.Npcap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FirewallCtlUI.Services
{
    public class CaptureService
    {
        ICaptureDevice CurrentCaptureDevice;
        public List<CaptureDevice> GetDevices()
        {
            return CaptureDeviceList.Instance.Select(i => new CaptureDevice() { Name = i.Name, Description = i.Description }).ToList();
        }

        public void SetDevice(string name)
        {
            if (CurrentCaptureDevice == null || CurrentCaptureDevice.Name != name)
            {
                if (CurrentCaptureDevice != null)
                {
                    CurrentCaptureDevice.OnPacketArrival -= device_OnPacketArrival;
                    CurrentCaptureDevice.StopCapture();
                }

                CurrentCaptureDevice = CaptureDeviceList.Instance.First(i => i.Name == name);
                CurrentCaptureDevice.OnPacketArrival += device_OnPacketArrival;

                int readTimeoutMilliseconds = 1000;
                if (CurrentCaptureDevice is NpcapDevice)
                {
                    var nPcap = CurrentCaptureDevice as NpcapDevice;
                    nPcap.Open(SharpPcap.Npcap.OpenFlags.DataTransferUdp | SharpPcap.Npcap.OpenFlags.NoCaptureLocal, readTimeoutMilliseconds);
                }
                else if (CurrentCaptureDevice is LibPcapLiveDevice)
                {
                    var livePcapDevice = CurrentCaptureDevice as LibPcapLiveDevice;
                    livePcapDevice.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);
                }
                else
                    throw new InvalidOperationException("unknown device type of " + CurrentCaptureDevice.GetType().ToString());

                CurrentCaptureDevice.StartCapture();
            }
        }

        private static void device_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            var time = e.Packet.Timeval.Date;
            var len = e.Packet.Data.Length;
            Console.WriteLine("{0}:{1}:{2},{3} Len={4}",
                time.Hour, time.Minute, time.Second, time.Millisecond, len);
            Console.WriteLine(e.Packet.ToString());
        }
    }
}
