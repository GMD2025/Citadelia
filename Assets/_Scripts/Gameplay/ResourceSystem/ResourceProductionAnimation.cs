using System;
using _Scripts.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Gameplay.ResourceSystem
{
    public class ResourceProductionAnimation : MonoBehaviour
    {
        [SerializeField] private Image resourceIcon;
        [SerializeField] private TextMeshProUGUI amountText;

        private Canvas canvas;

        public void SetResourceData(ResourceProductionData resource)
        {
            resourceIcon.sprite = resource.resourceType.icon;
            amountText.text =$"+{resource.productionAmount}";
        }
    }
}