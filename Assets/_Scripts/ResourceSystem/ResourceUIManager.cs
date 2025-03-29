using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace _Scripts.ResourceSystem
{
    public class ResourceUIManager : MonoBehaviour
    {
        [SerializeField]
        private ResourceProductionServiceSO resourceProdServiceSo;
        private List<ResourceUI> resourceUIs;

        private ResourceProductionService resourceProdService;
        private void Start()
        {
            resourceProdService = resourceProdServiceSo.Get;
            resourceUIs = GetComponentsInChildren<ResourceUI>(includeInactive: true).ToList();
            resourceProdService.OnResourceChanged += HandleResourceChanged;
        }

        private void HandleResourceChanged(ResourceSO resourceSo, int newAmount)
        {
            var ui = resourceUIs.FirstOrDefault(ui => ui.ResourceSo == resourceSo);
            ui?.UpdateUI(newAmount);
        }

        private void OnDestroy()
        {
            resourceProdService.OnResourceChanged -= HandleResourceChanged;
        }
    }
}