using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace _Scripts.TilemapGrid
{
    public class HighlightGridAreaController : MonoBehaviour
    {
        [SerializeField] private GameObject highlighterPrefab;
        [SerializeField] public bool shouldHighlight { get; private set; } = true;
        [SerializeField] private Vector2Int highlightSize = new(1, 1);
        [SerializeField] private TileBase transparentTile;

        [Header("Highlight Colors")] [SerializeField]
        private Color highlightColor = Color.white;

        [SerializeField] private Color highlightColorDeny = Color.red;


        [Header("Tilemaps preventing from placing building")] [SerializeField]
        private Tilemap[] tilemapsToDeny;
        
        public bool Selectable { get; private set; } = true;

        private SpriteRenderer highlightRenderer;
        public GameObject highlightObject { get; private set; }
        public GameObject highlightParent { get; private set; }
        private Grid grid;
        private Tilemap[] tilemaps;
        private List<GameObject> supplementalHighlights = new List<GameObject>();

        public bool ShouldHighlight
        {
            get => shouldHighlight;
            set => shouldHighlight = value;
        }

        public Vector2Int HighlightSize
        {
            get => highlightSize;
            set => SetHighlightedArea(value);
        }

        private void SetHighlightedArea(Vector2Int value)
        {
            int newX = Mathf.Max(1, value.x);
            int newY = Mathf.Max(1, value.y);
            highlightSize = new Vector2Int(
                newX, newY
            );

            foreach (var sh in supplementalHighlights)
            {
                Destroy(sh);
            }

            supplementalHighlights.Clear();
            for (int x = 1; x <= newX; x++)
            {
                for (int y = 1; y <= newY; y++)
                {
                    if (x == 1 && y == 1) continue;
                    GameObject newCell = Instantiate(highlighterPrefab, highlightObject.transform);
                    int signX = x % 2 == 0 ? 1 : -1;
                    int signY = y % 2 == 0 ? 1 : -1;
                    float addX = grid.cellSize.x * Mathf.Floor(x / 2) * signX;
                    float addY = grid.cellSize.y * Mathf.Floor(y / 2) * signY;
                    newCell.transform.position +=
                        new Vector3(addX,
                            addY, 0);
                    supplementalHighlights.Add(newCell);
                }
            }
        }
        
        private void Awake()
        {
            grid = GetComponent<Grid>();
            tilemaps = GetComponentsInChildren<Tilemap>();
            highlightParent = GameObject.Find("Highlight");
            CreateHighlightObject();
        }

        private void Update()
        {
            HandleInput();
            RepaintHighlight();
        }

        private void CreateHighlightObject()
        {
            if (highlighterPrefab == null)
            {
                Debug.LogError("Outline Prefab not assigned!");
                return;
            }

            highlightObject = Instantiate(highlighterPrefab, highlightParent.transform);
            highlightRenderer = highlightObject.GetComponent<SpriteRenderer>();
            highlightRenderer.color = highlightColor;
            highlightObject.SetActive(false);
        }

        private void HandleInput()
        {
            if (!shouldHighlight)
            {
                highlightObject.SetActive(false);
                return;
            }

            highlightObject.SetActive(true);

            IGridInput input = DependencyContainer.Instance.GridInput;

            if (input.GetCurrentPosition(grid) is { } inputPosition)
                HighlightGridArea(inputPosition, highlightSize);
        }

        private void HighlightGridArea(Vector3Int startTilePosition, Vector2Int size)
        {
            Vector3Int offset = new Vector3Int(
                -(size.x - 1) / 2,
                -(size.y - 1) / 2,
                0
            );

            Vector3Int adjustedStartPosition = startTilePosition + offset;
            Vector3 cellBottomLeft = grid.CellToWorld(adjustedStartPosition);

            Vector3 centerPosition = new Vector3(
                cellBottomLeft.x + 0.5f + Mathf.Floor((size.x - 1) / 2),
                cellBottomLeft.y + 0.5f + Mathf.Floor((size.y - 1) / 2),
                0
            );

            highlightObject.transform.position = centerPosition;
        }

        private void RepaintHighlight()
        {
            if (tilemaps == null || tilemaps.Length == 0)
                return;

            Tilemap tilemapToDeny = tilemaps[tilemaps.Length - 1];
            GameObject[] gameObjects = highlightObject.GetComponentsInChildren<Transform>()
                .Select(t => t.gameObject)
                .ToArray();

            bool allCellsValid = true;
            foreach (var gameObject in gameObjects)
            {
                SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
                if (!renderer)
                    continue;

                Vector3Int tilePos = Vector3Int.RoundToInt(gameObject.transform.position - new Vector3(0.5f, 0.5f, 0));

                bool hasTile = tilemaps.Any(tm => tm.GetTile(tilePos) != null);
                bool foundInDeny = tilemapsToDeny.Any(t => t.GetTile(tilePos) != null);

                renderer.enabled = hasTile;

                if (!hasTile || foundInDeny)
                {
                    renderer.color = highlightColorDeny;
                    allCellsValid = false;
                }
                else
                {
                    renderer.color = highlightColor;
                }
            }

            Selectable = allCellsValid;
        }

        public void SetTileAsOccupied()
        {
            Tilemap tilemap = tilemaps
                .OrderBy(t => t.GetComponent<TilemapRenderer>().sortingOrder)
                .Last();
            GameObject[] higlightTiles = highlightObject.GetComponentsInChildren<Transform>().Select(t => t.gameObject)
                .ToArray();
            foreach (var higlightTile in higlightTiles)
            {
                tilemap.SetTile(Vector3Int.FloorToInt(higlightTile.transform.position), transparentTile);
            }
        }
    }
}