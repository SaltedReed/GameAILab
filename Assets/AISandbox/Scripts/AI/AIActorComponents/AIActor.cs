using UnityEngine;
using GameAILab.Core;


namespace GameAILab.Sandbox
{

    public class AIActor : Actor
    {
        public virtual IActor Target { get; set; }

        public virtual Vector3 BattlePoint
        {
            get => m_battlePoint;
            set => m_battlePoint = value;
        }
        protected Vector3 m_battlePoint;

        public AISquad Squad { get; set; }


        private void OnDrawGizmos()
        {
            // battle point --------------------------
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(BattlePoint, 0.5f);
        }

    }

}