// === BlockAnchorAligner.cs ===
using UnityEngine;
using _Scripts.CustomInspector.Button;
using System.Collections.Generic;

namespace _Scripts.Systems.WallBuilder
{
    [ExecuteAlways]
    public class BlockAnchorAligner : MonoBehaviour
    {
        private Sprite lastSprite; // Cache to detect sprite changes

        private void OnEnable()
        {
            CreateMissingAnchors();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying) return;
            CreateMissingAnchors();
        }
#endif

        public void CreateMissingAnchors()
        {
            Bounds bounds;
            if (!TryGetAggregateBounds(out bounds)) return;

            Vector3 center = bounds.center;
            float halfHeight = bounds.extents.y;
            float halfWidth = bounds.extents.x;

            CreateOrUpdateAnchor("AnchorTop", new Vector3(center.x, center.y + halfHeight, 0), false);
            CreateOrUpdateAnchor("AnchorBottom", new Vector3(center.x, center.y - halfHeight, 0), false);
            CreateOrUpdateAnchor("AnchorLeft", new Vector3(center.x - halfWidth, center.y, 0), false);
            CreateOrUpdateAnchor("AnchorRight", new Vector3(center.x + halfWidth, center.y, 0), false);
        }

        [InspectorButton("Update Anchors If Sprite Changed")]
        public void AlignAnchors()
        {
            Bounds bounds;
            if (!TryGetAggregateBounds(out bounds)) return;

            Vector3 center = bounds.center;
            float halfHeight = bounds.extents.y;
            float halfWidth = bounds.extents.x;

            CreateOrUpdateAnchor("AnchorTop", new Vector3(center.x, center.y + halfHeight, 0), true);
            CreateOrUpdateAnchor("AnchorBottom", new Vector3(center.x, center.y - halfHeight, 0), true);
            CreateOrUpdateAnchor("AnchorLeft", new Vector3(center.x - halfWidth, center.y, 0), true);
            CreateOrUpdateAnchor("AnchorRight", new Vector3(center.x + halfWidth, center.y, 0), true);
        }

        private bool TryGetAggregateBounds(out Bounds result)
        {
            SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>(true);
            result = new Bounds();
            if (renderers.Length == 0) return false;

            result = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                result.Encapsulate(renderers[i].bounds);
            }
            return true;
        }

        private void CreateOrUpdateAnchor(string name, Vector3 worldPos, bool forceUpdate)
        {
            Transform anchor = transform.Find(name);
            Vector3 localPos = transform.InverseTransformPoint(worldPos);

            if (anchor == null)
            {
                var go = new GameObject(name);
                go.transform.parent = transform;
                anchor = go.transform;
                anchor.localPosition = localPos;
            }
            else if (forceUpdate)
            {
                anchor.localPosition = localPos;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (UnityEditor.Selection.activeGameObject != gameObject) return;

            DrawAnchorDot("AnchorTop", Color.red);
            DrawAnchorDot("AnchorBottom", Color.green);
            DrawAnchorDot("AnchorLeft", Color.blue);
            DrawAnchorDot("AnchorRight", Color.yellow);
        }

        private void DrawAnchorDot(string name, Color color)
        {
            Transform anchor = transform.Find(name);
            if (anchor == null) return;

            Gizmos.color = color;
            Gizmos.DrawSphere(anchor.position, 0.03f);
        }
#endif
    }
}
