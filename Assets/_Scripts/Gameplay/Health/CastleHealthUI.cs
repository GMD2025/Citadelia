using System.Linq;
using _Scripts.CustomInspector.Button;
using _Scripts.Gameplay.Enemy;
using _Scripts.Utils;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Gameplay.Health
{
    public class CastleHealthUI : NetworkBehaviour
    {
        [SerializeField] private int numberOfSprites = 10;
        [SerializeField] private Sprite sprite;
        [SerializeField] private Vector2 spriteSize = new(32, 32);
        [SerializeField] private Vector2 spacing = new(2, 0);
        [SerializeField] private Vector2 padding = new(10, 10);
        [SerializeField, Min(0f)] private float backImageFill = 0.3f;

        private HealthController healthController;
        private RectTransform rect;
        private Image[] healthImages;

        public void SetHealthController(HealthController healthController)
        {
            this.healthController = healthController;
            GenerateUI();
        }

        private void GenerateUI()
        {
            rect = GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1);
            rect.anchorMax = new Vector2(0f, 1);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(0, -padding.y);
            RegenerateSprites(numberOfSprites);
            healthController.OnHealthChange += UpdateHealthUI;
            UpdateHealthUI(healthController.Health, healthController.MaxHealth);
        }

        public override void OnDestroy()
        {
            if (healthController != null)
                healthController.OnHealthChange -= UpdateHealthUI;
        }

        private void RegenerateSprites(int count)
        {
            for (int i = rect.childCount - 1; i >= 0; i--)
                Utils.Utils.SmartDestroy(rect.GetChild(i).gameObject);

            healthImages = new Image[count];
            
            // Calculate total width to help with center positioning
            float totalWidth = count * spriteSize.x + (count - 1) * spacing.x;
            float startX = -totalWidth / 2 + spriteSize.x / 2;

            for (int i = 0; i < count; i++)
            {
                GameObject container = new GameObject($"HealthSpriteContainer_{i}", typeof(RectTransform));
                container.transform.SetParent(rect, false);
                RectTransform containerRT = container.GetComponent<RectTransform>();
                containerRT.sizeDelta = spriteSize;
                // Center anchoring for containers
                containerRT.anchorMin = containerRT.anchorMax = new Vector2(0.5f, 0.5f);
                containerRT.pivot = new Vector2(0.5f, 0.5f);
                // Position relative to center
                containerRT.anchoredPosition = new Vector2(startX + i * (spriteSize.x + spacing.x), 0);

                GameObject backGO = new GameObject("Background", typeof(RectTransform), typeof(Image));
                backGO.transform.SetParent(container.transform, false);
                RectTransform backRT = backGO.GetComponent<RectTransform>();
                backRT.anchorMin = backRT.anchorMax = backRT.pivot = new Vector2(0.5f, 0.5f);
                backRT.sizeDelta = spriteSize;
                Image backImg = backGO.GetComponent<Image>();
                backImg.sprite = sprite;
                backImg.color = new Color(1, 1, 1, backImageFill);
                backImg.preserveAspect = true;

                GameObject frontGO = new GameObject("Fill", typeof(RectTransform), typeof(Image));
                frontGO.transform.SetParent(container.transform, false);
                RectTransform frontRT = frontGO.GetComponent<RectTransform>();
                frontRT.anchorMin = frontRT.anchorMax = frontRT.pivot = new Vector2(0.5f, 0.5f);
                frontRT.sizeDelta = spriteSize;
                Image frontImg = frontGO.GetComponent<Image>();
                frontImg.sprite = sprite;
                frontImg.type = Image.Type.Filled;
                frontImg.fillMethod = Image.FillMethod.Vertical;
                frontImg.fillOrigin = (int)Image.OriginVertical.Top;
                frontImg.fillAmount = 1f;
                frontImg.preserveAspect = true;

                healthImages[i] = frontImg;
            }

            rect.sizeDelta = new Vector2(
                totalWidth + padding.x * 2,
                spriteSize.y + padding.y * 2);
        }

        private void UpdateHealthUI(int current, int max)
        {
            if (healthImages == null || healthImages.Length == 0)
                return;

            float healthPerSprite = (float)max / healthImages.Length;
            for (int i = 0; i < healthImages.Length; i++)
            {
                float healthForThisSprite = current - (i * healthPerSprite);
                float fillAmount = Mathf.Clamp01(healthForThisSprite / healthPerSprite);
                healthImages[i].fillAmount = fillAmount;
            }
        }
    }
}