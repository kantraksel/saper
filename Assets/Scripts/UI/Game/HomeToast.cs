using TheGame.GameModes.Saper;
using UnityEngine;
using UnityEngine.UI;

namespace TheGame.UI.Game
{
    public class HomeToast : SpectatorToast
    {
        [Header("HomeToast")]
        [SerializeField] private Button button;

        protected override void OnValidate()
        {
            Debug.Assert(controller != null, "[HomeToast] Controller is null");
            Debug.Assert(button != null, "[HomeToast] Button is null");
        }

        public override void Setup(ulong id, string name)
        {
            button.interactable = false;

            Id = id;
        }

        public override void Clear() { }

        public override void ChangeActivity(bool value)
        {
            button.interactable = !value;
        }

        public override void ChangeState(PlayerState state) { }
    }
}
