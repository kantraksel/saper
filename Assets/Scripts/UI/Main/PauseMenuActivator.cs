using UnityEngine;

namespace TheGame.UI
{
    public class PauseMenuActivator : MonoBehaviour
    {
        [SerializeField] private MenuController menu;
        [SerializeField] private GameObject loading;

        private void OnValidate()
        {
            Debug.Assert(menu != null, "[PauseMenuActivator] Menu is null");
            Debug.Assert(loading != null, "[PauseMenuActivator] Loading is null");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                if (!loading.activeInHierarchy)
                    menu.SwitchPauseMenu();
        }
    }
}
