using System;
using System.Collections.Generic;


namespace GameAILab.Decision.BT
{

    public enum BehaviourState
    {
        Invalid,
        Running,
        Success,
        Failure,
    }


    public abstract class Behaviour
    {
        public string name;
        public BehaviourState State { get; protected set; } = BehaviourState.Invalid;
        public bool IsTerminated => State == BehaviourState.Success || State == BehaviourState.Failure;
       
        public virtual BehaviourTree Tree 
        {
            get => m_tree;
            set
            {
                m_tree = value;
                OnUpdateTree(value);
            }
        }
        protected BehaviourTree m_tree;


        public Behaviour()
        {

        }

        public Behaviour(string bhvName)
        {
            this.name = bhvName;
        }

        public BehaviourState Tick()
        {
            if (State != BehaviourState.Running)
            {
                UnityEngine.Debug.Log(name + " init");
                OnInit();
            }

            UnityEngine.Debug.Log(name + " update");
            State = OnUpdate();

            if (State != BehaviourState.Running)
            {
                UnityEngine.Debug.Log(name + " terminate");
                OnTerminate();
            }

            return State;
        }

        protected abstract BehaviourState OnUpdate();
        protected virtual void            OnInit() { }
        protected virtual void            OnTerminate() { }

        public virtual void Reset()
        {
            State = BehaviourState.Invalid;
        }

        protected virtual void OnUpdateTree(BehaviourTree tree) { }
    }


    public abstract class Decorator : Behaviour
    {
        public Behaviour Child 
        {
            get => m_child;
            protected set
            {
                m_child = value;
                if (value != null)
                    OnUpdateTree(Tree);
            }
        }
        protected Behaviour m_child;

        public Decorator(Behaviour child)
        {
            if (child is null)
            {
                throw new NullReferenceException();
            }
            Child = child;
        }

        public Decorator(string bhvName, Behaviour child)
        {
            name = bhvName;

            if (child is null)
            {
                throw new NullReferenceException();
            }
            Child = child;
        }

        protected override void OnUpdateTree(BehaviourTree tree)
        {
            base.OnUpdateTree(tree);
            Child.Tree = tree;
        }
    }


    public abstract class Composite : Behaviour
    {
        protected List<Behaviour> m_behaviours = new List<Behaviour>();


        public Composite()
        {

        }

        public Composite(string bhvName)
            : base(bhvName)
        {
            
        }

        public override void Reset()
        {
            base.Reset();
            
            foreach (Behaviour b in m_behaviours)
            {
                b.Reset();
            }
        }

        protected override void OnUpdateTree(BehaviourTree tree)
        {
            base.OnUpdateTree(tree);

            foreach (Behaviour bhv in m_behaviours)
            {
                bhv.Tree = Tree;
            }
        }

        public void AddChild(Behaviour child)
        {
            if (child != null)
            {
                child.Tree = Tree;
                m_behaviours.Add(child);
            }
        }

        public void RemoveChild(Behaviour child)
        {
            if (child != null)
            {
                child.Tree = null;
                m_behaviours.Remove(child);
            }
        }
                
    }


    public sealed class Sequence : Composite
    {
        private int m_current = 0;


        public Sequence()
        {

        }

        public Sequence(string bhvName)
            : base(bhvName)
        {

        }

        protected override void OnInit()
        {
            base.OnInit();
            m_current = 0;            
        }

        protected override BehaviourState OnUpdate()
        {
            while (true)
            {
                if (m_current < 0 || m_current >= m_behaviours.Count)
                {
                    throw new IndexOutOfRangeException($"m_current: {m_current}");
                }

                Behaviour bhv = m_behaviours[m_current];
                if (bhv is null)
                {
                    throw new NullReferenceException($"m_behaviours[{m_current}] is null");
                }

                BehaviourState state = bhv.Tick();
                if (state != BehaviourState.Success)
                {
                    return state;
                }

                if (++m_current == m_behaviours.Count)
                {
                    return BehaviourState.Success;
                }
            }
        }
    }

    public sealed class Selector : Composite
    {
        private int m_current = 0;


        public Selector()
        {

        }

        public Selector(string bhvName)
            : base(bhvName)
        {

        }

        protected override void OnInit()
        {
            base.OnInit();
            m_current = 0;
        }

        protected override BehaviourState OnUpdate()
        {
            while (true)
            {
                if (m_current < 0 || m_current >= m_behaviours.Count)
                {
                    throw new IndexOutOfRangeException($"m_current: {m_current}");
                }

                Behaviour bhv = m_behaviours[m_current];
                if (bhv is null)
                {
                    throw new NullReferenceException($"m_behaviours[{m_current}] is null");
                }

                BehaviourState state = bhv.Tick();
                if (state != BehaviourState.Failure)
                {
                    return state;
                }

                if (++m_current == m_behaviours.Count)
                {
                    return BehaviourState.Failure;
                }
            }
        }
    }

    public sealed class Parallel : Composite
    {
        public enum Policy
        {
            One,
            All
        }


        public Policy SuccessPolicy { get; set; } = Policy.All;
        public Policy FailurePolicy { get; set; } = Policy.One;


        public Parallel()
        {

        }

        public Parallel(string bhvName)
            : base(bhvName)
        {

        }

        protected override BehaviourState OnUpdate()
        {
            if (SuccessPolicy == FailurePolicy)
            {
                UnityEngine.Debug.LogError($"SuccessPolicy: {SuccessPolicy}, FailurePolicy: {FailurePolicy}");
            }

            int succCount = 0, failCount = 0;
            foreach (Behaviour bhv in m_behaviours)
            {
                if (bhv.IsTerminated)
                {
                    continue;
                }

                BehaviourState state = bhv.Tick();

                if (state == BehaviourState.Success)
                {
                    if (SuccessPolicy == Policy.One)
                    {
                        return BehaviourState.Success;
                    }
                    ++succCount;
                }
                else if (state == BehaviourState.Failure)
                {
                    if (FailurePolicy == Policy.All)
                    {
                        return BehaviourState.Failure;
                    }
                    ++failCount;
                }
            }

            if (SuccessPolicy == Policy.All && succCount == m_behaviours.Count)
            {
                return BehaviourState.Success;
            }
            if (FailurePolicy == Policy.All && failCount == m_behaviours.Count)
            {
                return BehaviourState.Failure;
            }
            return BehaviourState.Running;
        }

    }

    public sealed class BehaviourTree : Behaviour
    {
        public Behaviour Root 
        {
            get => m_root;
            set
            {
                m_root = value;
                if (value != null)
                    m_root.Tree = this;
            }
        }
        private Behaviour m_root;

        public Blackboard Blackboard { get; set; }


        public BehaviourTree()
        {
            Setup();
        }

        public BehaviourTree(string bhvName)
            : base(bhvName)
        {
            Setup();
        }

        private void Setup()
        {
            Tree = this;
        }

        protected override BehaviourState OnUpdate()
        {
            if (Root is null)
            {
                throw new NullReferenceException();
            }

            State = Root.Tick();
            return State;
        }

    }


}