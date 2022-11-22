using Steamworks;
using System;

namespace SteamSockets
{
    internal static class Sockets
    {
        public static HSteamListenSocket CreateListenSocketP2P(int nLocalVirtualPort, int nOptions, SteamNetworkingConfigValue_t[] pOptions)
        {
#if UNITY_SERVER
            return SteamGameServerNetworkingSockets.CreateListenSocketP2P(0, nOptions, pOptions);
#else
            return SteamNetworkingSockets.CreateListenSocketP2P(0, nOptions, pOptions);
#endif
        }

        public static bool CloseListenSocket(HSteamListenSocket hSocket)
        {
#if UNITY_SERVER
            return SteamGameServerNetworkingSockets.CloseListenSocket(hSocket);
#else
            return SteamNetworkingSockets.CloseListenSocket(hSocket);
#endif
        }

        public static EResult AcceptConnection(HSteamNetConnection hConn)
        {
#if UNITY_SERVER
            return SteamGameServerNetworkingSockets.AcceptConnection(hConn);
#else
            return SteamNetworkingSockets.AcceptConnection(hConn);
#endif
        }

        public static bool CloseConnection(HSteamNetConnection hPeer, int nReason, string pszDebug, bool bEnableLinger)
        {
#if UNITY_SERVER
            return SteamGameServerNetworkingSockets.CloseConnection(hPeer, nReason, pszDebug, bEnableLinger);
#else
            return SteamNetworkingSockets.CloseConnection(hPeer, nReason, pszDebug, bEnableLinger);
#endif
        }

        public static bool GetConnectionInfo(HSteamNetConnection hConn, out SteamNetConnectionInfo_t pInfo)
        {
#if UNITY_SERVER
            return SteamGameServerNetworkingSockets.GetConnectionInfo(hConn, out pInfo);
#else
            return SteamNetworkingSockets.GetConnectionInfo(hConn, out pInfo);
#endif
        }

        public static EResult SendMessageToConnection(HSteamNetConnection hConn, IntPtr pData, uint cbData, int nSendFlags, out long pOutMessageNumber)
        {
#if UNITY_SERVER
            return SteamGameServerNetworkingSockets.SendMessageToConnection(hConn, pData, cbData, nSendFlags, out pOutMessageNumber);
#else
            return SteamNetworkingSockets.SendMessageToConnection(hConn, pData, cbData, nSendFlags, out pOutMessageNumber);
#endif
        }

        public static EResult FlushMessagesOnConnection(HSteamNetConnection hConn)
        {
#if UNITY_SERVER
            return SteamGameServerNetworkingSockets.FlushMessagesOnConnection(hConn);
#else
            return SteamNetworkingSockets.FlushMessagesOnConnection(hConn);
#endif
        }

        public static int ReceiveMessagesOnConnection(HSteamNetConnection hConn, IntPtr[] ppOutMessages, int nMaxMessages)
        {
#if UNITY_SERVER
            return SteamGameServerNetworkingSockets.ReceiveMessagesOnConnection(hConn, ppOutMessages, nMaxMessages);
#else
            return SteamNetworkingSockets.ReceiveMessagesOnConnection(hConn, ppOutMessages, nMaxMessages);
#endif
        }
    }
}
