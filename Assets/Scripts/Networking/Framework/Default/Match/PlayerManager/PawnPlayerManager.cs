using Mirror;
using TheGame.Networking.Server;
using UnityEngine;

namespace TheGame.Networking.Framework
{
    public class PawnPlayerManager : BasePlayerManager
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject controllerPrefab;

        protected override void OnValidate()
        {
            base.OnValidate();

            Debug.Assert(playerPrefab != null, "[PawnPlayerManager] PlayerPrefab is null");
            Debug.Assert(controllerPrefab != null, "[PawnPlayerManager] ControllerPrefab is null");
        }

        public override void CreatePlayer<T>(ulong accountId, T args)
        {
            var client = sessionStorage.GetConnection(accountId);
            var data = client.authenticationData as SessionData;

            CreateEntity(playerPrefab, data.Id, ref data.PawnEntity);
            CreateEntity(controllerPrefab, data.Id, ref data.Entity);

            NetworkServer.Spawn(data.PawnEntity);
            game.OnPlayerCreated(client, data, args);
        }

        public override void DestroyAllPlayers()
        {
            foreach (var id in playerList.Ids)
            {
                var data = sessionStorage.GetSession(id);
                game.OnPlayerDestroy(data);

                NetworkServer.Destroy(data.Entity);
                NetworkServer.Destroy(data.PawnEntity);
            }
        }
    }
}
