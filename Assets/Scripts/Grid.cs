using System;
using System.Drawing;
using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public Vector2 gridWorldSize;//节点坐标
    public float nodeRadius;//节点半径
    Node[,] grid;
    public LayerMask unwalkableMask;
    int gridSizeX, gridSizeY;//节点的长宽数量
    float nodeDiameter;//节点直径
    GameObject wallObj;
    Vector3 worldBttomLeft;//左下角坐标
    public Transform player;
    void OnDrawGizmos()
    { //用来显示一个三维向量的包围框
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        if (grid != null)
        {
            Node playerNode = NodeFromWorldPoint(player.transform.position);
            foreach (Node n in grid)
            {
                Gizmos.color = playerNode == n ? Color.cyan : (n.walkable ? Color.white : Color.red); //玩家则置为青色,否则根据是否障碍物进行判断,障碍物红色,通路白色
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));//着色
            }
        }
    }
    void Start()
    {
        player.gameObject.SetActive(true);
        wallObj = GameObject.Find("Walls");
        wallObj.SetActive(true);
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);//四舍五入到整数
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreatGrid();
    }
    void CreatGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];//创建一个Node节点二维数组
        worldBttomLeft = transform.position + Vector3.left * gridWorldSize.x / 2 + Vector3.back * gridWorldSize.y / 2;//网格的左下角坐标
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBttomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);//计算出每一个节点的坐标
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));//定义的球体是否和物体相撞,处于layerMask的物体
                grid[x, y] = new Node(walkable, worldPoint);
            }
        }
        wallObj.SetActive(false);
        player.gameObject.SetActive(false);
        //NodeFromWorldPoint(new Vector3(0, 0, 0));
    }

    //根据节点中心坐标返回对应的Node
    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        //计算索引
        int xIndex = Mathf.RoundToInt(worldPosition.x - worldBttomLeft.x - 0.5f);
        int yIndex = Mathf.RoundToInt(worldPosition.z - worldBttomLeft.z - 0.5f);
        //print((worldPosition.x - worldBttomLeft.x).ToString() + "  " + (worldPosition.z - worldBttomLeft.z).ToString());
        print("x : " + xIndex + " , y : " + yIndex + "");
        return grid[xIndex, yIndex];
    }
}
