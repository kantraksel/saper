using Mirror;
using UnityEngine;

namespace TheGame.Networking.Server.States
{
    internal class StopServer : MonoBehaviour, IBaseManager
    {
        [SerializeField] private SessionStorage sessionStorage;

        private void OnValidate()
        {
            Debug.Assert(sessionStorage != null, "[StopServerManager] SessionStorage is null");
        }

        public void Setup(ServerCoordinator parent)
        {

        }

        public void Enable()
        {

        }

        public void Disable()
        {
            sessionStorage.ClearData();
        }

        public bool OnPlayerAuth(NetworkConnectionToClient conn)
        {
            conn.Disconnect(DisconnectCode.Shutdown);
            return false;
        }

        public void OnPlayerConnect(NetworkConnectionToClient conn)
        {
            conn.Disconnect(DisconnectCode.Shutdown);
        }

        public void OnPlayerDisconnect(NetworkConnectionToClient conn)
        {
            try
            {
                var data = conn.authenticationData as SessionData;
                data.AllowDestroy = true;
            }
            catch { }
        }
    }
}
