using System.Collections;
using TheGame.UI.Game;
using UnityEngine;

namespace TheGame.GameModes.Saper.SP
{
    public class SinglePlayer : MonoBehaviour
    {
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private GameObject parent;

        [SerializeField] private Vector2Int size;
        [SerializeField] private int bombCount;
        private Map map;
        private VisualTile[,] visualMap;
        private bool isLocked;
        [SerializeField] private int seed;

        [SerializeField] private TileInteract tileInteract;

        private void OnValidate()
        {
            Debug.Assert(parent != null, "[SinglePlayer] Parent is null");
            Debug.Assert(tilePrefab != null, "[SinglePlayer] TilePrefab is null");
            Debug.Assert(tileInteract != null, "[SinglePlayer] TileInteract is null");
        }

        private void Start()
        {
            map = new()
            {
                BombCount = bombCount,
                EmptyCount = (size.x * size.y) - bombCount,
                Size = size,
                ServerMap = MapGenerator.GenerateServerMap(size.x, size.y, bombCount, seed),
            };
            visualMap = MapGenerator.GenerateVisualMap(size.x, size.y, parent.transform, tilePrefab, Interact);

            isLocked = false;

            tileInteract.OnInteractFinish = (map, position, updateState, _) =>
            {
                UpdateTile(position, updateState);
                CheckEndConditions(map);
            };
        }

        private void Interact(VisualTile tile, bool isLeftMouseButton)
        {
            if (isLocked) return;

            int x = tile.Position.x;
            int y = tile.Position.y;
            var stile = map.ServerMap[x, y];
            if (stile.IsRevealed)
                return;

            tileInteract.Interact(map, stile, isLeftMouseButton);
        }

        private void CheckEndConditions(Map map)
        {
            //NOTE: Saper v1
            if (map.IsFinished)
            {
                isLocked = true;

                if (map.HasBlownUp)
                    Debug.LogError("BOOOOOOOOM!");
                else
                    Debug.Log("Kalm");
            }
        }

        private void UpdateTile(Vector2Int position, TileState state)
        {
            var value = 0;
            if (state >= 0)
            {
                value = (int)state;
                state = TileState.Shown;
            }

            var vtile = visualMap[position.x, position.y];
            if (state == TileState.Shown)
                vtile.SetFieldValue(value);
            vtile.SetState(state);
        }

        public void ResetMap()
        {
            foreach (var tile in visualMap)
            {
                Destroy(tile.gameObject);
            }

            Start();
        }
    }
}
