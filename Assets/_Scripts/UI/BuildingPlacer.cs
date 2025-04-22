using _Scripts.ResourceSystem;
using _Scripts.TilemapGrid;
using _Scripts.UI.Buildings;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace _Scripts.UI
{
    public class BuildingPlacer : MonoBehaviour
    {
        public Building Building { get; set; }
        public string BuildingSortingLayer { get; set; }
        private GameObject draggedBuilding;

        private HighlightGridAreaController gridController;
        private ResourceProductionService resourceService;
        private Grid gridGameObject;
        private Vector3 cellsize;


        private void Awake()
        {
            gridController = FindAnyObjectByType<HighlightGridAreaController>();
            resourceService = FindAnyObjectByType<ResourceProductionService>();
            gridGameObject = FindAnyObjectByType<Grid>();
            cellsize = gridGameObject.GetComponent<Grid>().cellSize;
        }

        void Update()
        {
            if (draggedBuilding != null)
            {
                Vector3Int? pointer = DependencyContainer.Instance.GridInput.GetCurrentPosition(gridController.GetComponent<Grid>());
                if (pointer is { } p) draggedBuilding.transform.position = new Vector2(p.x + cellsize.x/2, p.y + cellsize.y/2);
            }
        }

        public void OnPointerDown()
        {
            gridController.HighlightSize = Building.cellsize;
            draggedBuilding = new GameObject(Building.name);
            draggedBuilding.transform.position = gridController.transform.position;

            SpriteRenderer spriteRenderer = draggedBuilding.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = Building.sprite;
            spriteRenderer.sortingLayerName = BuildingSortingLayer;
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
        }

        public void OnPointerUp()
        {
            if (gridController.Selectable)
            {
                bool richEnough = resourceService.SpendResources(Building.resources);

                if (richEnough)
                {
                    GameObject newCell = Instantiate(draggedBuilding, gridGameObject.transform);
                    var spriteRenderer = newCell.GetComponent<SpriteRenderer>();
                    spriteRenderer.color = new Color(1, 1, 1, 1);
                }
                
                gridController.SetTileAsOccupied();
            }
            else
            {
                gridController.highlightParent.transform.DOShakePosition(1f, new Vector3(0.3f, 0, 0), 30);
            }
            
            // Reset to one tile for PC to see the standard highlight on hover
            gridController.HighlightSize = new Vector2Int(1, 1);
            Destroy(draggedBuilding);
        }

        public void Cancel()
        {
            Destroy(draggedBuilding);
        }
    }
}