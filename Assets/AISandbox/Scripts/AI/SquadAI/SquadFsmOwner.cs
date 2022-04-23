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
            
        }

        protected override StateMachine BuildUpFsm()
        {
            AISquad squad = GetComponent<AISquad>();

            StateMachineBuilder builder = new StateMachineBuilder();
            StateMachine fsm = builder.StateMachine("aisquad fsm", gameObject)
                                          .DefaultState(new SquadRelaxedState(stateName_relaxed, squad))
                                          .State(new SquadCombatState(stateName_combat, squad))
                                      .EndStateMachine();
            return fsm;
        }
    }

}