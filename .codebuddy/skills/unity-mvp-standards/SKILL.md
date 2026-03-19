---
name: unity-mvp-standards
description: Unity MVP架构开发标准。强制执行数据与表现解耦（MVP架构）、依赖注入（Zenject/VContainer）、响应式编程（UniRx）。触发时机：编写/审查/重构Unity C#代码、实现功能、修复Bug、架构设计。核心约束：Model不依赖Unity、Presenter处理生命周期、严格依赖注入、ReactiveProperty数据绑定。适用于所有基于MVP架构的Unity项目。
version: 1.0.0
author: Unity MVP Standards Team
---

# Unity MVP架构代码规范

通用的Unity MVP架构开发标准，**核心原则：数据与表现严格解耦**。

## 🎯 核心原则

### 金科玉律：数据与表现解耦

```
Model（数据层）  →  Presenter（逻辑层）  →  View（表现层）
   ↓                      ↓                      ↓
纯C#类              MonoBehaviour          Unity组件
不依赖Unity          处理生命周期            UI/渲染
可单元测试          业务协调               用户交互
```

**严格禁止**：
- ❌ Model中使用MonoBehaviour
- ❌ Model中使用Unity API（Transform、GameObject等）
- ❌ Model中使用协程
- ❌ View直接访问Model
- ❌ 业务逻辑写在MonoBehaviour中

## 技能触发时机

当进行以下操作时自动触发：
- ✅ 编写/审查/重构Unity C#代码
- ✅ 实现新功能
- ✅ 修复Bug
- ✅ 架构设计
- ✅ 代码审查
- ✅ 批量修改代码

## 优先级分层

| 优先级 | 类别 | 说明 |
|--------|------|------|
| **🚫 P0** | **硬性约束** | 违反将导致编译错误或架构崩溃 |
| **🔴 P1** | **架构规范** | MVP架构、依赖注入、数据绑定 |
| **🟡 P2** | **代码质量** | 命名、注释、异常处理 |
| **🟢 P3** | **性能优化** | 内存、GC、Update优化 |
| **🔵 P4** | **最佳实践** | 设计模式、代码组织 |

## 快速参考指南

| 优先级 | 任务 | 参考文档 |
|-------|------|---------|
| **🚫 P0: 硬性约束** | | |
| P0 | Model不依赖Unity、纯C#实现 | [mvp-separation.md](references/constraints/mvp-separation.md) ⭐⭐⭐ |
| P0 | 依赖注入规范、构造函数注入 | [dependency-injection.md](references/constraints/dependency-injection.md) ⭐⭐⭐ |
| P0 | MonoBehaviour使用限制 | [monobehaviour-rules.md](references/constraints/monobehaviour-rules.md) ⭐⭐⭐ |
| **🔴 P1: 架构规范** | | |
| P1 | MVP架构实现规范 | [mvp-architecture.md](references/architecture/mvp-architecture.md) ⭐⭐ |
| P1 | Zenject依赖注入模式 | [zenject-patterns.md](references/architecture/zenject-patterns.md) ⭐⭐ |
| P1 | UniRx响应式编程规范 | [unirx-patterns.md](references/architecture/unirx-patterns.md) ⭐⭐ |
| P1 | 事件管理与订阅 | [event-management.md](references/architecture/event-management.md) ⭐ |
| P1 | 业务模块开发标准 | 命名空间规范、日志管理 ⭐⭐ |
| **🟡 P2: 代码质量** | | |
| P2 | 命名规范、注释规范 | [code-style.md](references/quality/code-style.md) ⭐ |
| P2 | 异常处理、日志规范 | [error-handling.md](references/quality/error-handling.md) |
| P2 | 空引用检查、安全调用 | [null-safety.md](references/quality/null-safety.md) ⭐ |
| **🟢 P3: 性能优化** | | |
| P3 | 避免GC分配、对象池 | [performance.md](references/performance/performance.md) |
| **🔵 P4: 设计模式** | | |
| P4 | 命令模式、工厂模式 | [design-patterns.md](references/patterns/design-patterns.md) |

## 🚫 P0 快速检查清单

### Model层约束
- [ ] **绝对禁止**继承MonoBehaviour
- [ ] **绝对禁止**使用Unity API（Transform、GameObject等）
- [ ] **绝对禁止**使用协程
- [ ] **必须**是纯C#类
- [ ] **必须**可以在非Unity环境下单元测试

### 依赖注入约束
- [ ] **必须**使用构造函数注入
- [ ] **禁止**使用`FindObjectOfType`
- [ ] **禁止**使用单例模式（除非通过Zenject）
- [ ] **必须**在Installer中注册
- [ ] **必须**使用接口依赖

### MonoBehaviour约束
- [ ] **仅用于**Unity生命周期管理
- [ ] **禁止**包含业务逻辑
- [ ] **必须**通过Inject获取依赖
- [ ] **必须**正确管理事件订阅/取消

## 🔴 P1 架构规范速查

### MVP三层职责

**Model（数据层）**：
```csharp
// ✅ 正确：纯C#，使用ReactiveProperty
public class GameState
{
    public ReactiveProperty<GameStateType> State { get; }
        = new ReactiveProperty<GameStateType>();

    public IReadOnlyReactiveProperty<int> Score => _score;
    private ReactiveProperty<int> _score = new ReactiveProperty<int>();

    public void AddScore(int points)
    {
        _score.Value += points;
    }
}

// ❌ 错误：Model依赖Unity
public class GameState : MonoBehaviour  // 错误！
{
    public Transform player;  // 错误！依赖Unity
}
```

**Presenter（逻辑层）**：
```csharp
// ✅ 正确：MonoBehaviour，通过Inject获取依赖
public class GameStatePresenter : MonoBehaviour
{
    [SerializeField] private GameObject _winPanel;

    private GameState _gameState;

    [Inject]
    public void Construct(GameState gameState)
    {
        _gameState = gameState;
    }

    private void Start()
    {
        // 订阅Model变化，更新View
        _gameState.State
            .Subscribe(OnStateChanged)
            .AddTo(this);
    }

    private void OnStateChanged(GameStateType state)
    {
        _winPanel.SetActive(state == GameStateType.Won);
    }
}
```

**View（表现层）**：
```csharp
// View层就是Unity的GameObject、Prefab、UI组件
// 通过Presenter的SerializeField引用
```

### Zenject注入模式

```csharp
// Installer中注册
public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        // Model - 单例
        Container.Bind<GameState>().AsSingle();

        // Service - 接口绑定
        Container.BindInterfacesAndSelfTo<ScoreService>().AsSingle();

        // Factory - 工厂绑定
        Container.BindFactory<Card, Card.Factory>()
            .FromComponentInNewPrefab(_cardPrefab);
    }
}
```

### UniRx数据绑定

```csharp
// Model暴露ReactiveProperty
public IReadOnlyReactiveProperty<int> Health => _health;
private ReactiveProperty<int> _health = new ReactiveProperty<int>(100);

// Presenter订阅变化
_model.Health
    .Subscribe(h => UpdateHealthBar(h))
    .AddTo(this);  // 自动管理生命周期
```

## 业务模块开发标准

### 命名空间规范（P1）

**强制规则：同一业务模块公用一个命名空间**

```csharp
// ✅ 正确：整个Shop模块使用同一个命名空间
namespace Game.Shop
{
    // Models/ShopModel.cs
    public class ShopModel { }

    // Presenters/ShopPresenter.cs
    public class ShopPresenter : MonoBehaviour { }

    // Services/ShopService.cs
    public class ShopService : IShopService { }

    // Views/ShopItemView.cs
    public class ShopItemView { }
}

// ❌ 错误：为每个文件夹生成不同命名空间
namespace Game.Shop.Models  // 错误！
{
    public class ShopModel { }
}

namespace Game.Shop.Presenters  // 错误！
{
    public class ShopPresenter : MonoBehaviour { }
}

namespace Game.Shop.Services  // 错误！
{
    public class ShopService : IShopService { }
}
```

**规范说明**：
- ✅ 按**业务模块**划分命名空间，不按文件夹结构
- ✅ 一个业务模块 = 一个命名空间
- ❌ 禁止为每个子文件夹创建子命名空间
- ✅ 相关的Model、Presenter、Service都在同一命名空间下

**业务模块示例**：
```
Game.Shop          - 商店系统
Game.Inventory     - 背包系统
Game.Combat        - 战斗系统
Game.Level         - 关卡系统
Game.UI            - 通用UI
Game.Core          - 核心功能
```

### 日志管理规范（P1）

**强制规则：每个业务模块必须有日志总开关**

```csharp
// ✅ 正确：业务模块日志管理
namespace Game.Shop
{
    /// <summary>
    /// 商店模块日志工具
    /// </summary>
    public static class ShopLogger
    {
        // 日志总开关
        private const bool ENABLE_LOG = true;

        /// <summary>
        /// 输出普通日志
        /// </summary>
        public static void Log(string message)
        {
            if (!ENABLE_LOG) return;
            Debug.Log($"[Shop] {message}");
        }

        /// <summary>
        /// 输出警告日志
        /// </summary>
        public static void LogWarning(string message)
        {
            if (!ENABLE_LOG) return;
            Debug.LogWarning($"[Shop] {message}");
        }

        /// <summary>
        /// 输出错误日志（错误日志不受开关控制）
        /// </summary>
        public static void LogError(string message)
        {
            Debug.LogError($"[Shop] {message}");
        }
    }

    // 使用示例
    public class ShopService : IShopService
    {
        public void BuyItem(string itemId)
        {
            ShopLogger.Log($"购买道具: {itemId}");

            if (string.IsNullOrEmpty(itemId))
            {
                ShopLogger.LogError("道具ID为空！");
                return;
            }

            ShopLogger.Log("购买成功");
        }
    }
}

// ❌ 错误：直接使用Debug.Log
public class ShopService : IShopService
{
    public void BuyItem(string itemId)
    {
        Debug.Log($"购买道具: {itemId}");  // 错误！无法统一控制
    }
}
```

**规范说明**：
- ✅ 每个业务模块创建 `{ModuleName}Logger` 静态类
- ✅ 使用 `ENABLE_LOG` 常量作为总开关
- ✅ 所有日志输出必须调用 Logger 函数，不能直接使用 Debug.Log
- ✅ 错误日志（LogError）不受开关控制，始终输出
- ✅ 日志前缀使用模块名，便于过滤（如 `[Shop]`）

**完整模块示例**：
```csharp
namespace Game.Shop
{
    // 1. 日志工具
    public static class ShopLogger
    {
        private const bool ENABLE_LOG = true;
        public static void Log(string message) { /* ... */ }
        public static void LogWarning(string message) { /* ... */ }
        public static void LogError(string message) { /* ... */ }
    }

    // 2. Model
    public class ShopModel
    {
        public void UpdateGold(int amount)
        {
            ShopLogger.Log($"金币变化: {amount}");
        }
    }

    // 3. Service
    public class ShopService : IShopService
    {
        public void BuyItem(string itemId)
        {
            ShopLogger.Log($"购买道具: {itemId}");
        }
    }

    // 4. Presenter
    public class ShopPresenter : MonoBehaviour
    {
        private void Start()
        {
            ShopLogger.Log("商店界面初始化");
        }
    }
}
```

**检查清单**：
- [ ] 每个业务模块有独立的命名空间
- [ ] 命名空间不按文件夹分层
- [ ] 每个模块有 `{ModuleName}Logger` 类
- [ ] Logger 有 ENABLE_LOG 开关
- [ ] 所有日志调用 Logger 函数，不直接用 Debug.Log

## 常见错误 vs 最佳实践

| ❌ 错误 | ✅ 正确 |
|--------|--------|
| Model继承MonoBehaviour | Model是纯C#类 |
| Model使用Transform | Model不依赖Unity |
| View直接访问Model | 通过Presenter中转 |
| 使用FindObjectOfType | 使用依赖注入 |
| 使用单例模式 | 使用Zenject单例 |
| 业务逻辑在MonoBehaviour | 业务逻辑在Service |
| 忘记取消事件订阅 | 使用AddTo(this) |
| 直接修改public字段 | 使用ReactiveProperty |
| 按文件夹划分命名空间 | 按业务模块划分命名空间 |
| 直接使用Debug.Log | 使用模块Logger统一管理 |

## MVP架构示例对比

### ❌ 错误的架构（耦合）

```csharp
public class PlayerController : MonoBehaviour
{
    public int health = 100;  // 数据在MonoBehaviour
    public Transform targetEnemy;

    void Update()
    {
        // 业务逻辑在Update
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Attack();
        }
    }

    void Attack()
    {
        // 业务逻辑在MonoBehaviour
        var enemy = FindObjectOfType<Enemy>();
        enemy.TakeDamage(10);
    }
}
```

### ✅ 正确的架构（解耦）

```csharp
// Model - 纯C#数据
public class PlayerModel
{
    public IReadOnlyReactiveProperty<int> Health => _health;
    private ReactiveProperty<int> _health = new ReactiveProperty<int>(100);

    public void TakeDamage(int damage)
    {
        _health.Value -= damage;
    }
}

// Service - 业务逻辑
public class CombatService
{
    private readonly PlayerModel _player;
    private readonly IEnemyService _enemyService;

    public CombatService(PlayerModel player, IEnemyService enemyService)
    {
        _player = player;
        _enemyService = enemyService;
    }

    public void Attack()
    {
        _enemyService.DamageNearestEnemy(10);
    }
}

// Presenter - Unity生命周期
public class PlayerPresenter : MonoBehaviour
{
    [SerializeField] private Slider _healthBar;

    private PlayerModel _player;
    private CombatService _combat;

    [Inject]
    public void Construct(PlayerModel player, CombatService combat)
    {
        _player = player;
        _combat = combat;
    }

    private void Start()
    {
        _player.Health
            .Subscribe(h => _healthBar.value = h / 100f)
            .AddTo(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _combat.Attack();
        }
    }
}
```

## 代码审查检查清单

### P0 硬性约束
- [ ] Model不依赖Unity（无MonoBehaviour、无Unity API）
- [ ] 所有依赖通过构造函数注入
- [ ] MonoBehaviour仅处理Unity生命周期
- [ ] 事件正确订阅和取消

### P1 架构规范
- [ ] MVP三层职责清晰
- [ ] 使用ReactiveProperty暴露状态
- [ ] 使用Zenject注册依赖
- [ ] 使用UniRx的AddTo管理订阅
- [ ] 业务模块使用统一命名空间（不按文件夹分层）
- [ ] 每个业务模块有独立的Logger类
- [ ] 所有日志通过Logger输出（不直接用Debug.Log）

### P2 代码质量
- [ ] 命名符合规范（PascalCase/_camelCase）
- [ ] 中文注释完整
- [ ] 空引用检查
- [ ] 异常处理

### P3 性能
- [ ] Update中无GC分配
- [ ] 组件引用已缓存
- [ ] 使用对象池（如适用）

## 快速命令参考

### 检查Model是否依赖Unity
```bash
# 检查是否继承MonoBehaviour
grep "class.*Model.*:.*MonoBehaviour" Assets/Scripts --include="*.cs" -r

# 检查是否使用Unity API
grep -E "(Transform|GameObject|Coroutine)" Assets/Scripts/Models --include="*.cs" -r
```

### 检查依赖注入
```bash
# 查找未使用Inject的依赖
grep "GetComponent\|FindObjectOfType" Assets/Scripts --include="*.cs" -r

# 查找硬编码的单例
grep "\.Instance" Assets/Scripts --include="*.cs" -r
```

### 检查事件泄漏
```bash
# 查找+=订阅
grep "+=" Assets/Scripts --include="*.cs" -r

# 检查对应的-=取消订阅
grep "OnDestroy" -A 10 Assets/Scripts --include="*.cs" -r | grep "-="
```

### 检查命名空间规范
```bash
# 查找过度嵌套的命名空间（可能违反业务模块规范）
grep "namespace.*\..*\..*\." Assets/Scripts --include="*.cs" -r

# 查找按文件夹分层的命名空间（Models/Services/Presenters）
grep -E "namespace.*(\.Models|\.Services|\.Presenters|\.Views)" Assets/Scripts --include="*.cs" -r
```

### 检查日志规范
```bash
# 查找直接使用Debug.Log的代码（应该使用Logger）
grep -E "Debug\.(Log|LogWarning|LogError)" Assets/Scripts --include="*.cs" -r

# 查找Logger类定义
grep "class.*Logger" Assets/Scripts --include="*.cs" -r

# 检查Logger是否有日志开关
grep "ENABLE_LOG" Assets/Scripts --include="*.cs" -r
```

## 详细规范文档

完整规范请参考 `references/` 目录：

**P0 约束**：
- [MVP分离约束](references/constraints/mvp-separation.md) ⭐⭐⭐
- [依赖注入规范](references/constraints/dependency-injection.md) ⭐⭐⭐
- [MonoBehaviour规则](references/constraints/monobehaviour-rules.md) ⭐⭐⭐

**P1 架构**：
- [MVP架构实现](references/architecture/mvp-architecture.md)
- [Zenject模式](references/architecture/zenject-patterns.md)
- [UniRx模式](references/architecture/unirx-patterns.md)
- [事件管理](references/architecture/event-management.md)

**P2 质量**：
- [代码风格](references/quality/code-style.md)
- [错误处理](references/quality/error-handling.md)
- [空引用安全](references/quality/null-safety.md)

## 提交前最终检查

```bash
# 1. 编译通过
# Unity中编译，无错误和警告

# 2. 架构检查
grep "class.*Model.*:.*MonoBehaviour" Assets/Scripts -r  # 应该为空

# 3. 依赖注入检查
grep "FindObjectOfType\|GetComponent" Assets/Scripts/Models -r  # 应该为空

# 4. 事件订阅检查
# 每个+=应该有对应的-=或AddTo(this)
```

## 记住这些原则

1. **Model = 纯C#**：Model永远不依赖Unity
2. **Presenter = 协调者**：Presenter只协调，不包含业务逻辑
3. **依赖注入优先**：永远不使用Find、GetComponent
4. **ReactiveProperty**：状态变化使用响应式
5. **AddTo(this)**：订阅自动管理生命周期
6. **接口编程**：依赖抽象不依赖具体
7. **业务模块命名空间**：一个业务模块一个命名空间，不按文件夹分层
8. **统一日志管理**：每个模块有Logger，所有日志通过Logger输出

**当不确定时，问自己**：
- 这个类可以在非Unity环境测试吗？（Model应该可以）
- 这个依赖可以通过构造函数注入吗？（应该可以）
- 这个订阅会造成内存泄漏吗？（使用AddTo避免）
- 这个命名空间是按业务模块划分的吗？（不是按文件夹）
- 这个日志是通过模块Logger输出的吗？（不直接用Debug.Log）
