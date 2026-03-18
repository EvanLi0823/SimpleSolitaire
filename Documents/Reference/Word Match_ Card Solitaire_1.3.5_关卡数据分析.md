# Word Match: Card Solitaire — 关卡数据深度分析

> 版本：1.3.5 | 分析基础：IL2CPP global-metadata.dat + APK 反编译 | 引擎：Unity 2022.3.62（IL2CPP v31）

---

## 一、关卡数据核心结构

### 1.1 关卡类层次关系

```
ColorWordLevelData                  ← 所有关卡的基类
├── Categories           : 本关使用的单词类别列表
├── OperateSlotSizes     : 操作槽数量/尺寸配置
└── DifficultyType       : 难度类型（枚举）

    ├── ColorWordFixedLevelData     ← 固定种子关卡（手工编排）
    │   ├── FixedSeeds              : 固定随机种子数组
    │   ├── FixedSeedData           : 种子详细数据
    │   ├── RandomSeed1             : 随机种子1
    │   └── RandomSeed2             : 随机种子2
    │
    └── ColorWordLoopLevelData      ← 循环生成关卡（算法生成）
        ├── LoopID                  : 循环组ID
        ├── MinWordCnt              : 最少单词数量
        ├── MaxWordCnt              : 最多单词数量
        ├── MinUseSpriteCnt         : 最少图标使用数
        └── MaxUseSpriteCnt         : 最多图标使用数

GameplayLevelData                   ← 运行时关卡数据（内存中）
├── Words                : 本关单词集合（WordKVPInfo[]）
├── UseSprite            : 是否使用图标
├── ExclusiveID          : 专属ID（限定关卡）
├── ThemeID              : 主题皮肤ID
├── WordThemeInfo        : 单词主题信息
├── WordSlotInfo         : 单词槽信息
├── Infos                : 额外信息列表（WordFilterData）
├── Filters              : 过滤规则（WordFilterData.Filters）
├── NewLevelParams       : 新关卡参数标记
├── InfiniteMoves        : 是否无限步数
├── MoveParam1           : 步数参数1（起始步数相关）
├── MoveParam2           : 步数参数2（附加步数相关）
└── ExtraMove            : 额外步数（通过广告/购买获得）
```

### 1.2 单词卡片数据结构

```
WordKVPInfo
├── Words                : 单词文本键值对
├── UseSprite            : 是否显示图标（true=图+词，false=纯文字）
└── ExclusiveID          : 专属词条ID

EColorWordCardType（卡片类型枚举）
├── Word                 : 普通单词卡（含单词+图片）
├── Category             : 类别卡（代表一个类别目标）
└── Joker                : 万能卡（可匹配任意类别）

EColorWordSlotType（槽类型枚举）
├── Operate              : 操作槽（玩家放牌区域）
└── Prepare              : 待发牌槽（牌库/发牌区）
```

---

## 二、游戏棋盘与规则

### 2.1 棋盘结构

```
ColorWordBoard
├── OperateSlotCnt       : 操作槽数量（玩家活动区）
├── TargetSlotCnt        : 目标槽数量（完成区）
├── PrepareSlot          : 发牌槽（牌库）
│
├── MoveCnt              : 已用步数计数
├── LeftMoveCnt          : 剩余步数
├── CurProgress          : 当前完成进度
├── TargetProgress       : 目标总进度
│
├── KeepNoValidFromShuffle  : 洗牌后维持无效移动状态标记
└── GlobalNotFoundLoop      : 全局无效循环计数

ColorWordBoardMemento（棋盘快照，用于撤回）
├── OperateSlots         : 操作槽快照
└── FinishedCnt          : 已完成类别数
```

### 2.2 胜负条件

| 条件 | 说明 |
|------|------|
| **胜利** | 所有目标槽完成（`CheckIsWin`），即所有类别卡片配对归位 |
| **失败** | 步数耗尽（`CheckOutOfMoves`） |
| **无效移动** | `IsNoValidMove` / `IsNoValidMoveGeneral` 触发无效移动提示 |
| **死局检测** | `IsDealable` 判断当前局面是否可解 |

### 2.3 操作命令系统

游戏采用**命令模式（Command Pattern）**管理所有操作，支持完整的撤回（Undo）机制：

```
GameplayCommandCenter
├── GameplayMoveCommand        ← 移动卡片（核心操作）
│   ├── MoveCards             : 移动的卡片列表
│   ├── MoveEndCards          : 移动完成的卡片
│   ├── OriginSlot            : 来源槽
│   ├── TargetSlot            : 目标槽
│   ├── ExposedCard           : 翻开的卡片
│   ├── IsSlotFinished        : 目标槽是否完成
│   ├── CleanedCards          : 被清除的卡片
│   └── ResetCategoryIdx      : 重置的类别索引
│
├── ColorWordDealCommand       ← 发牌操作
│   ├── OriginLocators        : 来源位置
│   ├── TargetLocators        : 目标位置
│   └── CardDealPriorities    : 发牌优先级
│
├── ColorWordShuffleCommand    ← 洗牌操作
└── GameplayBatchCommand       ← 批量操作（多步合并为一次撤回）
```

### 2.4 提示系统（Hint）

```
ColorWordHintHandler
├── TryGetNextHint()      : 获取下一个提示
├── ColorWordHintResult
│   ├── OriginCard        : 建议操作的来源卡
│   ├── TargetSlot        : 建议放置的目标槽
│   ├── HintIdx           : 提示序号
│   └── CardIdx           : 卡片索引
└── 搜索策略
    ├── SearchForEachPrepareSlot  : 搜索待发牌区
    ├── SearchForEachHomeSlot     : 搜索归位槽
    ├── SearchForEachOperateSlot  : 搜索操作槽
    └── SearchForSpecificCard     : 搜索特定卡片
```

---

## 三、关卡配置参数（GameplayConfig）

### 3.1 步数配置

| 字段名 | 类型 | 含义 |
|--------|------|------|
| `MovesParam1` | int | 步数基础参数1 |
| `MovesParam2` | int | 步数基础参数2 |
| `DefaultAddMoves` | int | 默认附加步数（关卡初始补充） |
| `LevelAddMoves` | int | 关卡进阶附加步数 |
| `NewPlayLevelAddMoves` | int | 新游玩关卡的附加步数 |
| `HighGoldAddMoves` | config | 高金币时附加步数配置 |
| `MagicMoveCardCounts` | int | 魔法道具操作的卡片数量 |
| `InfiniteMoves` | bool | 是否为无限步数关卡 |

### 3.2 关卡分组配置

| 字段名 | 含义 |
|--------|------|
| `MainGroup` | 主线关卡分组数据 |
| `LoopMainGroup` | 循环主线关卡分组 |
| `LoopMainGroupLevelRange` | 循环关卡的关卡范围定义 |
| `IsRandomSeedFixedOn` | 是否启用固定随机种子（保证关卡一致性） |
| `UseFixedSeedFailTimes` | 使用固定种子失败后切换随机的次数 |

### 3.3 奖励与货币配置

| 字段名 | 含义 |
|--------|------|
| `LevelWinCoin` | 关卡胜利基础金币奖励 |
| `LevelWinCoinMultiple` | 金币倍率（连胜加成） |
| `LevelDailyWinCoinMultiple` | 每日挑战关卡金币倍率 |
| `LevelHistoryDailyWinCoinMultiple` | 历史每日挑战金币倍率 |
| `MovesExCoinLimit` | 超出步数时的金币上限 |
| `DailyNeedAd` | 每日挑战是否需要观看广告 |
| `DailyNeedCoinNum` | 每日挑战需要的金币数 |

### 3.4 界面与系统配置

| 字段名 | 含义 |
|--------|------|
| `AdBaseOpenLevel` | 开始显示广告的关卡门槛 |
| `ShowHomeButton` | 是否显示返回主页按钮 |
| `ContinueToLevel` | 继续时跳转的目标关卡 |
| `SettingHomeShowLevel` | 显示首页设置的关卡门槛 |
| `ResetDeck` | 是否允许重置牌库 |
| `PassingCardOpen` | 过牌时是否自动翻开卡片 |
| `WordGameTypeCardRates` | 各类型卡片生成概率配置 |

---

## 四、道具配置（ItemConfig / MoveConfig）

### 4.1 道具系统

```
ItemConfig
├── NewUserCount          : 新用户初始道具数量
├── AdRewardCnt           : 观看广告获得道具数
├── CoinRewardCnt         : 金币兑换道具数
├── CoinCost              : 道具金币消耗
└── UnlockLevel           : 道具解锁所需关卡数

MoveConfig（步数道具专项配置）
├── DailyAdMaxTimes       : 每日最多观看广告获得步数次数
├── RewardCntArray        : 不同观看次数对应的奖励步数数组
└── （继承 ItemConfig 字段）
```

### 4.2 三种核心道具

| 道具 | UI组件 | 功能说明 |
|------|--------|---------|
| **Undo（撤回）** | `UndoBtn` / `Go_UndoBtn` | 撤销上一步操作，还原棋盘状态 |
| **Magic（魔法）** | `MagicBtn` / `Go_MagicBtn` | 触发 `CouldUseMagic` 检查，自动移动 `MagicMoveCardCounts` 张卡片 |
| **Joker（万能牌）** | `JokerBtn` / `Go_JokerBtn` | `CouldUseJoker` 检查后，投入一张 JokerCard 到棋盘 |

---

## 五、游戏模式详解

### 5.1 主线关卡（Main Loop）

**关卡类型**：`ColorWordLoopLevelData`（算法循环生成）

| 参数 | 说明 |
|------|------|
| `LoopID` | 关卡在循环组内的编号 |
| `MinWordCnt` ~ `MaxWordCnt` | 本关单词卡片数量范围 |
| `MinUseSpriteCnt` ~ `MaxUseSpriteCnt` | 图标显示的卡片数量范围 |
| `DifficultyType` | Normal / Hard / ExtraHard |

**难度标识**（字体材质区分）：

| 难度 | 字体材质 | 说明 |
|------|----------|------|
| Normal | `FontMatNormal` | 普通难度，绿色步数显示 |
| Hard | `FontMatHard` | 困难，特殊颜色显示 |
| ExtraHard | `FontMatExtraHard` | 超难，高亮警告色 |
| CountDown | `FontMatCountDown` | 步数极少时进入倒计时警告样式 |

### 5.2 每日挑战（Daily Challenge）

**关卡类型**：`ColorWordFixedLevelData`（固定种子，确保所有玩家当天挑战相同关卡）

**存档处理器**：`DailyChallengeArchiveHandler` + `DailyArchiveHandler`

| 机制 | 说明 |
|------|------|
| 日历显示 | `HomeDailySubPage` 显示当月日历，每日一关 |
| 完美记录 | 记录当天是否完美通关（`GameplayTargetLevelInfo.OnProgressUpdate`） |
| 奖励触发 | `PlayFinishDailyChallengeAnim` + `ShowGetDailyCoinAnim` |
| 里程碑 | `reachedMilestone` 字段触发特殊里程碑奖励 |
| 金币加成 | `LevelDailyWinCoinMultiple`（超出历史最快则用 `LevelHistoryDailyWinCoinMultiple`） |
| 广告门槛 | `DailyNeedAd`（是否强制看广告才能开始） |

### 5.3 困难关卡（Hard Level）

**页面**：`HardLevelPage`

专门设计的高难度关卡，独立于主线循环之外，在特定关卡进度解锁后可进入。

### 5.4 恐龙探险（Dino Quest）

**关卡类型**：`DinoPassLevelData`

```
DinoPassLevelData
├── MinPlayerNum          : 最少玩家数（多人对战）
├── MaxPlayerNum          : 最多玩家数
└── MinDisuseNum          : 最少废弃数
```

**存档**：`DinoQuestArchiveHandler`

**服务器配置（DinoQuestConfig）**：
```
DinoQuestConfig
├── ActiveTime            : 活动开始时间
├── FailTime              : 活动结束时间
└── Rewards               : 奖励配置
```

**页面流程**：
```
DinoQuestStartPage → DinoQuestWaitPlayersPage → DinoQuestMainPage
                                                    ↓
                                          DinoQuestFailPage 或 DinoQuestGotRewardsPage
```

**服务端接口**：
- `C2S_colorsortuser_random_get` — 匹配随机对手
- `C2S_colorsortuser_get` — 获取指定用户信息
- `C2S_colorsort_user_info_report` — 上报用户对局信息

### 5.5 彩色连胜（Color Streak）

**存档**：`ColorStreakArchiveHandler`

**数据库表**：`TB_ColorStreakStage`（定义连胜阶段奖励）

| 字段 | 说明 |
|------|------|
| `CoinCnt` | 本次连胜获得金币数 |
| `CoinMultiply` | 金币倍率（连胜越多倍率越高） |
| `SupportColorStreak` | 当前关卡是否支持 Color Streak |
| `FlyCoinBeforeReturnHome` | 返回主页前先播放金币飞动画 |

**页面**：
- `ColorStreakMainPage` — 连胜结算界面（显示倍率和金币）
- `ColorStreakExplainPage` — 规则说明
- `ColorStreakGainPage` — 获得奖励动画
- `ColorStreakQuitLevelPage` — 中途退出时的连胜处理

### 5.6 节日挑战（Festival Challenge）

**存档**：`FestivalArchiveHandler`

```
FestivalConfig
├── StartTime             : 活动开始时间戳
├── EndTime               : 活动结束时间戳
└── BlockUnlockLevel      : 解锁活动所需关卡数

FestivalChallengeConfig
└── EntranceShowLevel     : 显示活动入口所需关卡数

FestivalChallengeData（TB_FestivalChallengeData）
FestivalChallengeShopData（TB_FestivalChallengeShopData）
```

**页面**：
```
FestivalChallengeUnlockPage → FestivalChallengeExplainPage
                                        ↓
                             FestivalChallengeMainPage
                                        ↓
                             FestivalChallengeShopPage（限时商店）
```

### 5.7 主题套包（Chain Package）

**存档**：`ChainPackageArchiveHandler`

**数据结构**：
```
ChainPackageData（TB_ChainPackageData）
├── itemID               : 套包ID
└── remark               : 备注/描述

ChainPackageArchive
ChainPackageMgr
ChainPackageTreasure
ChainPackageTreasureItem
ChainPackageLockParticle
ChainPackCardBurstParticle
ChainPackTreasureBurstParticle
```

**UI组件**：
- `ChainPackagePage` — 主套包展示页
- `ShopGoodsPackagePopPage` — 套包购买弹窗

---

## 六、数据库表结构总览（游戏本地数据）

| 表名 | 中文含义 | 主要字段 |
|------|----------|---------|
| `TB_ItemData` | 道具数据 | 道具ID、类型、数量、解锁条件 |
| `TB_GoodsInfo` | 商品信息 | 商品ID、价格、内容 |
| `TB_GoodsPackageInfo` | 商品套包信息 | 套包ID、包含商品、折扣 |
| `TB_GoodsPackageStyleInfo` | 套包样式信息 | 套包外观、颜色主题 |
| `TB_LevelGoodsInfo` | 关卡商品信息 | 特定关卡可购买的商品 |
| `TB_PackageData` | 礼包数据 | 礼包内容、有效期 |
| `TB_DailyRewardData` | 每日奖励数据 | 签到天数、奖励类型、数量 |
| `TB_TaskData` | 任务数据 | 任务ID、目标、奖励 |
| `TB_TaskType` | 任务类型 | 任务分类枚举 |
| `TB_JourneyReward` | 旅途奖励 | 里程碑关卡、奖励内容 |
| `TB_ChainPackageData` | 主题套包数据 | 套包ID、关联道具 |
| `TB_ColorStreakStage` | 连胜阶段数据 | 阶段号、倍率、奖励 |
| `TB_FestivalData` | 节日活动数据 | 活动ID、时间范围 |
| `TB_FestivalChallengeData` | 节日挑战关卡 | 挑战关卡配置 |
| `TB_FestivalChallengeShopData` | 节日商店数据 | 限时商品列表 |
| `TB_ConstValue` | 常量表 | 全局数值常量 |
| `TB_CustomLocalizedData` | 自定义本地化 | 语言码、文本映射 |

---

## 七、玩家存档系统（Archive Handlers）

每个模块拥有独立的存档处理器，支持**本地存储**和**服务器同步**：

| 存档处理器 | 管理数据 |
|-----------|---------|
| `UserArchiveHandler` | 用户基础信息（昵称、头像、等级、UID） |
| `LevelArchiveHandler` | 关卡进度（已通关数、最高关卡、星级） |
| `InventoryArchiveHandler` | 道具背包（Undo/Magic/Joker数量） |
| `DailyArchiveHandler` | 每日数据（登录天数、连续登录天数） |
| `DailyChallengeArchiveHandler` | 每日挑战记录（每日完成状态、完美记录） |[Word Match_ Card Solitaire_1.3.5_功能介绍](Word Match_ Card Solitaire_1.3.5_功能介绍.md)
| `DailyRewardArchiveHandler` | 签到奖励状态 |
| `DailyTasksArchiveHandler` | 每日任务进度 |
| `JourneyArchiveHandler` | 旅途进度（里程碑解锁状态） |
| `ColorStreakArchiveHandler` | 连胜记录（当前连胜数、最高倍率） |
| `DinoQuestArchiveHandler` | 恐龙探险数据（对战记录、奖励领取） |
| `FestivalArchiveHandler` | 节日活动进度 |
| `ChainPackageArchiveHandler` | 主题套包购买/使用记录 |
| `ThemeArchiveHandler` | 已解锁主题皮肤 |
| `ChargeArchiveHandler` | 购买/充值记录 |
| `AdArchiveHandler` | 广告观看记录（防超限） |
| `TutorialArchiveHandler` | 新手引导完成状态 |
| `NotificationArchiveHandler` | 通知偏好设置 |

**存档同步机制**：
```
AccountManager
├── FetchGameSettings()         : 从服务器拉取游戏配置
├── UpdateAllArchives()         : 批量更新全部存档
├── UploadAllArchives()         : 上传存档到服务器
├── LoadDataFromLocal()         : 从本地读取存档
└── UploadToServerInterval      : 自动上传间隔时间
```

---

## 八、服务器通信协议

### 8.1 客户端→服务器（C2S）

| 消息名 | 功能 |
|--------|------|
| `C2S_HEART` | 心跳包（保持连接） |
| `C2S_reg_login` | 注册/登录（合并接口） |
| `C2S_update_user` | 更新用户数据（昵称、头像等） |
| `C2S_get_game_setting` | 拉取服务端游戏配置（关卡参数、活动配置等） |
| `C2S_preference` | 上报用户偏好设置 |
| `C2S_colorsort_user_info_report` | 上报对局信息 |
| `C2S_colorsortuser_get` | 获取指定用户信息 |
| `C2S_colorsortuser_random_get` | 随机匹配对手（Dino Quest） |
| `C2S_sudokuscorerank_get` | 获取排行榜数据 |

### 8.2 服务器→客户端（S2C）

| 消息名 | 功能 |
|--------|------|
| `S2C_reg_login` | 登录/注册响应（含用户数据、游戏配置） |

### 8.3 连接参数
```
NetworkManager
├── mKey      : AES加密密钥
├── mIv       : AES初始向量
├── mSalt     : 加密盐值
├── mSeq      : 请求序列号
└── cTimeOutCode / cTimeOutMsg : 超时处理
```

---

## 九、排行榜系统

### 9.1 排行榜类型（ERankingListPageType）

| 类型 | 说明 |
|------|------|
| `WorldTotal` | 全球总榜 |
| `LocalTotal` | 本地（好友）总榜 |
| `WorldWeekly` | 全球周榜（每周重置） |
| `LocalWeekly` | 本地（好友）周榜 |

### 9.2 周榜结算

- `WeeklySettlementSubPage` — 每周结算界面
- `SubPageWeeklySettlement` — 结算子页面
- 服务端接口：`C2S_sudokuscorerank_get`
- 排名显示：`TxtMyRank`（我的排名）、`TxtMyLevel`（我的等级）

---

## 十、游戏设置选项

### 10.1 可配置项（Setting_*）

| 设置项 | 说明 |
|--------|------|
| `Setting_Sound` | 音效开关 |
| `Setting_Music` | 音乐开关 |
| `Setting_Vibration` | 振动反馈开关 |
| `Setting_AutoLock` | 自动锁屏禁用开关 |
| `Setting_CardCounts` | 显示卡片数量提示 |
| `Setting_CanRedealCards` | 是否允许重新发牌 |
| `Setting_EndGameAlert` | 游戏结束前是否弹出确认 |
| `Setting_LeftHanded` | 左手模式（UI镜像翻转） |
| `Setting_MatchHint` | 匹配提示高亮开关 |
| `Setting_MoveTips` | 步数提示开关 |
| `Setting_ProgressTime` | 是否显示进度计时 |
| `Setting_SmartMove` | 智能移动辅助（自动完成明显操作） |

---

## 十一、UI 页面全览（58个页面）

### 11.1 游戏进行中页面
| 页面 | 中文说明 |
|------|---------|
| `GameScene` | 主游戏场景（棋盘） |
| `LevelSettingPage` | 关卡内设置 |
| `QuitLevelPage` | 退出关卡确认（含多种子类型） |
| `NoValidMovePage` | 无有效移动提示 |
| `LevelRetryPage` | 重试关卡 |
| `HardLevelPage` | 困难关卡入口 |
| `StageBannerPage` | 阶段横幅（进入新阶段时） |

### 11.2 胜负结算页面
| 页面 | 中文说明 |
|------|---------|
| `SettlementPage` | 关卡结算（胜利） |
| `LosePage` | 失败页面 |
| `LosePurchaseItemPage` | 失败后购买道具页 |
| `ColorStreakMainPage` | 连胜结算 |
| `ColorStreakGainPage` | 连胜奖励展示 |
| `ColorStreakQuitLevelPage` | 连胜中途退出处理 |

### 11.3 每日系统页面
| 页面 | 中文说明 |
|------|---------|
| `DailyChallengePage` | 每日挑战入口 |
| `DailyEntryLevelPage` | 每日挑战关卡进入页 |
| `DailyRewardsPage` | 每日奖励页（签到） |
| `DailyTrophyRewardPage` | 每日奖杯奖励 |
| `DailyStarTipsPage` | 每日星级提示 |
| `TasksPage` | 任务列表页 |

### 11.4 商店与购买页面
| 页面 | 中文说明 |
|------|---------|
| `LevelItemShopPage` | 关卡内道具商店 |
| `CoinItemShopPage` | 金币购买页 |
| `LevelItemAdPage` | 关卡内广告道具页 |
| `LevelItemDoubleBtnPage` | 双按钮选项页（买或看广告） |
| `LevelItemMoreOffersPage` | 更多优惠页 |
| `ShopGoodsPackagePopPage` | 商品套包弹窗 |
| `ChainPackagePage` | 主题套包展示 |
| `AdsRemoverCardPage` | 去广告——卡片页 |
| `AdsRemoverRewardPage` | 去广告——奖励页 |
| `AdsRemoverLevelPage` | 去广告——关卡页 |
| `ChargeFailPage` | 充值失败页 |
| `UpgradePage` | 升级/解锁页 |

### 11.5 VIP 系统页面
| 页面 | 中文说明 |
|------|---------|
| `VIPCardPage` | VIP会员卡展示 |
| `VIPExplainPage` | VIP权益说明 |

### 11.6 特殊活动页面
| 页面 | 中文说明 |
|------|---------|
| `DinoQuestStartPage` | 恐龙探险开始 |
| `DinoQuestWaitPlayersPage` | 等待玩家匹配 |
| `DinoQuestExplainPage` | 恐龙探险规则说明 |
| `DinoQuestMainPage` | 恐龙探险主对战界面 |
| `DinoQuestFailPage` | 恐龙探险失败 |
| `DinoQuestGotRewardsPage` | 恐龙探险奖励领取 |
| `FestivalChallengeUnlockPage` | 节日挑战解锁 |
| `FestivalChallengeMainPage` | 节日挑战主界面 |
| `FestivalChallengeExplainPage` | 节日挑战说明 |
| `FestivalChallengeShopPage` | 节日限时商店 |
| `ColorStreakExplainPage` | 连胜规则说明 |

### 11.7 用户系统页面
| 页面 | 中文说明 |
|------|---------|
| `EditProfilePage` | 编辑个人资料 |
| `SetNamePage` | 设置昵称 |
| `UserInfoPage` | 用户信息查看 |
| `HomeSettingPage` | 主页设置 |
| `SettingLanguagePage` | 语言设置 |
| `NotificationPage` | 通知设置 |
| `FeedbackPage` | 意见反馈 |
| `RatePage` | 评分页 |
| `ThankRatePage` | 评分感谢页 |

### 11.8 教程与引导
| 页面 | 中文说明 |
|------|---------|
| `LevelItemTutorialPage` | 关卡内教程提示 |
| `CommonExplainPage` | 通用说明页 |
| `SortTutorialPage` | 排序教程引导 |

---

## 十二、关卡数据流转图

```
服务器
  └─ C2S_get_game_setting ──→ GameplayConfig（步数/奖励/广告参数）
                               FestivalConfig（活动时间）
                               DinoQuestConfig（活动窗口）
[Word Match_ Card Solitaire_1.3.5_功能介绍](Word Match_ Card Solitaire_1.3.5_功能介绍.md)
本地数据库（MessagePack格式）
  └─ TB_* 表 ──→ TB_LevelGoodsInfo（关卡商品）
                  TB_ItemData（道具属性）
                  TB_TaskData（任务配置）
                  TB_JourneyReward（里程碑奖励）
                  TB_ColorStreakStage（连胜阶段）

Unity Addressables（资产包）
  └─ word_assets_all.bundle ──→ 5,567个单词图标资产（t_word_*）
  └─ localization-*.bundle ──→ 13种语言文本表

关卡加载流程
  ├─ [主线关卡] ColorWordLoopLevelData ──→ ColorWordLevelGenerator.GenerateLevelData()
  │                                           ├─ 按 LoopID + RandomSeed 生成卡片布局
  │                                           └─ 按 Categories 抽取单词+图标
  │
  ├─ [每日挑战] ColorWordFixedLevelData ──→ SetupLevelByLevelData()
  │                                           └─ 按 FixedSeeds 确定性生成（全服一致）
  │
  └─ [运行时] GameplayLevelData ──→ GameplayEnvironment → ColorWordBoard
                                        ├─ OperateSlots 初始化
                                        ├─ PrepareSlot（牌库）初始化
                                        └─ TargetSlots 初始化
```

---

*文档生成时间：2026-03-17 | 分析来源：IL2CPP global-metadata.dat 逆向工程（Unity IL2CPP v31）*
