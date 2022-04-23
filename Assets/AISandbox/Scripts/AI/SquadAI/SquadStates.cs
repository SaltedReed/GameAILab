using UnityEngine;
using GameAILab.Decision.FSM;

namespace GameAILab.Sandbox
{

    public abstract class SquadState : State
    {
        public AISquad OwnerSquad { get; protected set; }

        public SquadState(string name, AISquad squad)
            : base(name)
        {
            OwnerSquad = squad;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Debug.Log($"aisquad entered {Name}");
        }

        public override void OnExit()
        {
            base.OnEnter();
            Debug.Log($"aisquad exiting {Name}");
        }
    }


    public class SquadRelaxedState : SquadState
    {
        public SquadRelaxedState(string name, AISquad squad)
            : base(name, squad) { }

        public override string OnUpdate()
        {
            if (OwnerSquad.Target != null)
                return Owner.GetComponent<SquadFsmOwner>().stateName_combat;
            return Name;
        }
    }


    public class SquadCombatState : SquadState
    {
        public SquadCombatState(string name, AISquad squad)
            : base(name, squad) { }

        public override void OnEnter()
        {
            base.OnEnter();

            OwnerSquad.BuildUpBattlePoints();
            OwnerSquad.UpdateMemberBattlePoints();
            OwnerSquad.UpdateMemberTargets(OwnerSquad.Target);
        }

        public override string OnUpdate()
        {
            if (OwnerSquad.Target is null)
                return Owner.GetComponent<SquadFsmOwner>().stateName_relaxed;

            Vector3 curTargetPos = OwnerSquad.Target.Go.transform.position;
            if (Vector3.SqrMagnitude(curTargetPos - OwnerSquad.LastTargetPos) > 0.1f)
            {
                OwnerSquad.LastTargetPos = curTargetPos;

                OwnerSquad.BuildUpBattlePoints();
                OwnerSquad.UpdateMemberBattlePoints();
            }

            return Name;
        }

        public override void OnExit()
        {
            base.OnExit();

            OwnerSquad.UpdateMemberTargets(null);
            OwnerSquad.ClearBattlePoints();
        }
    }

}