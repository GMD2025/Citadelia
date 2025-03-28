using System;
using System.Collections;
using UnityEngine;

namespace _Scripts.Utils
{
    public class IntervalExecutor
    {
        private readonly MonoBehaviour coroutineHost;
        private readonly float interval;
        private readonly Action executeAction;

        private Coroutine coroutine;

        public IntervalExecutor(MonoBehaviour coroutineHost, float interval, Action executeAction)
        {
            this.coroutineHost = coroutineHost;
            this.interval = interval;
            this.executeAction = executeAction;
        }

        public void Start()
        {
            coroutine ??= coroutineHost.StartCoroutine(Run());
        }

        public void Stop()
        {
            if (coroutine == null) return;
            
            coroutineHost.StopCoroutine(coroutine);
            coroutine = null;
        }

        private IEnumerator Run()
        {
            while (true)
            {
                yield return new WaitForSeconds(interval);
                executeAction?.Invoke();
            }
        }
    }
}