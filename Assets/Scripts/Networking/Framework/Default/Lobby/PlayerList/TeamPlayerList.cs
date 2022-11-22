using Mirror;
using System.Collections.Generic;
using UnityEngine;

namespace TheGame.Networking.Framework
{
    public class TeamPlayerList : BasePlayerList
    {
        [SerializeField] private BaseTeamLobbyList lobbyList;
        [SerializeField] private ServerInfo serverInfo;
        [SerializeField] private int teamLimit;
        private readonly SyncDictionary<ulong, Player> container = new();
        private int redCount;
        private int blueCount;
        private readonly SyncVar<int> readyCount = new(default);

        public IReadOnlyDictionary<ulong, Player> Container => container;
        public override IEnumerable<ulong> Ids => Container.Keys;
        public override int PlayerCount => Container.Count;

        private void OnValidate()
        {
            Debug.Assert(lobbyList != null, "[TeamPlayerList] LobbyList is null");
            Debug.Assert(serverInfo != null, "[TeamPlayerList] ServerInfo is null");
            if (teamLimit <= 0)
            {
                teamLimit = 5;
                Debug.LogWarning("[TeamPlayerList] Set team limit to 5");
            }
        }

        private void Awake()
        {
            container.Callback += OnListChanged;
            readyCount.Callback += OnReadyCountUpdate;
        }

        public override void OnStopClient()
        {
            lobbyList.ClearPlayers();
        }

        public override void OnStopServer()
        {
            Clear();
        }

        #region server
        public override bool Add(ulong ID, string name)
        {
            if (!AllowListChange || container.ContainsKey(ID))
                return false;

            var team = GetAvailableTeam();
            if (team == Team.Neutral)
                return false;

            var player = new Player { ID = ID, Name = name, Team = team };
            container.Add(ID, player);
            return true;
        }

        public override void Remove(ulong ID)
        {
            if (!AllowListChange) return;
            if (container.Remove(ID, out var player))
            {
                switch (player.Team)
                {
                    case Team.Blue:
                        --blueCount;
                        break;

                    case Team.Red:
                        --redCount;
                        break;
                }
            }
        }

        public override void Clear()
        {
            container.Clear();
            redCount = 0;
            blueCount = 0;
            readyCount.Value = 0;
        }

        public override void UpdateReadyState(ulong ID, bool isReady)
        {
            if (!AllowListChange || !container.TryGetValue(ID, out var player)) return;

            if (!player.IsReady)
            {
                if (isReady)
                    ++readyCount.Value;
            }
            else
            {
                if (!isReady)
                    --readyCount.Value;
            }

            player.IsReady = isReady;
            container[ID] = player;
        }

        public bool ChangeTeam(ulong ID, Team targetTeam)
        {
            var player = container[ID];
            if (player.Team == targetTeam)
                return false;

            switch (targetTeam)
            {
                case Team.Blue:
                    {
                        if (blueCount >= teamLimit)
                            return false;

                        ++blueCount;
                        break;
                    }

                case Team.Red:
                    {
                        if (redCount >= teamLimit)
                            return false;

                        ++redCount;
                        break;
                    }

                default:
                    return false;
            }

            switch (player.Team)
            {
                case Team.Blue:
                    {
                        --blueCount;
                        break;
                    }

                case Team.Red:
                    {
                        --redCount;
                        break;
                    }

                default:
                    return false;
            }

            player.Team = targetTeam;
            container[ID] = player;
            return true;
        }

        private Team GetAvailableTeam()
        {
            var team = Team.Neutral;

            if (blueCount < redCount)
                team = Team.Blue;
            else if (blueCount > redCount)
                team = Team.Red;
            else if (blueCount != teamLimit)
                team = Team.Blue;

            switch (team)
            {
                case Team.Blue:
                    ++blueCount;
                    break;

                case Team.Red:
                    ++redCount;
                    break;
            }

            return team;
        }
        #endregion

        #region client
        public override void ClientSyncList()
        {
            if (NetworkClient.isHostClient) return;
            lobbyList.ClearPlayers();

            var index = 0;
            foreach (var item in container)
            {
                OnListChanged(SyncDictionary<ulong, Player>.Operation.OP_ADD, item.Key, item.Value);
                ++index;
            }
        }

        private void OnListChanged(SyncDictionary<ulong, Player>.Operation op, ulong key, Player item)
        {
            switch (op)
            {
                case SyncDictionary<ulong, Player>.Operation.OP_ADD:
                    {
                        lobbyList.AddPlayer(item.ID, item.Name, item.Team);
                        serverInfo.UpdateCurrentPlayers(Container.Count);
                        break;
                    }

                case SyncDictionary<ulong, Player>.Operation.OP_REMOVE:
                    {
                        lobbyList.RemovePlayer(key);
                        serverInfo.UpdateCurrentPlayers(Container.Count);
                        break;
                    }

                case SyncDictionary<ulong, Player>.Operation.OP_CLEAR:
                    {
                        lobbyList.ClearPlayers();
                        serverInfo.UpdateCurrentPlayers(Container.Count);
                        break;
                    }

                case SyncDictionary<ulong, Player>.Operation.OP_SET:
                    {
                        lobbyList.UpdatePlayer(key, item.IsReady, item.Team);
                        break;
                    }
            }

            OnReadyCountUpdate(default, readyCount);
        }

        private void OnReadyCountUpdate(int _, int value)
        {
            lobbyList.UpdateReadyCount(value, container.Count);
        }
        #endregion

        public class Player
        {
            public ulong ID;
            public string Name;
            public bool IsReady;
            public Team Team;
        }

        public enum Team
        {
            Neutral,
            Blue,
            Red,
        }
    }
}
