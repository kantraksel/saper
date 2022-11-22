using System;
using UnityEngine;

namespace TheGame.GameModes.Saper
{
    public class TileInteract : MonoBehaviour
    {
        public Action<Map, Vector2Int, TileState, bool?> OnInteractFinish;

        public void Interact(Map map, Map.Tile tile, bool doReveal)
        {
            if (tile.IsRevealed)
                return;

            if (doReveal && tile.IsRegionActive)
                RevealRegion(map, tile);
            else
                InteractTile(map, tile, doReveal);
        }

        private void RevealRegion(Map map, Map.Tile tile)
        {
            foreach (var entry in tile.EmptyRegion)
            {
                InteractTile(map, entry, true, true);
            }
        }

        private void InteractTile(Map map, Map.Tile tile, bool doReveal, bool overrideMarks = false)
        {
            if (tile.IsRevealed)
                return;

            bool? mapFinishBlown = null;
            TileState updateState;
            if (doReveal)
            {
                if (overrideMarks)
                {
                    if (tile.IsFlagged && tile.IsBomb)
                        --map.BombsFlagged;

                    tile.IsFlagged = false;
                    tile.IsQuestionMarked = false;
                }
                else if (tile.IsFlagged || tile.IsQuestionMarked)
                    return;

                if (tile.IsBomb)
                {
                    map.IsLocked = true;
                    map.IsFinished = true;
                    map.HasBlownUp = true;
                    updateState = TileState.Bombed;
                    mapFinishBlown = true;
                }
                else
                {
                    updateState = (TileState)tile.NearBombs;
                    ++map.TilesRevealed;
                }

                tile.IsRevealed = true;
            }
            else if (tile.IsFlagged)
            {
                tile.IsFlagged = false;
                tile.IsQuestionMarked = true;
                updateState = TileState.QuestionMarked;

                if (tile.IsBomb)
                    --map.BombsFlagged;
            }
            else if (tile.IsQuestionMarked)
            {
                tile.IsQuestionMarked = false;
                updateState = TileState.Hidden;

                if (tile.EmptyRegion != null)
                    tile.IsRegionActive = true;
            }
            else
            {
                tile.IsFlagged = true;
                updateState = TileState.Flagged;

                if (tile.IsBomb)
                    ++map.BombsFlagged;

                tile.IsRegionActive = false;
            }

            if (IsMapSolved(map))
            {
                map.IsFinished = true;
                map.IsLocked = true;
                mapFinishBlown = false;
            }

            OnInteractFinish?.Invoke(map, tile.Position, updateState, mapFinishBlown);
        }

        private bool IsMapSolved(Map map)
        {
            return map.BombsFlagged == map.BombCount
                && map.EmptyCount == map.TilesRevealed;
        }
    }
}
