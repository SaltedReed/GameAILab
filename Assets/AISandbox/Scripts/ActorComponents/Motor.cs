using System;
using UnityEngine;

namespace GameAILab.Sandbox
{

    public abstract class Motor : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField]
        protected float m_reachRadius = 0.1f;
        [SerializeField]
        protected float m_speed;

        public virtual float ReachRadius { get => m_reachRadius; set => m_reachRadius = value; }
        public virtual float Speed { get => m_speed; set => m_speed=value; }
        public virtual bool IsStopped { get; }

        public abstract void MoveTo(Vector3 destination);
        public abstract void Stop();

        public bool IsAt(Vector3 pos)
        {
            float sqrDst = Vector3.SqrMagnitude(transform.position - pos);
            return sqrDst < ReachRadius * ReachRadius;
        }
    }

}