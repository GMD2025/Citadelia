using System;
using _Scripts.TilemapGrid;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace _Scripts
{
    public class FingerTracking : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public Building building;
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
            gridController.HighlightSize = building.cellsize;
            Debug.Log(building.cellsize);
            Debug.Log("Highilight size - " + gridController.HighlightSize);
            draggedBuilding = new GameObject(building.name);
            draggedBuilding.transform.position = gridController.transform.position;

            SpriteRenderer spriteRenderer = draggedBuilding.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = building.sprite;
            spriteRenderer.sortingOrder = 4;
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Debug.Log(gridController.CellPosition);
            GameObject newCell = Instantiate(draggedBuilding);
            newCell.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            Destroy(draggedBuilding);
        }
    }
}