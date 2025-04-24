using _Scripts.Data;
using _Scripts.Gameplay.Buildings;
using UnityEngine;

namespace _Scripts.Utils
{
    public static class BuildingPlacerUtility
    {
        private static Vector2 GetRealSize(BuildingData buildingData, Grid grid)
        {
            Vector2 targetSize = new Vector2(
                grid.cellSize.x * buildingData.cellsize.x,
                grid.cellSize.y * buildingData.cellsize.y
            );

            return targetSize;
        }
        
        public static Vector3 GetLocalScale(BuildingController buildingController, Grid grid)
        {
            var targetWorldSize = GetRealSize(buildingController.Data, grid);
            var spriteSize = buildingController.Sprite.bounds.size;
            return new Vector3(targetWorldSize.x / spriteSize.x, targetWorldSize.y / spriteSize.y, 1f);
        }
    }
}