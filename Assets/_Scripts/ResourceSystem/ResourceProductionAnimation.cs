using System;
using _Scripts.ResourceSystem.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.ResourceSystem
{
    public class ResourceProductionAnimation : MonoBehaviour
    {
        [SerializeField] private Image resourceIcon;
        [SerializeField] private TextMeshProUGUI amountText;

        public void SetResourceData(ResourceProductionData resource)
        {
            resourceIcon.sprite = resource.resourceType.icon;
            amountText.text =$"+{resource.productionAmount}";
        }
    }
}