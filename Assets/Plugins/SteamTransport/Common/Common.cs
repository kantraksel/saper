using Steamworks;
using System;
using System.Runtime.InteropServices;

namespace SteamSockets
{
    internal static class Common
    {
        public const int MaxMessageSize = Constants.k_cbMaxSteamNetworkingSocketsMessageSizeSend;
        public const int MessageStackSize = 256;

        private const int DisconnectCodeMin = (int)ESteamNetConnectionEnd.k_ESteamNetConnectionEnd_App_Min + 1;

        public static EResult SendInternal(HSteamNetConnection hConn, ArraySegment<byte> data, Channel channel)
        {
            var handle = GCHandle.Alloc(data.Array, GCHandleType.Pinned);
            var ptr = handle.AddrOfPinnedObject();
            int nChannel = channel == Channel.Reliable ? Constants.k_nSteamNetworkingSend_Reliable : Constants.k_nSteamNetworkingSend_Unreliable;

            var result = Sockets.SendMessageToConnection(hConn, ptr, (uint)data.Count, nChannel, out var _);

            handle.Free();
            return result;
        }

        public static ArraySegment<byte> ReceiveInternal(IntPtr ptr, byte[] data, out Channel channel)
        {
            var message = SteamNetworkingMessage_t.FromIntPtr(ptr);
            Marshal.Copy(message.m_pData, data, 0, message.m_cbSize);

            channel = Channel.Unreliable;
            if ((message.m_nFlags | Constants.k_nSteamNetworkingSend_Reliable) != 0)
                channel = Channel.Reliable;

            SteamNetworkingMessage_t.Release(ptr);

            return new ArraySegment<byte>(data, 0, message.m_cbSize);
        }

        public static void CloseConnection(HSteamNetConnection hConn, byte code)
        {
            int reason = DisconnectCodeMin + code;
            Sockets.CloseConnection(hConn, reason, "", true);
        }

        public static byte GetDisconnectCode(int reason)
        {
            switch ((ESteamNetConnectionEnd)reason)
            {
                case ESteamNetConnectionEnd.k_ESteamNetConnectionEnd_Remote_Timeout:
                case ESteamNetConnectionEnd.k_ESteamNetConnectionEnd_Misc_Timeout:
                    return SteamDisconnectCode.Timeout;

                case ESteamNetConnectionEnd.k_ESteamNetConnectionEnd_Local_ManyRelayConnectivity:
                case ESteamNetConnectionEnd.k_ESteamNetConnectionEnd_Local_HostedServerPrimaryRelay:
                case ESteamNetConnectionEnd.k_ESteamNetConnectionEnd_Misc_P2P_Rendezvous:
                    return SteamDisconnectCode.HostUnreachable;

                case ESteamNetConnectionEnd.k_ESteamNetConnectionEnd_Local_OfflineMode:
                case ESteamNetConnectionEnd.k_ESteamNetConnectionEnd_Misc_SteamConnectivity:
                    return SteamDisconnectCode.ServiceNotAvailable;
            }

            int code = reason - DisconnectCodeMin;
            if (code < byte.MinValue || code > byte.MaxValue)
            {
                Log.Warning($"Steam: Disconnected with unhandled code {reason}");
                return SteamDisconnectCode.Generic;
            }

            return (byte)code;
        }

        public static SteamNetworkingConfigValue_t[] GetNetworkingConfig(int timeout)
        {
            return new SteamNetworkingConfigValue_t[]
            {
                new SteamNetworkingConfigValue_t()
                {
                    m_eValue = ESteamNetworkingConfigValue.k_ESteamNetworkingConfig_TimeoutInitial,
                    m_eDataType = ESteamNetworkingConfigDataType.k_ESteamNetworkingConfig_Int32,
                    m_val = new SteamNetworkingConfigValue_t.OptionValue() { m_int32 = timeout },
                },
                new SteamNetworkingConfigValue_t()
                {
                    m_eValue = ESteamNetworkingConfigValue.k_ESteamNetworkingConfig_TimeoutConnected,
                    m_eDataType = ESteamNetworkingConfigDataType.k_ESteamNetworkingConfig_Int32,
                    m_val = new SteamNetworkingConfigValue_t.OptionValue() { m_int32 = timeout },
                },
            };
        }
    }
}
