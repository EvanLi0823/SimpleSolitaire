# Word Solitaire 预制体配置说明

## 预制体列表

### 1. WordCard.prefab - 词语卡牌
**路径**: `Assets/SimpleSolitaire/Resources/Prefabs/WordSolitaire/WordCard.prefab`

**结构**:
```
WordCard (GameObject)
├── RectTransform (锚点: 0.5, 0.5, 尺寸: 100x140)
├── CanvasRenderer
├── Image (卡牌背景) - 绑定到WordSolitaireCard.BackgroundImage
├── WordSolitaireCard (脚本组件)
├── WordText (TextMeshProUGUI, 默认显示, 中央) - 绑定到WordSolitaireCard.WordText
├── WordImage (Image, 默认隐藏, 中央) - 绑定到WordSolitaireCard.WordImage
└── JokerIcon (GameObject, 默认隐藏, 中央) - 绑定到WordSolitaireCard.JokerIcon
```

**WordSolitaireCard脚本配置**:
- BackgroundImage: 指向自身Image组件
- CardRect: 指向自身RectTransform
- WordText: 绑定WordText子对象
- WordImage: 绑定WordImage子对象
- JokerIcon: 绑定JokerIcon子对象

---

### 2. CategorySlot.prefab - 分类槽
**路径**: `Assets/SimpleSolitaire/Resources/Prefabs/WordSolitaire/CategorySlot.prefab`

**结构**:
```
CategorySlot (GameObject)
├── RectTransform (锚点: 0.5, 0.5, 尺寸: 110x150)
├── CanvasRenderer
├── Image (槽位背景)
└── WordSolitaireDeck (脚本组件)
    - DeckType = CategorySlot
    - CategoryId = (在运行时设置)
    - TargetCardCount = (在运行时设置)
```

---

### 3. ColumnDeck.prefab - 列区牌堆
**路径**: `Assets/SimpleSolitaire/Resources/Prefabs/WordSolitaire/ColumnDeck.prefab`

**结构**:
```
ColumnDeck (GameObject)
├── RectTransform (锚点: 0.5, 0.5, 尺寸: 100x140)
├── CanvasRenderer
├── Image (空槽位背景, 可选)
└── WordSolitaireDeck (脚本组件)
    - DeckType = Column
    - ColumnIndex = (在运行时设置)
```

---

### 4. PackDeck.prefab - 牌库
**路径**: `Assets/SimpleSolitaire/Resources/Prefabs/WordSolitaire/PackDeck.prefab`

**结构**:
```
PackDeck (GameObject)
├── RectTransform (锚点: 0.5, 0.5, 尺寸: 100x140)
├── CanvasRenderer
├── Image (卡背堆叠效果)
└── WordSolitaireDeck (脚本组件)
    - DeckType = Pack
```

---

### 5. HandDeck.prefab - 手牌区
**路径**: `Assets/SimpleSolitaire/Resources/Prefabs/WordSolitaire/HandDeck.prefab`

**结构**:
```
HandDeck (GameObject)
├── RectTransform (锚点: 0.5, 0.5, 尺寸: 220x140)
├── CanvasRenderer
└── WordSolitaireDeck (脚本组件)
    - DeckType = Hand
```

---

## 核心组件说明

### WordSolitaireCard.cs
- **功能**: 词语卡牌显示和交互
- **关键属性**:
  - WordId: 单词ID
  - CategoryId: 所属类别ID
  - CardType: 卡牌类型 (Text/Image/Joker)
  - WordImageSprite: 图片资源

### WordSolitaireDeck.cs
- **功能**: 词语牌堆管理
- **关键属性**:
  - DeckType: 牌堆类型 (Pack/Hand/Column/CategorySlot)
  - CategoryId: 类别ID（仅CategorySlot使用）
  - TargetCardCount: 目标卡牌数量（仅CategorySlot使用）
- **关键方法**:
  - AcceptCard(): 检查是否接受卡牌
  - UpdateCardsPosition(): 更新卡牌位置

### WordSolitaireCardLogic.cs
- **功能**: 核心游戏逻辑
- **关键属性**:
  - PackDeck: 牌库引用
  - HandDeck: 手牌区引用
  - ColumnDecks: 列区牌堆数组
  - CategorySlots: 分类槽数组
- **关键方法**:
  - InitializeLevel(): 初始化关卡
  - CheckWinCondition(): 检查胜利条件
  - TryAutoMatchCard(): 尝试自动匹配

## 配置步骤

1. 在Unity Editor中创建上述预制体
2. 将脚本组件绑定到对应GameObject
3. 配置锚点(Anchor)和布局
4. 设置默认精灵和图片
5. 保存到指定路径

## 注意事项

- 所有UI元素使用Anchor驱动布局，支持竖屏/横屏适配
- 卡牌尺寸建议使用 100x140 像素比例
- 字体使用项目中已有的Baloo2-Bold
- 颜色配置参考WordSolitaireCard.cs中的默认颜色值

## 场景配置

在WordSolitaireScene.unity中配置：

1. **Managers对象**:
   - 添加WordSolitaireCardLogic组件
   - 绑定所有牌堆引用(PackDeck, HandDeck, ColumnDecks, CategorySlots)
   - 绑定WordCardPrefab

2. **牌堆对象**:
   - 创建PackDeck、HandDeck、ColumnDecks(4个)、CategorySlots(根据关卡)
   - 分别绑定对应的预制体

3. **UI对象**:
   - 配置Top_WordSolitaire.prefab（金币、关卡显示）
   - 配置Bottom_WordSolitaire.prefab（提示、撤回按钮）
