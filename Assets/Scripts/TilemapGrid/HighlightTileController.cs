using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TilemapGrid
{
    public class HighlightTileController : MonoBehaviour
    {
        public List<Tilemap> interactiveTilemaps; // Assign in Inspector
        public Material highlightMaterial; // Assign material in Inspector
        public float highlightWidth = 0.1f;

        [SerializeField] private Vector2Int highlightSize = new Vector2Int(1, 1); // Private but visible in Inspector

        public Vector2Int HighlightSize
        {
            get => highlightSize;
            set
            {
                highlightSize = new Vector2Int(
                    Mathf.Max(1, value.x), // Ensure at least 1x1 size
                    Mathf.Max(1, value.y)
                );
            }
        }

        private GameObject highlightObject;
        private LineRenderer lineRenderer;
        private Grid grid;

        void Start()
        {
            grid = GetComponent<Grid>();
            CreateHighlightObject();
        }

        void Update()
        {
            lineRenderer.startWidth = highlightWidth;
            lineRenderer.endWidth = highlightWidth;
            HandleUserHover();
        }

        void CreateHighlightObject()
        {
            highlightObject = new GameObject("TileHighlight");
            lineRenderer = highlightObject.AddComponent<LineRenderer>();
            lineRenderer.material = highlightMaterial;
            lineRenderer.positionCount = 5;
            lineRenderer.startWidth = highlightWidth;
            lineRenderer.endWidth = highlightWidth;
            lineRenderer.sortingOrder = 999;
            highlightObject.SetActive(false);
        }

        void HandleUserHover()
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            worldPosition.z = 0;
            Vector3Int cellPosition = grid.WorldToCell(worldPosition);

            if (interactiveTilemaps.Any(tilemap => tilemap.HasTile(cellPosition)))
            {
                HighlightTile(cellPosition, highlightSize);
            }
            else
            {
                ClearHighlight();
            }
        }

        public void HighlightTile(Vector3Int startTilePosition, Vector2Int size)
        {
            Vector3 bottomLeftWorldPos = grid.CellToWorld(startTilePosition); // Start at bottom-left corner
            float tileSize = grid.cellSize.x;
    
            // Calculate full shape size
            float width = size.x * tileSize;
            float height = size.y * tileSize;

            Vector3[] corners = new Vector3[]
            {
                bottomLeftWorldPos,                          // Bottom-left
                bottomLeftWorldPos + new Vector3(width, 0),  // Bottom-right
                bottomLeftWorldPos + new Vector3(width, height), // Top-right
                bottomLeftWorldPos + new Vector3(0, height), // Top-left
                bottomLeftWorldPos // Closing loop
            };

            lineRenderer.SetPositions(corners);
            highlightObject.SetActive(true);
        }


        public void ClearHighlight()
        {
            highlightObject.SetActive(false);
        }
    }
}
