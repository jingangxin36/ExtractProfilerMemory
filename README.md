# ExtractProfilerMemory 

an editor tool to save unity profiler memory

编辑器工具, 用于提取UnityProfiler提取内存数据

### 功能

- 提取`Unity Profiler`窗口的内存`Detailed`数据
- 支持Editor和通过IP连接手机来监测数据
- 可以提取指定大小范围内的数据, 例如只输出内存大于1MB的数据
- 可以指定输出的层级( 深度 ), 例如只输出2级数据
- 输出的数据按照其占内存大小来排序

### 如何使用

1. 打开窗口`Window/Extract Profiler Memory`
![](https://github.com/jingangxin36/ExtractProfilerMemory/Project/Images/1.png)
2. 选择连接的设备, Editor或手机
![](https://github.com/jingangxin36/ExtractProfilerMemory/Project/Images/2.png)
3. 将Profiler窗口切换到Memory视图, 并选择Detailed选项(方便数据显示)
![](https://github.com/jingangxin36/ExtractProfilerMemory/Project/Images/3.png)
4. 点击`Take Sample`, 获取内存快照
5. 选择合适的内存大小过滤和深度过滤
6. 点击`Extract Memory`,将数据输出到txt文件

### 效果截图

1. Android环境
![](https://github.com/jingangxin36/ExtractProfilerMemory/Project/Images/2.1.png)
2. Editor环境
![](https://github.com/jingangxin36/ExtractProfilerMemory/Project/Images/2.2.png)

### 使用环境

Unity 5.6及以上

### 参考

- [Unity Editor 提取Profiler数据 - Memory](https://www.jianshu.com/p/5674e96f2b8e)


