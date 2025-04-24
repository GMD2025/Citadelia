using System.Collections.Generic;
using System.Linq;
using _Scripts.Data;
using UnityEngine;

namespace _Scripts.Gameplay.ResourceSystem
{
    public class ResourceUIManager : MonoBehaviour
    {
        private List<ResourceUI> resourceUIs;

        private ResourceProductionService resourceProdService;
        private void Start()
        {
            resourceProdService = DependencyContainer.Instance.Resolve<ResourceProductionService>();
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