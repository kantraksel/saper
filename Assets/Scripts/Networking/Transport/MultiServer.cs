using Mirror;
using System;
using System.Collections.Generic;

namespace TheGame.Networking.CustomTransport
{
    internal class MultiServer
    {
        private MultiTransport parent;
        private List<Transport> transports;
        private readonly Dictionary<int, Connection> connections = new();
        private readonly Dictionary<Connection, int> connections2 = new();
        private int connectionIdPool;

        public void OnEnable(MultiTransport transport)
        {
            parent = transport;
            transports = transport.transports;

            connectionIdPool = 1;

            #if UNITY_SERVER
            transports.ForEach(transport => transport.enabled = true);
            #endif
        }

        public void OnDisable()
        {
            #if UNITY_SERVER
            transports.ForEach(transport => transport.enabled = false);
            #endif
        }

        public void EarlyUpdate()
        {
            transports.ForEach(transport => transport.ServerEarlyUpdate());
        }

        public void LateUpdate()
        {
            transports.ForEach(transport => transport.ServerLateUpdate());
        }

        public void Start()
        {
            AddCallbacks();
            foreach (var transport in transports)
            {
                if (!transport.enabled) continue;
                transport.ServerStart();
            }
        }

        public void Stop()
        {
            foreach (var transport in transports)
            {
                if (!transport.enabled) continue;
                transport.ServerStop();
            }
        }

        public bool IsActive()
        {
            foreach (var transport in transports)
            {
                if (!transport.enabled) continue;
                if (!transport.ServerActive())
                    return false;
            }
            return true;
        }

        public void Disconnect(int connectionId, byte code)
        {
            var conn = ToTransportConn(connectionId);
            transports[conn.transportId].ServerDisconnect(conn.connectionId, code);
        }

        public string GetClientAddress(int connectionId)
        {
            var conn = ToTransportConn(connectionId);
            return transports[conn.transportId].ServerGetClientAddress(conn.connectionId);
        }

        public void Send(int connectionId, ArraySegment<byte> segment, int channelId)
        {
            var conn = ToTransportConn(connectionId);
            transports[conn.transportId].ServerSend(conn.connectionId, segment, channelId);
        }

        public Uri GetUri()
        {
            foreach (var transport in transports)
            {
                if (!transport.enabled) continue;
                return transport.ServerUri();
            }
            return null;
        }

        private void AddCallbacks()
        {
            for (int i = 0; i < transports.Count; ++i)
            {
                //NOTE: lambda uses local vars, so 'i' must be copied
                int transportId = i;
                var transport = transports[i];

                parent.ServerSetCallbacks(transport);

                var onServerConnected = transport.OnServerConnected;
                transport.OnServerConnected = baseId =>
                {
                    var conn = new Connection { transportId = transportId, connectionId = baseId };
                    var connId = connectionIdPool++;

                    connections.Add(connId, conn);
                    connections2.Add(conn, connId);

                    onServerConnected(connId);
                };

                var onServerDisconnected = transport.OnServerDisconnected;
                transport.OnServerDisconnected = baseId =>
                {
                    var conn = new Connection { transportId = transportId, connectionId = baseId };
                    var connId = connections2[conn];

                    onServerDisconnected(connId);

                    connections2.Remove(conn);
                    connections.Remove(connId);
                };

                var onServerDataReceived = transport.OnServerDataReceived;
                transport.OnServerDataReceived = (baseId, data, channel) =>
                {
                    onServerDataReceived(ToConnectionId(transportId, baseId), data, channel);
                };

                var onServerError = transport.OnServerError;
                transport.OnServerError = (baseId, error) =>
                {
                    onServerError(ToConnectionId(transportId, baseId), error);
                };
            }
        }

        private int ToConnectionId(int transportId, int baseId)
        {
            var conn = new Connection { transportId = transportId, connectionId = baseId };
            return connections2[conn];
        }

        private Connection ToTransportConn(int connId)
        {
            return connections[connId];
        }

        struct Connection
        {
            public int transportId;
            public int connectionId;
        }
    }
}
