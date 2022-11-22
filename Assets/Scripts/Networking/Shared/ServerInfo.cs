using Mirror;
using System;
using System.Security.Cryptography;

namespace TheGame.Networking
{
    public class ServerInfo : NetworkBehaviour
    {
        [SyncVar] private string sessionId;
        [SyncVar] private string serverName;
        [SyncVar] private int maxPlayers;
        private int currentPlayers;

        public string SessionId => sessionId;
        public string ServerName => serverName;
        public int MaxPlayers => maxPlayers;
        public int CurrentPlayers => currentPlayers;
        public Action<int> CurrentPlayersUpdated { get; set; }

        internal void SetupServer(int totalSlots)
        {
            serverName = "Minesweeper listen server";
            maxPlayers = totalSlots;
            sessionId = GenerateSessionId();
        }

        private string GenerateSessionId()
        {
            var bytes = new byte[sizeof(ulong)];
            using var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return BitConverter.ToUInt64(bytes).ToString("x8");
        }

        public void UpdateCurrentPlayers(int value)
        {
            currentPlayers = value;
            CurrentPlayersUpdated?.Invoke(value);
        }
    }
}
