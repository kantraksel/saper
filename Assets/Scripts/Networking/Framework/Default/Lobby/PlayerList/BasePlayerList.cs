using Mirror;
using System.Collections.Generic;

namespace TheGame.Networking.Framework
{
    public abstract class BasePlayerList : NetworkBehaviour
    {
        public bool AllowListChange { get; set; }

        public virtual IEnumerable<ulong> Ids => null;
        public virtual int PlayerCount => default;

        public abstract bool Add(ulong ID, string name);
        public abstract void Remove(ulong ID);
        public abstract void Clear();
        public abstract void UpdateReadyState(ulong ID, bool isReady);

        public abstract void ClientSyncList();
    }
}
