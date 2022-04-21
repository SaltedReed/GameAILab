using System;
using System.Linq;
using System.Collections.Generic;

namespace GameAILab.Decision.DualUt
{

    /// <summary>
    /// Objects with utility <= 0 will never be chosen
    /// </summary>
    public abstract class UtObject
    {
        public string Name { get; protected set; }
        public int Rank { get; set; }
        public float CachedUtility { get; protected set; }
        public DualUtilityAI AI { get; set; }


        public UtObject(string name)
        {
            Name = name;
        }

        public float CalculateUtility()
        {
            CachedUtility = DoCalculation();
            return CachedUtility;
        }

        protected abstract float DoCalculation();
    }


    public class DualUtilityAI
    {
        // objects whose rank is in [HighestRank-rankGap, highestRank] will be thought as having the highest rank
        public int rankGap;
        public string Name { get; set; }
        public UtObject CurrentUtObject { get; private set; }
        public int HighestRank { get; private set; }
        //public Blackboard Blackboard { get; set; }

        private List<UtObject> m_utObjects = new List<UtObject>();
        private List<UtObject> m_highestRankUtObjects;


        #region UtObject Management

        public void AddUtObject(UtObject utObject)
        {
            if (utObject is null)
            {
                throw new NullReferenceException();
            }

            if (!m_utObjects.Contains(utObject))
            {
                utObject.AI = this;
                m_utObjects.Add(utObject);
            }
        }

        public void RemoveUtObject(UtObject utObject)
        {
            if (utObject != null)
            {
                m_utObjects.Remove(utObject);
            }
        }

        #endregion


        public void Tick()
        {
            if (CurrentUtObject is null)
            {
                CurrentUtObject = SelectUtObject();
                if (CurrentUtObject is null)
                {
                    return;
                }
            }
        }

        public UtObject SelectUtObject()
        {
            if (m_utObjects.Count == 0)
                return null;

            FindHighestRankUtObjects();
            EvaluateHighestRankUtObjects();
            EliminateHighestRankUtObjects();
            if (m_highestRankUtObjects.Count == 0)
                return null;

            UtObject utObject = null;
            float total = m_highestRankUtObjects.Sum((UtObject u) => { return u.CachedUtility; });
            float choice = UnityEngine.Random.Range(0, total);
            float accu = 0;
            foreach (UtObject u in m_highestRankUtObjects)
            {
                accu += u.CachedUtility;
                if (choice <= accu)
                {
                    utObject = u;
                    break;
                }
            }

            return utObject;
        }

        private void FindHighestRankUtObjects()
        {
            HighestRank = m_utObjects.Max((UtObject u) => { return u.Rank; });
            m_highestRankUtObjects = m_utObjects.FindAll((UtObject u) => { return HighestRank - u.Rank <= rankGap; });
        }

        private void EvaluateHighestRankUtObjects()
        {
            foreach (UtObject u in m_highestRankUtObjects)
            {
                u.CalculateUtility();
            }
        }

        private void EliminateHighestRankUtObjects()
        {
            m_highestRankUtObjects.RemoveAll((UtObject u) => { return u.CachedUtility <= 0.0f; });
        }

    }

}

#if false
namespace GameAILab.Decision.DualUt
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
        public int Rank { get; set; }
        public float CachedUtility { get; protected set; }
        public ActionState State { get; protected set; } = ActionState.Invalid;
        public DualUtilityAI AI { get; set; }


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


    public sealed class DualUtilityAI
    {
        // actions whose rank is in [HighestRank-rankGap, highestRank] will be thought as having the highest rank
        public int rankGap;
        public string Name { get; set; }
        public Action CurrentAction { get; private set; }
        public int HighestRank { get; private set; }
        public Blackboard Blackboard { get; set; }

        private List<Action> m_actions = new List<Action>();
        private List<Action> m_highestRankActions;


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

            FindHighestRankActions();
            EvaluateHighestRankActions();
            EliminateHighestRankActions();
            if (m_highestRankActions.Count == 0)
                return null;

            Action action = null;
            float total = m_highestRankActions.Sum((Action a) => { return a.CachedUtility; });
            float choice = UnityEngine.Random.Range(0, total);
            float accu = 0;
            foreach (Action a in m_highestRankActions)
            {
                accu += a.CachedUtility;
                if (choice <= accu)
                {
                    action = a;
                    break;
                }
            }

            return action;
        }

        private void FindHighestRankActions()
        {
            HighestRank = m_actions.Max((Action a) => { return a.Rank; });
            m_highestRankActions = m_actions.FindAll((Action a) => { return HighestRank - a.Rank <= rankGap; });
        }

        private void EvaluateHighestRankActions()
        {
            foreach (Action a in m_highestRankActions)
            {
                a.CalculateUtility();
            }
        }

        private void EliminateHighestRankActions()
        {
            m_highestRankActions.RemoveAll((Action a) => { return a.CachedUtility <= 0.0f; });
        }

    }

}
#endif