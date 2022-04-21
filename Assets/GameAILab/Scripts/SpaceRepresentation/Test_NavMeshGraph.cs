using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAILab.Generation;

public class Test_NavMeshGraph : MonoBehaviour
{
    public Mesh mesh;
    public MeshHolder meshHolder;
    public NavMeshGraph graph;

    private bool scanned = false;

    // Start is called before the first frame update
    void Start()
    {
        meshHolder = new MeshHolder();
        meshHolder.verts = mesh.vertices;
        meshHolder.indices = mesh.triangles;

        graph = new NavMeshGraph();
        graph.meshHolder = meshHolder;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            graph.Scan();
            scanned = true;
        }
    }

    private void OnDrawGizmos()
    {
        if (!scanned)
            return;


        foreach (TriMeshNode n in graph.nodes)
        {
            Gizmos.color = Color.yellow;

            Vector3 v0 = n.GetVertex(0);
            Vector3 v1 = n.GetVertex(1);
            Vector3 v2 = n.GetVertex(2);

            Gizmos.DrawSphere(v0, 0.1f);
            Gizmos.DrawSphere(v1, 0.1f);
            Gizmos.DrawSphere(v2, 0.1f);

            Gizmos.DrawLine(v0, v1);
            Gizmos.DrawLine(v1, v2);
            Gizmos.DrawLine(v0, v2);

            Gizmos.color = Color.green;

            foreach (TriMeshEdge e in n.edges)
            {
                Gizmos.DrawLine(n.worldPos, e.to.worldPos);
            }
        }
    }
}
