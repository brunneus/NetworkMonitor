using NetworkMonitor.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkMonitor.UI.ViewModels
{
    public class TCPConnectionAnalysisViewModel
    {
        public TCPConnectionAnalysisViewModel(IEnumerable<TCPPackage> tcpPackages, TCPPackage selectedPackage)
        {
            var packages = tcpPackages.Where(p => p.BelongToSameConnectionOf(selectedPackage)).ToList();

            var sincronizePackage = packages.FirstOrDefault(tp => tp.Syn);
            if (sincronizePackage == null)
                return;

            var finishPackage = packages.LastOrDefault(tp => tp.Fin);
            if (finishPackage == null)
                return;

            var firstPackageIndex = packages.IndexOf(sincronizePackage);
            var lastPackageIndex = packages.IndexOf(finishPackage);

            var connectionPackages = packages.GetRange(firstPackageIndex, (lastPackageIndex - firstPackageIndex) + 1);




            //var packagesRecieved = tcpPackages.Where(p => p.SourceAdress == selectedPackage.DestinationAddress &&
            //                                              p.DestinationAddress == selectedPackage.SourceAdress &&
            //                                              p.SourcePort == selectedPackage.DestinationPort &&
            //                                              p.DestinationPort == selectedPackage.SourcePort);

            //PackagesSent = new List<TCPPackage>(packagesSent);
            //PackagesRecieved = new List<TCPPackage>(packagesRecieved);

            PackagesSent = new List<TCPPackage>(connectionPackages);

            var senderWindowSize = int.MinValue;
            var recieverWindow = int.MinValue;

            int sentBytes = 0;
            PackagesSent.ForEach(ps =>
            {
                sentBytes += ps.BytesCount;
                if (ps.WindowSize > senderWindowSize)
                    senderWindowSize = ps.WindowSize;
            });

            int recievedBytes = 0;
            PackagesRecieved.ForEach(ps =>
            {
                recievedBytes += ps.BytesCount;
                if (ps.WindowSize > recieverWindow)
                    recieverWindow = ps.WindowSize;
            });

            int senderWindowSizeEmpty = PackagesSent.Count(ps => ps.WindowSize == 0);
            int recieverWindowSizeEmpty = PackagesRecieved.Count(ps => ps.WindowSize == 0);

            TestMessage = $"Sent bytes: { sentBytes} \nRecieved bytes: { recievedBytes} \nSender windows size empty: { senderWindowSizeEmpty } \nRecieved windows size empty: { recieverWindowSizeEmpty } \nSender Window size: { senderWindowSize} \nReciever window size: {recieverWindow}  ";
        }

        public string TestMessage { get; set; }

        public List<TCPPackage> PackagesSent { get; set; }

        public List<TCPPackage> PackagesRecieved { get; set; }
    }
}
