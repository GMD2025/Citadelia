using System.Linq;
using _Scripts.Gameplay.Health;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Gameplay.Castle
{
    [RequireComponent(typeof(HealthController))]
    public class CastleController : NetworkBehaviour
    {
        private void Start()
        {
            var castleUi = FindObjectsByType<CastleHealthUI>(FindObjectsSortMode.None).FirstOrDefault();
            if (!castleUi)
            {
                Debug.Log("No castle ui");
                return;
            }
            castleUi.SetHealthController(GetComponent<HealthController>());
        }
    }
}