# 依赖注入规范（P0 - 硬性约束）

## 核心原则

**所有依赖必须通过依赖注入获取，禁止使用查找和单例**

## 依赖注入框架

支持的框架：
- **Zenject** （推荐）
- **VContainer** （轻量级替代）
- **自定义DI容器**

本文档以Zenject为例，但原则适用于所有DI框架。

## 硬性规则

### ✅ 必须做
- **必须**使用构造函数注入或[Inject]方法注入
- **必须**在Installer中注册所有依赖
- **必须**依赖接口而非具体实现
- **必须**使用DI容器管理单例生命周期

### ❌ 禁止做
- **禁止**使用`FindObjectOfType`
- **禁止**使用`GetComponent`获取服务
- **禁止**使用静态单例模式（`ClassName.Instance`）
- **禁止**使用`GameObject.Find`
- **禁止**在构造函数或Awake中访问其他对象

## 注入模式

### 1. 构造函数注入（推荐 - 用于Model和Service）

```csharp
// ✅ 正确：构造函数注入
public class PlayerModel
{
    private readonly IScoreService _scoreService;
    private readonly IAudioService _audioService;

    // 依赖通过构造函数注入
    public PlayerModel(
        IScoreService scoreService,
        IAudioService audioService)
    {
        _scoreService = scoreService;
        _audioService = audioService;
    }

    public void Attack()
    {
        _scoreService.AddScore(10);
        _audioService.PlaySound("attack");
    }
}
```

### 2. 方法注入（用于MonoBehaviour）

```csharp
// ✅ 正确：使用[Inject]方法注入
public class GamePresenter : MonoBehaviour
{
    private IGameModel _game;
    private IAudioService _audio;

    [Inject]
    public void Construct(IGameModel game, IAudioService audio)
    {
        _game = game;
        _audio = audio;
    }

    private void Start()
    {
        // 此时依赖已注入完成
        _game.StartGame();
    }
}
```

### 3. 字段注入（不推荐，但可用于快速原型）

```csharp
// ⚠️ 可用但不推荐：字段注入
public class SomePresenter : MonoBehaviour
{
    [Inject] private IGameModel _game;  // 私有字段注入
    [Inject] public IAudioService Audio { get; set; }  // 属性注入

    // 注意：不要在Awake中使用，依赖可能未注入
    void Start()
    {
        _game.StartGame();  // 此时已注入
    }
}
```

## 注册模式

### Installer配置

```csharp
public class GameInstaller : MonoInstaller
{
    [SerializeField] private GameConfig _config;
    [SerializeField] private CardPrefab _cardPrefab;

    public override void InstallBindings()
    {
        // 1. 单例Model
        Container.Bind<PlayerModel>().AsSingle();
        Container.Bind<GameState>().AsSingle();

        // 2. 接口绑定Service（单例）
        Container.BindInterfacesAndSelfTo<AudioService>().AsSingle();
        Container.BindInterfacesAndSelfTo<ScoreService>().AsSingle();
        Container.BindInterfacesAndSelfTo<InputService>().AsSingle();

        // 3. 配置绑定（ScriptableObject）
        Container.Bind<GameConfig>()
            .FromInstance(_config)
            .AsSingle();

        // 4. 工厂绑定（用于动态创建对象）
        Container.BindFactory<Card, Card.Factory>()
            .FromComponentInNewPrefab(_cardPrefab)
            .UnderTransformGroup("Cards");

        // 5. 内存池绑定（用于对象池）
        Container.BindMemoryPool<Bullet, BulletPool>()
            .WithInitialSize(20)
            .FromComponentInNewPrefab(_bulletPrefab)
            .UnderTransformGroup("Bullets");

        // 6. 瞬态绑定（每次请求创建新实例）
        Container.Bind<TempData>().AsTransient();
    }
}
```

### 场景Installer

```csharp
// 场景特定的依赖注入
public class MenuSceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<MenuModel>().AsSingle();
        Container.Bind<MenuPresenter>()
            .FromComponentInHierarchy()
            .AsSingle();
    }
}
```

## 生命周期管理

### 单例（AsSingle）

```csharp
// 整个游戏生命周期内只有一个实例
Container.Bind<GameManager>().AsSingle();
Container.BindInterfacesAndSelfTo<AudioService>().AsSingle();
```

### 瞬态（AsTransient）

```csharp
// 每次请求都创建新实例
Container.Bind<TempCalculator>().AsTransient();
```

### 工厂（Factory）

```csharp
// 通过工厂动态创建
Container.BindFactory<Enemy, Enemy.Factory>()
    .FromComponentInNewPrefab(_enemyPrefab);

// 使用工厂
public class EnemySpawner
{
    private readonly Enemy.Factory _enemyFactory;

    public EnemySpawner(Enemy.Factory enemyFactory)
    {
        _enemyFactory = enemyFactory;
    }

    public void SpawnEnemy()
    {
        var enemy = _enemyFactory.Create();
    }
}
```

## 常见错误对比

### ❌ 错误示例

```csharp
// ❌ 错误：使用FindObjectOfType
public class BadPresenter : MonoBehaviour
{
    private GameManager _manager;

    void Awake()
    {
        _manager = FindObjectOfType<GameManager>();
    }
}

// ❌ 错误：使用静态单例
public class BadManager : MonoBehaviour
{
    public static BadManager Instance;

    void Awake()
    {
        Instance = this;
    }
}

public class BadClient
{
    void DoSomething()
    {
        BadManager.Instance.DoWork();  // 硬依赖！
    }
}

// ❌ 错误：使用GetComponent获取服务
public class BadService : MonoBehaviour
{
    void Start()
    {
        var audio = GetComponent<AudioService>();
    }
}

// ❌ 错误：在Awake中使用注入的依赖
public class BadLifecycle : MonoBehaviour
{
    [Inject] private IAudioService _audio;

    void Awake()
    {
        _audio.PlaySound("test");  // 危险！可能未注入
    }
}
```

### ✅ 正确示例

```csharp
// ✅ 正确：使用依赖注入
public class GoodPresenter : MonoBehaviour
{
    private IGameManager _manager;

    [Inject]
    public void Construct(IGameManager manager)
    {
        _manager = manager;
    }

    void Start()  // 在Start中使用，确保已注入
    {
        _manager.DoWork();
    }
}

// ✅ 正确：通过DI容器管理单例
public class GoodManager
{
    // 不需要Instance，通过DI获取
}

// 在Installer中注册
Container.BindInterfacesAndSelfTo<GoodManager>().AsSingle();

// ✅ 正确：依赖接口
public class GoodClient
{
    private readonly IGoodManager _manager;

    public GoodClient(IGoodManager manager)
    {
        _manager = manager;
    }

    void DoSomething()
    {
        _manager.DoWork();  // 松耦合！
    }
}
```

## 接口设计原则

### 定义服务接口

```csharp
// 接口定义服务契约
public interface IAudioService
{
    void PlaySound(string clipName);
    void PlayMusic(string musicName);
    void StopMusic();
}

// 具体实现
public class AudioService : IAudioService
{
    public void PlaySound(string clipName)
    {
        // 实现
    }

    public void PlayMusic(string musicName)
    {
        // 实现
    }

    public void StopMusic()
    {
        // 实现
    }
}

// 注册
Container.BindInterfacesAndSelfTo<AudioService>().AsSingle();
```

## 测试支持

### 依赖注入便于测试

```csharp
// 生产代码依赖接口
public class PlayerController
{
    private readonly IAudioService _audio;

    public PlayerController(IAudioService audio)
    {
        _audio = audio;
    }

    public void Attack()
    {
        _audio.PlaySound("attack");
    }
}

// 测试代码使用Mock
[Test]
public void Attack_Should_PlaySound()
{
    // Arrange
    var mockAudio = new MockAudioService();
    var player = new PlayerController(mockAudio);

    // Act
    player.Attack();

    // Assert
    Assert.IsTrue(mockAudio.WasPlaySoundCalled);
}
```

## 检查清单

### 代码审查检查
- [ ] 没有使用FindObjectOfType
- [ ] 没有使用GameObject.Find
- [ ] 没有使用静态单例（.Instance）
- [ ] 所有MonoBehaviour使用[Inject]方法注入
- [ ] 所有纯C#类使用构造函数注入
- [ ] 依赖在Installer中注册
- [ ] 依赖接口而非具体类
- [ ] 不在Awake中使用注入的依赖

### 快速验证命令

```bash
# 查找禁止的模式
grep "FindObjectOfType\|GameObject\.Find" Assets/Scripts --include="*.cs" -r

# 查找静态单例
grep "\.Instance" Assets/Scripts --include="*.cs" -r

# 查找是否所有MonoBehaviour都使用了Inject
grep "class.*:.*MonoBehaviour" Assets/Scripts --include="*.cs" -r -A 5 | grep -c "\[Inject\]"
```

## 最佳实践

### 1. 最小化公共接口

```csharp
// ✅ 好的接口：最小化暴露
public interface IGameService
{
    void StartGame();
    void EndGame();
    IObservable<GameState> GameStateChanged { get; }
}

// ❌ 坏的接口：暴露过多
public interface IBadGameService
{
    void StartGame();
    void EndGame();
    void InternalMethod1();  // 不应该暴露
    void InternalMethod2();  // 不应该暴露
    int internalField;       // 不应该暴露
}
```

### 2. 使用只读属性

```csharp
// ✅ 好的实践：只读暴露
public class GameModel
{
    public IReadOnlyReactiveProperty<int> Score => _score;
    private ReactiveProperty<int> _score = new ReactiveProperty<int>();

    public void AddScore(int points)
    {
        _score.Value += points;
    }
}
```

### 3. 依赖倒置

```csharp
// ✅ 高层模块依赖抽象
public class GameController
{
    private readonly IGameModel _model;       // 依赖接口
    private readonly IAudioService _audio;    // 依赖接口

    public GameController(IGameModel model, IAudioService audio)
    {
        _model = model;
        _audio = audio;
    }
}

// ❌ 高层模块依赖具体
public class BadGameController
{
    private readonly GameModel _model;        // 依赖具体类
    private readonly AudioService _audio;     // 依赖具体类

    // 难以测试和替换实现
}
```

## 总结

**记住**：
1. **构造函数注入** - 纯C#类的首选
2. **[Inject]方法注入** - MonoBehaviour的首选
3. **禁止Find** - 永远不使用FindObjectOfType
4. **禁止单例** - 使用DI容器管理单例
5. **接口依赖** - 依赖抽象不依赖具体
6. **Start中使用** - 不在Awake中使用注入的依赖

**违反依赖注入的后果**：
- 硬耦合，难以测试
- 依赖隐藏，不清晰
- 难以替换实现
- 无法并行开发

**遵守依赖注入的好处**：
- 松耦合，易测试
- 依赖清晰可见
- 易于替换实现
- 支持并行开发
