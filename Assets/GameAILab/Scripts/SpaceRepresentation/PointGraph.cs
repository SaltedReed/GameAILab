using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameAILab.Generation
{

	public class WaypointNode
    {
		public PointGraph graph;
		public int indexInGraph;
		public Vector3 worldPos;
		public WaypointEdge[] edges;
	}

	public class WaypointEdge
	{
		public WaypointNode to;
		public float cost;
	}

	public class PointGraph
    {
		public Transform root;
		public float maxDistance;
		public LayerMask raycastLayer;
		public WaypointNode[] nodes;

		public void Scan()
        {
			if (root is null)
            {
				Debug.LogError("PointGraph.root is null");
				return;
            }

			CreateNodes();
			CalculateNodes();
			CreateConnections();
        }

		private void CreateNodes()
        {
			int count = root.childCount;
			nodes = new WaypointNode[count];

			for (int i=0; i<count; ++i)
            {
				WaypointNode node = new WaypointNode();
				node.graph = this;
				node.indexInGraph = i;
				nodes[i] = node;
            }
        }

		private void CalculateNodes()
        {
			int index = 0;
			foreach (Transform child in root)
            {
				nodes[index].worldPos = child.position;
				++index;
            }
        }

		private void CreateConnections()
        {
			List<WaypointEdge> edges = new List<WaypointEdge>();
			for (int i=0; i<nodes.Length; ++i)
            {
				edges.Clear();
				WaypointNode node = nodes[i];

				for (int j=0; j<nodes.Length; ++j)
				{
					WaypointNode other = nodes[j];
					if (node == other)
                    {
						continue;
                    }

					float dst = (node.worldPos - other.worldPos).magnitude;
					if (CanConnect(node, other, dst))
                    {
						edges.Add(new WaypointEdge { to = other, cost = dst });
                    }
				}

				node.edges = edges.ToArray();
			}
        }

		private bool CanConnect(WaypointNode n1, WaypointNode n2, float distance)
        {
			if (maxDistance > 0.0f && distance > maxDistance)
            {
				return false;
            }

			if (Physics.Linecast(n1.worldPos, n2.worldPos, raycastLayer))
            {
				return false;
            }

			return true;
        }

	}

}