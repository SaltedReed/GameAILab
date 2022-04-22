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
            ASwat owner = fsm.Owner.GetComponent<ASwat>();
            if (owner is null)
            {
                Debug.LogError("no ASwat script attach to fsm.Owner");
                return;
            }

            if (owner.IsBattlePointDirty && owner.Target != null)
            {
                fsm.ChangeStateByName(/*owner.*/stateName_combatMoveTo);
            }
            else if (owner.Target is null && fsm.CurrentState.Name != /*owner.*/stateName_patrol)
            {
                fsm.ChangeStateByName(/*owner.*/stateName_patrol);
            }
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