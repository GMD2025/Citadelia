using _Scripts.TilemapGrid;
using UnityEngine;

namespace _Scripts
{
    public class DependencyContainer
    {
        private static DependencyContainer instance;
        public static DependencyContainer Instance => instance ?? (instance = new DependencyContainer());
        
        public IGridInput GridInput { get; private set; }

        private DependencyContainer()
        {
            GridInput = new GridInputKeyboard();
        }
    }
}