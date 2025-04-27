

using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.CustomInspector;
using _Scripts.CustomInspector.Button;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace _Scripts.TilemapGrid
{
    public class SymmetricTilemapCreator : NetworkBehaviour
    {
        [Header("Settings")]
        [Tooltip("Tilemaps like roads that rely on directional alignment should be flipped vertically.")]
        [SerializeField]
        private Tilemap[] tilemapsToFlip;

        [Tooltip("Enable debug gizmos for tile cell positions.")]
        [SerializeField]
        private bool shouldEnableGizmo = false;

        private Grid grid;
        private HighlightGridAreaController highlightGridController;
        private Tilemap[] tilemaps;
        private GameObject reflectedTilemapsParent;
        private List<Tilemap> reflectedTilemapsToDeny = new List<Tilemap>();

        private void OnValidate()
        {
            grid = GetComponent<Grid>();
            tilemaps = grid.GetComponentsInChildren<Tilemap>();
            highlightGridController = grid.GetComponent<HighlightGridAreaController>();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            grid = GetComponent<Grid>();
            highlightGridController = GetComponent<HighlightGridAreaController>();
            tilemaps = grid.GetComponentsInChildren<Tilemap>()
                .Where(tm => tm.transform.parent == grid.transform)
                .ToArray();

            CreateSymmetricalTilemaps();

            if (IsClient && !IsServer)
                SwapOriginalAndReflectedHierarchy();
        }


        [InspectorButton("Clear Symmetrical Tilemaps")]
        private void Clear()
        {
            var existing = grid.transform.Find("ReflectedTilemaps");
            if (existing != null)
                DestroyImmediate(existing.gameObject);

            CenterTilemaps();
        }

        [InspectorButton("Center Tilemaps")]
        private void CenterTilemaps()
        {
            var tilemaps = grid.GetComponentsInChildren<Tilemap>();
            foreach (var tilemap in tilemaps)
            {
                if (tilemap == null) continue;

                tilemap.CompressBounds();
                var bounds = tilemap.cellBounds;
                var centerOffset = new Vector3Int(
                    Mathf.FloorToInt(bounds.xMin + bounds.size.x / 2f),
                    Mathf.FloorToInt(bounds.yMin + bounds.size.y / 2f),
                    0);

                var tiles = new Dictionary<Vector3Int, TileBase>();
                var matrices = new Dictionary<Vector3Int, Matrix4x4>();

                foreach (var pos in bounds.allPositionsWithin)
                {
                    var tile = tilemap.GetTile(pos);
                    if (tile == null) continue;
                    tiles[pos] = tile;
                    matrices[pos] = tilemap.GetTransformMatrix(pos);
                }

                tilemap.ClearAllTiles();
                foreach (var kvp in tiles)
                {
                    var newPos = kvp.Key - centerOffset;
                    tilemap.SetTile(newPos, kvp.Value);
                    tilemap.SetTransformMatrix(newPos, matrices[kvp.Key]);
                }

                tilemap.transform.localPosition = Vector3.zero;
            }
        }

        [InspectorButton("Create Symmetrical Tilemaps")]
        private void CreateSymmetricalTilemaps()
        {
            // Remove any previous reflected maps
            var existing = grid.transform.Find("ReflectedTilemaps");
            if (existing != null)
                DestroyImmediate(existing.gameObject);

            CenterTilemaps();

            reflectedTilemapsParent = new GameObject("ReflectedTilemaps");
            reflectedTilemapsParent.transform.SetParent(grid.transform, worldPositionStays: true);

            var tilemaps = grid.GetComponentsInChildren<Tilemap>();
            int maxY = tilemaps.Max(tm => tm.cellBounds.yMax);
            int minY = tilemaps.Min(tm => tm.cellBounds.yMin);
            int totalHeight = maxY - minY;

            Vector3Int offsetDown = new Vector3Int(0, -totalHeight / 2, 0);
            Vector3Int offsetUp   = new Vector3Int(0,  totalHeight / 2, 0);
            int mirrorLineY = minY + maxY - 1;

            foreach (var original in tilemaps)
            {
                if (original == null) continue;
                
                    

                original.CompressBounds();
                var bounds = original.cellBounds;

                var data = new List<(Vector3Int pos, TileBase tile, Matrix4x4 mat)>();
                foreach (var pos in bounds.allPositionsWithin)
                {
                    var tile = original.GetTile(pos);
                    if (tile == null) continue;
                    data.Add((pos, tile, original.GetTransformMatrix(pos)));
                }

                original.ClearAllTiles();
                foreach (var (pos, tile, mat) in data)
                {
                    var newPos = pos + offsetDown;
                    original.SetTile(newPos, tile);
                    original.SetTransformMatrix(newPos, mat);
                }

                // Create reflected clone
                var go = new GameObject(original.name + "_Reflected");
                go.transform.SetParent(reflectedTilemapsParent.transform, worldPositionStays: true);

                var reflected = go.AddComponent<Tilemap>();
                var renderer = go.AddComponent<TilemapRenderer>();
                renderer.sortingOrder = original.GetComponent<TilemapRenderer>().sortingOrder;

                foreach (var (pos, tile, mat) in data)
                {
                    int mirroredY = mirrorLineY - pos.y;
                    var mirrorPos = new Vector3Int(pos.x, mirroredY, 0) + offsetUp;
                    reflected.SetTile(mirrorPos, tile);

                    var finalMat = mat;
                    if (tilemapsToFlip != null && tilemapsToFlip.Contains(original))
                        finalMat = Matrix4x4.Scale(new Vector3(1, -1, 1)) * mat;

                    reflected.SetTransformMatrix(mirrorPos, finalMat);
                }

                if (highlightGridController.TilemapsToDeny.Contains(original))
                    reflectedTilemapsToDeny.Add(reflected);
            }
        }

        /// <summary>
        /// Swaps the hierarchy so that what was original under grid
        /// moves to ReflectedTilemaps, and vice versa.
        /// </summary>
        private void SwapOriginalAndReflectedHierarchy()
        {
            if (reflectedTilemapsParent == null) return;
            Tilemap[] reflectedTilemaps = reflectedTilemapsParent.GetComponentsInChildren<Tilemap>();

            foreach (Tilemap tilemap in tilemaps)
            {
                tilemap.transform.SetParent(reflectedTilemapsParent.transform);
            }

            var newOriginalTilemaps = new List<Tilemap>();
            foreach (Tilemap tilemap in reflectedTilemaps)
            {
                tilemap.transform.SetParent(grid.transform);
                newOriginalTilemaps.Add(tilemap);
            }

            highlightGridController.UpdateTilemaps(newOriginalTilemaps, reflectedTilemapsToDeny);
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