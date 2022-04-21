using System;
using GameAILab.Decision.BT;

namespace GameAILab.Decision.BTVariants
{

    public static class UtBehaviourTreeBuilder
    {
        public static BehaviourTreeBuilder UtBehaviour(this BehaviourTreeBuilder builder, UtBehaviour task)
        {
            if (task is null)
            {
                throw new NullReferenceException();
            }

            Composite parent = builder.AttachToRootOrParent(task);
            if (builder.tree.Root != null && parent is null)
            {
                throw new InvalidOperationException();
            }

            return builder;
        }


        public static BehaviourTreeBuilder UtSelector(this BehaviourTreeBuilder builder, string name, UtSelector.Policy failurePolicy)
        {
            UtSelector node = new UtSelector(name);
            node.failurePolicy = failurePolicy;
            builder.AttachToRootOrParent(node);
            builder.bhvStack.Push(node);

            return builder;
        }

        public static BehaviourTreeBuilder EndUtSelector(this BehaviourTreeBuilder builder)
        {
            builder.bhvStack.Pop();
            return builder;
        }
    }

}