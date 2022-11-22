using Mirror;
using Mirror.Authenticators;
using System;
using TheGame.UI;
using TheGame.UI.Classic;
using UnityEngine;

namespace TheGame.Networking
{
    public class SimpleClient : MonoBehaviour
    {
        [SerializeField] private ConnectingWindow connectingWindow;
        [SerializeField] private TimeoutAuthenticator timeoutAuthenticator;
        [SerializeField] private MenuMessage menuMessage;
        [SerializeField] private NetMgr netMgr;
        public NetMgr NetMgr => netMgr;

        private void OnValidate()
        {
            Debug.Assert(connectingWindow != null, "[SimpleClient] ConnectingWindow is null");
            Debug.Assert(timeoutAuthenticator != null, "[SimpleClient] TimeoutAuthenticator is null");
            Debug.Assert(menuMessage != null, "[SimpleClient] MenuMessage is null");
            Debug.Assert(netMgr != null, "[SimpleClient] NetMgr is null");
        }

        //TODO: dynamically get default scheme instead of const
        public void Connect(string address, string scheme = "kcp")
        {
            var uri = new Uri($"{scheme}://{address}");
            Connect(uri);
        }

        public void Connect(Uri uri)
        {
            connectingWindow.Open(netMgr.GetTimeout());
            netMgr.StopAllModes();
            netMgr.StartClient(uri);
        }

        public void OnStartAuth()
        {
            //NOTE: invoking close - future compat
            connectingWindow.Close();
            connectingWindow.Open(timeoutAuthenticator.timeout);
        }

        public void OnFinishAuth()
        {
            connectingWindow.Close();
        }

        public void OnDisconnected(byte code)
        {
            connectingWindow.Close();

            ShowReason(code);
        }

        private void ShowReason(byte code)
        {
            var reason = GetReason(code);
            if (reason != null)
                menuMessage.Show("Disconnected", reason);
        }

        private string GetReason(byte code)
        {
            switch (code)
            {
                case DisconnectCode.Timeout:
                    return "Connection timed out";

                case DisconnectCode.HostUnreachable:
                    return "Remote host unreachable";

                case DisconnectCode.ServerFull:
                    return "Server is full";

                case DisconnectCode.Shutdown:
                    return "Server shut down";

                case DisconnectCode.AuthRejected:
                    return "Authentication failed";

                case DisconnectCode.AuthTimeout:
                    return "Authentication timed out";

                case DisconnectCode.ClientDisconnect:
                    return null;

                case DisconnectCode.InternalError:
                    return "Internal error";

                case DisconnectCode.ServiceNotAvailable:
                    return "Service not available";

                case GDisconnectCode.LoadTimeout:
                    return "Client was loading too long";

                case GDisconnectCode.NotInLobby:
                    return "Match has been started";

                case GDisconnectCode.LobbyNotAvailable:
                    return "Lobby is no longer available";

                case GDisconnectCode.NoDevices:
                    return "No network devices available";

                default:
                    return "You have been disconnected by server";
            }
        }
    }
}
