using System.Collections.Generic;
using UnityEngine;
using GameAILab.SpaceRepre;
using GameAILab.Core;
using GameAILab.Profile;

namespace GameAILab.Pathfinding
{

    public sealed class Dijkstra_Basic
    {
        private class PathNode
        {
            public GraphNode node = null;
            public GraphNode parent = null;
            public float g = float.MaxValue;
            public bool isClosed = false;
        }


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
            LabProfiler.StartProfile("Dijkstra Basic");

            path = null;
            this.start = start;
            this.end = end;
            this.graph = graph;
            m_current = null;
            allNodes = new List<GraphNode>();

            m_pathNodes = new PathNode[graph.NodeCount];
            for (int i=0; i<m_pathNodes.Length; ++i)
            {
                m_pathNodes[i] = new PathNode();
                m_pathNodes[i].node = graph.GetNode(i);
            }

            m_open = new List<PathNode>();

            PathNode pn = m_pathNodes[start.NodeIndex];
            pn.parent = start;
            pn.g = 0.0f;

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

            LabProfiler.EndProfile("Dijkstra Basic");
        }

        private PathNode GetBestNode()
        {
            if (m_open.Count <= 0)
                return null;

            int resultIndex = 0;
            PathNode result = m_open[0];

            for (int i = 1; i < m_open.Count; ++i)
            {
                if (m_open[i].g < result.g)
                {
                    result = m_open[i];
                    resultIndex = i;
                }
            }

            m_open.RemoveAt(resultIndex);
            return result;
        }

        private void UpdatePathNode(PathNode pn)
        {
            if (pn.isClosed)
                return;

            if (pn.parent is null)
            {
                OpenNode(pn);
            }

            float newCost = m_current.g + pn.node.Cost;
            if (newCost < pn.g)
            {
                pn.parent = m_current.node;
                pn.g = newCost;
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


    public sealed class Dijkstra_PriorityQueue
    {
        private class PathNode
        {
            public GraphNode node = null;
            public GraphNode parent = null;
            public float g = float.MaxValue;
            public bool isClosed = false;

            public override string ToString()
            {
                string str = "";

                str += "node index: " + node.NodeIndex + "\n";
                str += "node world position: " + node.WorldPos + "\n";
                str += "parent index: " + parent.NodeIndex + "\n";
                str += "g: " + g + "\n";
                str += "is close: " + isClosed + "\n";

                return str;
            }
        }


        public NavGraph graph;
        public GraphNode start;
        public GraphNode end;
        public GraphNode[] path;
        public List<GraphNode> allNodes;

        private PathNode[] m_pathNodes;
        private BinaryHeap<PathNode> m_open;
        private PathNode m_current;

        public void StartFind(NavGraph graph, GraphNode start, GraphNode end)
        {
            LabProfiler.StartProfile("Dijkstra PQ");

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
            { return (int)(n2.g*1000) - (int)(n1.g*1000); });

            PathNode pn = m_pathNodes[start.NodeIndex];
            pn.parent = start;
            pn.g = 0.0f;

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

            LabProfiler.EndProfile("Dijkstra PQ");
        }

        private PathNode GetBestNode()
        {
            if (m_open.Count <= 0)
                return null;

            return m_open.RemoveTop();
        }

        private void UpdatePathNode(PathNode pn)
        {
            if (pn.isClosed)
                return;

            if (pn.parent is null)
            {
                OpenNode(pn);
            }

            float newCost = m_current.g + pn.node.Cost;
            if (newCost < pn.g)
            {
                pn.parent = m_current.node;
                pn.g = newCost;
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