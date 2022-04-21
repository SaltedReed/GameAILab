using UnityEngine;
using GameAILab.Core;
using GameAILab.Perception;

namespace GameAILab.Sandbox
{


    public abstract class SightStimuliListener : MonoBehaviour, ISightStimuliListener
    {
        public virtual GameObject Go { get => gameObject; set { } }

        public virtual AffiliationType TargetAffiliations { get => m_targetAffiliation; set => m_targetAffiliation = value; }
        [SerializeField]
        protected AffiliationType m_targetAffiliation = AffiliationType.Friendly;

        public virtual float HalfAngleDegrees { get => m_halfAngleDegrees; set => m_halfAngleDegrees = value; }
        [SerializeField]
        protected float m_halfAngleDegrees = 30;

        public virtual float Radius { get => m_radius; set => m_radius = value; }
        [SerializeField]
        protected float m_radius = 10;

        public virtual float Height { get => m_height; set => m_height = value; }
        [SerializeField]
        protected float m_height = 2;

        public abstract void OnSenseUpdate(Stimuli stimuli);

        protected virtual void OnDrawGizmos()
        {
            // sight cone ---------------------------
            Gizmos.color = Color.yellow;

            Vector3 pos = transform.position;
            Vector3 left = Quaternion.AngleAxis(-HalfAngleDegrees, Vector3.up) * transform.forward;
            Vector3 right = Quaternion.AngleAxis(HalfAngleDegrees, Vector3.up) * transform.forward;

            Gizmos.DrawRay(pos, left * Radius);
            Gizmos.DrawRay(pos, right * Radius);

            Gizmos.DrawRay(pos + new Vector3(0, Height, 0), left * Radius);
            Gizmos.DrawRay(pos + new Vector3(0, Height, 0), right * Radius);
        }
    }

}