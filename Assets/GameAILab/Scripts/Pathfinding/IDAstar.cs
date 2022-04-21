//#define DEBUG_IDASTAR

using System.Collections.Generic;
using GameAILab.Profile;
using GameAILab.SpaceRepre;

namespace GameAILab.Pathfinding
{

    public sealed class IDAstar
    {
        public Heuristic heuristic;
        public NavGraph graph;
        public GraphNode start;
        public GraphNode end;
        public GraphNode[] path;

        private float m_threshold;
        private float m_tempThreshold;
        private bool m_found;
        private List<GraphNode> m_tempPath;
        private List<GraphNode> m_visited;


        public void StartFind(NavGraph graph, GraphNode start, GraphNode end)
        {
            LabProfiler.StartProfile("IDAstar");

            if (heuristic is null)
                heuristic = new FourNeighGridHeuristic();
            path = null;
            this.graph = graph;
            this.start = start;
            this.end = end;
            m_found = false;
            m_tempPath = new List<GraphNode>();
            m_visited = new List<GraphNode>();

            m_threshold = heuristic.Get(start, end, graph);

            while (true)
            {
                m_tempThreshold = float.MaxValue;

                Dfs(start, 0.0f);
                if (m_found)
                {
                    m_tempPath.Insert(0, start);
                    path = m_tempPath.ToArray();
                    break;
                }

                m_threshold = m_tempThreshold;
                ClearVisited();
            }

            LabProfiler.EndProfile("IDAstar");
        }

        private void Dfs(GraphNode node, float distance)
        {
#if DEBUG_IDASTAR
            string str = $"======node: {node.DebugString()}\ndistance: {distance}\nthreshold: {m_threshold}\n";
            UnityEngine.Debug.Log(str);
#endif
            if (node == end)
            {
                m_found = true;
                m_tempPath.Insert(0, node);

#if DEBUG_IDASTAR
                str = "found";
                UnityEngine.Debug.Log(str);
#endif
                return;
            }

            if (IsVisited(node))
            {
                return;
            }
            SetVisited(node);

            float h = heuristic.Get(node, end, graph);
            float f = distance + h;

#if DEBUG_IDASTAR
            UnityEngine.Debug.Log($"g: {distance}, h: {h}, f: {f}, threshold: {m_threshold}");
#endif

            if (f > m_threshold)
            {
                if (f < m_tempThreshold)
                {
                    m_tempThreshold = f;
                }

                return;
            }

            GraphNode[] succs = graph.GetSuccessors(node);
            for (int i=0; i<succs.Length; ++i)
            {
                GraphNode to = succs[i];
                if (IsVisited(to))
                {
#if DEBUG_IDASTAR
                    str = $"visited {to.NodeIndex}, skip it\n";
                    UnityEngine.Debug.Log(str);
#endif
                    continue;
                }

#if DEBUG_IDASTAR
                str = $"to visit {to.NodeIndex}";
                UnityEngine.Debug.Log(str);
#endif
                Dfs(to, distance + to.Cost);
                if (m_found)
                {
                    m_tempPath.Insert(0, node);

#if DEBUG_IDASTAR
                    str = "found";
                    UnityEngine.Debug.Log(str);
#endif
                    return;
                }
            }
        }

        private bool IsVisited(GraphNode node)
        {
            return m_visited.Contains(node);
        }

        private void SetVisited(GraphNode node)
        {
            m_visited.Add(node);
        }
                        
        private void ClearVisited()
        {
            m_visited.Clear();
        }

    }

}