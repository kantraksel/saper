using TheGame.Networking.Server;
using TheGame.Networking.Server.States;
using UnityEngine;

namespace TheGame.GameModes.Saper
{
    public class ModeStandard : MonoBehaviour, IMode
    {
        [SerializeField] private SaperGamemodeClient saperClient;
        [SerializeField] private Match match;
        private int playerCount;
        private int playersFinished;

        private void OnValidate()
        {
            Debug.Assert(saperClient != null, "[ModeStandard] SaperClient is null");
            Debug.Assert(match != null, "[ModeStandard] Match is null");
        }

        public void OnMatchStart(int playerCount)
        {
            this.playerCount = playerCount;
            playersFinished = 0;
        }

        public void OnMatchCollapse(object rawData)
        {
            /*
            if (rawData != null)
            {
                var data = rawData as SessionData;
                saperClient.RpcAnnounce($"{data.AccountName} won!");
            }
            else
            {
                saperClient.RpcAnnounce("No winners!");
            }
            */
        }

        public void CheckMatchEndConditions(Map map, SessionData data)
        {
            if (playersFinished == playerCount)
                match.CollapseMatch(null);
        }

        public void OnPlayerFinished(bool isBombed)
        {
            ++playersFinished;
        }
    }
}
