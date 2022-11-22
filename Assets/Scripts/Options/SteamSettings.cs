using TheGame.Integration;
using TheGame.UI;
using UnityEngine;
using UnityEngine.UI;

namespace TheGame.Options
{
    public class SteamSettings : MonoBehaviour
    {
        [SerializeField] private Toggle toggle;
        [SerializeField] private Settings settings;
        [SerializeField] private MenuMessage menuMessage;
        [SerializeField] private IntegrationSystem integration;
        private bool isSteamEnabled;

        private const string prefsKey = "SteamEnabled";

        private void OnValidate()
        {
            Debug.Assert(toggle != null, "[Steam] Toggle is null");
            Debug.Assert(settings != null, "[Steam] Settings is null");
            Debug.Assert(menuMessage != null, "[Steam] MenuMessage is null");
            Debug.Assert(integration != null, "[Steam] Integration is null");
        }

        private void Awake()
        {
            if (GameStart.IsServer)
                enabled = false;
        }

        private void Start()
        {
            int isEnabled = PlayerPrefs.GetInt(prefsKey);
            isSteamEnabled = isEnabled == 1;
            toggle.isOn = isSteamEnabled;

            if (isSteamEnabled)
                integration.InitSteam();
        }

        public void OnOptionPreSave()
        {
            if (toggle.isOn)
            {
                if (isSteamEnabled) return;
                settings.DelaySave();

                const string message = "Steam is not officially supported. You enable Steam integration on your own responsibility!";
                menuMessage.Show("Steam", message, MenuMessage.Type.OkCancel, (hasAccepted) =>
                {
                    if (hasAccepted)
                    {
                        isSteamEnabled = true;
                        settings.RequestRestartNeeded();
                    }
                    else
                    {
                        toggle.isOn = false;
                    }
                    settings.Save();
                });
            }
            else if (isSteamEnabled)
            {
                isSteamEnabled = false;
                settings.RequestRestartNeeded();
            }
        }

        public void OnOptionSave()
        {
            PlayerPrefs.SetInt(prefsKey, isSteamEnabled ? 1 : 0);
        }

        public void OnOptionRefresh()
        {
            toggle.isOn = isSteamEnabled;
        }
    }
}
