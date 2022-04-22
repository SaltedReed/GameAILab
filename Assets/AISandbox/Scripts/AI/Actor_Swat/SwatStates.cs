﻿using UnityEngine;
using GameAILab.Decision.FSM;


namespace GameAILab.Sandbox
{

    public abstract class SwatState : State
    {
        public ASwat OwnerActor { get; protected set; }

        public SwatState(string name, ASwat owner)
            : base(name)
        {
            OwnerActor = owner;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            Debug.Log($"swat entered {Name}");
        }

        public override void OnExit()
        {
            base.OnExit();

            Debug.Log($"swat exiting {Name}");
        }
    }


    public class SwatPatrolState : SwatState
    {
        public SwatPatrolState(string name, ASwat owner)
            : base(name, owner) { }

        public override void OnEnter()
        {
            base.OnEnter();

            OwnerActor.Anim.SetBool(OwnerActor.animKey_moving, true);
            OwnerActor.Anim.SetBool(OwnerActor.animKey_combat, false);
            OwnerActor.Patrol.RestartPatrol();
        }

        public override string OnUpdate()
        {
            return Name;
        }

        public override void OnExit()
        {
            base.OnExit();

            OwnerActor.Patrol.StopPatrol();
        }
    }

    public class CombatMoveToState : SwatState
    {
        public CombatMoveToState(string name, ASwat owner)
            : base(name, owner) { }

        public override void OnEnter()
        {
            base.OnEnter();

            OwnerActor.Anim.SetBool(OwnerActor.animKey_moving, true);
            OwnerActor.Anim.SetBool(OwnerActor.animKey_combat, true);
            OwnerActor.MoveToBattlePoint();
        }

        public override string OnUpdate()
        {
            if (OwnerActor.Movement.IsAt(OwnerActor.BattlePoint))
                return Owner.GetComponent<SwatFsmOwner>().stateName_aim;
            return Name;
        }

    }

    public class AimState : SwatState
    {
        public AimState(string name, ASwat owner)
            : base(name, owner) { }

        public override void OnEnter()
        {
            base.OnEnter();

            OwnerActor.Anim.SetBool(OwnerActor.animKey_moving, false);
            OwnerActor.Anim.SetBool(OwnerActor.animKey_combat, true);
        }

        public override string OnUpdate()
        {
            return Name;
        }

    }

}