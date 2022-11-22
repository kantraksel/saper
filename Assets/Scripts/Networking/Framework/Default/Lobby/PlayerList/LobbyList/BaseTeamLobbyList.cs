using UnityEngine;
using static TheGame.Networking.Framework.TeamPlayerList;

namespace TheGame.Networking.Framework
{
    public abstract class BaseTeamLobbyList : MonoBehaviour
    {
        public abstract void AddPlayer(ulong ID, string name, Team team);
        public abstract void RemovePlayer(ulong ID);
        public abstract void ClearPlayers();
        public abstract void UpdatePlayer(ulong ID, bool isReady, Team team);
        public abstract void UpdateReadyCount(int readyCount, int totalCount);
    }
}
