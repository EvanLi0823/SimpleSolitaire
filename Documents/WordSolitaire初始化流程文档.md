# WordSolitaire 初始化流程文档

## 完整的初始化流程

```
Unity 启动
    ↓
WordSolitaireGameManager.Awake()
    ↓
【步骤 1】WordSolitaireGameManager.InitializeGame() (重写基类)
    ↓
    ├─ base.InitializeGame() [调用基类初始化]
    │   ↓
    │   └─ _undoPerformComponent.Initialize()
    │   └─ _adsManagerComponent.RewardAction += OnRewardActionState
    │   └─ _cardLogic.SubscribeEvents()
    │   └─ AudioController.Instance
    │
    ├─ 【重要】LoadCurrentLevel() [最先加载关卡数据]
    │   ↓
    │   ├─ 从 _levelDataManager.GetCurrentLevel() 获取当前关卡
    │   ├─ 设置 _currentLevel
    │   ├─ 设置 _remainingSteps = _currentLevel.MaxMoves
    │   └─ 输出日志：关卡加载完成
    │
    ├─ wordDataManager?.PreloadAllCategories() [预加载词库]
    │
    └─ _coinManager.Initialize() [初始化金币管理器]
    ↓
【步骤 2】WordSolitaireGameManager.Start()
    ↓
【步骤 3】GameManager.InitGameState() (基类)
    ↓
    ├─ InitSettingBtns()
    │
    ├─ 检查必要组件（_cardLogic, _howToPlayComponent, _undoPerformComponent）
    │
    ├─ 检查是否显示继续游戏弹窗
    │   ↓
    │   ├─ 如果有存档 → ShowContinueLayer()
    │   │   ↓
    │   │   └─ 等待用户选择（继续/不继续）
    │   │
    │   └─ 如果无存档 → 直接开始新游戏
    │       ↓
    │       └─ _cardLogic.InitCardLogic()  ← 调用 WordSolitaireCardLogic.InitCardLogic()
    │
    └─ InitMenuView(false)
    ↓
【步骤 4】WordSolitaireCardLogic.InitCardLogic() (重写基类)
    ↓
    ├─ GenerateDecks() [生成分类槽和列区]
    │   ↓
    │   ├─ ClearExistingDecks() [清理现有Deck]
    │   ├─ GenerateCategorySlots() [生成分类槽]
    │   │   ↓
    │   │   ├─ 从 CategoryIds 获取分类列表
    │   │   ├─ 遍历分类，为每个分类创建 CategorySlot
    │   │   ├─ 从 DeckPrefab 实例化 Deck
    │   │   ├─ 配置 Deck 属性（DeckType, CategoryId, TargetCardCount）
    │   │   ├─ 注册到 OrientationManager（支持动态方向适配）
    │   │   └─ 添加到 CategorySlots 数组
    │   │
    │   └─ GenerateColumnDecks() [生成列区]
    │       ↓
    │       ├─ 从 ColumnCount 获取列数
    │       ├─ 遍历列数，为每列创建 ColumnDeck
    │       ├─ 从 DeckPrefab 实例化 Deck
    │       ├─ 配置 Deck 属性
    │       ├─ 注册到 OrientationManager
    │       └─ 添加到 ColumnDecks 数组
    │
    ├─ InitDeckCards() [初始化牌堆数组]
    │   ↓
    │   ├─ AceDeckArray = new Deck[CategorySlots.Length]
    │   │   └─ 遍历 CategorySlots，赋值到 AceDeckArray
    │   │
    │   ├─ BottomDeckArray = new Deck[ColumnDecks.Length]
    │   │   └─ 遍历 ColumnDecks，赋值到 BottomDeckArray
    │   │
    │   └─ WasteDeck = HandDeck
    │
    ├─ base.InitCardLogic() [调用基类 CardLogic.InitCardLogic]
    │   ↓
    │   ├─ InitCardNodes() [初始化卡牌节点]
    │   ├─ InitAllDeckArray() [初始化所有牌堆数组]
    │   │   ↓
    │   │   ├─ 遍历 AceDeckArray，设置 Type = DECK_TYPE_ACE
    │   │   ├─ 遍历 BottomDeckArray，设置 Type = DECK_TYPE_BOTTOM
    │   │   ├─ 设置 WasteDeck.Type = DECK_TYPE_WASTE
    │   │   ├─ 设置 PackDeck.Type = DECK_TYPE_PACK
    │   │   └─ 遍历 AllDeckArray，设置 DeckNum
    │   │
    │   ├─ UndoPerformerComponent.ResetUndoStates()
    │   └─ ParticleStars.Stop()
    │
    └─ InitializeCards() [初始化卡牌]
        ↓
        └─ 根据 _currentLevel.InitialCardsPerColumn 分发卡牌到列区
    ↓
【步骤 5】_cardLogic.Shuffle(false) [洗牌]
    ↓
【步骤 6】InitMenuView(false) [初始化菜单视图]
    ↓
游戏开始
```

## 关键设计要点

### 1. 关卡数据加载时机

**必须在 `InitializeGame()` 中最先加载**，而不是在 `InitCardLogic()` 中加载。

**原因**：
- `InitCardLogic()` 会在游戏过程中多次调用（如重新开始、加载存档等）
- 关卡数据只需要在游戏开始时加载一次
- 所有后续的初始化都依赖于已加载的关卡数据

### 2. 方法的职责分离

#### WordSolitaireGameManager

- **`LoadCurrentLevel()`**：负责加载关卡数据
- **`InitCardLogic()`**：负责初始化卡牌逻辑（GameManager 抽象方法实现）
  - 调用 `WordSolitaireCardLogic.InitializeLevel(_currentLevel)`
  - 发布步数初始化事件

#### WordSolitaireCardLogic

- **`InitCardLogic()`**：负责初始化卡牌逻辑（CardLogic 公共方法实现）
  - 生成分类槽和列区
  - 初始化牌堆数组
  - 调用基类的 `InitCardLogic()`
  - 初始化卡牌

### 3. 方法名冲突处理

| 类 | 方法名 | 作用 |
|---|---|---|
| GameManager | `protected abstract void InitCardLogic()` | 需要子类实现的抽象方法，用于初始化关卡数据 |
| CardLogic | `public virtual void InitCardLogic()` | 初始化卡牌逻辑的公共方法 |

**WordSolitaireGameManager 重写了 GameManager 的 `InitCardLogic()`**
**WordSolitaireCardLogic 重写了 CardLogic 的 `InitCardLogic()`**

两个方法独立工作，没有冲突。

### 4. 防御性编程

在 `WordSolitaireCardLogic` 中添加了防御性检查：

```csharp
// GenerateDecks() 中
if (_currentLevel == null)
{
    Debug.LogWarning("[WordSolitaireCardLogic] _currentLevel 为 null，跳过生成Deck");
    return;
}

// InitDeckCards() 中
if (CategorySlots != null && CategorySlots.Length > 0)
{
    AceDeckArray = new Deck[CategorySlots.Length];
    // ...
}
else
{
    Debug.LogWarning("[WordSolitaireCardLogic] CategorySlots 为 null 或空，创建空的 AceDeckArray");
    AceDeckArray = new Deck[0];
}
```

这样可以防止在异常情况下（如关卡数据未加载）导致崩溃。

## 时序图

```
时间轴：
t0: Unity 启动
t1: WordSolitaireGameManager.InitializeGame()
    └─ LoadCurrentLevel() ← 关卡数据在此加载
t2: WordSolitaireGameManager.Start()
t3: GameManager.InitGameState()
    └─ WordSolitaireGameManager.InitCardLogic()
        └─ WordSolitaireCardLogic.InitializeLevel(_currentLevel)
t4: WordSolitaireCardLogic.InitCardLogic()
    └─ GenerateDecks()
    └─ InitDeckCards()
    └─ CardLogic.InitCardLogic()
        └─ InitAllDeckArray()
t5: _cardLogic.Shuffle(false)
t6: 游戏开始
```

## 常见问题

### Q1: 为什么不在 InitCardLogic() 中加载关卡数据？

A: 因为 InitCardLogic() 会在以下情况被多次调用：
- 新游戏开始
- 重新开始游戏
- 加载存档
- 点击"继续"按钮

如果在每次调用时都加载关卡数据，会导致：
- 不必要的重复加载
- 如果 LevelDataManager 的实现有副作用，可能导致问题

### Q2: 为什么有两个 InitCardLogic() 方法？

A: 这是架构设计的问题：
- GameManager 的 `InitCardLogic()` 是抽象方法，要求子类实现
- CardLogic 的 `InitCardLogic()` 是公共方法，用于初始化卡牌逻辑

两个方法虽然同名，但是独立的方法，作用不同。

### Q3: 如果关卡数据加载失败怎么办？

A: 在 `LoadCurrentLevel()` 中有完整的错误处理：
```csharp
if (_levelDataManager == null)
{
    Debug.LogError("[WordSolitaireGameManager] _levelDataManager 未配置");
    _currentLevel = null;
    _remainingSteps = 999;  // 使用无限步数
    return;
}

_currentLevel = _levelDataManager.GetCurrentLevel();
if (_currentLevel == null)
{
    Debug.LogError("[WordSolitaireGameManager] 无法加载当前关卡数据");
    _remainingSteps = 999;  // 使用无限步数
    return;
}
```

然后在 `GenerateDecks()` 和 `InitDeckCards()` 中有防御性检查：
```csharp
if (_currentLevel == null)
{
    Debug.LogWarning("[WordSolitaireCardLogic] _currentLevel 为 null，跳过生成Deck");
    return;
}
```

### Q4: 如何确保初始化顺序正确？

A: 关键是在 `InitializeGame()` 中最先调用 `LoadCurrentLevel()`：
```csharp
protected override void InitializeGame()
{
    base.InitializeGame();
    
    // 【重要】最先加载关卡数据
    LoadCurrentLevel();  // ← 必须在这里调用
    
    // 其他初始化...
}
```

这样可以确保：
1. 关卡数据在所有其他初始化之前加载
2. `InitCardLogic()` 被调用时，`_currentLevel` 已经存在
3. 所有依赖关卡数据的初始化都能正常工作

## 总结

WordSolitaire 的初始化流程设计遵循了以下原则：

1. **优先级明确**：关卡数据最先加载
2. **职责分离**：每个方法只做一件事
3. **防御性编程**：在关键点添加 null 检查
4. **可维护性**：代码结构清晰，易于理解和修改

这个初始化流程确保了游戏能够正确、安全地启动，并且在各种异常情况下都能优雅降级。
