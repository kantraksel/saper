using Mirror;
using System.Collections.Generic;
using TheGame.Networking.Server;
using UnityEngine;

namespace TheGame.Networking.Framework
{
    public class DefaultLobby : BaseLobby
    {
        [SerializeField] protected ServerCoordinator serverCoordinator;
        [SerializeField] protected SessionStorage sessionStorage;
        [SerializeField] protected BasePlayerList playerList;
        protected short readyCount;
        protected bool isEnabled;

        public override int PlayersInLobby => playerList.PlayerCount;

        protected virtual void OnValidate()
        {
            Debug.Assert(serverCoordinator != null, "[DefaultLobby] ServerCoordinator is null");
            Debug.Assert(sessionStorage != null, "[DefaultLobby] SessionStorage is null");
            Debug.Assert(playerList != null, "[DefaultLobby] PlayerList is null");
        }

        public override void OnServerEnable()
        {
            isEnabled = true;
            readyCount = 0;

            playerList.AllowListChange = true;
            RemoveDisconnectedPlayers();
            sessionStorage.MarkAllSessionsAsNew();
        }

        public override void OnServerDisable()
        {
            playerList.AllowListChange = false;
            isEnabled = false;
        }

        public override bool OnPlayerAuth(NetworkConnectionToClient conn)
        {
            var data = conn.authenticationData as SessionData;
            if (playerList.Add(data.Id, data.Name))
                return true;

            conn.Disconnect(GDisconnectCode.LobbyNotAvailable);
            return false;
        }

        public override void OnPlayerDisconnect(NetworkConnectionToClient conn)
        {
            var data = conn.authenticationData as SessionData;
            data.AllowDestroy = true;

            if (data.IsPlayer)
                --readyCount;

            playerList.Remove(data.Id);
        }

        [Command(requiresAuthority = false)]
        public void CmdReady(NetworkConnectionToClient sender = null)
        {
            if (!isEnabled) return;

            var data = sender.authenticationData as SessionData;
            data.IsPlayer = !data.IsPlayer;

            if (data.IsPlayer) ++readyCount;
            else --readyCount;

            playerList.UpdateReadyState(data.Id, data.IsPlayer);
            if (PlayersInLobby == readyCount)
                serverCoordinator.LoadMatch();
        }

        protected void RemoveDisconnectedPlayers()
        {
            //NOTE: SessionStorage is reliable, because "old sessions" are removed in Match.Disable()

            var ids = new List<ulong>();

            foreach (var id in playerList.Ids)
                ids.Add(id);

            playerList.Clear();

            foreach (var id in ids)
            {
                var data = sessionStorage.GetSession(id);
                if (data == null) continue;
                playerList.Add(data.Id, data.Name);
            }
        }
    }
}
