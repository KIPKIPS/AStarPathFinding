using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//使用泛型类,不光可以处理节点的排序,还可以处理其他类型的数据
//小根堆
public class Heap<T> where T : IHeapItem<T> {
    private T[] items;
    private int curItemCount;

    public Heap(int maxHeapSize) {
        items = new T[maxHeapSize];//创建一个数组用来存储堆
    }

    public int Count {
        get {
            return curItemCount;
        }
    }

    public void UpdateItem(T item) {
        SortUp(item);
    }

    public void Add(T item) {
        item.HeapIndex = curItemCount;
        items[curItemCount] = item;
        SortUp(item);
        curItemCount++;
    }

    public void SortUp(T item) {//自底向上
        int parentIndex = (item.HeapIndex - 1) / 2;
        while (true) {
            T parentItem = items[parentIndex];
            if (item.Compare(item, parentItem) > 0) {
                Swap(item, parentItem);
            } else {
                break;
            }

            parentIndex = (item.HeapIndex - 1) / 2;//父节点索引
        }
    }
    public T RemoveFirst() { //移除堆顶元素,并返回
        T firstItem = items[0];
        curItemCount--;
        items[0] = items[curItemCount];//堆顶元素和堆底元素交换
        items[0].HeapIndex = 0;//堆顶元素索引置为0,更新索引
        SortDown(items[0]);
        return firstItem;
    }

    public void SortDown(T item) {//自顶向下调整
        while (true) {
            int childLeftIndex = item.HeapIndex * 2 + 1;
            int childRightIndex = item.HeapIndex * 2 + 2;
            //寻找最小值的节点索引
            int swapIndex = 0;
            if (childLeftIndex < curItemCount) {
                swapIndex = childLeftIndex;//初始化
                if (childRightIndex < curItemCount) {
                    //Compare接口,比较节点Left和节点Right,负值说明R的代价小于L
                    if (items[childLeftIndex].Compare(items[childLeftIndex], items[childRightIndex]) < 0) {
                        swapIndex = childRightIndex;
                    }
                }

                if (item.Compare(item, items[swapIndex]) < 0) {
                    Swap(item, items[swapIndex]);
                } else {
                    return;
                }
            } else {
                return;
            }

        }
    }

    public bool Contains(T item) {
        return Equals(items[item.HeapIndex], item);
    }

    void Swap(T itemA, T itemB) {
        //引用类型交换赋值
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;

        //值类型的借助临时变量交换
        int itemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;
    }
}

public interface IHeapItem<T> : IComparer<T> {
    int HeapIndex { get; set; }
}
