# 阶段4：代码审查 - 详细指南

## 负责代理
- `unity-code-reviewer` - 参考 `.claude/agents/unity-code-reviewer.md`
- `unity-performance` - 参考 `.claude/agents/unity-performance.md`（性能审查）

## 目标
确保代码质量、规范性、安全性、可维护性和**高性能**，验证实现是否符合架构设计。

## 执行步骤

### 1. 代码浏览（5分钟）
- 识别修改和新增的文件
- 理解代码整体结构
- 对照设计文档检查实现

### 2. 系统化审查（15-20分钟）

#### 2.1 架构一致性审查
- [ ] 是否按照设计文档实现？
- [ ] 模块划分是否符合设计？
- [ ] 依赖关系是否正确？
- [ ] 接口定义是否一致？
- [ ] 是否有未经设计的偏离？

#### 2.2 代码质量审查

**职责单一性**：
```csharp
// ❌ 违反单一职责
public class GameManager
{
    public void StartGame() { }
    public void UpdateUI() { }
    public void PlaySound() { }
    public void SaveData() { }
}

// ✅ 职责清晰
public class GameController
{
    private readonly IUIService _ui;
    private readonly IAudioService _audio;
    private readonly IStorageService _storage;

    public void StartGame()
    {
        _ui.ShowGameScreen();
        _audio.PlayMusic();
    }
}
```
- [ ] 每个类是否只有一个职责？
- [ ] 方法是否简短（<50行）？
- [ ] 是否避免"上帝类"？

**代码重复**：
- [ ] 是否存在重复代码？
- [ ] 重复代码是否已提取？

**复杂度**：
- [ ] 嵌套层级是否合理（<3层）？
- [ ] 条件判断是否过于复杂？
- [ ] 是否应该使用多态？

#### 2.3 命名规范审查
```csharp
// ✅ 正确命名
public class PlayerController                  // PascalCase
{
    private readonly IGameService _service;    // _camelCase
    private const int MaxHealth = 100;         // PascalCase
    public int CurrentHealth { get; set; }     // PascalCase

    public void TakeDamage(int damage)         // PascalCase
    {
        int remaining = CurrentHealth - damage; // camelCase
    }
}
```
- [ ] 类名是否PascalCase？
- [ ] 私有字段是否_camelCase？
- [ ] 方法名是否PascalCase？
- [ ] 变量名是否有意义？

#### 2.4 注释规范审查
```csharp
// ✅ 好的注释（中文）
/// <summary>
/// 游戏控制器，负责管理游戏流程
/// </summary>
public class GameController
{
    /// <summary>
    /// 开始新游戏
    /// </summary>
    /// <param name="level">关卡编号</param>
    public void StartGame(int level)
    {
        // 初始化游戏状态
        InitializeGameState();
    }
}
```
- [ ] 公共类和方法是否有XML注释？
- [ ] 注释是否使用中文？
- [ ] 复杂逻辑是否有解释？
- [ ] 注释是否准确？

#### 2.5 Unity特定审查

**MonoBehaviour使用**：
```csharp
// ❌ 业务逻辑在MonoBehaviour
public class GameManager : MonoBehaviour
{
    void Update()
    {
        // 大量业务逻辑
    }
}

// ✅ MonoBehaviour只处理Unity生命周期
public class GameView : MonoBehaviour
{
    private IGameController _controller;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _controller.Jump();
        }
    }
}
```
- [ ] MonoBehaviour是否只处理Unity生命周期？
- [ ] 业务逻辑是否分离？

**协程管理**：
```csharp
// ✅ 正确的协程管理
public class CoroutineExample : MonoBehaviour
{
    private Coroutine _routine;

    void Start()
    {
        _routine = StartCoroutine(UpdateLoop());
    }

    void OnDestroy()
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
        }
    }

    IEnumerator UpdateLoop()
    {
        while (enabled)
        {
            yield return new WaitForSeconds(1f);
        }
    }
}
```
- [ ] 协程是否正确停止？
- [ ] OnDestroy是否清理协程？

**事件管理**：
```csharp
// ✅ 正确的事件管理
public class EventExample : MonoBehaviour
{
    void Start()
    {
        EventManager.OnGameOver += HandleGameOver;
    }

    void OnDestroy()
    {
        EventManager.OnGameOver -= HandleGameOver;
    }

    void HandleGameOver() { }
}
```
- [ ] 事件是否取消订阅？
- [ ] 是否使用AddTo自动管理？

#### 2.6 安全性审查

**空引用检查**：
```csharp
// ✅ 有空检查
public void MoveTo()
{
    if (_target == null)
    {
        Debug.LogWarning("目标为空");
        return;
    }
    transform.position = _target.position;
}
```
- [ ] 是否检查可能为null的引用？
- [ ] GetComponent是否检查返回值？

**异常处理**：
```csharp
// ✅ 正确的异常处理
public void LoadData()
{
    try
    {
        var data = File.ReadAllText("data.json");
    }
    catch (FileNotFoundException e)
    {
        Debug.LogError($"文件未找到: {e.Message}");
        UseDefaultData();
    }
}
```
- [ ] 是否避免空catch块？
- [ ] 是否记录异常信息？

**资源释放**：
```csharp
// ✅ 正确释放资源
void OnDestroy()
{
    ClearObjects();
    UnsubscribeEvents();
    StopAllCoroutines();
}
```
- [ ] 创建的对象是否销毁？
- [ ] 事件是否取消订阅？
- [ ] 协程是否停止？

#### 2.7 性能影响审查

**常见性能问题**：
```csharp
// ❌ 性能问题
void Update()
{
    var player = GameObject.Find("Player");  // 每帧查找
    var rb = GetComponent<Rigidbody>();      // 每帧获取
}

// ✅ 性能优化
private Transform _player;
private Rigidbody _rb;

void Awake()
{
    _player = GameObject.FindWithTag("Player").transform;
    _rb = GetComponent<Rigidbody>();
}
```
- [ ] 是否缓存组件引用？
- [ ] 是否减少GC分配？
- [ ] Update中是否有昂贵操作？

### 3. 输出审查报告（必须）

#### 代码审查报告模板
```markdown
# [功能名称] 代码审查报告

## 一、审查概要
- 审查文件：[列出文件]
- 审查时间：[时间]
- 总体评价：✅通过 / ⚠️需要修改 / ❌不通过

## 二、优点
- [列出代码的优点]

## 三、问题清单

### 🔴 高优先级（必须修改）
1. **[问题类型]** 文件:行号
   - 问题：...
   - 影响：...
   - 修改建议：...

### 🟡 中优先级（建议修改）
[同上]

### 🟢 低优先级（可选优化）
[同上]

## 四、代码示例

### 问题代码
\`\`\`csharp
// 当前代码
\`\`\`

### 建议修改
\`\`\`csharp
// 改进后代码
\`\`\`

## 五、检查清单完成情况
- [x] 架构一致性
- [x] 代码质量
- [x] 命名规范
- [ ] 注释完整性（需补充）
- [x] Unity特定规范
- [x] 安全性
- [x] 性能影响

## 六、后续行动
- [ ] 修复高优先级问题
- [ ] 修复中优先级问题
- [ ] 重新审查
```

### 4. 与开发者讨论（5-10分钟）
- 说明发现的问题
- 听取开发者解释
- 讨论修改方案
- 确认修改计划

## 审查态度

### 保持专业
- 关注代码，不针对个人
- 用"建议"而非"命令"
- 承认好的实现

### 示例反馈
```
❌ "这代码写得太烂了"
✅ "这里可以改进：建议使用依赖注入替代单例，这样更易测试"

❌ "命名太差"
✅ "_mgr不够清晰，建议改为_gameManager，更符合命名规范"
```

## 审查结论判断

### ✅ 通过
- 无高优先级问题
- 中低优先级问题可接受
- 代码质量良好

### ⚠️ 需要修改
- 有中优先级问题需解决
- 需要小幅调整
- 总体质量可接受

### ❌ 不通过
- 有高优先级问题
- 代码质量不达标
- 需要重构

## 快速审查检查表

**1分钟快速检查**：
- [ ] 是否有编译错误？
- [ ] 命名是否规范？
- [ ] 是否有明显的性能问题？
- [ ] 是否有资源泄漏？

**5分钟重点检查**：
- [ ] 架构一致性
- [ ] 代码质量
- [ ] Unity特定问题
- [ ] 安全性问题

## 常见问题速查

### 必须修复
- 编译错误
- 空引用未检查
- 资源未释放
- 事件未取消订阅
- 严重性能问题

### 建议修复
- 命名不规范
- 注释不足
- 代码重复
- 方法过长
- 复杂度高

### 可选优化
- 变量命名可以更清晰
- 可以添加更多注释
- 可以提取辅助方法
