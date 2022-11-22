using UnityEngine;

namespace TheGame.Networking
{
    public abstract class BaseLobbyClient : MonoBehaviour
    {
        public virtual void OnClientEnable() { }
        public virtual void OnClientDisable() { }
        public virtual void OnClientSync() { }
    }
}
