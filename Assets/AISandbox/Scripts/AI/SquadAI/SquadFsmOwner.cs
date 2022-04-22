using UnityEngine;
using GameAILab.Decision.FSM;


namespace GameAILab.Sandbox
{


    public class SquadFsmOwner : StateMachineOwner
    {
        public string stateName_relaxed = "relaxed";
        public string stateName_combat = "combat";

        public override void OnAIUpdate(StateMachine fsm)
        {
            throw new System.NotImplementedException();
        }

        protected override StateMachine BuildUpFsm()
        {
            throw new System.NotImplementedException();
        }
    }

}