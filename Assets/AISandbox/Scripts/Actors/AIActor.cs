using UnityEngine;


namespace GameAILab.Sandbox
{

    public class AIActor : Actor
    {
        public virtual Vector3 BattlePoint
        {
            get => m_battlePoint;
            set => m_battlePoint = value;
        }
        protected Vector3 m_battlePoint;

        public Animator AnimController => m_animator;
        protected Animator m_animator;


        private void OnDrawGizmos()
        {
            // battle point --------------------------
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(BattlePoint, 0.5f);
        }

    }

}