using Mirror;
using System.Collections.Generic;
using UnityEngine;

namespace TheGame.Networking.Framework
{
    public class ClassicPlayerList : BasePlayerList
    {
        [SerializeField] private BaseClassicLobbyList lobbyList;
        [SerializeField] private ServerInfo serverInfo;
        private readonly SyncDictionary<ulong, Player> container = new();

        public IReadOnlyDictionary<ulong, Player> Container => container;
        public override IEnumerable<ulong> Ids => Container.Keys;
        public override int PlayerCount => Container.Count;

        private void OnValidate()
        {
            Debug.Assert(lobbyList != null, "[ClassicPlayerList] LobbyListController is null");
            Debug.Assert(serverInfo != null, "[ClassicPlayerList] ServerInfo is null");
        }

        private void Awake()
        {
            container.Callback += OnListChanged;
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

            var player = new Player { ID = ID, Name = name };
            container.Add(ID, player);
            return true;
        }

        public override void Remove(ulong ID)
        {
            if (!AllowListChange) return;
            container.Remove(ID);
        }

        public override void Clear()
        {
            container.Clear();
        }

        public override void UpdateReadyState(ulong ID, bool isReady)
        {
            if (!AllowListChange || !container.TryGetValue(ID, out var player)) return;

            player.IsReady = isReady;
            container[ID] = player;
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
                        lobbyList.AddPlayer(item.ID, item.Name);
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
                        lobbyList.UpdatePlayer(key, item.IsReady);
                        break;
                    }
            }
        }
        #endregion

        public class Player
        {
            public ulong ID;
            public string Name;
            public bool IsReady;
        }
    }
}
