using System;
using System.Collections.Generic;

namespace GameAILab.Decision.EventDrivenBT
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
        public BehaviourState State { get; set; } = BehaviourState.Invalid;
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

        public Action<BehaviourState> onComplete;


        public Behaviour()
        {

        }

        public Behaviour(string bhvName)
        {
            this.name = bhvName;
        }

        public abstract BehaviourState OnUpdate();
        public virtual void OnInit() { }
        public virtual void OnTerminate() { }

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

        protected override void OnUpdateTree(BehaviourTree tree)
        {
            base.OnUpdateTree(tree);
            Child.Tree = tree;
        }
    }


    public abstract class Composite : Behaviour
    {
        protected List<Behaviour> m_behaviours = new List<Behaviour>();

        public Composite(BehaviourTree tree)
        {
            Tree = tree;
        }

        public Composite(string bhvName, BehaviourTree tree)
            : base(bhvName)
        {
            Tree = tree;
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


        public Sequence(BehaviourTree tree)
            : base(tree)
        {

        }

        public Sequence(string bhvName, BehaviourTree tree)
            : base(bhvName, tree)
        {

        }

        public override void OnInit()
        {
            base.OnInit();
            m_current = 0;

            m_tree.Begin(m_behaviours[m_current], OnChildComplete);
        }

        public override BehaviourState OnUpdate()
        {
            return BehaviourState.Running;
        }

        private void OnChildComplete(BehaviourState state)
        {
            if (state == BehaviourState.Failure)
            {
                m_tree.Stop(this, BehaviourState.Failure);
            }
            else if (state == BehaviourState.Success)
            {
                ++m_current;
                if (m_current == m_behaviours.Count)
                {
                    m_tree.Stop(this, BehaviourState.Success);
                }
                else
                {
                    m_tree.Begin(m_behaviours[m_current], OnChildComplete);
                }
            }
        }
    }


    public sealed class Selector : Composite
    {
        private int m_current = 0;


        public Selector(BehaviourTree tree)
            : base(tree)
        {

        }

        public Selector(string bhvName, BehaviourTree tree)
            : base(bhvName, tree)
        {

        }

        public override void OnInit()
        {
            base.OnInit();
            m_current = 0;

            m_tree.Begin(m_behaviours[m_current], OnChildComplete);
        }

        public override BehaviourState OnUpdate()
        {
            return BehaviourState.Running;
        }

        private void OnChildComplete(BehaviourState state)
        {
            if (state == BehaviourState.Success)
            {
                m_tree.Stop(this, BehaviourState.Success);
            }
            else if (state == BehaviourState.Failure)
            {
                ++m_current;
                if (m_current == m_behaviours.Count)
                {
                    m_tree.Stop(this, BehaviourState.Failure);
                }
                else
                {
                    m_tree.Begin(m_behaviours[m_current], OnChildComplete);
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

        private int m_succCount = 0;
        private int m_failCount = 0;


        public Parallel(BehaviourTree tree)
            : base(tree)
        {

        }

        public Parallel(string bhvName, BehaviourTree tree)
            : base(bhvName, tree)
        {

        }

        public override void OnInit()
        {
            base.OnInit();

            m_succCount = 0;
            m_failCount = 0;

            for (int i=m_behaviours.Count-1; i>=0; --i)
            {
                m_tree.Begin(m_behaviours[i], OnChildComplete);
            }
        }

        public override BehaviourState OnUpdate()
        {
            return BehaviourState.Running;
        }

        private void OnChildComplete(BehaviourState state)
        {
            if (SuccessPolicy == FailurePolicy)
            {
                UnityEngine.Debug.LogError($"SuccessPolicy: {SuccessPolicy}, FailurePolicy: {FailurePolicy}");
            }

            if (state == BehaviourState.Success)
            {
                if (SuccessPolicy == Policy.One)
                {
                    m_tree.Stop(this, BehaviourState.Success);
                    return;
                }
                ++m_succCount;
            }
            if (state == BehaviourState.Failure)
            {
                if (FailurePolicy == Policy.One)
                {
                    m_tree.Stop(this, BehaviourState.Failure);
                    return;
                }
                ++m_failCount;
            }

            if (SuccessPolicy == Policy.All && m_succCount == m_behaviours.Count)
            {
                m_tree.Stop(this, BehaviourState.Success);
            }
            if (FailurePolicy == Policy.All && m_failCount == m_behaviours.Count)
            {
                m_tree.Stop(this, BehaviourState.Failure);
            }
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

        private List<Behaviour> m_behaviours = new List<Behaviour>();


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

        public override BehaviourState OnUpdate()
        {
            int count = m_behaviours.Count;
            if (count == 0)
            {
                return BehaviourState.Invalid;
            }

            for (int i = 0; i < count; ++i)
            {
                Step();
            }
            return BehaviourState.Running;
        }

        public void Step()
        {
            Behaviour bhv = m_behaviours[0];
            m_behaviours.Remove(bhv);

            BehaviourState result = bhv.Tick();

            if (result != BehaviourState.Running)
            {
                bhv.onComplete?.Invoke(result);
            }
            else
            {
                m_behaviours.Add(bhv);
            }
        }

        public void Begin(Behaviour behaviour, Action<BehaviourState> onComplete)
        {
            if (behaviour is null)
            {
                throw new NullReferenceException();
            }

            behaviour.onComplete += onComplete;
            m_behaviours.Insert(0, behaviour);
        }

        public void Stop(Behaviour behaviour, BehaviourState result)
        {
            if (behaviour is null)
            {
                throw new NullReferenceException();
            }

            behaviour.State = result;
            behaviour.onComplete?.Invoke(result);
            m_behaviours.Remove(behaviour);
        }

    }

}