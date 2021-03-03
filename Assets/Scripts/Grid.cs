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
    void OnDrawGizmos()
    { //用来显示一个三维向量的包围框
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        if (grid != null)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = n.walkable ? Color.white : Color.red;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
            }
        }
    }
    void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);//四舍五入到整数
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreatGrid();
    }
    void CreatGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];//创建一个Node节点二维数组
        Vector3 worldBttomLeft = transform.position + Vector3.left * gridWorldSize.x / 2 + Vector3.back * gridWorldSize.y / 2;//网格的左下角坐标
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBttomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);//计算出每一个节点的坐标
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));//定义的球体是否和物体相撞,处于layerMask的物体
                grid[x, y] = new Node(walkable, worldPoint);
            }
        }
    }
}
