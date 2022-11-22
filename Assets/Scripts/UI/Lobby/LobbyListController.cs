using System.Collections.Generic;
using TheGame.Networking.Framework;
using UnityEngine;

namespace TheGame.UI.Lobby
{
    public class LobbyListController : BaseClassicLobbyList
    {
        [SerializeField] private List<PlayerTile> tiles;

        private int freeSlot;

        public override void RemovePlayer(ulong ID)
        {
            for (int i = 0; i < freeSlot; ++i)
            {
                if (tiles[i].ID == ID)
                {
                    int limit = freeSlot - 1;
                    for (; i < limit; ++i)
                    {
                        tiles[i + 1].MoveTo(tiles[i]);
                    }
                    tiles[limit].DisableTile();
                    --freeSlot;

                    return;
                }
            }

            Debug.LogWarning("[LobbyListController] Could not find player to remove");
        }

        public override void AddPlayer(ulong ID, string name)
        {
            if (freeSlot + 1 == tiles.Count)
                Debug.LogWarning("[LobbyListController] List is full");
            else
            {
                tiles[freeSlot].EnableTile(ID, name);
                ++freeSlot;
            }
        }

        public override void ClearPlayers()
        {
            tiles.ForEach((tile) => tile.DisableTile());
            freeSlot = 0;
        }

        public override void UpdatePlayer(ulong ID, bool isReady)
        {
            for (int i = 0; i < freeSlot; ++i)
            {
                var tile = tiles[i];
                if (tile.ID == ID)
                {
                    tile.ChangeReadyStatus(isReady);
                    return;
                }
            }

            Debug.LogWarning("[LobbyListController] Could not find player to update");
        }
    }
}
