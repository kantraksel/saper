using UnityEngine;

namespace TheGame.Networking.Framework
{
    public class DefaultLobbyClient : BaseLobbyClient
    {
        [SerializeField] private GameObject lobbyUI;
        [SerializeField] private BasePlayerList playerList;

        private void OnValidate()
        {
            Debug.Assert(lobbyUI != null, "[DefaultLobbyClient] LobbyUI is null");
            Debug.Assert(playerList != null, "[DefaultLobbyClient] PlayerList is null");
        }

        public override void OnClientEnable()
        {
            lobbyUI.SetActive(true);
        }

        public override void OnClientDisable()
        {
            lobbyUI.SetActive(false);
        }

        public override void OnClientSync()
        {
            playerList.ClientSyncList();
        }
    }
}
