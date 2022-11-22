using System;
using TheGame.Integration.RichPresence;
using TheGame.Networking;
using UnityEngine;

namespace TheGame.Integration
{
    public class RichPresenceManager : MonoBehaviour
    {
        [SerializeField] private SteamRP steam;
        [SerializeField] private DiscordRP discord;
        [SerializeField] private SimpleClient client;
        [SerializeField] private ServerInfo serverInfo;
        private PresenceData data;
        private bool hasJoined;

        private void OnValidate()
        {
            Debug.Assert(steam != null, "[RichPresenceManager] Steam is null");
            Debug.Assert(discord != null, "[RichPresenceManager] Discord is null");
            Debug.Assert(client != null, "[RichPresenceManager] Client is null");
            Debug.Assert(serverInfo != null, "[RichPresenceManager] ServerInfo is null");
        }

        private void Awake()
        {
            if (GameStart.IsServer)
                enabled = false;
        }

        private void Start()
        {
            serverInfo.CurrentPlayersUpdated += OnCurrentPlayersUpdated;
        }

        public void InitSteam() => steam.Initialize();
        public void InitDiscord() => discord.Initialize();

        public void OnJoin()
        {
            var uri = client.NetMgr.GetClientConnectUri();
            var connectString = uri.ToString();

            data = new PresenceData
            {
                SessionId = serverInfo.SessionId,
                CurrentPlayers = serverInfo.CurrentPlayers,
                MaxPlayers = serverInfo.MaxPlayers,
                ConnectString = connectString,
            };

            steam.OnJoin(data);
            discord.OnJoin(data);

            hasJoined = true;
        }

        public void OnLeave()
        {
            steam.OnLeave();
            discord.OnLeave();
            hasJoined = false;
        }

        public void OnJoinFriend(string connectString)
        {
            var uri = new Uri(connectString);
            client.Connect(uri);
        }

        private void OnCurrentPlayersUpdated(int value)
        {
            if (!hasJoined || value == 0)
                return;

            data.CurrentPlayers = value;

            steam.OnUpdate(data);
            discord.OnUpdate(data);
        }
    }
}
