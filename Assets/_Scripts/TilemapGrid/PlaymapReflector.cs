using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.CustomInspector.Button;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Scripts.TilemapGrid
{
    public class PlaymapReflector : MonoBehaviour
    {
        private Grid grid;
        private Tilemap[] tilemaps;
        private List<Tilemap> reflectedTilemaps;

        private Vector3Int positionOffset;

        private void Awake()
        {
            grid = GetComponent<Grid>();
            tilemaps = grid.GetComponentsInChildren<Tilemap>().Where(t => !t.name.Contains("_Reflected")).ToArray();
            positionOffset = Vector3Int.zero;
            positionOffset.y = tilemaps.OrderBy(tilemap => tilemap.size.y).Select(t => t.cellBounds.size.y).First();
        }

        private void Start()
        {
            Reflect();
        }

        [InspectorButton("reflect")]
        private void Reflect()
        {
            tilemaps = grid.GetComponentsInChildren<Tilemap>().Where(t => !t.name.Contains("_Reflected")).ToArray();
    
            if (reflectedTilemaps == null) reflectedTilemaps = new List<Tilemap>();
            reflectedTilemaps.Clear();

            int maxY = tilemaps.Max(tm => tm.cellBounds.yMax);
            int minY = tilemaps.Min(tm => tm.cellBounds.yMin);
            int totalHeight = maxY - minY;
            var positionOffset = new Vector3Int(0, totalHeight, 0);

            foreach (var originalTilemap in tilemaps)
            {
                if (originalTilemap == null) continue;

                var reflectedTilemapGo = new GameObject($"{originalTilemap.name}_Reflected");
                reflectedTilemapGo.transform.SetParent(grid.transform, false);

                var newTilemap = reflectedTilemapGo.AddComponent<Tilemap>();
                var newRenderer = reflectedTilemapGo.AddComponent<TilemapRenderer>();
                newRenderer.sortingOrder = originalTilemap.GetComponent<TilemapRenderer>().sortingOrder;

                originalTilemap.CompressBounds();
                var bounds = originalTilemap.cellBounds;
                int mirrorLineY = minY + maxY - 1;

                for (int x = bounds.xMin; x < bounds.xMax; x++)
                {
                    for (int y = bounds.yMin; y < bounds.yMax; y++)
                    {
                        var originalPos = new Vector3Int(x, y, 0);
                        var tile = originalTilemap.GetTile(originalPos);
                        if (tile == null) continue;

                        int mirroredY = mirrorLineY - y;
                        var mirroredPos = new Vector3Int(x, mirroredY, 0);

                        newTilemap.SetTile(mirroredPos + positionOffset, tile);

                        var flipMatrix = Matrix4x4.identity;
                        newTilemap.SetTransformMatrix(mirroredPos + positionOffset, flipMatrix);
                    }
                }

                reflectedTilemaps.Add(newTilemap);
            }
        }

        [InspectorButton("clear")]
        private void Clear()
        {
            foreach (var reflected in reflectedTilemaps)
            {
                if (reflected != null)
                {
                    Utils.Utils.SmartDestroy(reflected.gameObject);
                }
            }

            reflectedTilemaps.Clear();
        }

    }
}