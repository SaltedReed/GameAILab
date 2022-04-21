using UnityEngine;

namespace GameAILab.SpaceRepre
{

    public abstract class NavGraph
    {
        public abstract int NodeCount { get; }

        public NavGraph()
        {
            Lab.graphs.Add(this);
        }

        public abstract GraphNode GetNode(int index);
        public abstract GraphNode[] GetSuccessors(GraphNode from);
        public abstract GraphNode GetNearest(Vector3 point);
    }


}