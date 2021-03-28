using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.Linq;
using Debug = UnityEngine.Debug;

public class PathFinding : MonoBehaviour {
    //public Transform seeker, target;
    Grid grid;
    private PathRequestManager requestManager;
    void Awake() {
        requestManager = GetComponent<PathRequestManager>();
        grid = GetComponent<Grid>();
    }
    void Start() {

    }
    void Update() {
        // if (Input.GetButtonDown("Jump")) {
        //     FindPath(seeker.position, target.position);
        // }
    }

    public void StartFindPath(Vector3 startPos, Vector3 targetPos) {
        StartCoroutine(FindPath(startPos, targetPos));
    }

    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos) {
        //定时器
        Stopwatch sw = new Stopwatch();
        sw.Start();
        Vector3[] wayPoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPoint(startPos); //获取开始节点
        Node targetNode = grid.NodeFromWorldPoint(targetPos); //获取目标节点
        if (startNode.walkable && targetNode.walkable) {
            //List<Node> openSet = new List<Node>();//
            //使用堆的数据结构来构造openSet
            Heap<Node> openSet = new Heap<Node>(grid.MapSize);
            HashSet<Node> closedSet = new HashSet<Node>();

            openSet.Add(startNode); //起始节点添加到开放列表
            //开放列表不为空表示存在可以搜索的节点,寻路
            while (openSet.Count > 0) {
                // Node currentNode = openSet[0];
                // //这里效率太慢,每次都遍历整个openSet来寻找最低消耗的节点,使用堆来优化,维护一个堆数据结构
                // for (int i = 0; i < openSet.Count; i++) {
                //     //若查询到F(总消耗)小于当前节点的,或者总消耗相同,但H(剩余耗费)小于当前节点的
                //     if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost) {
                //         currentNode = openSet[i];
                //     }
                // }
                // openSet.Remove(currentNode);
                Node currentNode = openSet.RemoveFirst();//移除堆顶,小根堆
                closedSet.Add(currentNode);

                if (currentNode == targetNode) { //寻找到目标节点
                    sw.Stop();
                    print("Time : " + sw.ElapsedMilliseconds + "ms");
                    pathSuccess = true;
                    //RetracePath(startNode, targetNode);
                    break;
                }
                //将邻居添加到开列表
                foreach (Node neighbour in grid.GetNeighbours(currentNode)) {
                    if (!neighbour.walkable || closedSet.Contains(neighbour)) { //该邻居节点是障碍物,或者已经检测过了
                        continue;
                    }
                    //计算新的G,从当前节点移动到目标邻居节点
                    int newMoveCost = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newMoveCost < neighbour.gCost || !openSet.Contains(neighbour)) { //已消耗小于
                        neighbour.gCost = newMoveCost; //更新gCost
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;
                        if (!openSet.Contains(neighbour)) {
                            openSet.Add(neighbour);
                            neighbour.existedInOpenset = true; //标记是否存在过
                        } else {
                            openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }
        }
        yield return null;
        if (pathSuccess) {
            wayPoints = RetracePath(startNode, targetNode);
        }
        requestManager.FinishedProcessingPath(wayPoints, pathSuccess);
    }
    //获取节点之间的距离
    int GetDistance(Node nodeA, Node nodeB) {
        int disX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int disY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        int min = disX > disY ? disY : disX;
        int max = disX <= disY ? disY : disX;
        return 14 * min + 10 * max;
    }

    Vector3[] SimplifyPath(List<Node> path) {
        List<Vector3> wayPoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;
        for (int i = 1; i < path.Count; i++) {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if (directionOld != directionNew) {
                wayPoints.Add(path[i].worldPosition);
            }

            directionOld = directionNew;
        }
        Debug.Log(wayPoints.Count);
        return wayPoints.ToArray();
    }

    //回溯路径
    Vector3[] RetracePath(Node startNode, Node endNode) {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;
        while (currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        Vector3[] wayPoints = SimplifyPath(path);
        // path.Reverse();//逆置一下
        //grid.path = path;
        Array.Reverse(wayPoints);
        return wayPoints;
    }
}
