# ExtractProfilerMemory 

an editor tool to save unity profiler memory

编辑器工具, 用于提取UnityProfiler内存数据

### 功能

- 提取`Unity Profiler`窗口的内存`Detailed`数据
- 支持Editor和通过IP连接手机来监测数据
- 可以提取指定大小范围内的数据, 例如只输出内存大于1MB的数据
- 可以指定输出的层级( 深度 ), 例如只输出2级数据
- 输出的数据按照其占内存大小来排序

### 如何使用

1. 打开窗口`Window/Extract Profiler Memory`
![](https://github.com/jingangxin36/ExtractProfilerMemory/blob/master/Project/Images/1.png)
2. 选择连接的设备, Editor或手机
![](https://github.com/jingangxin36/ExtractProfilerMemory/blob/master/Project/Images/2.png)
3. 将Profiler窗口切换到Memory视图, 并选择Detailed选项(方便数据显示)
![](https://github.com/jingangxin36/ExtractProfilerMemory/blob/master/Project/Images/3.png)
4. 点击`Take Sample`, 获取内存快照
5. 选择合适的内存大小过滤和深度过滤
6. 点击`Extract Memory`,将数据输出到txt文件

### 效果截图

1. Android环境
![](https://github.com/jingangxin36/ExtractProfilerMemory/blob/master/Project/Images/2.1.png)
2. Editor环境
![](https://github.com/jingangxin36/ExtractProfilerMemory/blob/master/Project/Images/2.2.png)

### 使用环境

Unity 5.6及以上

### 实现方法介绍

#### 数据来自哪里

利用ReSharper插件的反编译功能, 可以得到UnityEditor.ProfilerWindow的CS部分的代码, 如下图

![](https://github.com/jingangxin36/ExtractProfilerMemory/blob/master/Project/Images/Reflection.png)

我们要提取的数据就是红框标记字段, 字段的说明如下

- `UnityEditor.ProfilerWindow`: 对应Editor下的Profiler窗口
- `ProfilerWindow.m_ProfilerWindows`:Profiler窗口下各种类型窗口的集合
- `ProfilerWindow.m_CurrentArea`: 当前窗口的类型, 我们要提取的数据来自`ProfilerArea.Memory`
- `ProfilerWindow.m_MemoryListView`: 内存窗口下的内存数据记录类
- `MemoryTreeList.m_Root`: 记录了内存对象列表的根节点, 即所有内存对象占的总内存
- `MemoryElement.children`: 当前内存对象类型包含的子内存对象
- `MemoryElement.totalMemory`: 当前内存对象所占的总内存
- `MemoryElement.name`: 当前内存对象的名字, 例如: `Other`, `Assets`, `ShaderLab`等

#### 通过反射提取数据

**什么是反射**

在.NET中的反射可以实现从对象的外部来了解对象（或程序集）内部结构的功能

**作用:**

- 可以动态创建出对象并执行它其中的方法。
- 可以在运行时获得程序或程序集中每一个类型（包括类、结构、委托、接口和枚举等）的成员和成员的信息。
- 有了反射，可对每一个类型了如指掌。
- 还可以直接创建对象，即使这个对象的类型在编译时还不知道。


此工具利用反射原理, 通过递归遍历`MemoryElement.children`来提取我们所需的数据结构`totalMemory`,`name`, `totalMemory`存入自定义类`MemoryElement`, 同时增加字段`_depth`来记录该内存对象的深度, 方便数据过滤

关键代码如下

```c#
    public static MemoryElement Create(Dynamic srcMemoryElement, int depth, int filterDepth, float filterSize)
    {
        if (srcMemoryElement == null) return null;
        var dstMemoryElement = new MemoryElement { _depth = depth };
        Dynamic.CopyFrom(dstMemoryElement, srcMemoryElement.InnerObject,
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetField);

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
```

### 项目地址

- [jingangxin36/ExtractProfilerMemory](https://github.com/jingangxin36/ExtractProfilerMemory)

### 参考

- [Unity Editor 提取Profiler数据 - Memory](https://www.jianshu.com/p/5674e96f2b8e)


