using Mirror;
using UnityEngine;

namespace TheGame.Networking.Server.States
{
    internal class Lobby : MonoBehaviour, IBaseManager
    {
        private BaseLobby lobby;

        public void Setup(ServerCoordinator parent)
        {
            lobby = parent.GameLobby;
        }

        public void Enable()
        {
            NetworkServer.SetAllClientsNotReady();
            lobby.OnServerEnable();
        }

        public void Disable()
        {
            lobby.OnServerDisable();
        }

        public bool OnPlayerAuth(NetworkConnectionToClient conn)
        {
            return lobby.OnPlayerAuth(conn);
        }

        public void OnPlayerConnect(NetworkConnectionToClient conn)
        {
            lobby.OnPlayerConnect(conn);
        }

        public void OnPlayerDisconnect(NetworkConnectionToClient conn)
        {
            lobby.OnPlayerDisconnect(conn);
        }
    }
}
