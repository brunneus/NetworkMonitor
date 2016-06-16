using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PacketDotNet;

namespace NetworkMonitor.Engine
{
    public class TCPPackage : Package
    {
        private readonly bool _syn;
        public bool Syn
        {
            get
            {
                return _syn;
            }
        }

        private readonly bool _fin;
        public bool Fin
        {
            get
            {
                return _fin;
            }
        }

        private readonly bool _rst;
        public bool RST
        {
            get
            {
                return _rst;
            }
        }

        private readonly bool _ack;
        public bool ACK
        {
            get
            {
                return _ack;
            }
        }

        private readonly int _bytesCount;
        public int BytesCount
        {
            get
            {
                return _bytesCount;
            }
        }

        private readonly uint _sequenceNumber;
        public uint SequenceNumber
        {
            get
            {
                return _sequenceNumber;
            }
        }

        private readonly int _windowSize;
        public int WindowSize
        {
            get
            {
                return _windowSize;
            }
        }

        private readonly uint _acknowledgementNumber;
        public uint AcknowledgementNumber
        {
            get
            {
                return _acknowledgementNumber;
            }
        }

        private readonly int _sourcePort;
        public int SourcePort
        {
            get
            {
                return _sourcePort;
            }
        }

        private readonly int _destinationPort;
        public int DestinationPort
        {
            get
            {
                return _destinationPort;
            }
        }

        public TCPPackage(IpPacket ipPacket) : base(ipPacket)
        {
            if (ipPacket.Protocol != IPProtocolType.TCP)
                throw new ArgumentException("Cannot create a instance of TCPPackage from a ipPacket with protocol different than TCP");

            var tcpPacket = (TcpPacket)ipPacket.Extract(typeof(TcpPacket));
            _syn = tcpPacket.Syn;
            _fin = tcpPacket.Fin;
            _rst = tcpPacket.Rst;
            _ack = tcpPacket.Ack;
            _bytesCount = tcpPacket.Bytes.Count();
            _sequenceNumber = tcpPacket.SequenceNumber;
            _windowSize = tcpPacket.WindowSize;
            _acknowledgementNumber = tcpPacket.AcknowledgmentNumber;
            _sourcePort = tcpPacket.SourcePort;
            _destinationPort = tcpPacket.DestinationPort;
        }

        public bool BelongToSameConnectionOf(TCPPackage other)
        {
            if (other == null)
                return false;

            var sourceAdressIsEqual = other.SourceAdress == SourceAdress;
            var destinationAdressIsEqual = other.DestinationAddress == DestinationAddress;
            var sourcePortIsEqual = other.SourcePort == SourcePort;
            var destinationPortIsEqual = other.DestinationPort == DestinationPort;

            var sourceAddressIsEqualToOtherDestinationAddress = SourceAdress == other.DestinationAddress;
            var destinationAddressIsEqualToOtherSourceAddress = DestinationAddress == other.SourceAdress;
            var sourcePortIsEqualToOtherDestinationPort = SourcePort == other.DestinationPort;
            var destinationPortIsEqualToOtherSourcePort = DestinationPort == other.SourcePort;

            return (sourceAdressIsEqual && sourcePortIsEqual && destinationPortIsEqual && destinationAdressIsEqual) ||
                   (sourceAddressIsEqualToOtherDestinationAddress &&
                    destinationAddressIsEqualToOtherSourceAddress &&
                    sourcePortIsEqualToOtherDestinationPort &&
                    destinationPortIsEqualToOtherSourcePort);
        }
    }
}
