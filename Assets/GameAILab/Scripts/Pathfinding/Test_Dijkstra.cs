using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAILab;
using GameAILab.Profile;
using GameAILab.Pathfinding;
using GameAILab.SpaceRepre;

public class Test_Dijkstra : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;

    public bool drawAllSearchedNodes_basic = false;

    public Color color_start = Color.green;
    public Color color_end = Color.green;

    public Color color_searched_basic = Color.yellow;

    public Color color_basic = Color.green;
    public Color color_priorityQueue = Color.blue;

    private GraphNode start;
    private GraphNode end;

    private Dijkstra_Basic basic;
    private Dijkstra_PriorityQueue pq;

    private bool found_basic = false;
    private bool found_pq = false;

    private Vector3 nodeSize;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            GridGraph gg = Lab.graphs[0] as GridGraph;

            nodeSize = new Vector3(gg.NodeSize, 1.0f, gg.NodeSize) * 0.9f;

            start = gg.GetNearest(startPoint.position);
            end = gg.GetNearest(endPoint.position);

            basic = new Dijkstra_Basic();
            pq = new Dijkstra_PriorityQueue();


            basic.StartFind(gg, start, end);
            found_basic = true;

            pq.StartFind(gg, start, end);
            found_pq = true;
        }
        

        if (Input.GetKeyDown(KeyCode.R))
        {
            LabProfiler.PrintResults();
        }
    }

    void OnDrawGizmos()
    {
        if (start != null)
        {
            Gizmos.color = color_start;
            Gizmos.DrawSphere(start.WorldPos, 0.3f);
        }
        if (end != null)
        {
            Gizmos.color = color_end;
            Gizmos.DrawSphere(end.WorldPos, 0.3f);
        }

        if (found_basic)
        {
            if (drawAllSearchedNodes_basic)
            {
                Gizmos.color = color_searched_basic;
                foreach (GraphNode n in basic.allNodes)
                    Gizmos.DrawCube(n.WorldPos, nodeSize);
            }

            Gizmos.color = color_basic;
            for (int i = 0; i < basic.path.Length - 1; ++i)
                Gizmos.DrawLine(basic.path[i].WorldPos, basic.path[i + 1].WorldPos);
        }
        if (found_pq)
        { 
            Gizmos.color = color_priorityQueue;
            for (int i = 0; i < pq.path.Length - 1; ++i)
                Gizmos.DrawLine(pq.path[i].WorldPos, pq.path[i + 1].WorldPos);
        }
    }
}
