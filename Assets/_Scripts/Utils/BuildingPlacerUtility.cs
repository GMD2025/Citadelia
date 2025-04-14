using _Scripts.UI.Buildings;
using UnityEngine;

namespace _Scripts.Utils
{
    public class BuildingPlacerUtility
    {
        public static Vector2 GetRealSize(BuildingData buildingData, Grid grid)
        {
            Vector2 targetSize = new Vector2(
                grid.cellSize.x * buildingData.cellsize.x,
                grid.cellSize.y * buildingData.cellsize.y
            );

            return targetSize;
        }
        
        public static Vector3 GetLocalScale(BuildingData buildingData, Grid grid)
        {
            var targetWorldSize = GetRealSize(buildingData, grid);
            var spriteSize = buildingData.Sprite.bounds.size;
            return new Vector3(targetWorldSize.x / spriteSize.x, targetWorldSize.y / spriteSize.y, 1f);
        }
            
        public static void AdjustSize(GameObject buildingGameObject, BuildingData buildingData, Grid grid)
        {
            Vector2 targetSize = new Vector2(
                grid.cellSize.x * buildingData.cellsize.x,
                grid.cellSize.y * buildingData.cellsize.y
            );
            
            SpriteRenderer sr = buildingGameObject.GetComponent<SpriteRenderer>();
            if (sr != null && sr.sprite != null)
            {
                Vector2 spriteSize = sr.sprite.bounds.size;
                Vector3 newScale = new Vector3(
                    targetSize.x / spriteSize.x,
                    targetSize.y / spriteSize.y,
                    1f
                );

                buildingGameObject.transform.localScale = newScale;
            }

            BoxCollider2D box2D = buildingGameObject.GetComponent<BoxCollider2D>();
            if (box2D != null)
            {
                box2D.size = targetSize;
                box2D.offset = Vector2.zero;
            }
        }
    }
}