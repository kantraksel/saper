using Mirror;
using TheGame.Networking.Server;
using UnityEngine;

namespace TheGame.Networking.Framework
{
    public class DefaultPlayerManager : BasePlayerManager
    {
        [SerializeField] private GameObject playerPrefab;

        protected override void OnValidate()
        {
            base.OnValidate();

            Debug.Assert(playerPrefab != null, "[DefaultPlayerManager] PlayerPrefab is null");
        }

        public override void CreatePlayer<T>(ulong accountId, T args)
        {
            var client = sessionStorage.GetConnection(accountId);
            var data = client.authenticationData as SessionData;

            CreateEntity(playerPrefab, data.Id, ref data.Entity);

            game.OnPlayerCreated(client, data, args);
        }

        public override void DestroyAllPlayers()
        {
            foreach (var id in playerList.Ids)
            {
                var data = sessionStorage.GetSession(id);
                game.OnPlayerDestroy(data);

                NetworkServer.Destroy(data.Entity);
            }
        }
    }
}
