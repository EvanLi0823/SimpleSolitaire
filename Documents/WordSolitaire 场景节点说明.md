# WordSolitaire 场景节点说明

> **版本**: v2.0
> **创建日期**: 2026-03-19
> **更新日期**: 2026-03-19
> **基于场景**: WordSolitaireScene.unity
> **文档目的**: 详细说明 WordSolitaire 场景的各个节点功能、布局、组件配置和脚本挂载方式
> **更新内容**: 补充完整的脚本挂载说明，基于实际脚本文件验证

---

## 一、场景整体结构

WordSolitaireScene.unity 是一个基于 Unity 的词语接龙游戏场景，采用预制体架构和组件化设计。场景整体遵循 Canvas-Screen 分层模型，适配移动端竖屏操作。

```
GameScene_WordSolitaire
├── Main Camera                    # 主摄像机
├── Canvas                         # UI根容器
│   ├── GameBG                     # 游戏背景图（可点击切换）
│   ├── Game_BG_Shadow            # 背景阴影层
│   ├── Screen                     # 游戏主屏幕区（上/中/下三段布局）
│   │   ├── Center                 # 中心游戏区域（牌桌）
│   │   ├── Top                    # 顶部信息栏预制体
│   │   └── Bottom                 # 底部道具栏预制体
│   └── 弹窗层（Layers）            # 各类功能弹窗（默认隐藏）
├── EventSystem                    # Unity事件系统
└── Managers                       # 管理器父节点
    ├── OrientationManager         # 屏幕方向管理器
    ├── DeckSizeManager           # 牌堆尺寸管理器
    └── [WordSolitaire专属管理器]    # 游戏功能管理器
```

**场景特点**:
- 采用 **预制体架构**，Top/Bottom 为独立预制体，便于复用和维护
- 支持 **屏幕方向切换**（竖屏/横屏），布局自动适配
- 使用 **事件总线（GameEventBus）** 解耦组件通信
- 遵循 **MVP架构模式**，数据与表现分离

---

## 二、Canvas 根节点

### 2.1 Canvas（UI根容器）
- **组件**: `Canvas`、`CanvasScaler`、`GraphicRaycaster`
- **CanvasScaler配置**:
  - UI Scale Mode: `Scale With Screen Size`
  - Reference Resolution: `720 x 1280`（竖屏设计分辨率）
  - Screen Match Mode: `Match Width or Height` (Match: 0.5)
- **功能**: 处理所有UI元素的渲染和点击事件，提供多分辨率自适应

### 2.2 GameBG（游戏背景）
- **类型**: Button（可点击）
- **功能**: 显示游戏背景图片，点击可切换不同背景主题
- **位置**: 覆盖整个Canvas区域，作为底层视觉元素

### 2.3 Game_BG_Shadow（背景阴影）
- **类型**: Image
- **功能**: 在背景上添加阴影效果，增强视觉层次感
- **透明度**: 半透明黑色，营造景深效果

---

## 三、Screen 主屏幕区

Screen 节点是游戏内容的直接显示区域，采用上中下三段式布局：

```
Screen
├── Top           (0, 0.85) → (1, 1)      # 顶部信息栏
├── Center        (0, 0.15) → (1, 0.85)   # 中心游戏区
└── Bottom        (0, 0) → (1, 0.15)      # 底部道具栏
```

### 3.1 Top（顶部信息栏） - 预制体实例
**预制体路径**: `Prefabs/Composite/Top_WordSolitaire.prefab`

Top 区域显示游戏状态信息，采用水平布局：

```
Top
├── Shadow                          # 底部装饰阴影
└── MenuBG                          # 菜单背景容器（Horizontal Layout Group）
    ├── MenuItemCoins               # 金币显示组
    │   ├── CoinIcon (💰)           # 金币图标
    │   ├── CoinLabel (Text)        # 金币数量显示（如 "600"）
    │   └── CoinAddButton           # 金币增加按钮（点击打开商店）
    ├── MenuItemCombo               # 连击奖励组
    │   ├── ComboIcon (💰+)         # 连击图标
    │   └── ComboLabel (Text)       # 连击奖励显示（如 "+6"）
    ├── MenuItemLevel               # 关卡信息组
    │   ├── LevelLabel (Text)       # 关卡数（如 "166"）
    │   └── LevelText (Text)        # "Level" 文本标签
    └── SettingButton (⚙️)          # 设置按钮（点击打开设置界面）
```

**组件配置**:
- `TopInfoBar.cs`: 管理顶部UI显示和按钮交互
  - **挂载位置**: `Screen/Top` 节点
  - **脚本路径**: `Assets/SimpleSolitaire/Resources/Scripts/Controller/WordSolitaire/UI/Components/TopInfoBar.cs`
  - **Inspector配置**:
    ```csharp
    [Header("UI组件")]
    [SerializeField] private Text _coinsText;      // 拖拽: MenuItemCoins/CoinLabel
    [SerializeField] private Text _levelText;      // 拖拽: MenuItemLevel/LevelLabel
    [SerializeField] private Button _settingsButton; // 拖拽: SettingButton
    
    [Header("图标")]
    [SerializeField] private Image _coinsIcon;     // 拖拽: MenuItemCoins/CoinIcon
    [SerializeField] private Image _levelIcon;     // 拖拽: MenuItemLevel/LevelIcon
    ```
- **事件订阅**: `GameEventBus.OnCoinsChanged`、`OnLevelChanged`
- **自动查找**: 如果Inspector未赋值，脚本会在Awake中自动查找子节点

### 3.2 Center（中心游戏区）
Center 是游戏核心区域，包含卡牌操作的所有功能节点：

```
Center
├── CardLayer                      # 卡牌层容器
│   ├── WordSolitaireCardLogic     # ⭐ 游戏逻辑控制器（组件）
│   │
│   ├── UpperSection               # 上部区域（手牌堆 + 牌库 + 步数显示）
│   │   ├── MovesDisplay           # 步数显示（绿色盾牌形状，左上）
│   │   │   ├── MovesText (Text: "Moves")
│   │   │   └── MovesLabel (Text: "160")  # 剩余步数
│   │   ├── HandDeckGroup          # 手牌堆区域（中间偏上）
│   │   │   └── HandCardsContainer # 手牌卡牌堆叠显示（最多显示3张）
│   │   └── PackDeck               # 牌库（右侧）
│   │       ├── CardBackStack      # 卡背堆叠容器（有牌时显示）
│   │       │   ├── CardBack_0     # 底层卡背图片
│   │       │   ├── CardBack_1     # 中层卡背图片（位置偏移）
│   │       │   └── CardBack_2     # 顶层卡背图片（位置偏移）
│   │       ├── CountLabelBack/CountLabel   # 剩余数量标签（右上角显示，如 "16"）
│   │       └── RestoreButton      # 恢复库存按钮（无牌时显示，文本:"恢复库存"）
│   │
│   ├── MiddleSection              # 中部区域（分类槽 Toolbox）
│   │   ├── EmptySlotsContainer    # 空槽位容器（左侧奖杯图标）
│   │   │   ├── EmptySlot_0 (🏆)
│   │   │   └── EmptySlot_1 (🏆)
│   │   └── CategorySlotsContainer # 分类槽容器（Toolbox）
│   │       ├── CategorySlot_0     # 类别槽1（如 Spanner 1/5）
│   │       │   ├── ProgressLabel (Text: "1/5")
│   │       │   ├── CrownIcon (👑)
│   │       │   └── CategoryName (Text: "Spanner")
│   │       └── CategorySlot_1     # 类别槽2（如 Paper 1/6）
│   │           ├── ProgressLabel (Text: "1/6")
│   │           ├── CrownIcon (👑)
│   │           └── CategoryName (Text: "Paper")
│   │
│   └── LowerSection               # 下部区域（列区）
│       └── ColumnDecksContainer   # 列区容器（4列）
│           ├── ColumnDeck_0       # 列1
│           ├── ColumnDeck_1       # 列2
│           ├── ColumnDeck_2       # 列3
│           └── ColumnDeck_3       # 列4
```

#### 3.2.1 UpperSection（上部区域）详细配置

**位置范围**: Anchors: `(0, 0.75) → (1, 0.95)`（顶部区域）

| 节点 | 位置 | 组件 | 功能说明 |
|------|------|------|----------|
| **MovesDisplay** | Anchors: `(0, 0.8) → (0.2, 0.95)`（左上） | `Image`（绿色盾牌形状）、`Text` ×2、`MovesDisplay.cs` | 显示剩余步数，绿色盾牌形状背景 |
| **HandDeckGroup** | Anchors: `(0.3, 0) → (0.6, 1)`（中间偏左） | `Horizontal Layout Group`（Spacing: -80） | 手牌水平堆叠显示，负间距实现重叠效果 |
| **PackDeck** | Anchors: `(0.75, 0.1) → (0.9, 0.9)`（右侧） | `Button`、`Vertical Layout Group`（Spacing: -15） | 牌库区域，点击翻牌到手牌堆，显示剩余牌数 |

#### 3.2.2 MiddleSection（中部区域 - Toolbox）详细配置

**位置范围**: Anchors: `(0, 0.5) → (1, 0.75)`（中上部）

| 节点 | 位置 | 组件 | 功能说明 |
|------|------|------|----------|
| **EmptySlotsContainer** | Anchors: `(0.05, 0) → (0.25, 1)` | `Horizontal Layout Group`（Spacing: 10） | 空槽位容器，显示奖杯图标（🏆） |
| **CategorySlotsContainer** | Anchors: `(0.3, 0) → (0.9, 1)` | `Horizontal Layout Group`（Spacing: 15） | 分类槽容器，显示已激活的分类槽 |

**分类槽预制体结构**:
```
CategorySlot（预制体）
├── Image（白色背景，黄色边框）
├── Vertical Layout Group（Padding: 10）
│   ├── ProgressLabel（Text: "1/5"）  # 进度显示（顶部）
│   ├── CrownIcon（👑）              # 皇冠图标
│   └── CategoryName（Text: "Spanner"） # 类别名称
```

#### 3.2.3 LowerSection（下部区域 - 列区）详细配置

**位置范围**: Anchors: `(0.05, 0.05) → (0.95, 0.5)`（下部区域）

| 节点 | 位置 | 组件 | 功能说明 |
|------|------|------|----------|
| **ColumnDecksContainer** | Anchors: `(0, 0) → (1, 1)` | `Grid Layout Group`（4列，Cell Size: 140×200） | 4列卡牌区容器，网格布局 |
| **ColumnDeck（预制体）** | - | `Vertical Layout Group`（Spacing: -120）、`DropZone` | 单列卡牌堆叠区域，负间距实现堆叠效果，支持拖拽放置 |

### 3.3 Bottom（底部道具栏） - 预制体实例
**预制体路径**: `Prefabs/Composite/Bottom_WordSolitaire.prefab`

Bottom 区域显示游戏道具和功能按钮：

```
Bottom
├── ToolBar                         # 道具栏容器（Horizontal Layout Group）
│   ├── HintButton                  # 提示按钮
│   │   ├── Icon (💡)               # 提示图标
│   │   └── CountLabel (Text: "6")  # 剩余提示次数
│   ├── UndoButton                  # 撤回按钮
│   │   ├── Icon (↩️)               # 撤回图标
│   │   └── CountLabel (Text: "3")  # 剩余撤回次数
│   └── JokerButton                 # 万能牌按钮
│       ├── Icon (🃏)               # 万能牌图标
│       └── CountLabel (Text: "1")  # 剩余万能牌数量
└── Shadow                          # 顶部装饰阴影
```

**组件配置**:
- `BottomToolBar.cs`: 管理底部按钮交互和道具数量显示
  - **挂载位置**: `Screen/Bottom` 节点
  - **脚本路径**: `Assets/SimpleSolitaire/Resources/Scripts/Controller/WordSolitaire/UI/Components/BottomToolBar.cs`
  - **Inspector配置**:
    ```csharp
    [Header("道具按钮")]
    [SerializeField] private Button _hintButton;   // 拖拽: HintButton
    [SerializeField] private Button _undoButton;   // 拖拽: UndoButton
    [SerializeField] private Button _jokerButton;  // 拖拽: JokerButton
    
    [Header("数量文本")]
    [SerializeField] private Text _hintCountText;  // 拖拽: HintButton/CountLabel
    [SerializeField] private Text _undoCountText;  // 拖拽: UndoButton/CountLabel
    [SerializeField] private Text _jokerCountText; // 拖拽: JokerButton/CountLabel
    
    [Header("图标")]
    [SerializeField] private Image _hintIcon;      // 拖拽: HintButton/Icon
    [SerializeField] private Image _undoIcon;      // 拖拽: UndoButton/Icon
    [SerializeField] private Image _jokerIcon;     // 拖拽: JokerButton/Icon
    ```
- **事件订阅**: 通过GameEventBus订阅道具数量变化（代码中动态订阅）
- **自动查找**: 如果Inspector未赋值，脚本会在Awake中自动查找子节点

---

## 四、弹窗层（Layers）

所有弹窗层初始为隐藏状态（`SetActive(false)`），通过 `UILayerManager` 控制显示/隐藏：

| 弹窗名称 | 预制体路径 | 功能说明 | 触发方式 |
|----------|------------|----------|----------|
| **WinLayer** | `Prefabs/Composite/Layers/WinLayerUI.prefab` | 胜利结算界面 | 游戏胜利时自动弹出 |
| **LoseLayer** | `Prefabs/Composite/Layers/LoseLayerUI.prefab` | 失败提示界面 | 步数耗尽时自动弹出 |
| **ShopLayer** | `Prefabs/Composite/Layers/ShopLayerUI.prefab` | 商店界面 | 点击金币增加按钮或商店按钮 |
| **SettingsLayer** | `Prefabs/Composite/Layers/SettingsLayerUI.prefab` | 设置界面 | 点击设置按钮（⚙️） |
| **ExitLayer** | `Prefabs/Composite/Layers/ExitLayerUI.prefab` | 退出确认 | 点击退出按钮 |
| **HowToPlayLayer** | `Prefabs/Composite/Layers/HowToPlayLayerUI.prefab` | 游戏教程 | 首次游戏或帮助按钮 |

**弹窗管理机制**:
- 使用 `UILayerManager` 单例管理弹窗堆栈
- 支持动画队列（等待上一个弹窗关闭后显示下一个）
- 弹窗继承 `UILayerBase` 基类，统一接口

---

## 五、Managers 管理器组

Managers 节点包含所有游戏功能管理器：

```
Managers
├── WordSolitaireGameManager        # ⭐ 游戏总控制器
├── WordSolitaireCardLogic          # ⭐ 游戏逻辑控制器（同时挂载在Center）
├── WordSolitaireHintManager        # ⭐ 提示管理器
├── WordSolitaireUndoPerformer      # ⭐ 撤销执行器
├── WordSolitaireStatisticsController # ⭐ 统计控制器
├── CoinManager                     # ⭐ 金币管理器
├── ItemManager                     # ⭐ 道具管理器
├── LevelDataManager                # ⭐ 关卡数据管理器
├── WordDataManager                 # ⭐ 词库数据管理器
├── OrientationManager              # 屏幕方向管理器（复用）
└── DeckSizeManager                 # 牌堆尺寸管理器（复用）
```

### 5.1 核心管理器功能说明

| 管理器 | 主要职责 | 关键方法 | 脚本路径 |
|--------|----------|----------|----------|
| **WordSolitaireGameManager** | 游戏流程总控，协调各组件 | `InitializeGame()`、`HasWinGame()` | `Scripts/Controller/WordSolitaire/WordSolitaireGameManager.cs` |
| **WordSolitaireCardLogic** | 卡牌游戏逻辑，规则验证 | `AcceptCard()`、`OnDragEnd()` | `Scripts/Controller/WordSolitaire/WordSolitaireCardLogic.cs` |
| **CoinManager** | 金币管理，收支记录 | `AddCoins()`、`SpendCoins()` | `Scripts/Controller/WordSolitaire/Data/CoinManager.cs` |
| **LevelDataManager** | 关卡数据管理 | `LoadLevel()`、`GetCurrentLevel()` | `Scripts/Controller/WordSolitaire/Data/LevelDataManager.cs` |
| **WordDataManager** | 词库数据管理 | `GetCategoryById()` | `Scripts/Controller/WordSolitaire/Data/WordDataManager.cs` |
| **OrientationManager** | 屏幕方向适配 | `OnOrientationChanged()` | `Scripts/Controller/Orientation/OrientationManager.cs` |
| **DeckSizeManager** | 牌堆尺寸管理 | `CurrentDeckSize` | `Scripts/Controller/DeckSizeManager.cs` |

### 5.2 事件总线（GameEventBus）使用

管理器间通过静态事件总线解耦通信：

```csharp
// Word Solitaire 特有事件
public static event Action<int> OnCoinsChanged;        // 金币变化
public static event Action<int> OnLevelChanged;        // 关卡变化
public static event Action<int> OnHintCountChanged;    // 提示数量变化
public static event Action<int> OnUndoCountChanged;    // 撤回数量变化
public static event Action<int> OnJokerCountChanged;   // 万能牌数量变化
public static event Action<int> OnCategoryMatched;     // 分类匹配成功
public static event Action OnPackEmpty;                // 牌库清空
public static event Action OnWordSolitaireWin;         // 游戏胜利
```

**使用规范**:
- 发布者调用 `Publish*()` 方法（内置 null 检查）
- 订阅者在 `OnEnable` 注册，`OnDisable` 注销，避免内存泄漏

---

## 六、预制体资源说明

### 6.1 核心预制体列表

| 预制体名称 | 路径 | 用途 |
|------------|------|------|
| **Top_WordSolitaire.prefab** | `Prefabs/Composite/` | 顶部信息栏（金币/连击奖励/关卡/设置） |
| **Bottom_WordSolitaire.prefab** | `Prefabs/Composite/` | 底部道具栏（提示/撤回/万能牌） |
| **CategorySlot.prefab** | `Prefabs/WordSolitaire/` | 分类槽（Toolbox） - 详见6.2节 |
| **EmptySlot.prefab** | `Prefabs/WordSolitaire/` | 空槽位（奖杯图标） |
| **WordCard.prefab** | `Prefabs/WordSolitaire/` | 词语卡牌（文字/图片/分类卡/万能卡） - 详见6.3节 |
| **PackDeck.prefab** | `Prefabs/WordSolitaire/` | 牌库（含卡背堆叠效果和恢复库存按钮） |

### 6.2 CategorySlot.prefab 详细结构

**预制体路径**: `Assets/SimpleSolitaire/Resources/Prefabs/WordSolitaire/CategorySlot.prefab`

**用途**: 分类槽（Toolbox），用于显示分类进度并接受匹配的卡牌

**节点结构**:
```
CategorySlot (根节点)
└── Image (组件: Image)  # 白色背景，黄色边框
```

**组件配置**:
- **Image**: 显示白色背景和黄色边框
  - Source Image: `frame_9slice_yellow` (9切片 sprite)
  - Color: 白色 (1, 1, 1, 1)
  - Raycast Target: false (避免遮挡卡牌点击)

**挂载脚本**: `WordSolitaireDeck.cs`
- **DeckType**: `CategorySlot`
- **CategoryId**: 在实例化时动态设置（如"Spanner"、"Paper"）
- **TargetCardCount**: 目标卡牌数量（根据关卡配置）

**使用说明**:
- 该预制体在场景中的 `MiddleSection/CategorySlotsContainer` 下动态实例化
- 每个实例代表一个分类槽，显示该类别的完成进度
- 通过 `Horizontal Layout Group` 自动排列

---

### 6.3 WordCard.prefab 详细结构

**预制体路径**: `Assets/SimpleSolitaire/Resources/Prefabs/WordSolitaire/WordCard.prefab`

**用途**: 词语卡牌，支持三种显示模式（文字卡、图片卡、万能卡）

**节点结构**:
```
WordCard (根节点)  # 挂载 WordSolitaireCard.cs
├── Background     # 卡牌背面（卡背图片）
│   └── Image (组件)
│
└── Front          # 卡牌正面容器（默认隐藏）
    ├── FrontNormal      # 普通卡正面（文字/图片卡使用）
    │   └── Image (组件: 白色背景)
    ├── FrontCategories  # 分类卡正面（显示类别图标和计数）
    │   └── Image (组件: 黄色背景)
    │       └── CategoriesCount (Text)  # 显示 "1/5" 等进度
    ├── FrontJocker      # 万能卡正面（绿色背景）
    │   └── Image (组件)
    ├── Text             # 单词文本（仅文字卡显示）
    │   └── Text (组件)
    └── Icon             # 分类图标（默认隐藏，用于分类卡）
        └── Image (组件)
```

**根节点组件**:
- **RectTransform**: 尺寸 100×100 (在运行时动态调整)
- **WordSolitaireCard.cs**: 核心脚本，控制卡牌显示和交互

**脚本序列化字段配置**:
```csharp
[从基类Card继承]
BackgroundImage:        拖拽 → Background (Image组件)

[Header("UI组件")]
_frontContainer:        拖拽 → Front
_frontNormalImage:      拖拽 → Front/FrontNormal
_frontCategoriesImage:  拖拽 → Front/FrontCategories
_frontJokerImage:       拖拽 → Front/FrontJocker
_wordText:              拖拽 → Front/Text
_wordImage:             拖拽 → FrontNormal（或其他图片节点）
_jokerIcon:             拖拽 → Front/FrontJocker
_categoryIcon:          拖拽 → Front/Icon
_categoryCountText:     拖拽 → Front/FrontCategories/CategoriesCount
```

**自动查找说明**:
- `BackgroundImage` 和上述所有UI字段均支持自动查找
- 查找路径基于节点名称，确保预制体结构完整即可
- 推荐不手动配置，使用自动查找（减少维护成本）

**卡牌类型切换逻辑**:
- **文字卡 (CardType.Text)**: 显示 `FrontNormal` + `Text`
- **图片卡 (CardType.Image)**: 显示 `FrontNormal` + 设置 `_wordImage.sprite`
- **万能卡 (CardType.Joker)**: 显示 `FrontJocker`
- **分类卡**: 显示 `FrontCategories` + `Icon` + `CategoriesCount`

**使用说明**:
- 卡牌通过 `WordSolitaireCardLogic.InitSingleCard()` 或 `InitWithWordItem()` 初始化
- 根据 `CardType` 自动切换显示不同的 Front 子节点
- 拖拽功能由基类 `Card` 实现，支持拖放到各个牌堆

---

### 6.4 预制体使用规范
- 场景中的 Top/Bottom 为预制体实例，修改预制体后实例自动更新
- 如需独立修改实例而不影响预制体，右键选择 "Unpack Prefab"
- 所有新预制体需测试竖屏和横屏两种模式
- **重要**: WordCard 的 Front 子节点默认隐藏，由脚本控制显示

---

## 七、布局参数与自适应

### 7.1 屏幕方向适配
- **方向管理器**: `OrientationManager` 监听设备旋转
- **数据容器**: `OrientationDataContainer` 存储方向特定参数（牌堆位置、大小）
- **方向感知组件**: `OrientationObject` 基类，子类自动响应方向切换

### 7.2 关键布局参数

| 参数 | 竖屏值 | 横屏值 | 说明 |
|------|--------|--------|------|
| **设计分辨率** | 720×1280 | 1280×720 | CanvasScaler 基准分辨率 |
| **牌堆间距（垂直）** | 15-30px | 10-20px | 卡牌堆叠时的垂直间隔 |
| **牌堆间距（水平）** | 20-40px | 30-50px | 分类槽间的水平间隔 |
| **卡牌尺寸** | 100×140 | 120×168 | 根据屏幕方向动态调整 |

### 7.3 自适应规则
1. **Anchor驱动布局**: 所有UI元素使用Anchor定位，而非固定坐标
2. **CanvasScaler缩放**: 根据屏幕宽高比自动缩放UI
3. **Grid Layout Group**: 列区使用网格布局，自动分配空间
4. **Content Size Fitter**: 动态内容区域使用自动尺寸适配

---

## 八、脚本组件架构

### 8.1 核心脚本继承关系

```
MonoBehaviour
├── GameManager (abstract)
│   └── WordSolitaireGameManager
├── CardLogic (abstract)
│   └── WordSolitaireCardLogic
├── Card (abstract) + 拖拽接口
│   └── WordCard
├── Deck (abstract) + 点击接口
│   └── WordDeck
├── UndoPerformer (abstract)
│   └── WordSolitaireUndoPerformer
├── HintManager (abstract)
│   └── WordSolitaireHintManager
├── StatisticsController (abstract)
│   └── WordSolitaireStatisticsController
└── 其他管理器（CoinManager、ItemManager等）
```

### 8.2 命名空间约定
```csharp
namespace SimpleSolitaire.Controller.WordSolitaire
namespace SimpleSolitaire.Model.WordSolitaire.Enum
namespace SimpleSolitaire.Model.WordSolitaire.Config
```

### 8.3 序列化字段规范
- 私有字段使用 `[SerializeField]` 暴露给 Inspector
- 序列化字段命名：驼峰命名 + 下轴线前缀（如 `_cardLogic`）
- 公开属性使用 PascalCase，提供 Inspector 访问接口
- **自动查找机制**: 对于UI组件，脚本使用 `ComponentFinder` 工具类在 `Awake()` 中自动查找子节点，如果Inspector未配置则自动查找，提高预制体易用性并支持缓存

---

## 九、数据资源配置

### 9.1 数据文件路径

| 数据类型 | 资源路径 | 格式 |
|----------|----------|------|
| **词库数据** | `Resources/Data/WordSolitaire/Words/` | ScriptableObject |
| **关卡数据** | `Resources/Data/WordSolitaire/Levels/` | ScriptableObject |
| **分类图标** | `Resources/Sprites/WordSolitaire/CategoryIcons/` | Sprite |
| **词语图片** | `Resources/Sprites/WordSolitaire/WordImages/` | Sprite |
| **卡牌背景** | `Resources/Sprites/WordSolitaire/Cards/` | Sprite |

### 9.2 Excel 数据源
- **Words.xlsx**: 词库表格，包含单词、类别、难度等信息
- **Levels.xlsx**: 关卡配置表格，定义关卡参数和词组分配
- **Categories.xlsx**: 类别定义表格，管理分类元数据

---

## 十、性能优化要点

1. **对象池管理**: 卡牌实例使用对象池，避免频繁 Instantiate/Destroy
2. **资源缓存**: 精灵图片使用字典缓存，避免重复 Resources.Load
3. **事件清理**: 确保 `OnEnable`/`OnDisable` 配对使用，防止内存泄漏
4. **协程管理**: 长时间协程提供取消机制，场景切换时清理
5. **UI 批处理**: 相似材质UI元素集中渲染，减少 Draw Call

---

## 附录 A：脚本挂载详细指南

### A.1 脚本挂载总览表

| 节点路径 | 挂载脚本 | 脚本路径 | 挂载方式 |
|---------|---------|---------|---------|
| `Screen/Top` | `TopInfoBar` | `Scripts/Controller/WordSolitaire/UI/Components/TopInfoBar.cs` | 拖拽到Inspector |
| `Screen/Bottom` | `BottomToolBar` | `Scripts/Controller/WordSolitaire/UI/Components/BottomToolBar.cs` | 拖拽到Inspector |
| `Screen/Center/CardLayer` | `WordSolitaireCardLogic` | `Scripts/Controller/WordSolitaire/WordSolitaireCardLogic.cs` | 拖拽到Inspector |
| `Managers` | `WordSolitaireGameManager` | `Scripts/Controller/WordSolitaire/WordSolitaireGameManager.cs` | 拖拽到Inspector |
| `Managers` | `CoinManager` | `Scripts/Controller/WordSolitaire/Data/CoinManager.cs` | 拖拽到Inspector |
| `Managers` | `LevelDataManager` | `Scripts/Controller/WordSolitaire/Data/LevelDataManager.cs` | 拖拽到Inspector |
| `Managers` | `WordDataManager` | `Scripts/Controller/WordSolitaire/Data/WordDataManager.cs` | 拖拽到Inspector |
| `Managers` | `WordSolitaireHintManager` | `Scripts/Controller/WordSolitaire/WordSolitaireHintManager.cs` | 拖拽到Inspector |
| `Managers` | `WordSolitaireUndoPerformer` | `Scripts/Controller/WordSolitaire/WordSolitaireUndoPerformer.cs` | 拖拽到Inspector |
| `Managers` | `WordSolitaireStatisticsController` | `Scripts/Controller/WordSolitaire/WordSolitaireStatisticsController.cs` | 拖拽到Inspector |
| 所有牌堆节点 | `WordSolitaireDeck` | `Scripts/Controller/WordSolitaire/WordSolitaireDeck.cs` | 拖拽到Inspector |

### A.2 牌堆节点脚本挂载

**所有牌堆节点都必须挂载 `WordSolitaireDeck` 组件**

#### 1. 手牌区 (HandDeck)
- **节点**: `Screen/Center/CardLayer/UpperSection/HandDeck`
- **脚本**: `WordSolitaireDeck.cs`
- **DeckType**: `Hand`
- **Inspector配置**:
  ```csharp
  public WordDeckType DeckType = Hand;  // 在枚举中选择Hand
  public string CategoryId = "";        // 留空（手牌区不需要类别）
  ```

#### 2. 牌库 (PackDeck)
- **节点**: `Screen/Center/CardLayer/UpperSection/PackDeck`
- **脚本**: `WordSolitaireDeck.cs`
- **DeckType**: `Pack`
- **Inspector配置**:
  ```csharp
  public WordDeckType DeckType = Pack;  // 在枚举中选择Pack
  public string CategoryId = "";        // 留空
  ```

#### 3. 分类槽 (CategorySlot)
- **节点**: `Screen/Center/CardLayer/MiddleSection/CategorySlotsContainer/CategorySlot_x`
- **预制体**: `Prefabs/WordSolitaire/CategorySlot.prefab`
- **脚本**: `WordSolitaireDeck.cs`
- **DeckType**: `CategorySlot`
- **Inspector配置**:
  ```csharp
  public WordDeckType DeckType = CategorySlot;  // 在枚举中选择CategorySlot
  public string CategoryId = "xxx";             // 填写具体的类别ID（如"Spanner"）
  public int TargetCardCount = 5;               // 目标卡牌数量（根据关卡配置）
  public int ColumnIndex = 0;                   // 列索引（分类槽留0）
  ```
- **自动生成**: 
  - `CurrentCardCount`: 当前已放置的卡牌数量（只读属性）
  - `IsComplete`: 是否已完成（当前数量 >= 目标数量）

**配置说明**:
- `CategoryId` 必须与 `WordCategoryData` 中的类别ID一致
- `TargetCardCount` 通常从关卡配置中读取，表示该分类需要收集的卡牌数量
- 当 `IsComplete = true` 时，槽位不再接受新卡牌

#### 4. 列区 (ColumnDeck)
- **节点**: `Screen/Center/CardLayer/LowerSection/ColumnDecksContainer/ColumnDeck_x`
- **脚本**: `WordSolitaireDeck.cs`
- **DeckType**: `Column`
- **Inspector配置**:
  ```csharp
  public WordDeckType DeckType = Column;  // 在枚举中选择Column
  public string CategoryId = "";          // 留空（列区不需要类别）
  ```

### A.3 核心脚本引用关系图

```
WordSolitaireGameManager (Managers节点)
├── 直接拖拽引用:
│   ├── _coinManager → CoinManager (Managers节点)
│   ├── _levelDataManager → LevelDataManager (Managers节点)
│   └── _gameConfig → WordSolitaireConfig.asset (Resources/Data/)
│
└── 代码自动查找 (FindObjectOfType):
    ├── CardLogicComponent → WordSolitaireCardLogic (CardLayer节点)
    ├── HintManagerComponent → WordSolitaireHintManager (Managers节点)
    └── UndoPerformerComponent → WordSolitaireUndoPerformer (Managers节点)

WordSolitaireCardLogic (CardLayer节点)
├── 直接拖拽引用:
│   ├── WordDataManager → WordDataManager (Managers节点)
│   ├── LevelDataManager → LevelDataManager (Managers节点)
│   ├── HandDeck → UpperSection/HandDeck (牌堆节点)
│   ├── ColumnDecks → LowerSection/ColumnDecksContainer/* (4个列)
│   ├── CategorySlots → MiddleSection/CategorySlotsContainer/* (多个分类槽)
│   └── WordCardPrefab → Prefabs/WordSolitaire/WordCard.prefab
│
└── 继承自基类 (CardLogic):
    └── PackDeck → UpperSection/PackDeck (基类字段，类型为Deck)

TopInfoBar (Top节点)
├── 直接拖拽引用 (可选):
│   ├── _coinsText → MenuItemCoins/CoinLabel
│   ├── _levelText → MenuItemLevel/LevelLabel
│   └── _settingsButton → SettingButton
│
└── 代码自动查找 (如果Inspector未赋值):
    └── Awake()中自动查找子节点

BottomToolBar (Bottom节点)
├── 直接拖拽引用 (可选):
│   ├── _hintButton → HintButton
│   ├── _undoButton → UndoButton
│   ├── _jokerButton → JokerButton
│   └── 对应的数量文本和图标
│
└── 代码自动查找 (如果Inspector未赋值):
    └── Awake()中自动查找子节点

WordSolitaireCard (WordCard预制体根节点)
├── 直接拖拽引用 (可选):
│   ├── BackgroundImage → Background (从基类Card继承)
│   ├── _frontContainer → Front
│   ├── _frontNormalImage → Front/FrontNormal
│   ├── _frontCategoriesImage → Front/FrontCategories
│   ├── _frontJokerImage → Front/FrontJocker
│   ├── _wordText → Front/Text
│   ├── _wordImage → Front/FrontNormal（或指定图片节点）
│   ├── _jokerIcon → Front/FrontJocker
│   ├── _categoryIcon → Front/Icon
│   └── _categoryCountText → Front/FrontCategories/CategoriesCount
│
└── 代码自动查找 (如果Inspector未赋值):
    └── Awake()中使用ComponentFinder工具类自动查找
        - 使用 `this.Get<Image>("Background")` 查找背景图
        - 使用 `this.Get<Transform>("Front")` 查找Front容器
        - 使用路径查找Front子节点: `this.Get<Image>("Front/FrontNormal")` 等
        - 使用 `this.Get<Text>("Front/FrontCategories/CategoriesCount")` 查找分类计数
        - 自动将_frontNormalImage赋值给_wordImage作为默认值
        - 所有查找结果自动缓存，提高性能
```

### A.4 挂载步骤详解

#### 步骤1: 挂载管理器（Managers节点）
1. 选中 `Managers` 节点
2. 在Inspector中点击 "Add Component"
3. 逐个添加以下脚本:
   - `WordSolitaireGameManager`
   - `CoinManager`
   - `LevelDataManager`
   - `WordDataManager`
   - `WordSolitaireHintManager`
   - `WordSolitaireUndoPerformer`
   - `WordSolitaireStatisticsController`

#### 步骤2: 配置GameManager引用
1. 选中 `Managers/WordSolitaireGameManager`
2. 在Inspector中拖拽配置:
   - `_coinManager` → `Managers/CoinManager`
   - `_levelDataManager` → `Managers/LevelDataManager`
   - `_gameConfig` → 从Project窗口拖拽WordSolitaireConfig.asset

#### 步骤3: 挂载CardLogic
1. 选中 `Screen/Center/CardLayer`
2. 添加组件 `WordSolitaireCardLogic`
3. 在Inspector中拖拽配置:
   - `WordDataManager` → `Managers/WordDataManager`
   - `LevelDataManager` → `Managers/LevelDataManager`
   - `HandDeck` → `UpperSection/HandDeck`
   - `ColumnDecks` → 设置Size=4，逐个拖拽4个列
   - `CategorySlots` → 设置Size=实际数量，逐个拖拽分类槽
   - `WordCardPrefab` → 从Project窗口拖拽WordCard.prefab

#### 步骤4: 挂载UI组件
**Top信息栏**:
1. 选中 `Screen/Top`
2. 添加组件 `TopInfoBar`
3. （可选）拖拽配置文本和按钮引用

**Bottom工具栏**:
1. 选中 `Screen/Bottom`
2. 添加组件 `BottomToolBar`
3. （可选）拖拽配置按钮和文本引用

#### 步骤5: 挂载牌堆Deck组件
1. 对每个牌堆节点（HandDeck、PackDeck、CategorySlots、ColumnDecks）
2. 添加组件 `WordSolitaireDeck`
3. 在Inspector中设置:
   - `DeckType` → 选择对应的枚举值
   - `CategoryId` → 分类槽需要填写具体ID
   - `TargetCardCount` → 分类槽填写目标数量（如5）

#### 步骤6: 配置WordCard预制体（可选）
1. 在Project窗口中找到 `Prefabs/WordSolitaire/WordCard.prefab`
2. 双击打开预制体编辑模式
3. 选中根节点 `WordCard`
4. 确保 `WordSolitaireCard.cs` 组件已挂载
5. （可选）在Inspector中拖拽配置UI引用：
   - 如果希望提高性能或明确引用，可将Background和所有Front子节点拖拽到对应的序列化字段
   - 如果不配置，脚本会在 `Awake()` 中使用ComponentFinder工具类自动查找（推荐方式，减少配置工作量）
6. 保存预制体（Ctrl+S 或 Cmd+S）

**重要提示**: 
- WordCard预制体的UI引用支持**自动查找**，无需手动配置即可正常工作
- 自动查找使用ComponentFinder工具类，支持缓存机制，提高性能
- 自动查找路径基于节点名称（Background、Front等），确保预制体结构不被破坏
- 如需手动配置，请严格按照A.3节的引用关系图进行拖拽

### A.5 验证清单

完成挂载后，请检查以下项目:

- [ ] **Managers节点** 包含所有7个管理器脚本
- [ ] **GameManager** 的3个引用字段已配置（蓝色）
- [ ] **CardLogic** 的数据管理器引用已配置
- [ ] **CardLogic** 的所有牌堆引用已配置（HandDeck、ColumnDecks数组、CategorySlots数组）
- [ ] **CardLogic** 的WordCardPrefab已配置
- [ ] **Top和Bottom** 节点已挂载对应的UI脚本
- [ ] **所有牌堆节点** 已挂载WordSolitaireDeck并配置了DeckType
- [ ] **分类槽** 的CategoryId和TargetCardCount已正确填写
- [ ] **WordCard预制体** 已挂载WordSolitaireCard脚本（UI引用可自动查找，可选配置）

### A.6 常见问题排查

**问题1: NullReferenceException - "xxx is null"**
- **原因**: Inspector引用字段未配置
- **解决**: 检查对应脚本的Inspector，确保所有引用字段已拖拽配置。对于WordCard，支持自动查找（BackgroundImage、Front相关节点），可忽略此错误

**问题2: 牌堆不响应点击**
- **原因**: 牌堆节点未挂载WordSolitaireDeck或DeckType配置错误
- **解决**: 检查牌堆节点的Deck组件和DeckType设置

**问题3: UI不更新显示**
- **原因**: UI脚本的文本/按钮引用未配置
- **解决**: 检查TopInfoBar或BottomToolBar的Inspector，手动拖拽配置引用。WordCard的UI支持自动查找

**问题4: WordCard不显示文字/图片/背景**
- **原因**: Background、Front节点或其子节点名称被修改，导致自动查找失败
- **解决**: 确保预制体结构保持完整，节点名称必须与文档一致（Background、Front、FrontNormal、Text等）

**问题5: 自动查找性能问题**
- **原因**: 频繁调用Find()可能影响性能（ComponentFinder已优化，支持缓存，实际影响极小）
- **解决**: 
  - 无需处理，ComponentFinder会自动缓存查找结果
  - 如需进一步优化，可手动在Inspector中配置所有引用，避免运行时的Find操作

---

## 附录 B：关键节点引用关系

### A.1 GameManager 引用配置
**挂载位置**: `Managers/WordSolitaireGameManager`

**脚本**: `WordSolitaireGameManager.cs`

```yaml
Inspector字段配置:
  [Header("Word Solitaire 组件")]
  - _coinManager:           拖拽 → Managers/CoinManager
  - _levelDataManager:      拖拽 → Managers/LevelDataManager
  
  [Header("游戏配置")]
  - _gameConfig:            拖拽 → Resources/Data/WordSolitaire/WordSolitaireConfig.asset
```

**代码获取**（使用FindObjectOfType）：
- CardLogicComponent:      自动查找 WordSolitaireCardLogic
- HintManagerComponent:    自动查找 WordSolitaireHintManager
- UndoPerformerComponent:  自动查找 WordSolitaireUndoPerformer
- TopInfoBar:              自动查找 TopInfoBar
- BottomToolBar:           自动查找 BottomToolBar

### A.2 CardLogic 引用配置
**挂载位置**: `Screen/Center/CardLayer/WordSolitaireCardLogic`

**脚本**: `WordSolitaireCardLogic.cs`

```yaml
Inspector字段配置:
  [Header("Word Solitaire 组件")]
  - WordDataManager:        拖拽 → Managers/WordDataManager
  - LevelDataManager:       拖拽 → Managers/LevelDataManager
  
  // 牌堆引用（拖拽配置）
  - HandDeck:               拖拽 → UpperSection/HandDeck
  - ColumnDecks:            拖拽 → LowerSection/ColumnDecksContainer下的4个列
                             （Size=4，逐个拖拽ColumnDeck_0~3）
  - CategorySlots:          拖拽 → MiddleSection/CategorySlotsContainer下的分类槽
                             （Size根据关卡配置，逐个拖拽CategorySlot_x）
  
  [Header("预制体")]
  - WordCardPrefab:         拖拽 → Resources/Prefabs/WordSolitaire/WordCard.prefab
```

**注意**: PackDeck继承自基类CardLogic，类型为Deck，已在基类中定义，无需在派生类中重复配置

---

*文档结束 - WordSolitaire 场景节点说明已完*

> **更新记录**:
> - 2026-03-19: 创建场景节点说明文档，基于 WordSolitaireScene.unity 实时结构
> - 基于产品功能文档 `词语联想接龙-产品功能文档.md` 验证功能完整性
> - 参考架构文档 `GameScene架构与游戏流程分析.md` 确保技术一致性