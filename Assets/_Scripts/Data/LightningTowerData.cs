using UnityEngine;

namespace _Scripts.Data
{
    [CreateAssetMenu(fileName = "LightningTower", menuName = "Scriptable Objects/Buildings/Data/Lightning Mage Tower")]
    public class LightningTowerData : ScriptableObject
    {
        [Header("Detection")]
        public float detectionRadius = 5f;
        
        [Header("Timing & Cooldowns")]
        [Tooltip("Fire rate")]
        public float fireDelay = 2f;
        [Tooltip("Stun Duration")] 
        public float stunDuration = 0.3f;
        [Tooltip("Lightning destroy delay")]
        public float destroyDelay = 0.2f;
        [Tooltip("Strike per target cooldown")]
        public float strikeCooldownPerTarget = 0.2f;


        [Header("Biggest frame")] public Sprite biggestFrame;
        
        
        [Header("Lightning Projectile Properties")]
        public int damageOnHit = 30;

        public int maxActiveStrikes = 3;
    }
}