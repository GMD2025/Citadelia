using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Data;
using _Scripts.Gameplay.Buildings;
using _Scripts.Gameplay.UserInput;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class ButtonGenerator : MonoBehaviour
    {
        [SerializeField] private List<GameObject> buildingPrefabs;
        [SerializeField] private GameObject tileButtonPrefab;
        [SerializeField] private string buildingSortingLayer;

        private List<BuildingController> buildings;


        private void Awake()
        {
            buildings = new List<BuildingController>();
            foreach (var prefab in buildingPrefabs)
            {
                var buildingController = prefab.GetComponent<BuildingController>();
                if(!buildingController) continue;
                buildings.Add(buildingController);
            }
        }

        private void Start()
        {
            LoadTiles();
        }

        private void LoadTiles()
        {
            foreach (var building in buildings.Where(building => building != null))
            {
                CreateButton(building);
            }
        }

        private void CreateButton(BuildingController building)
        {
            GameObject buttonObj = Instantiate(tileButtonPrefab, transform);

            if (building.Data.resources.Length != 0)
            {
                buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = building.Data.resources[0].amount.ToString();
                buttonObj.GetComponentInChildren<SpriteRenderer>().sprite = building.Data.resources[0].resourceData.icon;
            }
            
            var buildingPlacer = buttonObj.GetComponent<BuildingPlacer>();
            if (LocalDependencyContainer.Instance.SelectedInputMode == InputMode.Pointer) buttonObj.AddComponent<TouchTracking>();
            buildingPlacer.BuildingController = building;
            buildingPlacer.BuildingSortingLayer = buildingSortingLayer;
            Image image = buttonObj.GetComponent<Image>();
            if (image != null)
            {
                image.sprite = building.Sprite;
            }
        }
    }
}