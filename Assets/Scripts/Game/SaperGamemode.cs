using Mirror;
using System.Collections;
using System.Collections.Generic;
using TheGame.Networking.Framework;
using TheGame.Networking.Server;
using UnityEngine;

namespace TheGame.GameModes.Saper
{
    public sealed class SaperGamemode : DefaultGame
    {
        [SerializeField] private SessionStorage sessionStorage;
        [SerializeField] private SaperGamemodeClient saperClient;
        [SerializeField] private ClassicPlayerList playerList;
        [SerializeField] private MapSync mapSync;
        [SerializeField] private Scoreboard scoreboard;
        [SerializeField] private GameClock gameClock;
        [SerializeField] private ModeFastMatch modeFastMatch;
        [SerializeField] private ModeStandard modeStandard;

        private Options options;
        private IMode activeMode;
        private readonly List<int> spectatorList = new();
        public IReadOnlyList<int> SpectatorConnections => spectatorList;

        protected override void OnValidate()
        {
            base.OnValidate();

            Debug.Assert(sessionStorage != null, "[SaperGamemode] SessionStorage is null");
            Debug.Assert(saperClient != null, "[SaperGamemode] SaperClient is null");
            Debug.Assert(playerList != null, "[SaperGamemode] PlayerList is null");
            Debug.Assert(mapSync != null, "[SaperGamemode] MapSync is null");
            Debug.Assert(scoreboard != null, "[SaperGamemode] Scoreboard is null");
            Debug.Assert(gameClock != null, "[SaperGamemode] GameClock is null");
            Debug.Assert(modeFastMatch != null, "[SaperGamemode] ModeFastMatch is null");
            Debug.Assert(modeStandard != null, "[SaperGamemode] ModeStandard is null");
        }

        protected override void Setup()
        {
            //TODO: tmp
            options.MapSize = new(8, 8);
            options.BombCount = 10;
            options.StartWaitTime = 3.0f;
            options.AllowSpectateFromStart = true;

            activeMode = modeFastMatch;
        }

        public override void OnMatchBegin()
        {
            base.OnMatchBegin();

            if (options.AllowSpectateFromStart)
                AddPlayersToSpectator();
            StartCoroutine(StartEvent());
        }

        public override IEnumerator OnMatchCollapse(object data)
        {
            _ = base.OnMatchCollapse(data);

            gameClock.Stop();
            saperClient.RpcMatchEnd();
            ChangeMapsState(true);
            activeMode.OnMatchCollapse(data);

            yield return new WaitForSecondsRealtime(7.0f);
        }

        public override void OnMatchEnd()
        {
            base.OnMatchEnd();

            gameClock.Stop();
            spectatorList.Clear();
            scoreboard.Clear();
        }

        public override void OnPlayerHotJoined(NetworkConnectionToClient conn, SessionData data)
        {
            base.OnPlayerHotJoined(conn, data);

            saperClient.RpcAnnounce($"{data.Name} has joined the game");

            var player = data.Entity.GetComponent<Player>();
            var map = player.Map;

            UpdateScoreboardDisconnect(data, false, player);
            saperClient.TargetResyncEnvironment(conn, options.MapSize, options.BombCount);
            mapSync.SendMapToConnection(map, data.Id, conn);

            if (map.IsFinished || options.AllowSpectateFromStart)
                StartSpectatorSync(conn);
        }

        public override void OnPlayerLeft(NetworkConnectionToClient conn, SessionData data)
        {
            base.OnPlayerLeft(conn, data);

            saperClient.RpcAnnounce($"{data.Name} has left the game");
            UpdateScoreboardDisconnect(data, true);

            spectatorList.Remove(conn.connectionId);
        }

        #region StartEvent()
        private IEnumerator StartEvent()
        {
            yield return null;
            var timer = TimerTask();

            var size = options.MapSize;
            int bombs = options.BombCount;
            int nonBombs = (size.x * size.y) - bombs;

            mapSync.RpcGenerateMapChain(size, bombs);

            int playerCount = 0;
            foreach (var item in playerList.Container)
            {
                yield return null;

                ++playerCount;
                var data = sessionStorage.GetSession(item.Key);

                var player = data.Entity.GetComponent<Player>();
                player.Map.Size = size;
                player.Map.BombCount = bombs;
                player.Map.EmptyCount = nonBombs;

                var map = MapGenerator.GenerateServerMap(size.x, size.y, bombs);
                player.Map.ServerMap = map;

                scoreboard.UpdateScore(new PlayerScore { id = item.Key, state = PlayerState.Active });
            }
            activeMode.OnMatchStart(playerCount);

            yield return timer;
            ChangeMapsState(false);

            //TODO: announce
        }

        private void ChangeMapsState(bool lockMap)
        {
            foreach (var item in playerList.Container)
            {
                var data = sessionStorage.GetSession(item.Key);

                var player = data.Entity.GetComponent<Player>();
                player.Map.IsLocked = lockMap;
            }

            //saperClient.RpcUpdateLock(lockMap);
        }

        private IEnumerator TimerTask()
        {
            var startTime = (uint)(options.StartWaitTime * 1000);

            gameClock.StartBackward(startTime);
            yield return gameClock.WaitForEnd();
            gameClock.StartForward();
        }
        #endregion

        #region GameChecks
        public void OnPlayerFinished(NetworkConnection conn, bool isBombed)
        {
            var data = conn.authenticationData as SessionData;

            activeMode.OnPlayerFinished(isBombed);
            StartSpectatorSync(conn);
            saperClient.RpcPlayerFinished(data.Id, isBombed);
            UpdateScoreboard(data);
        }

        public void CheckMatchEndConditions(Map map, SessionData data)
        {
            activeMode.CheckMatchEndConditions(map, data);
        }
        #endregion

        #region Spectator
        private void StartSpectatorSync(NetworkConnection conn, bool sendMaps = true)
        {
            var connId = conn.connectionId;
            if (spectatorList.Contains(connId))
                return;

            spectatorList.Add(connId);

            if (!sendMaps)
                return;

            var session = conn.authenticationData as SessionData;
            foreach (var item in playerList.Container)
            {
                var data = sessionStorage.GetSession(item.Key);
                if (data.Id == session.Id)
                    continue;

                var map = data.Entity.GetComponent<Player>().Map;
                mapSync.SendMapToConnection(map, data.Id, conn);
            }
        }

        private void AddPlayersToSpectator()
        {
            foreach (var item in playerList.Container)
            {
                var conn = sessionStorage.GetConnection(item.Key);
                StartSpectatorSync(conn, false);
            }
        }
        #endregion

        #region scoreboard
        private void UpdateScoreboard(SessionData data)
        {
            var player = data.Entity.GetComponent<Player>();
            var map = player.Map;

            var score = scoreboard.Scores[data.Id];
            score.state = map.HasBlownUp ? PlayerState.Died : PlayerState.Won;
            score.timeEnd = gameClock.Value;
            score.completionPercent = GetCompletion(map);
            score.score = GetScore(ref score);
            scoreboard.UpdateScore(score);
        }

        private int GetCompletion(Map map)
        {
            int value = map.BombsFlagged + map.TilesRevealed;
            value = value * 100 / (map.BombCount + map.EmptyCount);
            return value;
        }

        private int GetScore(ref PlayerScore score)
        {
            if (score.timeEnd == 0) score.timeEnd = 1;
            float value = score.timeEnd / 60;
            value = score.completionPercent / value;
            return (int)value;
        }

        private void UpdateScoreboardDisconnect(SessionData data, bool hasDisconnected, Player player = null)
        {
            if (player == null)
                player = data.Entity.GetComponent<Player>();

            if (player.Map.IsFinished)
                return;

            var score = scoreboard.Scores[data.Id];
            score.state = hasDisconnected ? PlayerState.Disconnected : PlayerState.Active;
            scoreboard.UpdateScore(score);
        }
        #endregion

        public struct Options
        {
            public Vector2Int MapSize;
            public int BombCount;

            public float StartWaitTime;
            public bool AllowSpectateFromStart;
        }
    }
}
