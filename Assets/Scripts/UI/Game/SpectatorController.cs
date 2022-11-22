using System.Collections.Generic;
using TheGame.GameModes.Saper;
using TheGame.Networking.Framework;
using TheGame.Options;
using UnityEngine;
using UnityEngine.UI;

namespace TheGame.UI.Game
{
    public class SpectatorController : MonoBehaviour
    {
        [SerializeField] private List<SpectatorToast> toasts;
        [SerializeField] private SpectatorToast localToast;
        [SerializeField] private Button activator;
        [SerializeField] private VisualMapController mapController;
        [SerializeField] private ClassicPlayerList playerList;
        [SerializeField] private PlayerProfile profile;
        private SpectatorToast activeToast;

        private void OnValidate()
        {
            Debug.Assert(localToast != null, "[SpectatorController] LocalToast is null");
            Debug.Assert(activator != null, "[SpectatorController] Activator is null");
            Debug.Assert(mapController != null, "[SpectatorController] MapController is null");
            Debug.Assert(playerList != null, "[SpectatorController] PlayerList is null");
            Debug.Assert(profile != null, "[SpectatorController] Profile is null");
        }

        private void Awake()
        {
            toasts.Add(localToast);
        }

        public void EnterSpectatorMode()
        {
            //NOTE: unused, should be implemented as soon as "no lurking" option is restored
            //currently spectator is always enabled
        }

        public void Clear()
        {
            gameObject.SetActive(false);
            activator.interactable = false;

            foreach (var toast in toasts)
            {
                toast.Clear();
            }

            activeToast = null;
        }

        public void ShowSwitch() => gameObject.SetActive(!gameObject.activeSelf);

        public void AddPlayer(ClassicPlayerList.Player player)
        {
            if (player.ID == profile.Id)
            {
                localToast.Setup(player.ID, player.Name);
                ChangeActiveToast(localToast);
                return;
            }

            foreach (var toast in toasts)
            {
                if (!toast.IsActive)
                {
                    toast.Setup(player.ID, player.Name);
                    activator.interactable = true;
                    return;
                }
            }

            Debug.LogWarning("[SpectatorController] There are no free toasts available!");
        }

        public void UpdateState(ulong id, PlayerState state)
        {
            foreach (var toast in toasts)
            {
                if (toast.Id == id)
                {
                    toast.ChangeState(state);
                    return;
                }
            }

            Debug.LogWarning("[SpectatorController] No matching toast found!");
        }

        public void SpectatePlayer(SpectatorToast toast)
        {
            var player = playerList.Container[toast.Id];

            ChangeActiveToast(toast);
            mapController.SelectActiveMap(player.ID);
        }

        private void ChangeActiveToast(SpectatorToast toast)
        {
            activeToast?.ChangeActivity(false);
            activeToast = toast;
            activeToast.ChangeActivity(true);
        }
    }
}
