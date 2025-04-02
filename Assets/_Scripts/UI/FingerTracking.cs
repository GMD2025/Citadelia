using _Scripts.TilemapGrid;
using _Scripts.UI.Buildings;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace _Scripts.UI
{
    public class FingerTracking : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public Building Building { get; set; }
        public string BuildingSortingLayer { get; set; }
        private GameObject lastHighlightedTile;
        private GameObject draggedBuilding;

        private HighlightGridAreaController gridController;
        private Grid gridGameObject;
        private Vector3 cellsize;


        private void Awake()
        {
            gridController = FindAnyObjectByType<HighlightGridAreaController>();
            gridGameObject = FindAnyObjectByType<Grid>();
            cellsize = gridGameObject.GetComponent<Grid>().cellSize;
        }

        void Update()
        {
            if (draggedBuilding != null)
            {
                Vector3 pointer = gridController.CellPosition;
                draggedBuilding.transform.position = new Vector2(pointer.x + cellsize.x/2, pointer.y + cellsize.y/2);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            gridController.HighlightSize = Building.cellsize;
            draggedBuilding = new GameObject(Building.name);
            draggedBuilding.transform.position = gridController.transform.position;

            SpriteRenderer spriteRenderer = draggedBuilding.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = Building.sprite;
            spriteRenderer.sortingLayerName = BuildingSortingLayer;
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (gridController.Selectable) gridController.SetTileAsOccupied();
            // Reset to one tile for PC to see the standard highlight on hover
            gridController.HighlightSize = new Vector2Int(1, 1);
            Destroy(draggedBuilding);

            if (!gridController.Selectable)
            {
                gridController.highlightParent.transform.DOShakePosition(1f, new Vector3(0.3f, 0, 0), 30);
                return;
            }
            
            GameObject newCell = Instantiate(draggedBuilding);
            var spriteRenderer = newCell.GetComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(1, 1, 1, 1);
        }
    }
}