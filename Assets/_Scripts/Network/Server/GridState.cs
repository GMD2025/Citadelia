using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Network.Server
{
    public class GridState
    {
        private List<Cell> grid;

        public List<Cell> Grid => grid;

        public GridState()
        {
            grid = new List<Cell>();
        }
        
        public Cell[,] ToMatrix()
        {
            if (grid == null || grid.Count == 0)
                return new Cell[0, 0];

            int maxX = 0, maxY = 0;
            foreach (var cell in grid)
            {
                maxX = Mathf.Max(maxX, cell.Id.x);
                maxY = Mathf.Max(maxY, cell.Id.y);
            }

            var matrix = new Cell[maxX + 1, maxY + 1];
            foreach (var cell in grid)
            {
                matrix[cell.Id.x, cell.Id.y] = cell;
            }

            return matrix;
        } 
    }
}