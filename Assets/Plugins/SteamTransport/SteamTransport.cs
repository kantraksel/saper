using Mirror;
using Steamworks;
using System;
using UnityEngine;

namespace SteamSockets
{
    public class SteamTransport : Transport
    {
        private readonly Server server;
        private readonly Client client;
        private readonly UriBuilder uriBuilder;

        [Tooltip("Timeout in milliseconds")]
        public int Timeout = 10000;

        public SteamTransport()
        {
            Scheme = "steam";

            Log.Info = _ => {};

            server = new(
                (connId) => OnServerConnected(connId),
                (connId) => OnServerDisconnected(connId),
                (connId, segment, channel) => OnServerDataReceived(connId, segment, FromSteamChannel(channel)),
                null
                );

            client = new(
                () => OnClientConnected(),
                (reason) => OnClientDisconnected(reason),
                (segment, channel) => OnClientDataReceived(segment, FromSteamChannel(channel)),
                null
                );

            uriBuilder = new();
            uriBuilder.Scheme = Scheme;
        }

        private void OnValidate()
        {
            Debug.Assert(!enabled, "This Behaviour should stay disabled - the object is enabled dynamically!", this);
        }

        private void Reset()
        {
            enabled = false;
        }

        public override void Shutdown() { }

        public override void ClientEarlyUpdate()
        {
            if (enabled)
                client.TickIncoming();
        }

        public override void ClientLateUpdate() => client.TickOutgoing();

        public override void ServerEarlyUpdate()
        {
            if (enabled)
                server.TickIncoming();
        }

        public override void ServerLateUpdate() => server.TickOutgoing();

        public override bool Available()
        {
            try
            {
                InteropHelp.TestIfPlatformSupported();
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        public override int GetMaxPacketSize(int channelId) => Common.MaxMessageSize;

        #region Client
        public override void ClientConnect(Uri uri)
        {
            if (uri.Scheme != Scheme)
            {
                OnClientError(new ArgumentException($"Invalid url {uri}, use {Scheme}://SteamID instead", nameof(uri)));
                OnClientDisconnected(DisconnectCode.Generic);
                return;
            }

            ClientConnect(uri.Host);
        }

        public override void ClientConnect(string address) => client.Connect(address, Timeout);
        public override void ClientDisconnect(byte code) => client.Disconnect(code);
        public override bool ClientConnected() => client.IsConnected;
        public override void ClientSend(ArraySegment<byte> segment, int channelId) => client.Send(segment, ToSteamChannel(channelId));
        #endregion

        #region Server
        public override void ServerStart() => server.Start(Timeout);
        public override void ServerStop() => server.Stop();
        public override bool ServerActive() => server.IsActive;
        public override void ServerDisconnect(int connectionId, byte code) => server.Disconnect(connectionId, code);
        public override string ServerGetClientAddress(int connectionId) => server.GetClientAddress(connectionId);
        public override void ServerSend(int connectionId, ArraySegment<byte> segment, int channelId) => server.Send(connectionId, segment, ToSteamChannel(channelId));

        public override Uri ServerUri()
        {
            uriBuilder.Host = SteamUser.GetSteamID().m_SteamID.ToString();
            return uriBuilder.Uri;
        }
        #endregion

        private static int FromSteamChannel(Channel channel)
        {
            return channel == Channel.Reliable ? Channels.Reliable : Channels.Unreliable;
        }

        private static Channel ToSteamChannel(int channel)
        {
            return channel == Channels.Reliable ? Channel.Reliable : Channel.Unreliable;
        }

        public override string ToString() => nameof(SteamTransport);
    }
}
