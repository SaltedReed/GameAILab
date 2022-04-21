using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAILab.Generation;

public class Test_PointGraph : MonoBehaviour
{
    public Transform root;
    public float maxDistance;
    public LayerMask raycastLayer;

    public PointGraph graph;

    private bool scanned;

    // Start is called before the first frame update
    void Start()
    {
        graph = new PointGraph();
        graph.root = root;
        graph.maxDistance = maxDistance;
        graph.raycastLayer = raycastLayer;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            graph.Scan();
            scanned = true;
        }
    }

    private void OnDrawGizmos()
    {
        if (!scanned)
            return;

        Gizmos.color = Color.yellow;
        foreach (WaypointNode n in graph.nodes)
        {
            foreach (WaypointEdge e in n.edges)
            {
                Gizmos.DrawLine(n.worldPos, e.to.worldPos);
            }
        }
    }
}
