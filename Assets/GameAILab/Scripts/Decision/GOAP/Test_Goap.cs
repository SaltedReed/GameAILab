using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAILab.Decision.GOAP;

public class Test_Goap : MonoBehaviour
{
    private void Start()
    {
#if true
        GoapPlannerBuilder builder = new GoapPlannerBuilder();
        GoapPlanner planner =
            builder.Planner("p1")
                .Action("Attack")
                    .Precondition("kWeaponIsLoaded", true)
                    .Effect("kTargetIsDead", true)
                .EndAction()
                .Action("LoadWeapon")
                    .Precondition("kWeaponIsArmed", true)
                    .Effect("kWeaponIsLoaded", true)
                .EndAction()
                .Action("DrawWeapon")
                    .Effect("kWeaponIsArmed", true)
                .EndAction()
            .EndPlanner();

        State start = new State();
        start.Add("kTargetIsDead", false);

        State goal = new State();
        goal.Add("kTargetIsDead", true);

        GoapGraph graph = planner.BuildUpGraph(start, goal);
        GoapNode leaf = planner.StartFind(graph, start, goal);
        List<Action> plan = GoapPlanner.BuildUpPlan(leaf);

        string planstr = "plan: ";
        foreach (Action a in plan)
            planstr += a.DebugStr() + "\n";
        Debug.Log(planstr);
#endif

#if false
        Action aAttack = new Action();
        aAttack.name = "Attack";
        //aAttack.pre = new State();
        aAttack.pre.atoms.Add("kWeaponIsLoaded", true);
        //aAttack.effect = new State();
        aAttack.effect.atoms.Add("kTargetIsDead", true);

        Action aLoadWeapon = new Action();
        aLoadWeapon.name = "LoadWeapon";
        //aLoadWeapon.pre = new State();
        aLoadWeapon.pre.atoms.Add("kWeaponIsArmed", true);
        //aLoadWeapon.effect = new State();
        aLoadWeapon.effect.atoms.Add("kWeaponIsLoaded", true);

        Action aDrawWeapon = new Action();
        aDrawWeapon.name = "DrawWeapon";
        aDrawWeapon.pre = null;
        //aDrawWeapon.effect = new State();
        aDrawWeapon.effect.atoms.Add("kWeaponIsArmed", true);

        Action aThrow = new Action();
        aThrow.name = "Throw";
        aThrow.effect.atoms.Add("kTargetIsDead", true);

        GoapPlanner planner = new GoapPlanner();
        planner.actions.Add(aAttack);
        planner.actions.Add(aLoadWeapon);
        planner.actions.Add(aDrawWeapon);
        //planner.actions.Add(aThrow);

        State start = new State();
        start.atoms.Add("kTargetIsDead", false);

        State goal = new State();
        goal.atoms.Add("kTargetIsDead", true);

        GoapGraph graph = planner.BuildUpGraph(start, goal);
        GoapNode leaf = planner.StartFind(graph, start, goal);
        List<Action> plan = GoapPlanner.BuildUpPlan(leaf);

        string planstr = "plan: ";
        foreach (Action a in plan)
            planstr += a.DebugStr() + "\n";
        Debug.Log(planstr);

#endif

    }
}
