using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAILab.SpaceRepre;
using GameAILab.Profile;
using GameAILab.Pathfinding;

public class Test_GridGraph : MonoBehaviour
{
    public int nodeSize;
    public string mapPath;

    private GridGraph graph;
    private bool scanned = false;


    void Start()
    {
        graph = new GridGraph();
        graph.NodeSize = nodeSize;
        graph.Center = transform.position;
        graph.checkCollision = false;        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G) && !scanned)
        {
            graph.BuildFromFile(mapPath);
            scanned = true;
        }
    }

    void OnDrawGizmos()
    {
        if (!scanned)
            return;

        Gizmos.color = Color.yellow;
        foreach (GridNode n in graph.nodes)
        {
            if (!n.Walkable)
                continue;

            Gizmos.DrawWireCube(n.WorldPos, new Vector3(nodeSize, 1.0f, nodeSize));
        }        
    }

}