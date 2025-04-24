using System.Linq;
using _Scripts.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Gameplay.ResourceSystem
{
    public class ResourceUI : MonoBehaviour
    {
        [SerializeField] private ResourceData resourceData;
        
        private TextMeshProUGUI amountText;
        private Image resourceImg; 
        
        public ResourceData ResourceData => resourceData;

        private void Awake()
        {
            amountText = GetComponentInChildren<TextMeshProUGUI>();
            resourceImg = GetComponentsInChildren<Image>()
                .FirstOrDefault(img => img.gameObject.name == "ResourceImg");
        }

        private void Start()
        {
            resourceImg.sprite = resourceData.icon;
            amountText.text = $"{resourceData.initialAmount}";
        }

        public void UpdateUI(int currentAmount)
        {
            amountText.text = $"{currentAmount}";
        }
    }
}