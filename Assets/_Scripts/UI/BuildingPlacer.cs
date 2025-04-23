using _Scripts.ResourceSystem;
using _Scripts.TilemapGrid;
using _Scripts.UI.Buildings;
using _Scripts.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace _Scripts.UI
{
    public class BuildingPlacer : MonoBehaviour
    {
        [SerializeField] private bool isTodayBlackFriday;
        public BuildingData BuildingData { get; set; }
        public string BuildingSortingLayer { get; set; }
        private GameObject draggedBuilding;

        private HighlightGridAreaController gridController;
        private ResourceProductionService resourceService;
        private Grid gridGameObject;
        private Vector3 cellsize;


        private void Awake()
        {
            gridController = FindAnyObjectByType<HighlightGridAreaController>();
            resourceService = DependencyContainer.Instance.Resolve<ResourceProductionService>();
            gridGameObject = FindAnyObjectByType<Grid>();
            cellsize = gridGameObject.GetComponent<Grid>().cellSize;
        }

        void Update()
        {
            if (draggedBuilding != null)
                draggedBuilding.transform.position = gridController.highlightParent.transform.position;
        }

        public void OnPointerDown()
        {
            gridController.HighlightSize = BuildingData.cellsize;
            draggedBuilding = new GameObject(BuildingData.name);
            draggedBuilding.transform.position = gridController.highlightParent.transform.position;

            SpriteRenderer spriteRenderer = draggedBuilding.AddComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
            spriteRenderer.sprite = BuildingData.Sprite;
            spriteRenderer.sortingLayerName = BuildingSortingLayer;
            draggedBuilding.transform.localScale = BuildingPlacerUtility.GetLocalScale(BuildingData, gridGameObject);
        }
        public void OnPointerUp()
        {
            if (gridController.Selectable)
            {
                bool richEnough = resourceService.SpendResources(BuildingData.resources);

                if ((isTodayBlackFriday || richEnough))
                {
                    GameObject newBuilding = Instantiate(BuildingData.buildingPrefab,
                        draggedBuilding.transform.position, Quaternion.identity, gridGameObject.transform);
                    BuildingPlacerUtility.AdjustSize(newBuilding, BuildingData, gridGameObject);
                    gridController.SetTileAsOccupied();
                }

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