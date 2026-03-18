# Solitaire Associations Journey 1.6.1 关卡配置数据分析

> 数据来源：global-metadata.dat（IL2CPP元数据）、AssetBundle、AndroidManifest、Unity二进制资产

---

## 一、关卡数据结构

### 1.1 关卡 JSON 核心字段

从 `CardColumnJson.cs`、`WordDataJson.cs`、`LevelDotJson.cs` 中提取的字段：

| 字段名 | 类型 | 说明 |
|--------|------|------|
| `columns` | Array | 牌柱数组（Tableau 列） |
| `words` | Array | 关联词语列表 |
| `wordsData` | Object | 词语数据对象 |
| `topic` | String | 关卡主题/分类 |
| `difficulty` | Enum | 关卡难度 |
| `movesLimit` | Int | 步数上限 |
| `movesLimitFrom` | Int | 步数上限范围（最小） |
| `movesLimitTo` | Int | 步数上限范围（最大） |
| `openCards` | Int | 初始翻开的牌数 |
| `timeSeconds` | Int | 时间限制（秒） |
| `wildcards` | Int | 万能牌（Wildcard/Joker）数量 |

### 1.2 牌列结构（CardColumnJson）

| 字段名 | 说明 |
|--------|------|
| `cardId` | 牌的唯一ID |
| `cardIds` | 牌ID列表（用于栈中的多张牌） |
| `columnId` | 列ID |
| `face` | 牌面朝向（正/反） |
| `CardType` | 牌的类型（见1.4节） |

### 1.3 词语数据结构（WordDataJson）

| 字段名 | 说明 |
|--------|------|
| `wordId` | 词语ID |
| `wordsRequired` | 需要完成的词语数量 |
| `wordsPlaced` | 已放置的词语数量 |
| `category` | 词语所属分类 |

---

## 二、关卡类型

### 2.1 LevelType 枚举

| 类型 | 说明 |
|------|------|
| `StandardLevelType` | 标准关卡（主要游戏模式） |
| `SwitchLevelType` | 切换关卡（变体模式） |
| `TutorialLevelType` | 教程关卡 |

### 2.2 难度（LevelDifficulty）

从代码结构推断（分析仅限元数据，原始枚举值经过加密）：
- 难度字段 `difficulty` 在关卡JSON中配置
- 通过 `LevelFilter`、`LevelStatus` 等类进行过滤和状态管理

---

## 三、牌类型（CardType）

游戏包含以下牌型（来自 `Core\Cards\` 目录）：

| 类名 | 说明 |
|------|------|
| `Card` | 基础牌（标准接龙用牌） |
| `WordCard` | 词语牌（带有关联词） |
| `CategoryCard` | 分类牌（带有词语类别标识） |
| `JokerCard` | 百搭牌（万能匹配） |
| `HintCard` | 提示牌（特殊功能牌） |

---

## 四、场地容器（Containers）

| 容器类 | 游戏区域 | 说明 |
|--------|----------|------|
| `StockContainer` | 牌库区 | 未翻开的备用牌堆 |
| `PlayStack / PlayStackContainer` | 游戏牌堆 | Tableau 中的牌柱 |
| `HomeContainer / HomeSlot` | 基础堆 | 完成区域（Foundation） |
| `CardContainer` | 通用容器 | 其他卡牌区域 |

---

## 五、关卡地图系统

### 5.1 地图数据文件（按语言分包）

```
SolitaireGameContent/
├── EN/  mapEnDefaultV3.json   ← 英语关卡地图（加密）
├── ZH/  mapZhDefaultV3.json
├── DE/  mapDeDefaultV3.json
├── ES/  mapEsDefaultV3.json
├── FR/  mapFrDefaultV3.json
├── IT/  mapItDefaultV3.json
├── JA/  mapJaDefaultV3.json
├── KO/  mapKoDefaultV3.json
├── PT/  mapPtDefaultV3.json
└── RU/  mapRuDefaultV3.json
```

> 所有地图/关卡数据均采用 **AES-CBC + HMAC** 加密（`JsonAesCbcHmacCrypto`），无法直接读取。

### 5.2 地图相关类

| 类名 | 说明 |
|------|------|
| `BasicLevelMap` | 基础关卡地图结构 |
| `IncludedBasicLevelMap` | 内置离线关卡地图 |
| `LevelMapElement` | 地图上的单个节点 |
| `LevelMapFileNames*` | 各语言地图文件名枚举 |
| `LevelMapConfigProvider` | 地图配置提供器 |

### 5.3 AssetBundle 分包

英语关卡共 **31 个 AssetBundle**（编号 11617~11707），每 3 个编号一个包，对应：
- 每个 bundle 包含 1 个 `level.json`（加密）
- 文件路径格式：`assets/__levelbundlebuildtemp/en_{bundle_id}/level.json`

---

## 六、关卡进度与保存系统

### 6.1 进度数据结构

| 类名 | 说明 |
|------|------|
| `LevelProgressData` | 关卡进度（当前关卡、完成数等） |
| `LevelSaveDto` / `LevelSaveData_V0` | 关卡存档数据 |
| `CardSaveDto` | 单张牌的存档状态 |
| `CardsDeckSaveData` | 整个牌组存档 |
| `InGameStreakProgress` | 游戏内连胜进度 |
| `UserProgressData` | 用户整体进度 |

### 6.2 版本迁移

| 迁移路径 | 说明 |
|----------|------|
| `0.1.62 → 0.1.72` | 历史版本数据迁移 |
| `M0 → M1` | 当前迁移版本 |

---

## 七、游戏系统（Core Systems）

| 系统类 | 功能 |
|--------|------|
| `CardLogicSystem / V2` | 牌的逻辑处理（匹配规则） |
| `CardViewSystem / V2 / V3` | 牌的视觉渲染 |
| `GameStateLogicSystem` | 游戏状态机（游玩/暂停/结束） |
| `InGameStreakLogicSystem` | 游戏内连胜逻辑 |
| `InGameStreakViewSystem` | 游戏内连胜 UI |
| `PossibleMovesSystem` | 可用步骤计算 |
| `EqualityStrategySystem` | 卡牌相等策略（匹配算法） |
| `PriorityStrategySystem` | 移动优先级策略 |
| `UndoSystem / V2` | 撤销逻辑 |
| `HintViewSystem` | 提示 UI 逻辑 |
| `TutorialAnalyticsSystem` | 教程数据采集 |
| `AnalyticsObservableSystem` | 可观测分析系统 |
| `HapticViewSystem` | 震动反馈 |
| `AudioViewSystem` | 音频播放 |
| `CheatObservableSystem` | 作弊检测（测试用） |
| `SimulationCardViewSystem` | 模拟器（自动玩法测试） |

---

## 八、教程系统

| 组件 | 说明 |
|------|------|
| `TutorialLevelType` | 教程关卡类型 |
| `TutorialWorld` | 教程世界/场景 |
| `TutorialStateEntity` | 教程状态实体 |
| `TutorialStepEntity` | 教程步骤实体 |
| `TutorialStepConfig` | 步骤配置 |
| `TutorialTargetType` | 目标类型（点击/拖拽等） |
| `TutorialType` | 教程类型 |
| `JokerTutorialData` | 百搭牌教程数据 |
| `FirstTutorialViewSystem` | 首次教程视图 |
| `InGameStreakTutorialPopup` | 游戏内连胜教程弹窗 |

---

## 九、游戏内连胜系统（InGameStreak）

> 注意：InGameStreak 是**关卡内**的小连胜，区别于跨关卡的 WinStreak。

| 组件 | 说明 |
|------|------|
| `InGameStreakData` | 连胜数据 |
| `InGameStreakProgress` | 连胜进度 |
| `InGameStreakLogicSystem` | 连胜逻辑 |
| `InGameStreakView` | 连胜 UI |
| `InGameStreakStepView` | 连胜步骤显示 |
| `CompleteStageAnimation` | 阶段完成动画 |
| `CoinsCollideListener` | 金币碰撞收集（连胜奖励动效） |
| `InGameStreakConfigProvider` | 配置（服务端可控） |

---

## 十、跨关卡连胜系统（WinStreak）

### 10.1 宝箱阶段

| 阶段 | 宝箱ID | 说明 |
|------|--------|------|
| 1 | `Chest1ID` | 第一阶段宝箱（最易获得） |
| 2 | `Chest2ID` | 第二阶段宝箱 |
| 3 | `Chest3ID` | 第三阶段宝箱 |
| 4 | `Chest4ID` | 第四阶段宝箱 |
| 5 | `Chest5ID` | 第五阶段宝箱（最难获得） |

### 10.2 奖励类型

| 奖励 | 说明 |
|------|------|
| 金币 (Coins) | `winStreakIconCoinsBubble` |
| 提示 (Hints) | `winStreakIconHintBubble` |
| 撤销 (Undos) | `winStreakIconUndoBubble` |

### 10.3 WinStreak 配置（服务端可控）

通过 `WinStreakConfig`（远程配置）控制：
- 各阶段所需胜场数
- 各宝箱奖励内容和数量
- 计时器时长（`winStreakTimerIcon`）
- 失败后保留状态策略

### 10.4 数据持久化

| 类 | 说明 |
|----|------|
| `WinStreakData` | 当前连胜状态 |
| `CurrentStreakMilestoneInfo` | 当前里程碑信息 |
| `StoredWinStreakAward` | 已获奖励记录 |
| `StoredWinStreakMilestone` | 里程碑进度存档 |

---

## 十一、道具（Booster）系统

### 11.1 道具类型

| 道具 | 类名 | 说明 |
|------|------|------|
| 提示 | `BoosterHint` | 高亮可移动的牌 |
| 撤销 | `BoosterUndo` | 撤销上一步操作 |
| 百搭牌 | `BoosterWildJoker` | 可与任意牌匹配 |

### 11.2 获取来源（BoosterSourceType）

- 关卡奖励（`BoosterReward`）
- 观看激励视频广告（`BoosterAdsWatchedCount`）
- 商店购买（金币/真实货币）
- 连胜宝箱
- 新手礼包

### 11.3 配置（服务端可控）

通过 `BoosterConfig`（远程配置）控制：
- 初始道具数量
- 广告观看道具奖励
- 道具价格（`BoosterPricesConfig`）

---

## 十二、商店与内购系统

### 12.1 IAP 商品（9种）

| 商品 ID | 类型 | 对应类 |
|---------|------|--------|
| smallcoinpack2 | 消耗品 | `IAPSmallCoinPackItem` |
| mediumcoinpack | 消耗品 | `IAPMediumCoinPackItem` |
| largecoinpack | 消耗品 | `IAPLargeCoinPackItem` |
| hugecoinpack | 消耗品 | `IAPHugeCoinPackItem` |
| megacoinpack | 消耗品 | `IAPMegaCoinPackItem` |
| premiumcoinpack | 消耗品 | `IAPPremiumCoinPackItem` |
| starterpack | 消耗品 | `IAPStarterPackItem` |
| noadspack | 非消耗品 | `IAPNoAdsPackItem` |
| noads | 非消耗品 | `IAPNoAdsItem` |

### 12.2 商店 UI 组件

| 组件 | 说明 |
|------|------|
| `ShopWindowView` | 主商店窗口 |
| `CoinsPackView` | 金币包展示 |
| `FreeCoinsPackView` | 免费金币包（看广告） |
| `SpecialPackView` | 特殊礼包 |
| `StarterOfferPackView` | 新手优惠礼包 |
| `PackPaginatorView` | 礼包翻页 |
| `ShopWinScreen` | 胜利后商店推广 |

### 12.3 新手礼包（StarterOffer）

- 限时倒计时展示（`StarterOfferTimerView`）
- 包含道具 + 金币 + 折扣组合（`DiscountOfferItemView`、`BoosterOfferItemView`、`CoinsOfferItemView`）
- 支持 A/B 测试（`StarterOfferConfig` 远程配置）

---

## 十三、关卡分发与下载系统（LevelSystemV2）

| 组件 | 说明 |
|------|------|
| `FilesManifest` | 关卡文件清单（含 hash 校验） |
| `BackendFilesLoader` | 从服务器下载关卡 |
| `LevelAvailabilityService` | 关卡是否可用检查 |
| `LevelValidator` | 关卡数据合法性校验 |
| `ContentFormatMigrator` | 关卡数据格式迁移 |
| `FileByLocaleHelper` | 按语言获取对应文件 |

**CDN 地址**（从二进制字符串提取）：
- 开发服: `https://asset.hadev.work`
- 生产 CDN: `https://asset-hitappsgames.b-cdn.net`

---

## 十四、远程配置（A/B 测试）项

所有配置均通过 Firebase Remote Config + 自研分群系统下发，支持动态调整：

| 配置名 | 控制内容 |
|--------|----------|
| `CoreLogicConfig` | 核心玩法逻辑参数 |
| `BoosterConfig` | 道具价格、初始数量 |
| `WinStreakConfig` | 连胜阶段、宝箱奖励 |
| `ShopConfig` | 商店展示、折扣配置 |
| `SoftCurrencyConfig` | 金币经济参数 |
| `StarterOfferConfig` | 新手礼包内容和触发时机 |
| `LiveOpsConfig` | 活动运营参数 |
| `AnimationsConfig` | 动画效果开关 |
| `CompensationConfig` | 补偿弹窗触发条件 |
| `PaywallConfig` | 付费门槛配置 |
| `ReviveBottomBoxConfig` | 复活弹窗底部扩展配置 |
| `AdsConfig` | 广告频率控制 |
| `AdsUnitIdsConfig` | 广告位 ID 配置 |
| `MainWidgetsConfig` | 主界面Widget显示 |
| `LocalNotificationsConfig` | 推送通知内容 |
| `UpdatePopupConfig` | 更新弹窗触发条件 |
| `InGameStreakConfig` | 游戏内连胜参数 |

---

## 十五、UI 弹窗体系

| 弹窗 | 场景 |
|------|------|
| `WinPopup / WinScreen` | 关卡胜利 |
| `LoseScreen / RewardedLoseView` | 关卡失败（含激励广告复活） |
| `NoMovesPopup` | 无可用步骤 |
| `OutOfMovesPopup` | 步数耗尽 |
| `PausePopup` | 暂停菜单 |
| `WinStreakMainScreen` | 连胜主界面 |
| `WinStreakWinPopup` | 连胜宝箱开启 |
| `WinStreakInfoPopup` | 连胜规则说明 |
| `WinStreakAreYouSurePopup` | 放弃连胜确认 |
| `StarterOfferPopup` | 新手礼包弹窗 |
| `CompensationPopup` | 补偿弹窗（游戏异常时发放） |
| `JokerTutorialView` | 百搭牌使用教程 |
| `WildJokerSelectionPopup` | 百搭牌目标选择 |
| `ShopWindowView` | 商店 |
| `SettingsPopup` | 游戏设置 |
| `ChangeLocalePopup` | 语言切换 |
| `NoInternetPopup` | 无网络提示 |
| `ContactUsView` | 联系客服 |
| `RateUsView` | 评分引导 |
| `UpdatePopup` | 版本更新提示 |
| `AdsPreferencePopup` | 广告偏好设置（GDPR） |
| `PrivacySettings` | 隐私设置 |
| `DeleteMyDataPopup` | 删除用户数据（GDPR） |
| `SomethingWentWrongPopup` | 异常错误提示 |
| `AdBreakView` | 广告休息界面 |
| `AboutPopup` | 关于游戏 |
| `BubbleBanner` | 气泡横幅（悬浮广告/通知） |

---

## 十六、安全系统

| 系统 | 实现 |
|------|------|
| 关卡数据加密 | AES-CBC + HMAC（`JsonAesCbcHmacCrypto`） |
| 防篡改检测 | `TamperGuardService` + `SecureInt128` |
| JSON 数据防篡改 | `JsonGuardService` |
| IAP 服务端校验 | `IAPServerValidatorService`（支持全量/静默两种模式） |
| 数据签名 | `UserDataProcessorSigner` + `TSign` |

---

## 十七、总结

| 维度 | 数据 |
|------|------|
| 关卡文件数（英语） | 31 个 AssetBundle（约31个关卡组） |
| 关卡语言支持 | 10 种（EN/ZH/DE/ES/FR/IT/JA/KO/PT/RU） |
| 牌类型 | 5 种（基础牌、词语牌、分类牌、百搭牌、提示牌） |
| 关卡类型 | 3 种（标准、切换、教程） |
| 道具类型 | 3 种（提示、撤销、百搭） |
| 宝箱阶段 | 5 阶段 |
| IAP商品数 | 9 种 |
| 远程配置项 | 17+ 项 |
| 关卡数据加密 | AES-CBC + HMAC（无法直接读取原始关卡内容） |
