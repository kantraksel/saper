using Discord;
using Steamworks;
using UnityEngine;

namespace TheGame.Integration.RichPresence
{
    public class SteamRP : MonoBehaviour
    {
        [SerializeField] private RichPresenceManager manager;
        [SerializeField] private SteamIntegration integration;

        private void OnValidate()
        {
            Debug.Assert(manager != null, "[SteamRP] Manager is null");
            Debug.Assert(integration != null, "[SteamRP] Integration is null");
            Debug.Assert(!enabled, "This Behaviour should stay disabled - the object is enabled dynamically!", this);
        }

        private void Reset()
        {
            enabled = false;
        }

        public void Initialize()
        {
            integration.AddCallback(Callback<GameRichPresenceJoinRequested_t>.Create(OnJoinFriend));
            enabled = true;
        }

        public void OnJoin(PresenceData data)
        {
            if (!enabled)
                return;

            OnUpdate(data);
        }

        public void OnLeave()
        {
            if (!enabled)
                return;

            SteamFriends.ClearRichPresence();
        }

        private void OnJoinFriend(GameRichPresenceJoinRequested_t request)
        {
            manager.OnJoinFriend(request.m_rgchConnect);
        }

        public void OnUpdate(PresenceData data)
        {
            if (!enabled)
                return;

            SteamFriends.SetRichPresence("status", "Playing multiplayer");
            SteamFriends.SetRichPresence("connect", data.ConnectString);
            SteamFriends.SetRichPresence("steam_player_group", data.SessionId);
            SteamFriends.SetRichPresence("steam_player_group_size", data.CurrentPlayers.ToString());
        }
    }
}
