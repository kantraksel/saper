using TheGame.GameModes.Saper;
using UnityEngine;
using UnityEngine.UI;

namespace TheGame.UI.Game
{
    public class IconToast : MonoBehaviour
    {
        [Header("IconToast")]
        [SerializeField] protected Image icon;
        [SerializeField] protected Sprite iconActive;
        [SerializeField] protected Sprite iconWon;
        [SerializeField] protected Sprite iconDied;
        [SerializeField] protected Sprite iconDisconnected;

        protected virtual void OnValidate()
        {
            Assert(icon != null, "Icon is null");
            Assert(iconActive != null, "IconActive is null");
            Assert(iconWon != null, "IconWon is null");
            Assert(iconDied != null, "IconDied is null");
            Assert(iconDisconnected != null, "IconDisconnected is null");
        }

        private void Assert(bool condition, string message)
        {
            Debug.Assert(condition, $"[{GetType().Name}] {message}");
        }

        protected Sprite GetIcon(PlayerState state)
        {
            switch (state)
            {
                case PlayerState.Active:
                    return iconActive;

                case PlayerState.Won:
                    return iconWon;

                case PlayerState.Died:
                    return iconDied;

                case PlayerState.Disconnected:
                    return iconDisconnected;

                default:
                    return null;
            }
        }
    }
}
