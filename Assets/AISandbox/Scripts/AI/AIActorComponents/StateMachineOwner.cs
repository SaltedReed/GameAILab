using UnityEngine;
using GameAILab.Decision.FSM;


namespace GameAILab.Sandbox
{

    public abstract class StateMachineOwner : MonoBehaviour
    {
        public bool registerOnPawn = true;

        // write state transition code here
        public abstract void OnAIUpdate(StateMachine fsm);

        protected abstract StateMachine BuildUpFsm();

        protected virtual void Start()
        {
            if (registerOnPawn)
            {
                StateMachine fsm = BuildUpFsm();
                Game.Instance.AISys.RegisterFsm(this, fsm);
            }
        }

        protected virtual void OnDestroy()
        {
            Game.Instance.AISys.UnregisterFsm(this);
        }

    }

}