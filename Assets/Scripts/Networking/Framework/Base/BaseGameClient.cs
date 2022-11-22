using Mirror;
using System.Collections;

namespace TheGame.Networking
{
    public abstract class BaseGameClient : NetworkBehaviour
    {
        public virtual IEnumerator OnMatchLoad() => null;
        public virtual void OnMatchLoaded() { }
        /// <summary>
        /// OnMatchLoaded() will not be called after OnMatchLoad(),
        /// if loading takes too long and server changes state
        /// </summary>
        public virtual void OnMatchUnload() { }
    }
}
