using System.Collections.Generic;
using System.Linq;
using _Scripts.CustomInspector.Button;
using UnityEngine;


namespace _Scripts.Systems.WallBuilder
{
    public enum AnchorSite { Top, Bottom, Left, Right, TopLeft, TopRight, BottomLeft, BottomRight }
    public enum SortingBehavior { LessThan, GreaterThan, NoChange }

    [System.Serializable]
    public class AnchorConfig
    {
        public AnchorSite site;
        public GameObject blockPrefab;
        public SortingBehavior sortingBehavior = SortingBehavior.NoChange;
    }

    public class StackPlacer : MonoBehaviour
    {
        [SerializeField] private List<AnchorConfig> anchorConfigs = new();
        [SerializeField] private SpriteRenderer spriteRenderer;

        private const string STACKED_SUFFIX = "_Stacked";
        private const string COMPOSITE_ROOT_NAME = "RootBlock";

        private void OnValidate()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
        }

        [InspectorButton("Stack Selected Sides")]
        public void StackAll()
        {
            foreach (var cfg in anchorConfigs)
                if (cfg.blockPrefab != null)
                    StackSide(cfg);
        }

        private void StackSide(AnchorConfig cfg)
        {
            var site = cfg.site;
            Vector3 targetLocal = ComputeAnchorLocal(site, transform);
            if (targetLocal == Vector3.positiveInfinity) return;

            string instanceName = site + STACKED_SUFFIX;
            var existing = transform.Find(instanceName);
            if (existing) DestroyImmediate(existing.gameObject);

            var stackedObj = Instantiate(cfg.blockPrefab, transform, false);
            stackedObj.name = instanceName;

            var root = stackedObj.transform.Find(COMPOSITE_ROOT_NAME);
            if (root == null)
                root = stackedObj.transform;

            Vector3 selfLocal = ComputeAnchorLocal(GetOpposite(site), root);
            if (selfLocal == Vector3.positiveInfinity) return;

            Vector3 rootLocal = root.localPosition;
            Vector3 finalLocal = targetLocal + (rootLocal - selfLocal);
            stackedObj.transform.localPosition = finalLocal;

            var stackedPlacer = stackedObj.GetComponent<StackPlacer>();
            if (stackedPlacer != null && spriteRenderer != null)
            {
                var sr = stackedPlacer.GetComponent<SpriteRenderer>();
                if (sr != null)
                    sr.sortingOrder = cfg.sortingBehavior switch
                    {
                        SortingBehavior.GreaterThan => spriteRenderer.sortingOrder + 1,
                        SortingBehavior.LessThan    => spriteRenderer.sortingOrder - 1,
                        _                           => sr.sortingOrder
                    };
            }
        }

        private Vector3 ComputeAnchorLocal(AnchorSite site, Transform context)
        {
            var transforms = context.GetComponentsInChildren<Transform>(true);

            Transform top = transforms.FirstOrDefault(t => t.name == "AnchorTop");
            Transform bottom = transforms.FirstOrDefault(t => t.name == "AnchorBottom");
            Transform left = transforms.FirstOrDefault(t => t.name == "AnchorLeft");
            Transform right = transforms.FirstOrDefault(t => t.name == "AnchorRight");

            Vector3 local(Vector3 world) => context.InverseTransformPoint(world);

            return site switch
            {
                AnchorSite.Top         => top     != null ? local(top.position)    : Vector3.positiveInfinity,
                AnchorSite.Bottom      => bottom  != null ? local(bottom.position) : Vector3.positiveInfinity,
                AnchorSite.Left        => left    != null ? local(left.position)   : Vector3.positiveInfinity,
                AnchorSite.Right       => right   != null ? local(right.position)  : Vector3.positiveInfinity,
                AnchorSite.TopLeft     => top != null && left != null    ? local(new Vector3(left.position.x, top.position.y, 0f))    : Vector3.positiveInfinity,
                AnchorSite.TopRight    => top != null && right != null   ? local(new Vector3(right.position.x, top.position.y, 0f))   : Vector3.positiveInfinity,
                AnchorSite.BottomLeft  => bottom != null && left != null ? local(new Vector3(left.position.x, bottom.position.y, 0f)) : Vector3.positiveInfinity,
                AnchorSite.BottomRight => bottom != null && right != null? local(new Vector3(right.position.x, bottom.position.y, 0f)): Vector3.positiveInfinity,
                _ => Vector3.positiveInfinity
            };
        }

        private AnchorSite GetOpposite(AnchorSite site) => site switch
        {
            AnchorSite.Top         => AnchorSite.Bottom,
            AnchorSite.Bottom      => AnchorSite.Top,
            AnchorSite.Left        => AnchorSite.Right,
            AnchorSite.Right       => AnchorSite.Left,
            AnchorSite.TopLeft     => AnchorSite.TopRight,
            AnchorSite.TopRight    => AnchorSite.TopLeft,
            AnchorSite.BottomLeft  => AnchorSite.BottomRight,
            AnchorSite.BottomRight => AnchorSite.BottomLeft,
            _                      => AnchorSite.Top
        };

    }
}
