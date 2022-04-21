using UnityEngine;
using GameAILab.SpaceRepre;

namespace GameAILab.Pathfinding
{

    public abstract class Heuristic
    {
        public abstract float Get(GraphNode s, GraphNode g, NavGraph graph);
    }

    public sealed class FourNeighGridHeuristic : Heuristic
    {
        public override float Get(GraphNode s, GraphNode g, NavGraph graph)
        {
            GridNode gs = s as GridNode;
            GridNode gg = g as GridNode;

            return Mathf.Abs(gg.XCoord - gs.XCoord) + Mathf.Abs(gg.ZCoord - gs.ZCoord);
        }
    }

    public sealed class EuclideanDistHeuristic : Heuristic
    {
        public override float Get(GraphNode s, GraphNode g, NavGraph graph)
        {
            return (s.WorldPos - g.WorldPos).magnitude;
        }
    }



}