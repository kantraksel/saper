using System.Collections;
using UnityEngine;

namespace TheGame.Networking.Client.States
{
    internal class Lobby : MonoBehaviour, IBaseManager
    {
        private ClientCoordinator parent;
        private BaseLobbyClient lobby;
        private bool isReady;

        public void Setup(ClientCoordinator parent)
        {
            this.parent = parent;
            lobby = parent.GameLobby;
        }

        public IEnumerator Load()
        {
            isReady = false;

            parent.Ready();
            yield return new WaitUntil(() => isReady);
        }

        public void Enable()
        {
            lobby.OnClientEnable();
        }

        public void Disable()
        {
            lobby.OnClientDisable();
        }

        public void OnClientReady()
        {
            isReady = true;

            lobby.OnClientSync();
        }
    }
}
