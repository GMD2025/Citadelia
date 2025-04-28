using UnityEngine;

namespace _Scripts.Network
{
    public class PersistScenes: MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}