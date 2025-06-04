using System.Collections;
using _Scripts.Gameplay.Health;
using _Scripts.Gameplay.Units;
using Unity.Netcode;
using UnityEngine;

namespace _Scripts.Gameplay.Castle
{
    [RequireComponent(typeof(HealthController), typeof(Team))]
    public class CastleController : NetworkBehaviour
    {
        private Team team;
        private bool bound;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (!IsClient) 
                return;

            team = GetComponent<Team>();

            team.TeamId.OnValueChanged += OnTeamIdChanged;
        }

        private void OnTeamIdChanged(int previous, int current)
        {
            if (bound) return;

            if (current < 0) return;

            if (current == (int)NetworkManager.Singleton.LocalClientId)
            {
                StartCoroutine(DelayedUIBind());
                bound = true;
            }
            team.TeamId.OnValueChanged -= OnTeamIdChanged;
        }

        private IEnumerator DelayedUIBind()
        {
            yield return null;

            var ui = FindObjectOfType<CastleHealthUI>();
            if (!ui)
            {
                Debug.LogWarning("[CastleController] CastleHealthUI not found!");
                yield break;
            }

            var hc = GetComponent<HealthController>();
            ui.SetHealthController(hc);
            Debug.Log(
                $"[Client:{NetworkManager.Singleton.LocalClientId}] " +
                $"Bound HUD to local castle (Team {team.TeamId.Value})"
            );
        }
    }
}