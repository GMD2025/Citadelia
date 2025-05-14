using UnityEngine;

namespace _Scripts.Gameplay.UserInput
{
    public class GridInputKeyboardGamepad : IGridInput
    {
        public GridInputKeyboardGamepad()
        {
            mainCamera = Camera.main;
        }

        public override Vector3Int? GetCurrentPosition(Grid grid)
        {
            return CellPosition;
        }

        public void MoveCurrentPosition(Vector2 direction)
        {
            CellPosition += Vector3Int.RoundToInt(direction);
        }
    }
}