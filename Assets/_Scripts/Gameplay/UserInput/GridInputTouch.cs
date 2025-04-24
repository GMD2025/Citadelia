using UnityEngine;

namespace _Scripts.Gameplay.UserInput
{
    public class GridInputTouch : IGridInput
    {
        
        public GridInputTouch()
        {
            mainCamera = Camera.main;
        }

        public override Vector3Int? GetCurrentPosition(Grid grid)
        {
            Vector3 inputPosition = Input.mousePosition;
            if (inputPosition.x < 0 || inputPosition.x > Screen.width || inputPosition.y < 0 || inputPosition.y > Screen.height)
            {
                return null;
            }
            Vector2 worldPosition = mainCamera.ScreenToWorldPoint(inputPosition);
            
            return grid.WorldToCell(worldPosition);
        }

    }
}