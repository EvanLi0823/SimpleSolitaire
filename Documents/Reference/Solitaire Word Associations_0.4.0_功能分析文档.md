# Solitaire Word Associations 0.4.0 功能分析文档

## 基本信息

| 字段 | 内容 |
|------|------|
| 应用名称 | Solitaire Word Associations |
| 包名 | com.games.card.wordsolitaire |
| 版本号 | 0.4.0（版本码 101） |
| 游戏引擎 | Unity 6000.0.62f1 |
| 目标平台 | Android（最低 API 26 / Android 8.0，目标 API 36） |
| 架构支持 | ARM64-v8a |
| 文件大小 | ~173 MB（XAPK，含主包 134 MB + 原生库分包 39 MB） |
| 开发商官网 | outloud.games |
| Firebase 项目 | solitaire-word-associations |

---

## 一、核心玩法

### 1.1 游戏类型

本作是 **经典接龙（Klondike Solitaire）与词语联想（Word Associations）的融合**。玩家在标准纸牌接龙基础上，需完成与当前关卡主题绑定的单词分类任务。

### 1.2 牌局结构

- **发牌堆（Stock）**：翻牌来源，可循环抽取。
- **列（Columns）**：多列摆放区，牌面朝下或朝上排列。
- **基础堆（Foundation）**：目标堆，按规则顺序叠放完成即过关。
- **万能牌（Joker）**：可放置于任意有效位置的特殊牌。

### 1.3 关卡系统

- 关卡数据通过 **Unity Addressables** 动态加载，共 235 个资产包。
- 每个关卡绑定一个**词语分类主题（Deck）**，主题总数超过 **200 个**（详见第二章）。
- 关卡难度分三级：`CategoryLevel1`（初级）、`CategoryLevel2`（中级）、`CategoryLevel3`（高级）。
- 关卡参数示例：`moves: 110`（最大步数）、`decksCount: 4`（牌组数量）。
- 包含独立的新手教程关卡与多种专项教程（见第九章）。

### 1.4 玩法状态机

游戏主场景通过状态机管理，覆盖以下状态：

| 状态 | 描述 |
|------|------|
| `PlayFieldCoreState` | 核心游戏循环 |
| `PlayFieldDragCardState` | 拖拽牌操作 |
| `PlayFieldClearCategoryState` | 清除分类操作 |
| `PlayFieldCompleteLevelState` | 关卡完成流程 |
| `PlayFieldContinueLevelState` | 继续关卡（用道具续命）|
| `PlayFieldDeadlockState` | 死锁检测与处理 |
| `PlayFieldOutOfMovesState` | 步数耗尽处理 |
| `PlayFieldUseJokerState` | 使用万能牌 |
| `PlayFieldTutorialState` | 新手引导 |
| `PlayFieldDailyEventTutorialState` | 日常活动引导 |
| `PlayFieldLevelRewardsState` | 关卡奖励发放 |
| `PlayFieldInterstitialAdState` | 插屏广告展示 |
| `PlayFieldRewardedAdState` | 激励广告展示 |
| `PlayFieldEventEndedState` | 限时活动结束 |

---

## 二、词语分类系统（Categories / Decks）

游戏内置 **200+ 个主题分类**，通过 Addressables 按需加载，按领域划分如下：

### 基础生活类（约 30 个）
食物、国家、浴室用品、动物、家居、音乐、购物、乐器、人体、海滩、运动/足球、节日、电子设备、健康、厨房、交通、灯具、材料、甜品、工艺、森林、鸟类、水果、糖果、童年、烹饪、黄金、家务、棋盘游戏、茶时

### 历史文化类（约 25 个）
古希腊、罗马、美索不达米亚、埃及、波斯、日本、印度、石器时代、中世纪、阿兹特克、玛雅、斯堪的纳维亚、奥运、马戏团、首都城市、哲学、牛仔、海盗、地标建筑、欧洲

### 自然科学类（约 25 个）
海洋、地球科学、大气、气候、野生动物、地质学、植物、牛奶、葡萄、纺织、番茄、遗传学、科技、微生物学、实验室、解剖学、三角学、生物学、沙拉

### 节日季节类（约 15 个）
圣诞节、生日、万圣节、复活节、野餐、花园、风水、圣帕特里克节、感恩节、秋季、美女与野兽、白雪公主、长发公主、灰姑娘、斋月（Ramadan）

### 设计时尚类（约 20 个）
昆虫、饮品、配饰、珠宝首饰、蔬菜、星座、植物叶片、Emoji、头饰、非洲、亚洲、美洲、恐龙、浆果、夜行动物

### 流行娱乐类（约 30 个）
赛博朋克、货币、热带鱼、多肉植物、小美人鱼、化妆、气味、雨、南极、贝壳、理发、宇航员、船舶、晶体、韩国、鹦鹉、烧烤、洗衣、极限运动、水族馆、寿司/日料

### 高级深度类（约 25 个）
心理学、墓地、ASMR、夜魔侠（Yakuza）、民主、博物馆、橄榄球、电影、方程式赛车、苔原、退休、机械、普希金、陀思妥耶夫斯基

---

## 三、道具与辅助系统

### 3.1 撤销（Undo）

- 撤销上一步操作，支持多步回退。
- 消耗**金币**购买（价格由 `UndoPriceCoins` 配置）。
- 可通过**观看激励广告**免费获取。

### 3.2 移动推荐（Move Recommendation）

- 高亮显示当前局面中一个合法且优质的移动方案。
- 帮助玩家在复杂局面中找到最优走法。
- 消耗金币或通过广告获取。

### 3.3 清除分类（Clear Category）

- 直接完成当前关卡中的某一词语分类任务。
- 是本版本新增的强力道具，适用于分类步骤卡关时使用。
- 消耗金币购买，或通过广告获取。

### 3.4 万能牌（Joker）

- 可放置于场上任意有效位置的特殊牌。
- 消耗**金币**购买（`JokerPriceCoins`）。
- 可通过**观看激励广告**免费获取。
- 有专属新手引导教程。

### 3.5 额外步数（Continue Level）

- 步数耗尽时（`PlayFieldOutOfMovesPopupView`），玩家可选择：
  - 花费**金币**购买额外步数继续游戏（`PlayFieldContinueLevelPopupView`）。
  - 直接失败重新开始。

---

## 四、货币与商店系统

### 4.1 金币（Coins）

金币是游戏唯一通用货币，来源包括：

- 关卡完成奖励（`LevelCompleteCoinReward`）
- 观看广告后额外金币（`LevelCompleteAdCoinReward`）
- 每日首次购买奖励（`CoinBonusFirstPurchaseTimeSeconds`）
- 按游戏天数动态奖励（`CoinBonusPlayerDaysInGame`）
- 内购充值
- 日常活动奖励

### 4.2 商店（Store）

- 入口：`StoreView`
- 支持金币礼包内购（`CoinStoreItemView`）。
- 支持道具直接购买（`BoosterStoreItemView`）。
- **去广告（Remove Ads）** 内购选项（`NoAdsItemView`）。
- **限时优惠（Offer）**：`OfferView` / `OfferPresentationView`，带有限时倒计时。
- 内购由 **Google Play Billing v8.0.0** 支持。

---

## 五、日常活动系统（Daily Events）

- 游戏内独立的限时活动模块（`StarkGames.Solitaire.DailyEvents.*`）。
- 活动期间在主游戏界面显示活动进度组件（`DailyEventWidgetView`）。
- 完成特定关卡中的分类操作可积累活动积分（`PlayFieldClearedCategoryGiveDailyEventPointsState`）。
- 活动结束时有专属弹窗（`PlayFieldEventEndedPopupView`）。
- 活动失败（未在限时内完成）时弹出提示（`PlayFieldLoseEventLivePopupView`）。
- 可购买活动资源加快进度（`BuyEventResourcesPopupView`）。
- 包含专属活动引导教程（`PlayFieldDailyEventTutorialState` / `PlayFieldDailyEventProgressTutorialState`）。

---

## 六、进度与里程碑系统

- 玩家整体进度由 `StarkGames.Solitaire.Progression.*` 模块管理。
- 支持**里程碑（Milestone）**奖励，到达特定关卡数解锁。
- `ProgressionWidgetView` 在主界面实时展示当前进度。
- `ProgressionInfoPopupView` 可查看详细进度说明。

---

## 七、广告系统（Ads）

广告平台：**AppLovin MAX**（广告聚合平台）

| 广告类型 | 说明 |
|---------|------|
| 激励视频广告（Rewarded） | 可换取道具（撤销、万能牌、移动推荐、清除分类）或金币 |
| 插屏广告（Interstitial） | 关卡切换间隙展示（`PlayFieldInterstitialAdState`）|

**AppLovin MAX 聚合的广告网络（媒体适配器）：**

| 广告网络 | 说明 |
|---------|------|
| Google AdMob | AdMob 适配器 |
| Google Ad Manager | 程序化广告 |
| Facebook Audience Network | Meta 广告 |
| IronSource (LevelPlay) | 视频广告 |
| Mintegral (MBridge) | 程序化广告 |
| Moloco | 程序化广告 |
| Fyber / DT Exchange | 交易所广告 |
| Unity Ads | Unity 广告网络 |
| Vungle / Liftoff | 视频广告 |
| InMobi | 移动广告 |
| Nefta | 游戏原生广告 |
| Digital Turbine Ignite | 预装分发 |

---

## 八、数据分析与归因

| 服务 | SDK | 功能 |
|------|-----|------|
| Firebase Analytics | `Firebase.Analytics` | 行为数据分析 |
| Firebase Crashlytics | `Firebase.Crashlytics` | 崩溃日志收集（含 NDK） |
| AppMetrica (Yandex) | `io.appmetrica.analytics`（unity-6.8.0）| 用户行为分析 + 广告收入归因 |
| Singular | `Singular.SingularSDK` | 安装归因、深度链接、延迟深度链接 |
| Facebook SDK | `Facebook.Unity` | 应用事件自动记录、广告主 ID 采集 |

---

## 九、教程系统（Tutorial）

- 分步引导新玩家（`PlayFieldTutorialState`）。
- 包含专属教程关卡（独立于正式关卡）。
- 支持以下专项教程：
  - 基础发牌与移牌操作
  - 分类清除操作（`PlayFieldClearCategoryTutorialState`）
  - 无效移动提示（`PlayFieldInvalidMoveTutorialState`）
  - 万能牌使用（`Joker_Tutorial`）
  - 日常活动玩法（`PlayFieldDailyEventTutorialState`）
  - 活动进度追踪（`PlayFieldDailyEventProgressTutorialState`）

---

## 十、死锁检测系统

- 引擎可自动识别**牌局死锁**状态（`PlayFieldDeadlockState` / `PlayFieldDeadlockPopupView`）。
- 死锁发生时提示玩家，并提供解锁选项（使用道具或重玩）。

---

## 十一、商业化与评价系统

### 11.1 应用内评价

- 集成 **Google Play In-App Review API**（版本 `1.8.4`）。
- 在适当时机弹出评价请求（`RateUsPopupView`）。

### 11.2 A/B 测试

- 内置 A/B 测试框架（`StarkGames.Common.ABTesting.*`）。
- 支持远程配置（`StarkGames.Common.ConfigProvider.*`），可动态调整游戏参数。

### 11.3 防盗版

- 集成 **Pairip License Check**（`com.pairip.licensecheck.*`），验证 Google Play 授权，防止破解版运行。

---

## 十二、本地化支持

- 支持 **80+ 语言**（通过 Addressables 动态加载语言包）。
- 词语分类关键词按语言动态加载。
- 支持语言切换确认弹窗（`LanguageSettingsPopupView`）。

---

## 十三、设置与个性化

- 设置面板（`PlayFieldSettingsView`）：音效、音乐、通知、语言切换。
- **震动反馈**（`StarkGames.Common.Vibration.*`）：操作触觉反馈，可在设置中关闭。
- **通知系统**（`StarkGames.Common.Notifications.*`）：游戏重新上线提醒、日常活动提醒等。

---

## 十四、主要界面一览

| 界面 | 说明 |
|------|------|
| `PlayFieldView` | 游戏主玩法界面 |
| `PlayFieldSettingsView` | 游戏内设置 |
| `StoreView` | 商店主界面 |
| `LanguageSettingsPopupView` | 语言设置弹窗 |
| `PlayFieldCompleteLevelPopupView` | 关卡完成弹窗 |
| `PlayFieldOutOfMovesPopupView` | 步数耗尽弹窗 |
| `PlayFieldContinueLevelPopupView` | 继续关卡弹窗 |
| `PlayFieldDeadlockPopupView` | 死锁提示弹窗 |
| `PlayFieldRestartLevelPopupView` | 重新开始弹窗 |
| `PlayFieldEventEndedPopupView` | 活动结束弹窗 |
| `PlayFieldLoseEventLivePopupView` | 活动失败弹窗 |
| `BuyEventResourcesPopupView` | 购买活动资源弹窗 |
| `DailyEventWidgetView` | 日常活动进度组件 |
| `ProgressionWidgetView` | 关卡进度组件 |
| `CoinsMeterView` | 金币余额显示 |
| `RewardPopupView` | 奖励弹窗 |
| `RateUsPopupView` | 评价弹窗 |
| `NoInternetPopupView` | 无网络提示 |
| `OfferView` | 限时优惠弹窗 |
| `InfoPopupWithOkButtonView` | 通用信息弹窗 |
| `DialogueBubbleView` | 对话气泡（教程/提示）|
| `InterstitialAdView` | 插屏广告界面 |

---

## 十五、技术架构

| 组件 | 版本/说明 |
|------|-----------|
| 游戏引擎 | Unity 6000.0.62f1（Unity 6） |
| 脚本语言 | C#（IL2CPP 编译），Kotlin/Java 用于 Android 集成 |
| 脚本后端 | IL2CPP（Release，代码剥离开启） |
| 渲染管线 | URP（Universal Render Pipeline） |
| 动画框架 | DOTween（补间动画） |
| 文字渲染 | TextMeshPro |
| 图片加载 | Coil、Picasso（Square） |
| 资源管理 | Unity Addressables 2.7.4（动态加载，235 个资产包） |
| 持久化 | AndroidX Room（SQLite）、AndroidX DataStore |
| HTTP 客户端 | Ktor |
| JSON 序列化 | Newtonsoft.Json |
| 后台任务 | AndroidX WorkManager |
| 内购 | Google Play Billing v8.0.0 |
| 广告聚合 | AppLovin MAX |
| 崩溃监控 | Firebase Crashlytics（含 NDK 支持） |
| 数据分析 | Firebase Analytics、AppMetrica、Singular、Facebook SDK |
| 通知 | Android 原生通知（AndroidX） |
| 编译优化 | Unity Burst Compiler（`lib_burst_generated.so`） |
| 帧率优化 | Android Frame Pacing（Swappy，`libswappywrapper.so`） |
| 调试工具 | SRDebugger（随 Release 包打包）|
| 同意管理 | Google UMP（CMP）v3.2.0，GDPR 合规 |

---

## 十六、与 2.0.0 版本对比（主要变化）

| 维度 | 0.4.0 版本 | 2.0.0 版本 |
|------|-----------|-----------|
| 包名 | `com.games.card.wordsolitaire` | `com.sng.solitaire.word.connections` |
| 版本码 | 101 | 61 |
| Unity 版本 | 6000.0.62f1 | 6000.0.58f2 |
| 广告平台 | AppLovin MAX（多网络聚合）| Google AdMob |
| 广告 SDK 数量 | 11 个网络适配器 | 1 个 |
| 归因 SDK | Singular + AppMetrica + Facebook | 无独立归因 |
| 内购版本 | Google Play Billing v8.0.0 | v7.1.1 |
| 主题/分类数量 | 200+ | 39 |
| 日常活动系统 | ✅ 有 | ❌ 无 |
| A/B 测试框架 | ✅ 有 | ❌ 无 |
| 清除分类道具 | ✅ 有 | ❌ 无 |
| 移动推荐道具 | ✅ 有 | ❌ 无 |
| 防盗版（Pairip）| ✅ 有 | ❌ 无 |
| 商店去广告选项 | ✅ 有 | ❌ 无 |
| 限时优惠系统 | ✅ 有 | ❌ 无 |

---

## 十七、所需权限说明

| 权限 | 用途 |
|------|------|
| `INTERNET` | 网络请求（广告、分析、配置） |
| `ACCESS_NETWORK_STATE` | 检测网络连接状态 |
| `ACCESS_WIFI_STATE` | 检测 Wi-Fi 状态 |
| `BILLING` | Google Play 内购 |
| `CHECK_LICENSE` | Google Play 授权验证（防盗版）|
| `WAKE_LOCK` | 防止游戏过程中屏幕休眠 |
| `VIBRATE` | 操作反馈震动 |
| `POST_NOTIFICATIONS` | 推送通知 |
| `FOREGROUND_SERVICE` | 后台服务（资源下载等）|
| `ACCESS_ADSERVICES_AD_ID` | 广告 ID 访问 |
| `ACCESS_ADSERVICES_ATTRIBUTION` | 广告归因 |
| `ACCESS_ADSERVICES_CUSTOM_AUDIENCE` | 自定义受众（个性化广告）|
| `ACCESS_ADSERVICES_TOPICS` | 广告主题（Privacy Sandbox）|
| `AD_ID` | Google 广告 ID |

---

*文档生成日期：2026-03-17*
*分析来源：APK 静态逆向分析（Unity Metadata、Smali、Manifest、Addressables、第三方 SDK）*
