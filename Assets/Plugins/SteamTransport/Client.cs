using Steamworks;
using System;

namespace SteamSockets
{
    internal class Client
    {
        private readonly Action OnConnected;
        private readonly Action<byte> OnDisconnected;
        private readonly Action<ArraySegment<byte>, Channel> OnData;
        private readonly Action<string> OnError;

        private HSteamNetConnection hSocket;
        private readonly Callback onConnectionChanged;

        private readonly IntPtr[] messagePtrs;
        private readonly byte[] receiveBuffer;

        public Client(Action onConnected, Action<byte> onDisconnected, Action<ArraySegment<byte>, Channel> onData, Action<string> onError)
        {
            OnConnected = onConnected;
            OnDisconnected = onDisconnected;
            OnData = onData;
            OnError = onError;

            hSocket = HSteamNetConnection.Invalid;
            onConnectionChanged = Callback<SteamNetConnectionStatusChangedCallback_t>.Create(OnConnectionChanged);

            messagePtrs = new IntPtr[Common.MessageStackSize];
            receiveBuffer = new byte[Common.MaxMessageSize];
        }

        public bool IsConnected { get; private set; }

        public void Connect(string address, int timeout)
        {
            if (hSocket != HSteamNetConnection.Invalid)
                throw new InvalidOperationException("Steam: client is already running!");

            var identity = new SteamNetworkingIdentity();
            identity.SetSteamID64(ulong.Parse(address));

            var options = Common.GetNetworkingConfig(timeout);
            hSocket = SteamNetworkingSockets.ConnectP2P(ref identity, 0, options.Length, options);
        }

        public void Disconnect(byte code) => DisconnectInternal(code);

        private void OnConnectionChanged(SteamNetConnectionStatusChangedCallback_t info)
        {
            if (hSocket == HSteamNetConnection.Invalid)
                return;

            var state = info.m_info.m_eState;
            switch (state)
            {
                case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connecting:
                    break;

                case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connected:
                    {
                        IsConnected = true;
                        OnConnected();
                        break;
                    }

                case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ClosedByPeer:
                case ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ProblemDetectedLocally:
                    {
                        var code = Common.GetDisconnectCode(info.m_info.m_eEndReason);
                        DisconnectInternal(code);
                        break;
                    }

                default:
                    Log.Warning($"[SteamClient] Unknown event '{state}', possible resource leak!");
                    break;
            }
        }

        public void Send(ArraySegment<byte> data, Channel channel)
        {
            var result = Common.SendInternal(hSocket, data, channel);
            if (result != EResult.k_EResultOK)
            {
                Log.Warning($"Failed to send message: {result}. Disconnecting.");
                DisconnectInternal(SteamDisconnectCode.Generic);
            }
        }

        public void TickIncoming()
        {
            if (hSocket == HSteamNetConnection.Invalid)
                return;

            int count = Sockets.ReceiveMessagesOnConnection(hSocket, messagePtrs, messagePtrs.Length);
            for (int i = 0; i < count; ++i)
            {
                var data = Common.ReceiveInternal(messagePtrs[i], receiveBuffer, out var channel);
                OnData(data, channel);
            }
        }

        public void TickOutgoing()
        {
            if (hSocket == HSteamNetConnection.Invalid)
                return;

            Sockets.FlushMessagesOnConnection(hSocket);
        }

        private void DisconnectInternal(byte code)
        {
            if (hSocket == HSteamNetConnection.Invalid)
                return;

            OnDisconnected(code);
            Common.CloseConnection(hSocket, code);

            hSocket = HSteamNetConnection.Invalid;
            IsConnected = false;
        }
    }
}
