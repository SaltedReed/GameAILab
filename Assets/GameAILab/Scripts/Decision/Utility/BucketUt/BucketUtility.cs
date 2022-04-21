using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameAILab.Decision.BucketUt
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
        public Bucket Bucket { get; set; }
        public BucketUtilityAI AI { get; set; }


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


    /// <summary>
    /// Buckets with utility <= 0 will never be chosen
    /// </summary>
    public class Bucket
    {
        public string Name { get; protected set; }
        public float CachedUtility { get; protected set; }
        public BucketUtilityAI Utility { get; set; }

        public List<Action> actions = new List<Action>();
        public Func<Bucket, float> doCalculation;

        public Bucket(string name)
        {
            Name = name;
        }

        public float CalculateUtility()
        {
            CachedUtility = DoCalculation();
            return CachedUtility;
        }

        protected float DoCalculation()
        {
            if (doCalculation is null)
            {
                return 1.0f;
            }
            else
            {
                return doCalculation(this);
            }
        }

    }


    public sealed class BucketUtilityAI
    {
        public string Name { get; set; }
        public Action CurrentAction { get; private set; }
        public Blackboard Blackboard { get; set; }

        private List<Bucket> m_buckets = new List<Bucket>();
        private List<Action> m_potentialActions;


        #region Action Management

        public void AddBucket(Bucket bucket)
        {
            if (bucket is null)
            {
                throw new NullReferenceException();
            }

            if (!m_buckets.Contains(bucket))
            {
                bucket.Utility = this;
                m_buckets.Add(bucket);
            }
        }

        public void RemoveBucket(Bucket bucket)
        {
            if (bucket is null)
            {
                return;
            }

            m_buckets.Remove(bucket);
        }

        public void AddAction(Bucket bucket, Action action)
        {
            if (bucket is null)
            {
                throw new NullReferenceException();
            }
            if (action is null)
            {
                throw new NullReferenceException();
            }

            if (bucket.actions.Find((Action a) => { return a.Name == action.Name; }) != null)
            {
                Debug.LogError($"bucket {bucket.Name} already contains action {action.Name}");
                return;
            }

            if (!m_buckets.Contains(bucket))
            {
                AddBucket(bucket);
            }

            action.Bucket = bucket;
            action.AI = this;
            bucket.actions.Add(action);
        }

        public void RemoveAction(Bucket bucket, Action action)
        {
            if (action is null)
            {
                return;
            }
            if (bucket is null)
            {
                throw new NullReferenceException();
            }

            action.Bucket = null;
            bucket.actions.Remove(action);
        }

        public void AddAction(string bucketName, Action action)
        {
            if (action is null)
            {
                throw new NullReferenceException();
            }

            Bucket bucket = m_buckets.Find((Bucket b) => { return b.Name == bucketName; });
            if (bucket is null)
            {
                Debug.LogError($"cannot add action {action.Name} to bucket {bucketName}, " +
                    $"because the bucket cannot be found");
                return;
            }

            AddAction(bucket, action);
        }

        public void RemoveAction(string bucketName, Action action)
        {
            if (action is null)
            {
                return;
            }

            Bucket bucket = m_buckets.Find((Bucket b) => { return b.Name == bucketName; });
            if (bucket is null)
            {
                Debug.LogError($"cannot remove action {action.Name} from bucket {bucketName}, " +
                    $"because the bucket cannot be found");
                return;
            }

            RemoveAction(bucket, action);
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
            if (m_buckets.Count == 0)
                return null;

            Bucket bucket = FindHighestUtilityBucket();
            if (bucket is null)
                return null;

            FindPotentialActions(bucket);
            if (m_potentialActions.Count == 0)
                return null;

            Action action = null;
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

            return action;
        }

        private Bucket FindHighestUtilityBucket()
        {
            Bucket bucket = null;

            float maxUt = 0.0f;
            foreach (Bucket b in m_buckets)
            {
                if (b.CalculateUtility() > maxUt)
                {
                    maxUt = b.CachedUtility;
                    bucket = b;
                }
            }

            return bucket;
        }
    
        private void FindPotentialActions(Bucket bucket)
        {
            bucket.actions.Sort((Action a1, Action a2) =>
                { return Mathf.CeilToInt(a2.CalculateUtility() - a1.CalculateUtility()); });

            m_potentialActions = bucket.actions.FindAll((Action a) => { return a.CachedUtility > 0.0f; });
        }
    }

}
