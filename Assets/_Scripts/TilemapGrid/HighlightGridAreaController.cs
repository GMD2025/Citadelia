using System.Collections.Generic;
using System.Linq;
using _Scripts.Gameplay.UserInput;
using _Scripts.Utils;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Scripts.TilemapGrid
{
    public class HighlightGridAreaController : MonoBehaviour
    {
        [Header("Highlighter Setup")]
        [SerializeField] private GameObject highlighterCellPrefab;
        [SerializeField] private GameObject highlighterParentPrefab;
        [SerializeField] private bool shouldHighlight = true;
        [SerializeField] private Vector2Int highlightSize = new(1, 1);

        [Header("Highlight Colors")]
        [SerializeField] private Color highlightColor = Color.white;
        [SerializeField] private Color highlightColorDeny = Color.red;

        [SerializeField] private TileBase transparentTile;

        [Header("Tilemaps preventing from placing building")]
        [SerializeField] private Tilemap[] tilemapsToDeny;


        public Tilemap[] TilemapsToDeny => tilemapsToDeny;
        public Vector2Int HighlightSize
        {
            get => highlightSize;
            set => SetHighlightedArea(value);
        }
        public GameObject highlightParent {get; private set;}
        public Vector2Int[] HighlightedCells {
            get
            { 
                return highlightParent.GetComponentsInChildren<Transform>()
                    .Select(t => t.gameObject.transform.position)
                    .Select(p => VectorUtil.ToVector2Int(p))
                    .ToArray();
            }
        }

        
        public bool Selectable { get; private set; } = true;
        private Vector2Int lastHighlightSize = new(1, 1);
        private SpriteRenderer highlightRenderer;
        private Grid grid;
        private Tilemap[] tilemaps;
        private List<GameObject> supplementalHighlights = new List<GameObject>();


        private void Awake()
        {
            grid = GetComponent<Grid>();
            tilemaps = GetComponentsInChildren<Tilemap>();
            CreateHighlightObject();
        }

        private void Update()
        {
            HandleInput();
            if (Application.isPlaying && highlightSize != lastHighlightSize)
            {
                SetHighlightedArea(highlightSize);
            }
            RepaintHighlight();
        }

        private void SetHighlightedArea(Vector2Int value)
        {
            lastHighlightSize = value;
            int width = Mathf.Max(1, value.x);
            int height = Mathf.Max(1, value.y);
            highlightSize = new Vector2Int(width, height);

            // tear down old cells
            foreach (var go in supplementalHighlights) Destroy(go);
            supplementalHighlights.Clear();

            // instantiate new cells as children, positioned locally around (0,0)
            for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                var cell = Instantiate(highlighterCellPrefab, highlightParent.transform);
                cell.name = $"HighlightCell ({x},{y})";

                // local offset so that (0,0) is at the group center
                float offsetX = (x - (width - 1) / 2f) * grid.cellSize.x;
                float offsetY = (y - (height - 1) / 2f) * grid.cellSize.y;
                cell.transform.localPosition = new Vector3(offsetX, offsetY, 0);

                supplementalHighlights.Add(cell);
            }
        }

        private void HighlightGridArea(Vector3Int startTilePosition, Vector2Int size)
        {
            var offset = new Vector3Int(
                -(size.x - 1) / 2,
                -(size.y - 1) / 2,
                0
            );
            
            var bottomLeftCell = startTilePosition + offset;
            Vector3 worldBL = grid.CellToWorld(bottomLeftCell);
            float halfW = size.x * grid.cellSize.x * 0.5f;
            float halfH = size.y * grid.cellSize.y * 0.5f;
            highlightParent.transform.position = worldBL + new Vector3(halfW, halfH, 0);
        }


        private void CreateHighlightObject()
        {
            if (highlighterCellPrefab == null)
            {
                Debug.LogError("Outline Prefab not assigned!");
                return;
            }

            highlightParent = Instantiate(highlighterParentPrefab, transform);
            highlightParent.transform.SetParent(transform, worldPositionStays: true);
            SetHighlightedArea(highlightSize);
        }

        private void HandleInput()
        {
            if (!shouldHighlight)
            {
                highlightParent.SetActive(false);
                return;
            }

            highlightParent.SetActive(true);

            IGridInput input = LocalDependencyContainer.Instance.Resolve<IGridInput>();

            if (input.GetCurrentPosition(grid) is { } inputPosition)
                HighlightGridArea(inputPosition, highlightSize);
        }

        private void RepaintHighlight()
        {
            if (tilemaps == null || tilemaps.Length == 0)
                return;

            GameObject[] gameObjects = highlightParent.GetComponentsInChildren<Transform>()
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

        public void UpdateTilemaps(List<Tilemap> newTilemaps, List<Tilemap> newTilemapsToDeny)
        {
            tilemaps = newTilemaps.ToArray();
            tilemapsToDeny = newTilemapsToDeny.ToArray();
        }

        public void ResetHighlighterSize()
        {
            HighlightSize = new Vector2Int(1, 1);
        }

        public void SetTileAsOccupied()
        {
            Tilemap tilemap = tilemaps
                .OrderBy(t => t.GetComponent<TilemapRenderer>().sortingOrder)
                .Last();
            foreach (var higlightTile in HighlightedCells)
            {
                tilemap.SetTile(VectorUtil.ToVector3Int(higlightTile), transparentTile);
            }
        }
    }
}