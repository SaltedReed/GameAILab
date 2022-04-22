using System;
using UnityEngine;

namespace GameAILab.Sandbox
{

    public class Damageable : MonoBehaviour
    {
        [Header("Health")]
        public float maxHealth = 100;

        public float Health
        {
            get => m_health;
            set
            {
                float old = m_health;
                m_health = Mathf.Clamp(value, 0.0f, maxHealth);

                onHealthChange?.Invoke(gameObject, old, m_health);

                if (m_health <= 0.0f)
                    onDie?.Invoke(gameObject);
            }
        }
        private float m_health;

        public bool IsDead => Health <= 0.0f;

        public Action<GameObject, float, float> onHealthChange;
        public Action<GameObject> onDie;


        protected virtual void Awake()
        {
            Health = maxHealth;
        }
    }

}