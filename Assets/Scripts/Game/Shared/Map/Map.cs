using System.Collections.Generic;
using UnityEngine;

namespace TheGame.GameModes.Saper
{
    public class Map
    {
        public Tile[,] ServerMap { get; set; }
        //public VisualTile[,] VisualMap { get; set; }

        public Vector2Int Size { get; set; }

        public int BombCount { get; set; }
        public int EmptyCount { get; set; }

        //NOTE: in game vars
        public int BombsFlagged { get; set; }
        public int TilesRevealed { get; set; }
        public bool IsLocked { get; set; }
        public bool IsFinished { get; set; }
        public bool HasBlownUp { get; set; }

        public class Tile
        {
            public Vector2Int Position { get; set; }

            public bool IsBomb { get; set; }

            /// <summary>
            /// Has player clicked the field?
            /// </summary>
            public bool IsRevealed { get; set; }

            /// <summary>
            /// Has player marked the field?
            /// </summary>
            public bool IsFlagged { get; set; }

            /// <summary>
            /// Has player marked the field with question mark?
            /// </summary>
            public bool IsQuestionMarked { get; set; }

            /// <summary>
            /// if IsBomb, always 0
            /// </summary>
            public int NearBombs { get; set; }

            /// <summary>
            /// Only valid if NearBombs == 0
            /// </summary>
            public List<Tile> EmptyRegion { get; set; }

            /// <summary>
            /// EmptyRegion != null && !IsFlagged && !IsQuestionMarked
            /// </summary>
            public bool IsRegionActive { get; set; }

            public TileState GetState()
            {
                TileState state;
                if (IsRevealed)
                {
                    if (IsBomb)
                        state = TileState.Bombed;
                    else
                        state = (TileState)NearBombs;
                }
                else if (IsFlagged)
                    state = TileState.Flagged;
                else if (IsQuestionMarked)
                    state = TileState.QuestionMarked;
                else
                    state = TileState.Hidden;

                return state;
            }
        }
    }
}
