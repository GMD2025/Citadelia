using UnityEngine;

namespace _Scripts.Utils
{
    public class PersistScenes: MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}