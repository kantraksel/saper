// where-allocation version of KcpClientConnectionNonAlloc.
// may not be wanted on all platforms, so it's an extra optional class.
using System;

namespace kcp2k
{
    public class KcpClientNonAlloc : KcpClient
    {
        public KcpClientNonAlloc(Action OnConnected,
                                 Action<ArraySegment<byte>, KcpChannel> OnData,
                                 Action<byte> OnDisconnected,
                                 Action<ErrorCode, string> OnError)
            : base(OnConnected, OnData, OnDisconnected, OnError)
        {
        }

        protected override KcpClientConnection CreateConnection() =>
            new KcpClientConnectionNonAlloc();
    }
}