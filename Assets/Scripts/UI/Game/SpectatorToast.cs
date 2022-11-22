using TheGame.GameModes.Saper;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TheGame.UI.Game
{
    public class SpectatorToast : IconToast
    {
        [Header("ScoreboardToast")]
        [SerializeField] protected SpectatorController controller;
        [SerializeField] private TMP_Text displayName;
        [SerializeField] private Image activity;
        [SerializeField] private Color colorActive;
        [SerializeField] private Color colorInactive;
        public bool IsActive => gameObject.activeSelf;
        public ulong Id { get; protected set; }

        protected override void OnValidate()
        {
            base.OnValidate();
            Debug.Assert(controller != null, "[SpectatorToast] Controller is null");
            Debug.Assert(displayName != null, "[SpectatorToast] DisplayName is null");
            Debug.Assert(activity != null, "[SpectatorToast] Activity is null");
        }

        public virtual void Setup(ulong id, string name)
        {
            gameObject.SetActive(true);

            Id = id;
            displayName.text = name;
        }

        public virtual void Clear()
        {
            displayName.text = null;
            icon.sprite = GetIcon(PlayerState.Active);
            ChangeActivity(false);

            gameObject.SetActive(false);
        }

        public virtual void ChangeActivity(bool value)
        {
            activity.color = value ? colorActive : colorInactive;
        }

        public virtual void OnToastClicked()
        {
            controller.SpectatePlayer(this);
        }

        public virtual void ChangeState(PlayerState state)
        {
            icon.sprite = GetIcon(state);
        }
    }
}
