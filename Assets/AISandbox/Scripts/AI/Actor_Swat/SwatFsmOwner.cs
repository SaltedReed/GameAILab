using UnityEngine;
using GameAILab.Decision.FSM;


namespace GameAILab.Sandbox
{


    public class SwatFsmOwner : StateMachineOwner
    {
        public string stateName_patrol = "patrol";
        public string stateName_combatMoveTo = "combatMoveTo";
        public string stateName_aim = "aim";

        public override void OnAIUpdate(StateMachine fsm)
        {

        }

        protected override StateMachine BuildUpFsm()
        {
            ASwat actor = GetComponent<ASwat>();
            if (actor is null)
                throw new System.NullReferenceException();

            StateMachineBuilder builder = new StateMachineBuilder();
            StateMachine fsm = builder.StateMachine("aiactor fsm", gameObject)
                                           .DefaultState(new SwatPatrolState(/*actor.*/stateName_patrol, actor))
                                           .State(new CombatMoveToState(/*actor.*/stateName_combatMoveTo, actor))
                                           .State(new AimState(/*actor.*/stateName_aim, actor))
                                      .EndStateMachine();
            return fsm;
        }

    }

}