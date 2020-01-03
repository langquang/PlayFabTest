using Share.APIServer.Request.Config;
using LiteNetLib;
using LiteNetLib.Utils;
using Share.NetworkV2.TransportData.Base;
using Share.NetworkV2.TransportData.Header;
using Share.NetworkV2.Utils;

namespace Share.NetworkV2
{
    public class NetPlayer
    {
        private readonly NetPeer       _connection;
        private          NetDataWriter _writer;
        private          string        _token;

        public bool IsLogined { get; set; }
        public string PlayerId { get; private set; }

        public NetPlayer(string playerId, NetPeer peer, string token)
        {
            PlayerId = playerId;
            _connection = peer;
            Token = token;
        }

        public string Token
        {
            get => _token;
            set
            {
                _token = value;
                if (_writer == null)
                {
                    _writer = new NetDataWriter();
                }
            }
        }

        public bool IsValidConnection => _connection != null && _connection.ConnectionState == ConnectionState.Connected && !string.IsNullOrEmpty(Token);

        public void Send(INetData netData)
        {
#if DEBUG_NETWORK_V2
            Debugger.Write($"[Net] Send <{PlayerId}> {netData.Header.NetType} >> {Debugger.FindConstName<NetAPICommand>(netData.Header.NetCommand)}");
#endif
            // Secret key between client and server
            if (Token != null && netData.Header is RequestHeader header)
                header.Token = Token;

            // clear already data in writer
            _writer.Reset();
            // write data 
            netData.Serialize(_writer);
            // send
            _connection.Send(_writer, DeliveryMethod.ReliableOrdered);
            _connection.Flush(); // mask immediate send
        }
    }
}