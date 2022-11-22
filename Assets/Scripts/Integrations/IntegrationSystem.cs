using TheGame.UI;
using UnityEngine;

namespace TheGame.Integration
{
    public class IntegrationSystem : MonoBehaviour
    {
        [SerializeField] private DiscordIntegration discord;
        [SerializeField] private SteamIntegration steam;
        [SerializeField] private MenuMessage menuMessage;
        [SerializeField] private RichPresenceManager richPresence;

        private bool steamInitialized;
        private bool discordInitialized;

        public bool IsP2PActive => steamInitialized;

        private void OnValidate()
        {
            Debug.Assert(discord != null, "[IntegrationSystem] Discord is null");
            Debug.Assert(steam != null, "[IntegrationSystem] Steam is null");

            Debug.Assert(menuMessage != null, "[IntegrationSystem] MenuMessage is null");
            Debug.Assert(richPresence != null, "[IntegrationSystem] RichPresence is null");
        }

        private void Awake()
        {
            if (!GameStart.IsServer)
            {
                InitDiscord();
            }
        }

        private void InitDiscord()
        {
            discordInitialized = discord.Initialize();
            if (discordInitialized)
            {
                richPresence.InitDiscord();
            }
        }

        public void InitSteam()
        {
            steamInitialized = steam.Initialize();
            if (steamInitialized)
            {
                richPresence.InitSteam();
            }
            else
            {
                menuMessage.Show("Integrations", "SteamApi init failed!\n Steam featuers are not available");
            }
        }
    }
}
