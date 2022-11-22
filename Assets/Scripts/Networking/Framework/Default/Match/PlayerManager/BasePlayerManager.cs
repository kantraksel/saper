using Mirror;
using System.Collections.Generic;
using TheGame.Networking.Server;
using UnityEngine;

namespace TheGame.Networking.Framework
{
    public abstract class BasePlayerManager : MonoBehaviour
    {
        [SerializeField] protected SessionStorage sessionStorage;
        [SerializeField] protected BasePlayerList playerList;
        [SerializeField] protected DefaultGame game;

        protected GameObject players;

        protected virtual void OnValidate()
        {
            Debug.Assert(sessionStorage != null, "[BasePlayerManager] SessionStorage is null");
            Debug.Assert(playerList != null, "[BasePlayerManager] PlayerList is null");
            Debug.Assert(game != null, "[BasePlayerManager] Game is null");
        }

        protected virtual void Awake()
        {
            players = new("Players");
        }

        protected virtual void OnDestroy()
        {
            Destroy(players);
        }

        public abstract void CreatePlayer<T>(ulong accountId, T args);
        public abstract void DestroyAllPlayers();

        public virtual void TransferPlayersToServer()
        {
            foreach (var id in playerList.Ids)
            {
                var data = sessionStorage.GetSession(id);
                if (data.Entity)
                {
                    var conn = data.Entity.GetComponent<NetworkIdentity>().connectionToClient;
                    if (conn != null)
                        NetworkServer.RemovePlayerForConnection(conn, false);
                }
            }
        }

        public virtual void TransferPlayersToClients()
        {
            foreach (var id in playerList.Ids)
            {
                var conn = sessionStorage.GetConnection(id);
                if (conn != null && conn.isReady)
                {
                    var data = conn.authenticationData as SessionData;
                    NetworkServer.AddPlayerForConnection(conn, data.Entity);
                }
            }
        }

        public virtual void TransferPlayerToServer(NetworkConnectionToClient conn, SessionData data)
        {
            NetworkServer.RemovePlayerForConnection(conn, false);
        }

        public virtual void TransferPlayerToClient(NetworkConnectionToClient conn, SessionData data)
        {
            NetworkServer.AddPlayerForConnection(conn, data.Entity);
        }

        protected void CreateEntity(GameObject prefab, ulong ownerId, ref GameObject entity)
        {
            if (entity)
            {
                Debug.LogError($"[PlayerManager] {ownerId}'s {prefab.name} already exists!");
                Debug.LogWarning("[PlayerManager] Continuing, but game may be broken!");
            }

            entity = Instantiate(prefab, players.transform);
            entity.name = $"{prefab.name} [ownerId={ownerId}]";
        }

        public IEnumerable<ulong> GetPlayerList() => playerList.Ids;
    }
}
