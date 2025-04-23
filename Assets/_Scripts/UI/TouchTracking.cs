using System;
using _Scripts.ResourceSystem;
using _Scripts.TilemapGrid;
using _Scripts.UI.Buildings;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace _Scripts.UI
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
            buildingPlacer.OnPointerDown();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
           buildingPlacer.OnPointerUp();
        }
    }
}