using Mirror;
using System.Collections;
using TheGame.Networking.Server;
using UnityEngine;

namespace TheGame.Networking
{
    public abstract class BaseGame : MonoBehaviour
    {
        [SerializeField] protected int loadTimeout;
        public int LoadTimeout => loadTimeout;

        protected virtual void Reset()
        {
            loadTimeout = 60;
        }

        protected void Awake()
        {
            Setup();
        }

        protected virtual void Setup() { }

        public virtual IEnumerator OnMatchLoad() => null;
        public virtual void OnMatchBegin() { }
        public virtual IEnumerator OnMatchCollapse(object data) => null;
        public virtual void OnMatchEnd() { }

        public virtual void OnPlayerHotJoined(NetworkConnectionToClient conn, SessionData data) { }
        public virtual void OnPlayerLeft(NetworkConnectionToClient conn, SessionData data) { }
    }
}
