using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAILab.Decision.HTN;


public class Condition_True : Condition
{
    public override bool IsSatisfied(State worldState)
    {
        return true;
    }
}

public class Condition_WsCanSeeEnemy : Condition
{
    public override bool IsSatisfied(State worldState)
    {
        return worldState.WsCanSeeEnemy;
    }
}

public class Op_NavToEnemy : Operator
{
    public override void Run()
    {
        Debug.Log("Op_NavToEnemy");
    }
}

public class Op_AnimatedAttack : Operator
{
    public override void Run()
    {
        Debug.Log("Op_AnimatedAttack");
    }
}

public class Ef_LocEnemy : Effect
{
    public override void Apply(State state)
    {
        state.WsLocation = 1;
    }

    public override State Sim(State state)
    {
        throw new System.NotImplementedException();
    }
}

public class Test_HTN : MonoBehaviour
{
    private void Start()
    {
        Op_NavToEnemy op_NavToEnemy = new Op_NavToEnemy();
        Op_AnimatedAttack op_AnimatedAttack = new Op_AnimatedAttack();

        Ef_LocEnemy ef_LocEnemy = new Ef_LocEnemy();


        HtnPlanner planner = new HtnPlanner();

        // tNavToEnemy -----------
        PrimitiveTask tNavToEnemy = new PrimitiveTask();
        tNavToEnemy.operators.Add(op_NavToEnemy);
        tNavToEnemy.opEffectMap.Add(op_NavToEnemy, new List<Effect> { ef_LocEnemy });

        // tBeTrunkThumper -----------
        CompoundTask tBeTrunkThumper = new CompoundTask { name = "tBeTrunkThumper" };

        Method m1 = new Method();
        m1.conditions.Add(new Condition_WsCanSeeEnemy());
        m1.subtasks.Add(tNavToEnemy);
        //m1.subtasks.Add(tDoTrunkSlam);

        //Method m2 = new Method();
        //m2.conditions.Add(new Condition_True());
        //m2.subtasks.Add(tChooseBridgeToCheck);
        //m2.subtasks.Add(tNavToBridge);
        //m2.subtasks.Add(tCheckBridge);

        tBeTrunkThumper.methods.Add(m1);
        //tBeTrunkThumper.methods.Add(m2);
    }
}
