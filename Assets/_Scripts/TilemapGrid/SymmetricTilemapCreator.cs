

using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.CustomInspector;
using _Scripts.CustomInspector.Button;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Scripts.TilemapGrid
{
    // P.S.
    // I fucking hate the tile system in Unity, it sucks so much
    // if you see the tile somewhere drawn it doesn't always mean that it's actual position matches visual placement of the tile
    // if you want to change the position of the tilemap this means you MUST change each single tile position.
    // Simply moving the tilemap will only change the visual part of the tilemap engine.
    // by the link below you can view how the position (drawn by gizmo) differs from actual tilebase placement
    // (the green dots are hidden under magenta, because they overlap)
    // https://docs.google.com/presentation/d/1V88n3ZTK4adhwx3VvxhSGTsAOh5vA7dBGUmfeR3YjOU/edit?usp=sharing
    public class SymmetricTilemapCreator : MonoBehaviour
    {
        [SerializeField, Tooltip("Tilemaps like roads that rely on directional alignment should be flipped vertically (Y axis) relative to each tile's pivot.")]
        private Tilemap[] tilemapsToFlip;

        [SerializeField] private bool shouldEnableGizmo = false;
        
        
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
            grid = GetComponent<Grid>();
            tilemaps = grid.GetComponentsInChildren<Tilemap>();
            CreateSymmetricalTilemaps();
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

        [InspectorButton("Create Symmetrical Tilemaps")]
        private void CreateSymmetricalTilemaps()
        {
            Clear();
            CenterTilemaps();

            reflectedTilemapsParent = new GameObject("ReflectedTilemaps");
            reflectedTilemapsParent.transform.SetParent(grid.transform);

            tilemaps = grid.GetComponentsInChildren<Tilemap>();
            int maxY = tilemaps.Max(tm => tm.cellBounds.yMax);
            int minY = tilemaps.Min(tm => tm.cellBounds.yMin);
            int totalHeight = maxY - minY;

            Vector3Int cellOffsetDown = new Vector3Int(0, -(totalHeight / 2), 0); 
            Vector3Int cellOffsetUp   = new Vector3Int(0,   totalHeight / 2, 0);

            int mirrorLineY = minY + maxY - 1;

            foreach (var originalTilemap in tilemaps)
            {
                if (originalTilemap == null) continue;

                originalTilemap.CompressBounds();
                var bounds = originalTilemap.cellBounds;

                var tileDataList = new List<(Vector3Int pos, TileBase tile, Matrix4x4 matrix)>();
                // persist current tiles state, pos, tilebase and matrix 4x4 (needed for scaling)
                foreach (var pos in bounds.allPositionsWithin)
                {
                    TileBase tile = originalTilemap.GetTile(pos);
                    if (tile == null) continue;

                    Matrix4x4 mat = originalTilemap.GetTransformMatrix(pos);
                    tileDataList.Add((pos, tile, mat));
                }

                // delete all tiles and put them back on the new pos (with -offset)
                originalTilemap.ClearAllTiles();
                foreach (var (pos, tile, mat) in tileDataList)
                {
                    Vector3Int newPos = pos + cellOffsetDown;
                    originalTilemap.SetTile(newPos, tile);
                    originalTilemap.SetTransformMatrix(newPos, mat);
                }

                GameObject reflectedGo = new GameObject(originalTilemap.name + "_Reflected");
                reflectedGo.transform.SetParent(reflectedTilemapsParent.transform);

                Tilemap reflectedTilemap = reflectedGo.AddComponent<Tilemap>();
                TilemapRenderer reflectedRenderer = reflectedGo.AddComponent<TilemapRenderer>();
                reflectedRenderer.sortingOrder =
                    originalTilemap.GetComponent<TilemapRenderer>().sortingOrder;

                // draw tiles with changing the order of rows + offset (with -offset)
                foreach (var (pos, tile, mat) in tileDataList)
                {
                    int mirroredY = mirrorLineY - pos.y;
                    Vector3Int mirroredPos = new Vector3Int(pos.x, mirroredY, 0);

                    mirroredPos += cellOffsetUp;

                    reflectedTilemap.SetTile(mirroredPos, tile);

                    // if the tilemap need each tile flipping, it applies here
                    Matrix4x4 finalMatrix = mat;
                    if (tilemapsToFlip != null && tilemapsToFlip.Contains(originalTilemap))
                    {
                        Matrix4x4 flip = Matrix4x4.Scale(new Vector3(1, -1, 1));
                        finalMatrix = flip * finalMatrix;
                    }
                    reflectedTilemap.SetTransformMatrix(mirroredPos, finalMatrix);
                }
            }
        }


        [InspectorButton("Clear Symmetrical Tilemaps")]
        private void Clear()
        {
            Transform existing = grid.transform.Find("ReflectedTilemaps");
            if (existing != null)
            {
                DestroyImmediate(existing.gameObject);
            }

            reflectedTilemapsParent = null;
            CenterTilemaps();
        }
        
        private void OnDrawGizmosSelected()
        {
            if (!shouldEnableGizmo)
                return;
            
            if (tilemaps == null || grid == null) return;

            float dotSize = 0.1f;

            foreach (var tilemap in tilemaps)
            {
                if (tilemap == null) continue;

                Gizmos.color = Color.red;

                tilemap.CompressBounds();
                var bounds = tilemap.cellBounds;

                foreach (var pos in bounds.allPositionsWithin)
                {
                    if (!tilemap.HasTile(pos)) continue;

                    Vector3 worldPos = grid.CellToWorld(pos) + grid.cellSize / 2f;
                    Gizmos.DrawSphere(worldPos, dotSize * 3);
                }
            }

            if (reflectedTilemapsParent != null)
            {
                var reflectedTilemaps = reflectedTilemapsParent.GetComponentsInChildren<Tilemap>();
                Gizmos.color = Color.cyan;

                foreach (var tilemap in reflectedTilemaps)
                {
                    if (tilemap == null) continue;

                    tilemap.CompressBounds();
                    var bounds = tilemap.cellBounds;

                    foreach (var pos in bounds.allPositionsWithin)
                    {
                        if (!tilemap.HasTile(pos)) continue;

                        Vector3 worldPos = grid.CellToWorld(pos) + grid.cellSize / 2f;
                        Gizmos.DrawSphere(worldPos, dotSize);
                    }
                }
            }
        }
    }
}