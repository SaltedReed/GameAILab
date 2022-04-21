using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAILab;
using GameAILab.SpaceRepre;
using GameAILab.Pathfinding;
using GameAILab.Profile;


public class Test_IDAstar : MonoBehaviour
{
    public enum HeuristicType
    {
        FourNeighbor,
        EuclideanDistance
    }

    public Transform startPoint;
    public Transform endPoint;

    public HeuristicType heuType;

    public Color color_start = Color.green;
    public Color color_end = Color.green;

    public Color color_basic = Color.green;

    private GraphNode start;
    private GraphNode end;

    private IDAstar basic;

    private bool found_basic = false;

    private Vector3 nodeSize;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            GridGraph gg = Lab.graphs[0] as GridGraph;

            nodeSize = new Vector3(gg.NodeSize, 1.0f, gg.NodeSize) * 0.9f;

            start = gg.GetNearest(startPoint.position);
            end = gg.GetNearest(endPoint.position);

            basic = new IDAstar();
            switch (heuType)
            {
                case HeuristicType.FourNeighbor:
                    basic.heuristic = new FourNeighGridHeuristic();
                    break;
                case HeuristicType.EuclideanDistance:
                    basic.heuristic = new EuclideanDistHeuristic();
                    break;
            }

            basic.StartFind(gg, start, end);
            found_basic = true;
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
            Gizmos.color = color_basic;
            for (int i = 0; i < basic.path.Length - 1; ++i)
                Gizmos.DrawLine(basic.path[i].WorldPos, basic.path[i + 1].WorldPos);
        }
    }

}
