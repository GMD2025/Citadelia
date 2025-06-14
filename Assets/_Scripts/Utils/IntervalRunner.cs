﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Utils
{
    public static class IntervalRunner
    {
        private class Executor
        {
            private readonly MonoBehaviour host;
            private readonly Func<float> interval;
            private readonly Action callback;
            private Coroutine routine;

            public Executor(MonoBehaviour host, Func<float> interval, Action callback)
            {
                this.host = host;
                this.interval = interval;
                this.callback = callback;
            }

            public void Start()
            {
                routine ??= host.StartCoroutine(Run());
            }

            public void Stop()
            {
                if (routine != null)
                {
                    host.StopCoroutine(routine);
                    routine = null;
                }
            }

            public IEnumerator RunOnce()
            {
                yield return new WaitForSeconds(interval());
                callback?.Invoke();
            }

            private IEnumerator Run()
            {
                while (true)
                {
                    yield return RunOnce();
                }
            }
        }

        private static readonly Dictionary<(MonoBehaviour, Action), Executor> executors = new();

        public static void Start(MonoBehaviour host, Func<float> intervalGetter, Action callback)
        {
            if (!Application.isPlaying || host == null || callback == null)
                return;

            var key = (host, callback);
            if (executors.ContainsKey(key))
                return;

            var executor = new Executor(host, intervalGetter, callback);
            executors[key] = executor;
            executor.Start();
        }

        public static void Stop(MonoBehaviour host, Action callback)
        {
            var key = (host, callback);
            if (executors.TryGetValue(key, out var executor))
            {
                executor.Stop();
                executors.Remove(key);
            }
        }

        public static void RunOnce(MonoBehaviour host, Func<float> intervalGetter, Action callback)
        {
            if (!Application.isPlaying || host == null || !host || callback == null)
                return;

            try
            {
                host.StartCoroutine(RunOnceCoroutine(host, intervalGetter, callback));
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to start interval coroutine for {host?.GetType().Name}.{callback?.Method.Name}: {ex.Message}");
            }
        }

        private static IEnumerator RunOnceCoroutine(MonoBehaviour host, Func<float> intervalGetter, Action callback)
        {
            float delay;
            try
            {
                delay = intervalGetter?.Invoke() ?? 0f;
            }
            catch (Exception e)
            {
                Debug.LogError($"[{host?.GetType().Name}.{intervalGetter?.Method.Name}] threw during interval setup: {e.Message}");
                yield break;
            }

            yield return new WaitForSeconds(delay);

            if (!host)
                yield break;

            try
            {
                callback?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError($"[{host.GetType().Name}.{callback.Method.Name}] threw during delayed execution: {e.Message}");
            }
        }

        public static void StopAll(MonoBehaviour host)
        {
            var keysToRemove = new List<(MonoBehaviour, Action)>();

            foreach (var kvp in executors)
            {
                if (kvp.Key.Item1 == host)
                {
                    kvp.Value.Stop();
                    keysToRemove.Add(kvp.Key);
                }
            }

            foreach (var key in keysToRemove)
                executors.Remove(key);
        }
    }
}
