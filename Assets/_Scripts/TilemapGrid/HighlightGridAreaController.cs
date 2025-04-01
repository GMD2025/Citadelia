using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace _Scripts.TilemapGrid
{
    public class HighlightGridAreaController : MonoBehaviour
    {
        [SerializeField] private GameObject highlighterPrefab;
        [SerializeField] private bool shouldHighlight = true;
        [SerializeField] private Vector2Int highlightSize = new(1, 1);

        [Header("Highlight Colors")]
        [SerializeField] private Color highlightColor = Color.white;
        [SerializeField] private Color highlightColorDeny = Color.red;

        public Vector3Int CellPosition { get; private set; }

        public bool ShouldHighlight
        {
            get => shouldHighlight;
            set => shouldHighlight = value;
        }

        public Vector2Int HighlightSize
        {
            get => highlightSize;
            set
            {
                Debug.Log($"Setting highlight size: {value}");
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
                        newCell.transform.position +=
                            new Vector3(grid.cellSize.x * (x - 1), grid.cellSize.y * (y - 1), 0);
                        newCell.GetComponent<SpriteRenderer>().color = highlightColorDeny;
                        supplementalHighlights.Add(newCell);
                    }
                }
            }
        }

        private SpriteRenderer highlightRenderer;
        private GameObject highlightObject;
        private Grid grid;
        private Camera mainCamera;
        private Tilemap[] tilemaps;
        private List<GameObject> supplementalHighlights = new List<GameObject>();

        public BoundsInt HighlightedAreaBounds { get; private set; }

        private void Awake()
        {
            grid = GetComponent<Grid>();
            mainCamera = Camera.main;
            tilemaps = GetComponentsInChildren<Tilemap>();
            CreateHighlightObject();
        }

        private void Update()
        {
            HandleUserHover();
            RepaintHighlight();
        }

        private void CreateHighlightObject()
        {
            if (highlighterPrefab == null)
            {
                Debug.LogError("Outline Prefab not assigned!");
                return;
            }

            highlightObject = Instantiate(highlighterPrefab);
            highlightRenderer = highlightObject.GetComponent<SpriteRenderer>();
            highlightRenderer.color = highlightColor;
            highlightObject.SetActive(false);
        }

        private void HandleUserHover()
        {
            if (!shouldHighlight)
            {
                highlightObject.SetActive(false);
                return;
            }

            highlightObject.SetActive(true);

            Vector3 mousePos = Input.mousePosition;

            if (mousePos.x < 0 || mousePos.x > Screen.width || mousePos.y < 0 || mousePos.y > Screen.height)
            {
                return;
            }

            Vector2 worldPosition = mainCamera.ScreenToWorldPoint(mousePos);

            CellPosition = grid.WorldToCell(worldPosition);

            HighlightGridArea(CellPosition, highlightSize);
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
                cellBottomLeft.x + 0.5f,
                cellBottomLeft.y + 0.5f,
                0
            );

            highlightObject.transform.position = centerPosition;
            // highlightObject.transform.localScale = new Vector3(width, height, 1);

            HighlightedAreaBounds = new BoundsInt(
                Vector3Int.FloorToInt(adjustedStartPosition),
                new Vector3Int(size.x, size.y,
                    1) // ensure size.z = 1, otherwise tilemap doesn't return any tiles from the bounds
            );
        }

        public Dictionary<Vector3Int, TileBase> GetHighlightedTiles(BoundsInt bounds)
        {
            if (tilemaps == null || tilemaps.Length == 0)
            {
                return null;
            }

            Dictionary<Vector3Int, TileBase> topLayerTiles = new();

            for (int i = tilemaps.Length - 1; i >= 0; i--) // Start from topmost tilemap
            {
                Tilemap tilemap = tilemaps[i];
                TileBase[] tiles = tilemap.GetTilesBlock(bounds);

                if (tiles.Length == 0)
                {
                    continue;
                }

                int index = 0;
                foreach (Vector3Int pos in bounds.allPositionsWithin)
                {
                    if (tiles[index] != null && !topLayerTiles.ContainsKey(pos))
                    {
                        topLayerTiles[pos] = tiles[index];
                    }

                    index++;
                }
            }

            return topLayerTiles;
        }

        private void RepaintHighlight()
        {
            if (tilemaps == null || tilemaps.Length == 0)
            {
                return;
            }
            Tilemap tilemap = tilemaps[3];
            GameObject[] gameObjects = highlightObject.GetComponentsInChildren<Transform>().Select(t => t.gameObject).ToArray();
            foreach (var gameObject in gameObjects)
            {
                TileBase found = tilemap.GetTile(Vector3Int.RoundToInt(gameObject.transform.position - new Vector3(0.5f, 0.5f, 0)));
                gameObject.GetComponent<SpriteRenderer>().color = found ? highlightColorDeny : highlightColor;
            }
        }
    }
}