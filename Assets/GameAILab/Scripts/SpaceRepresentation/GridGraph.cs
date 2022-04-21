using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace GameAILab.SpaceRepre
{

    public class GridGraph : NavGraph
    {
        // [units: cell]
        public int Width { get; set; }
        // [units: cell]
        public int Depth { get; set; }
        // [units: world]
        public int NodeSize { get; set; }
        // [units: world]
        public Vector3 Center { get; set; }

        public GridNode[] nodes;
        public override int NodeCount => nodes is null ? 0 : nodes.Length;

        public bool checkCollision = true;
        public LayerMask raycastLayer;

        private Matrix4x4 m_graph2World;
        private Matrix4x4 m_world2Graph;

        private readonly int[] m_neighOffsetsX = { -1, 0, 1, 0 };
        private readonly int[] m_neighOffsetsZ = { 0, 1, 0, -1 };
        private readonly int[] m_neighOffsetsIndex = new int[4];


        public override GraphNode GetNode(int index)
        {
            if (index < 0 || index >= NodeCount)
                return null;
            return nodes[index];
        }

        public override GraphNode[] GetSuccessors(GraphNode from)
        {
            GridNode node = from as GridNode;
            int n = 4;
            List<GraphNode> succs = new List<GraphNode>();

            for (int i=0; i<n; ++i)
            {
                int ox = node.XCoord + m_neighOffsetsX[i];
                int oz = node.ZCoord + m_neighOffsetsZ[i];
                if (ox >= 0 && ox < Width && oz >= 0 && oz < Depth)
                {
                    GridNode other = nodes[node.NodeIndex + m_neighOffsetsIndex[i]];
                    if (CanConnect(node, other))
                    {
                        succs.Add(other);
                    }
                }
            }

            return succs.ToArray();
        }

        public override GraphNode GetNearest(Vector3 point)
        {
            if (nodes is null || nodes.Length <= 0)
                return null;

            GraphNode result = nodes[0];
            float minSqrDist = Vector3.SqrMagnitude(point - result.WorldPos);

            for (int i=1; i<nodes.Length; ++i)
            {
                Vector3 npos = nodes[i].WorldPos;
                float sqrDist = Vector3.SqrMagnitude(point - npos);

                if (sqrDist >= minSqrDist)
                    continue;

                // if the node is close enough, then ignore the rest
                if (sqrDist < 0.001f)
                {
                    result = nodes[i];
                    break;
                }
                else
                {
                    result = nodes[i];
                    minSqrDist = sqrDist;
                }
            }

            return result;
        }


        public void BuildFromFile(string path)
        {
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    ParseGraphInfo(reader);

                    if (Depth <= 0 || Depth > 1024 || Width <= 0 || Width > 1024)
                    {
                        throw new FileLoadException();
                    }

                    CreateNodesFromFile(reader);
                }
            }
            catch (FileNotFoundException e)
            {
                Debug.LogError(e.Message + "\nStopped building Graph");
            }

            UpdateMatrix();
            UpdateOffsets();

            CalculateNodes();
        }

        private void ParseGraphInfo(StreamReader reader)
        {
            string line;
            string[] words;

            // read type
            line = reader.ReadLine();

            // read Depth
            line = reader.ReadLine();
            words = line.Split(' ');
            if ("height" != words[0])
            {
                throw new FileLoadException();
            }
            Depth = int.Parse(words[1]);

            // read Width
            line = reader.ReadLine();
            words = line.Split(' ');
            if ("width" != words[0])
            {
                throw new FileLoadException();
            }
            Width = int.Parse(words[1]);

            // read "map"
            line = reader.ReadLine();
        }

        private void CreateNodesFromFile(StreamReader reader)
        {
            nodes = new GridNode[Depth * Width];

            // the bottom-left node has index zero
            int index = Width * (Depth - 1);
            for (int z = Depth - 1; z >= 0; --z)
            {
                string line = reader.ReadLine();
                for (int x = 0; x < Width; ++x)
                {
                    char nodeInfo = line[x];

                    GridNode node = new GridNode();
                    node.Graph = this;
                    node.NodeIndex = index;
                    node.XCoord = x;
                    node.ZCoord = z;
                    node.Walkable = nodeInfo == '.';
                    nodes[index] = node;

                    ++index;
                }
                index -= Width + Width;
            }
        }


        public void Scan()
        {
            UpdateMatrix();
            UpdateOffsets();

            CreateNodes();
            CalculateNodes();
        }

        public Vector3 Graph2World(int x, int z, float height = 0.0f)
        {
            return m_graph2World.MultiplyPoint3x4(new Vector3(x + 0.5f, height, z + 0.5f));
        }

        public void World2Graph(Vector3 point, out int x, out int z)
        {
            Vector3 pos = m_world2Graph.MultiplyPoint3x4(point);
            x = (int)(pos.x - 0.5f);
            z = (int)(pos.z - 0.5f);
        }

        private void CreateNodes()
        {
            nodes = new GridNode[Width * Depth];

            int index = 0;
            for (int z = 0; z < Depth; ++z)
            {
                for (int x = 0; x < Width; ++x)
                {
                    GridNode node = new GridNode();
                    node.Graph = this;
                    node.NodeIndex = index;
                    node.XCoord = x;
                    node.ZCoord = z;
                    nodes[index] = node;

                    ++index;
                }
            }
        }

        private void CalculateNodes()
        {
            int count = Depth * Width;
            for (int i = 0; i < count; ++i)
            {
                GridNode node = nodes[i];
                node.WorldPos = Graph2World(node.XCoord, node.ZCoord);

                if (!checkCollision)
                {
                    continue;
                }

                if (Physics.OverlapBox(node.WorldPos, new Vector3(NodeSize, 1.0f, NodeSize), Quaternion.identity,
                        raycastLayer).Length > 0)
                {
                    node.Walkable = false;
                }
            }
        }


        private bool CanConnect(GridNode n1, GridNode n2)
        {
            if (!n1.Walkable || !n2.Walkable)
            {
                return false;
            }
            return true;
        }

        private void UpdateOffsets()
        {
            m_neighOffsetsIndex[0] = -1;
            m_neighOffsetsIndex[1] = Width;
            m_neighOffsetsIndex[2] = 1;
            m_neighOffsetsIndex[3] = -Width;
        }

        private void UpdateMatrix()
        {
            Vector3 origin = Center - new Vector3(Width * NodeSize, 0.0f, Depth * NodeSize) * 0.5f;
            m_graph2World = Matrix4x4.TRS(origin, Quaternion.identity, new Vector3(NodeSize, 1.0f, NodeSize));
            m_world2Graph = Matrix4x4.Inverse(m_graph2World);
        }

    }

}