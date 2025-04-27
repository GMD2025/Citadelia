using UnityEngine;
using UnityEngine.EventSystems;

namespace _Scripts.Gameplay.UserInput
{
    public class TouchTracking : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private BuildingPlacer buildingPlacer;
        private void Awake()
        {
            buildingPlacer = GetComponent<BuildingPlacer>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            buildingPlacer.PlaceBuildingPlaceholder();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
           buildingPlacer.PlaceBuilding();
        }
    }
}