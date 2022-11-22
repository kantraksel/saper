using UnityEngine;
using UnityEngine.UI;

namespace TheGame.UI
{
    public class EAWarning : MonoBehaviour
    {
        [SerializeField] private Toggle toggle;
        private const string prefsKey = "AlphaWarning_DontShow";

        private void OnValidate()
        {
            Debug.Assert(toggle != null, "[EAWarning] Toggle is null");
        }

        private void Awake()
        {
            if (PlayerPrefs.GetInt(prefsKey, 0) == 1)
                gameObject.SetActive(false);
        }

        public void OnConfirm()
        {
            PlayerPrefs.SetInt(prefsKey, toggle.isOn ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
