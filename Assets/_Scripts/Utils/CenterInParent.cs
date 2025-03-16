using _Scripts.CustomInspector.Button;
using UnityEngine;

namespace _Scripts.Utils
{
    public class CenterInParent : MonoBehaviour
    {
        private Renderer objectRenderer;

        void Awake()
        {
            Center();
        }

        [InspectorButton("Center Object")]
        void Center()
        {
            objectRenderer = GetComponent<Renderer>();

            if (objectRenderer == null)
            {
                Debug.LogWarning("Renderer not found on " + gameObject.name);
                return;
            }

            Bounds bounds = objectRenderer.bounds;
            Transform parent = transform.parent;

            if (parent != null)
            {
                transform.position = parent.position - bounds.extents;
            }
            else
            {
                transform.position = -bounds.extents;
            }
        }
    }
}