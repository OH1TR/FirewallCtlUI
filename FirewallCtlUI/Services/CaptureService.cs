﻿using FirewallCtlUI.DTO;
using PacketDotNet;
using SharpPcap;
using SharpPcap.LibPcap;
using SharpPcap.Npcap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FirewallCtlUI.Services
{
    public class CaptureService
    {
        ICaptureDevice CurrentCaptureDevice;

        List<string> MyNetworks = new List<string>();

        List<string> LocalIps = new List<string>();

        Dictionary<string, Dictionary<string, List<Tuple<DateTime, int>>>> connections = new Dictionary<string, Dictionary<string, List<Tuple<DateTime, int>>>>();
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
                    nPcap.Open(SharpPcap.Npcap.OpenFlags.DataTransferUdp | OpenFlags.NoCaptureLocal, readTimeoutMilliseconds);
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

        internal void SetMyNetworks(List<string> myNetworks)
        {
            lock(MyNetworks)
            {
                MyNetworks.Clear();
                MyNetworks.AddRange(myNetworks);
            }
        }

        private void device_OnPacketArrival(object sender, CaptureEventArgs e)
        {
            var time = e.Packet.Timeval.Date;
            var p = (EthernetPacket)Packet.ParsePacket(e.Packet.LinkLayerType, e.Packet.Data);
            var ip = p.Extract<IPPacket>();

            if (ip != null)
                LogIPPacket(ip.SourceAddress, ip.DestinationAddress, time);
        }

        private void LogIPPacket(IPAddress source, IPAddress destination, DateTime arrival)
        {

            lock (LocalIps)
            {

            }
        }

        public static bool IsInSubnet(IPAddress address, string subnetMask)
        {
            var slashIdx = subnetMask.IndexOf("/");
            if (slashIdx == -1)
            { // We only handle netmasks in format "IP/PrefixLength".
                throw new NotSupportedException("Only SubNetMasks with a given prefix length are supported.");
            }

            // First parse the address of the netmask before the prefix length.
            var maskAddress = IPAddress.Parse(subnetMask.Substring(0, slashIdx));

            if (maskAddress.AddressFamily != address.AddressFamily)
            { // We got something like an IPV4-Address for an IPv6-Mask. This is not valid.
                return false;
            }

            // Now find out how long the prefix is.
            int maskLength = int.Parse(subnetMask.Substring(slashIdx + 1));

            if (maskAddress.AddressFamily == AddressFamily.InterNetwork)
            {
                // Convert the mask address to an unsigned integer.
                var maskAddressBits = BitConverter.ToUInt32(maskAddress.GetAddressBytes().Reverse().ToArray(), 0);

                // And convert the IpAddress to an unsigned integer.
                var ipAddressBits = BitConverter.ToUInt32(address.GetAddressBytes().Reverse().ToArray(), 0);

                // Get the mask/network address as unsigned integer.
                uint mask = uint.MaxValue << (32 - maskLength);

                // https://stackoverflow.com/a/1499284/3085985
                // Bitwise AND mask and MaskAddress, this should be the same as mask and IpAddress
                // as the end of the mask is 0000 which leads to both addresses to end with 0000
                // and to start with the prefix.
                return (maskAddressBits & mask) == (ipAddressBits & mask);
            }

            if (maskAddress.AddressFamily == AddressFamily.InterNetworkV6)
            {
                // Convert the mask address to a BitArray.
                var maskAddressBits = new BitArray(maskAddress.GetAddressBytes());

                // And convert the IpAddress to a BitArray.
                var ipAddressBits = new BitArray(address.GetAddressBytes());

                if (maskAddressBits.Length != ipAddressBits.Length)
                {
                    throw new ArgumentException("Length of IP Address and Subnet Mask do not match.");
                }

                // Compare the prefix bits.
                for (int maskIndex = 0; maskIndex < maskLength; maskIndex++)
                {
                    if (ipAddressBits[maskIndex] != maskAddressBits[maskIndex])
                    {
                        return false;
                    }
                }

                return true;
            }

            throw new NotSupportedException("Only InterNetworkV6 or InterNetwork address families are supported.");
        }
    }
}
