using System;
using UnityEngine;
using UnityEngine.AI;

namespace GameAILab.Sandbox
{

    public class NavMeshAgentMotor : Motor
    {
        public override float Speed { get => m_navAgent.speed; set => m_navAgent.speed = value; }
        public override bool IsStopped => m_navAgent.isStopped;

        [SerializeField]
        protected NavMeshAgent m_navAgent;

        public override void MoveTo(Vector3 destination)
        {
            if (m_navAgent.isStopped)
            {
                m_navAgent.isStopped = false;
            }
            m_navAgent.destination = destination;
        }

        public override void Stop()
        {
            if (!m_navAgent.isActiveAndEnabled || m_navAgent.isStopped)
                return;

            m_navAgent.isStopped = true;
        }

    }

}