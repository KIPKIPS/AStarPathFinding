using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//网格节点类
public class Node
{
    public bool walkable;//该网格节点是否可以行走
    public Vector3 worldPosition;//节点的世界坐标

    //构造函数
    public Node(bool _walkable, Vector3 _worldPosition)
    {
        walkable = _walkable;
        worldPosition = _worldPosition;
    }

}
