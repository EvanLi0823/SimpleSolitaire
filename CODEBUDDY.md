# CODEBUDDY.md
本文件为 CodeBuddy 提供此代码库的工作指导。

## 构建和测试

### Unity 项目构建
在 Unity Editor 2022.3.62f2c1 中打开项目：
- 打开 File → Build Settings
- 选择目标平台（iOS/Android/PC Standalone）
- 点击 Build 或 Build and Run

### 运行测试
- 各游戏模式场景位于 `Assets/Scenes/` 目录下
- 打开特定场景后，在 Unity Editor 中点击 Play 按钮进行测试
- 使用 `CardLogicTester` 测试纸牌逻辑（无需完整游戏设置）

### 项目配置
- **解决方案文件**: `Solitaire2.sln`
- **主要脚本目录**: `Assets/SimpleSolitaire/Resources/Scripts/Controller/`
- **资源加载路径**: Unity Resources 系统（`Public.PATH_TO_*` 常量定义）

---

## 核心架构概览

本项目是一个 Unity 2D 多变体纸牌游戏，采用**组件化架构**和**继承层次**模式，实现了五个经典纸牌变体：Klondike、Spider、Freecell、Pyramid、Tripeaks。

### 架构层次

```
GameManager (抽象基类)
    ↓ 继承
KlondikeGameManager / SpiderGameManager / FreecellGameManager / ...

Card (抽象基类)
    ↓ 继承
KlondikeCard / SpiderCard / FreecellCard / ...

Deck (抽象基类)
    ↓ 继承
KlondikeDeck / SpiderDeck / FreecellDeck / ...

CardLogic (抽象基类)
    ↓ 继承
KlondikeCardLogic / SpiderCardLogic / FreecellCardLogic / ...
```

### 关键系统说明

#### 1. 基类层（`Base/` 目录）
每个游戏变体必须实现以下基类扩展：

- **GameManager**: 游戏流程总控，管理计时、分数、步数、UI 弹窗、广告、撤销/重做、自动完成
- **Card**: 纸牌对象，实现拖放逻辑（`IBeginDragHandler`, `IDragHandler`, `IEndDragHandler`, `IPointerClickHandler`）
- **Deck**: 牌堆管理，定义 `PushCard`、`Pop`、`UpdateCardsPosition`、`AcceptCard` 等操作
- **CardLogic**: 核心游戏逻辑，管理纸牌初始化、洗牌、移动规则、提示系统
- **HintManager**: 提示系统，高亮显示可移动纸牌
- **UndoPerformer**: 撤销/重做系统，保存游戏状态快照
- **StatisticsController**: 统计数据，使用 `PlayerPrefs` 持久化

#### 2. 事件总线（GameEventBus.cs）
**解耦通信核心**，管理器间不直接引用，通过静态事件通信：
- **HUD 事件**: `OnScoreChanged`, `OnTimeChanged`, `OnStepChanged`, `OnHUDInitialized`
- **设置事件**: `OnSettingsChanged`

使用规范：
- 发布者调用 `Publish*()` 方法（内置 null 检查）
- 订阅者在 `OnEnable` 注册，`OnDisable` 注销，避免内存泄漏

#### 3. UI 层系统（`UI/` 目录）
采用**弹窗堆栈管理**（UILayerManager）和**中介者模式**（GameLayerMediator）：

- **UILayerManager**: 单例，管理弹窗显示/隐藏，支持动画队列（等待上一个弹窗关闭后显示下一个）
- **GameLayerMediator**: 封装弹窗显示逻辑，定义 LayerKey 常量，保持 GameManager 方法签名不变
- **各弹窗 UI**: `WinLayerUI`, `ExitLayerUI`, `SettingLayerUI` 等，继承 `UILayerBase`

#### 4. 屏幕方向系统（`Orientation/` 目录）
处理竖屏/横屏切换和左手/右手操作：
- **OrientationManager**: 管理设备方向变化
- **OrientationDataContainer**: 存储方向特定数据（牌堆位置、大小）
- **OrientationObject**: 方向感知组件基类，子类自动响应方向切换

修改 UI 时务必测试竖屏和横屏两种模式。

#### 5. 广告系统
当前 Google Mobile Ads SDK 集成已禁用（注释掉）：
- `AdsManager.cs` 中 `using GoogleMobileAds.Api;` 已注释
- 如需重新启用：取消注释所有广告相关代码并测试

### 添加新游戏模式

遵循以下步骤：

1. 在 `Assets/SimpleSolitaire/Resources/Scripts/Controller/` 下创建新目录（如 `NewGame/`）
2. 创建继承基类的类（命名模式: `{GameMode}{ClassName}`）：
   - `{GameMode}GameManager`: 继承 `GameManager`
   - `{GameMode}Card`: 继承 `Card`
   - `{GameMode}Deck`: 继承 `Deck`
   - `{GameMode}CardLogic`: 继承 `CardLogic`
   - `{GameMode}UndoPerformer`: 继承 `UndoPerformer`
   - `{GameMode}HintManager`: 继承 `HintManager`
   - `{GameMode}StatisticsController`: 继承 `StatisticsController`
3. 在 `CardLogic` 子类中实现游戏特定规则（`AcceptCard`、移动验证等）
4. 创建 Unity 场景并挂载对应脚本

---

## 开发规范

### 命名空间约定
所有代码使用 `SimpleSolitaire` 及其子命名空间：
- `SimpleSolitaire.Controller`: 主要控制器类
- `SimpleSolitaire.Model.Enum`: 枚举类型
- `SimpleSolitaire.Model.Config`: 配置类

### Unity 序列化
- 私有字段使用 `[SerializeField]` 暴露给 Inspector
- 序列化字段命名：驼峰命名 + 下划线前缀（如 `_cardLogic`）

### 代码注释
**本项目使用中文进行开发交互**，所有代码注释、提交信息和文档都应使用中文。代码中的重要方法和复杂逻辑需要添加中文注释。

### 文档生成约束
- **禁止自动生成文档**（`./Documents/` 目录）
- 生成任何文档前必须先向用户确认

---

## 重要注意事项

- **Google Mobile Ads**: 当前已禁用，所有引用已注释
- **平台**: 主要为移动设备（iOS/Android）设计，支持独立应用
- **资源加载**: 使用 Unity Resources 系统加载精灵和资源
- **自动完成**: 某些变体包含自动完成功能（剩余明显移动时触发）
- **粒子效果**: 纸牌移动触发由 `CardLogic` 管理的粒子系统
