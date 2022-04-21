using UnityEngine;
using GameAILab.Core;

namespace GameAILab.Sandbox
{

    public class Actor : MonoBehaviour, IActor
    {
        public int Id { get; set; }
        public virtual GameObject Go { get => gameObject; set { } }

        public virtual AffiliationType Affiliation { get => m_affiliation; set => m_affiliation = value; }
        [SerializeField]
        protected AffiliationType m_affiliation;

        protected virtual void Start()
        {
            ActorSystem actorSystem = Game.Instance.ActorSys;
            actorSystem.Register(this);
        }

        protected virtual void OnDestroy()
        {
            ActorSystem actorSystem = Game.Instance.ActorSys;
            actorSystem.Unregister(this);
        }
    }

}