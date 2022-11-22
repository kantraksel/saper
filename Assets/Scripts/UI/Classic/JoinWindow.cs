using System.Text;
using TheGame.Networking;
using TMPro;
using UnityEngine;

namespace TheGame.UI.Classic
{
    public class JoinWindow : Window
    {
        [SerializeField] private TMP_InputField ipBox;
        [SerializeField] private SimpleClient client;
        private string connectScheme;

        protected override void OnValidate()
        {
            base.OnValidate();

            Debug.Assert(ipBox != null, "[JoinWindow] IpBox is null");
            Debug.Assert(client != null, "[JoinWindow] Client is null");
        }

        protected override void Start()
        {
            base.Start();

            ipBox.onSubmit.AddListener((arg) => Join());

#if DEBUG
            ipBox.characterLimit = 17;
            Debug.Log($"[JoinWindow] Extended input limit to {ipBox.characterLimit} - debug mode detected!");
#endif
        }

        public void Join()
        {
            Close();

            if (string.IsNullOrEmpty(connectScheme))
                connectScheme = "kcp";

            var address = string.IsNullOrWhiteSpace(ipBox.text) ? "127.0.0.1" : ipBox.text;
            client.Connect(address, connectScheme);
        }

#if DEBUG
        private readonly StringBuilder builder = new();

        private void Update()
        {
            if (Input.anyKeyDown)
            {
                for (var i = KeyCode.A; i <= KeyCode.Z; ++i)
                {
                    if (Input.GetKeyDown(i))
                        builder.Append((char)i);
                }

                int exceedingChars = builder.Length - 20;
                if (exceedingChars > 0)
                    builder.Remove(0, exceedingChars);

                string code = builder.ToString();
                if (code.Contains("steam"))
                {
                    connectScheme = "steam";
                    Debug.Log("[JoinWindow] Switched transport to Steam");
                    builder.Clear();
                }
                else if (code.Contains("kcp"))
                {
                    connectScheme = "kcp";
                    Debug.Log("[JoinWindow] Switched transport to Kcp");
                    builder.Clear();
                }
            }
        }
#endif
    }
}
