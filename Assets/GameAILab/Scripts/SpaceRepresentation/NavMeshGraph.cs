using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAILab.Generation
{

    public class MeshHolder
    {
        public Vector3[] verts;
        public int[] indices;
    }


    public class TriMeshNode
    {
        public NavMeshGraph graph;
        public int indexInGraph;
        public MeshHolder meshHolder;
        public int v0;
        public int v1;
        public int v2;
        public TriMeshEdge[] edges;
        public Vector3 worldPos;

        public Vector3 GetVertex(int index)
        {
            int v = v0;
            if (1 == index)      v = v1;
            else if (2 == index) v = v2;

            Vector3 vert = meshHolder.verts[v];

            return vert;
        }

        public int GetVertexIndex(int index)
        {
            if (1 == index) return v1;
            if (2 == index) return v2;
            return v0;
        }
    }

    public class TriMeshEdge
    {
        public TriMeshNode to;
        public float cost;
    }


    public class NavMeshGraph
    {
        private struct MeshEdgeVector
        {
            public int fromIndex;
            public int toIndex;

            public MeshEdgeVector(int from, int to)
            {
                fromIndex = from;
                toIndex = to;
            }
        }

        public TriMeshNode[] nodes;

        // temp
        public MeshHolder meshHolder;

        public void Scan()
        {
            CreateNodes();
            CalculateNodes();
            CreateConnections();
        }

        private void CreateNodes()
        {
            int n = 3;
            nodes = new TriMeshNode[meshHolder.indices.Length / 3];
            for (int i=0; i<nodes.Length; ++i)
            {
                TriMeshNode node = new TriMeshNode();
                node.graph = this;
                node.indexInGraph = i;
                node.meshHolder = meshHolder;
                node.v0 = meshHolder.indices[i * n + 0];
                node.v1 = meshHolder.indices[i * n + 1];
                node.v2 = meshHolder.indices[i * n + 2];
                nodes[i] = node;
            }
        }
    
        private void CalculateNodes()
        {
            for (int i=0; i<nodes.Length; ++i)
            {
                TriMeshNode node = nodes[i];

                Vector3 v0 = node.GetVertex(0);
                Vector3 v1 = node.GetVertex(1);
                Vector3 v2 = node.GetVertex(2);

                node.worldPos = new Vector3(
                    (v0.x + v1.x + v2.x) / 3.0f,
                    (v0.y + v1.y + v2.y) / 3.0f,
                    (v0.z + v1.z + v2.z) / 3.0f
                    );
            }
        }

        private void CreateConnections()
        {
            int n = 3;

            // record neighbors
            Dictionary<MeshEdgeVector, int> edgeNodeMap = new Dictionary<MeshEdgeVector, int>();
            for (int i=0; i<nodes.Length; ++i)
            {
                TriMeshNode node = nodes[i];
                for (int v=0; v<n; ++v)
                {
                    int fromIndex = node.GetVertexIndex(v);
                    int toIndex = node.GetVertexIndex((v+1)%n);
                    MeshEdgeVector vec = new MeshEdgeVector(fromIndex, toIndex);
                    if (!edgeNodeMap.ContainsKey(vec))
                    {
                        edgeNodeMap.Add(vec, i);
                    }
                }
            }

            // create connections between neighbors
            List<TriMeshEdge> edges = new List<TriMeshEdge>();
            for (int i = 0; i < nodes.Length; ++i)
            {
                edges.Clear();
                TriMeshNode node = nodes[i];

                for (int v = 0; v < n; ++v)
                {
                    int fromIndex = node.GetVertexIndex(v);
                    int toIndex = node.GetVertexIndex((v + 1) % n);
                    MeshEdgeVector vec = new MeshEdgeVector(toIndex, fromIndex);
                    int otherIndex;
                    if (edgeNodeMap.TryGetValue(vec, out otherIndex))
                    {
                        TriMeshNode other = nodes[otherIndex];
                        edges.Add(new TriMeshEdge { to = other, cost = (node.worldPos-other.worldPos).magnitude });
                    }
                }

                node.edges = edges.ToArray();
            }
        }

    }

}