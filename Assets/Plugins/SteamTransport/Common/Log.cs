using System;
using UnityEngine;

namespace SteamSockets
{
    public static class Log
    {
        public static Action<string> Info = Debug.Log;
        public static Action<string> Warning = Debug.LogWarning;
        public static Action<string> Error = Debug.LogError;
    }
}
