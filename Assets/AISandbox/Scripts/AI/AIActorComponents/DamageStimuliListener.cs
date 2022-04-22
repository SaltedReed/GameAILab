using UnityEngine;
using GameAILab.Core;
using GameAILab.Perception;

namespace GameAILab.Sandbox
{

    public abstract class DamageStimuliListener : MonoBehaviour, IDamageStimuliListener
    {
        public virtual GameObject Go { get => gameObject; set { } }

        public virtual AffiliationType TargetAffiliations { get => m_targetAffiliation; set => m_targetAffiliation = value; }
        [SerializeField]
        protected AffiliationType m_targetAffiliation = AffiliationType.Friendly;

        public abstract void OnSenseUpdate(Stimuli stimuli);

    }

}