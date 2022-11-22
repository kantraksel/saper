using System;
using TheGame.Networking;
using TheGame.Options;
using TheGame.UI.Classic;
using UnityEngine;

namespace TheGame.UI
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private NetMgr networkManager;
        [SerializeField] private JoinWindow joinWindow;
        [SerializeField] private MenuMessage menuMessage;
        [SerializeField] private GameObject pauseLayout;
        [SerializeField] private GameObject backgroundSolid;
        [SerializeField] private GameObject backgroundTransparent;

        private void OnValidate()
        {
            Debug.Assert(joinWindow != null, "[MenuController] JoinWindow is null");
            Debug.Assert(menuMessage != null, "[MenuController] MenuMessage is null");
            Debug.Assert(networkManager != null, "[MenuController] NetworkManager is null");
            Debug.Assert(pauseLayout != null, "[MenuController] PauseLayout is null");
            Debug.Assert(backgroundSolid != null, "[MenuController] BackgroundSolid is null");
            Debug.Assert(backgroundTransparent != null, "[MenuController] BackgroundTransparent is null");
        }

        public void SwitchPauseMenu()
        {
            if (!gameObject.activeSelf || pauseLayout.activeSelf)
            {
                gameObject.SetActive(!gameObject.activeSelf);
            }
        }

        public void EnterMainMenu()
        {
            pauseLayout.SetActive(false);
            backgroundTransparent.SetActive(false);
            backgroundSolid.SetActive(true);
            gameObject.SetActive(true);
        }

        public void ExitMainMenu()
        {
            gameObject.SetActive(false);
            backgroundSolid.SetActive(false);
            backgroundTransparent.SetActive(true);
            pauseLayout.SetActive(true);
        }

        public void HostGame()
        {
            if (networkManager.IsP2PActive)
            {
                StartHostGame(true);
                return;
            }

            string message = "To host a game you have to fulfill requirements for dedicated servers.";
            menuMessage.Show("Attention!", message, MenuMessage.Type.OkCancel, StartHostGame);
        }

        private void StartHostGame(bool okInvoked)
        {
            if (okInvoked)
            {
                if (!networkManager.Available())
                {
                    menuMessage.Show("Fatal Error!", "Transport is not available, network features will not work!");
                    return;
                }
                networkManager.StopAllModes();
                networkManager.StartHost();
            }
        }

        [Obsolete("Unused, see main menu layout")]
        public void StartServer() => networkManager.StartServer();

        public void LeaveGame()
        {
            networkManager.StopAllModes();
        }

        public void OpenJoinWindow() => joinWindow.Open();

        public void Exit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#endif
            Application.Quit();
        }

        public bool IsPauseMenuActive()
        {
            return pauseLayout.activeInHierarchy;
        }
    }
}
