using Mirror;
using TheGame.Networking.Server.States;
using UnityEngine;

namespace TheGame.Tests.EventDebug
{
    public class DMEnd : MonoBehaviour
    {
        [SerializeField] private bool end;
        [SerializeField] private Match match;

        private void OnValidate()
        {
            Debug.Assert(match != null);
        }

        private void Awake()
        {
            if (!Application.isEditor)
                Destroy(this);

            end = false;
        }
        
        private void Update()
        {
            if (end)
            {
                end = false;
                //NOTE: Host wins
                var data = NetworkServer.localConnection.authenticationData;
                match.CollapseMatch(data);
            }
        }
    }
}
