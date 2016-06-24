using PacketDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkMonitor.Engine
{
    public class Package : INotifyPropertyChanged
    {
        private readonly string _sourceAdress;
        public string SourceAdress
        {
            get
            {
                return _sourceAdress;
            }
        }

        private readonly string _destinationAddress;
        public string DestinationAddress
        {
            get
            {
                return _destinationAddress;
            }
        }

        private readonly string _nextHeader;
        public string NextHeader
        {
            get
            {
                return _nextHeader;
            }
        }

        private readonly IPProtocolType _protocol;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public IPProtocolType Protocol
        {
            get
            {
                return _protocol;
            }
        }

        public string CreatedTime { get; private set; }

        public Package(IpPacket ipPacket)
        {
            this.CreatedTime = DateTime.Now.ToString("HH:mm:ss");
            _sourceAdress = ipPacket.SourceAddress.ToString();
            _destinationAddress = ipPacket.DestinationAddress.ToString();
            _nextHeader = ipPacket.NextHeader.ToString();
            _protocol = ipPacket.Protocol;
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
