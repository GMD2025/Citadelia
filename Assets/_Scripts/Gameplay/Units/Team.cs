using Unity.Netcode;

namespace _Scripts.Gameplay.Units
{
    public class Team : NetworkBehaviour
    {
        public NetworkVariable<int> TeamId = new(-1,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

        public void SetTeam(int team)
        {
            if (IsServer)
                TeamId.Value = team;
        }
    }
}