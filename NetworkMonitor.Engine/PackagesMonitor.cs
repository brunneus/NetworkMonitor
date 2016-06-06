using PacketDotNet;
using SharpPcap;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkMonitor.Engine
{
    public class PackagesMonitor
    {
        private static PackagesMonitor _instance;
        public static PackagesMonitor Instance
        {
            get
            {
                return _instance ?? (_instance = new PackagesMonitor());
            }
        }

        public event Action<PackageGeneratedArgs> PackageGenerated = delegate { };

        public void BeginCapture(BackgroundWorker worker, ICaptureDevice networkDevice)
        {
            networkDevice.Open(DeviceMode.Promiscuous, 1000);

            while (!worker.CancellationPending)
            {
                var rawPackage = networkDevice.GetNextPacket();
                if (rawPackage == null)
                    continue;

                var packet = Packet.ParsePacket(rawPackage.LinkLayerType, rawPackage.Data);
                var ipPacket = (IpPacket)packet.Extract(typeof(IpPacket));

                if (ipPacket == null)
                    continue;

                PackageGenerated(new PackageGeneratedArgs()
                {
                    GeneratedPackage = ipPacket.Protocol == IPProtocolType.TCP ?
                                                new TCPPackage(ipPacket) :
                                                new Package(ipPacket)
                });
            }

            networkDevice.StopCapture();
            networkDevice.Close();
        }
    }

    public class PackageGeneratedArgs : EventArgs
    {
        public Package GeneratedPackage { get; set; }
    }
}
