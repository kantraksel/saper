using System;
using UnityEngine;

namespace TheGame
{
    public class EditorWarnings : MonoBehaviour
    {
        private void Awake()
        {
            CheckPlatformType();
            CheckDevMode();
        }

        private void CheckPlatformType()
        {
            if (!Application.isEditor)
            {
                Destroy(this);
                return;
            }

#if UNITY_SERVER
            Debug.LogWarning("[EditorWarnings] You are running player on Server platform!");
            Debug.Log("[EditorWarnings] Switch to Desktop platform if this is unintentional");
#endif
        }

        private void CheckDevMode()
        {
            var devMode = Environment.GetEnvironmentVariable("DEV_MODE");
            var isDevMode = Application.isEditor || devMode != null;

            if (isDevMode)
                Debug.Log("[EditorWarnings] Running in DEV MODE!");
        }
    }
}
