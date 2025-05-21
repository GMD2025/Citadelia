using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Data;
using _Scripts.Gameplay.Buildings;
using _Scripts.Gameplay.UserInput;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Systems.UI
{
    public class ButtonGenerator : MonoBehaviour
    {
        [SerializeField] private List<GameObject> buildingPrefabs;
        [SerializeField] private GameObject tileButtonPrefab;
        [SerializeField] private string buildingSortingLayer;

        private List<BuildingController> buildings;
        public static event Action OnButtonLoad;

        private void Start()
        {
            buildings = new List<BuildingController>();
            foreach (var prefab in buildingPrefabs)
            {
                var buildingController = prefab.GetComponent<BuildingController>();
                if(!buildingController) continue;
                buildings.Add(buildingController);
            }
            LoadTiles();
        }

        private void LoadTiles()
        {
            foreach (var building in buildings.Where(building => building != null))
            {
                CreateButton(building);
            }
            OnButtonLoad?.Invoke();
        }

        private void CreateButton(BuildingController building)
        {
            GameObject buttonObj = Instantiate(tileButtonPrefab, transform);

            if (building.Data.resources.Length != 0)
            {
                for (int i = 0; i < building.Data.resources.Length; i++)
                {
                    buttonObj.GetComponentsInChildren<TextMeshProUGUI>()[i].text = building.Data.resources[i].amount.ToString();
                    buttonObj.GetComponentsInChildren<SpriteRenderer>()[i].sprite = building.Data.resources[i].resourceData.icon;
                }
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