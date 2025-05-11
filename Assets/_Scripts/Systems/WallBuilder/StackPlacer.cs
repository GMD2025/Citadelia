// === StackPlacer.cs ===
using UnityEngine;
using _Scripts.CustomInspector.Button;
using System.Collections.Generic;
using System.Text;

namespace _Scripts.Utils
{
    public enum AnchorSite { Top, Bottom, Left, Right }

    public enum SortingBehavior { LessThan, GreaterThan, NoChange }

    [System.Serializable]
    public class AnchorConfig
    {
        public GameObject blockPrefab;
        public SortingBehavior sortingBehavior = SortingBehavior.NoChange;
    }

    public class StackPlacer : MonoBehaviour
    {
        [SerializeField] private AnchorConfig topConfig = new AnchorConfig { sortingBehavior = SortingBehavior.GreaterThan };
        [SerializeField] private AnchorConfig bottomConfig = new AnchorConfig { sortingBehavior = SortingBehavior.LessThan };
        [SerializeField] private AnchorConfig leftConfig = new AnchorConfig();
        [SerializeField] private AnchorConfig rightConfig = new AnchorConfig();

        [SerializeField] private SpriteRenderer spriteRenderer;

        private const string STACKED_IDENTIFIER = "_Stacked";
        private const string COMPOSITE_ROOT_NAME = "RootBlock";

        private void OnValidate()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
        }

        [InspectorButton("Stack All Sides")]
        public void StackAll()
        {
            StackAndRecurse(AnchorSite.Top);
            StackAndRecurse(AnchorSite.Bottom);
            StackAndRecurse(AnchorSite.Left);
            StackAndRecurse(AnchorSite.Right);
        }

        [InspectorButton("To String Debug")]
        public void LogStructure()
        {
            Debug.Log(GetStructureString(0));
        }

        private void StackAndRecurse(AnchorSite side)
        {
            StackSide(side);
            string stackedName = $"{side}{STACKED_IDENTIFIER}";
            Transform stacked = transform.Find(stackedName);
            if (stacked != null && stacked.GetComponentInChildren<StackPlacer>() is StackPlacer placer)
            {
                placer.StackAll();
            }
        }

        private void StackSide(AnchorSite side)
        {
            AnchorConfig config = GetConfigForSide(side);
            string stackedName = $"{side}{STACKED_IDENTIFIER}";
            Transform existingStacked = transform.Find(stackedName);

            if (config.blockPrefab == null && existingStacked != null)
            {
                DestroyImmediate(existingStacked.gameObject);
                return;
            }

            GameObject blockToStack = config?.blockPrefab;
            if (blockToStack == null) return;

            string targetAnchorName = $"Anchor{side}";
            string selfAnchorName = $"Anchor{GetOpposite(side)}";

            Transform targetAnchor = transform.Find(targetAnchorName);
            if (targetAnchor == null)
            {
                Debug.LogError($"Missing target anchor: {targetAnchorName}");
                return;
            }

            GameObject stackedObject = null;

            if (existingStacked != null)
            {
                bool hasChildStackPlacers = existingStacked.GetComponentsInChildren<StackPlacer>(true).Length > 1;

                if ((existingStacked.gameObject.name != stackedName ||
                    PrefabNameDiffers(existingStacked.gameObject, blockToStack)) && !hasChildStackPlacers)
                {
                    DestroyImmediate(existingStacked.gameObject);
                    stackedObject = InstantiateStackedBlock(blockToStack, stackedName);
                }
                else
                {
                    stackedObject = existingStacked.gameObject;
                }
            }
            else
            {
                stackedObject = InstantiateStackedBlock(blockToStack, stackedName);
            }

            if (stackedObject == null) return;

            Transform targetBlockRoot = stackedObject.transform.Find(COMPOSITE_ROOT_NAME);
            if (targetBlockRoot == null)
            {
                Debug.LogError($"Composite prefab missing {COMPOSITE_ROOT_NAME} inside {stackedObject.name}");
                return;
            }

            Transform stackedAnchor = targetBlockRoot.Find(selfAnchorName);
            if (stackedAnchor == null)
            {
                Debug.LogError($"Missing self anchor: {selfAnchorName} on {targetBlockRoot.name}");
                return;
            }

            Vector3 offset = targetBlockRoot.position - stackedAnchor.position;
            stackedObject.transform.position = targetAnchor.position + offset;

            UpdateSortingOrder(targetBlockRoot.GetComponent<StackPlacer>(), side);
        }

        private void UpdateSortingOrder(StackPlacer stackedPlacer, AnchorSite side)
        {
            if (stackedPlacer == null || spriteRenderer == null) return;

            SpriteRenderer stackedRenderer = stackedPlacer.GetComponent<SpriteRenderer>();
            if (stackedRenderer == null) return;

            AnchorConfig config = GetConfigForSide(side);
            if (config == null) return;

            switch (config.sortingBehavior)
            {
                case SortingBehavior.GreaterThan:
                    stackedRenderer.sortingOrder = spriteRenderer.sortingOrder + 1;
                    break;
                case SortingBehavior.LessThan:
                    stackedRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;
                    break;
                case SortingBehavior.NoChange:
                    break;
            }
        }

        private GameObject InstantiateStackedBlock(GameObject blockPrefab, string newName)
        {
            GameObject newBlock = Instantiate(blockPrefab, transform);
            newBlock.name = newName;
            return newBlock;
        }

        private bool PrefabNameDiffers(GameObject existingObject, GameObject newPrefab)
        {
            string existingName = existingObject.name.Replace("(Clone)", "").Replace(STACKED_IDENTIFIER, "");
            return !existingName.Contains(newPrefab.name);
        }

        private AnchorConfig GetConfigForSide(AnchorSite side)
        {
            return side switch
            {
                AnchorSite.Top => topConfig,
                AnchorSite.Bottom => bottomConfig,
                AnchorSite.Left => leftConfig,
                AnchorSite.Right => rightConfig,
                _ => null
            };
        }

        private AnchorSite GetOpposite(AnchorSite side)
        {
            return side switch
            {
                AnchorSite.Top => AnchorSite.Bottom,
                AnchorSite.Bottom => AnchorSite.Top,
                AnchorSite.Left => AnchorSite.Right,
                AnchorSite.Right => AnchorSite.Left,
                _ => AnchorSite.Top
            };
        }

        public string GetStructureString(int indent)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(new string(' ', indent * 2) + name);

            foreach (AnchorSite side in System.Enum.GetValues(typeof(AnchorSite)))
            {
                string stackedName = $"{side}{STACKED_IDENTIFIER}";
                Transform stacked = transform.Find(stackedName);
                if (stacked != null && stacked.GetComponentInChildren<StackPlacer>() is StackPlacer placer)
                {
                    sb.Append(placer.GetStructureString(indent + 1));
                }
            }

            return sb.ToString();
        }
    }
}
