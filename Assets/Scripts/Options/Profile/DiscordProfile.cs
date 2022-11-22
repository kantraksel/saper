using TheGame.Integration;
using UnityEngine;

namespace TheGame.Options
{
    public class DiscordProfile : MonoBehaviour
    {
        [SerializeField] private DiscordIntegration integration;
        [SerializeField] private PlayerProfile profile;

        public bool IsActive => UserId != 0;
        public ulong UserId { get; private set; }
        public string UserName { get; private set; }

        private void OnValidate()
        {
            Debug.Assert(integration != null, "[Discord] Integration is null");
            Debug.Assert(profile != null, "[Discord] Profile is null");
        }

        private void Start()
        {
            integration.ProfileUpdateEvent = OnProfileUpdate;
        }

        private void OnDestroy()
        {
            integration.ProfileUpdateEvent = null;
        }

        private void OnProfileUpdate(long id, string name)
        {
            UserId = (ulong)id;
            UserName = name;

            profile.OnBackendUpdate();
        }
    }
}
