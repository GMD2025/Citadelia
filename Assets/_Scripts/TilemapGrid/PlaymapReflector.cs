

using System.Collections.Generic;
using System.Linq;
using _Scripts.CustomInspector;
using _Scripts.CustomInspector.Button;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Scripts.TilemapGrid
{
    public class PlaymapReflector : MonoBehaviour
    {
        [SerializeField, Tooltip("Tilemaps like roads that rely on directional alignment should be flipped vertically (Y axis) relative to each tile's pivot.")]
        private Tilemap[] tilemapsToFlip;        
        
        
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

        [InspectorLabel("Debug Buttons")]
        [InspectorButton("center tilemaps")]
        private void CenterTilemaps()
        {
            foreach (var tilemap in tilemaps)
            {
                if (tilemap == null) continue;

                tilemap.CompressBounds();

                var bounds = tilemap.cellBounds;
                var centerOffset = new Vector3Int(
                    Mathf.FloorToInt(bounds.xMin + bounds.size.x / 2f),
                    Mathf.FloorToInt(bounds.yMin + bounds.size.y / 2f),
                    0
                );

                var tiles = new Dictionary<Vector3Int, TileBase>();
                var transforms = new Dictionary<Vector3Int, Matrix4x4>();

                foreach (var pos in bounds.allPositionsWithin)
                {
                    var tile = tilemap.GetTile(pos);
                    if (tile != null)
                    {
                        tiles.Add(pos, tile);
                        transforms.Add(pos, tilemap.GetTransformMatrix(pos));
                    }
                }

                tilemap.ClearAllTiles();
                foreach (var kvp in tiles)
                {
                    var newPos = kvp.Key - centerOffset;
                    tilemap.SetTile(newPos, kvp.Value);
                    tilemap.SetTransformMatrix(newPos, transforms[kvp.Key]);
                }

                tilemap.transform.localPosition = Vector3.zero;
            }
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
                        
                        if(tilemapsToFlip.Contains(originalTilemap))
                            flipMatrix = Matrix4x4.Scale(new Vector3(1, -1, 1));
                        
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
            Transform existing = grid.transform.Find("ReflectedTilemaps");
            if (existing != null)
            {
                DestroyImmediate(existing.gameObject);
            }

            reflectedTilemapsParent = null;

            tilemaps?
                .Where(t => t != null)
                .ToList()
                .ForEach(t => t.transform.position = Vector3.zero);
        }
    }
}