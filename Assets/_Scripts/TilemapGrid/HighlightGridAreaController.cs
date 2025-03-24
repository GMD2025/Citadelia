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
        [SerializeField] private Color highlightColor = Color.white;
        [SerializeField] private bool shouldHighlight = true;
        [SerializeField] private Vector2Int highlightSize = new(1, 1);

        public bool ShouldHighlight
        {
            get => shouldHighlight;
            set => shouldHighlight = value;
        }

        public Vector2Int HighlightSize
        {
            get => highlightSize;
            set =>
                highlightSize = new Vector2Int(
                    Mathf.Max(1, value.x),
                    Mathf.Max(1, value.y)
                );
        }

        private SpriteRenderer highlightRenderer;
        private GameObject highlightObject;
        private Grid grid;
        private Camera mainCamera;
        private Tilemap[] tilemaps;

        private BoundsInt highlightedAreaBounds;

        private void Awake()
        {
            grid = GetComponent<Grid>();
            mainCamera = Camera.main;
            tilemaps = GetComponentsInChildren<Tilemap>();
            CreateHighlightObject();
        }

        private void Update()
        {
            highlightRenderer.color = highlightColor;
            HandleUserHover();
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

            Vector2 worldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            Vector3Int cellPosition = grid.WorldToCell(worldPosition);

            HighlightGridArea(cellPosition, highlightSize);
            GetHighlightedTiles(highlightedAreaBounds);
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

            float tileSizeX = grid.cellSize.x;
            float tileSizeY = grid.cellSize.y;
            float width = size.x * tileSizeX;
            float height = size.y * tileSizeY;

            Vector3 centerPosition = new Vector3(
                cellBottomLeft.x + (width / 2),
                cellBottomLeft.y + (height / 2),
                0
            );

            highlightObject.transform.position = centerPosition;
            highlightObject.transform.localScale = new Vector3(width, height, 1);

            highlightedAreaBounds = new BoundsInt(
                Vector3Int.FloorToInt(adjustedStartPosition), 
                new Vector3Int(size.x, size.y, 1) // ensure size.z = 1, otherwise tilemap doesn't return any tiles from the bounds
            );
        }

        public TileBase[] GetHighlightedTiles(BoundsInt bounds)
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

            return topLayerTiles.Values.ToArray();
        }
    }
}