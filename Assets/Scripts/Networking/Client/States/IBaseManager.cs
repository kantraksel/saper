using System.Collections;

namespace TheGame.Networking.Client.States
{
    internal interface IBaseManager
    {
        public void Setup(ClientCoordinator parent);
        public IEnumerator Load() => null;
        public void Enable();
        public void Disable();

        public void OnClientReady() { }
    }
}
