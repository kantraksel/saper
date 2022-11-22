using UnityEngine;

namespace TheGame.Networking.Client.States
{
    internal class LoadScreen : MonoBehaviour
    {
        [SerializeField] private GameObject loadingUI;

        private void OnValidate()
        {
            Debug.Assert(loadingUI != null, "[LoadScreen] LoadingUI is null");
        }

        public void Enable()
        {
            loadingUI.SetActive(true);
        }

        public void Disable()
        {
            loadingUI.SetActive(false);
        }
    }
}
