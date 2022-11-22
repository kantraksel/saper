using Mirror;
using UnityEngine;

namespace TheGame.GameModes.Saper
{
    public class Player : NetworkBehaviour
    {
        private MapSync sync;

        public Map Map { get; private set; }

        private void Awake()
        {
            sync = MapSync.Instance;
            Map = new();
            Map.IsLocked = true;
        }

        public override void OnStartLocalPlayer()
        {
            sync.LocalPlayer = this;
        }

        [Command]
        public void CmdInteract(Vector2Int position, bool doReveal)
        {
            if (Map.IsLocked)
                return;

            var x = position.x;
            var y = position.y;

            if (x < 0 || y < 0)
                return;
            else if (Map.Size.x <= x || Map.Size.y <= y)
                return;

            var tile = Map.ServerMap[x, y];
            sync.Interact(connectionToClient, Map, tile, doReveal);
        }
    }
}
