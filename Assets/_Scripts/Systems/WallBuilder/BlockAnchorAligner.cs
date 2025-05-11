using UnityEngine;
using UnityEditor;
using _Scripts.CustomInspector.Button;
using System.Linq;

namespace _Scripts.Systems.WallBuilder
{
    [ExecuteAlways]
    public class BlockAnchorAligner : MonoBehaviour
    {
        private const string COMPOSITE_ROOT_NAME = "RootBlock";
        private Sprite lastSprite;
        private const float EPSILON = 0.0001f;

        private void OnEnable()
        {
            lastSprite = GetComponentInChildren<SpriteRenderer>()?.sprite;
            CreateMissingAnchors();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying) return;
            var current = GetComponentInChildren<SpriteRenderer>()?.sprite;
            if (current != lastSprite)
            {
                UpdateExistingAnchors();
                lastSprite = current;
            }
            CreateMissingAnchors();
        }
#endif

        [InspectorButton("Create Missing Anchors")]
        public void CreateMissingAnchors()
        {
            var parent = transform.Find(COMPOSITE_ROOT_NAME) ?? transform;
            if (!TryGetAggregateBounds(out var bounds)) return;
            foreach (var kv in GetDefaultPositions(bounds))
            {
                if (parent.Find(kv.Key) == null)
                    CreateAnchor(parent, kv.Key, kv.Value);
            }
        }

        [InspectorButton("Update Existing Anchors")]
        public void UpdateExistingAnchors()
        {
            var parent = transform.Find(COMPOSITE_ROOT_NAME) ?? transform;
            if (!TryGetAggregateBounds(out var bounds)) return;
            foreach (var kv in GetDefaultPositions(bounds))
            {
                var t = parent.Find(kv.Key);
                if (t == null) continue;
                Vector3 defaultLocal = parent.InverseTransformPoint(kv.Value);
                if ((t.localPosition - defaultLocal).sqrMagnitude < EPSILON)
                {
                    t.localPosition = defaultLocal;
                }
            }
        }

        private System.Collections.Generic.Dictionary<string, Vector3> GetDefaultPositions(Bounds bounds)
        {
            Vector3 c = bounds.center;
            float h = bounds.extents.y;
            float w = bounds.extents.x;
            return new System.Collections.Generic.Dictionary<string, Vector3>
            {
                {"AnchorTop",    new Vector3(c.x,    c.y + h, 0f)},
                {"AnchorBottom", new Vector3(c.x,    c.y - h, 0f)},
                {"AnchorLeft",   new Vector3(c.x - w, c.y,     0f)},
                {"AnchorRight",  new Vector3(c.x + w, c.y,     0f)}
            };
        }

        private void CreateAnchor(Transform parent, string name, Vector3 worldPos)
        {
            var go = new GameObject(name) { hideFlags = HideFlags.NotEditable };
            go.transform.SetParent(parent, worldPositionStays: false);
            go.transform.localPosition = parent.InverseTransformPoint(worldPos);
        }

        private bool TryGetAggregateBounds(out Bounds result)
        {
            var rs = GetComponentsInChildren<SpriteRenderer>(true);
            result = new Bounds();
            if (rs.Length == 0) return false;
            result = rs[0].bounds;
            foreach (var r in rs.Skip(1)) result.Encapsulate(r.bounds);
            return true;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var parent = transform.Find(COMPOSITE_ROOT_NAME) ?? transform;
            if (Selection.activeGameObject == gameObject)
                DrawAllAnchors(parent);
            else if (Selection.activeGameObject?.transform.IsChildOf(parent) == true)
            {
                var t = Selection.activeGameObject.transform;
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(t.position, 0.03f);
            }
        }

        private void DrawAllAnchors(Transform parent)
        {
            void D(Vector3 pos)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(pos, 0.03f);
            }
            foreach (var name in new[] {"AnchorTop", "AnchorBottom", "AnchorLeft", "AnchorRight"})
            {
                var t = parent.Find(name);
                if (t != null) D(t.position);
            }
            var top = parent.Find("AnchorTop");
            var bottom = parent.Find("AnchorBottom");
            var left = parent.Find("AnchorLeft");
            var right = parent.Find("AnchorRight");
            if (top != null && left  != null) D(new Vector3(left.position.x,  top.position.y,    0f));
            if (top != null && right != null) D(new Vector3(right.position.x, top.position.y,    0f));
            if (bottom!= null && left  != null) D(new Vector3(left.position.x,  bottom.position.y, 0f));
            if (bottom!= null && right != null) D(new Vector3(right.position.x, bottom.position.y, 0f));
        }
#endif
    }
}
