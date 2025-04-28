using _Scripts.Data;
using _Scripts.Gameplay.Buildings;
using _Scripts.Gameplay.ResourceSystem;
using _Scripts.Network;
using _Scripts.TilemapGrid;
using _Scripts.Utils;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Gameplay.UserInput
{
    public class BuildingPlacer : MonoBehaviour
    {
        [SerializeField] private bool isTodayBlackFriday;
        public BuildingController BuildingController { get; set; }
        public string BuildingSortingLayer { get; set; }
        private GameObject draggedBuilding;

        private HighlightGridAreaController gridController;
        private ResourceProductionService resourceService;
        private Grid gridGameObject;


        private void Awake()
        {
            gridController = FindAnyObjectByType<HighlightGridAreaController>();
            resourceService = LocalDependencyContainer.Instance.Resolve<ResourceProductionService>();
            gridGameObject = FindAnyObjectByType<Grid>();
        }

        void Update()
        {
            if (draggedBuilding)
                draggedBuilding.transform.position = gridController.highlightParent.transform.position;
        }

        public void PlaceBuildingPlaceholder()
        {
            gridController.HighlightSize = BuildingController.Data.cellsize;
            draggedBuilding = new GameObject(BuildingController.Data.name);
            draggedBuilding.transform.position = gridController.highlightParent.transform.position;

            SpriteRenderer spriteRenderer = draggedBuilding.AddComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
            spriteRenderer.sprite = BuildingController.Sprite;
            spriteRenderer.sortingLayerName = BuildingSortingLayer;
            draggedBuilding.transform.localScale = BuildingPlacerUtility.GetLocalScale(BuildingController, gridGameObject);
        }
        public void PlaceBuilding()
        {
            if (gridController.Selectable)
            {
                bool richEnough = resourceService.SpendResources(BuildingController.Data.resources);

                if ((isTodayBlackFriday || richEnough))
                {
                    if (NetworkManager.Singleton.IsClient && NetworkManager.Singleton.LocalClient.PlayerObject)
                    {
                        Debug.Log("Client instantiating");
                        var playerCmd = NetworkManager.Singleton.LocalClient.PlayerObject
                            .GetComponent<NetworkSpawnerCommands>();

                        if (playerCmd)
                            playerCmd.SpawnNetworkObjectServerRpc(BuildingController.Id, draggedBuilding.transform.position);
                        gridController.SetTileAsOccupied();
                    }
                }

            }
            else
            {
                gridController.highlightParent.transform.DOShakePosition(1f, new Vector3(0.3f, 0, 0), 30);
            }

            gridController.ResetHighlighterSize();
            Destroy(draggedBuilding);
        }

        public void Cancel()
        {
            Destroy(draggedBuilding);
        }
    }
}