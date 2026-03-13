# MVP分离约束（P0 - 硬性约束）

## 核心原则

**数据与表现必须严格分离**

这是Solitaire项目架构的基石，违反此原则将导致架构崩溃。

## 三层职责定义

### Model（数据层）

**职责**：
- 管理游戏数据和状态
- 实现业务规则
- 提供数据变化通知（通过ReactiveProperty）

**约束**：
- ✅ **必须**是纯C#类
- ✅ **必须**可以在非Unity环境下运行和测试
- ❌ **绝对禁止**继承MonoBehaviour
- ❌ **绝对禁止**使用Unity API（Transform、GameObject、Coroutine等）
- ❌ **绝对禁止**使用Unity命名空间（除UnityEngine外的如UnityEngine.UI）
- ❌ **绝对禁止**依赖场景对象
- ❌ **绝对禁止**使用协程

**允许**：
- ✅ UniRx的ReactiveProperty（用于数据绑定）
- ✅ C#标准库
- ✅ 纯C#第三方库

### Presenter（逻辑层）

**职责**：
- 处理Unity生命周期（Awake、Start、Update等）
- 协调Model和View
- 订阅Model变化，更新View
- 处理用户输入，调用Model方法

**约束**：
- ✅ **必须**继承MonoBehaviour（或Presenter基类）
- ✅ **必须**通过[Inject]获取依赖
- ❌ **禁止**包含业务逻辑（委托给Service）
- ❌ **禁止**直接修改Model内部状态（调用Model的public方法）

### View（表现层）

**定义**：
- Unity的GameObject、Prefab
- UI组件（Button、Text、Image等）
- Animator、ParticleSystem等Unity组件

**约束**：
- ✅ 通过Presenter的SerializeField引用
- ❌ **禁止**直接访问Model
- ❌ **禁止**包含业务逻辑

## 示例对比

### ❌ 错误示例：Model依赖Unity

```csharp
// 错误！Model不应该继承MonoBehaviour
public class Card : MonoBehaviour
{
    public Transform transform;  // 错误！依赖Unity
    public Sprite sprite;        // 错误！依赖Unity

    void Start()  // 错误！Model不应该有生命周期
    {
        // ...
    }

    IEnumerator FlipAnimation()  // 错误！Model不应该用协程
    {
        yield return new WaitForSeconds(1f);
    }
}
```

### ✅ 正确示例：Model纯C#实现

```csharp
// 正确！纯C#类
public class Card
{
    // 数据字段
    public Suit Suit { get; private set; }
    public Rank Rank { get; private set; }

    // 响应式属性
    public IReadOnlyReactiveProperty<bool> IsFaceUp => _isFaceUp;
    private ReactiveProperty<bool> _isFaceUp = new ReactiveProperty<bool>(false);

    public IReadOnlyReactiveProperty<bool> IsHighlighted => _isHighlighted;
    private ReactiveProperty<bool> _isHighlighted = new ReactiveProperty<bool>(false);

    // 构造函数
    public Card(Suit suit, Rank rank)
    {
        Suit = suit;
        Rank = rank;
    }

    // 业务方法
    public void Flip()
    {
        _isFaceUp.Value = !_isFaceUp.Value;
    }

    public void Highlight()
    {
        _isHighlighted.Value = true;
    }

    public void Unhighlight()
    {
        _isHighlighted.Value = false;
    }

    // 业务规则
    public bool CanPlaceOn(Card other)
    {
        if (other == null) return Rank == Rank.King;
        return Rank == other.Rank - 1 && Suit.IsOppositeColor(other.Suit);
    }
}
```

### ✅ 正确示例：Presenter处理Unity

```csharp
public class CardPresenter : MonoBehaviour
{
    // View引用（SerializeField）
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private GameObject _highlightEffect;

    // Model引用（通过Factory创建）
    private Card _card;

    // 依赖注入
    private CardSprites _cardSprites;

    [Inject]
    public void Construct(CardSprites cardSprites)
    {
        _cardSprites = cardSprites;
    }

    // 设置Model
    public void SetCard(Card card)
    {
        _card = card;

        // 订阅Model变化
        _card.IsFaceUp
            .Subscribe(OnFaceUpChanged)
            .AddTo(this);

        _card.IsHighlighted
            .Subscribe(OnHighlightChanged)
            .AddTo(this);
    }

    // 响应Model变化，更新View
    private void OnFaceUpChanged(bool isFaceUp)
    {
        _spriteRenderer.sprite = isFaceUp
            ? _cardSprites.GetSprite(_card.Suit, _card.Rank)
            : _cardSprites.GetBackSprite();
    }

    private void OnHighlightChanged(bool isHighlighted)
    {
        _highlightEffect.SetActive(isHighlighted);
    }
}
```

## 检查清单

### Model层检查
- [ ] 不继承MonoBehaviour
- [ ] 不使用Transform、GameObject
- [ ] 不使用Coroutine
- [ ] 不使用Unity生命周期方法
- [ ] 可以在纯C#单元测试中运行
- [ ] 使用ReactiveProperty暴露状态

### Presenter层检查
- [ ] 继承MonoBehaviour
- [ ] 通过[Inject]获取依赖
- [ ] 使用SerializeField引用View组件
- [ ] 订阅Model使用AddTo(this)
- [ ] 不包含业务逻辑

## 快速验证命令

```bash
# 检查Model是否依赖Unity
grep "class.*Model.*:.*MonoBehaviour" Assets/Scripts/Models --include="*.cs" -r

# 检查Model是否使用Unity API
grep -E "(Transform|GameObject|Coroutine|IEnumerator)" Assets/Scripts/Models --include="*.cs" -r

# 检查Model是否使用Unity命名空间
grep "using UnityEngine\." Assets/Scripts/Models --include="*.cs" -r | grep -v "using UnityEngine;"
```

## 重构指南

### 如何将耦合代码重构为MVP

**步骤1：识别数据和逻辑**
```csharp
// 原始耦合代码
public class Player : MonoBehaviour
{
    public int health = 100;  // 数据
    public Transform target;  // Unity依赖

    void Update()
    {
        // 业务逻辑
        if (health <= 0) Die();
    }
}
```

**步骤2：提取Model**
```csharp
// 提取纯数据到Model
public class PlayerModel
{
    public IReadOnlyReactiveProperty<int> Health => _health;
    private ReactiveProperty<int> _health = new ReactiveProperty<int>(100);

    public void TakeDamage(int damage)
    {
        _health.Value -= damage;
        if (_health.Value <= 0)
        {
            _health.Value = 0;
            // 触发死亡事件
        }
    }
}
```

**步骤3：创建Presenter**
```csharp
// Presenter处理Unity
public class PlayerPresenter : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private PlayerModel _player;

    [Inject]
    public void Construct(PlayerModel player)
    {
        _player = player;
    }

    private void Start()
    {
        _player.Health
            .Subscribe(OnHealthChanged)
            .AddTo(this);
    }

    private void OnHealthChanged(int health)
    {
        if (health <= 0)
        {
            _animator.SetTrigger("Die");
        }
    }
}
```

## 常见问题

### Q: Model需要播放动画怎么办？
**A**: Model不播放动画，Model触发状态变化，Presenter监听状态变化播放动画。

```csharp
// Model
public class Card
{
    public IReadOnlyReactiveProperty<bool> IsFlipping => _isFlipping;
    private ReactiveProperty<bool> _isFlipping = new ReactiveProperty<bool>(false);

    public void StartFlip()
    {
        _isFlipping.Value = true;
    }

    public void EndFlip()
    {
        _isFlipping.Value = false;
    }
}

// Presenter
_card.IsFlipping
    .Where(isFlipping => isFlipping)
    .Subscribe(_ => PlayFlipAnimation())
    .AddTo(this);
```

### Q: Model需要位置信息怎么办？
**A**: Model不需要Transform，Model需要的是逻辑位置（如索引、坐标），Presenter将逻辑位置映射到Unity Transform。

```csharp
// Model - 逻辑位置
public class Card
{
    public IReadOnlyReactiveProperty<CardPosition> Position => _position;
    private ReactiveProperty<CardPosition> _position;
}

// Presenter - 物理位置
_card.Position
    .Subscribe(pos => transform.position = GetWorldPosition(pos))
    .AddTo(this);
```

### Q: Model需要延迟执行怎么办？
**A**: Model不用协程，使用UniRx的延迟操作。

```csharp
// Model
public class GameState
{
    public void DelayedAction()
    {
        Observable.Timer(TimeSpan.FromSeconds(1))
            .Subscribe(_ => DoAction());
    }
}
```

## 总结

**记住这个公式**：
```
Model = 纯C# + ReactiveProperty
Presenter = MonoBehaviour + [Inject] + Subscribe
View = SerializeField
```

**违反MVP分离的后果**：
- 无法单元测试
- 代码耦合严重
- 难以维护
- 架构混乱
- 团队协作困难

**坚守MVP分离的好处**：
- 代码清晰易懂
- 易于测试
- 易于重用
- 架构稳固
- 团队协作高效
