using _Scripts.Systems.TilemapGrid;
using UnityEngine;

namespace _Scripts.Gameplay.UserInput
{
    public class GridInputKeyboardGamepad : IGridInput
    {
        private HighlightGridAreaController obj = GameObject.Find("Grid").GetComponent<HighlightGridAreaController>();
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
            Vector3Int newPosition = CellPosition + Vector3Int.RoundToInt(direction);
            bool move = ! obj.isNextToTheBorder(direction);
            if (move) CellPosition = newPosition;
        }
    }
}