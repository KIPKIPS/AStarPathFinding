using System;
using System.Drawing;
using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {
    public bool onlyDisplayPathGizoms;
    public Vector2 gridWorldSize;//节点坐标
    public float nodeRadius;//节点半径
    Node[,] grid;
    public LayerMask unwalkableMask;
    int gridSizeX, gridSizeY;//节点的长宽数量
    float nodeDiameter;//节点直径
    GameObject wallObj;
    Vector3 worldBottomLeft;//左下角坐标
    public Transform seeker;
    public Transform target;
    public List<Node> path;
    float lastTime = 0f;
    void Awake() {
        seeker = GameObject.Find("Seeker").transform;
        target = GameObject.Find("Target").transform;
    }
    void Update() {
        lastTime += Time.deltaTime;
    }
    void OnDrawGizmos() { //用来显示一个三维向量的包围框
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        if (onlyDisplayPathGizoms) {
            if (path != null) {
                foreach (Node n in path) {
                    Gizmos.color = Color.black;
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));//着色
                }
            }
        } else {
            if (grid != null) {
                Node playerNode = NodeFromWorldPoint(seeker.transform.position);
                Node targetNode = NodeFromWorldPoint(target.transform.position);
                foreach (Node n in grid) {
                    Gizmos.color = playerNode == n ? Color.cyan : (targetNode == n ? Color.green : (n.walkable ? Color.white : Color.red)); //起始点则置为青色,目标绿色,否则根据是否障碍物进行判断,障碍物红色,通路白色
                    if (path != null) {
                        if (path.Contains(n) && n != targetNode) {
                            Gizmos.color = Color.black;
                        }
                        //else if (n.existedInOpenset) {
                        //Gizmos.color = Color.magenta;
                        //}
                    }
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));//着色
                }
            }
        }

    }
    void Start() {
        seeker.gameObject.SetActive(true);
        target.gameObject.SetActive(true);
        wallObj = GameObject.Find("Walls");
        wallObj.SetActive(true);
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);//四舍五入到整数
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreatGrid();
    }

    public int MapSize {
        get {
            return gridSizeX * gridSizeY;
        }
    }
    void CreatGrid() {
        grid = new Node[gridSizeX, gridSizeY];//创建一个Node节点二维数组
        worldBottomLeft = transform.position + Vector3.left * gridWorldSize.x / 2 + Vector3.back * gridWorldSize.y / 2;//网格的左下角坐标
        for (int x = 0; x < gridSizeX; x++) {
            for (int y = 0; y < gridSizeY; y++) {
                //计算出每一个节点的坐标
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));//定义的球体是否和物体相撞,处于layerMask的物体
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
        wallObj.SetActive(false);
        seeker.gameObject.SetActive(false);
        target.gameObject.SetActive(false);
        //NodeFromWorldPoint(new Vector3(0, 0, 0));
    }

    //根据节点中心坐标返回对应的Node
    public Node NodeFromWorldPoint(Vector3 worldPosition) { //计算索引
        int xIndex = Mathf.RoundToInt(worldPosition.x - worldBottomLeft.x - 0.5f);
        int yIndex = Mathf.RoundToInt(worldPosition.z - worldBottomLeft.z - 0.5f);
        //print((worldPosition.x - worldBottomLeft.x).ToString() + "  " + (worldPosition.z - worldBottomLeft.z).ToString());
        //print("x : " + xIndex + " , y : " + yIndex + "");
        xIndex = xIndex < 0 ? 0 : xIndex >= gridSizeX ? gridSizeX - 1 : xIndex;
        yIndex = yIndex < 0 ? 0 : yIndex >= gridSizeY ? gridSizeY - 1 : yIndex;
        return grid[xIndex, yIndex];
    }

    //获取一个节点的邻居节点
    public List<Node> GetNeighbours(Node node) {
        List<Node> neighboursList = new List<Node>();
        if (node.gridX <= grid.GetLength(0) && node.gridY <= grid.GetLength(1)) {
            for (int x = -1; x <= 1; x++) {
                for (int y = -1; y <= 1; y++) {
                    if (x == 0 && y == 0) {
                        continue;
                    }
                    int checkX = node.gridX + x;
                    int checkY = node.gridY + y;
                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
                        neighboursList.Add(grid[checkX, checkY]);
                    }
                }
            }
        }
        return neighboursList;
    }
}
