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
        [SerializeField] private bool shouldHighlight = true;
        [SerializeField] private Vector2Int highlightSize = new(1, 1);
        [SerializeField] private TileBase transparentTile;

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
                        int signX = x % 2 == 0 ? 1 : -1;
                        int signY = y % 2 == 0 ? 1 : -1;
                        float addX = grid.cellSize.x * Mathf.Floor(x / 2) * signX;
                        float addY = grid.cellSize.y * Mathf.Floor(y / 2) * signY;
                        Debug.Log($"Adding highlight cell {addX}, {addY}, while x and y are {x}, {y}");
                        newCell.transform.position +=
                            new Vector3(addX,
                                addY, 0);
                        supplementalHighlights.Add(newCell);
                    }
                }
            }
        }

        public bool Selectable { get; private set; } = true;

        private SpriteRenderer highlightRenderer;
        public GameObject highlightObject {get; private set;}
        public GameObject highlightParent {get; private set;}
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
            highlightParent = GameObject.Find("Highlight");
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
            
            highlightObject = Instantiate(highlighterPrefab, highlightParent.transform);
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
                cellBottomLeft.x + 0.5f + Mathf.Floor((size.x - 1 )/ 2),
                cellBottomLeft.y + 0.5f + Mathf.Floor((size.y - 1 )/ 2),
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
        
        private void RepaintHighlight()
        {
            if (tilemaps == null || tilemaps.Length == 0)
            {
                return;
            }
            Tilemap tilemap = tilemaps[tilemaps.Length - 1];
            GameObject[] gameObjects = highlightObject.GetComponentsInChildren<Transform>().Select(t => t.gameObject).ToArray();
            bool denied = false;
            foreach (var gameObject in gameObjects)
            {
                SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
                TileBase found = tilemap.GetTile(Vector3Int.RoundToInt(gameObject.transform.position - new Vector3(0.5f, 0.5f, 0)));
                if (found)
                {
                    renderer.color = highlightColorDeny;
                    denied = true;
                }
                else
                {
                    renderer.color = highlightColor;
                }
            }
            Selectable = !denied;
        }

        public void SetTileAsOccupied()
        {
            Tilemap tilemap = tilemaps[^1];
            GameObject[] higlightTiles = highlightObject.GetComponentsInChildren<Transform>().Select(t => t.gameObject).ToArray();
            foreach (var higlightTile in higlightTiles)
            {
                tilemap.SetTile(Vector3Int.FloorToInt(higlightTile.transform.position), transparentTile);
            }
        }
    }
}