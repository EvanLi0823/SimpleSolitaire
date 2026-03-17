# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 交互语言
**本项目使用中文进行开发交互。**所有代码注释、提交信息和文档都应使用中文。代码中的重要方法和复杂逻辑需要添加中文注释。

## 文档生成约束

**🚫 严格禁止自动生成文档**

### 文档生成路径
- **指定路径**: `./Documents/`
- 所有项目文档必须生成到 `./Documents/` 文件夹
- 禁止在其他目录生成文档文件

### 文档生成权限
- **绝对禁止**：未经询问自动生成任何文档
- **必须执行**：生成任何文档之前，必须先向用户询问是否需要生成文档

**记住**：文档生成不是必需的，代码本身和注释才是核心。只有在用户明确需要时才生成文档。

---

## 项目概览

**项目名称**: SimpleSolitaire - 多变体纸牌游戏
**Unity版本**: 2022.3.62f2c1
**开发语言**: C#
**主要命名空间**: SimpleSolitaire

这是一个Unity 2D纸牌游戏项目，包含多个经典纸牌游戏变体：Klondike、Spider、Freecell、Pyramid和Tripeaks。

---

## Unity 开发环境

- **Unity 版本**: 2022.3.62f2c1
- **解决方案文件**: `Solitaire2.sln`
- **主要资源目录**: `Assets/SimpleSolitaire/`

---

## 项目结构

### 核心架构

项目采用基于组件的Unity架构，清晰的职责分离：

**基类层** (`Assets/SimpleSolitaire/Resources/Scripts/Controller/Base/`):
- `GameManager`: 所有游戏模式管理器的抽象基类
- `Card`: 纸牌对象的抽象基类，实现拖放功能
- `Deck`: 牌堆管理的基类
- `CardLogic`: 每个游戏变体扩展的逻辑基类
- `UndoPerformer`: 处理撤销/重做功能
- `HintManager`: 管理提示系统
- `StatisticsController`: 跟踪游戏统计

**游戏模式** (每个在独立目录中):
- `Klondike/`: 经典纸牌实现
- `Spider/`: 蜘蛛纸牌变体
- `Freecell/`: Freecell纸牌变体
- `Pyramid/`: 金字塔纸牌变体
- `Tripeaks/`: Tripeaks纸牌变体

每个游戏模式实现：
- `{GameMode}GameManager`: 扩展 `GameManager`
- `{GameMode}Card`: 扩展 `Card`
- `{GameMode}CardLogic`: 扩展 `CardLogic`
- `{GameMode}UndoPerformer`: 扩展 `UndoPerformer`
- `{GameMode}HintManager`: 扩展 `HintManager`
- `{GameMode}StatisticsController`: 扩展 `StatisticsController`

### 关键系统

**屏幕方向系统** (`Orientation/`):
- 处理竖屏/横屏方向切换
- `OrientationManager`: 管理设备方向变化
- `OrientationDataContainer`: 存储方向特定数据
- `OrientationObject` 组件: 各种方向感知UI组件
- 支持左手/右手游戏操作的手部方向

**广告集成**:
- `AdsManager.cs`: 当前Google Mobile Ads SDK依赖已注释掉
- 处理广告时，注意所有GoogleMobileAds.Api引用已禁用

**布局系统**:
- `LayoutEditor/`: 创建和编辑纸牌布局的工具
- `LayoutSolitaire/`: 布局数据结构
- Pyramid和Tripeaks变体的布局容器

---

## 开发规范

### 命名空间约定
所有代码使用 `SimpleSolitaire` 命名空间及子命名空间：
- `SimpleSolitaire.Controller`: 主要控制器类
- `SimpleSolitaire.Model.Enum`: 枚举类型
- `SimpleSolitaire.Model.Config`: 配置类

### Unity特定模式

**序列化字段**: 项目广泛使用Unity的序列化系统：
- 对需要在Inspector中暴露的私有字段使用 `[SerializeField]`
- 遵循Unity的序列化字段命名约定（带下划线前缀的驼峰命名）

**组件引用**: 大多数管理器通过 `[SerializeField]` 引用其他组件：
```csharp
[SerializeField] private CardLogic _cardLogic;
[SerializeField] private AdsManager _adsManagerComponent;
[SerializeField] private UndoPerformer _undoPerformComponent;
```

**纸牌系统**: 纸牌实现Unity的事件接口：
- `IBeginDragHandler`, `IDragHandler`, `IEndDragHandler` 用于拖放
- `IPointerClickHandler` 用于点击交互

### 添加新游戏模式

要添加新的纸牌变体：
1. 在 `Controller/` 下创建新目录
2. 实现所有必需的类，扩展基类
3. 遵循命名模式: `{GameMode}{ClassName}`
4. 在CardLogic子类中实现游戏特定逻辑
5. 如需要，创建布局容器

### 广告系统

Google Mobile Ads SDK集成已被注释。如需重新启用：
- 取消注释 `AdsManager.cs` 中的 `using GoogleMobileAds.Api;`
- 取消注释所有相关实现代码
- 测试横幅广告、插屏广告和奖励视频广告
- 验证 `IsHasKeyNoAds()` 功能

### 屏幕方向系统

屏幕方向系统复杂且影响多个组件：
- 修改需要更新 `OrientationManager` 和相关数据容器
- 组件可以使用 `OrientationObject` 基类实现方向感知
- 底部菜单、牌堆大小和图层缩放都响应方向变化
- 修改UI时测试竖屏和横屏模式

---

## 常见任务

### 构建项目
在Unity Editor 2022.3.62f2c1中打开并使用Unity的标准构建流程：
- File → Build Settings
- 选择目标平台（移动功能选择iOS/Android）
- Build 或 Build and Run

### 测试游戏模式
每个游戏模式在 `Assets/Scenes/` 下有自己的场景：
- 打开要测试的特定变体场景
- 通过Unity Editor的播放按钮进行播放模式测试
- 使用 `CardLogicTester` 在无需完整游戏设置的情况下测试纸牌逻辑

### 统计和撤销
- 统计使用Unity的 `PlayerPrefs` 持久化
- 撤销系统在变体特定实现中跟踪游戏状态变化
- 每个游戏模式可以有不同的撤销规则

---

## 重要注意事项

- **Google Mobile Ads**: 当前已禁用 - 所有引用已注释
- **平台**: 为移动设备（iOS/Android）设计，但支持独立应用
- **资源加载**: 使用Unity的Resources系统加载精灵和资源
- **自动完成**: 某些变体包含当剩余明显移动时的自动完成功能
- **粒子效果**: 纸牌移动触发由CardLogic管理的粒子效果
