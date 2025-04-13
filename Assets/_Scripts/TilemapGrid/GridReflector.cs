using System;
using System.Collections.Generic;
using _Scripts.CustomInspector.Button;
using UnityEngine;
using UnityEngine.Tilemaps;

// TODO: Deprecated

public class GridReflector : MonoBehaviour
{
    public Grid grid;

    private void OnValidate()
    {
        grid = GetComponent<Grid>();
    }

    [InspectorButton("reflect")]
    public void DuplicateAndMirrorTilemaps()
    {
        if (grid == null)
        {
            Debug.LogError("Grid is not assigned.");
            return;
        }
        
        // Create container for reflected tilemaps and add it to the grid.
        GameObject reflectedContainer = new GameObject("ReflectedTilemapsContainer");
        reflectedContainer.transform.SetParent(grid.transform, false);
        reflectedContainer.transform.position = grid.transform.position;
        reflectedContainer.transform.localRotation = Quaternion.Euler(0, 180, 180);

        // Now duplicate each child from the original list
        foreach (Transform child in grid.transform)
        {
            if (!child.GetComponent<Tilemap>())
                continue;
            
            GameObject duplicatedTilemap = Instantiate(child.gameObject, reflectedContainer.transform);
            duplicatedTilemap.name = child.gameObject.name + " Reflected";

            // Optional: Refresh tilemap colliders if needed.
            TilemapCollider2D tilemapCollider = duplicatedTilemap.GetComponent<TilemapCollider2D>();
            if (tilemapCollider != null)
            {
                tilemapCollider.enabled = false;
                tilemapCollider.enabled = true;
            }
        }
        RefreshColliders();
        Debug.Log("Duplicated and mirrored tilemaps successfully.");
    }

    [InspectorButton("refresh colliders")]
    private void RefreshColliders()
    {
        foreach (Transform child in grid.transform)
        {
            TilemapCollider2D tilemapCollider = child.GetComponent<TilemapCollider2D>();
            if (tilemapCollider != null)
            {
                tilemapCollider.enabled = false;
                tilemapCollider.enabled = true;
            }
        }
    }
}
