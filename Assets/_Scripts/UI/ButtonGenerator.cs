using System.Collections.Generic;
using _Scripts.UI.Buildings;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class ButtonGenerator : MonoBehaviour
    {
        [SerializeField] private List<Building> buildings;
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
            foreach (Building building in buildings)
            {
                if (building != null)
                {
                    CreateButton(building);
                }
            }
        }

        void CreateButton(Building building)
        {
            GameObject buttonObj = Instantiate(tileButtonPrefab, transform);

            if (building.resources.Length != 0)
            {
                buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = building.resources[0].amount.ToString();
                buttonObj.GetComponentInChildren<SpriteRenderer>().sprite = building.resources[0].resourceData.icon;
            }
            
            var fingerTracking = buttonObj.GetComponent<FingerTracking>();
            fingerTracking.Building = building;
            fingerTracking.BuildingSortingLayer = buildingSortingLayer;
            Image image = buttonObj.GetComponent<Image>();
            if (image != null)
            {
                image.sprite = building.sprite;
            }
        }

        void Update()
        {
        }
    }
}