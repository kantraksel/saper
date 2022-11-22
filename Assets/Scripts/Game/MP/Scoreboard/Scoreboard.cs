using Mirror;
using System.Collections.Generic;
using TheGame.Networking.Framework;
using TheGame.UI.Game;
using UnityEngine;

namespace TheGame.GameModes.Saper
{
    public class Scoreboard : NetworkBehaviour
    {
        [SerializeField] private ClassicPlayerList playerList;
        [SerializeField] private ScoreboardController controller;
        [SerializeField] private SpectatorController spectator;

        private readonly SyncDictionary<ulong, PlayerScore> players = new();
        public IReadOnlyDictionary<ulong, PlayerScore> Scores => players;

        private void OnValidate()
        {
            Debug.Assert(playerList != null, "[Scoreboard] PlayerList is null");
            Debug.Assert(controller != null, "[Scoreboard] Controller is null");
            Debug.Assert(spectator != null, "[Scoreboard] Spectator is missing!");
        }

        private void Awake()
        {
            players.Callback += OnScoreUpdated;
        }

        public void SetupClient()
        {
            ClearClient();
            foreach (var player in playerList.Container.Values)
            {
                controller.AddPlayer(player);
                spectator.AddPlayer(player);
            }

            foreach (var item in players)
            {
                OnScoreUpdated(SyncIDictionary<ulong, PlayerScore>.Operation.OP_ADD, item.Key, item.Value);
            }
        }

        public void Clear()
        {
            players.Clear();
        }

        public void UpdateScore(PlayerScore score)
        {
            players[score.id] = score;
        }

        private void OnScoreUpdated(SyncIDictionary<ulong, PlayerScore>.Operation op, ulong key, PlayerScore item)
        {
            switch (op)
            {
                case SyncIDictionary<ulong, PlayerScore>.Operation.OP_ADD:
                case SyncIDictionary<ulong, PlayerScore>.Operation.OP_SET:
                    {
                        controller.UpdateScore(item);
                        spectator.UpdateState(item.id, item.state);
                        break;
                    }
            }
        }

        public void ClearClient()
        {
            controller.Clear();
            spectator.Clear();
        }

        public void ShowAnimated() => controller.ShowAnimated();
    }
}
