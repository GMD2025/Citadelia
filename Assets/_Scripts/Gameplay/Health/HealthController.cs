using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Gameplay.Health
{
    public class HealthController : MonoBehaviour
    {
        [SerializeField] private int startingHealth = 100;
        public int StartingHealth => startingHealth;
        public int Health => health;
        private int health;

        public event Action OnDied;
        public event Action OnHealthChange;

        private void Awake()
        {
            health = startingHealth;
            TextMeshProUGUI text = GetComponentInChildren<TextMeshProUGUI>();
            RectMask2D mask = GetComponentInChildren<RectMask2D>();

            if (text != null)
            {
                AdaptTextToHealth(text);
                OnHealthChange += () => AdaptTextToHealth(text);
                OnHealthChange += () => AdaptMask(mask);
            }

            StartCoroutine(startTaking());
        }

        IEnumerator startTaking()
        {
            while (true)
            {
                yield return new WaitForSeconds(3f);
                TakeDamage(10);
            }
        }

        public void TakeDamage(int amount)
        {
            health -= amount;
            OnHealthChange?.Invoke();

            if (health <= 0)
            {
                Die();
            }
        }


        private void Die()
        {
            OnDied?.Invoke();
            Destroy(gameObject);
        }

        public void HealBy(int amount)
        {
            health = Mathf.Clamp(health + amount, 0, startingHealth);
            OnHealthChange?.Invoke();
        }

        private void AdaptTextToHealth(TextMeshProUGUI text)
        {
            text.text = $"{health.ToString()} / {startingHealth.ToString()}";
        }
        private void AdaptMask(RectMask2D mask)
        {
            float width = mask.rectTransform.rect.width;
            mask.padding = new Vector4(0,0,(1 - health/startingHealth) * width, 0);
        }
    }
}