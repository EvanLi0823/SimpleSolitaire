# Solitaire Word Associations 2.0.0 功能分析文档

## 基本信息

| 字段 | 内容 |
|------|------|
| 应用名称 | Solitaire Word（索利泰尔单词游戏） |
| 包名 | com.sng.solitaire.word.connections |
| 版本号 | 2.0.0（版本码 61） |
| 游戏引擎 | Unity 6000.0.58f2 |
| 目标平台 | Android（最低 API 24，目标 API 35） |
| 架构支持 | ARM64-v8a |
| 文件大小 | ~59 MB（XAPK） |

---

## 一、核心玩法

### 1.1 游戏类型
本作是 **经典接龙（Klondike Solitaire）与词语联想（Word Associations）的结合**。玩家在标准纸牌接龙的基础上，还需完成与当前关卡主题相关的词语分类任务。

### 1.2 牌局结构
- **发牌堆（Stock）**：翻牌源，可循环抽取。
- **列（Columns）**：7 列摆放区，牌面朝下或朝上。
- **基础堆（Foundation）**：4 个目标堆，按花色从 A 到 K 顺序叠放，完成即通关。
- **万能牌（Joker/Wildcard）**：可放置于任意位置的特殊牌。

### 1.3 关卡系统
- 关卡数据存储于 `LevelJSONFiles/levels_` 系列文件中（至少 100 关）。
- 另有独立的新手教程关卡（`LevelJSONFiles/tutorial_levels`）。
- 关卡元数据包含：关卡编号、最大步数、难度等级、基础堆完成数等信息。
- 每个关卡绑定一个**词语分类主题**，共 39 个主题分类（见下文）。
- 分类难度分三级：`CategoryLevel1`（初级）、`CategoryLevel2`（中级）、`CategoryLevel3`（高级）。

---

## 二、词语分类系统（Categories）

游戏内置 **39 个主题分类**，每个分类配有专属图片，玩家需在接龙过程中将对应主题的单词牌归入正确基础堆：

| 编号 | 英文标识 | 中文译名 |
|------|----------|----------|
| 1 | adorables | 可爱动物 |
| 2 | appliances | 家用电器 |
| 3 | arctic | 北极 |
| 4 | bakery | 烘焙/面包坊 |
| 5 | balls | 各类球 |
| 6 | bathroom | 浴室用品 |
| 7 | berry | 浆果类 |
| 8 | birds | 鸟类 |
| 9 | brunch | 早午餐 |
| 10 | camping | 野营 |
| 11 | cat_toys | 猫咪玩具 |
| 12 | cats | 猫类 |
| 13 | chinese | 中国主题 |
| 14 | circus | 马戏团 |
| 15 | citrus | 柑橘类 |
| 16 | connections | 词语连接 |
| 17 | dairy | 乳制品 |
| 18 | deck | 甲板/露台 |
| 19 | desert_animals | 沙漠动物 |
| 20 | dishes | 餐具/菜肴 |
| 21 | drive | 驾驶 |
| 22 | egypt | 埃及 |
| 23 | europe | 欧洲 |
| 24 | eye | 眼睛相关 |
| 25 | farm_new | 农场 |
| 26 | flags | 旗帜 |
| 27 | food | 食物 |
| 28 | furniture | 家具 |
| 29 | granary | 粮仓/谷物 |
| 30 | gym | 健身房 |
| 31 | hair | 发型/发饰 |
| 32 | hats | 帽子 |
| 33 | herb | 草药/香草 |
| 34 | insects | 昆虫 |
| 35 | japanese | 日本主题 |
| 36 | jewelry | 珠宝首饰 |
| 37 | knight | 骑士 |
| 38 | lamps | 灯具 |
| 39 | landmarks | 地标建筑 |

---

## 三、道具与辅助系统

### 3.1 提示（Hint）
- 高亮显示一个合法移动位置，帮助玩家找到出路。
- 消耗**金币**购买（基础价格 `HINT_COIN_COST`，每次使用后费用递增 `HINT_COIN_COST_INCREASE`）。
- 可通过**观看激励广告**免费获取（`ADS_HINTS_REWARD`）。

### 3.2 撤销（Undo）
- 撤销上一步操作，支持多步回退。
- 消耗**金币**购买（基础价格 `UNDO_COIN_COST`，费用递增 `UNDO_COIN_COST_INCREASE`）。
- 可通过**观看激励广告**免费获取（`ADS_UNDOS_REWARD`）。

### 3.3 万能牌（Joker）
- 可放置于场上任意有效位置的特殊牌。
- 消耗**金币**购买（`JOKER_COIN_COST`）。
- 可通过**观看激励广告**免费获取（`ADS_JOKER_REWARD`）。
- 有专属新手引导教程（`Joker_Tutorial`）。

### 3.4 额外步数（Extra Moves）
- 当步数耗尽弹出失败提示（`OutOfMovesPopup`）时，玩家可选择：
  - 花费**金币**购买额外步数（`Extra_Moves_With_Coin`）。
  - 直接失败重玩。

---

## 四、货币与商店系统

### 4.1 金币（Coins）
金币是游戏的主要通用货币，来源包括：
- 每日奖励（`DAILY_COINS_REWARD`）
- 关卡完成奖励
- 激励广告奖励
- 倍数游戏奖励（`multiplier_game_reward`）
- 内购充值

### 4.2 商店（Shop）
- 入口：`ShopScreen`
- 支持多种金币礼包内购（由 **Google Play Billing v7.1.1** 支持，Unity Purchasing v4.13.0）。
- 包含货币商店（`CoinStoreController`）和通用商店（`CurrencyStoreController`）。

---

## 五、每日奖励系统（Daily Bonus）

- 每天定时开放一次奖励（`DailyBonusController` / `DailyBonusPopup`）。
- 需要达到指定关卡等级方可解锁（`DAILY_BONUS_LEVEL_REQUIREMENT`）。
- 奖励内容包括：
  - 提示道具（`DAILY_BONUS_HINTS_REWARD`）
  - 万能牌（`DAILY_BONUS_JOKERS_REWARD`）
  - 撤销道具（`DAILY_BONUS_UNDOS_REWARD`）
  - 金币（`DAILY_COINS_REWARD`）

---

## 六、倍数小游戏（Multiplier Game）

- 关卡结束后可能触发倍数小游戏（`MultiplierGameController`）。
- 玩家可通过操作提升最终奖励倍数。
- 可选择中途停止（`OnStopMultiplierPressed`）锁定当前倍率。
- 奖励乘以倍数后发放（`multiplier_game_reward`）。

---

## 七、成就系统（Achievements）

- 集成 **MoreMountains Achievement Manager**（`MMAchievementManager`）。
- 支持多种成就类型（`AchievementType` / `AchievementTypes`）。
- 成就解锁时有专属动效展示（`AchievementDisplayPrefab` / `AchievementFadeDuration`）。
- 成就列表通过 `AchievementsList` 管理。

---

## 八、通知系统（Notifications）

- 使用 **Unity Android Notifications** 插件（`UnityNotificationManager`）。
- 支持以下通知场景：
  - 每日奖励提醒
  - 重新上线提示（`offline_notification_title`）
- 应用具有 `POST_NOTIFICATIONS` 权限。
- 通知调度由 `NotificationController` 管理。

---

## 九、教程系统（Tutorial）

- 分步骤引导新玩家（`TutorialController` / `TutorialStepDefinition`）。
- 有专属的教程关卡（`LevelJSONFiles/tutorial_levels`）。
- 包含以下教程类型：
  - 基础发牌与移牌操作
  - 万能牌使用方式（`Joker_Tutorial`）
  - 发牌堆操作提示（`ShowTutorialStockHint`）
- 支持跳过教程（`_skipTutorialPopup`）。

---

## 十、自定义系统（Customization）

- 支持卡牌背面样式切换（`CustomizeController`）。
- 存在多款卡牌背面设计（如 `Card_Back_5` 等）。
- 桌面主题（`ThemeStyleSheet` / `ApplyThemeStyleSheet`）可解锁。
- 自定义面板通过 `OpenCustomizePanel` 入口打开。

---

## 十一、广告系统（Ads）

- 广告平台：**Google AdMob**
  - 应用 ID：`ca-app-pub-1958331135942879~3116162624`
- 广告类型：
  - **激励视频广告**（Rewarded）：可换取提示、万能牌、撤销道具
  - **插屏广告**（Interstitial）：关卡切换时展示
- 广告展示阈值（`ADS_TIME_THRESHOLD`）：控制广告频率，避免过度打扰。

---

## 十二、数据分析与崩溃监控

| 服务 | 功能 |
|------|------|
| Firebase Crashlytics | 崩溃日志收集与上报 |
| Firebase Analytics | 行为数据分析（游戏开始、结束、购买等关键事件） |
| Google AdMob Insight | 广告效果追踪 |
| Google Play Referrer | 安装来源追踪 |

---

## 十三、本地化支持

- 支持 **70+ 语言区域**（通过 manifest.json 的 `locales_name` 字段可见）。
- 词语分类关键词支持按语言动态加载（`Localization/CategoryKeywords/{category}_{language}`）。
- 支持语言切换确认弹窗（`Language_Change_Confirm_Title / Text`）。

---

## 十四、主要界面一览

| 界面 | 说明 |
|------|------|
| `MainMenuScreen` | 主菜单 |
| `LoadingScreen` | 加载过渡界面 |
| `LevelCompletedScreen` | 关卡完成界面（含完美通关 `Level_Complete_Perfect`）|
| `OutOfMovesPopup` | 步数耗尽弹窗 |
| `DailyBonusPopup` | 每日奖励弹窗 |
| `ShopScreen` | 商店界面 |
| `SettingsController` | 设置界面（音效/音乐/通知等）|
| `CustomizeController` | 自定义皮肤界面 |
| `InfoPopup` / `GeneralPopup` | 通用信息弹窗 |
| `ConfirmPopup` | 确认操作弹窗（如新游戏确认）|

---

## 十五、技术架构

| 组件 | 版本/说明 |
|------|-----------|
| 游戏引擎 | Unity 6000.0.58f2 |
| 脚本语言 | C#（Assembly-CSharp），Kotlin/Java 层用于 Android 集成 |
| 动画框架 | DOTween（缓动动画），Spine（骨骼动画），MoreMountains Feedbacks |
| 资源管理 | Unity Addressables（资源按需加载，Bundle 分组） |
| 持久化 | SQLite（Room），Unity DataStore |
| 内购 | Unity Purchasing v4.13.0 / Google Play Billing v7.1.1 |
| 广告 | Google AdMob + Unity Ads |
| 崩溃监控 | Firebase Crashlytics（NDK 支持） |
| 数据分析 | Firebase Analytics |
| 通知 | Unity Android Notifications |
| 网络 | LiteNetLib（P2P），标准 HTTP/HTTPS |
| UI 特效 | UI Particle（Coffee），All In 1 Sprite Shader |

---

## 十六、所需权限说明

| 权限 | 用途 |
|------|------|
| `INTERNET` | 网络请求（广告、分析、排行榜） |
| `ACCESS_NETWORK_STATE` | 检测网络连接状态 |
| `BILLING` | Google Play 内购 |
| `WAKE_LOCK` | 防止游戏过程中屏幕休眠 |
| `VIBRATE` | 操作反馈震动 |
| `POST_NOTIFICATIONS` | 推送通知 |
| `AD_ID` / `ADSERVICES` | 广告个性化与归因 |
| `FOREGROUND_SERVICE` | 后台任务（如下载资源）|

---

*文档生成日期：2026-03-17*
*分析来源：APK 静态逆向分析（Unity Metadata、Smali、Manifest、Assets）*
