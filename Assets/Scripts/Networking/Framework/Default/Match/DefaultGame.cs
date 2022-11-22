using Mirror;
using System.Collections;
using TheGame.Networking.Server;
using UnityEngine;

namespace TheGame.Networking.Framework
{
    public class DefaultGame : BaseGame
    {
        [SerializeField] protected BasePlayerManager playerManager;

        protected virtual void OnValidate()
        {
            Debug.Assert(playerManager != null, "[DefaultGame] PlayerManager is null");
        }

        public override IEnumerator OnMatchLoad()
        {
            foreach (var id in playerManager.GetPlayerList())
            {
                yield return null;
                playerManager.CreatePlayer(id, 0);
            }
        }

        public override void OnMatchBegin()
        {
            playerManager.TransferPlayersToClients();
        }

        public override IEnumerator OnMatchCollapse(object data)
        {
            playerManager.TransferPlayersToServer();
            return null;
        }

        public override void OnMatchEnd()
        {
            playerManager.DestroyAllPlayers();
        }

        public override void OnPlayerHotJoined(NetworkConnectionToClient conn, SessionData data)
        {
            playerManager.TransferPlayerToClient(conn, data);
        }

        public override void OnPlayerLeft(NetworkConnectionToClient conn, SessionData data)
        {
            playerManager.TransferPlayerToServer(conn, data);
        }

        public virtual void OnPlayerCreated<T>(NetworkConnectionToClient conn, SessionData data, T args) { }
        public virtual void OnPlayerDestroy(SessionData data) { }
    }
}
