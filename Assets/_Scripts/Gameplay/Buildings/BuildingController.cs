using System;
using _Scripts.Data;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Gameplay.Buildings
{
    public class BuildingController: NetworkBehaviour, IHaveId
    {
        [SerializeField] private BuildingData data;
        
        
        private Grid grid;
        
        
        public BuildingData Data => data;
        public Sprite Sprite => GetComponent<SpriteRenderer>()?.sprite;
        public int Id { get; set; }
        
        private void Awake()
        {
            grid = FindFirstObjectByType<Grid>();
        }

        private void Start()
        {
            AdjustSize();
        }
            
        private void AdjustSize()
        {
            Vector2 targetSize = new Vector2(
                grid.cellSize.x * data.cellsize.x,
                grid.cellSize.y * data.cellsize.y
            );
            
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null && sr.sprite != null)
            {
                Vector2 spriteSize = sr.sprite.bounds.size;
                Vector3 newScale = new Vector3(
                    targetSize.x / spriteSize.x,
                    targetSize.y / spriteSize.y,
                    1f
                );

                transform.localScale = newScale;
            }

            BoxCollider2D box2D = GetComponent<BoxCollider2D>();
            if (box2D != null)
            {
                box2D.size = targetSize;
                box2D.offset = Vector2.zero;
            }
        }

    }
}