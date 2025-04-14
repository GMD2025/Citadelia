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

        private GameObject reflectedTilemapsParent;

        private void OnValidate()
        {
            grid = GetComponent<Grid>();
            tilemaps = grid.GetComponentsInChildren<Tilemap>();
        }

        private void Start()
        {
            Reflect();
        }

        [InspectorButton("reflect")]
        private void Reflect()
        {
            Clear();
            reflectedTilemapsParent = new GameObject("ReflectedTilemaps");
            reflectedTilemapsParent.transform.SetParent(grid.transform);
            tilemaps = grid.GetComponentsInChildren<Tilemap>();

            int maxY = tilemaps.Max(tm => tm.cellBounds.yMax);
            int minY = tilemaps.Min(tm => tm.cellBounds.yMin);
            int totalHeight = maxY - minY;
            var positionOffset = new Vector3Int(0, totalHeight / 2, 0);

            foreach (var originalTilemap in tilemaps)
            {
                if (originalTilemap == null) continue;

                var reflectedTilemapGo = new GameObject($"{originalTilemap.name}_Reflected");
                reflectedTilemapGo.transform.SetParent(reflectedTilemapsParent.transform);

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

                        newTilemap.SetTile(mirroredPos, tile);

                        var flipMatrix = Matrix4x4.identity;
                        newTilemap.SetTransformMatrix(mirroredPos, flipMatrix);
                    }
                }
                Vector3 worldOffset = Vector3.Scale(positionOffset, grid.cellSize);
                originalTilemap.transform.position -= worldOffset;
                newTilemap.transform.position += worldOffset;
            }
        }

        [InspectorButton("clear")]
        private void Clear()
        {
            if (reflectedTilemapsParent != null) Utils.Utils.SmartDestroy(reflectedTilemapsParent);
            tilemaps?
                .Where(t => t)
                .ToList()
                .ForEach(t => t.transform.position = Vector3.zero);

        }
    }
}