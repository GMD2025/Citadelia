using System;
using UnityEngine;

public static class LifecycleHooks
{
    public static ref Action OnStart(GameObject go)
    {
        return ref Attach(go).OnStartCallback;
    }

    public static ref Action OnDestroy(GameObject go)
    {
        return ref Attach(go).OnDestroyCallback;
    }

    private static LifeCycleProxy Attach(GameObject go)
    {
        if (!go.TryGetComponent<LifeCycleProxy>(out var proxy))
            proxy = go.AddComponent<LifeCycleProxy>();
        return proxy;
    }

    private class LifeCycleProxy : MonoBehaviour
    {
        public Action OnStartCallback;
        public Action OnDestroyCallback;

        private void Start()
        {
            OnStartCallback?.Invoke();
        }

        private void OnDestroy()
        {
            OnDestroyCallback?.Invoke();
        }
    }
}