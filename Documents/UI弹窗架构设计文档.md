# UI 弹窗架构设计文档

> 版本：1.1 | 日期：2026-03-17 | 命名空间：`SimpleSolitaire.Controller.UI` / `SimpleSolitaire.Utility`

---

## 目录

1. [架构概览](#1-架构概览)
2. [核心类说明](#2-核心类说明)
3. [弹窗优先级体系](#3-弹窗优先级体系)
4. [弹窗生命周期](#4-弹窗生命周期)
5. [优先级调度机制](#5-优先级调度机制)
6. [组件自动绑定工具（ComponentFinder）](#6-组件自动绑定工具componentfinder)
7. [当前弹窗清单](#7-当前弹窗清单)
8. [新增弹窗指南](#8-新增弹窗指南)
9. [场景接线说明](#9-场景接线说明)

---

## 1. 架构概览

```
┌─────────────────────────────────────────────────────────────┐
│                        GameManager                          │
│  OnClickSettingBtn() → _layerMediator.ShowSettingLayer()    │
└──────────────────────────┬──────────────────────────────────┘
                           │ 委托
                           ▼
┌─────────────────────────────────────────────────────────────┐
│                    GameLayerMediator                        │
│  LayerKey 常量 + Show/Hide 封装 + 复合流程编排              │
└──────────────────────────┬──────────────────────────────────┘
                           │ 调用
                           ▼
┌─────────────────────────────────────────────────────────────┐
│                    UILayerManager                           │
│  单例 | 缓存字典 | 优先级堆栈 | 等待队列 | CardLayer 管理  │
└──────────────────────────┬──────────────────────────────────┘
                           │ 注册/调度
                    ┌──────┴──────┐
                    ▼             ▼
            ┌─────────────┐  ┌─────────────┐  ...
            │ WinLayerUI  │  │SettingLayerUI│
            │(UILayerBase)│  │(UILayerBase) │
            └─────────────┘  └─────────────┘
```

**设计原则：**
- **单向依赖**：GameManager → Mediator → Manager → Layer，不反向。
- **GameObject 缓存**：弹窗关闭时 `SetActive(false)`，不销毁，避免重复实例化开销。
- **优先级堆栈**：高优先级弹窗压栈覆盖低优先级，低优先级进等待队列。
- **Manager 掌控注册**：UILayerManager 完全掌控弹窗实例化与缓存注册，通过 `_layerPrefabs` 按需实例化，`LayerKey` 由类名自动生成。被管理的 `UILayerBase` 不调用 `Register()`（避免自注册反模式）。
- **组件代码绑定**：通过 `ComponentFinder` 在 `OnBindComponents()` 中查找子节点，无需 Inspector 拖拽。

---

## 2. 核心类说明

### 2.1 UILayerBase（抽象基类）

**文件**：`Controller/UI/UILayerBase.cs`
**挂载位置**：每个弹窗 GameObject 的根节点

| 成员 | 类型 | 说明 |
|---|---|---|
| `_priority` | `LayerPriority`（Inspector） | 弹窗优先级，决定压栈行为 |
| `_animationType` | `LayerAnimationType`（Inspector） | `Standard`=Animator动画，`Instant`=直接切换 |
| `LayerKey` | `string`（只读） | 自动返回 `GetType().Name`，无需配置 |
| `IsVisible` | `bool` | 是否处于显示状态（含被压栈暂停的状态） |
| `IsPaused` | `bool` | 是否被高优先级弹窗压栈暂停 |
| `OnShowCompleted` | `event Action` | 弹窗完全出现后触发 |
| `OnHideCompleted` | `event Action` | 弹窗完全隐藏后触发（GO已SetActive(false)） |

**供子类 override 的模板方法：**

```csharp
protected virtual  void OnBindComponents() { }   // 组件绑定（Awake 自动调用）
protected abstract void OnLayerShow();            // 弹窗显示时的业务逻辑
protected abstract void OnLayerHide();            // 弹窗隐藏完成时的业务逻辑
protected virtual  void OnLayerPause()   { }     // 被压栈时（可暂停内部动画等）
protected virtual  void OnLayerResume()  { }     // 从压栈恢复时
```

---

### 2.2 UILayerManager（管理器）

**文件**：`Controller/UI/UILayerManager.cs`
**挂载位置**：场景 Managers 父节点（Script Execution Order 需早于所有 UILayerBase 子类）

**核心数据结构：**

```
_layerCache     Dictionary<string, UILayerBase>   弹窗实例缓存（Key = 类名）
_layerStack     Stack<UILayerBase>                活跃弹窗堆栈（栈顶 = 当前最高层）
_waitingQueue   Queue<string>                     低优先级等待队列
_cardLayer      GameObject                        游戏主牌桌层（弹窗全关时自动显示）
_popupRoot      Transform                         弹窗父节点（显示时将弹窗挂载到此节点下）
_layerPrefabs   List<UILayerBase>                 弹窗预制体列表（首次 Show 时按需实例化）
```

**实例化与注册流程：**

`Show(key)` 调用时，若 `_layerCache` 中不存在该 Key，`UILayerManager` 自动从 `_layerPrefabs` 找到匹配预制体，实例化到 `_popupRoot` 下，并直接写入缓存。**被管理的 `UILayerBase` 不参与注册**（Manager 掌控，避免自注册反模式）。

**公开 API：**

```csharp
void Show(string layerKey, bool forceShow = false)  // 请求显示
void Hide(string layerKey)                          // 请求隐藏
void HideTop()                                      // 隐藏栈顶（返回操作）
void HideAll()                                      // 隐藏所有弹窗并清空堆栈
bool IsShowing(string layerKey)                     // 查询是否显示中
UILayerBase GetTopLayer()                           // 获取当前栈顶弹窗
void SetMainLayerVisible(bool visible)              // 手动控制牌桌层显隐
```

**CardLayer 自动管理规则：**
`_layerStack.Count == 0` 时自动显示 CardLayer，否则隐藏。每次 `Push` 或 `Pop` 后自动触发。

---

### 2.3 GameLayerMediator（中介者）

**文件**：`Controller/UI/GameLayerMediator.cs`
**挂载位置**：GameManager 所在 GameObject

职责：
1. 集中定义所有弹窗的 `LayerKey` 常量，消除魔法字符串
2. 为每个弹窗封装 `Show{X}Layer()` / `Hide{X}Layer()` 方法
3. 编排需要跨弹窗协作的复合流程

**LayerKey 常量（与脚本类名一一对应）：**

```csharp
public const string WinLayer          = "WinLayerUI";
public const string ExitLayer         = "ExitLayerUI";
public const string ContinueGameLayer = "ContinueLayerUI";
public const string GameLayer         = "GameLayerUI";
public const string AdsLayer          = "AdsLayerUI";
public const string StatisticsLayer   = "StatisticsLayerUI";
public const string SettingLayer      = "SettingLayerUI";
public const string HowToPlayLayer    = "HowToPlayLayerUI";
```

**复合流程处理（内置在 Mediator 中）：**

| 流程 | 处理逻辑 |
|---|---|
| 打开统计（从设置入口） | 先 Hide Setting → Show Statistics（Statistics 进等待队列，Setting 动画结束后自动弹出） |
| 从设置打开教程 | Hide Setting → Show HowToPlay → 订阅 `OnHideCompleted` → 自动重新打开 Setting |

---

### 2.4 LayerPriority / LayerAnimationType（枚举）

**文件**：`Controller/UI/LayerPriority.cs`

```csharp
public enum LayerPriority
{
    Notify   = 20,   // 轻提示（预留）
    Info     = 40,   // 纯信息：HowToPlay
    Feature  = 60,   // 功能性：广告、统计、设置
    Gameplay = 80,   // 游戏流程：GameLayer、ContinueGame
    System   = 100,  // 系统级：胜利、退出确认
}

public enum LayerAnimationType
{
    Standard = 0,    // Animator Appear/Disappear 触发器 + 音效 + 0.42s 延迟
    Instant  = 1,    // 直接 SetActive，无动画无音效（调试/极简弹窗）
}
```

---

## 3. 弹窗优先级体系

```
System(100)   ██████████  WinLayerUI · ExitLayerUI
Gameplay(80)  ████████    GameLayerUI · ContinueLayerUI
Feature(60)   ██████      AdsLayerUI · StatisticsLayerUI · SettingLayerUI
Info(40)      ████        HowToPlayLayerUI
Notify(20)    ██          （预留）
```

**压栈规则：**
- 新弹窗优先级 **≥** 栈顶优先级 → **压栈**，栈顶弹窗 `Pause()`
- 新弹窗优先级 **<** 栈顶优先级 → **进入等待队列**，栈顶关闭后自动处理

---

## 4. 弹窗生命周期

### 显示流程

```
UILayerManager.Show(key)
    │
    ├─ 查找缓存（_layerCache）
    ├─ 检查 IsVisible（已显示则跳过）
    ├─ 检查优先级
    │     ├─ 可显示 → PushLayer()
    │     │     ├─ 当前栈顶 Pause()
    │     │     ├─ 新弹窗入栈
    │     │     ├─ layer.Show()
    │     │     │     ├─ SetActive(true)
    │     │     │     ├─ [Standard] 播放 Appear 动画 + WindowOpen 音效
    │     │     │     ├─ OnLayerShow()         ← 子类实现业务逻辑
    │     │     │     └─ OnShowCompleted 事件
    │     │     └─ UpdateCardLayerVisibility()
    │     └─ 不可显示 → 入等待队列
```

### 隐藏流程

```
UILayerManager.Hide(key)
    │
    ├─ 查找缓存，检查 IsVisible
    └─ PopLayer()
          ├─ RemoveFromStack()（从堆栈中移除，支持非栈顶关闭）
          └─ layer.Hide(onComplete)
                ├─ [Standard] 播放 Disappear 动画 + WindowClose 音效
                ├─ 等待 0.42s（WindowAnimationTime）
                ├─ OnLayerHide()               ← 子类实现业务逻辑
                ├─ SetActive(false)            ← GameObject 缓存，不销毁
                ├─ onComplete 回调
                │     ├─ 新栈顶 Resume()
                │     ├─ UpdateCardLayerVisibility()
                │     └─ ProcessWaitingQueue()
                └─ OnHideCompleted 事件
```

---

## 5. 优先级调度机制

### 场景示例：从设置弹窗打开统计弹窗

```
初始状态：Stack=[] Queue=[]

1. Show("SettingLayerUI")
   → Stack=[Setting] CardLayer隐藏

2. 点击统计按钮 → ShowStatisticsLayer()
   → Hide("SettingLayerUI")       先移除Setting
   → Stack=[] （Setting动画播放中）
   → Show("StatisticsLayerUI")    优先级60，Stack空→直接入栈
   → Stack=[Statistics]

   动画结束后（0.42s）：
   → Setting.SetActive(false)
   → ProcessWaitingQueue()（队列空，无操作）
```

### 场景示例：游戏中弹出系统级弹窗

```
当前状态：Stack=[Setting(60)]

Win条件触发 → Show("WinLayerUI")
   优先级 100 ≥ 60 → 压栈
   Setting.Pause()
   → Stack=[Setting(60), Win(100)]

关闭Win → Hide("WinLayerUI")
   → Stack=[Setting(60)]
   Setting.Resume()
   → 返回设置弹窗
```

---

## 6. 组件自动绑定工具（ComponentFinder）

**文件**：`Utility/ComponentFinder.cs`
**命名空间**：`SimpleSolitaire.Utility`

所有 `UILayerBase` 子类在 `OnBindComponents()` 中通过扩展方法自动查找组件，无需 Inspector 拖拽。

### 三种查找方式

```csharp
protected override void OnBindComponents()
{
    // 1. 路径查找（最精确，推荐）—— 沿 "父/子/目标" 路径定位
    _title = this.Get<Text>("Header/TitleText");

    // 2. 名称查找（全子树深度优先搜索）
    _closeBtn = this.Find<Button>("CloseButton");

    // 3. 场景全局查找（跨 GameObject 引用，如 GameManager）
    _gameManager = this.FindInScene<GameManager>();
}
```

### 缓存机制

| 查找类型 | 缓存 Key 格式 |
|---|---|
| 路径查找 | `{rootInstanceID}:p:{path}:{TypeName}` |
| 名称查找 | `{rootInstanceID}:n:{nodeName}:{TypeName}` |
| 场景查找 | `scene:{TypeName}:{includeInactive}` |

- 查找结果**默认缓存**，相同路径第二次调用直接返回缓存值
- 缓存组件被销毁时**自动失效**（下次访问时清理）
- `UILayerBase.OnDestroy()` 自动调用 `ComponentFinder.ClearCache(transform)` 清理本弹窗条目
- 场景切换时需手动调用 `ComponentFinder.ClearAll()` 清理全部缓存

---

## 7. 当前弹窗清单

| 类名 | 优先级 | 动画 | 触发入口 | 特殊逻辑 |
|---|---|---|---|---|
| `WinLayerUI` | System(100) | Standard | `GameManager.HasWinGame()` | 填充时间/分数/步数，保存最佳成绩 |
| `ExitLayerUI` | System(100) | Standard | 返回键/退出按钮 | Yes→保存状态+退出，No→关闭 |
| `ContinueLayerUI` | Gameplay(80) | Standard | 游戏启动时检测存档 | 通过 `OnContinueYes/No` 事件通知 GameManager |
| `GameLayerUI` | Gameplay(80) | Standard | Play 按钮 | 通过 `OnRandomClicked/OnReplayClicked` 事件通知 |
| `AdsLayerUI` | Feature(60) | Standard | 广告入口按钮 | 支持 `SetAdsType()` 预配置，`ShowRewardResult()` 展示结果 |
| `StatisticsLayerUI` | Feature(60) | Standard | 设置弹窗内统计按钮 | `OnRefreshRequested` 事件驱动数据刷新 |
| `SettingLayerUI` | Feature(60) | Standard | 底部设置按钮 | `OnRefreshRequested` 事件驱动开关刷新；内含"规则"和"统计"跳转 |
| `HowToPlayLayerUI` | Info(40) | Standard | 首次启动自动 / 设置→规则 | 显示时调用 `SetFirstPage()` 重置到第一页 |

---

## 8. 新增弹窗指南

### Step 1 — 创建脚本

在 `Controller/UI/Layers/` 目录下新建 `XxxLayerUI.cs`：

```csharp
using SimpleSolitaire.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleSolitaire.Controller.UI
{
    public class XxxLayerUI : UILayerBase
    {
        // 子节点组件（代码绑定，无需 Inspector 拖拽）
        private Text _titleText;

        protected override void OnBindComponents()
        {
            _titleText = this.Find<Text>("TitleText");
            // 跨节点引用：this.FindInScene<GameManager>()
        }

        protected override void OnLayerShow()
        {
            // 弹窗显示时填充数据
        }

        protected override void OnLayerHide()
        {
            // 弹窗隐藏时清理（可为空）
        }
    }
}
```

在 Inspector 中设置：
- `Priority`：根据业务选择合适等级
- `Animation Type`：通常保持 `Standard`

> **LayerKey 无需配置**，自动等于类名 `"XxxLayerUI"`。

### Step 2 — 在 GameLayerMediator 中注册

```csharp
public const string XxxLayer = "XxxLayerUI";

public void ShowXxxLayer() => _layerManager.Show(XxxLayer);
public void HideXxxLayer() => _layerManager.Hide(XxxLayer);
```

### Step 3 — 在 GameManager 中添加入口方法（可选）

```csharp
public void OnClickXxxBtn()
{
    _layerMediator?.ShowXxxLayer();
}
```

### Step 4 — 将预制体添加到 UILayerManager

在 Unity Inspector 中，将新弹窗的预制体拖入 `UILayerManager._layerPrefabs` 列表。首次调用 `Show("XxxLayerUI")` 时，Manager 自动实例化并注册，无需手动设置 `SetActive` 状态。

---

## 9. 场景接线说明

### UILayerManager

| 字段 | 说明 |
|---|---|
| `_cardLayer` | 游戏主牌桌层 GameObject（弹窗全关时自动显示） |
| `_popupRoot` | 弹窗父节点 Transform（弹窗实例化和显示时挂载到此节点；为空则使用本 GameObject） |
| `_layerPrefabs` | 弹窗预制体列表（按需实例化，首次 Show 时自动创建，Prefab 上需挂载对应 UILayerBase 子类） |

### GameLayerMediator

| 字段 | 说明 |
|---|---|
| `_layerManager` | UILayerManager 引用（可留空，Awake 时自动从单例获取） |

### GameManager

| 字段 | 说明 |
|---|---|
| `_layerMediator` | GameLayerMediator 引用（同节点挂载，直接拖入） |

### 弹窗 GameObject 最低配置要求

1. 根节点挂载 `Animator` 组件（即使 AnimationType=Instant 也需存在）
2. 根节点挂载对应 `XxxLayerUI` 脚本
3. Inspector 中设置 `Priority` 和 `Animation Type`
4. 将 Prefab 添加到 `UILayerManager._layerPrefabs` 列表（Manager 负责实例化，无需手动管理 SetActive 状态）

> ⚠️ **反模式警告**：不要在 `UILayerBase.Awake()` 中调用 `UILayerManager.Instance?.Register(this)`。注册应由 UILayerManager 在实例化时统一完成，被管理对象不参与缓存管理。

### Script Execution Order（重要）

```
UILayerManager  →  UILayerBase 的所有子类
（确保注册时 Instance 已存在）
```

在 Project Settings → Script Execution Order 中，将 `UILayerManager` 的执行顺序调整为比 Default 更早（填负数，如 -100）。
