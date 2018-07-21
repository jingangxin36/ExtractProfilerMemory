
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public static class ProfilerWindow
{
    private static List<Dynamic> _wnds = null;

    private static Dynamic _GetWnd(ProfilerArea area)
    {
        if (null == _wnds)
        {
            DynamicType dynamicType = new DynamicType(typeof(EditorWindow));
            Dynamic type = dynamicType.GetType("UnityEditor.ProfilerWindow");
            IList list = type.PrivateStaticField<IList>("m_ProfilerWindows");
            _wnds = new List<Dynamic>();
            for (int i = 0; i < list.Count; i++)
            {
                _wnds.Add(new Dynamic(list[i]));
            }
        }
        for (int i = 0; i < _wnds.Count; i++)
        {
            Dynamic dynamic = _wnds[i];
            ProfilerArea val = (ProfilerArea)dynamic.PrivateInstanceField("m_CurrentArea");
            if (val == area)
            {
                return dynamic;
            }
        }
        return null;
    }

    public static MemoryElement GetMemoryDetailRoot(int filterDepth, float filterSize)
    {
        Dynamic dynamic = _GetWnd(ProfilerArea.Memory);
        if (null != dynamic)
        {
            Dynamic dynamic2 = new Dynamic(dynamic.PrivateInstanceField("m_MemoryListView"));
            object obj = dynamic2.PrivateInstanceField("m_Root");
            if (null != obj)
            {
                return MemoryElement.Create(new Dynamic(obj), 0, filterDepth, filterSize);
            }
            return null;
        }
        return null;
    }

    public static void WriteMemoryDetail(StreamWriter writer, MemoryElement root)
    {
        if (null != root)
        {
            writer.WriteLine(root.ToString());
            for (int i = 0; i < root.children.Count; i++)
            {
                var memoryElement = root.children[i];
                if (null != memoryElement)
                {
                    WriteMemoryDetail(writer, memoryElement);
                }
            }
        }
    }

    public static void RefreshMemoryData()
    {
        
        Dynamic dynamic = _GetWnd(ProfilerArea.Memory);
        if (null != dynamic)
        {
            dynamic.CallPrivateInstanceMethod("RefreshMemoryData");
        }
        else
        {
            Debug.Log("请打开Profiler 窗口的 Memory 视图");
        }
    }
}

public class DynamicType
{
    private readonly Assembly _assembly;

    public DynamicType(Type type)
    {
        _assembly = type.Assembly;
    }

    public Dynamic GetType(string typeName)
    {
        return new Dynamic(_assembly.GetType(typeName));
    }
}
