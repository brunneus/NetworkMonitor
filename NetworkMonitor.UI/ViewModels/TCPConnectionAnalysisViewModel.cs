using NetworkMonitor.Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkMonitor.UI.ViewModels
{
    public class TCPConnectionAnalysisViewModel : INotifyPropertyChanged
    {
        private string _sourceAdress;

        public TCPConnectionAnalysisViewModel(IEnumerable<TCPPackage> tcpPackages, TCPPackage selectedPackage)
        {
            _connectionPackage = new List<TCPPackage>();
            var packages = tcpPackages.Where(p => p.BelongToSameConnectionOf(selectedPackage)).ToList();

            var sincronizePackage = packages.FirstOrDefault(tp => tp.Syn);
            if (sincronizePackage == null)
                return;

            var finishPackage = packages.LastOrDefault(tp => tp.Fin);
            if (finishPackage == null)
                return;

            var firstPackageIndex = packages.IndexOf(sincronizePackage);
            var lastPackageIndex = packages.IndexOf(finishPackage);

            _connectionPackage = packages.GetRange(firstPackageIndex, (lastPackageIndex - firstPackageIndex) + 1);
            _sourceAdress = selectedPackage.SourceAdress;

            TotalSenderWindowSize = ConnectionPackages.Where(p => p.SourceAdress == selectedPackage.SourceAdress).Max(a => a.WindowSize);
            TotalRecieverWindowSize = ConnectionPackages.Where(p => p.SourceAdress == selectedPackage.DestinationAddress).Max(a => a.WindowSize);
            FindDuplicatedPackages();
        }
        
        public int TotalRecieverWindowSize { get; set; }

        public int TotalSenderWindowSize { get; set; }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private List<TCPPackage> _connectionPackage;
        public IEnumerable<TCPPackage> ConnectionPackages
        {
            get
            {
                return _connectionPackage;
            }
        }

        private TCPPackage _selectedPackage;
        public TCPPackage SelectedPackage
        {
            get
            {
                return _selectedPackage;
            }
            set
            {
                _selectedPackage = value;
                PropertyChanged(this, new PropertyChangedEventArgs("SelectedPackage"));
                UpdateWindowSize();
            }
        }

        private int _filledSpaceSenderBuffer;
        public int FilledSpaceSenderBuffer
        {
            get
            {
                return _filledSpaceSenderBuffer;
            }
            set
            {
                _filledSpaceSenderBuffer = value;
                PropertyChanged(this, new PropertyChangedEventArgs("FilledSpaceSenderBuffer"));
            }
        }

        private int _filledSpaceRecieverBuffer;
        public int FilledSpaceRecieverBuffer
        {
            get
            {
                return _filledSpaceRecieverBuffer;
            }

            set
            {
                _filledSpaceRecieverBuffer = value;
                PropertyChanged(this, new PropertyChangedEventArgs("FilledSpaceRecieverBuffer"));
            }
        }

        private ePackageFlow _selectedPackageFlow;
        public ePackageFlow SelectedPackageFlow
        {
            get
            {
                return _selectedPackageFlow;
            }

            set
            {
                _selectedPackageFlow = value;
                PropertyChanged(this, new PropertyChangedEventArgs("SelectedPackageFlow"));
            }
        }

        public bool HasAnyDuplicatedPackage
        {
            get
            {
                return _connectionPackage.Any(p => p.IsADuplicatedPackage);
            }
        }

        private void UpdateWindowSize()
        {
            if (_selectedPackage.SourceAdress == _sourceAdress)
            {
                SelectedPackageFlow = ePackageFlow.SenderToReciever;
                FilledSpaceSenderBuffer = TotalSenderWindowSize - _selectedPackage.WindowSize;
            }
            else
            {
                SelectedPackageFlow = ePackageFlow.RecieverToSender;
                FilledSpaceRecieverBuffer = TotalRecieverWindowSize - _selectedPackage.WindowSize;
            }
        }

        private void FindDuplicatedPackages()
        {
            foreach (var package in _connectionPackage)
            {
                package.IsADuplicatedPackage = false;
                if (_connectionPackage
                    .Any(p => 
                            p != package &&
                            p.SequenceNumber == package.SequenceNumber &&
                            p.AcknowledgementNumber == package.AcknowledgementNumber))
                    package.IsADuplicatedPackage = true;
            }
        }
    }

    public enum ePackageFlow
    {
        SenderToReciever,
        RecieverToSender
    }
}
