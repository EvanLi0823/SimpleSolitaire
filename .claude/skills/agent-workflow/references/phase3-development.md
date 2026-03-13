# 阶段3：开发实施 - 详细指南

## 负责代理
`unity-developer` - 参考 `.claude/agents/unity-developer.md`

## 前置条件
⚠️ **必须**：已获得用户对设计方案的明确确认

## 目标
根据设计文档，编写高质量、可维护的代码实现功能。

## 执行步骤

### 1. 准备工作（5分钟）

#### 1.1 环境检查
```bash
# 检查Unity编译状态
mcp__UnityMCP__read_console(types: ["error", "warning"])

# 如有错误，先修复现有问题
```

#### 1.2 代码结构准备
- 创建必要的文件夹
- 设置Assembly Definition（如需要）
- 准备ScriptableObject配置

### 2. 按模块实施（主要阶段）

#### 2.1 实施顺序原则
- 先实现核心模块（底层依赖）
- 再实现服务层
- 最后实现表现层
- 采用增量式开发

#### 2.2 编码规范（必须遵守）

**命名规范**：
```csharp
// ✅ 正确
public class PlayerController { }           // PascalCase - 类
private readonly IService _service;          // _camelCase - 私有字段
public int Health { get; set; }              // PascalCase - 属性
public void Attack() { }                     // PascalCase - 方法
private void UpdateState() { }               // PascalCase - 私有方法
```

**注释规范（当前项目要求中文）**：
```csharp
/// <summary>
/// 玩家控制器，负责处理玩家输入和状态管理
/// </summary>
public class PlayerController
{
    /// <summary>
    /// 执行攻击动作
    /// </summary>
    /// <param name="target">攻击目标</param>
    public void Attack(Enemy target)
    {
        // 检查是否可以攻击
        if (!CanAttack()) return;

        // 执行攻击逻辑
        DealDamage(target);
    }
}
```

**架构规范**：
```csharp
// ✅ 分离关注点
// View层（MonoBehaviour）
public class GameView : MonoBehaviour
{
    private IGameController _controller;

    [Inject]
    public void Construct(IGameController controller)
    {
        _controller = controller;
    }
}

// Controller层（纯C#）
public class GameController : IGameController
{
    private readonly IGameModel _model;

    public GameController(IGameModel model)
    {
        _model = model;
    }
}
```

### 3. 持续验证（每个模块完成后）

#### 3.1 编译检查
```bash
# 刷新Unity并请求编译
mcp__UnityMCP__refresh_unity(compile: "request", mode: "force")

# 检查错误
mcp__UnityMCP__read_console(types: ["error", "warning"])

# 如有错误，立即修复
```

#### 3.2 代码自查
- [ ] 是否遵循命名规范？
- [ ] 是否添加必要注释？
- [ ] 是否有空引用检查？
- [ ] 是否有异常处理？
- [ ] 是否符合架构设计？

### 4. 依赖注入配置

#### 4.1 Zenject配置示例
```csharp
public class FeatureInstaller : MonoInstaller
{
    [SerializeField] private FeatureConfig _config;

    public override void InstallBindings()
    {
        // 配置绑定
        Container.Bind<FeatureConfig>()
            .FromInstance(_config)
            .AsSingle();

        // 服务绑定
        Container.BindInterfacesAndSelfTo<FeatureService>()
            .AsSingle()
            .NonLazy();

        // 模型绑定
        Container.Bind<FeatureModel>()
            .AsSingle();
    }
}
```

### 5. 集成到现有系统

#### 5.1 集成检查清单
- [ ] 新模块是否正确注册到DI容器？
- [ ] 事件订阅是否正确设置？
- [ ] 资源引用是否正确配置？
- [ ] 场景引用是否正确设置？
- [ ] 是否影响现有功能？

### 6. 开发阶段检查清单

#### 代码质量检查
- [ ] 职责单一：每个类只做一件事
- [ ] 方法简短：方法不超过50行
- [ ] 避免重复：复用相同逻辑
- [ ] 低耦合：模块间依赖清晰

#### Unity特定检查
- [ ] MonoBehaviour只处理Unity生命周期
- [ ] 协程正确管理（启动和停止）
- [ ] 事件正确订阅和取消订阅
- [ ] 资源正确加载和释放

#### 性能检查
- [ ] 避免在Update中频繁操作
- [ ] 缓存组件引用
- [ ] 减少GC分配
- [ ] 使用对象池（如适用）

#### 安全性检查
- [ ] 空引用检查
- [ ] 数组边界检查
- [ ] 异常处理
- [ ] 资源释放

## 常见实施模式

### 模式1：接口-实现-注册
```csharp
// 1. 定义接口
public interface IFeatureService
{
    void DoSomething();
}

// 2. 实现接口
public class FeatureService : IFeatureService
{
    public void DoSomething()
    {
        // 实现
    }
}

// 3. 注册到DI
Container.BindInterfacesAndSelfTo<FeatureService>().AsSingle();
```

### 模式2：Model-View-Presenter
```csharp
// Model（数据）
public class FeatureModel
{
    public ReactiveProperty<int> Value { get; } = new ReactiveProperty<int>();
}

// Presenter（逻辑）
public class FeaturePresenter : MonoBehaviour
{
    private FeatureModel _model;

    [Inject]
    public void Construct(FeatureModel model)
    {
        _model = model;
        _model.Value.Subscribe(OnValueChanged).AddTo(this);
    }
}
```

### 模式3：命令模式
```csharp
// 命令接口
public interface ICommand
{
    void Execute();
    void Undo();
}

// 具体命令
public class FeatureCommand : ICommand
{
    public void Execute() { }
    public void Undo() { }
}

// 注册命令工厂
Container.BindFactory<FeatureCommand, FeatureCommand.Factory>()
    .FromPoolableMemoryPool(x => x.WithInitialSize(10));
```

## 开发技巧

### 技巧1：增量开发
- 先实现最小可用功能
- 逐步添加功能
- 每次添加都测试

### 技巧2：使用TODO标记
```csharp
// TODO: 添加输入验证
// TODO: 优化性能
// FIXME: 修复边界情况
```

### 技巧3：日志调试
```csharp
#if UNITY_EDITOR
    Debug.Log($"[FeatureService] 执行操作: {operationName}");
#endif
```

## 完成标准

### 代码完成检查
- [ ] 所有设计的模块都已实现
- [ ] 代码无编译错误和警告
- [ ] 遵循项目规范
- [ ] 添加必要注释
- [ ] 通过初步测试

### 准备进入下一阶段
- [ ] 清理调试代码
- [ ] 整理代码格式
- [ ] 更新TODO标记
- [ ] 准备接受代码审查
