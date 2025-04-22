using UnityEngine;

namespace _Scripts.Gameplay.Buildings.Systems.Flame.Data
{
    [CreateAssetMenu(fileName = "FlameTower", menuName = "Scriptable Objects/Buildings/Data/Flame Mage Tower")]
    public class FlameTowerData : ScriptableObject
    {
        [Header("Detection")]
        public float detectionRadius = 5f;
        
        [Header("Delay System")]
        [Tooltip("Fire rate")]
        public float fireDelay = 2f;
        [Tooltip("Time that it takes to create a new flame after firing and old one")]
        public float flameRestoreDelay = 10f;
        [Tooltip("Time which the flame is inactive right after instantiating")]
        public float cooldownAfterRestore = 0.5f;
        
        [Header("Flame Projectile Properties")]
        public float speed = 10f;
        public int damageOnHit = 50;
    }
}