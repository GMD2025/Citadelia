using _Scripts.TilemapGrid;
using _Scripts.UI.Buildings;
using UnityEngine;
using UnityEngine.EventSystems;

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
            Debug.Log(Building.cellsize);
            Debug.Log("Highilight size - " + gridController.HighlightSize);
            draggedBuilding = new GameObject(Building.name);
            draggedBuilding.transform.position = gridController.transform.position;

            SpriteRenderer spriteRenderer = draggedBuilding.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = Building.sprite;
            spriteRenderer.sortingLayerName = BuildingSortingLayer;
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Debug.Log(gridController.CellPosition);
            GameObject newCell = Instantiate(draggedBuilding);
            var spriteRenderer = newCell.GetComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(1, 1, 1, 1);
            Destroy(draggedBuilding);
        }
    }
}