using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.CustomInspector;
using _Scripts.CustomInspector.Button;
using NavMeshPlus.Components;
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

        [Tooltip("Enable debug gizmos for tile cell positions.")] [SerializeField]
        private bool shouldEnableGizmo = false;

        private Grid grid;
        private HighlightGridAreaController highlightGridController;
        private Tilemap[] tilemaps;
        private GameObject reflectedTilemapsParent;
        private List<Tilemap> reflectedTilemapsToDeny = new List<Tilemap>();
        private NavMeshSurface navMeshSurface;

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
            navMeshSurface = FindFirstObjectByType<NavMeshSurface>();

            CreateSymmetricalTilemaps();

            if (IsClient && !IsServer)
                SwapOriginalAndReflectedHierarchy();

            navMeshSurface.BuildNavMeshAsync();
        }


        [InspectorButton("Clear Symmetrical Tilemaps")]
        private void Clear()
        {
            var existing = grid.transform.Find("ReflectedTilemaps");
            if (existing != null)
                DestroyImmediate(existing.gameObject);

            // CenterTilemaps();
        }

        private void CenterTilemap(Tilemap tilemap, Vector3Int centerOffset)
        {
            tilemap.CompressBounds();
            var tilesData = this.tilesData(tilemap);
            tilemap.ClearAllTiles();
            foreach (var kvp in tilesData)
            {
                var newPos = kvp.Key - centerOffset;
                tilemap.SetTile(newPos, kvp.Value);
            }

            tilemap.transform.localPosition = Vector3.zero;
        }

        private void ShiftTilemap(Tilemap tilemap, Vector3Int offset)
        {
            var tilesData = this.tilesData(tilemap);
            tilemap.ClearAllTiles();
            foreach (var (pos, tile) in tilesData)
            {
                var newPos = pos + offset;
                tilemap.SetTile(newPos, tile);
            }
        }

        private Vector3Int CenterOffset()
        {
            // Handle ground layer as the superset. Calculate Vector t omove ground layer, all others repear the same movement.
            tilemaps[0].CompressBounds();
            var bounds = tilemaps[0].cellBounds;
            return new Vector3Int(
                Mathf.FloorToInt(bounds.xMin + bounds.size.x / 2f),
                Mathf.FloorToInt(bounds.yMin + bounds.size.y / 2f),
                0);
        }

        private Dictionary<Vector3Int, TileBase> tilesData(Tilemap tilemap)
        {
            var data = new Dictionary<Vector3Int, TileBase>();
            foreach (var pos in tilemap.cellBounds.allPositionsWithin)
            {
                var tile = tilemap.GetTile(pos);
                if (tile == null) continue;
                data[pos] = tile;
            }
            
            return data;
        }

        private void CreateSymmetricalTilemaps()
        {
            Vector3Int centerOffset = CenterOffset();
            foreach (var tilemap in tilemaps)
            {
                CenterTilemap(tilemap, centerOffset);
            }
            reflectedTilemapsParent = new GameObject("ReflectedTilemaps");
            reflectedTilemapsParent.transform.SetParent(grid.transform, worldPositionStays: true);

            if (tilemaps.Length == 0) return;

            int maxY = tilemaps[0].cellBounds.yMax;
            int minY = tilemaps[0].cellBounds.yMin;

            int halfHeight = Mathf.CeilToInt((maxY - minY) / 2f);
            int mirrorLineY = minY + maxY - 2;
                
            Debug.Log("MIAXY is " + maxY);
            Vector3Int offsetDown = new Vector3Int(0, -halfHeight, 0); // Shift originals downward
            Vector3Int offsetUp = new Vector3Int(0, +halfHeight, 0); // Shift mirrors upward
            
            foreach (var original in tilemaps)
            {
                ShiftTilemap(original, offsetDown);
                
                // Create reflected clone
                var reflectedGameObject = Instantiate(original.gameObject, reflectedTilemapsParent.transform);
                reflectedGameObject.name = original.name + "_Reflected";
                
                var reflected = reflectedGameObject.GetComponent<Tilemap>();
                
                reflected.ClearAllTiles();
                
                foreach (var (pos, tile) in tilesData(original))
                {
                    int mirroredY = mirrorLineY - pos.y;
                    Debug.Log(mirrorLineY + " Mirrored");
                    var mirrorPos = new Vector3Int(pos.x, mirroredY, 0);
                
                    reflected.SetTile(mirrorPos, tile);
                
                    var finalMat = original.GetTransformMatrix(pos);
                    if (tilemapsToFlip != null && tilemapsToFlip.Contains(original))
                        finalMat = Matrix4x4.Scale(new Vector3(1, -1, 1)) * finalMat;
                
                    reflected.SetTransformMatrix(mirrorPos, finalMat);
                }
                
                if (highlightGridController.TilemapsToDeny.Contains(original))
                    reflectedTilemapsToDeny.Add(reflected);
            }
        }

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

            float dotSize = 0.2f;

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
                    Gizmos.DrawSphere(worldPos, dotSize * 1.5f);
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