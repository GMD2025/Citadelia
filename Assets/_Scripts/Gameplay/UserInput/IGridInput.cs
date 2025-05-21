using System;
using UnityEngine;

namespace _Scripts.Gameplay.UserInput
{
    [Serializable]
    public abstract class IGridInput
    {
        protected Camera mainCamera;
        public Vector3Int CellPosition { get; protected set; } = new Vector3Int(0, -5, 0);

        public abstract Vector3Int? GetCurrentPosition(Grid grid);

    }
}