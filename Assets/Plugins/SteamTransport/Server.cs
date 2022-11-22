using Steamworks;
using System;
using System.Collections.Generic;

namespace SteamSockets
{
    internal class Server
    {
        private readonly Action<int> OnConnected;
        private readonly Action<int> OnDisconnected;
        private readonly Action<int, ArraySegment<byte>, Channel> OnData;
        private readonly Action<int, string> OnError;

        private HSteamListenSocket hSocket;
        private readonly Callback onConnectionChanged;
        private int nextConnId;
        private readonly Dictionary<int, HSteamNetConnection> connections;
        private readonly Dictionary<HSteamNetConnection, int> connections2;
        private readonly HashSet<int> connectionsDisconnected;

        private readonly IntPtr[] messagePtrs;
        private readonly byte[] receiveBuffer;

        public Server(Action<int> onConnected, Action<int> onDisconnected, Action<int, ArraySegment<byte>, Channel> onData, Action<int, string> onError)
        {
            OnConnected = onConnected;
            OnDisconnected = onDisconnected;
            OnData = onData;
            OnError = onError;

            hSocket = HSteamListenSocket.Invalid;
            onConnectionChanged = Callback<SteamNetConnectionStatusChangedCallback_t>.Create(OnConnectionChanged);
            nextConnId = 1;
            connections = new();
            connections2 = new();
            connectionsDisconnected = new();

            messagePtrs = new IntPtr[Common.MessageStackSize];
            receiveBuffer = new byte[Common.MaxMessageSize];
        }

        public bool IsActive => hSocket != HSteamListenSocket.Invalid;

        public void Start(int timeout)
        {
            if (hSocket != HSteamListenSocket.Invalid)
            {
                Log.Warning("Steam: server is already running! Stopping existing instance");
                Stop();
            }

            var options = Common.GetNetworkingConfig(timeout);
            hSocket = Sockets.CreateListenSocketP2P(0, options.Length, options);
        }

        public void Stop()
        {
            if (hSocket != HSteamListenSocket.Invalid)
            {
                Sockets.CloseListenSocket(hSocket);
                hSocket = HSteamListenSocket.Invalid;

                connections.Clear();
                connections2.Clear();
                connectionsDisconnected.Clear();
                nextConnId = 1;
            }
        }

        private void OnConnectionChanged(SteamNetConnectionStatusChangedCallback_t info)
        {
            if (hSocket == HSteamListenSocket.Invalid)
                return;

            var state = info.m_info.m_eState;
            switch (state)
            {
                case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connecting:
                    {
                        var result = Sockets.AcceptConnection(info.m_hConn);
                        if (result != EResult.k_EResultOK)
                            Common.CloseConnection(info.m_hConn, SteamDisconnectCode.Generic);

                        break;
                    }

                case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connected:
                    {
                        int connId = nextConnId++;

                        connections.Add(connId, info.m_hConn);
                        connections2.Add(info.m_hConn, connId);

                        OnConnected(connId);
                        break;
                    }

                case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ClosedByPeer:
                case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ProblemDetectedLocally:
                    {
                        var code = Common.GetDisconnectCode(info.m_info.m_eEndReason);
                        DisconnectInternal(info.m_hConn, code);
                        break;
                    }

                case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_None:
                    break;

                default:
                    Log.Warning($"[SteamServer] Unknown event '{state}', possible resource leak!");
                    break;
            }
        }

        public void Disconnect(int connId, byte code)
        {
            if (!connections.TryGetValue(connId, out var hConn))
                return;

            DisconnectInternal(connId, hConn, code);
        }

        public string GetClientAddress(int connId)
        {
            if (!connections.TryGetValue(connId, out var hConn))
                return string.Empty;

            Sockets.GetConnectionInfo(hConn, out var info);
            return info.m_identityRemote.GetSteamID64().ToString();
        }

        public void Send(int connId, ArraySegment<byte> data, Channel channel)
        {
            if (!connections.TryGetValue(connId, out var hConn))
                return;

            var result = Common.SendInternal(hConn, data, channel);
            if (result != EResult.k_EResultOK)
            {
                Log.Warning($"Failed to send message: {result}. Disconnecting.");
                DisconnectInternal(connId, hConn, SteamDisconnectCode.Generic);
            }
        }

        public void TickIncoming()
        {
            foreach (var item in connections)
            {
                int count = Sockets.ReceiveMessagesOnConnection(item.Value, messagePtrs, messagePtrs.Length);
                for (int i = 0; i < count; ++i)
                {
                    var data = Common.ReceiveInternal(messagePtrs[i], receiveBuffer, out var channel);
                    OnData(item.Key, data, channel);
                }
            }

            foreach (var connId in connectionsDisconnected)
            {
                connections.Remove(connId, out var hConn);
                connections2.Remove(hConn);
            }
            connectionsDisconnected.Clear();
        }

        public void TickOutgoing()
        {
            foreach (var hConn in connections.Values)
            {
                Sockets.FlushMessagesOnConnection(hConn);
            }
        }

        private void DisconnectInternal(HSteamNetConnection hConn, byte code)
        {
            if (!connections2.TryGetValue(hConn, out var connId))
                return;

            DisconnectInternal(connId, hConn, code);
        }

        private void DisconnectInternal(int connId, HSteamNetConnection hConn, byte code)
        {
            OnDisconnected(connId);
            Common.CloseConnection(hConn, code);

            connectionsDisconnected.Add(connId);
        }
    }
}
