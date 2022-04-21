//#define LOW_H_FIRST

using System.Collections.Generic;
using UnityEngine;
using GameAILab.SpaceRepre;
using GameAILab.Core;
using GameAILab.Profile;

namespace GameAILab.Pathfinding
{

    public sealed class Astar_Basic
    {
        private class PathNode
        {
            public GraphNode node = null;
            public GraphNode parent = null;
            public float g = float.MaxValue;
            public float h;
            public float totalCost = float.MaxValue;
            public bool isClosed = false;
        }

        public Heuristic heuristic;
        public NavGraph graph;
        public GraphNode start;
        public GraphNode end;
        public GraphNode[] path;
        public List<GraphNode> allNodes;

        private PathNode[] m_pathNodes;
        private List<PathNode> m_open;
        private PathNode m_current;


        public void StartFind(NavGraph graph, GraphNode start, GraphNode end)
        {
            LabProfiler.StartProfile("Astar Basic");

            if (heuristic is null)
                heuristic = new FourNeighGridHeuristic();
            path = null;
            this.start = start;
            this.end = end;
            this.graph = graph;
            m_current = null;
            allNodes = new List<GraphNode>();

            m_pathNodes = new PathNode[graph.NodeCount];
            for (int i = 0; i < m_pathNodes.Length; ++i)
            {
                m_pathNodes[i] = new PathNode();
                m_pathNodes[i].node = graph.GetNode(i);
            }

            m_open = new List<PathNode>();

            PathNode pn = m_pathNodes[start.NodeIndex];
            pn.parent = start;
            pn.h = heuristic.Get(pn.node, end, graph);
            pn.g = 0.0f;
            pn.totalCost = pn.h + pn.g;

            OpenNode(pn);

            while (m_open.Count > 0)
            {
                m_current = GetBestNode();

                if (m_current.node == end)
                    break;

                GraphNode[] succs = graph.GetSuccessors(m_current.node);
                for (int i = 0; i < succs.Length; ++i)
                {
                    PathNode succPathNode = m_pathNodes[succs[i].NodeIndex];
                    UpdatePathNode(succPathNode);
                }
                m_current.isClosed = true;
            }

            BuildUpPath();

            LabProfiler.EndProfile("Astar Basic");
        }

        private PathNode GetBestNode()
        {
            LabProfiler.StartProfile("Astar Basic OpenList");
            if (m_open.Count <= 0)
                return null;

            int resultIndex = 0;
            PathNode result = m_open[0];

            for (int i = 1; i < m_open.Count; ++i)
            {
                if (m_open[i].totalCost < result.totalCost)
                {
                    result = m_open[i];
                    resultIndex = i;
                }
#if LOW_H_FIRST
                else if (m_open[i].totalCost == result.totalCost &&
                    m_open[i].h < result.h)
                {
                    result = m_open[i];
                    resultIndex = i;
                }
#endif
            }

            m_open.RemoveAt(resultIndex);
            LabProfiler.EndProfile("Astar Basic OpenList");

            return result;
        }

        private void UpdatePathNode(PathNode pn)
        {
            if (pn.isClosed)
                return;

            if (pn.parent is null)
            {
                pn.parent = m_current.node;
                pn.h = heuristic.Get(pn.node, end, graph);
                UpdateG(pn);
                pn.totalCost = pn.h + pn.g;

                OpenNode(pn);
            }
            else
            {
                float newCost = m_current.g + pn.node.Cost;
                if (newCost < pn.g)
                {
                    pn.parent = m_current.node;
                    UpdateGRecursive(pn);
                    pn.totalCost = pn.h + pn.g;
                }
            }
        }

        private void UpdateG(PathNode pn)
        {
            pn.g = m_current.g + pn.node.Cost;
        }

        private void UpdateGRecursive(PathNode pn)
        {
            UpdateG(pn);

            GraphNode[] succs = graph.GetSuccessors(pn.node);
            foreach (GraphNode s in succs)
            {
                PathNode spn = m_pathNodes[s.NodeIndex];
                if (spn.parent == pn.node)
                {
                    UpdateGRecursive(spn);
                }
            }
        }

        private void OpenNode(PathNode pn)
        {
            m_open.Add(pn);
            allNodes.Add(pn.node);
        }

        private void BuildUpPath()
        {
            List<GraphNode> nodeList = new List<GraphNode>();

            m_current = m_pathNodes[end.NodeIndex];
            while (m_current.parent != m_current.node)
            {
                nodeList.Insert(0, m_current.node);

                if (m_current.parent is null)
                {
                    Debug.Log($"Failed to find a path from {start.WorldPos} to {end.WorldPos}");
                    path = null;
                    return;
                }

                m_current = m_pathNodes[m_current.parent.NodeIndex];
            }
            nodeList.Insert(0, m_current.node);

            path = nodeList.ToArray();
            Debug.Log($"Path finding from {start.WorldPos} to {end.WorldPos} completed, path length {path.Length}, searched node number {allNodes.Count}");
        }

    }


    public sealed class Astar_PQ
    {
        private class PathNode
        {
            public GraphNode node = null;
            public GraphNode parent = null;
            public float g = float.MaxValue;
            public float h;
            public float totalCost = float.MaxValue;
            public bool isClosed = false;

            public override string ToString()
            {
                string str = "";

                str += "node index: " + node.NodeIndex + "\n";
                str += "node world position: " + node.WorldPos + "\n";
                str += "parent index: " + parent.NodeIndex + "\n";
                str += "g: " + g + "\n";
                str += "h: " + h + "\n";
                str += "total cost: " + totalCost + "\n";
                str += "is close: " + isClosed + "\n";

                return str;
            }
        }

        public Heuristic heuristic;
        public NavGraph graph;
        public GraphNode start;
        public GraphNode end;
        public GraphNode[] path;
        // for debug
        public List<GraphNode> allNodes;

        private PathNode[] m_pathNodes;
        private BinaryHeap<PathNode> m_open;
        private PathNode m_current;


        public void StartFind(NavGraph graph, GraphNode start, GraphNode end)
        {
            LabProfiler.StartProfile("Astar PQ");

            if (heuristic is null)
                heuristic = new FourNeighGridHeuristic();
            path = null;
            this.start = start;
            this.end = end;
            this.graph = graph;
            m_current = null;
            allNodes = new List<GraphNode>();

            m_pathNodes = new PathNode[graph.NodeCount];
            for (int i = 0; i < m_pathNodes.Length; ++i)
            {
                m_pathNodes[i] = new PathNode();
                m_pathNodes[i].node = graph.GetNode(i);
            }

            // todo: store Int3 position
            m_open = new BinaryHeap<PathNode>(m_pathNodes.Length, (PathNode n1, PathNode n2) =>
            {
#if LOW_H_FIRST
                if ((int)(n2.totalCost * 1000) != (int)(n1.totalCost * 1000))
                    return (int)(n2.totalCost*1000) - (int)(n1.totalCost*1000);
                else
                    return (int)(n2.h * 1000) - (int)(n1.h * 1000);
#else
                return (int)(n2.totalCost * 1000) - (int)(n1.totalCost * 1000);
#endif
            });

            PathNode pn = m_pathNodes[start.NodeIndex];
            pn.parent = start;
            pn.h = heuristic.Get(pn.node, end, graph);
            pn.g = 0.0f;
            pn.totalCost = pn.h + pn.g;

            OpenNode(pn);

            while (m_open.Count > 0)
            {
                m_current = GetBestNode();

                if (m_current.node == end)
                    break;

                GraphNode[] succs = graph.GetSuccessors(m_current.node);
                for (int i = 0; i < succs.Length; ++i)
                {
                    PathNode succPathNode = m_pathNodes[succs[i].NodeIndex];
                    UpdatePathNode(succPathNode);
                }
                m_current.isClosed = true;
            }

            BuildUpPath();

            LabProfiler.EndProfile("Astar PQ");
        }

        private PathNode GetBestNode()
        {
            LabProfiler.StartProfile("Astar PQ OpenList");
            if (m_open.Count <= 0)
                return null;

            PathNode node = m_open.RemoveTop();
            LabProfiler.EndProfile("Astar PQ OpenList");

            return node;
        }

        private void UpdatePathNode(PathNode pn)
        {
            if (pn.isClosed)
                return;

            if (pn.parent is null)
            {
                pn.parent = m_current.node;
                pn.h = heuristic.Get(pn.node, end, graph);
                UpdateG(pn, m_current);

                OpenNode(pn);
            }
            else
            {
                float newCost = m_current.g + pn.node.Cost;
                if (newCost < pn.g)
                {
                    pn.parent = m_current.node;
                    UpdateGRecursive(pn, m_current);
                }
            }
        }

        private void UpdateG(PathNode pn, PathNode parent)
        {
            pn.g = parent.g + pn.node.Cost;
            pn.totalCost = pn.h + pn.g;
        }

        private void UpdateGRecursive(PathNode pn, PathNode parent)
        {
            UpdateG(pn, parent);

            GraphNode[] succs = graph.GetSuccessors(pn.node);
            foreach (GraphNode s in succs)
            {
                PathNode spn = m_pathNodes[s.NodeIndex];
                if (spn.parent == pn.node)
                {
                    UpdateGRecursive(spn, pn);
                }
            }
        }

        private void OpenNode(PathNode pn)
        {
            m_open.Add(pn);
            allNodes.Add(pn.node);
        }

        private void BuildUpPath()
        {
            List<GraphNode> nodeList = new List<GraphNode>();

            m_current = m_pathNodes[end.NodeIndex];
            while (m_current.parent != m_current.node)
            {
                nodeList.Insert(0, m_current.node);

                if (m_current.parent is null)
                {
                    Debug.Log($"Failed to find a path from {start.WorldPos} to {end.WorldPos}");
                    path = null;
                    return;
                }

                m_current = m_pathNodes[m_current.parent.NodeIndex];
            }
            nodeList.Insert(0, m_current.node);

            path = nodeList.ToArray();
            Debug.Log($"Path finding from {start.WorldPos} to {end.WorldPos} completed, path length {path.Length}, searched node number {allNodes.Count}");
        }

    }

}