using UnityEngine;

namespace _Scripts.Data
{
    [CreateAssetMenu(fileName = "UnitAttack", menuName = "Scriptable Objects/Buildings/Data/Unit Attack Data", order = 0)]
    public class UnitAttackData : ScriptableObject
    {
        [SerializeField] private bool attackEnemies;
        [SerializeField] private GameObject targetCastle;
        [SerializeField] private int damagePerHit;
        [SerializeField] private float attackInterval;
        [SerializeField] private float attackRadius;

        public bool AttackEnemies => attackEnemies;
        public GameObject TargetCastle => targetCastle;
        public int DamagePerHit => damagePerHit;
        public float AttackInterval => attackInterval;
        public float AttackRadius => attackRadius;
    }
}