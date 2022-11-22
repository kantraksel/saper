using SteamSockets;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheGame.Integration
{
    public class SteamIntegration : MonoBehaviour
    {
        [SerializeField] private SteamTransport transport;

        public Action<ulong, string> ProfileUpdateEvent;
        public bool IsActive { get; private set; }

        private ulong localSteamId;
        private readonly List<Callback> callbacks = new();

        private void OnValidate()
        {
            Debug.Assert(transport != null, "[Steam] Transport is null");
            Debug.Assert(!enabled, "This Behaviour should stay disabled - the object is enabled dynamically!", this);
        }

        private void Reset()
        {
            enabled = false;
        }

        public bool Initialize()
        {
            if (!Packsize.Test())
            {
                Debug.LogError("[Steam] The wrong version of Steamworks.NET is used on this platform");
                return false;
            }

            try
            {
                IsActive = SteamAPI.Init();
                if (!IsActive)
                {
                    Debug.LogError("[Steam] SteamAPI_Init() failed");
                    return false;
                }
            }
            catch (DllNotFoundException)
            {
                Debug.LogError("[Steam] Could not load steam_api library");
                return false;
            }

            SteamNetworkingUtils.InitRelayNetworkAccess();

            enabled = true;
            transport.enabled = true;
            StartCoroutine(NotifyUserChange());

            Debug.Log("[Steam] Init complete");
            return true;
        }

        private void OnDestroy()
        {
            if (IsActive)
                SteamAPI.Shutdown();

            callbacks.Clear();
        }

        private void LateUpdate()
        {
            SteamAPI.RunCallbacks();
        }

        private IEnumerator NotifyUserChange()
        {
            yield return null;

            var name = SteamFriends.GetPersonaName();
            localSteamId = SteamUser.GetSteamID().m_SteamID;
            ProfileUpdateEvent?.Invoke(localSteamId, name);
        }

        public void AddCallback(Callback callback)
        {
            callbacks.Add(callback);
        }
    }
}
