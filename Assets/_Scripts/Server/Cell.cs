using UnityEngine;

namespace _Scripts.Server
{
    public class Cell
    {
        private Vector2Int id;
        private GameObject building;
        private bool isBuildingPlaced;
        
        public GameObject Building => building;
        public bool IsBuildingPlaced => isBuildingPlaced;
        public Vector2Int Id => id;

        public Cell(Vector2Int cellPosition)
        {
            id = cellPosition;
        }

        public void ClearTile()
        {
            building = null;
            isBuildingPlaced = false;
        }
        
        public void PlaceBuilding(GameObject newBuilding)
        {
            building = newBuilding;
            isBuildingPlaced = true;
        }
    }
}