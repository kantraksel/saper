using kcp2k;
using Mirror;
using SteamSockets;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TheGame.Networking.CustomTransport
{
    internal class MultiTransport : Transport
    {
        [SerializeField] private SteamTransport steamTransport;
        [SerializeField] private KcpTransport kcpTransport;

        [Tooltip("Connection timeout in milliseconds. Note that it overwrites timeout in owned transports.")]
        public int Timeout;

        public readonly List<Transport> transports = new();
        private readonly MultiServer server = new();
        private readonly MultiClient client = new();

        private void OnValidate()
        {
            Debug.Assert(steamTransport != null, "[MultiTransport] SteamTransport is null");
            Debug.Assert(kcpTransport != null, "[MultiTransport] KcpTransport is null");

            if (Timeout == 0)
            {
                Debug.Log("Timeout is 0, setting value to 10000");
                Timeout = 10000;
            }
        }

        private void Start()
        {
            //NOTE: order IS important - integrations should be before direct transport
            transports.Add(steamTransport);
            transports.Add(kcpTransport);

            steamTransport.Timeout = Timeout;
            kcpTransport.Timeout = Timeout;

            Scheme = kcpTransport.Scheme;
        }

        private void OnEnable()
        {
            server.OnEnable(this);
            client.OnEnable(this);
        }

        private void OnDisable()
        {
            server.OnDisable();
            client.OnDisable();
        }

        public override void Shutdown()
        {
            transports.ForEach(transport => transport.Shutdown());
        }

        public override void ClientEarlyUpdate() => client.EarlyUpdate();
        public override void ClientLateUpdate() => client.LateUpdate();
        public override void ServerEarlyUpdate() => server.EarlyUpdate();
        public override void ServerLateUpdate() => server.LateUpdate();

        public override bool Available()
        {
            //NOTE: available if any of the transports is available
            foreach (var transport in transports)
                if (transport.enabled && transport.Available())
                    return true;

            return false;
        }

        public override int GetMaxPacketSize(int channelId = 0)
        {
            //NOTE: smallest size = packet will be sent via all transports
            int mininumAllowedSize = int.MaxValue;
            foreach (var transport in transports)
            {
                int size = transport.GetMaxPacketSize(channelId);
                mininumAllowedSize = Mathf.Min(size, mininumAllowedSize);
            }
            return mininumAllowedSize;
        }

        #region Client
        public override void ClientConnect(string address) => client.Connect(address);
        public override void ClientConnect(Uri uri) => client.Connect(uri);
        public override bool ClientConnected() => client.IsConnected();
        public override void ClientDisconnect(byte code) => client.Disconnect(code);
        public override void ClientSend(ArraySegment<byte> segment, int channelId) => client.Send(segment, channelId);

        public void ClientSetCallbacks(Transport transport)
        {
            transport.OnClientConnected = OnClientConnected;
            transport.OnClientDataReceived = OnClientDataReceived;
            transport.OnClientError = OnClientError;
            transport.OnClientDisconnected = OnClientDisconnected;
        }
        #endregion

        #region Server
        public override void ServerStart() => server.Start();
        public override void ServerStop() => server.Stop();
        public override bool ServerActive() => server.IsActive();
        public override void ServerDisconnect(int connectionId, byte code) => server.Disconnect(connectionId, code);
        public override string ServerGetClientAddress(int connectionId) => server.GetClientAddress(connectionId);
        public override void ServerSend(int connectionId, ArraySegment<byte> segment, int channelId) => server.Send(connectionId, segment, channelId);
        public override Uri ServerUri() => server.GetUri();

        public void ServerSetCallbacks(Transport transport)
        {
            transport.OnServerConnected = OnServerConnected;
            transport.OnServerDisconnected = OnServerDisconnected;
            transport.OnServerDataReceived = OnServerDataReceived;
            transport.OnServerError = OnServerError;
        }
        #endregion

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine(nameof(MultiTransport));
            foreach (var transport in transports)
            {
                if (!transport.enabled) continue;
                builder.AppendLine(transport.ToString());
            }
            return builder.ToString().Trim();
        }
    }
}
