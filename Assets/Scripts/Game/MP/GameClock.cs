using Mirror;
using System;
using System.Collections;
using TheGame.UI.Game;
using UnityEngine;

namespace TheGame.GameModes.Saper
{
    public class GameClock : NetworkBehaviour
    {
        [SerializeField] private Clock clock;
        private readonly SyncVar<FullValue> value = new(default);
        private Coroutine task;
        private Mode mode;
        public float Value => value.Value.Value;
        private Coroutine clientCoroutine;

        private void OnValidate()
        {
            Debug.Assert(clock != null, "[GameClock] Clock is null");
        }

        #region server
        public void StartForward()
        {
            if (task != null)
                StopCoroutine(task);

            task = StartCoroutine(Ticker());
        }

        public void StartBackward(uint startTime)
        {
            if (task != null)
                StopCoroutine(task);

            task = StartCoroutine(BackwardTicker(startTime));
        }

        public void Stop()
        {
            if (task == null)
                return;

            StopCoroutine(task);
            task = null;
        }

        private IEnumerator Ticker()
        {
            mode = Mode.Forward;
            return StartClock(uint.MaxValue, (time) =>
            {
                time /= 1000;
                value.Value = new FullValue { Value = time, Mode = mode };
            });
        }

        private IEnumerator BackwardTicker(uint startTime)
        {
            mode = Mode.Backward;
            return StartClock(startTime, (time) =>
            {
                time = startTime - time;
                time /= 1000;
                value.Value = new FullValue { Value = time, Mode = mode };
            });
        }

        private IEnumerator StartClock(uint maxTime, Action<uint> callback)
        {
            uint currentTime = 0;
            do
            {
                yield return null;

                var delta = (uint)(Time.deltaTime * 1000.0f);
                currentTime += delta;

                if (currentTime >= maxTime)
                {
                    callback(maxTime);
                    break;
                }

                callback(currentTime);
            } while (true);
        }

        public IEnumerator WaitForEnd()
        {
            if (task == null)
                throw new InvalidOperationException("Clock is not running!");

            if (mode == Mode.Forward)
                throw new InvalidOperationException("Clock is in forward mode!");

            yield return task;
        }
        #endregion

        #region client
        public override void OnStartClient()
        {
            value.Callback += OnValueUpdated;
            OnValueUpdated(default, value.Value);
        }

        public override void OnStopClient()
        {
            if (clientCoroutine != null)
            {
                StopCoroutine(clientCoroutine);
                clientCoroutine = null;
            }
            value.Callback -= OnValueUpdated;
        }

        private void OnValueUpdated(FullValue oldValue, FullValue newValue)
        {
            switch (newValue.Mode)
            {
                case Mode.Forward:
                    clock.SetTime(newValue.Value / 60, newValue.Value % 60);
                    break;

                case Mode.Backward:
                    {
                        if (clientCoroutine != null)
                            StopCoroutine(clientCoroutine);
                        clientCoroutine = StartCoroutine(EmulateCentiseconds(newValue.Value));
                        break;
                    }
            }
        }

        private IEnumerator EmulateCentiseconds(uint second)
        {
            uint cs = 99;
            float deltaElapsed = 0;
            do
            {
                clock.SetTime(second, cs - (uint)deltaElapsed);
                yield return null;
                if (deltaElapsed == cs) break;

                deltaElapsed += (Time.deltaTime * 100) * 0.85f;
                if (cs < deltaElapsed) deltaElapsed = cs;

            } while (true);
            clientCoroutine = null;
        }
        #endregion

        enum Mode
        {
            Forward,
            Backward,
        }

        struct FullValue
        {
            public Mode Mode;
            public uint Value;
        }
    }
}
