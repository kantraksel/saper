using TheGame.UI;
using TheGame.UI.Classic;
using UnityEngine;
using UnityEngine.Events;

namespace TheGame.Options
{
    public class Settings : MonoBehaviour
    {
        [SerializeField] private OptionsWindow optionsWindow;
        [SerializeField] private MenuMessage menuMessage;

        private bool isRestartNeeded;
        private uint delaySaveCounter;

        public UnityEvent OnOptionsPreSave = new();
        public UnityEvent OnOptionsSave = new();
        public UnityEvent OnOptionsRefresh = new();

        private void OnValidate()
        {
            Debug.Assert(optionsWindow != null, "[Settings] OptionsWindow is null");
            Debug.Assert(menuMessage != null, "[Settings] MessageWindow is null");
        }

        public void Save()
        {
            OnOptionsPreSave.Invoke();

            if (delaySaveCounter > 0)
            {
                --delaySaveCounter;
                return;
            }

            OnOptionsSave.Invoke();
            PlayerPrefs.Save();

            if (isRestartNeeded)
            {
                menuMessage.Show("Options", "You need to restart the game to apply changes");
                isRestartNeeded = false;
            }

            optionsWindow.CloseForce();
        }

        public void Refresh()
        {
            OnOptionsRefresh.Invoke();
        }

        public void RequestRestartNeeded()
        {
            isRestartNeeded = true;
        }

        public void DelaySave()
        {
            ++delaySaveCounter;
        }
    }
}
