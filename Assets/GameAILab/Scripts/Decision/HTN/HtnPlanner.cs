using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAILab.Decision.HTN
{

    public abstract class Task
    {
        public string name;
    }


    public class PrimitiveTask : Task
    {
        public List<Condition> conditions = new List<Condition>();
        public List<Operator> operators = new List<Operator>();
        public Dictionary<Operator, List<Effect>> opEffectMap = new Dictionary<Operator, List<Effect>>();
    }


    public class CompoundTask : Task
    {
        public List<Method> methods = new List<Method>();
    }


    // temp
    public class State
    {
        public bool WsCanSeeEnemy;
        public int WsLocation; // 1: EnemyLocRef, 0: anywhere else
    }

    // todo: data driven
    public abstract class Effect
    {
        public abstract State Sim(State state);
        public abstract void Apply(State state);
    }

    // how to get the player, world state, etc?
    public abstract class Operator
    {
        // todo: operator state
        public abstract void Run();
    }


    // todo: data driven
    public abstract class Condition
    {
        public abstract bool IsSatisfied(State worldState);
    }


    public class Method
    {
        public List<Condition> conditions = new List<Condition>();
        public List<Task> subtasks = new List<Task>();
    }


    public class HtnPlanner
    {
        public string name;
        //public List<Task> tasks = new List<Task>();
        public Task root;

        public List<PrimitiveTask> plan { get; private set; }

        private List<Task> m_tasksToProcess;
        private State m_curState;
        private Task m_curTask;

        /*public List<PrimitiveTask> StartFind(State curState)
        {
            m_curState = curState;
            m_curTask = null;
            m_tasksToProcess = new List<Task>();
            plan = new List<PrimitiveTask>();

            m_tasksToProcess.Add(root);

            while (m_tasksToProcess.Count > 0)
            {
                m_curTask = m_tasksToProcess[0];
                m_tasksToProcess.RemoveAt(0);

                CompoundTask curCompTask = m_curTask as CompoundTask;
                if (curCompTask != null)
                {
                    Method bestMethod = curCompTask.FindBestMethod(m_curState);
                    if (bestMethod is null)
                    {
                        RestoreToLastDecomposedTask();
                    }
                    else
                    {
                        RecordDecompositionOfTask();
                        PushSubtasks(m_curTask);
                    }
                }
                else
                {
                    PrimitiveTask curPrimTask = m_curTask as PrimitiveTask;
                    if (curPrimTask.AreConditionsMet(m_curState))
                    {
                        ApplyEffects(m_curState, curPrimTask);
                        plan.Add(curPrimTask);
                    }
                    else
                    {
                        RestoreToLastDecomposedTask();
                    }
                }
            }
        }*/
    }

}