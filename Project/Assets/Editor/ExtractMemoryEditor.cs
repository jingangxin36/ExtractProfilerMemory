using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class ExtractMemoryEditor: EditorWindow
{

    private float _memorySize = 1f;
    private int _memoryDepth = 1;

    public static ExtractMemoryEditor Window;

    [UnityEditor.MenuItem("Window/Extract Profiler Memory")]
    public static void ShowWindow()
    {
        EditorApplication.ExecuteMenuItem("Window/Profiler");
        if (Window == null)
        {
            Window = CreateInstance<ExtractMemoryEditor>();
        }
        Window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();

        EditorGUILayout.LabelField("Current Target: " + ProfilerDriver.GetConnectionIdentifier(ProfilerDriver.connectedProfiler));

        if (GUILayout.Button("Take Sample"))
        {
            TakeSample();
        }

        _memorySize = EditorGUILayout.FloatField("Memory Size(MB) >= ", _memorySize);
        _memoryDepth = EditorGUILayout.IntField("Memory Depth(>=1)", _memoryDepth);

        if (GUILayout.Button("Extract Memory"))
        {
            if (_memoryDepth <= 0 )
            {
                _memoryDepth = 1;
            }
            ExtractMemory(_memorySize, _memoryDepth - 1);
        }

        EditorGUILayout.BeginVertical();
    }
    private MemoryElement _memoryElementRoot;
    private void ExtractMemory(float memSize, int memDepth)
    {
        float filterSize = memSize * 1024 * 1024;
        DirectoryInfo topDir = Directory.GetParent(Application.dataPath);
        var outputPath = string.Format("{0}/MemoryDetailed{1:yyyy_MM_dd_HH_mm_ss}.txt", topDir.FullName, DateTime.Now);
        File.Create(outputPath).Dispose();
        _memoryElementRoot = ProfilerWindow.GetMemoryDetailRoot(memDepth, filterSize);

        if (null != _memoryElementRoot)
        {
            StreamWriter writer = new StreamWriter(outputPath);
            writer.WriteLine("Memory Size: >= {0}MB", _memorySize);
            writer.WriteLine("Memory Depth: {0}", _memoryDepth);
            writer.WriteLine("Current Target: {0}", ProfilerDriver.GetConnectionIdentifier(ProfilerDriver.connectedProfiler));
            writer.WriteLine("**********************");
            ProfilerWindow.WriteMemoryDetail(writer, _memoryElementRoot);
            writer.Flush();
            writer.Close();
        }
        
        Process.Start(outputPath);
    }

    private void TakeSample()
    {
        ProfilerWindow.RefreshMemoryData();
    }
}
