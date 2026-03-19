# MonoBehaviour使用规则（P0 - 硬性约束）

## 核心原则

**MonoBehaviour仅用于Unity生命周期管理，不包含业务逻辑**

## MonoBehaviour的正确用途

### ✅ 允许使用MonoBehaviour的场景
1. **Presenter层** - 协调Model和View
2. **Unity生命周期管理** - Awake、Start、Update、OnDestroy
3. **Unity事件响应** - OnTriggerEnter、OnCollisionEnter
4. **SerializeField** - 引用Unity组件和Prefab
5. **协程宿主** - 启动协程（但业务逻辑应在Service）

### ❌ 禁止使用MonoBehaviour的场景
1. **Model层** - Model必须是纯C#类
2. **Service层** - Service应该是纯C#类（除非必须使用协程）
3. **数据存储** - 数据应该在Model中，不在MonoBehaviour

## 生命周期方法使用规范

### Awake

**用途**：
- 获取本对象的组件引用
- 初始化本地状态
- **不访问外部对象**

```csharp
// ✅ 正确使用Awake
public class CardPresenter : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Collider2D _collider;

    private Transform _transform;

    void Awake()
    {
        // 只获取本对象组件
        _transform = transform;

        // 初始化本地状态
        _collider.enabled = false;
    }
}

// ❌ 错误使用Awake
void Awake()
{
    // 错误！访问外部对象（此时可能未初始化）
    var manager = GameManager.Instance;
    manager.RegisterPlayer(this);

    // 错误！访问注入的依赖（此时未注入）
    _gameModel.StartGame();

    // 错误！"自注册反模式"：被管理对象调用 Manager 的缓存写入
    // 后果：GameObject 若以 SetActive(false) 开始，Awake 不触发 → 永远不注册
    UILayerManager.Instance?.Register(this);
}
```

### Start

**用途**：
- 使用注入的依赖
- 订阅Model事件
- 访问其他对象
- 初始化需要依赖的逻辑

```csharp
// ✅ 正确使用Start
public class GamePresenter : MonoBehaviour
{
    private IGameModel _model;

    [Inject]
    public void Construct(IGameModel model)
    {
        _model = model;
    }

    void Start()
    {
        // 此时依赖已注入，可以安全使用
        _model.State
            .Subscribe(OnStateChanged)
            .AddTo(this);

        // 初始化
        _model.Initialize();
    }
}
```

### Update

**用途**：
- 处理用户输入
- 更新动画
- **不包含业务逻辑**

```csharp
// ✅ 正确使用Update
void Update()
{
    // 处理输入，委托给Service
    if (Input.GetKeyDown(KeyCode.Space))
    {
        _gameService.Jump();
    }

    // 更新动画
    _animator.SetFloat("Speed", _playerModel.Speed.Value);
}

// ❌ 错误使用Update
void Update()
{
    // 错误！业务逻辑不应该在Update
    if (health <= 0)
    {
        Die();
        Respawn();
        ResetScore();
    }

    // 错误！复杂计算不应该在Update
    foreach (var enemy in enemies)
    {
        var distance = Vector3.Distance(transform.position, enemy.position);
        if (distance < 10f)
        {
            Attack(enemy);
        }
    }
}
```

### OnDestroy

**用途**：
- 取消事件订阅
- 释放资源
- 清理引用

```csharp
// ✅ 正确使用OnDestroy
void OnDestroy()
{
    // 取消事件订阅
    if (_model != null)
    {
        _model.OnGameOver -= HandleGameOver;
    }

    // 释放资源
    if (_texture != null)
    {
        Destroy(_texture);
    }
}

// ⚠️ 使用UniRx的AddTo可以自动管理
void Start()
{
    _model.State
        .Subscribe(OnStateChanged)
        .AddTo(this);  // 自动在OnDestroy时取消订阅
}
```

## MonoBehaviour分类

### 1. Presenter（最常见）

```csharp
// 协调Model和View
public class PlayerPresenter : MonoBehaviour
{
    // View引用
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _renderer;

    // Model引用（注入）
    private IPlayerModel _model;

    [Inject]
    public void Construct(IPlayerModel model)
    {
        _model = model;
    }

    void Start()
    {
        // 订阅Model，更新View
        _model.Health
            .Subscribe(h => UpdateHealthBar(h))
            .AddTo(this);
    }

    void Update()
    {
        // 处理输入
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _model.Jump();
        }
    }
}
```

### 2. View Component（纯Unity组件封装）

```csharp
// 封装Unity组件，不包含业务逻辑
public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private Text _text;

    public void SetHealth(int current, int max)
    {
        _slider.value = (float)current / max;
        _text.text = $"{current}/{max}";
    }
}
```

### 3. Unity Event Handler（Unity事件响应）

```csharp
// 处理Unity物理/碰撞事件
public class TriggerDetector : MonoBehaviour
{
    private ITriggerService _triggerService;

    [Inject]
    public void Construct(ITriggerService service)
    {
        _triggerService = service;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 委托给Service处理
        _triggerService.HandleTrigger(other.gameObject);
    }
}
```

### 4. Coroutine Host（协程宿主）

```csharp
// 仅作为协程宿主，逻辑在Service中
public class DelayedActionHost : MonoBehaviour
{
    private IDelayedActionService _service;

    [Inject]
    public void Construct(IDelayedActionService service)
    {
        _service = service;
        _service.SetCoroutineHost(this);
    }
}

// Service中的业务逻辑
public class DelayedActionService : IDelayedActionService
{
    private MonoBehaviour _coroutineHost;

    public void SetCoroutineHost(MonoBehaviour host)
    {
        _coroutineHost = host;
    }

    public void ExecuteAfterDelay(float delay, Action action)
    {
        _coroutineHost.StartCoroutine(DelayCoroutine(delay, action));
    }

    private IEnumerator DelayCoroutine(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
}
```

## 常见错误模式

### ❌ God MonoBehaviour

```csharp
// 错误！MonoBehaviour包含所有逻辑
public class GameManager : MonoBehaviour
{
    // 数据字段
    public int score;
    public int health;
    public List<Enemy> enemies;

    void Update()
    {
        // 大量业务逻辑
        UpdateEnemies();
        CheckWinCondition();
        UpdateUI();
        ProcessInput();
    }

    void UpdateEnemies() { /* 复杂逻辑 */ }
    void CheckWinCondition() { /* 复杂逻辑 */ }
    void UpdateUI() { /* 复杂逻辑 */ }
    void ProcessInput() { /* 复杂逻辑 */ }
}
```

### ✅ 正确拆分

```csharp
// Model
public class GameModel
{
    public IReadOnlyReactiveProperty<int> Score => _score;
    private ReactiveProperty<int> _score = new ReactiveProperty<int>();
}

// Service
public class GameService
{
    private readonly GameModel _model;

    public void AddScore(int points)
    {
        // 业务逻辑
    }

    public bool CheckWinCondition()
    {
        // 业务逻辑
        return false;
    }
}

// Presenter
public class GamePresenter : MonoBehaviour
{
    private GameModel _model;
    private GameService _service;

    [Inject]
    public void Construct(GameModel model, GameService service)
    {
        _model = model;
        _service = service;
    }

    void Start()
    {
        // 只订阅和协调
        _model.Score.Subscribe(UpdateScoreUI).AddTo(this);
    }

    void Update()
    {
        // 只处理输入
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _service.ProcessAction();
        }
    }
}
```

## 检查清单

### MonoBehaviour审查清单
- [ ] 是否只用于Unity生命周期？
- [ ] 是否通过[Inject]获取依赖？
- [ ] Awake中是否只访问本对象？
- [ ] **Awake中是否调用了外部 Manager.Register(this)？**（自注册反模式：应由 Manager 掌控注册，而非被管理对象在 Awake 中自行注册）
- [ ] Start中是否正确订阅Model？
- [ ] Update中是否包含业务逻辑？（应该没有）
- [ ] OnDestroy中是否取消订阅？
- [ ] 是否使用AddTo(this)自动管理订阅？

### 快速验证命令

```bash
# 查找MonoBehaviour中的业务逻辑关键字
grep -E "(Calculate|Process|Compute|Algorithm)" Assets/Scripts/Presenters --include="*.cs" -r

# 查找Awake中访问外部对象
grep "Awake" -A 10 Assets/Scripts --include="*.cs" -r | grep -E "(FindObject|Instance|GetComponent)"

# 查找Update中的复杂逻辑
grep "void Update()" -A 20 Assets/Scripts --include="*.cs" -r | wc -l
```

## 重构指南

### 步骤1：识别业务逻辑

找出MonoBehaviour中的业务逻辑代码：
- 复杂计算
- 状态管理
- 业务规则

### 步骤2：提取到Service

```csharp
// 原始代码
public class PlayerController : MonoBehaviour
{
    public int health = 100;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 业务逻辑
            if (health > 20)
            {
                health -= 20;
                JumpHigh();
            }
            else
            {
                JumpLow();
            }
        }
    }
}

// 重构后：Model
public class PlayerModel
{
    public IReadOnlyReactiveProperty<int> Health => _health;
    private ReactiveProperty<int> _health = new ReactiveProperty<int>(100);

    public void ConsumeHealth(int amount)
    {
        _health.Value -= amount;
    }
}

// 重构后：Service
public class JumpService
{
    private readonly PlayerModel _player;

    public JumpService(PlayerModel player)
    {
        _player = player;
    }

    public JumpType GetJumpType()
    {
        return _player.Health.Value > 20 ? JumpType.High : JumpType.Low;
    }
}

// 重构后：Presenter
public class PlayerPresenter : MonoBehaviour
{
    private PlayerModel _player;
    private JumpService _jump;

    [Inject]
    public void Construct(PlayerModel player, JumpService jump)
    {
        _player = player;
        _jump = jump;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var jumpType = _jump.GetJumpType();
            ExecuteJump(jumpType);
            _player.ConsumeHealth(20);
        }
    }
}
```

## 总结

**MonoBehaviour的职责**：
- Unity生命周期管理
- 协调Model和View
- 处理Unity事件
- 作为协程宿主

**MonoBehaviour不应该**：
- 包含业务逻辑
- 存储数据（数据在Model）
- 进行复杂计算（计算在Service）
- 直接操作其他对象（通过Service）

**记住公式**：
```
MonoBehaviour = Unity生命周期 + 依赖注入 + 订阅Model
```
