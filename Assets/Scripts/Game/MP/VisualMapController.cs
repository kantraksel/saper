using System;
using System.Collections.Generic;
using TheGame.Options;
using TheGame.UI.Game;
using UnityEngine;

namespace TheGame.GameModes.Saper
{
    public class VisualMapController : MonoBehaviour
    {
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private PlayerProfile profile;
        [SerializeField] private GameObject mapHolder;
        [SerializeField] private GameOverlay overlay;
        private GameObject mapInstance;
        private readonly Dictionary<ulong, Interior> interiors = new();
        private VisualTile[,] visualMap;
        private ulong activeId;
        private ulong localId => profile.Id;

        private void OnValidate()
        {
            Debug.Assert(tilePrefab != null, "[VisualMapController] TilePrefab is null");
            Debug.Assert(profile != null, "[VisualMapController] Profile is null");
            Debug.Assert(mapHolder != null, "[VisualMapController] MapHolder is null");
            Debug.Assert(overlay != null, "[VisualMapController] Overlay is null");
        }

        private void Awake()
        {
            mapInstance = new("Instance");
            mapInstance.AddComponent<RectTransform>();
            mapInstance.transform.SetParent(mapHolder.transform, true);
            mapInstance.transform.localPosition = Vector3.zero;
        }

        public void CreateMap(ulong id, Vector2Int size, int bombs)
        {
            var map = new Tile[size.x, size.y];
            
            for (int x = 0; x < size.x; ++x)
            {
                for (int y = 0; y < size.y; ++y)
                {
                    var tile = new Tile();
                    tile.Position = new(x, y);
                    map[x, y] = tile;
                }
            }

            interiors[id] = new Interior { Map = map, FlagsUnused = bombs };

            if (id == localId && activeId == 0)
                activeId = id;
        }

        public void GenerateVisualMap(Vector2Int size, Action<VisualTile, bool> onTileInteract)
        {
            if (visualMap != null) return;

            visualMap = MapGenerator.GenerateVisualMap(size.x, size.y, mapInstance.transform, tilePrefab, onTileInteract);
            SelectActiveMap(activeId);
        }

        public void ReleaseMaps()
        {
            if (visualMap != null)
            {
                foreach (var vtile in visualMap)
                {
                    Destroy(vtile.gameObject);
                }
            }
            visualMap = null;
            interiors.Clear();
            activeId = 0;
        }

        public void SelectActiveMap(ulong id)
        {
            if (interiors.TryGetValue(id, out var map))
            {
                activeId = id;

                foreach (var tile in map.Map)
                {
                    UpdateMap(tile, map.FlagsUnused);
                }
            }
        }

        public void UpdateTile(ulong id, Vector2Int position, TileState state)
        {
            UpdateTile(id, interiors[id], position, state);
        }

        private void UpdateTile(ulong id, Interior interior, Vector2Int position, TileState state)
        {
            var tile = interior.Map[position.x, position.y];
            if (state >= 0)
            {
                tile.Value = (int)state;
                state = TileState.Shown;
            }
            UpdateFlagCount(interior, tile.State, state);
            tile.State = state;

            if (activeId == id)
                UpdateMap(tile, interior.FlagsUnused);
        }

        private void UpdateMap(Tile tile, int flagCount)
        {
            var pos = tile.Position;
            var vtile = visualMap[pos.x, pos.y];
            vtile.SetState(tile.State);
            if (tile.State == TileState.Shown)
                vtile.SetFieldValue(tile.Value);

            overlay.UpdateFlagCount(flagCount);
        }

        public void UpdateMap(ulong id, TileInfo[] tiles)
        {
            var map = interiors[id];

            foreach (var info in tiles)
            {
                UpdateTile(id, map, info.position, info.state);
            }
        }

        private void UpdateFlagCount(Interior interior, TileState oldState, TileState newState)
        {
            if (oldState != TileState.Flagged && newState == TileState.Flagged)
            {
                --interior.FlagsUnused;
            }
            else if (oldState == TileState.Flagged && newState != TileState.Flagged)
            {
                ++interior.FlagsUnused;
            }
        }

        public bool IsLocalMapActive()
        {
            return activeId == localId;
        }

        public bool IsLocalMap(ulong id)
        {
            return id == localId;
        }

        class Tile
        {
            public Vector2Int Position { get; set; }
            public TileState State { get; set; }
            public int Value { get; set; }

            public Tile()
            {
                State = TileState.Hidden;
            }
        }

        class Interior
        {
            public Tile[,] Map { get; set; }
            public int FlagsUnused { get; set; }
        }
    }
}
