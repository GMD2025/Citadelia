using UnityEngine;

namespace _Scripts.Utils
{
    [ExecuteAlways]
    public class Billboard: MonoBehaviour
    {
        private new Camera camera;

        private void Awake()
        {
            camera = Camera.main;
        }

        private void LateUpdate()
        {
            transform.LookAt(transform.position + camera.transform.forward);
        }
    }
}