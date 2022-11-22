using System;
using TheGame.UI;
using TMPro;
using UnityEngine;

namespace TheGame.Options
{
    public class PlayerProfile : MonoBehaviour
    {
        [SerializeField] private TMP_InputField input;
        [SerializeField] private DiscordProfile discord;
        [SerializeField] private SteamProfile steam;
        [SerializeField] private MenuMessage menuMessage;

        private bool isDevMode;
        private ulong fallbackId;
        private bool accountLocked;

        private ulong userId;
        private string userName;
        private string currentName;

        private const string prefsName = "PlayerName";
        private const string defaultName = "Player";

        public ulong Id
        {
            get
            {
                accountLocked = true;
                return userId;
            } 
        }
        public string Name => currentName;

        private void OnValidate()
        {
            Debug.Assert(input != null, "[PlayerProfile] Input is null");
            Debug.Assert(discord != null, "[PlayerProfile] Discord is null");
            Debug.Assert(steam != null, "[PlayerProfile] Steam is null");
            Debug.Assert(menuMessage != null, "[PlayerProfile] MenuMessage is null");
        }

        private void Awake()
        {
            if (GameStart.IsServer)
            {
                enabled = false;
                return;
            }

            var devMode = Environment.GetEnvironmentVariable("DEV_MODE");
            isDevMode = Application.isEditor || devMode != null;

            currentName = PlayerPrefs.GetString(prefsName, string.Empty);
            fallbackId = (ulong)UnityEngine.Random.Range(1, int.MaxValue);

            OnBackendUpdate();
            Invoke(nameof(WarnFallback), 1.0f);
        }

        public void OnBackendUpdate()
        {
            if (accountLocked)
                return;

            if (currentName == userName)
                currentName = null;

            if (discord.IsActive)
            {
                userId = discord.UserId;
                userName = discord.UserName;
            }
            else if (steam.IsActive)
            {
                userId = steam.UserId;
                userName = steam.UserName;
            }
            else
            {
                userId = fallbackId;
                userName = defaultName;
            }

            if (string.IsNullOrEmpty(currentName))
                currentName = userName;

            input.text = currentName;

#if DEBUG
            if (isDevMode)
                userId = fallbackId;
#endif
        }

        public void OnOptionSave()
        {
            SetNewName();
            OnBackendUpdate();

            var name = currentName;
            if (currentName == userName)
                name = string.Empty;

            PlayerPrefs.SetString(prefsName, name);
        }

        private void SetNewName()
        {
            var name = input.text;

            if (name == currentName)
                return;

            name = name.Trim();
            if (name.Length > 2)
                currentName = name;
            else
            {
                currentName = userName;
                input.text = userName;
            }
        }

        public void OnOptionRefresh()
        {
            input.text = currentName;
        }

        private void WarnFallback()
        {
            if (!Application.isEditor && userId == fallbackId)
            {
                const string message = "Game could not establish connection with any supported platform. Your random id is not persistent and may conflict with other player!";
                menuMessage.Show("Integrations", message);
                return;
            }
        }
    }
}
