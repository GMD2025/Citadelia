using System.Collections.Generic;
using _Scripts.Gameplay.Buildings.Systems;
using _Scripts.TilemapGrid;
using _Scripts.UI.Buildings;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class ButtonGenerator : MonoBehaviour
    {
        [SerializeField] private List<BuildingData> buildings;
        [SerializeField] private GameObject tileButtonPrefab;
        [SerializeField] private string buildingSortingLayer;

        private Canvas canvas;

        void Start()
        {
            LoadTiles();
            canvas = GetComponentInParent<Canvas>();
        }

        void LoadTiles()
        {
            foreach (BuildingData building in buildings)
            {
                if (building != null)
                {
                    CreateButton(building);
                }
            }
        }

        void CreateButton(BuildingData buildingData)
        {
            GameObject buttonObj = Instantiate(tileButtonPrefab, transform);

            if (buildingData.resources.Length != 0)
            {
                buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = buildingData.resources[0].amount.ToString();
                buttonObj.GetComponentInChildren<SpriteRenderer>().sprite = buildingData.resources[0].resourceData.icon;
            }
            
            var buildingPlacer = buttonObj.GetComponent<BuildingPlacer>();
            if (DependencyContainer.Instance.inputMode == InputMode.Mouse) buttonObj.AddComponent<TouchTracking>();
            buildingPlacer.BuildingData = buildingData;
            buildingPlacer.BuildingSortingLayer = buildingSortingLayer;
            Image image = buttonObj.GetComponent<Image>();
            if (image != null)
            {
                image.sprite = buildingData.Sprite;
            }
        }

        void Update()
        {
        }
    }
}