using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IHeapItem<T> : IComparable<T>
{
    public int HeapIndex { get; set; }
}

public class Heap<T> where T : IHeapItem<T>
{
    private readonly T[] items;
    private uint count;
    public uint Count => count;

    public Heap(uint capacity)
    {
        items = new T[capacity];
        count = 0;
    }

    private void SortUp(T item)
    {
        while(true)
        {
            int parentIndex = (item.HeapIndex - 1) / 2;
            T parentItem = items[parentIndex];
            if (item.CompareTo(parentItem) > 0) // higher priority
                Swap(item, parentItem);
            else
                break;
        }
    }

    private void SortDown(T item)
    {
        while(true)
        {
            int childIndexLeft = item.HeapIndex * 2 + 1;
            if (childIndexLeft < count)
            {
                int childIndexRight = item.HeapIndex * 2 + 2;
                int swapIndex = childIndexLeft;

                if (childIndexRight < count)
                {
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                        swapIndex = childIndexRight;
                }

                if (item.CompareTo(items[swapIndex]) < 0)
                    Swap(item, items[swapIndex]);
                else
                    return;
            }

            return;
        }
    }

    private void Swap(T left, T right)
    {
        items[left.HeapIndex] = right;
        items[right.HeapIndex] = left;

        var tempIndex = right.HeapIndex;
        right.HeapIndex = left.HeapIndex;
        left.HeapIndex = tempIndex;
    }

    public void Add(T item)
    {
        item.HeapIndex = (int)count;
        items[count] = item;
        SortUp(item);
        count++;
    }

    public T RemoveFirst()
    {
        T firstItem = items[0];
        count--;
        items[0] = items[count];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return firstItem;
    }

    public bool Contains(T item) => Equals(items[item.HeapIndex], item);

    public void UpdateItem(T item) => SortUp(item);
}