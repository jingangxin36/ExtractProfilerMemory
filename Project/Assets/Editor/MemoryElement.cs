
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;


public class MemoryElement : IComparable<MemoryElement>
{
    //反射 name, totalMemory, children 需要保持命名与dll里面一致
    public string name;
    public long totalMemory;
    public List<MemoryElement> children = new List<MemoryElement>();


    private int _depth;

    private MemoryElement()
    {
    }

    public static MemoryElement Create(Dynamic srcMemoryElement, int depth, int filterDepth, float filterSize)
    {
        if (srcMemoryElement == null) return null;
        var dstMemoryElement = new MemoryElement { _depth = depth };
        Dynamic.CopyFrom(dstMemoryElement, srcMemoryElement.InnerObject,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetField);
        if (dstMemoryElement.name == null)
        {
            dstMemoryElement.name = "";
        }

        var srcChildren = srcMemoryElement.PublicInstanceField<IList>("children");
        if (srcChildren == null) return dstMemoryElement;
        foreach (var srcChild in srcChildren)
        {
            var memoryElement = Create(new Dynamic(srcChild), depth + 1, filterDepth, filterSize);
            if (memoryElement == null) continue;
            if (depth > filterDepth) continue;
            if (!(memoryElement.totalMemory >= filterSize)) continue;
            dstMemoryElement.children.Add(memoryElement);
        }

        dstMemoryElement.children.Sort();
        return dstMemoryElement;
    }


    public override string ToString()
    {
        var displayName = string.IsNullOrEmpty(name) ? "-" : name;
        string numText = null;
        if (totalMemory < 1024)
        {
            numText = totalMemory.ToString() + "B";
        }
        else if(totalMemory < 1024 * 1024)
        {
            numText = (totalMemory / 1024f).ToString("F2") + "KB";
        }
        else
        {
            numText = (totalMemory / (1024 * 1024f)).ToString("F2") + "MB";
        }
        return $"{new string('\t', _depth)}{displayName}, {numText}";
    }

    public int CompareTo(MemoryElement other)
    {
        if (other.totalMemory != totalMemory)
        {
            return other.totalMemory.CompareTo(totalMemory);
        }

        return other.name.CompareTo(name);
    }
}