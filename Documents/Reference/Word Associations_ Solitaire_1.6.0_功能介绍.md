# Word Solitaire: Associations Game — 功能介绍

> **应用名称**：Word Solitaire: Associations Game（单词接龙：联想游戏）
> **包名**：com.youxi.wordsolitaire
> **版本**：1.6.0（版本号 111）
> **开发引擎**：Unity 6000.2.9f1
> **支持平台**：Android 6.0+（minSdk 23），目标 API 36
> **安装包大小**：约 162 MB（XAPK）

---

## 一、核心玩法

### 1.1 游戏类型
本作是一款将**纸牌接龙（Solitaire）**与**单词联想（Word Associations）**融合的益智卡牌游戏。玩家在接龙牌局框架下，通过匹配同类别的单词卡牌来完成关卡。

### 1.2 基本规则
- 场景中摆放若干**面朝上或面朝下**的单词卡牌（Tableau）。
- 每张卡牌正面印有一个**英语单词**及对应**图案**，属于某个特定主题类别（Category）。
- 玩家需将同类别的卡牌依次移至对应的**类别槽**（Category Slot）中。
- 当某列卡牌移走后，下方的隐藏牌将被翻开，为玩家创造新的操作空间。
- 牌面下方设有**备用牌堆（Stock）**，玩家可抽取备用牌补充手牌。
- 每个关卡设有**步数限制（movesLimit）**，玩家需在限定步数内完成关卡。

### 1.3 联想类别系统
关卡中的卡牌归属于特定主题类别，玩家须辨认单词之间的联想关系，将同类词汇归组。已识别的部分类别包括：

| 类别名称 | 示例单词 |
|----------|---------|
| Habitats（栖息地） | Forest、Tundra、Wetlands、Prairie、Aquarium、Marsh |
| Antonyms（反义词） | Hot/Cold、Tall/Short、Light/Dark、Fast/Slow |
| Africa（非洲地名） | Uganda、Chad、Algeria、Mongolia、Zambia、Ghana、Libya、Tunisia |
| Picnic（野餐） | 野餐相关词汇 |
| US State（美国州） | 美国各州名称 |
| Leader（领导者） | 领导者相关词汇 |
| Farming（农业） | 农业相关词汇 |
| Eats（饮食） | 饮食相关词汇 |
| Cave（洞穴） | 洞穴相关词汇 |
| Card Games（纸牌游戏） | Solitaire、Cribbage、Rummy、Bridge、Snap、Poker、Whist、Go Fish、Baccarat |
| Legumes（豆类） | 豆类植物词汇 |
| Teasers（脑筋急转弯） | 谜题相关词汇 |
| Sleuth（侦探） | 侦探推理词汇 |
| Bakery（烘焙） | 烘焙食品词汇 |
| Cartoon（卡通） | 动画卡通词汇 |
| Buddhism（佛教） | 佛教相关词汇 |
| Kinship（亲属关系） | 亲属称谓词汇 |
| Monkeys（猴子） | 灵长类相关词汇 |

---

## 二、关卡系统

### 2.1 关卡规模
- 全游戏共设有 **963 个关卡**，按编号 1—963 排列。
- 关卡数据分批加载，分为四个资源包：1–100、101–300、305–600、601–930。

### 2.2 关卡配置参数
每个关卡包含以下配置项：
- `isRandom`：是否启用随机布局
- `movesLimit`：最大操作步数（如 75 步）
- `categories`：本关卡涉及的单词类别（含图标配置）
- `slotsRewarded`：通关后奖励的拼图槽位数量
- `cardColumns`：牌列配置（布局方式）
- `StockDraw`：备用牌堆抽牌规则
- `JokerUse`：是否允许使用万能牌（Joker）

### 2.3 关卡难度进阶
- 初期关卡步数宽松，类别清晰；
- 高级关卡增加类别数量、缩减步数限制、引入随机布局；
- 支持无限制模式（Easy Victory）供休闲玩家使用。

---

## 三、卡牌图鉴系统

### 3.1 图鉴规模
游戏内置 **341 个图案大类**，每类包含多张图片卡牌，覆盖数千个独立单词。图案以精致的 PNG 插画呈现，每张卡牌均有专属图案辅助记忆。

### 3.2 图鉴主题分类（部分）
| 大类 | 内容示例 |
|------|---------|
| 动物类 | ANIMALS、BIRDS、FISH、INSECTS、REPTILES、MAMMALS、CATS、DOGS、FELINE、RODENTS、PRIMATES |
| 食物类 | FRUITS、BREADS、DESSERTS、PASTA、SEAFOOD、MEAT、VEGGIES、CANDY、DRINKS、EATS |
| 自然类 | FLOWERS、LEAVES、PLANTS、FUNGI、SEAWEED、ROCKS、SAND、RAIN |
| 交通类 | CARS、SHIPS、VEHICLES、TRANSPORT、FLIGHT、WHEELS |
| 运动类 | SPORTS、GYM、HIKING、PLAY |
| 服饰类 | CLOTHES、SHOES、HATS、JEWELRY、GARMENTS、DRESS |
| 建筑类 | HOMES、LANDMARK、BRIDGES、TEMPLES |
| 神话幻想类 | FANTASY、FOLKLORE、MONSTERS、VAMPIRE、CHIMERA |
| 符文 / 文化类 | RUNES、TAROT、SYMBOLS、ZODIAC |
| 科技类 | DEVICES、GADGET、KEYBOARD、DESKTOP、SCREEN |

---

## 四、拼图奖励系统

- 每关通关后，玩家可获得**拼图碎片**（Puzzle Piece）。
- 拼图碎片飞向屏幕顶部的**进度条**，逐步填满后解锁一幅完整的**旅行风景图**。
- 游戏内置多幅世界知名地标的拼图画作，共覆盖以下地点：

**亚洲**：日本富士山、日本京都寺庙、泰国、越南会安、韩国首尔、台湾夜市、印度泰姬陵、柬埔寨吴哥窟、土耳其卡帕多基亚热气球

**欧洲**：瑞士阿尔卑斯山野餐、法国巴黎春色（含蒙马特）、意大利地中海村庄 / 比萨斜塔 / 罗马斗兽场、英国伦敦、葡萄牙里斯本街景、德国慕尼黑啤酒节、荷兰阿姆斯特丹运河、爱尔兰巨人之路、乌克兰基辅秋夜、西班牙弗拉门戈

**美洲**：美国纽约曼哈顿 / 金门大桥 / 约塞米蒂 / 红杉林 / 落基山 / 大峡谷 / 马蹄湾、巴西里约热内卢、阿根廷伊瓜苏瀑布 / 巴塔哥尼亚、墨西哥、波多黎各、加拿大落基山

**非洲 / 中东**：埃及吉萨金字塔、摩洛哥舍夫沙万 / 马拉喀什香料集市

**大洋洲**：澳大利亚悉尼歌剧院

---

## 五、道具与助手系统

| 道具名称 | 功能说明 |
|---------|---------|
| **Hint（提示）** | 高亮显示下一步可操作的卡牌，帮助玩家找到出路 |
| **Undo（撤销）** | 撤回上一步操作，恢复牌局状态（含 Undo 2 连撤） |
| **Joker（万能牌）** | 作为任意类别的通配符，可放入任意类别槽 |
| **Coins（金币）** | 游戏货币，通过移牌操作、关卡通关、每日奖励等途径获取，用于购买道具 |

---

## 六、商城与付费系统

### 6.1 主题皮肤包（Shop Packs）
商城提供季节性主题卡牌皮肤包，已确认包含：
- **Christmas Pack**（圣诞节主题）
- **Cupid Pack**（情人节 / 丘比特主题）
- 更多季节性包陆续推出

### 6.2 内购内容
- 金币包：一次性购买不同数量的金币
- 道具包：含提示、撤销等道具的礼包
- **去广告（NoAds）**：一次性购买永久去除广告

### 6.3 广告系统
游戏集成多家广告平台，提供激励视频广告（可换取金币或道具）及插屏广告：
- Google AdMob
- AppLovin MAX
- Unity Ads
- Facebook Audience Network

---

## 七、每日系统

- **每日通知（Daily Notification）**：游戏通过推送通知提醒玩家每日回归，循环周期可配置（dayInCycle）。
- **每日奖励**：通过消息或图标提示当日特别奖励。
- 游戏支持**断线保存**，玩家返回后可从上次中断的位置继续游戏（"Let's pick up where we left off"）。

---

## 八、音效与沉浸体验

| 音效事件 | 对应场景 |
|---------|---------|
| `deal_cards` | 游戏开始发牌 |
| `card_in_hand` | 手持卡牌 |
| `category_card_in` | 卡牌归入类别槽 |
| `category_increment` | 类别进度提升 |
| `category_out` | 类别完成飞出 |
| `coin_from_move_spawn` | 移牌获得金币 |
| `take_stock_card` | 从备用堆抽牌 |
| `stock_cards_restore` | 备用堆补牌 |
| `hint` | 使用提示 |
| `undo_move` | 撤销操作 |
| `puzzle_piece_spawn/fly_end` | 拼图碎片产生 / 落定 |
| `puzzle_piece_reach_progress_bar` | 拼图进度增加 |
| `puzzle_complete` | 拼图完成 |
| `talking_card_voice` | 卡牌发声（长 / 短版） |
| `victory` | 通关胜利 |
| `fail` | 关卡失败 |
| `no_move_left` | 无可移动提示 |
| `out_of_move` | 步数耗尽 |
| `ad_break` | 广告间隙提示音 |
| `background_track` | 循环背景音乐 |

游戏还支持**触感反馈（Vibrate）**，在特定操作时产生震动。

---

## 九、教学系统（Tutorial）

- 游戏内置分阶段教学（TutorialStage / TutorialSequence），逐步引导新手完成：
  - 移动卡牌（MoveCard）
  - 使用备用牌堆
  - 理解类别归组逻辑
  - 使用提示与撤销道具
- 教学可配置是否阻断其他输入（`blockOtherInputs`）、是否自动推进（`autoAdvanceOnDrop`）。

---

## 十、技术特性

| 特性 | 说明 |
|------|------|
| 引擎 | Unity 6000.2.9f1（最新 LTS 版本） |
| 渲染 | OpenGL ES 3.0 / Vulkan（可选） |
| 资源加载 | Addressables 2.7.4 + Play Asset Delivery（按需下载） |
| 数据持久化 | Room 数据库 + DataStore（本地存档） |
| 网络 | OkHttp + Retrofit（网络请求） |
| 分析 | Firebase Analytics + Crashlytics + AppsFlyer |
| 账号系统 | Google Sign-In / Firebase Auth |
| 内购 | Google Play Billing 5.0.1 |
| 反盗版 | Google Play License Check + pairip |
| 架构 | armeabi-v7a（含独立 ABI 分包） |

---

## 十一、应用权限说明

| 权限 | 用途 |
|------|------|
| INTERNET | 联网加载资源、广告、分析数据上报 |
| VIBRATE | 触感反馈 |
| ACCESS_NETWORK_STATE | 检测网络连接状态 |
| WAKE_LOCK | 游戏过程中保持屏幕常亮 |
| POST_NOTIFICATIONS | 发送每日回归推送通知 |
| BILLING | 处理应用内购买 |
| AD_ID / ACCESS_ADSERVICES_* | 广告标识与归因追踪 |
| FOREGROUND_SERVICE | 后台资源下载服务 |

---

## 十二、特色亮点总结

1. **接龙 × 联想双核玩法**：将经典纸牌接龙的布局策略与单词联想的智识趣味有机融合，老少皆宜。
2. **海量关卡内容**：963 个精心设计的关卡，涵盖数十种主题类别，长期游玩不重样。
3. **丰富图鉴系统**：341 个图案大类、数千张精美手绘单词卡牌图案，视觉体验出色。
4. **旅行拼图奖励**：通关累积碎片，解锁世界各地地标风景画，兼具知识性与成就感。
5. **多元道具助手**：提示、撤销、万能牌三重辅助，适配不同难度偏好的玩家。
6. **沉浸式音效**：每个操作节点均有精心设计的音效反馈，配合背景音乐营造愉悦游戏氛围。
7. **离线可玩**：核心关卡内容本地存储，无需持续联网即可游戏。
8. **季节性内容更新**：定期推出主题皮肤包（如圣诞、情人节），保持新鲜感。
