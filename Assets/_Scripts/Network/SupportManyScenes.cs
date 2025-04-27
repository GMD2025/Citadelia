using UnityEngine;

namespace _Scripts.Network
{
    public class SupportManyScenes: MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}