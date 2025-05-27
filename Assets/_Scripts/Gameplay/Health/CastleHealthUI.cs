using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Gameplay.Health
{
    [RequireComponent(typeof(RectTransform))]
    public class CastleHealthUI : MonoBehaviour
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

        /// <summary>
        /// Call this once, passing in the enemy HealthController to bind.
        /// </summary>
        public void SetHealthController(HealthController hc)
        {
            // unsubscribe from previous
            if (healthController != null)
                healthController.Health.OnValueChanged -= OnHealthVarChanged;

            healthController = hc;
            if (healthController == null)
            {
                Debug.LogError("CastleHealthUI: no HealthController to bind.");
                return;
            }

            // build UI
            GenerateUI();

            // subscribe to network‐variable changes
            healthController.Health.OnValueChanged += OnHealthVarChanged;

            // initial fill
            UpdateHealthUI(healthController.Health.Value, healthController.MaxHealth);
        }

        private void OnDestroy()
        {
            if (healthController != null)
                healthController.Health.OnValueChanged -= OnHealthVarChanged;
        }

        // called by the NetworkVariable when health changes
        private void OnHealthVarChanged(int previous, int current)
        {
            UpdateHealthUI(current, healthController.MaxHealth);
        }

        private void GenerateUI()
        {
            rect = GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(padding.x, -padding.y);

            RegenerateSprites(numberOfSprites);
        }

        private void RegenerateSprites(int count)
        {
            // clear old
            for (int i = rect.childCount - 1; i >= 0; i--)
                Destroy(rect.GetChild(i).gameObject);

            healthImages = new Image[count];
            float totalWidth = count * spriteSize.x + (count - 1) * spacing.x;
            float startX = -totalWidth / 2 + spriteSize.x / 2;

            for (int i = 0; i < count; i++)
            {
                // container
                var container = new GameObject($"HealthSprite_{i}", typeof(RectTransform));
                container.transform.SetParent(rect, false);
                var rt = container.GetComponent<RectTransform>();
                rt.sizeDelta = spriteSize;
                rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = new Vector2(startX + i * (spriteSize.x + spacing.x), 0f);

                // background
                var bg = new GameObject("BG", typeof(Image));
                bg.transform.SetParent(container.transform, false);
                var bgImg = bg.GetComponent<Image>();
                bgImg.sprite = sprite;
                bgImg.color = new Color(1f, 1f, 1f, backImageFill);
                bgImg.preserveAspect = true;
                bg.GetComponent<RectTransform>().sizeDelta = spriteSize;

                // fill
                var fg = new GameObject("Fill", typeof(Image));
                fg.transform.SetParent(container.transform, false);
                var fgImg = fg.GetComponent<Image>();
                fgImg.sprite = sprite;
                fgImg.type = Image.Type.Filled;
                fgImg.fillMethod = Image.FillMethod.Vertical;
                fgImg.fillOrigin = (int)Image.OriginVertical.Top;
                fgImg.fillAmount = 1f;
                fgImg.preserveAspect = true;
                fg.GetComponent<RectTransform>().sizeDelta = spriteSize;

                healthImages[i] = fgImg;
            }

            rect.sizeDelta = new Vector2(totalWidth + padding.x * 2, spriteSize.y + padding.y * 2);
        }

        private void UpdateHealthUI(int current, int max)
        {
            if (healthImages == null || healthImages.Length == 0) return;

            float per = (float)max / healthImages.Length;
            for (int i = 0; i < healthImages.Length; i++)
            {
                float hpChunk = current - i * per;
                healthImages[i].fillAmount = Mathf.Clamp01(hpChunk / per);
            }
        }
    }
}
