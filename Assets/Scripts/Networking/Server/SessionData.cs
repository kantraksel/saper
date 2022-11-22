using UnityEngine;

namespace TheGame.Networking.Server
{
    public class SessionData
    {
        public ulong Id;
        public string Name;
        
        //NOTE: internal management
        public bool AllowDestroy;
        public Coroutine LoadingTimeoutCoroutine;

        //NOTE: external usage
        public bool IsPlayer;
        public GameObject Entity;
        public GameObject PawnEntity;
    }
}
