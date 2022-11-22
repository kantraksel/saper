using System.Collections;
using UnityEngine;

namespace TheGame.Networking.Client.States
{
    internal class Match : MonoBehaviour, IBaseManager
    {
        private ClientCoordinator parent;
        private BaseGameClient gameMode;
        private bool isReady;

        public void Setup(ClientCoordinator parent)
        {
            this.parent = parent;
            gameMode = parent.GameMode;
        }

        public IEnumerator Load()
        {
            isReady = false;

            yield return gameMode.OnMatchLoad();

            parent.Ready();
            yield return new WaitUntil(() => isReady);
        }

        public void Enable()
        {
            gameMode.OnMatchLoaded();
        }

        public void Disable()
        {
            gameMode.OnMatchUnload();
        }

        public void SetMatchStarted()
        {
            isReady = true;
        }
    }
}
