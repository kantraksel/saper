using TheGame.Networking.Server;
using TheGame.Networking.Server.States;
using UnityEngine;

namespace TheGame.GameModes.Saper
{
    public class ModeFastMatch : MonoBehaviour, IMode
    {
        [SerializeField] private SaperGamemodeClient saperClient;
        [SerializeField] private Match match;
        private int playerCount;
        private int playersBombed;

        private void OnValidate()
        {
            Debug.Assert(saperClient != null, "[ModeFastMatch] SaperClient is null");
            Debug.Assert(match != null, "[ModeFastMatch] Match is null");
        }

        public void OnMatchStart(int playerCount)
        {
            this.playerCount = playerCount;
            playersBombed = 0;
        }

        public void OnMatchCollapse(object rawData)
        {
            if (rawData != null)
            {
                var data = rawData as SessionData;
                saperClient.RpcAnnounce($"{data.Name} won!");
            }
            else
            {
                saperClient.RpcAnnounce("No winners!");
            }
        }

        public void CheckMatchEndConditions(Map map, SessionData data)
        {
            if (map.IsFinished && !map.HasBlownUp)
                match.CollapseMatch(data);
            else if (playersBombed == playerCount)
                match.CollapseMatch(null);
        }

        public void OnPlayerFinished(bool isBombed)
        {
            if (isBombed)
                ++playersBombed;
        }
    }
}
