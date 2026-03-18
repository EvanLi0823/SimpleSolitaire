# Solitaire Associations Journey 1.6.1 功能列表

## 基本信息

| 项目 | 内容 |
|------|------|
| 应用名称 | Solitaire Associations: Journey |
| 包名 | com.hitappsgames.wordsolitaire |
| 版本 | 1.6.1 (版本码 968) |
| 开发引擎 | Unity 6000.0.62f1 |
| 最低安卓版本 | Android 8.0 (API 26) |
| 目标安卓版本 | Android 15 (API 35) |
| 架构支持 | ARM64-v8a |

---

## 核心玩法

- **纸牌接龙 + 词语联想混合玩法**：将经典纸牌接龙与词语/单词关联机制结合
- **关卡进度系统**：包含关卡解锁、进度追踪（Game.Common.Level、Game.LevelSystemV2）
- **地图旅程模式**：多语言地图数据，玩家沿地图推进
- **多章节/世界设计**：游戏内容按章节分包下载（大量 AssetBundle 文件）

---

## 道具与助力系统

- **提示 (Hint)**：游戏中可使用的提示道具
- **撤销 (Undo)**：反悔上一步操作
- **加速道具/增益 (Boosters)**：多种强化道具（Game.Features.Boosters）
- **复活弹窗 (Revive)**：失败后可消耗资源复活继续游戏

---

## 连胜系统 (Win Streak)

- **连胜进度条**：连续获胜累积奖励
- **5 阶段宝箱奖励**：WS_chest1 ~ WS_chest5，难度递进的宝箱
- **连胜计时器**：限时完成关卡机制
- **奖励类型**：金币、提示道具、撤销道具

---

## 经济与商店系统

- **软货币（金币）系统**（Game.Features.SoftCurrency）
- **游戏内商店**（Game.Features.Shop）
- **内购商品列表**：

| 商品 ID | 类型 | 说明 |
|---------|------|------|
| smallcoinpack2 | 消耗品 | 小额金币包 |
| mediumcoinpack | 消耗品 | 中额金币包 |
| largecoinpack | 消耗品 | 大额金币包 |
| hugecoinpack | 消耗品 | 超大金币包 |
| megacoinpack | 消耗品 | 巨额金币包 |
| premiumcoinpack | 消耗品 | 高级金币包 |
| starterpack | 消耗品 | 新手礼包 |
| noadspack | 非消耗品 | 去广告礼包 |
| noads | 非消耗品 | 永久去广告 |

- **新手礼包/首购优惠**（Game.Features.StarterOffer）
- **付费墙系统**（Game.Services.Paywall）

---

## 广告系统

- **广告变现**：支持多广告网络（AppLovin MAX 聚合）
- **激励视频广告**（可换取道具/金币）
- **插页广告**
- **开屏广告**
- **横幅广告**
- **去广告购买选项**
- **接入广告平台**：AppLovin、HyprMX、Chartboost、InMobi、Yandex、Pangle(ByteDance)、BidMachine、Moloco、PubNative、BIGO Ads

---

## 通知与留存系统

- **本地推送通知**（Game.Services.LocalNotifications）
- **精准定时提醒**（SCHEDULE_EXACT_ALARM 权限）
- **直播运营/活动服务 (Live Ops)**（Game.Services.LiveOpsService）
- **用户分群/A-B 测试**（Game.Services.Segmentation）
- **好评弹窗引导**（Hitapps.Runtime.RateUs）
- **游戏更新提示弹窗**（Game.Features.UpdatePopup）

---

## 用户账号与社交

- **用户账号系统**（Game.Common.User、modules.hitapps-user-service）
- **后端登录/会话管理**（含 Watchdog 会话保活）
- **Facebook 账号登录**
- **Facebook 社交功能**：好友查找、分享、对话框
- **数据备份**（Game.Common.BackupFileStorage）

---

## 本地化与语言支持

- **10 种语言内容包**：

| 语言代码 | 语言 |
|----------|------|
| EN | 英语 |
| ZH | 中文 |
| DE | 德语 |
| ES | 西班牙语 |
| FR | 法语 |
| IT | 意大利语 |
| JA | 日语 |
| KO | 韩语 |
| PT | 葡萄牙语 |
| RU | 俄语 |

- **动态语言切换**（Game.Feature.ChangeLocale）
- **本地化系统**：Unity Localization 1.5.8

---

## 数据分析与归因

- **Firebase Analytics**（事件追踪）
- **AppsFlyer**（广告归因）
- **Facebook SDK Analytics**（用户行为）
- **自研分析系统**（analytics-core）
- **分析覆盖范围**：教程、关卡、商店购买、登录、广告、连胜、新手礼包等全链路埋点

---

## 技术特性

- **离线检测与提示**（Game.Features.NoInternet）
- **安全防篡改**（Game.Services.TamperGuard、Game.Services.Security.JsonGuard）
- **IAP 服务端验证**（Game.Services.IAPServerValidation）
- **振动反馈**（Haptic Feedback，Hitapps.Module.Input.Feel）
- **数据持久化**：PlayerPrefs + 文件存储双重方案
- **GDPR 合规**（用户同意/隐私管理，Game.Services.Gdpr）
- **崩溃上报**：Firebase Crashlytics
- **后台数据同步**（FOREGROUND_SERVICE_DATA_SYNC 权限）

---

## 音频

- **独立音效与背景音乐管理**（Game.Audio、Game.Services.Audio）

---

## 总结

这是一款由 **HitApps Games** 开发的**混合休闲益智游戏**，将纸牌接龙与词语联想机制结合，具备完整的商业化闭环（广告 + 内购 + 订阅），配有连胜奖励、道具助力、活动运营、多语言支持等典型休闲手游功能模块。
