using System;
using System.Collections.Generic;


namespace GameAILab.Decision.BT
{

    public class BehaviourTreeBuilder
    {
        public BehaviourTree tree;
        public Stack<Behaviour> bhvStack = new Stack<Behaviour>();


        public BehaviourTreeBuilder Tree(string treeName)
        {
            bhvStack.Clear();
            tree = new BehaviourTree(treeName);
            return this;
        }

        public BehaviourTree EndTree()
        {
            if (bhvStack.Count > 0)
            {
                throw new InvalidOperationException();
            }

            return tree;
        }

        public BehaviourTreeBuilder Sequence(string name)
        {
            Sequence node = new Sequence(name);
            AttachToRootOrParent(node);
            bhvStack.Push(node);

            return this;
        }

        public BehaviourTreeBuilder EndSequence()
        {
            bhvStack.Pop();
            return this;
        }

        public BehaviourTreeBuilder Selector(string name)
        {
            Selector node = new Selector(name);
            AttachToRootOrParent(node);
            bhvStack.Push(node);

            return this;
        }

        public BehaviourTreeBuilder EndSelector()
        {
            bhvStack.Pop();
            return this;
        }

        public BehaviourTreeBuilder Parallel(string name)
        {
            Parallel node = new Parallel(name);
            AttachToRootOrParent(node);
            bhvStack.Push(node);

            return this;
        }

        public BehaviourTreeBuilder EndParallel()
        {
            bhvStack.Pop();
            return this;
        }

        public BehaviourTreeBuilder Decorator(Decorator decorator)
        {
            if (decorator is null)
            {
                throw new NullReferenceException();
            }

            Composite parent = AttachToRootOrParent(decorator);
            if (tree.Root != null && parent is null)
            {
                throw new InvalidOperationException();
            }

            return this;
        }

        public BehaviourTreeBuilder Task(Behaviour task)
        {
            if (task is null)
            {
                throw new NullReferenceException();
            }

            Composite parent = AttachToRootOrParent(task);
            if (tree.Root != null && parent is null)
            {
                throw new InvalidOperationException();
            }

            return this;
        }

        public Composite GetParent()
        {
            if (bhvStack.Count > 0)
            {
                return bhvStack.Peek() as Composite;
            }
            return null;
        }

        public Composite AttachToRootOrParent(Behaviour node)
        {
            if (tree.Root is null)
            {
                tree.Root = node;
            }

            Composite parent = GetParent();
            parent?.AddChild(node);

            return parent;
        }
    }

}