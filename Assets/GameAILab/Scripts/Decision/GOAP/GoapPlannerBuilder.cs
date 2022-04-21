using System.Collections.Generic;
using UnityEngine;

namespace GameAILab.Decision.GOAP
{

    public class GoapPlannerBuilder
    {
        protected GoapPlanner m_planner;
        protected Action m_curAct;

        public GoapPlannerBuilder Planner(string name)
        {
            m_planner = new GoapPlanner();
            m_planner.name = name;
            m_curAct = null;

            return this;
        }

        public GoapPlanner EndPlanner()
        {
            return m_planner;
        }

        public GoapPlannerBuilder Action(string name, float cost = 1)
        {
            m_curAct = new Action();
            m_curAct.name = name;
            m_curAct.cost = cost;

            if (m_planner.actions.Find((Action a) => { return a.name == name; }) != null)
            {
                Debug.LogError($"failed to add an action, because goap planner {m_planner.name} already contains an action named {name}");
            }
            else
            {
                m_planner.AddAction(m_curAct);
            }

            return this;
        }

        public GoapPlannerBuilder EndAction()
        {
            m_curAct = null;
            return this;
        }

        public GoapPlannerBuilder Precondition(string key, bool val)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new System.ArgumentException("action precondition key cannot be null or empty");
            }

            m_curAct.pre.atoms.Add(key, val);
            return this;
        }

        public GoapPlannerBuilder Effect(string key, bool val)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new System.ArgumentException("action effect key cannot be null or empty");
            }

            m_curAct.effect.atoms.Add(key, val);
            return this;
        }
    }

}