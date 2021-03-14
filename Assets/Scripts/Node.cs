using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//网格节点类
public class Node
{
    public bool walkable;//该网格节点是否可以行走
    public Vector3 worldPosition;//节点的世界坐标

    public int gCost;//G
    public int hCost;//H

    public int gridX;//节点在网格中的X坐标
    public int gridY;

    public Node parent;//父节点
    public bool existedInOpenset = false;

    //构造函数
    public Node(bool _walkable, Vector3 _worldPosition, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPosition = _worldPosition;
        gridX = _gridX;
        gridY = _gridY;
    }
    public int fCost
    { //F属性字段
        get
        {
            return hCost + gCost;
        }
    }

}
