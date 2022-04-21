using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace GameAILab.Decision.Ut
{

    public enum ActionState
    {
        Invalid,
        Running,
        Success,
        Failure
    }


    /// <summary>
    /// Actions with utility <= 0 will never be chosen
    /// </summary>
    public abstract class Action
    {
        public string Name { get; protected set; }
        public float CachedUtility { get; protected set; }
        public ActionState State { get; protected set; } = ActionState.Invalid;
        public UtilityAI AI { get; set; }


        public Action(string name)
        {
            Name = name;
        }

        public float CalculateUtility()
        {
            CachedUtility = DoCalculation();
            return CachedUtility;
        }

        protected abstract float DoCalculation();

        public ActionState Tick()
        {
            if (State != ActionState.Running)
            {
                OnInit();
            }

            State = OnUpdate();

            if (State != ActionState.Running)
            {
                OnTerminate(State);
            }

            return State;
        }

        protected abstract ActionState OnUpdate();

        protected virtual void OnInit() { }

        protected virtual void OnTerminate(ActionState state) { }

    }


    public sealed class UtilityAI
    {
        public enum Policy
        {
            Highest,
            WeightedRandom,
        }


        public Policy choosePolicy;
        public string Name { get; set; }
        public Action CurrentAction { get; private set; }
        public Blackboard Blackboard { get; set; }

        private List<Action> m_actions = new List<Action>();
        private List<Action> m_potentialActions;


        #region Action Management

        public void AddAction(Action action)
        {
            if (action is null)
            {
                throw new NullReferenceException();
            }

            if (!m_actions.Contains(action))
            {
                action.AI = this;
                m_actions.Add(action);
            }
        }

        public void RemoveAction(Action action)
        {
            if (action != null)
            {
                m_actions.Remove(action);
            }
        }

        #endregion


        public void Tick()
        {
            if (CurrentAction is null)
            {
                CurrentAction = SelectAction();
                if (CurrentAction is null)
                {
                    return;
                }
            }

            ActionState state = CurrentAction.Tick();
            if (state != ActionState.Running)
            {
                CurrentAction = SelectAction();
            }
        }

        public Action SelectAction()
        {
            if (m_actions.Count == 0)
                return null;

            FindPotentialActions();
            if (m_potentialActions.Count == 0)
                return null;

            Action action = null;
            switch (choosePolicy)
            {
                case Policy.Highest:
                    action = m_actions[0];
                    break;
                case Policy.WeightedRandom:
                    float total = m_potentialActions.Sum((Action a) => { return a.CachedUtility; });
                    float choice = UnityEngine.Random.Range(0, total);
                    float accu = 0;
                    foreach (Action a in m_potentialActions)
                    {
                        accu += a.CachedUtility;
                        if (choice <= accu)
                        {
                            action = a;
                            break;
                        }
                    }
                    break;
                default:
                    Debug.LogError($"does not support choosing policy {choosePolicy}");
                    break;
            }

            return action;
        }

        private void FindPotentialActions()
        {
            m_actions.Sort((Action a1, Action a2) => { return Mathf.CeilToInt(a2.CalculateUtility() - a1.CalculateUtility()); });
            m_potentialActions = m_actions.FindAll((Action a) => { return a.CachedUtility > 0.0f; });
        }
    }
}