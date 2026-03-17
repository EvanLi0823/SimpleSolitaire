# GameScene 架构与游戏流程详细分析

> 文档对象：`GameScene_Klondike(UndoCountable).unity`
> 游戏模式：Klondike（经典接龙纸牌）
> 生成日期：2026-03-16

---

## 一、场景总体节点结构

```
GameScene_Klondike(UndoCountable)
├── Main Camera           # 主摄像机
├── Canvas                # UI根节点（CanvasScaler + GraphicRaycaster）
│   ├── GameBG            # 游戏背景图（Button可点击切换背景）
│   ├── Game_BG_Shadow    # 背景阴影层
│   ├── Screen            # 游戏主屏幕区（上/中/下三段布局）
│   │   ├── Top           # 顶部区域（计分/计时 UI）
│   │   ├── Center        # 中心区域（牌桌 + 牌堆）
│   │   └── Bottom        # 底部区域（操作按钮菜单）
│   ├── KlondikeGameLayer # 游戏选项弹窗（随机/重玩/关闭）[默认隐藏]
│   ├── VisualizeSettings # 视觉设置弹窗（音效/方向/背景皮肤）[默认隐藏]
│   ├── StatisticsLayer   # 统计数据弹窗 [默认隐藏]
│   ├── AdsLayer          # 广告确认弹窗 [默认隐藏]
│   ├── ExitLayer         # 退出确认弹窗 [默认隐藏]
│   ├── WinLayer          # 胜利弹窗 [默认隐藏]
│   ├── ContinueGameLayer # 继续上局游戏弹窗 [默认隐藏]
│   └── HowToPlayLayer    # 游戏教程弹窗 [默认隐藏]
├── EventSystem           # Unity事件系统（StandaloneInputModule）
└── Managers              # 所有管理器的父节点
    ├── GameManager       # 游戏总控（KlondikeGameManager）
    ├── AdsManager        # 广告管理器
    ├── CongratulationManager  # 胜利祝贺文字管理器
    ├── CardShirtManager  # 牌背皮肤管理器
    ├── StatisticsManager # 统计数据控制器（KlondikeStatisticsController）
    ├── HintManager       # 提示管理器（KlondikeHintManager）
    ├── AutoCompleteManager    # 自动完成管理器
    ├── UndoPerformer     # 撤销执行器（KlondikeUndoPerformer）
    ├── SoundManager      # 音效控制器（AudioController单例）
    ├── HowToPlayManager  # 教程管理器（KlondikeHowToPlayManager）
    ├── OrientationManager     # 屏幕方向管理器
    ├── DeckSizeManager   # 牌堆尺寸管理器
    └── BottomMenuManager # 底部菜单按钮管理器
```

---

## 二、各节点功能说明

### 2.1 Main Camera
- 标准Unity 2D正交摄像机
- 挂载 `FlareLayer` 和 `AudioListener`
- 渲染整个UI Canvas

### 2.2 Canvas（UI根）
- `CanvasScaler`：处理不同分辨率适配
- `GraphicRaycaster`：处理UI点击射线检测
- 所有游戏UI元素的父节点

#### Screen / Top
顶部状态栏，显示：
- 计时器（`_timeLabel`，格式 `mm:ss`）
- 步数（`_stepsLabel`）
- 分数（`_scoreLabel`）

#### Screen / Center
核心牌桌区域，包含：
- **4个 Ace 基础牌堆**（`AceDeckArray`）：目标收牌区，按花色从 A 叠到 K
- **7个 Bottom 底部牌堆**（`BottomDeckArray`）：主要游戏区域
- **1个 Pack 牌包**（`PackDeck`）：翻牌来源
- **1个 Waste 弃牌堆**（`WasteDeck`）：翻出的牌显示区
- **52张 Card 纸牌**（`KlondikeCard` 组件）：可视化牌对象

#### Screen / Bottom
底部操作按钮区（由 `BottomMenuManager` 管理）：
- 撤销按钮（触发 `UndoPerformer.Undo()`）
- 提示按钮（触发 `HintManager.HintButtonAction()`）
- 自动完成按钮（触发 `AutoCompleteManager.CompleteGame()`）
- 设置按钮（触发 `GameManager.OnClickSettingBtn()`）
- 退出按钮（触发 `GameManager.OnClickExitBtn()`）

### 2.3 弹窗层（Layers）
所有弹窗层初始均**隐藏（SetActive=false）**，通过 `Animator` 的 `Appear`/`Disappear` 触发器播放进出场动画：

| 层级名 | 功能 | 触发方式 |
|--------|------|---------|
| `KlondikeGameLayer` | 新游戏选项（随机/重玩/关闭） | 设置按钮 |
| `VisualizeSettings` | 视觉/音效设置 | 设置按钮 |
| `StatisticsLayer` | 游戏数据统计 | 统计按钮 |
| `AdsLayer` | 广告观看确认 | 广告/撤销次数按钮 |
| `ExitLayer` | 退出确认 | 退出按钮 |
| `WinLayer` | 胜利结算界面 | 游戏胜利时自动弹出 |
| `ContinueGameLayer` | 继续上局确认 | 游戏启动时检测存档 |
| `HowToPlayLayer` | 游戏教程说明 | 规则按钮 |

### 2.4 Managers（管理器组）

#### KlondikeGameManager（游戏总控）
继承自抽象类 `GameManager`，是整个游戏的核心协调者：
- **生命周期**：`Awake()` 调用 `InitializeGame()` 初始化所有组件引用和事件订阅；`Start()` 调用 `InitGameState()` 决定是否显示继续游戏弹窗
- **计时器**：首次操作后启动协程 `GameTimer()`，每秒 `+1`，每30秒扣分 `-30`
- **UI协调**：统一管理所有弹窗的显示/隐藏，确保同时只有一个弹窗可见

#### KlondikeStatisticsController（统计控制器）
- 使用 `PlayerPrefs` 持久化存储游戏数据
- 追踪：游戏场数、胜利场数、总步数、最佳时间、最佳步数、平均游戏时长、总分

#### KlondikeHintManager（提示管理器）
- 维护可移动牌列表 `IsAvailableForMoveCardArray`
- 生成提示动画：将提示牌从当前位置平滑移动到目标位置，再弹回
- 支持三种提示来源：手动点击提示按钮、点击单张牌自动提示、自动完成驱动提示

#### KlondikeUndoPerformer（撤销执行器）
- 记录每次操作前所有牌堆状态快照（`UndoStates`）
- 支持可计数模式（`IsCountable=true`）：撤销有限次数，耗尽需看广告
- 存档格式：JSON序列化 `KlondikeUndoData`，存入 `PlayerPrefs`

#### AutoCompleteManager（自动完成管理器）
- 两种模式：`FullGameSession`（随时可用）、`OnlyWhenAllDecksClear`（仅底部牌堆全翻开时可用）
- 工作流程：循环调用 `HintManager.HintAndSet()` 直到无可移动牌
- 支持键盘快捷键 `A` 触发（调试用）

#### OrientationManager（方向管理器）
- 监听设备旋转事件，支持横/竖屏切换
- 支持左/右手操作模式（按钮布局镜像）
- 方向变化后通知所有 `OrientationObject` 组件更新布局

#### DeckSizeManager（牌堆尺寸管理器）
- 根据屏幕方向和分辨率动态调整牌的物理尺寸
- 为 `CardLogic` 提供 `DeckWidth`、`DeckHeight` 数值

---

## 三、牌的数据模型

### 3.1 Card 数据字段

```csharp
public int CardType;    // 花色: 0=黑桃, 1=红心, 2=梅花, 3=方块
public int CardNumber;  // 全局唯一编号 0~51（52张牌的索引）
public int Number;      // 牌面数字 1~13（A=1, J=11, Q=12, K=13）
public int CardStatus;  // 0=背面朝上（不可见）, 1=正面朝上（可见）
public int CardColor;   // 0=黑色（黑桃/梅花）, 1=红色（红心/方块）
public bool IsDraggable;// 当前是否可被拖拽
public Deck Deck;       // 当前所属牌堆
public int IndexZ;      // 拖拽开始时记录的 SiblingIndex（用于放回）
```

### 3.2 KlondikeCard 的编号规则（`InitWithNumber`）

```
CardNumber = 0~51
CardType   = CardNumber / 13      → 确定花色（0黑桃,1红心,2梅花,3方块）
Number     = (CardNumber % 13)+1  → 确定点数（1~13）
CardColor  = (CardType==1 || CardType==3) ? 1 : 0   → 红色或黑色
```

示例：
- CardNumber=0  → 黑桃A（Type=0, Number=1, Color=黑）
- CardNumber=13 → 红心A（Type=1, Number=1, Color=红）
- CardNumber=51 → 方块K（Type=3, Number=13, Color=红）

### 3.3 图片资源路径

```csharp
// 背面（CardStatus == 0）
"Sprites/Cards/Backs/{CardBack}"

// 正面（CardStatus == 1）
"Sprites/Cards/{CardFront}/{花色名}{数字}"
// 例：Sprites/Cards/Classic/spade1   → 黑桃A
//     Sprites/Cards/Classic/heart13  → 红心K
```

---

## 四、牌堆（Deck）系统

### 4.1 牌堆类型（DeckType）

| 类型 | 枚举值 | 说明 |
|------|--------|------|
| `DECK_TYPE_PACK` | Pack | 待翻牌包（点击翻牌到废牌堆） |
| `DECK_TYPE_WASTE` | Waste | 废牌堆（翻出的牌显示区） |
| `DECK_TYPE_BOTTOM` | Bottom | 底部7列主游戏牌堆 |
| `DECK_TYPE_ACE` | Ace | 4个基础收牌区（目标区） |

### 4.2 AllDeckArray 索引分配

`CardLogic.AllDeckArray`（长度13）的分配顺序：
- 索引 0~3：4个 `AceDeckArray`（Ace牌堆）
- 索引 4~10：7个 `BottomDeckArray`（底部牌堆）
- 索引 11：`WasteDeck`（废牌堆）
- 索引 12：`PackDeck`（牌包）

### 4.3 牌堆的核心操作

```csharp
// 入栈：将牌加入牌堆
void PushCard(Card card, bool isDraggable, int cardStatus)

// 出栈：取出顶部一张牌
Card Pop()

// 批量出栈：从指定牌开始取出该牌及其后所有牌（保持顺序）
Card[] PopFromCard(Card card)

// 碰撞检测：判断拖拽中的牌是否与本牌堆重叠
bool OverlapWithCard(Card card)

// 规则检测：判断牌是否可以放入本牌堆
bool AcceptCard(Card card)

// 更新位置：重新计算并设置所有牌的屏幕位置
void UpdateCardsPosition(bool firstTime)
```

---

## 五、游戏流程

### 5.1 初始化流程

```
应用启动
│
├── Awake()
│   └── GameManager.InitializeGame()
│       ├── 设置 targetFrameRate = 300
│       ├── UndoPerformer.Initialize()
│       ├── 订阅广告事件 AdsManager.RewardAction
│       └── CardLogic.SubscribeEvents()（监听规则切换Toggle）
│
└── Start()
    └── GameManager.InitGameState()
        ├── [有存档 && 首次教程已完成]
        │   └── 显示 ContinueGameLayer 弹窗
        └── [无存档 / 首次游戏]
            ├── CardLogic.InitCardLogic()   ← 初始化节点和牌堆数组
            ├── CardLogic.Shuffle(false)    ← 洗牌并发牌
            └── InitMenuView(false)         ← 重置计时/步数/分数显示
```

### 5.2 洗牌与发牌流程（`Shuffle(bReplay)`）

```
CardLogic.Shuffle(bReplay)
│
├── 1. HintManager.IsHintWasUsed = false
├── 2. GameManager.RestoreInitialState()   ← 重置UI（步数/分数/计时归0）
├── 3. RestoreInitialState()               ← 清空所有牌堆，牌回背面
│
├── 4. 生成随机顺序
│   ├── [bReplay=false] GenerateRandomCardNums()   ← 生成新的随机排列
│   └── [bReplay=true]  保留上局 CardNumberArray   ← 重玩相同顺序
│
├── 5. 遍历52张牌
│   ├── card.InitWithNumber(CardNumberArray[i])    ← 设置花色/点数/颜色
│   └── PackDeck.PushCard(card)                    ← 全部入牌包
│
├── 6. InitDeckCards()  ← Klondike特有发牌逻辑
│   ├── 底部牌堆[0] 发 1 张（Pop from Pack）
│   ├── 底部牌堆[1] 发 2 张
│   ├── ...
│   ├── 底部牌堆[6] 发 7 张（共28张）
│   ├── 每个底部牌堆调用 UpdateCardsPosition(firstTime=true)
│   │   └── 最后一张翻为正面（CardStatus=1），其余背面（CardStatus=0）
│   └── PackDeck（剩余24张）UpdateCardsPosition()
│
├── 7. SetPackDeckBg()   ← 更新牌包背景图（有牌/无牌）
├── 8. HintManager.UpdateAvailableForDragCards()  ← 计算可移动牌
└── 9. IsGameStarted = true
```

### 5.3 牌的位置更新规则（`UpdateCardsPosition`）

#### Pack 牌包
- 所有牌叠放在牌包节点位置（完全重叠）
- 所有牌 `IsDraggable = false`，显示背面

#### Waste 废牌堆
- 1张牌：显示在基础位置
- 2张牌：第2张（顶部）右移一个 `DECK_SPACE_HORIONTAL_WASTE`，可拖拽
- 3+张牌：最后两张错开显示（+1×和+2×水平间距），最顶牌可拖拽

#### Bottom 底部牌堆（垂直叠放）
- 背面牌之间间距：`DECK_SPACE_VERTICAL_BOTTOM_CLOSED`（较小）
- 正面牌之间间距：`DECK_SPACE_VERTICAL_BOTTOM_OPENED`（较大，便于看清牌面）
- 计算公式：每张牌位置 = 前一张牌位置 - `(0, space, 0)`

#### Ace 基础牌堆
- 所有牌叠放在同一位置（完全重叠，只显示顶部一张）

---

## 六、拖拽移牌流程

```
用户按下拖拽 → OnBeginDrag()
│
├── 检查前置条件（自动完成未激活 && 提示未进行 && IsDraggable=true）
├── 记录当前 SiblingIndex（IndexZ）
├── deck.SetCardsToTop(this)        ← 将本牌及其后所有牌置顶（渲染顺序最前）
└── 启动粒子特效协程（0.1秒后触发）

用户移动鼠标 → OnDrag()
│
├── 使用 RectTransformUtility 将屏幕坐标转为世界坐标
├── 计算位移 offset = newPos - lastPos
├── transform.position += offset     ← 移动本牌
└── deck.SetPositionFromCard(this, x, y)  ← 同步移动本牌后的所有牌

用户松开 → OnEndDrag()
│
├── 还原 SiblingIndex（放回原渲染层级）
├── 停止粒子特效
│
└── CardLogic.OnDragEnd(card)         ← 核心放牌逻辑
    │
    ├── 遍历所有牌堆（AllDeckArray）
    │   ├── 跳过 Pack 和 Waste 类型牌堆
    │   └── [OverlapWithCard 碰撞检测通过]
    │       └── [AcceptCard 规则检测通过]
    │           ├── WriteUndoState()           ← 记录撤销快照
    │           ├── srcDeck.PopFromCard(card)  ← 从来源牌堆取出牌组
    │           ├── targetDeck.PushCardArray() ← 加入目标牌堆
    │           ├── 更新两个牌堆位置
    │           ├── ActionAfterEachStep()       ← 更新步数/分数/计时/胜利检测
    │           └── 播放音效（移到Ace区=MoveToAce音效，其他=Move音效）
    │
    └── [无目标 / 规则不符] 播放错误音效（仅在有合法来源时）

└── deck.UpdateCardsPosition(false)   ← 还原本牌堆所有牌到正确位置
```

### 6.1 碰撞检测（`OverlapWithCard`）

使用矩形包围盒（AABB）重叠检测：
- 两个矩形（拖拽牌 vs 目标牌堆顶部牌）各向内收缩 5%（X轴）和 2.5%（Y轴）
- 完全重叠时返回 true

### 6.2 Klondike 放牌规则（`KlondikeDeck.AcceptCard`）

**底部牌堆（BOTTOM）：**
- 目标非空：牌颜色不同 && 点数 = 目标顶牌点数 - 1
- 目标为空：只接受 K（Number=13）

**Ace基础牌堆（ACE）：**
- 只接受顶部单张牌（不能拖拽牌堆）
- 目标非空：相同花色 && 点数 = 目标顶牌点数 + 1
- 目标为空：只接受 A（Number=1）

---

## 七、点击交互流程

### 7.1 点击牌包（Pack）

```
OnPointerClick → OnTapToPack → Deck.OnPointerClick → CardLogic.OnClickPack()
│
├── [ONE_RULE 单抽模式]
│   ├── 牌包有牌：Pack.Pop() → WasteDeck.PushCard()（翻一张）
│   └── 牌包无牌：WasteDeck 所有牌移回 PackDeck（重置）
│
└── [THREE_RULE 三抽模式]
    ├── 循环3次：Pack.Pop() → WasteDeck.PushCard()
    └── 牌包无牌时：先将 Waste 移回 Pack 再翻

└── ActionAfterEachStep()
```

### 7.2 点击普通牌（KlondikeCard）

```
OnPointerClick → OnTapToPlace → HintManager.HintAndSetByClick(card)
│
└── 触发提示动画：
    ├── 若该牌有可移动提示（Hints列表中存在）
    ├── 播放"牌移向目标位置"动画（持续 TapToPlaceTranslateTime=0.2秒）
    └── 动画结束后自动执行移牌操作（如同拖拽释放一样）
```

---

## 八、胜利检测

```
每次操作后 ActionAfterEachStep() 调用 CheckWinGame()：
│
└── 检查4个 AceDeckArray 是否每个都有13张牌
    └── [全部满足] → GameManager.HasWinGame()
        ├── 隐藏牌桌（_cardLayer.SetActive(false)）
        ├── 显示胜利弹窗（_winLayer）
        ├── 停止计时器
        ├── 计算最终分数：score + 600000 / time
        ├── 保存最佳分数到 PlayerPrefs
        ├── 播放胜利音效
        ├── 更新统计数据（胜局数/时间/分数/步数）
        └── 显示插屏广告
```

---

## 九、撤销系统

### 9.1 状态快照结构

```
UndoData（持久化根数据）
├── States: List<UndoStates>        ← 操作历史栈
│   └── UndoStates
│       ├── IsTemp: bool            ← 是否临时快照（应用切换时保存）
│       └── DecksRecord: List<DeckRecord>
│           └── DeckRecord
│               ├── DeckNum: int    ← 牌堆编号
│               └── CardsRecord: List<CardRecord>
│                   └── CardRecord
│                       ├── CardType, CardNumber, Number
│                       ├── CardStatus, CardColor, IsDraggable
│                       ├── IndexZ, SiblingIndex, DeckNum
│                       └── Pos: CardPosition（localPosition）
├── CardsNums: int[]               ← 本局牌序（用于重玩）
├── Score/Steps/Time: int          ← 当前游戏进度
└── Rule: DeckRule                  ← Klondike特有：当前抽牌规则
```

### 9.2 撤销执行流程

```
UndoPerformer.Undo()
│
├── 清空所有牌堆（AllDeckArray[i].Clear()）
├── 将52张牌全部移入 PackDeck（临时存储池）
│
└── UndoProcess()：按最后一个快照恢复
    ├── 遍历快照中的每个 DeckRecord
    ├── 从 PackDeck 取出牌（Pop），放入对应牌堆
    ├── 恢复牌的所有属性（CardType/Number/Status/Color/IsDraggable等）
    ├── 恢复牌的位置（localPosition）和渲染层级（SiblingIndex）
    └── 调用 deck.UpdateCardsPosition(false)

└── 删除最后一个快照，更新撤销按钮状态
```

### 9.3 存档保存时机

| 时机 | 说明 |
|------|------|
| 应用失去焦点 `OnApplicationFocus(false)` | 保存临时快照（`isTempState=true`） |
| 应用暂停 `OnApplicationPause(true)` | 保存临时快照 |
| 每次有效移牌操作前 `WriteUndoState()` | 保存正式撤销快照 |
| 编辑器退出播放模式 | 保存并退出 |

应用恢复时会调用 `Undo(removeOnlyState: true)` 删除临时快照（不恢复状态）。

---

## 十、分数计算规则

| 事件 | 分数变化 |
|------|---------|
| 每30秒计时扣分 | -30 |
| 牌移到 Ace 基础区 | +10 |
| 胜利最终分 | 当前分 + 600000 / 用时秒数 |

---

## 十一、精灵加载与缓存

```csharp
// CardLogic.LoadSprite()
public Sprite LoadSprite(string path)
{
    // 先查缓存字典 CachedSprtesDict
    // 未命中则 Resources.Load<Sprite>(path) 并缓存
}
```

所有精灵通过 Unity 的 `Resources.Load` 加载，路径：
- 牌背：`Sprites/Cards/Backs/{CardBack}`
- 牌面：`Sprites/Cards/{CardFront}/{花色}{点数}`
- 牌堆背景：`Sprites/Decks/{name}`

---

## 十二、屏幕方向与自适应

`OrientationManager` 监测设备方向变化，驱动以下适配：
1. **布局间距**：`DeckSpacesContainer` 存储横/竖屏两套间距数据
2. **牌堆尺寸**：`DeckSizeManager` 提供对应屏幕方向的牌尺寸
3. **UI缩放**：`OrientationLayerScaler` 在方向变化时缩放牌桌层
4. **组件激活**：`OrientationComponentActivator` 激活/隐藏对应方向的UI元素
5. **RectTransform**：`OrientationObjectRectSetter` 更新各元素的锚点和尺寸

---

## 十三、关键类继承关系总览

```
MonoBehaviour
├── GameManager (abstract)
│   └── KlondikeGameManager
├── CardLogic (abstract)
│   └── KlondikeCardLogic
├── Card (abstract) + IBeginDragHandler + IDragHandler + IEndDragHandler + IPointerClickHandler
│   └── KlondikeCard
├── Deck (abstract) + IPointerClickHandler
│   └── KlondikeDeck
├── UndoPerformer (abstract)
│   └── KlondikeUndoPerformer
├── HintManager (abstract)
│   └── KlondikeHintManager
├── StatisticsController (abstract)
│   └── KlondikeStatisticsController
├── AutoCompleteManager
├── CongratulationManager
├── AudioController（单例）
├── CardShirtManager
├── OrientationManager
├── DeckSizeManager
└── BottomMenuManager
```

---

## 十四、其他游戏模式对比

项目同时包含5种纸牌变体，均遵循相同的继承架构：

| 变体 | 牌数 | 特有机制 |
|------|------|---------|
| **Klondike** | 52 | 单抽/三抽规则，Pack→Waste翻牌 |
| **Spider** | 104 | 多套牌，同花色完整序列自动移除 |
| **Freecell** | 52 | FreecellAmount个自由格，无Pack/Waste |
| **Pyramid** | 52 | 两张牌点数之和=13可消除 |
| **Tripeaks** | 52 | 自定义布局，层叠金字塔结构 |

所有变体共享：`GameManager`、`CardLogic`、`Card`、`Deck`、`UndoPerformer`、`HintManager`、`StatisticsController` 基类。
