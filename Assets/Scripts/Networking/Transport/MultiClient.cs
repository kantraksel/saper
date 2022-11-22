using Mirror;
using System;

namespace TheGame.Networking.CustomTransport
{
    internal class MultiClient
    {
        private MultiTransport parent;
        private Transport clientTransport;

        public void OnEnable(MultiTransport transport)
        {
            parent = transport;
        }

        public void OnDisable() { }

        public void EarlyUpdate()
        {
            if (clientTransport != null)
                clientTransport.ClientEarlyUpdate();
        }

        public void LateUpdate()
        {
            if (clientTransport != null)
                clientTransport.ClientLateUpdate();
        }

        public void Connect(string address)
        {
            throw new NotSupportedException("Use Connect(Uri) instead");
        }

        public void Connect(Uri uri)
        {
            foreach (var transport in parent.transports)
            {
                if (!transport.enabled) continue;

                if (transport.Scheme == uri.Scheme)
                {
                    clientTransport = transport;
                    parent.ClientSetCallbacks(transport);
                    transport.ClientConnect(uri);
                    return;
                }
            }

            parent.OnClientError(new Exception($"[MultiClient] No transport found for '{uri.Scheme}'"));
            parent.OnClientDisconnected(GDisconnectCode.NoDevices);
        }

        public void Disconnect(byte code)
        {
            clientTransport?.ClientDisconnect(code);
        }

        public bool IsConnected()
        {
            return clientTransport != null && clientTransport.ClientConnected();
        }

        public void Send(ArraySegment<byte> segment, int channelId)
        {
            clientTransport.ClientSend(segment, channelId);
        }
    }
}
