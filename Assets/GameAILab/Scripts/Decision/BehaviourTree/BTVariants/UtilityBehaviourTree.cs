using System;
using System.Linq;
using System.Collections.Generic;
using GameAILab.Decision.BT;

namespace GameAILab.Decision.BTVariants
{

    public abstract class UtBehaviour : Behaviour
    {
        public float CachedUtility { get; protected set; }

        public UtBehaviour() { }

        public UtBehaviour(string bhvName)
            : base(bhvName) { }

        public float CalculateUtility()
        {
            CachedUtility = DoCalculation();
            return CachedUtility;
        }

        protected abstract float DoCalculation();
    }


    public sealed class UtSelector : Composite
    {
        public enum Policy
        {
            All,
            Highest,
            WeightedRandom,
        }

        public Policy failurePolicy;

        private int m_current;
        private List<UtBehaviour> m_sortedUtBehaviours;


        public UtSelector() { }

        public UtSelector(string bhvName)
            : base(bhvName) { }

        protected override void OnInit()
        {
            base.OnInit();

            SortBehaviours();
            SelectCurrentBehaviour();
        }

        protected override BehaviourState OnUpdate()
        {
            Behaviour bhv = m_sortedUtBehaviours[m_current];
            BehaviourState state = bhv.Tick();

            if (failurePolicy == Policy.All)
            {                
                if (state != BehaviourState.Failure)
                {
                    return state;
                }

                if (++m_current == m_sortedUtBehaviours.Count)
                {
                    return BehaviourState.Failure;
                }

                return BehaviourState.Running;
            }
            else
            {
                return state;
            }
        }

        private void SortBehaviours()
        {
            m_sortedUtBehaviours = new List<UtBehaviour>(m_behaviours.Count);
            for (int i=0; i<m_behaviours.Count; ++i)
            {
                UtBehaviour utb = m_behaviours[i] as UtBehaviour;
                if (utb is null)
                {
                    throw new InvalidCastException($"m_behaviours[{i}]");
                }

                m_sortedUtBehaviours.Add(utb);
            }

            m_sortedUtBehaviours.Sort((UtBehaviour b1, UtBehaviour b2) =>
            {
                return UnityEngine.Mathf.CeilToInt(b2.CalculateUtility() - b1.CalculateUtility());
            });
        }
    
        private void SelectCurrentBehaviour()
        {
            switch (failurePolicy)
            {
                case Policy.All:
                    m_current = 0;
                    break;
                case Policy.Highest:
                    m_current = 0;
                    break;
                case Policy.WeightedRandom:
                    float total = m_sortedUtBehaviours.Sum((UtBehaviour b) => { return b.CachedUtility; });
                    float choice = UnityEngine.Random.Range(0, total);
                    float accu = 0;
                    for (int i = 0; i < m_sortedUtBehaviours.Count; ++i)
                    {
                        accu += m_sortedUtBehaviours[i].CachedUtility;
                        if (choice <= accu)
                        {
                            m_current = i;
                            break;
                        }
                    }
                    break;
                default:
                    UnityEngine.Debug.LogError($"does not support policy {failurePolicy}");
                    break;
            }
        }
    }

}
