using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.ResourceSystem
{
    public class ResourceUI : MonoBehaviour
    {
        [SerializeField] private ResourceSO resourceSo;
        
        private TextMeshProUGUI amountText;
        private Image resourceImg; 
        
        public ResourceSO ResourceSo => resourceSo;

        private void Awake()
        {
            amountText = GetComponentInChildren<TextMeshProUGUI>();
            resourceImg = GetComponentsInChildren<Image>()
                .FirstOrDefault(img => img.gameObject.name == "ResourceImg");
        }

        private void Start()
        {
            resourceImg.sprite = resourceSo.icon;
            amountText.text = $"{resourceSo.initialAmount}";
        }

        public void UpdateUI(int currentAmount)
        {
            amountText.text = $"{currentAmount}";
        }
    }
}