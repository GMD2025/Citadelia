using _Scripts.Gameplay.Health;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Gameplay.Castle
{
    [RequireComponent(typeof(HealthController))]
    public class CastleController : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (!IsOwner) return;

            StartCoroutine(DelayedUIBind());
        }

        private System.Collections.IEnumerator DelayedUIBind()
        {
            // Wait a frame to ensure UI and health are synced
            yield return null;

            CastleHealthUI castleUi = FindObjectOfType<CastleHealthUI>();
            if (castleUi == null)
            {
                Debug.LogWarning("CastleHealthUI not found in scene!");
                yield break;
            }

            var health = GetComponent<HealthController>();
            if (health)
            {
                Debug.Log($"Binding CastleHealthUI to {gameObject.name}'s HealthController");
                castleUi.SetHealthController(health);
            }
        }
    }
}