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
            var packages = tcpPackages.Where(p => p.BelongToSameConnectionOf(selectedPackage)).ToList();

            var sincronizePackage = packages.FirstOrDefault(tp => tp.Syn);
            if (sincronizePackage == null)
                return;

            var finishPackage = packages.LastOrDefault(tp => tp.Fin);
            if (finishPackage == null)
                return;

            var firstPackageIndex = packages.IndexOf(sincronizePackage);
            var lastPackageIndex = packages.IndexOf(finishPackage);

            ConnectionPackages = packages.GetRange(firstPackageIndex, (lastPackageIndex - firstPackageIndex) + 1);

            TotalSenderWindowSize = ConnectionPackages.Where(p => p.SourceAdress == selectedPackage.SourceAdress).Max(a => a.WindowSize);
            TotalRecieverWindowSize = ConnectionPackages.Where(p => p.SourceAdress == selectedPackage.DestinationAddress).Max(a => a.WindowSize);

            _sourceAdress = selectedPackage.SourceAdress;
        }

        public string TestMessage { get; set; }

        public List<TCPPackage> ConnectionPackages { get; set; }
        
        public int TotalRecieverWindowSize { get; set; }

        public int TotalSenderWindowSize { get; set; }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

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
        
        private int _currentSenderWindowSize;
        public int CurrentSenderWindowSize
        {
            get
            {
                return _currentSenderWindowSize;
            }

            set
            {
                _currentSenderWindowSize = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CurrentSenderWindowSize"));
            }
        }

        private int _currentRecieverWindowSize;
        public int CurrentRecieverWindowSize
        {
            get
            {
                return _currentRecieverWindowSize;
            }

            set
            {
                _currentRecieverWindowSize = value;
                PropertyChanged(this, new PropertyChangedEventArgs("CurrentRecieverWindowSize"));
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

        private void UpdateWindowSize()
        {
            if (_selectedPackage.SourceAdress == _sourceAdress)
            {
                this.SelectedPackageFlow = ePackageFlow.SenderToReciever;
                var index = ConnectionPackages.IndexOf(_selectedPackage);
                CurrentSenderWindowSize = _selectedPackage.WindowSize;
            }
            else
            {
                this.SelectedPackageFlow = ePackageFlow.RecieverToSender;
                CurrentRecieverWindowSize = _selectedPackage.WindowSize;
            }
        }
    }

    public enum ePackageFlow
    {
        SenderToReciever,
        RecieverToSender
    }
}
