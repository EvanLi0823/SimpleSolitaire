# 阶段6：性能优化 - 详细指南

## 负责代理
`unity-performance` - 参考 `.claude/agents/unity-performance.md`

## 触发条件
⚠️ **条件触发**：仅当阶段5的性能测试**未达标**时才进入此阶段。

如果性能测试全部达标，直接跳过此阶段进入阶段7（交付）。

## 目标
通过系统化的性能分析和优化，使功能性能达到设计目标。

## 执行步骤

### 1. 性能问题分析（10-15分钟）

#### 1.1 收集性能数据
从阶段5的性能测试报告中获取：
- 当前帧率
- 内存使用情况
- GC分配情况
- CPU/GPU使用情况
- 具体的性能瓶颈点

#### 1.2 使用Unity Profiler深度分析
```
Window > Analysis > Profiler
- CPU Usage: 识别耗时函数
- Rendering: 检查Draw Call和渲染开销
- Memory: 检查内存分配和GC
- Physics: 检查物理计算开销（如适用）
```

#### 1.3 识别性能瓶颈
优先级排序：
1. **高优先级**：严重影响帧率的问题
2. **中优先级**：有明显影响但不致命的问题
3. **低优先级**：轻微影响或仅在极端情况下出现的问题

#### 1.4 根本原因分析
对每个性能问题进行根本原因分析：
- 为什么会慢？
- 瓶颈在哪里？
- 是设计问题还是实现问题？

### 2. 制定优化方案（10-15分钟）

#### 2.1 CPU优化方案

**减少GC分配**：
```csharp
// 问题：每帧分配
void Update()
{
    var list = new List<int>();  // 每帧分配
}

// 优化：重用对象
private List<int> _reusableList = new List<int>();
void Update()
{
    _reusableList.Clear();
}
```

**对象池**：
```csharp
// 实施对象池以避免频繁创建销毁
public class ObjectPool<T> where T : Component
{
    private Queue<T> _pool = new Queue<T>();
    private T _prefab;

    public T Get()
    {
        if (_pool.Count > 0)
        {
            var obj = _pool.Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        }
        return Object.Instantiate(_prefab);
    }

    public void Return(T obj)
    {
        obj.gameObject.SetActive(false);
        _pool.Enqueue(obj);
    }
}
```

**缓存组件引用**：
```csharp
// 问题：每次都获取
void Update()
{
    GetComponent<Rigidbody>().velocity = Vector3.zero;
}

// 优化：缓存引用
private Rigidbody _rb;
void Awake()
{
    _rb = GetComponent<Rigidbody>();
}
void Update()
{
    _rb.velocity = Vector3.zero;
}
```

**优化Update调用**：
```csharp
// 使用UpdateManager统一管理Update
public class UpdateManager : MonoBehaviour
{
    private List<IUpdatable> _updatables = new List<IUpdatable>();

    public void Register(IUpdatable updatable)
    {
        _updatables.Add(updatable);
    }

    void Update()
    {
        for (int i = 0; i < _updatables.Count; i++)
        {
            _updatables[i].OnUpdate(Time.deltaTime);
        }
    }
}
```

#### 2.2 GPU/渲染优化方案

**减少Draw Call**：
- 启用Static Batching
- 启用Dynamic Batching
- 使用GPU Instancing
- 合并材质

**UI优化**：
```csharp
// 分离动态和静态UI到不同Canvas
// 使用Canvas Group控制可见性
public class UIOptimizer : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;

    public void Hide()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }
}
```

#### 2.3 内存优化方案

**纹理压缩**：
- 使用合适的纹理格式（ASTC/ETC2/PVRTC）
- 设置合理的Max Size
- 启用Mipmap（3D场景）

**资源卸载**：
```csharp
// 及时卸载不再使用的资源
Resources.UnloadAsset(asset);
Resources.UnloadUnusedAssets();
System.GC.Collect();
```

### 3. 实施优化（主要阶段）

#### 3.1 按优先级实施
从高优先级问题开始，逐个解决。

#### 3.2 增量优化
- 一次优化一个问题
- 每次优化后立即测试
- 验证优化效果
- 记录优化前后对比

#### 3.3 避免过度优化
- 只优化真正的瓶颈
- 平衡性能和代码可读性
- 不要为了优化而优化

### 4. 验证优化效果（每次优化后）

#### 4.1 性能测试
使用与阶段5相同的测试方法重新测试：
- 帧率是否提升？
- 内存占用是否降低？
- GC分配是否减少？

#### 4.2 Profiler对比
对比优化前后的Profiler数据：
- CPU时间是否减少？
- 渲染开销是否降低？
- 内存峰值是否下降？

#### 4.3 功能验证
确保优化没有破坏功能：
- 功能是否正常？
- 是否引入新的Bug？
- 边界情况是否处理正确？

### 5. 迭代优化

如果性能仍未达标，返回步骤1重新分析，继续优化。

**迭代流程**：
```
分析 → 制定方案 → 实施 → 验证 → 达标？
                                ↓ 否
                                返回分析
```

**完成条件**：
- ✅ 所有性能指标达到目标
- ✅ 无明显性能问题
- ✅ 功能正常无Bug
- ✅ 代码质量可接受

## 性能优化清单

### CPU优化
- [ ] 减少GC分配（目标：<1KB/frame）
- [ ] 实施对象池（如需要）
- [ ] 缓存组件引用
- [ ] 优化Update调用
- [ ] 使用高效的数据结构
- [ ] 优化算法复杂度

### GPU/渲染优化
- [ ] 减少Draw Call
- [ ] 启用批处理
- [ ] 优化Shader
- [ ] UI分层优化
- [ ] LOD系统（如适用）

### 内存优化
- [ ] 压缩纹理
- [ ] 优化Mesh
- [ ] 及时卸载资源
- [ ] 避免内存泄漏
- [ ] 控制内存峰值

## 优化报告模板

```markdown
# [功能名称] 性能优化报告

## 一、优化前性能
- 平均帧率：XX FPS
- 最低帧率：XX FPS
- 内存占用：XX MB
- GC分配：XX KB/frame
- Draw Call：XX（如适用）
- 主要问题：[列出]

## 二、性能瓶颈分析
### 瓶颈1：[名称]
- 位置：[代码位置]
- 原因：[根本原因]
- 影响：[性能影响]

### 瓶颈2：[名称]
...

## 三、优化措施

### 优化1：[名称]
**问题**：[描述问题]

**优化前代码**：
\`\`\`csharp
// 优化前
\`\`\`

**优化后代码**：
\`\`\`csharp
// 优化后
\`\`\`

**效果**：
- 帧率提升：XX FPS
- 内存减少：XX MB
- GC减少：XX KB/frame

### 优化2：[名称]
...

## 四、优化后性能
- 平均帧率：XX FPS (+XX%)
- 最低帧率：XX FPS (+XX%)
- 内存占用：XX MB (-XX%)
- GC分配：XX KB/frame (-XX%)
- Draw Call：XX (-XX%)（如适用）

## 五、性能对比图表
[使用Profiler截图或数据图表]

## 六、剩余问题
[如有未解决的性能问题，列出并说明原因]

## 七、后续优化建议
[可选的进一步优化方向]
```

## 常见优化模式

### 模式1：对象池
适用：频繁创建销毁的对象（子弹、粒子、敌人等）

### 模式2：缓存
适用：重复计算的结果、频繁获取的组件引用

### 模式3：批处理
适用：大量相似操作、渲染优化

### 模式4：异步加载
适用：资源加载、场景加载、网络请求

### 模式5：LOD
适用：3D场景、大量模型、远近距离差异大

## 优化技巧

### 技巧1：先测量再优化
不要盲目优化，使用Profiler找到真正的瓶颈。

### 技巧2：增量优化
一次优化一个问题，便于验证效果和定位问题。

### 技巧3：性能预算
为每个系统设定性能预算，超出预算才优化。

### 技巧4：平衡优化
在性能、代码质量、开发时间之间找到平衡。

## 注意事项

### 避免的陷阱
- ❌ 过早优化
- ❌ 过度优化
- ❌ 牺牲可读性
- ❌ 引入Bug
- ❌ 盲目优化

### 优化原则
- ✅ 先分析再优化
- ✅ 测量优化效果
- ✅ 保持代码可读
- ✅ 渐进式优化
- ✅ 文档化优化

## 完成标准

### 性能达标
- [ ] 帧率达到目标（≥60 FPS或30 FPS）
- [ ] 内存占用合理
- [ ] 无内存泄漏
- [ ] GC分配控制在目标内（<1KB/frame）
- [ ] 无明显卡顿

### 质量保证
- [ ] 功能正常
- [ ] 无新Bug
- [ ] 代码可维护
- [ ] 有优化文档

### 准备进入下一阶段
- [ ] 性能测试通过
- [ ] 优化报告完成
- [ ] 代码审查通过（如有修改）
- [ ] 准备交付
