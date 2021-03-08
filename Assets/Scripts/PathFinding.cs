using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{

    Grid grid;
    void Awake()
    {
        grid = GetComponent<Grid>();
    }
    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.NodeFromWorldPoint(startPos);//获取开始节点
        Node targetNode = grid.NodeFromWorldPoint(targetPos);//获取目标节点

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);//起始节点添加到开放列表
        while (openSet.Count > 0)
        {

        }
    }
}
