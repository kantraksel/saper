using UnityEngine;

namespace TheGame.UI.Classic
{
    public class Window : MonoBehaviour
    {
        protected virtual void OnValidate() { }

        protected virtual void Start() { }

        public bool IsOpened => gameObject.activeSelf;

        public virtual void Open()
        {
            gameObject.SetActive(true);
        }

        public virtual void Close()
        {
            gameObject.SetActive(false);
        }
    }
}
