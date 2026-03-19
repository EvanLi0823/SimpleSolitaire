# WordSolitaire 场景节点说明

> **版本**: v1.0
> **创建日期**: 2026-03-19
> **基于场景**: WordSolitaireScene.unity
> **文档目的**: 详细说明 WordSolitaire 场景的各个节点功能、布局和组件配置

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
- `WordTopMenuUI.cs`: 管理顶部UI显示和按钮交互
- 事件订阅: `GameEventBus.OnCoinsChanged`、`OnComboReward`、`OnLevelChanged`

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
- `WordBottomMenuUI.cs`: 管理底部按钮交互和道具数量显示
- 事件订阅: `GameEventBus.OnHintCountChanged`、`OnUndoCountChanged`、`OnJokerCountChanged`

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

| 管理器 | 主要职责 | 关键方法 |
|--------|----------|----------|
| **WordSolitaireGameManager** | 游戏流程总控，协调各组件 | `InitializeGame()`、`InitGameState()`、`HasWinGame()` |
| **WordSolitaireCardLogic** | 卡牌游戏逻辑，规则验证 | `AcceptCard()`、`OnDragEnd()`、`CheckWinGame()` |
| **CoinManager** | 金币管理，收支记录 | `AddCoins()`、`SpendCoins()`、`OnCoinsChanged`事件 |
| **ItemManager** | 道具库存管理 | `UseItem()`、`AddItem()`、`GetItemCount()` |
| **OrientationManager** | 屏幕方向适配 | `OnOrientationChanged()`、`UpdateLayout()` |

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
| **CategorySlot.prefab** | `Prefabs/WordSolitaire/` | 分类槽（Toolbox） |
| **EmptySlot.prefab** | `Prefabs/WordSolitaire/` | 空槽位（奖杯图标） |
| **WordCard.prefab** | `Prefabs/WordSolitaire/` | 词语卡牌（文字/图片/分类卡/万能卡） |
| **PackDeck.prefab** | `Prefabs/WordSolitaire/` | 牌库（含卡背堆叠效果和恢复库存按钮） |

### 6.2 预制体使用规范
- 场景中的 Top/Bottom 为预制体实例，修改预制体后实例自动更新
- 如需独立修改实例而不影响预制体，右键选择 "Unpack Prefab"
- 所有新预制体需测试竖屏和横屏两种模式

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
- 序列化字段命名：驼峰命名 + 下划线前缀（如 `_cardLogic`）
- 公开属性使用 PascalCase，提供 Inspector 访问接口

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

## 附录：关键节点引用关系

### A.1 GameManager 引用配置
```yaml
WordSolitaireGameManager 组件引用:
  - TopMenuUI: 引用 Screen/Top 的 WordTopMenuUI 组件
  - BottomMenuUI: 引用 Screen/Bottom 的 WordBottomMenuUI 组件
  - CoinManager: 引用 Managers/CoinManager 组件
  - LevelDataManager: 引用 Managers/LevelDataManager 组件
  - WordDataManager: 引用 Managers/WordDataManager 组件
  - CardLogicComponent: 引用 Center/WordSolitaireCardLogic 组件
  - HintManagerComponent: 引用 Managers/WordSolitaireHintManager 组件
  - UndoPerformerComponent: 引用 Managers/WordSolitaireUndoPerformer 组件
```

### A.2 CardLogic 引用配置
```yaml
WordSolitaireCardLogic 组件引用:
  - GameManagerComponent: 引用 Managers/WordSolitaireGameManager 组件
  - CategoryDecksContainer: 引用 Center/CategoryDecksContainer 节点
  - HandDeck: 引用 Center/HandDeck 节点
  - PackDeck: 引用 Center/PackDeck 节点
  - ColumnDecksContainer: 引用 Center/ColumnDecksContainer 节点
```

---

*文档结束 - WordSolitaire 场景节点说明已完*

> **更新记录**:
> - 2026-03-19: 创建场景节点说明文档，基于 WordSolitaireScene.unity 实时结构
> - 基于产品功能文档 `词语联想接龙-产品功能文档.md` 验证功能完整性
> - 参考架构文档 `GameScene架构与游戏流程分析.md` 确保技术一致性