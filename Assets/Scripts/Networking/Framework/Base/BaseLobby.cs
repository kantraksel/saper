using Mirror;

namespace TheGame.Networking
{
    public abstract class BaseLobby : NetworkBehaviour
    {
        public virtual int PlayersInLobby => 0;

        public virtual void OnServerEnable() { }
        public virtual void OnServerDisable() { }

        public virtual bool OnPlayerAuth(NetworkConnectionToClient conn) => true;
        public virtual void OnPlayerConnect(NetworkConnectionToClient conn) { }
        public virtual void OnPlayerDisconnect(NetworkConnectionToClient conn) { }
    }
}
