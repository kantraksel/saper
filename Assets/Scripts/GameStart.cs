using TheGame.Networking;
using UnityEngine;

namespace TheGame
{
    public class GameStart : MonoBehaviour
    {
        [SerializeField] private NetMgr netMgr;

#if UNITY_SERVER
        public static bool IsServer => true;
#else
        public static bool IsServer => false;
#endif

        private void OnValidate()
        {
            Debug.Assert(netMgr != null, "[GameStart] NetMgr is null");
        }

        private void Awake()
        {
            enabled = !IsServer;
        }

        private void Start()
        {
            netMgr.StartServer();
        }
    }
}
