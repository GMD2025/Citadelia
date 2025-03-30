using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.ResourceSystem.Data;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.ResourceSystem
{
    public class ResourceUIManager : MonoBehaviour
    {
        [SerializeField]
        private ResourceProductionServiceRefData resourceProdServiceRefData;
        private List<ResourceUI> resourceUIs;

        private ResourceProductionService resourceProdService;
        private void Start()
        {
            resourceProdService = resourceProdServiceRefData.Get;
            resourceUIs = GetComponentsInChildren<ResourceUI>(includeInactive: true).ToList();
            resourceProdService.OnResourceChanged += HandleResourceChanged;
        }

        private void HandleResourceChanged(ResourceData resourceData, int newAmount)
        {
            var ui = resourceUIs.FirstOrDefault(ui => ui.ResourceData == resourceData);
            ui?.UpdateUI(newAmount);
        }

        private void OnDestroy()
        {
            resourceProdService.OnResourceChanged -= HandleResourceChanged;
        }
    }
}