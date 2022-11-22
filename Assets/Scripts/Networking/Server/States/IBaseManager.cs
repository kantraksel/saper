using Mirror;

namespace TheGame.Networking.Server.States
{
    internal interface IBaseManager
    {
        public void Setup(ServerCoordinator parent);
        public void Enable();
        public void Disable();

        public bool OnPlayerAuth(NetworkConnectionToClient conn);
        public void OnPlayerConnect(NetworkConnectionToClient conn);
        public void OnPlayerDisconnect(NetworkConnectionToClient conn);
    }
}
