using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheGame.Tests
{
    public class Coroutines : MonoBehaviour
    {
        public bool StartCoroutineU;
        public bool StopCoroutineU;
        public Coroutine cc;
        public Coroutines ccs;
        public Coroutine cc2;
        public Coroutine cc3;
        public bool cc3NextFrame;

        private void Start()
        {
            Debug.Log("Coroutine test");
        }

        private void Update()
        {
            if (cc3NextFrame)
            {
                cc3NextFrame = false;
                StopCoroutine(cc3);
                cc3 = null;
            }

            if (StartCoroutineU)
            {
                StartCoroutineU = false;
                cc = StartCoroutine(CoroutineRoot());
                cc2 = StartCoroutine(StopMyself());
                cc3 = StartCoroutine(ImmediateCheck());
                cc3NextFrame = true;
            }

            if (StopCoroutineU)
            {
                StopCoroutineU = false;
                StopCoroutine(cc);
            }

            Debug.Log("Update");
        }

        private IEnumerator CoroutineRoot()
        {
            Debug.Log("CoroutineRoot");
            yield return null;

            Debug.Log("CoroutineRoot start child");
            var c1 = CoroutineChild();
            Debug.Log("CoroutineRoot started child");
            yield return c1;
            Debug.Log("CoroutineRoot child ended");

            Debug.Log("CoroutineRoot start child v2");
            var c = CoroutineChild();
            Debug.Log("CoroutineRoot started child v2");
            yield return new WaitForSecondsRealtime(1.1f);
            Debug.Log("CoroutineRoot cooled");
            yield return c;
            Debug.Log("CoroutineRoot child ended v2");
            yield return ccs.LongCC();
        }

        private IEnumerator CoroutineChild()
        {
            Debug.Log("CoroutineChild");
            yield return new WaitForSecondsRealtime(1.0f);
            Debug.Log("CoroutineChild cooled");
        }

        private IEnumerator LongCC()
        {
            Debug.Log("LongCC");
            yield return new WaitForSecondsRealtime(7);
            Debug.Log("LongCC cooled");
        }

        private IEnumerator StopMyself()
        {
            Debug.Log("StopMyself");
            yield return null;
            Debug.Log("StopMyself 2");
            StopCoroutine(cc2);
            Debug.Log("StopMyself 3");
            yield return null;
            Debug.Log("StopMyself 4");
            yield return null;
            Debug.Log("StopMyself 5");
        }

        private IEnumerator ImmediateCheck()
        {
            //yield return null;
            Debug.Log("ImmediateCheck");
            yield return new WaitUntil(() => true);
            Debug.Log("ImmediateCheck2");
        }
    }
}
