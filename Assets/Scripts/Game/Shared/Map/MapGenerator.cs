using System.Collections.Generic;
using TheGame.UI.Game;
using UnityEngine;

namespace TheGame.GameModes.Saper
{
    public static class MapGenerator
    {
        /// <summary>
        /// Generates server map
        /// </summary>
        /// <param name="sizeX">Map's X size</param>
        /// <param name="sizeY">Map's Y size</param>
        /// <param name="bombCount">Number of bombs on map</param>
        /// <param name="seed">Map seed. If 0, random seed is used</param>
        public static Map.Tile[,] GenerateServerMap(int sizeX, int sizeY, int bombCount, int seed = 0)
        {
            //initalize value map
            var map = new Map.Tile[sizeX, sizeY];

            for (int i = 0; i < sizeX; ++i)
            {
                for (int j = 0; j < sizeY; ++j)
                {
                    var tile = new Map.Tile();
                    tile.Position = new(i, j);
                    map[i, j] = tile;
                }
            }

            if (seed == 0)
                seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            Debug.Log($"Seed: {seed}");
            var rng = new System.Random(seed);

            //number of bombs cant be higher than tiles count
            var a = sizeX * sizeY;
            if (bombCount > a)
                bombCount = a;

            //loop through all bombs
            for (int i = 0; i < bombCount; ++i)
            {
                //get random cords
                var x = rng.Next(0, sizeX);
                var y = rng.Next(0, sizeY);
                var tile = map[x, y];

                //if there is a bomb just find another place
                if (tile.IsBomb)
                {
                    --i;
                    continue;
                }

                //sets bomb on this random cords
                tile.IsBomb = true;

                //setting neighbours of the tiles
                AddNearbyBombArea(map, x, y, sizeX, sizeY);
            }

            BuildEmptyRegions(map, sizeX, sizeY);
            return map;
        }

        private static void AddNearbyBombArea(Map.Tile[,] map, int x, int y, int sizeX, int sizeY)
        {
            --x;
            --y;

            //X = cordX - 1
            if (x >= 0)
                AddNearbyBombColumn(map, x, y, sizeY);

            //X = cordX
            ++x;
            AddNearbyBombColumn(map, x, y, sizeY);

            //X = cordX + 1
            ++x;
            if (x < sizeX)
                AddNearbyBombColumn(map, x, y, sizeY);
        }

        private static void AddNearbyBombColumn(Map.Tile[,] map, int x, int y, int sizeY)
        {
            if (y >= 0)
                AddNearbyBomb(map, x, y);

            ++y;
            AddNearbyBomb(map, x, y);

            ++y;
            if (y < sizeY)
                AddNearbyBomb(map, x, y);
        }

        private static void AddNearbyBomb(Map.Tile[,] map, int x, int y)
        {
            var tile = map[x, y];
            if (!tile.IsBomb)
                ++tile.NearBombs;
        }

        private static void BuildEmptyRegions(Map.Tile[,] map, int sizeX, int sizeY)
        {
            var emptyMap = new int[sizeX, sizeY];

            CreateRegions(map, emptyMap, sizeX, sizeY);
            MergeRegions(emptyMap, sizeX, sizeY);
            AttachRegions(map, emptyMap, sizeX, sizeY);

#if DEBUG
            //NOTE: debug regions
            //DebugRegions(map, emptyMap, sizeX, sizeY);
#endif
        }

        private static void CreateRegions(Map.Tile[,] map, int[,] emptyMap, int sizeX, int sizeY)
        {
            int treeIndex = 0;
            bool indexUsed = false;
            for (int y = 0; y < sizeY; ++y)
            {
                for (int x = 0; x < sizeX; ++x)
                {
                    var tile = map[x, y];
                    if (tile.IsBomb || tile.NearBombs != 0)
                    {
                        if (indexUsed)
                        {
                            ++treeIndex;
                            indexUsed = false;
                        }
                        emptyMap[x, y] = -1;
                        continue;
                    }

                    if (x > 0)
                    {
                        var prevIndex = emptyMap[x - 1, y];
                        if (prevIndex != -1)
                        {
                            emptyMap[x, y] = prevIndex;
                            continue;
                        }
                    }

                    emptyMap[x, y] = treeIndex;
                    indexUsed = true;
                }

                if (indexUsed)
                {
                    ++treeIndex;
                    indexUsed = false;
                }
            }
        }

        private static void MergeRegions(int[,] emptyMap, int sizeX, int sizeY)
        {
            //todo: check second approach - group regions on lists and go to step 3
            for (int y = 0; y < sizeY - 1; ++y)
            {
                for (int x = 0; x < sizeX; ++x)
                {
                    MergeArea(emptyMap, sizeX, x, y, y + 1);
                }
            }

            for (int y = sizeY - 1; y > 0; --y)
            {
                for (int x = 0; x < sizeX; ++x)
                {
                    MergeArea(emptyMap, sizeX, x, y, y - 1);
                }
            }
        }

        private static void MergeArea(int[,] emptyMap, int sizeX, int x, int y, int pairY)
        {
            int minX = x - 1;
            int maxX = x + 1;

            if (minX < 0) minX = 0;
            if (maxX >= sizeX) maxX = sizeX - 1;

            var index = emptyMap[x, y];
            for (int xn = minX; xn <= maxX; ++xn)
            {
                var nIndex = emptyMap[xn, pairY];

                if (index != -1 && nIndex != -1 && index < nIndex)
                {
                    WriteRegion(index, xn, pairY, emptyMap, sizeX);
                }
            }
        }

        private static void WriteRegion(int index, int x, int y, int[,] map, int sizeX)
        {
            map[x, y] = index;

            for (int xn = x + 1; xn < sizeX; ++xn)
            {
                if (map[xn, y] == -1)
                    break;

                map[xn, y] = index;
            }

            for (int xn = x - 1; xn >= 0; --xn)
            {
                if (map[xn, y] == -1)
                    break;

                map[xn, y] = index;
            }
        }

        private static void AttachRegions(Map.Tile[,] map, int[,] emptyMap, int sizeX, int sizeY)
        {
            var regions = new Dictionary<int, List<Map.Tile>>();
            foreach (var index in emptyMap)
            {
                if (index == -1)
                    continue;

                if (!regions.ContainsKey(index))
                    regions[index] = new();
            }

            for (int x = 0; x < sizeX; ++x)
            {
                for (int y = 0; y < sizeY; ++y)
                {
                    var index = emptyMap[x, y];
                    if (index != -1)
                    {
                        var region = regions[index];
                        var tile = map[x, y];
                        region.Add(tile);
                        tile.EmptyRegion = region;
                        tile.IsRegionActive = true;
                    }
                }
            }

            var list = new List<int>();
            for (int x = 0; x < sizeX; ++x)
            {
                for (int y = 0; y < sizeY; ++y)
                {
                    var index = emptyMap[x, y];
                    if (index == -1)
                        AttachToNearbyRegions(x, y, emptyMap, map, sizeX, sizeY, regions, list);
                }
            }
        }

        private static void AttachToNearbyRegions(int x, int y, int[,] emptyMap, Map.Tile[,] map, int sizeX, int sizeY, Dictionary<int, List<Map.Tile>> regions, List<int> list)
        {
            int minX = x - 1;
            int maxX = x + 1;
            int minY = y - 1;
            int maxY = y + 1;

            if (minX < 0) minX = 0;
            if (maxX >= sizeX) maxX = sizeX - 1;
            if (minY < 0) minY = 0;
            if (maxY >= sizeY) maxY = sizeY - 1;

            for (int i = minX; i <= maxX; ++i)
            {
                for (int j = minY; j <= maxY; ++j)
                {
                    var index = emptyMap[i, j];
                    if (index == -1)
                        continue;

                    if (!list.Contains(index))
                        list.Add(index);
                }
            }

            foreach (var index in list)
            {
                regions[index].Add(map[x, y]);
            }
            list.Clear();
        }

#if DEBUG
        private static void DebugRegions(Map.Tile[,] map, int[,] emptyMap, int sizeX, int sizeY)
        {
            for (int x = 0; x < sizeX; ++x)
            {
                for (int y = 0; y < sizeY; ++y)
                {
                    var tile = map[x, y];
                    tile.NearBombs = emptyMap[x, y] + 1;
                }
            }
        }
#endif

        /// <summary>
        /// Generates visual (client) map
        /// </summary>
        /// <param name="sizeX">Map's width</param>
        /// <param name="sizeY">Map's height</param>
        /// <param name="parent">Parent of generated tiles. The object will be also scaled to fit map to screen!</param>
        /// <param name="tilePrefab">Tile prefab used in spawning tiles</param>
        /// <param name="interactCallback">Tile interact callback</param>
        public static VisualTile[,] GenerateVisualMap(int sizeX, int sizeY, Transform parent, GameObject tilePrefab, System.Action<VisualTile, bool> interactCallback)
        {
            var map = new VisualTile[sizeX, sizeY];

            var rectTransform = (RectTransform)tilePrefab.transform;
            var tileSize = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y);

            var startPosition = new Vector3(sizeX / 2, sizeY / 2) - new Vector3(0.5f, 0.5f);
            startPosition = ScaleMembers(startPosition, tileSize);
            for (int x = 0; x < sizeX; ++x)
            {
                for (int y = 0; y < sizeY; ++y)
                {
                    //create tile
                    var gameObject = Object.Instantiate(tilePrefab, parent);

                    //initialize tile
                    var input = gameObject.GetComponent<TileInput>();
                    input.OnInteract = interactCallback;

                    var tile = input.Tile;
                    tile.Position = new(x, y);
                    tile.Initialize();
                    map[x, y] = tile;

                    //set position
                    var position = Vector3.zero;
                    position -= startPosition; //align to middle of map
                    position += ScaleMembers(new(x, y), tileSize);
                    gameObject.transform.localPosition = position;
                }
            }

            float scaleX = sizeX;
            float scaleY = sizeY;
            scaleX *= 0.125f;
            scaleY *= 0.125f;
            parent.localScale = new Vector3(1 * scaleX, 1 * scaleY, 1);
            return map;
        }

        private static Vector3 ScaleMembers(Vector3 v, Vector2 scale)
        {
            v.x *= scale.x;
            v.y *= scale.y;
            return v;
        }
    }
}
