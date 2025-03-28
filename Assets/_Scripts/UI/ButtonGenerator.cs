using System.Collections.Generic;
using System.Linq;
using _Scripts;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.U2D;
using UnityEngine.UI;

public class ButtonGenerator : MonoBehaviour
{
    [SerializeField] private List<Building> buildings;
    [SerializeField] private GameObject tileButtonPrefab;
    private Canvas canvas;
    
    private string folderPath = "Assets/Tilemap/Tiles/KenneyMedievalTiles";
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
        // Debug.Log(sprite);
        GameObject buttonObj = Instantiate(tileButtonPrefab, transform);
        buttonObj.GetComponent<FingerTracking>().building = building;
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
