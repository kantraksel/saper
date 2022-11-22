using UnityEngine;

namespace TheGame.Networking.Framework
{
    public abstract class BaseClassicLobbyList : MonoBehaviour
    {
        public abstract void AddPlayer(ulong ID, string name);
        public abstract void RemovePlayer(ulong ID);
        public abstract void ClearPlayers();
        public abstract void UpdatePlayer(ulong ID, bool isReady);
    }
}
