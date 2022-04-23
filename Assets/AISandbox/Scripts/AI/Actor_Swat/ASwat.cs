using UnityEngine;
using GameAILab.Decision.FSM;


namespace GameAILab.Sandbox
{

    public class ASwat : AIActor
    {
        [Header("Animation")]
        public string animKey_moving = "moving";
        public string animKey_combat = "combat";
        public string animKey_hit = "hit";

        [Header("Movement")]
        public float runSpeed = 6;

        public Animator Anim => m_anim;
        private Animator m_anim;

        public Motor Movement => m_motor;
        private Motor m_motor;

        public Patrolman Patrol => m_patrol;
        private Patrolman m_patrol;

        public override Vector3 BattlePoint 
        { 
            get => base.BattlePoint;
            set
            {
                base.BattlePoint = value;
                IsBattlePointDirty = true;
            }
        }

        public bool IsBattlePointDirty { get; private set; }


        public void MoveToBattlePoint()
        {
            Movement.Speed = runSpeed;
            Movement.MoveTo(BattlePoint);
            IsBattlePointDirty = false;
        }


        protected override void Start()
        {
            base.Start();

            m_anim = GetComponentInChildren<Animator>(); Debug.Assert(m_anim != null);
            m_motor = GetComponent<Motor>();             Debug.Assert(m_motor != null);
            m_patrol = GetComponent<Patrolman>();        Debug.Assert(m_patrol != null);

            // StateMachineOwner.Start() requires basic components have finished their Start()s
            gameObject.AddComponent<SwatFsmOwner>();
        }

    }

}