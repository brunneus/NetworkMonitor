using PacketDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkMonitor.Engine
{
    public class Package
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
        public IPProtocolType Protocol
        {
            get
            {
                return _protocol;
            }
        }

        public Package(IpPacket ipPacket)
        {
            _sourceAdress = ipPacket.SourceAddress.ToString();
            _destinationAddress = ipPacket.DestinationAddress.ToString();
            _nextHeader = ipPacket.NextHeader.ToString();
            _protocol = ipPacket.Protocol;
        }
    }
}
