using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    public Transform seeker, target;
    Grid grid;
    void Awake()
    {
        seeker = GameObject.Find("Seeker").transform;
        target = GameObject.Find("Target").transform;
        grid = GetComponent<Grid>();
    }
    void Start()
    {

    }
    void Update()
    {
        FindPath(seeker.position, target.position);
    }
    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.NodeFromWorldPoint(startPos);//获取开始节点
        Node targetNode = grid.NodeFromWorldPoint(targetPos);//获取目标节点

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);//起始节点添加到开放列表

        while (openSet.Count > 0) //开放列表不为空表示存在可以搜索的节点,寻路
        {
            Node currentNode = openSet[0];
            //这里效率太慢,每次都遍历整个openSet来寻找最低消耗的节点,使用堆来优化,维护一个堆数据结构
            for (int i = 0; i < openSet.Count; i++)
            {
                //若查询到F(总消耗)小于当前节点的,或者总消耗相同,但H(剩余耗费)小于当前节点的
                if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            { //寻找到目标节点
                RetracePath(startNode, targetNode);
                return;
            }
            //将邻居添加到开列表
            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour)) //该邻居节点是障碍物,或者已经检测过了
                {
                    continue;
                }
                //计算新的G,从当前节点移动到目标邻居节点
                int newMoveCost = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMoveCost < neighbour.gCost || !openSet.Contains(neighbour)) //已消耗小于
                {
                    neighbour.gCost = newMoveCost;//更新gCost
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;
                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                        neighbour.existedInOpenset = true;
                    }
                }
            }
        }
    }
    //获取节点之间的距离
    int GetDistance(Node nodeA, Node nodeB)
    {
        int disX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int disY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        int min = disX > disY ? disY : disX;
        int max = disX <= disY ? disY : disX;
        return 14 * min + 10 * max;
    }

    //回溯路径
    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();//逆置一下
        grid.path = path;
    }
}
