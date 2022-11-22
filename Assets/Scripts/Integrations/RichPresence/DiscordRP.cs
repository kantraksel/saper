using TheGame.Networking;
using UnityEngine;

namespace TheGame.Integration.RichPresence
{
    public class DiscordRP : MonoBehaviour
    {
        [SerializeField] private RichPresenceManager manager;
        [SerializeField] private DiscordIntegration integration;
        private Discord.ActivityManager activityManager;
        private Discord.Activity activity;

        private void OnValidate()
        {
            Debug.Assert(manager != null, "[DiscordRP] Manager is null");
            Debug.Assert(integration != null, "[DiscordRP] Integration is null");
            Debug.Assert(!enabled, "This Behaviour should stay disabled - the object is enabled dynamically!", this);
        }

        private void Reset()
        {
            enabled = false;
        }

        public void Initialize()
        {
            activityManager = integration.GetActivityManager();
            activityManager.OnActivityJoin += OnJoinFriend;
            enabled = true;
        }

        public void OnJoin(PresenceData data)
        {
            if (!enabled)
                return;

            activity.Type = Discord.ActivityType.Playing;
            activity.State = "Playing multiplayer";

            OnUpdate(data);
        }

        public void OnLeave()
        {
            if (!enabled)
                return;

            activityManager.ClearActivity(_ => {});
        }

        private void OnJoinFriend(string secret)
        {
            manager.OnJoinFriend(secret);
        }

        public void OnUpdate(PresenceData data)
        {
            if (!enabled)
                return;

            activity.Party.Id = data.SessionId;
            activity.Party.Size.CurrentSize = data.CurrentPlayers;
            activity.Party.Size.MaxSize = data.MaxPlayers;
            activity.Secrets.Join = data.ConnectString;

            UpdateActivity();
        }

        private void UpdateActivity()
        {
            activityManager.UpdateActivity(activity, (result) =>
            {
                if (result != Discord.Result.Ok)
                    Debug.LogWarning("[DiscordRP] Failed to set activity");
            });
        }
    }
}
