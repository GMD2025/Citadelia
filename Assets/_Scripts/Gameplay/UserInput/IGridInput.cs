using System;
using UnityEngine;

namespace _Scripts.Gameplay.UserInput
{
    [Serializable]
    public abstract class IGridInput
    {
        [SerializeField] private string hui;
        protected Camera mainCamera;
        public Vector3Int CellPosition { get; protected set; }

        public abstract Vector3Int? GetCurrentPosition(Grid grid);

    }
}