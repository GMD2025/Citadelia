using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Gameplay.Health
{
    [RequireComponent(typeof(HealthController))]
    public class HealthUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private RectMask2D mask;

        private HealthController controller;

        private void Awake()
        {
            controller = GetComponent<HealthController>();
            controller.OnHealthChange += UpdateUI;
        }

        private void OnDestroy()
        {
            controller.OnHealthChange -= UpdateUI;
        }

        private void UpdateUI(int current, int max)
        {
            if (text != null)
                text.text = $"{current} / {max}";

            if (mask != null)
            {
                float width = mask.rectTransform.rect.width;
                mask.padding = new Vector4(0, 0, (1f - (float)current / max) * width, 0);
            }
        }
    }
}