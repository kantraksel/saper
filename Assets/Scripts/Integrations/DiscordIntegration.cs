using System;
using System.Collections;
using UnityEngine;

namespace TheGame.Integration
{
    public class DiscordIntegration : MonoBehaviour
    {
        private Discord.Discord discord;
        public Action<long, string> ProfileUpdateEvent;
        [SerializeField] private GlobalConfig config;

        private void OnValidate()
        {
            Debug.Assert(!enabled, "This Behaviour should stay disabled - the object is enabled dynamically!", this);
            Debug.Assert(config != null, "[DiscordIntegration] Config is null");
        }

        private void Reset()
        {
            enabled = false;
        }

        public bool Initialize()
        {
            try
            {
                discord = new Discord.Discord(config.DiscordClientId, (ulong)Discord.CreateFlags.NoRequireDiscord);

                var userManager = discord.GetUserManager();
                userManager.OnCurrentUserUpdate += () => StartCoroutine(NotifyProfileUpdate());
            }
            catch (Discord.ResultException e)
            {
                Debug.LogWarning($"[Discord] Failed to init: {e.Result}");
                return false;
            }

            enabled = true;
            Debug.Log("[Discord] Init complete");
            return true;
        }

        private void OnDestroy()
        {
            discord?.Dispose();
            discord = null;
        }

        private void LateUpdate()
        {
            try
            {
                discord.RunCallbacks();
            }
            catch (Discord.ResultException e)
            {
                enabled = false;
                Debug.LogException(e);
                Debug.LogError("[Discord] Integration has been disabled");
            }
        }

        private IEnumerator NotifyProfileUpdate()
        {
            yield return null;

            var currentUser = discord.GetUserManager().GetCurrentUser();
            ProfileUpdateEvent?.Invoke(currentUser.Id, currentUser.Username);
        }

        public Discord.ActivityManager GetActivityManager()
        {
            return discord?.GetActivityManager();
        }
    }
}
