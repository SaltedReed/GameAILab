using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameAILab.Sandbox
{

    [RequireComponent(typeof(Collider))]
    public class Projectile : MonoBehaviour
    {
        public float speed = 10f;
        public float lifetime = 5f;

        public Action<HitResult> onHit;


        protected virtual void Update()
        {
            transform.Translate(transform.up * speed * Time.deltaTime, Space.World);
        }

        protected virtual void OnCollisionEnter(Collision other)
        {
            if (onHit != null)
            {
                ContactPoint cp = other.GetContact(0);
                HitResult result = new HitResult
                {
                    collider = cp.otherCollider,
                    point = cp.point,
                    normal = cp.normal
                };
                onHit.Invoke(result);
            }
        }
    }

}