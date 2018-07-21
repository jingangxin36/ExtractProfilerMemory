
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


public class MemoryElement : IComparable<MemoryElement>
{
    //拷贝name,totalMemory, children 需要保持命名与dll里面一致
    public string name;
    public long totalMemory;
    public List<MemoryElement> children = new List<MemoryElement>();


    private int _depth;

    private MemoryElement()
    {
    }

    public static MemoryElement Create(Dynamic memoryElement, int dep, int filterDepth, float filterSize)
    {
        if (null == memoryElement) return null;
        var memoryElement1 = new MemoryElement {_depth = dep};
        var memoryElement2 = memoryElement1;
        Dynamic.ShallowCopyFrom(memoryElement2, memoryElement.InnerObject,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetField);

        var list = memoryElement.PublicInstanceField<IList>("children");
        if (null == list) return memoryElement2;
        foreach (var item2 in list)
        {
            var memoryElement3 = Create(new Dynamic(item2), dep + 1, filterDepth, filterSize);
            if (null == memoryElement3) continue;
            if (dep > filterDepth) continue;
            if (memoryElement3.totalMemory >= filterSize)
            {
                memoryElement2.children.Add(memoryElement3);
            }
        }

        memoryElement2.children.Sort();
        return memoryElement2;
    }


    public override string ToString()
    {
        var text = string.IsNullOrEmpty(name) ? "-" : name;
        var text2 = "KB";
        var num = totalMemory / 1024f;
        if (num > 512f)
        {
            num /= 1024f;
            text2 = "MB";
        }

        var resultString = string.Format(new string('\t', _depth) + " {0}, {1}{2}", text, num, text2);
        return resultString;
    }

    public int CompareTo(MemoryElement other)
    {
        if (other.totalMemory != totalMemory)
        {
            return (int) (other.totalMemory - totalMemory);
        }

        if (string.IsNullOrEmpty(name)) return -1;
        return !string.IsNullOrEmpty(other.name) ? string.Compare(name, other.name, StringComparison.Ordinal) : 1;

    }
}