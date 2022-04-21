using UnityEngine;
using GameAILab.Decision.FSM;


namespace GameAILab.Sandbox
{

    public abstract class StateMachineOwner<T> : MonoBehaviour
    {
        public abstract void OnAIUpdate(StateMachine<T> fsm);
    }

}