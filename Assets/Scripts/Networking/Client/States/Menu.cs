using TheGame.UI;
using UnityEngine;

namespace TheGame.Networking.Client.States
{
    internal class Menu : MonoBehaviour, IBaseManager
    {
        [SerializeField] private TheGame.UI.MenuController menuUI;

        private void OnValidate()
        {
            Debug.Assert(menuUI != null, "[UnknownState] MenuUI is null");
        }

        public void Setup(ClientCoordinator parent)
        {

        }

        public void Enable()
        {
            menuUI.EnterMainMenu();
        }

        public void Disable()
        {
            menuUI.ExitMainMenu();
        }
    }
}

