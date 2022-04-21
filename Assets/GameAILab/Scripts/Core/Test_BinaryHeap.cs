using System;
using System.Collections.Generic;
using UnityEngine;
using GameAILab.Core;

[Serializable]
public class Test1
{
    public float total;
    public float h;

    public override string ToString()
    {
        return $"total: {total}, h: {h}";
    }
}

public class Test_BinaryHeap : MonoBehaviour
{
    public Test1[] arr;
    public Test1 toadd;
    //public bool minHeap;

    private BinaryHeap<Test1> pq;

    private void Start()
    {
        Do();
        Debug.Log(pq.DebugString());
    }

    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.B))
        {
            Do();
            Debug.Log(pq.DebugString());
        }*/
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (toadd is null)
                throw new NullReferenceException();
            pq.Add(toadd);
            Debug.Log(pq.DebugString());
        }
    }

    private void Do()
    {
        Comparison<Test1> comparison = (Test1 i1, Test1 i2) =>
        {
            if (i2.total != i1.total)
                return Mathf.CeilToInt(i2.total - i1.total);
            else
                return Mathf.CeilToInt(i2.h - i1.h);
        };

        //Debug.Log($"to create binary heap: {arr.Length}");
        pq = new BinaryHeap<Test1>(arr.Length, comparison);

        foreach (var ele in arr)
        {
            //Debug.Log($"to add ele: {ele.ToString()}");

            pq.Add(ele);
        }

        /*string result = "";
        while (pq.Count > 0)
        {
            var ele = pq.RemoveTop();
            Debug.Log($"removed ele: {ele.ToString()}");

            result += ele.ToString() + " | ";
        }
        Debug.Log(result);*/
    }


}
