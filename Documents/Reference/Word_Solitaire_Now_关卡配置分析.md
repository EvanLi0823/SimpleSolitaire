# Word Solitaire Now! 1.6.8 关卡配置深度分析

> 数据来源：APK 内部 Unity AssetBundle 资源文件（data.unity3d + Localization Bundle）
> 分析日期：2026-03-17

---

## 一、关卡文件总览

游戏所有关卡数据以加密二进制格式存储在主 AssetBundle（data.unity3d，51MB）中，共包含以下关卡配置文件：

### 1.1 每日挑战关卡（Daily Puzzles）

| 文件名 | 大小 | 估算天数 | 说明 |
|--------|------|----------|------|
| `daily_A` | 778,304 B（760 KB） | ~353 天 | 综合难度每日关卡（A版本） |
| `daily_B` | 778,352 B（760 KB） | ~353 天 | 综合难度每日关卡（B版本） |
| `daily_expert_A` | 777,232 B（759 KB） | ~353 天 | 专家难度每日关卡 |
| `daily_hard_A` | 777,984 B（760 KB） | ~353 天 | 困难难度每日关卡 |
| `daily_normal_A` | 773,760 B（756 KB） | ~351 天 | 普通难度每日关卡 |

**说明：**
- 每日关卡文件各包含约 **350+ 天**的独立关卡数据，可覆盖近一年的每日推送
- 游戏配置中指定使用 `DailyLevelConfig = "A"` 版本
- 每日推出**三档难度**（Normal / Hard / Expert），玩家自由选择

### 1.2 主线关卡（Word Solitaire / WordSota 模式）

| 文件名 | 大小 | 说明 |
|--------|------|------|
| `easy` | 4,992 B | 简单难度固定关卡集 |
| `normal` | 6,816 B | 普通难度固定关卡集 |
| `hard` | 8,848 B | 困难难度固定关卡集 |
| `fixed` | 19,872 B | 全难度综合固定关卡集（最大） |

**难度特征：**
- 简单（Easy）：使用 `EasyLevelConfig = "B"` 版本
- 普通（Normal）：使用 `NormalLevelConfig = "B"` 版本
- 困难（Hard）：使用 `HardLevelConfig = "C"` 版本
- `fixed` 文件约为 `hard` 的 2.25 倍，为高难度精选合集

### 1.3 循环关卡（Loop 模式 —— 无尽挑战）

| 文件 | 大小 | 说明 |
|------|------|------|
| `loop_easy` | 272 B | 简单循环主体 |
| `loop_easy_A/B/C/D` | 464～1,536 B | 简单循环四组扩展包 |
| `loop_normal` | 272 B | 普通循环主体 |
| `loop_normal_A/B/C/D` | 528～1,648 B | 普通循环四组扩展包 |
| `loop_hard` | 272 B | 困难循环主体 |
| `loop_hard_A/B/C/D` | 544～1,680 B | 困难循环四组扩展包 |

**说明：**
- 循环模式每档难度有 **1个主体 + 4个扩展包（A/B/C/D）**，共 5 组
- D 组数据最大（最丰富），A/B/C 组逐步增量
- 主体文件（272B）为基础参数，扩展包为具体关卡数据

### 1.4 蜘蛛纸牌关卡（WordSpider 模式）

| 文件名 | 大小 | 说明 |
|--------|------|------|
| `spider` | 5,520 B | 蜘蛛纸牌固定关卡集 |
| `spider_fixed` | 384 B | 蜘蛛精选固定关卡 |
| `spider_loop` | 608 B | 蜘蛛循环关卡 |

### 1.5 词汇分类关卡（WordCategory 模式）

| 文件名 | 大小 | 说明 |
|--------|------|------|
| `loop_category` | 647,168 B（632 KB） | 词汇分类循环关卡全集 |
| `wordcategory_loop` | 352 B | 词汇分类循环基础配置 |
| `words` | 10,000 B | 核心词汇数据集 |

---

## 二、词库规模分析

### 2.1 WordSota 模式词库（带插图词汇）

| 指标 | 数值 |
|------|------|
| 词汇图标总数 | **4,762 个** |
| 最短词汇 | 2 字符（如 AI、AK） |
| 最长词汇 | 45 字符（描述性短语） |
| 平均词汇长度 | 约 9～10 字符 |
| 词汇覆盖首字母 | A～Z 全覆盖 |

**词汇长度分布（去空格后）：**

| 长度区间 | 词汇数量 | 占比 |
|----------|----------|------|
| 3～5 字符 | 462 | 9.7% |
| 6～9 字符 | 1,575 | 33.1% |
| 10～13 字符 | 1,602 | 33.6% |
| 14～17 字符 | 789 | 16.6% |
| 18 字符以上 | 334 | 7.0% |

**典型词汇示例（按主题）：**

| 主题 | 代表词汇 |
|------|---------|
| 动物 | Afghan Hound, African Lion, Anglerfish, Ankylosaurus |
| 食物 | Affogato, Acoustic Guitar, Acrylic Paint Set |
| 历史文化 | Abraham Lincoln, Achaemenid Gold Bracelet, Angkor Wat |
| 科技 | 3D Glasses, 404 Error Page, Action Camera, Air Fryer |
| 地标建筑 | Arc de Triomphe, Allianz Arena, Ancient Temple |
| 艺术 | AcroYoga, Accordion, Acupressure Mat |

### 2.2 WordCategory 模式词库（分类词汇）

| 指标 | 数值 |
|------|------|
| 本地化词条总数（低ID区） | **33,498 条** |
| 估算分类总数 | 约 **3,000～3,300 个** |
| 每组平均词汇数 | 约 9～11 个 |
| 平均词汇长度 | 5.9 字符 |
| 最长词条 | 18 字符（Tortoise formation / Mythical Creatures） |

**词汇按首字母分布：**

| 字母 | 词条数 | 占比 |
|------|--------|------|
| S | 4,217 | 12.6% |
| C | 3,434 | 10.3% |
| P | 2,616 | 7.8% |
| B | 2,412 | 7.2% |
| T | 2,085 | 6.2% |
| M | 1,955 | 5.8% |
| A | 1,723 | 5.1% |
| F | 1,711 | 5.1% |
| R | 1,605 | 4.8% |
| D | 1,542 | 4.6% |
| 其余字母合计 | 9,198 | 27.5% |

### 2.3 词汇分类样例（WordCategory 模式）

以下为从本地化数据提取的分类结构示例（每个分类含 1 个标题 + 约 9 个成员词汇）：

| 分类标题（英/中） | 成员词汇示例 |
|-------------------|-------------|
| 2 Wheels / 两轮 | Segway, Bicycle, Scooter, Moped, Motorbike, BMX, Tandem, Dray, Chopper, Minibike |
| 24 Hours / 24小时 | Night, Evening, Morning, Noon, Dawn, Midday, Midnight, Twilight |
| 3D / 3D形状 | Cube, Sphere, Cone, Torus |
| 4 Wheels / 四轮 | Trolleybus, Ambulance, Truck, Skateboard, Quadbike, Cabriolet, Carriage, Babycarriage |
| 4-Legged / 四足动物 | Hippo, Warthog, Porcupine, Rhino, Fox, Lynx, Chipmunk, Deer |
| ABC / 字母系统 | Latin, Cyrillic, Runic, Braille, Hebrew, Arabic, Greek, Letters |
| Abdomen / 腹腔器官 | Liver, Spleen, Stomach, Kidneys, Bladder, Colon, Appendix, Pancreas |
| Ability / 能力动作 | Create, Dodge, Think, Remember, Catch, Follow, Push, Explore |
| Abnormal / 异常词汇 | Atypical, Unusual, Mutation, Deviant, Odd, Freak |
| Abode / 居住场所 | House, Duplex, Shack, Mansion, Bungalow, Lodge, Hut, Villa |
| Abstract / 抽象概念 | Idea, Concept, Notion, Theory, Truth, Beauty |
| Abyss / 深渊地形 | Chasm, Depth, Gap, Rift, Void, Crevasse, Hole, Pit, Cavern |
| Academia / 学术场景 | Lecture, Campus, Scholar, Tuition, Theory, Lesson, Library, Exam |
| Acid / 酸类化学 | Ascorbic, Citric, Carbonic, Boric, Nitric, Sulfuric, Gluconic, Acetic, Formic, Folic |
| Acrobat / 杂技动作 | Tumbling, Trapeze, Hoops, Agility, Kip, Backflip, Strength, Twisting |
| Achieve / 成就相关 | Triumph, Victory, Merit, Success, Win, Feat |
| Addicted / 成瘾行为 | Smoking, Shopping, Caffeine, Gambling, Alcohol, Internet |
| Adages / 格言相关 | Adage, Maxim, Axiom, Saw, Dictum, Motto, Saying, Proverb |
| Adverbs / 副词 | Loudly, Slowly, Easily, Rarely, Quickly, Happily, Silently, Quietly, Swiftly |
| Acronym / 首字母缩写 | NATO, RADAR, UNESCO, ASAP, BOGO, FOMO, FAQ, NASA, AI, AWOL, SWAT |

---

## 三、关卡系统核心配置

数据来源：`local_game_settings.bin`（JSON 格式，可直接读取）

### 3.1 难度解锁机制

| 难度档位 | 解锁条件 |
|----------|----------|
| Easy（简单） | 直接可玩（无需解锁） |
| Normal（普通） | 完成 **3 关** |
| Hard（困难） | 完成 **5 关** |
| 挑战模式 | 完成 **5 关** |
| 词汇助手功能 | 完成 **5 关** |
| 探索模式 | 游玩 **10 次** |

### 3.2 积分倍率系统

#### 基础难度倍率

| 难度 | 基础分数倍率 |
|------|-------------|
| Easy（简单） | ×2 |
| Normal（普通） | ×4 |
| Hard（困难） | ×7 |

#### 时间奖励加成

完成速度越快，可叠加额外积分奖励：

| 完成时间 | 说明 |
|----------|------|
| 3 分钟内（180 秒） | 最高速度奖励 |
| 8 分钟内（480 秒） | 中等速度奖励 |
| 15 分钟内（900 秒） | 入门速度奖励 |

#### 游戏模式倍率

| 模式 | 倍率 |
|------|------|
| 普通模式 | ×1 |
| 挑战模式 | ×1.5 |
| 拼图活动（Jigsaw） | ×1.5 |
| 今日挑战（Daily） | ×2 |
| 连击挑战 第 1 连 | ×2 |
| 连击挑战 第 2 连 | ×3 |
| 连击挑战 第 3 连 | ×4 |
| 连击挑战 第 4 连 | ×5 |
| 连击挑战 第 5 连 | ×6 |
| 连击挑战 第 6 连 | ×7 |
| 连击挑战 第 7 连及以上 | ×8（上限） |

**最大理论倍率：** 困难难度(×7) × 每日连击挑战(×8) = **×56**

### 3.3 道具与辅助系统

| 配置项 | 参数值 | 说明 |
|--------|--------|------|
| 魔法提示初始数量 | **2 个** | 新玩家初始持有 |
| 魔法移牌数量 | **3 张** | 每次使用可移动的牌数 |
| 广告奖励移牌数 | **10 张** | 观看广告可获得的额外移牌 |
| 连击挑战补救天数 | **60 天** | 错过后可补签的最大天数 |
| 智能模式默认 | 关闭 | 需手动开启 |
| 自动暂停默认 | 关闭 | 需手动开启 |
| 可重新发牌 | 是 | 支持重新洗牌 |
| 内容过滤默认 | 开启 | 过滤不适当词汇 |
| 匹配提示默认 | 开启 | 高亮可匹配的牌 |
| 即时输入模式 | 支持 | 快速直接输入词汇 |

### 3.4 初始主题解锁

玩家注册后默认解锁以下主题：

| 类型 | 解锁主题 |
|------|---------|
| 卡面（Front） | yellow0、red1 |
| 卡背（Back） | 主题 0、主题 1 |
| 桌面（Table） | 主题 0、主题 1 |

---

## 四、每日格言系统（Motto）

| 指标 | 数值 |
|------|------|
| 格言总条目 | **366 条**（含闰年 2月29日） |
| 每日 1 条 | 全年 365+1 天全覆盖 |
| 支持语言数 | **13 种语言** |

**支持的格言语言：**
英文、繁体中文、日文、西班牙文、德文、法文、意大利文、俄文、韩文、葡萄牙文、简体中文、荷兰文、土耳其文

**格言来源示例：**

| 日期 | 作者 | 内容（英文摘要） |
|------|------|----------------|
| 01-01 | New Year Greeting | Happy New Year! May this year bring you joy and success. |
| 01-02 | LAO TZU | When you realize nothing is lacking, the whole world belongs to you. |
| 01-03 | ROBERT R. McCAMMON | We all start out knowing magic. |
| 01-04 | HENRY VAN DYKE | Time is too slow for those who wait... |
| 01-05 | LISA KLEYPAS | The trick was forgetting about what had been lost... |
| 01-09 | LORETTA YOUNG | Love isn't something you find. Love is something that finds you. |

---

## 五、广告与变现配置

数据来源：`local_game_settings.bin` → `AdSetting`

| 配置项 | 参数值 | 说明 |
|--------|--------|------|
| 插页广告最短间隔 | **120 秒** | 两次插页广告之间的最短等待时间 |
| 新游戏后保护时间 | **600 秒**（10 分钟） | 新游戏开始后不显示插页 |
| 每小时最多插页数 | **10 次** | 3600 秒内上限 |
| 横幅广告开关 | 开启 | 默认显示横幅 |
| 横幅广告可关闭 | 是 | 玩家可手动关闭 |
| 每日免费无广告奖励 | **3 次** | 无需付费的每日免费次数 |
| 广告移除等待时间 | **8 秒** | 无广告模式等待间隔 |
| 无广告自动播放 | 是 | 重新填充后自动显示 |

**插页广告触发权重（不同场景）：**

| 场景 | 权重 |
|------|------|
| 普通游戏结束 | 100 |
| 新每日关卡 | 100 |
| 重玩每日关卡 | 50 |
| 新活动关卡 | 50 |
| 重新开始活动 | 50 |
| 奖励观看后 | -20（降低频率） |

**跳过广告的逐渐衰减权重：**

| 次序 | 权重 |
|------|------|
| 第 1 次跳过 | 250（高概率跳过） |
| 第 2 次跳过 | 150 |
| 第 3 次跳过 | 50 |
| 第 4 次跳过 | 0（不再跳过） |

---

## 六、节日活动配置

### 6.1 当前激活节日（v1.6.8）

| 名称 | 主题 | 开始时间 | 结束时间 | 解锁关卡 |
|------|------|----------|----------|----------|
| St. Patrick's Day 2026 | stpatrick | 2026-03-03 | 2026-03-18 | 第 5 关 |

### 6.2 历史节日（从 UI 资源推断）

游戏支持以下节日限定主题关卡（从资源文件路径推断）：

| 节日 | 资源文件 |
|------|---------|
| 圣诞节 | t_sd_christmas |
| 万圣节 | t_sd_halloween |
| 情人节 | t_sd_valentine |
| 元旦/新年 2025 | t_sd_newyear_2025 |
| 复活节 | t_sd_easter |
| 感恩节 | t_sd_thanksgiving |
| 儿童节（Kodomo） | t_sd_kodomo |
| 法国国庆节 | t_sd_french |
| 美国独立日 | t_sd_independence |
| 圣帕特里克节 | t_sd_patrick |
| 春季活动 2025 | t_sd_spring_2025 |
| 女儿节 | t_sd_daughter |

---

## 七、评分引导与成就触发节点

数据来源：`local_game_settings.bin` → `GameSetting`

### 7.1 关卡成就上报节点

当玩家达到以下关卡数时触发成就上报：

**3, 5, 8, 10, 13, 15, 18, 20, 25, 30, 35**

### 7.2 应用评分弹窗触发节点

当累计胜利次数达到以下任一值时，弹出评分引导：

**4, 9, 17, 28, 40, 100, 200, 266, 333, 500, 800, 1000, 1500, 2000**

### 7.3 广告追踪关键节点

在以下胜利次数节点进行广告效果追踪：

**30, 50, 90, 160, 280, 390, 510**

### 7.4 每日面板显示进度节点

当累计胜利次数达到以下值时，解锁每日面板的新内容展示：

**20, 40, 80, 150, 250, 350, 450**

---

## 八、游戏模式规则说明（从 UI 字符串提取）

### 8.1 WordSota 模式（单词纸牌）

- 目标：将所有卡牌移动到**基础区（Foundation）**
- 基础区每叠必须以**分类牌（Category Card）**开头
- 同类词汇卡牌叠放到对应分类牌上方
- 支持在牌列（Column）之间移动相同分类的卡牌
- 完成分类后，该叠牌消失，可开始新的叠牌

### 8.2 WordCategory 模式（词汇分类）

- 本模式**没有分类牌**
- 将所有同分类的词汇牌叠放在一起后**自动完成**
- 完成所有分类即过关
- 单牌模式：只能将牌移到**基础区**或**空列**，不能叠放到已有分类堆

### 8.3 魔法提示系统

- **显示解法技巧与区域**：高亮提示可能的操作方向
- **显示最终答案**：直接揭示谜题答案（消耗提示次数）
- 初始拥有 2 个魔法提示
- 可通过观看广告获得额外提示

### 8.4 每日连胜规则

- 每日完成挑战可维持连胜计数
- 连胜越高，积分倍率越高（最高 ×8）
- 错过一天会重置倍率（可使用补救功能，最多补 60 天）
- 配合困难难度最大可达 ×56 倍积分

---

## 九、关卡数量估算

> 注：关卡数据经加密处理，以下数据基于文件大小与加密块对齐推算

| 关卡类型 | 文件大小 | 估算关卡数 |
|----------|----------|------------|
| 每日挑战（每档） | ~778 KB | **约 350 天** |
| 每日挑战（5 档合计） | ~3.8 MB | **约 1,750 关** |
| 简单主线 | 4.9 KB | 约 48～52 关 |
| 普通主线 | 6.7 KB | 约 71 关 |
| 困难主线 | 8.6 KB | 约 56～79 关 |
| 综合固定关卡 | 19.4 KB | 约 96～138 关 |
| 蜘蛛纸牌 | 5.4 KB | 约 48～55 关 |
| 词汇分类循环 | 632 KB | 大量循环关卡 |
| 循环简单（5 组合计） | ~3.5 KB | 多批次循环 |
| 循环普通（5 组合计） | ~4.5 KB | 多批次循环 |
| 循环困难（5 组合计） | ~4.7 KB | 多批次循环 |

---

## 十、技术实现细节

| 项目 | 说明 |
|------|------|
| 资源格式 | Unity AssetBundle（UnityFS，LZ4HC 压缩） |
| 关卡数据编码 | 自定义加密（高熵 ~8.0 bits/byte，16 字节对齐，疑似 AES） |
| 本地化格式 | Unity Localization Package StringTable（MonoBehaviour） |
| 游戏配置格式 | 明文 JSON（`local_game_settings.bin`） |
| 每日格言格式 | 明文 JSON（`motto.bin`），按日期键值组织 |
| 计费配置 | 明文 JSON（`BillingMode.bin`）：`{"androidStore":"GooglePlay"}` |
| 词汇图标 | Texture2D + Sprite，共 4,762 组，存储于独立 Bundle |
| 拼图图片 | 独立 Bundle（jigsawactivity），共 24 张明信片 |

---

## 附：关键 UI 字符串对照表（英中）

| 英文 | 中文 |
|------|------|
| Easy Level {0} | 简单关卡 {0} |
| Medium Level {0} | 中等关卡 {0} |
| Hard Level {0} | 困难关卡 {0} |
| Daily Challenge | 每日挑战 |
| Daily Streak | 每日连胜 |
| Win Streak | 连胜纪录 |
| Total Score | 总得分 |
| Perfect Wins | 完美胜利 |
| Magic Hint | 魔法提示 |
| Word Assistant | 单词助手 |
| Explore | 探索 |
| Challenge Trophies | 挑战奖杯 |
| Explore Collections | 活动藏品 |
| Crazy Challenge | 疯狂挑战 |
| Triple the Challenge | 三重难度，随心挑战 |
| Complete {0} {1} levels to unlock | 完成{0}个{1}关卡以解锁 |
| Only {0}% of players solved this on first try | 只有{0}%的玩家能在首次闯关解开这个谜题 |
| Keep Daily Streak to maintain the points multiplier! | 保持每日连胜以维持积分倍数！ |
| Watch a quick video to unlock banner-free 24h | 观看短视频，即可解锁无广告体验24小时！ |
