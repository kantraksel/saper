using Mirror;
using System.Collections.Generic;
using TheGame.Networking.Framework;
using TheGame.Networking.Server;
using TheGame.UI;
using TheGame.UI.Game;
using UnityEngine;

namespace TheGame.GameModes.Saper
{
    public class MapSync : NetworkBehaviour
    {
        public static MapSync Instance { get; private set; }

        [SerializeField] private SaperGamemode saperGamemode;
        [SerializeField] private VisualMapController mapController;
        [SerializeField] private ClassicPlayerList playerList;
        [SerializeField] private TileInteract tileInteract;
        [SerializeField] private MenuController menuController;
        public Player LocalPlayer { private get; set; }

        private void OnValidate()
        {
            Debug.Assert(saperGamemode != null, "[MapSync] SaperGamemode is null");
            Debug.Assert(mapController != null, "[MapSync] MapController is null");
            Debug.Assert(playerList != null, "[MapSync] PlayerList is null");
            Debug.Assert(tileInteract != null, "[MapSync] TileInteract is null");
            Debug.Assert(menuController != null, "[MapSync] MenuController is null");
        }

        private void Awake()
        {
            Instance = this;
        }

        #region Server
        public void Interact(NetworkConnection conn, Map map, Map.Tile tile, bool doReveal)
        {
            tileInteract.OnInteractFinish = (map, position, updateState, mapFinishBlown) =>
            {
                if (mapFinishBlown.HasValue)
                {
                    saperGamemode.OnPlayerFinished(conn, mapFinishBlown.Value);
                }

                var data = conn.authenticationData as SessionData;
                SendTileState(conn, data.Id, position, updateState);
                saperGamemode.CheckMatchEndConditions(map, data);
            };

            tileInteract.Interact(map, tile, doReveal);
        }

        private void SendTileState(NetworkConnection conn, ulong mapOwnerId, Vector2Int position, TileState updateState)
        {
            TargetUpdateTile(conn, mapOwnerId, position, updateState);

            foreach (var connId in saperGamemode.SpectatorConnections)
            {
                if (!NetworkServer.connections.TryGetValue(connId, out var spectator))
                    continue;

                var session = spectator.authenticationData as SessionData;
                if (session.Id == mapOwnerId)
                    continue;

                TargetUpdateTile(spectator, mapOwnerId, position, updateState);
            }
        }

        public void SendMapToConnection(Map map, ulong mapOwnerId, NetworkConnection conn)
        {
            var changeList = new List<TileInfo>();
            foreach (var tile in map.ServerMap)
            {
                var state = tile.GetState();
                if (state == TileState.Hidden)
                    continue;

                var info = new TileInfo();
                info.position = tile.Position;
                info.state = state;
                changeList.Add(info);
            }

            if (changeList.Count == 0)
                return;

            TargetUpdateMap(conn, mapOwnerId, changeList.ToArray());
        }
        #endregion

        #region Client
        [ClientRpc]
        public void RpcGenerateMapChain(Vector2Int size, int bombs) =>
            GenerateMapChain(size, bombs);

        public void GenerateMapChain(Vector2Int size, int bombs)
        {
            foreach (var id in playerList.Container.Keys)
            {
                mapController.CreateMap(id, size, bombs);
            }
            mapController.GenerateVisualMap(size, OnInteract);
        }

        [TargetRpc]
        public void TargetUpdateTile(NetworkConnection target, ulong id, Vector2Int position, TileState state)
        {
            mapController.UpdateTile(id, position, state);
        }

        [TargetRpc]
        public void TargetUpdateMap(NetworkConnection target, ulong id, TileInfo[] tiles)
        {
            mapController.UpdateMap(id, tiles);
        }

        private void OnInteract(VisualTile tile, bool doReveal)
        {
            if (menuController.IsPauseMenuActive() || !mapController.IsLocalMapActive())
                return;

            if (LocalPlayer != null)
                LocalPlayer.CmdInteract(tile.Position, doReveal);
        }
        #endregion
    }

    public struct TileInfo
    {
        public Vector2Int position;
        public TileState state;
    }
}
