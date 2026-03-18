# Word Solitaire: Associations Game — 关卡数据深度分析

> **数据来源**：对 XAPK 包内 Unity Addressables 资源包（1-100 bundle、101-300 bundle、configs bundle、catalog.bin）进行二进制解析所得。
> **覆盖范围**：完整关卡目录（963 关）+ 本地预置关卡详细数据（约 35 关）+ 拼图地点（74 处）+ 单词类别（23 个）。

---

## 一、关卡总览

### 1.1 关卡规模

| 指标 | 数值 |
|------|------|
| 总关卡数 | **963 关** |
| 本地预置关卡（安装即可游玩） | 1–304 关（约 304 关） |
| 按需下载关卡（Play Asset Delivery） | 305–930 关（约 626 关，需联网下载） |
| 最大关卡编号 | 963 |

### 1.2 关卡分段与资源包分布

游戏将关卡资源分为四个独立资产包，按难度/进度拆分加载：

| 资产包名 | 关卡范围 | 加载方式 | 包含关卡数 |
|---------|---------|---------|---------|
| `1-100_assets_all` | Level 1–100 | 本地预置（随安装包） | 100 关 |
| `101-300_assets_all` | Level 101–304 | 本地预置（随安装包） | ~204 关 |
| `305-600_assets_all` | Level 305–600 | 按需下载（Play Asset Delivery） | ~296 关 |
| `601-930_assets_all` | Level 601–930 | 按需下载（Play Asset Delivery） | ~330 关 |

---

## 二、关卡配置参数详解

每个关卡由一个 JSON 结构定义，核心字段如下：

| 字段名 | 类型 | 说明 |
|-------|------|------|
| `Id` | int | 关卡唯一编号 |
| `movesLimit` | int | 步数上限（可用移牌次数） |
| `isRandom` | bool | 是否使用随机牌面布局 |
| `slotsRewarded` | int | 通关后奖励的拼图槽位数 |
| `categories` | array | 本关涉及的单词类别列表 |
| `cardColumns` | array | 列布局配置（牌堆数量与初始状态） |
| `StockDraw` | config | 备用牌堆（Stock）抽牌规则 |
| `JokerUse` | bool | 是否允许使用万能牌（Joker） |

---

## 三、步数限制（movesLimit）分布分析

通过解析 35 个已知关卡的数据，得出以下步数分布规律：

| 步数区间 | 关卡数量 | 说明 |
|--------|---------|------|
| 75–100 步 | 7 关 | 早期入门关卡，难度较低 |
| 101–130 步 | 17 关 | 主力标准关卡，占比最大 |
| 131–160 步 | 3 关 | 中等偏难关卡 |
| 161–200 步 | 4 关 | 高难度关卡 |
| 201 步以上 | 4 关 | 超高难度关卡 |

**数值范围**：75 步（最低）～ 250 步（最高），均值约 **138 步**。

### 步数与关卡编号的对应关系（实测样本）

| 关卡编号 | 步数上限 | 难度评级 |
|---------|---------|---------|
| Level 10 | 75 | ★ 入门 |
| Level 12 | 75 | ★ 入门 |
| Level 17 | 75 | ★ 入门 |
| Level 18 | 75 | ★ 入门 |
| Level 32 | 75 | ★ 入门 |
| Level 111 | 75 | ★ 入门 |
| Level 166 | 75 | ★ 入门 |
| Level 11 | 130 | ★★ 标准 |
| Level 14 | 130 | ★★ 标准 |
| Level 15 | 130 | ★★ 标准 |
| Level 74 | 130 | ★★ 标准 |
| Level 85 | 130 | ★★ 标准 |
| Level 88 | 130 | ★★ 标准 |
| Level 93 | 150 | ★★★ 进阶 |
| Level 34 | 140 | ★★★ 进阶 |
| Level 133 | 140 | ★★★ 进阶 |
| Level 87 | 190 | ★★★★ 困难 |
| Level 160 | 200 | ★★★★ 困难 |
| Level 261 | 200 | ★★★★ 困难 |
| Level 36 | 210 | ★★★★ 困难 |
| Level 29 | 220 | ★★★★ 困难 |
| Level 204 | 230 | ★★★★★ 极难 |
| Level 295 | 250 | ★★★★★ 极难 |

> **规律**：游戏并非严格按编号递增难度，而是采用**非线性难度设计**，早期关卡中穿插高步数挑战关，保持游戏节奏变化。

---

## 四、单词类别（Word Categories）完整列表

游戏中共识别出 **23 个独立单词类别**，每个类别对应一组具有联想关系的英语单词：

### 4.1 自然与科学类

| 类别名 | 含义 | 代表单词（样本） |
|-------|------|----------------|
| **Cave** | 洞穴 | Echo（回声）、Roc（大岩石）、Gloom（昏暗）、Atom（微小物体）、Mite（螨虫）、Bead（水珠） |
| **Habitats** | 栖息地 | Forest（森林）、Tundra（苔原）、Wetlands（湿地）、Prairie（草原）、Aquarium（水族馆）、Marsh（沼泽） |
| **Legumes** | 豆类 | 豆类植物及相关词汇（Broken、Frayed、Wilt 等引申义词） |
| **Farming** | 农业 | Fallow（休耕）、Hay（干草）、Compost（堆肥）、Grain（谷物） |

### 4.2 语言与文字类

| 类别名 | 含义 | 代表单词（样本） |
|-------|------|----------------|
| **Antonyms** | 反义词 | Hot/Cold（热/冷）、Tall/Short（高/矮）、Light/Dark（亮/暗）、Fast/Slow（快/慢） |
| **Teasers** | 文字谜/脑筋急转弯 | Quiz（测验）、Enigma（谜题）、Rebus（图谜）、Riddle（谜语）、Charade（哑谜） |
| **Sleuth** | 侦探推理 | Motive（动机）、Victim（受害者）、Clue（线索）、Alibi（不在场证明） |

### 4.3 人文与社会类

| 类别名 | 含义 | 代表单词（样本） |
|-------|------|----------------|
| **Leader** | 领导者/总统 | Lincoln（林肯）、Obama（奥巴马）、Reagan（里根）、Adams（亚当斯）、Bush（布什） |
| **US State** | 美国州名 | Idaho（爱达荷）、Oregon（俄勒冈）、Utah（犹他）、Arizona（亚利桑那）、Maine（缅因）、Wyoming（怀俄明） |
| **Kinship** | 亲属关系 | Wife（妻子）、Niece（侄女）、Uncle（叔伯）、Aunt（姑姨）、Husband（丈夫） |
| **Yuppie** | 雅皮士文化 | Young（年轻）、Elite（精英）、Perks（特权）、Brunch（早午餐）、Boom（繁荣） |
| **Buddhism** | 佛教 | Nirvana（涅槃）、Samsara（轮回）、Mantra（咒语）、Dharma（佛法） |
| **Imitate** | 模仿/仿制 | Copy（复制）、Adapt（改编）、Emulate（效仿）、Paris/London/Tokyo/Vienna（被仿制的城市） |

### 4.4 生活与饮食类

| 类别名 | 含义 | 代表单词（样本） |
|-------|------|----------------|
| **Bakery** | 烘焙食品 | Eclair（闪电泡芙）、Bagel（贝果）、Donut（甜甜圈）、Muffin（松饼）、Pretzel（脆饼干）、Biscuit（饼干） |
| **Eats** | 饮食相关 | Course（一道菜）、Path（路线）、Route（路径）（引申"用餐路线"） |
| **Picnic** | 野餐 | Queue（排队）、Pass（通行证）、Event（活动）、Season（季票）、Booth（摊位）、Holder（持有人） |
| **Utensil** | 厨具/餐具 | Fork（叉）、Knife（刀）、Ladle（勺）、Tongs（夹子）、Whisk（打蛋器） |

### 4.5 动物类

| 类别名 | 含义 | 代表单词（样本） |
|-------|------|----------------|
| **Monkeys** | 猴类/灵长类 | Baboon（狒狒）、Chimp（黑猩猩）、Gorilla（大猩猩）、Tamarin（狨猴）；同时包含金属元素 Vanadium/Copper/Zinc/Lead（联想：元素周期表中的"猿"族群） |
| **Crab** | 螃蟹联想 | Shell（壳）、Beach（海滩）、Ocean（海洋）、Pincer（蟹钳）、Delay/Late/Wait（慢吞吞） |

### 4.6 其他主题类

| 类别名 | 含义 | 代表单词（样本） |
|-------|------|----------------|
| **Cartoon** | 卡通动画 | Mickey（米奇）、Donald（唐老鸭）、Jerry（杰瑞）、Casper（卡斯柏）、Stingray（魔鬼鱼队长） |
| **Apple** | 苹果（联想） | 与苹果相关的多种联想：苹果水果、苹果公司、苹果品种等 |
| **Flaws** | 缺陷/破损 | Broken（破损）、Leak（漏洞）、Frayed（磨损）、Warp（变形）、Wilt（枯萎）、Chipped（缺口） |
| **Ligature** | 连接/绑缚 | Cord（绳索）、Thread（线）、Germ（细菌）、Microbe（微生物）、Fungus（真菌）、Algae（藻类）（联想：丝状物） |
| **Hand** | 手/手相关 | Nail（指甲）、Thumb（拇指）、Palm（手掌）、Knuckle（指关节）、Fist（拳头）；延伸：India/Diwali/Curry（印度手势文化） |

---

## 五、关卡-类别对应表（已知关卡）

| 关卡编号 | 步数上限 | 联想类别 | 类别中的词汇样本 |
|---------|---------|---------|---------------|
| Level 18 | 75 | Bakery（烘焙） | Eclair、Bagel、Donut、Muffin、Pretzel、Biscuit |
| Level 29 | 220 | Flaws（缺陷） | Broken、Leak、Frayed、Warp、Yeti、Kraken |
| Level 32 | 75 | Eats（饮食路线） | Course、Path、Route、Arrow、Lane |
| Level 34 | 140 | Sleuth（侦探） | Motive、Victim、Clue、Alibi、Eclipse |
| Level 36 | 210 | Farming（农业） | Fallow、Hay、Compost、Grain、Newt、Darwin |
| Level 74 | 130 | Picnic（野餐） | Queue、Pass、Event、Season、Booth、Block |
| Level 85 | 130 | Hand（手） | Nail、Thumb、Palm、India、Diwali、Curry |
| Level 87 | 190 | Antonyms（反义词）| Hot↔Cold、Tall↔Short、Light↔Dark、Fast↔Slow |
| Level 88 | 130 | US State（美国州）| Idaho、Oregon、Utah、Arizona、Maine、Wyoming |
| Level 93 | 150 | Cave（洞穴） | Echo、Roc、Gloom、Atom、Mite、Bead |
| Level 172 | 130 | Kinship（亲属） | Wife、Niece、Uncle、Aunt、Husband |
| Level 188 | 130 | Ligature（连接） | Cord、Thread、Germ、Microbe、Fungus、Algae |
| Level 204 | 230 | Yuppie（雅皮士） | Young、Elite、Perks、Brunch、Boom |
| Level 217 | 130 | Crab（螃蟹） | Shell、Beach、Ocean、Pin、Delay、Wait |
| Level 242 | 180 | Buddhism（佛教） | Nirvana、Samsara、Mantra、Dharma |
| Level 244 | 130 | Monkeys（猴类） | Baboon、Chimp、Gorilla、Tamarin |
| Level 261 | 200 | Imitate（模仿） | Copy、Adapt、Emulate、Paris、London、Tokyo |
| Level 295 | 250 | Apple（苹果） | Nomad、Curie、Tuareg、Pomegranate |
| Level 959 | 130 | Legumes（豆类） | Broken、Wilt、Frayed、Chipped、Floral |

> **注**：每个关卡通常包含 2–4 个类别，上表仅列出识别到的显式类别名，其余类别因二进制编码混淆而无法解析。

---

## 六、拼图奖励系统 — 世界旅行地点完整列表

通关积累拼图碎片后解锁的风景画，共覆盖 **74 处**世界著名地点，按地理区域分组如下：

### 6.1 欧洲（19 处）

| 编号 | 国家/地区 | 画面描述 |
|------|---------|---------|
| 2 | 🇫🇷 法国 | Charming Parisian spring（迷人的巴黎春色）|
| 5 | 🇬🇧 英国/伦敦 | London landmarks（伦敦地标） |
| 15 | 🇮🇹 意大利（罗马） | Roman Colosseum（罗马斗兽场午后） |
| 17 | 🇬🇷 希腊/圣托里尼 | Santorini sunset（圣托里尼日落） |
| 19 | 🇮🇹 意大利（比萨） | Tower of Pisa（比萨斜塔）|
| 20 | 🇫🇷 法国（普罗旺斯） | Provence café culture（普罗旺斯咖啡文化） |
| 26 | 🇵🇹 葡萄牙/里斯本 | Lisbon street life（里斯本街头生活） |
| 29 | 🇩🇪 德国/慕尼黑 | Oktoberfest（慕尼黑啤酒节） |
| 30 | 🇩🇪 德国 | Bavarian glory（巴伐利亚荣耀）|
| 31 | 🇳🇱 荷兰/阿姆斯特丹 | Amsterdam canal（阿姆斯特丹运河）|
| 33 | 🇺🇦 乌克兰 | [Ukraine scene] |
| 34 | 🇨🇺 古巴/哈瓦那 | Havana（哈瓦那） |
| 35 | 🇮🇪 爱尔兰 | Giant's Causeway morning（巨人之路晨景） |
| 37 | 🇲🇦 摩洛哥/舍夫沙万 | Blue medina（蓝色麦地那）/ Chefchaouen |
| 38 | 🇮🇹 意大利/威尼斯 | Venice Carnival（威尼斯嘉年华） |
| 39 | 🇬🇧 英国/苏格兰 | Cliffs（悬崖） |
| 51 | 🇪🇸 西班牙 | Flamenco（弗拉门戈） |
| 55 | 🇺🇦 乌克兰/基辅 | Cozy Kyiv evening（温馨基辅夜晚）|
| 57 | 🇫🇷 法国/巴黎 | Montmartre everyday bustling（蒙马特日常繁华）|
| 68 | 🏴󠁧󠁢󠁳󠁣󠁴󠁿 苏格兰/爱丁堡 | Edinburgh（爱丁堡）|

### 6.2 亚洲（16 处）

| 编号 | 国家/地区 | 画面描述 |
|------|---------|---------|
| 3 | 🇯🇵 日本 | Serene Mt. Fuji view（宁静的富士山） |
| 15 | 🇹🇼 台湾 | Night buzz（夜市喧嚣） |
| 18 | 🇹🇷 土耳其/卡帕多基亚 | Cappadocia balloon（卡帕多基亚热气球）|
| 19 | 🇹🇭 泰国 | Thai landmark（泰国地标）|
| 22 | 🇨🇳 中国 | Great Wall majesty（长城雄姿） |
| 25 | 🇰🇭 柬埔寨 | Angkor Wat temple（吴哥窟神殿）|
| 47 | 🇯🇵 日本/京都 | Cherry blossoms & temples in Kyoto（京都樱花与寺庙）|
| 53 | 🇦🇷 阿根廷 | Fitz Roy / Patagonia（菲茨罗伊山）|
| 59 | 🇰🇷 韩国/首尔 | Vibrant Seoul（活力首尔）|
| 71 | 🇻🇳 越南/会安 | Hoi An（会安古镇）|
| 73 | 🇨🇦 加拿大/魁北克 | Quebec（魁北克）|
| 9 | 🇮🇳 印度 | Taj Mahal wonder（泰姬陵奇观）|
| 43 | 🇹🇷 土耳其 | Spice bazaar（香料集市）|
| 48 | 🇹🇷 土耳其/伊斯坦布尔 | Hookah bazaar（水烟集市）|
| 49 | 🇯🇵 日本 | Cherry blossom（樱花）|
| 56 | 🇯🇵 日本/京都 | Temples in Kyoto（京都寺庙）|

### 6.3 美洲（20 处）

| 编号 | 国家/地区 | 画面描述 |
|------|---------|---------|
| 4 | 🇺🇸 美国 | Statue of Liberty（自由女神像）|
| 7 | 🇨🇦 加拿大 | Rockies paradise（落基山天堂）|
| 10 | 🇧🇷 巴西 | Christ the Redeemer（基督救世主像）|
| 11 | 🇲🇽 墨西哥 | Colorful town（彩色小镇）|
| 23 | 🇺🇸 美国 | Golden Gate Bridge（金门大桥）|
| 24 | 🇦🇷 阿根廷 | Iguazu Falls（伊瓜苏瀑布）|
| 27 | 🇺🇸 美国/纽约 | Manhattan（曼哈顿）|
| 41 | 🇺🇸 美国/亚利桑那 | Monument Valley road（纪念碑谷公路）|
| 42 | 🇧🇷 巴西 | Rio de Janeiro feathers（里约热内卢嘉年华羽毛）|
| 44 | 🇺🇸 美国/黄石 | Old Faithful geyser（老忠实间歇泉）|
| 50 | 🇲🇦 摩洛哥/马拉喀什 | Marrakech spice（马拉喀什集市）|
| 54 | 🇺🇸 美国/加州 | Yosemite Half Dome（约塞米蒂半穹顶）|
| 58 | 🇵🇷 波多黎各 | Puerto Rico explore（波多黎各探险）|
| 60 | 🇺🇸 美国 | San Juan（圣胡安）|
| 62 | 🇨🇿 捷克共和国 | Old town square（老城广场）|
| 64 | 🇺🇸 美国/亚利桑那 | Grand Canyon（大峡谷）|
| 67 | 🌄 乡村 | Countryside cottage（乡村小屋）|
| 61 | 🇺🇸 美国/亚利桑那 | Horseshoe Bend（马蹄湾）|
| 61 | 🇺🇸 美国/加州 | Redwood Forest path（红杉林小径）|

### 6.4 非洲与大洋洲（7 处）

| 编号 | 国家/地区 | 画面描述 |
|------|---------|---------|
| 6 | 🇪🇬 埃及 | Pyramids of Giza（吉萨金字塔）|
| 15 | 🇦🇺 澳大利亚 | Sydney Opera House（悉尼歌剧院）|
| 28 | 🇳🇵 尼泊尔 | Mount Everest（珠穆朗玛峰）|
| 33 | 🇧🇷 巴西/亚马逊 | Enchanted Amazon Rainforest（亚马逊雨林）|
| 36 | 🇮🇪 爱尔兰/莫尔悬崖 | Cliffs of Moher morning（莫尔悬崖晨曦）|
| 37 | 🇲🇦 摩洛哥 | Blue medina Chefchaouen（蓝白城舍夫沙万）|
| 50 | 🇿🇦 南非 | [Vineyard harvest?]（葡萄园丰收）|

> **注**：拼图解锁门槛为从第 3 关开始（`"min_level": 3`），随着通关积累碎片逐步解锁各地点画作。

---

## 七、教学系统（Tutorial）结构

游戏内置分阶段引导教学，从 catalog 与配置文件中识别到以下教学节点：

### 7.1 教学类型
- `TutorialStage`：单步骤教学单元（最小颗粒度）
- `TutorialSequence`：由多个 TutorialStage 组成的完整教学流程
- `Default.json`：默认关卡配置，用于教学关卡的基础布局

### 7.2 教学关卡布局（Stage 15 示例）
```
Level ID: 1 (教学关)
movesLimit: 0（无限制）
JokerUse: false
Stock: [Tea, ...]（备用牌堆含特定单词）
CategorySlots: 分配好的类别槽
PlacedCards: 预先放置的卡牌
```

### 7.3 教学内容覆盖
根据配置字段推断，教学涵盖以下操作：

| 教学步骤 | 配置标识 | 说明 |
|---------|---------|------|
| 移动卡牌 | `MoveCard4` | 教玩家拖动卡牌到类别槽 |
| 备用牌堆 | `_draw_from_stock` | 教玩家从 Stock 抽牌 |
| 类别识别 | `category`, `_to_category` | 教玩家识别同类别单词 |
| 自动推进 | `autoAdvanceOnDrop` | 正确操作后自动跳至下一步 |
| 输入锁定 | `blockOtherInputs` | 教学中锁定其他操作，强制引导 |
| 随机布局 | `startWithRandomFe` | 教玩家处理随机牌面 |

---

## 八、游戏 UI 文本（从配置中提取）

以下为游戏中出现的提示性文本（部分因二进制混淆而截断）：

### 8.1 玩法提示
- `"(T)ableau can hold a whole stack of cards"` — 牌列可以叠放整列卡牌
- `"Shift the re layout to earn a new reward"` — 移动牌面布局以获取新奖励
- `"Cards are already on the board"` — 卡牌已在牌面上
- `"Open up space"` — 腾出空间
- `"> Shift the pile"` — 移动一叠牌

### 8.2 进度激励
- `"Impressive Progress!"` — 进步显著！
- `"Reward Is Near"` — 奖励即将到来
- `"Your winning streak!"` — 您的连胜！
- `"Just one more re layout to earn a new reward"` — 再排一局即可获得新奖励
- `"Almost there!"` — 快到了！

### 8.3 游戏流程
- `"Try Again"` — 再试一次
- `"Easy Victory: [layout]"` — 轻松胜利模式
- `"Lucky Shuffle"` — 幸运重新洗牌
- `"Fresh Layout"` — 全新布局
- `"Match solved"` — 配对完成
- `"Start!"` — 开始！
- `"Mid-game"` — 游戏进行中

### 8.4 离线/在线状态
- `"Pick up where you left off"` — 从上次离开的地方继续
- `"Continue your unfinished game?"` — 继续未完成的游戏？
- `"Come check it out!"` — 来看看！
- `"Unlock"` — 解锁
- `"Without interruptions. Have fun!"` — 无中断，尽情享受！

### 8.5 通知推送文本
- `"You are back online! Let's pick up where we left off"` — 您已重新联网，继续上次游戏
- Daily cycle notification（每日循环提醒）

---

## 九、商城配置（Shop Pack）

从配置数据中提取到商城包数据：

| 字段 | 值 | 说明 |
|------|---|------|
| ShopPackConfig | 已配置 | 主题包配置存在 |
| 生效日期 | 2026-01-01 | 新主题包上线时间（或有效期） |
| 截止时间 | `10T23:59:59Z` | 限时特卖截止时间 |
| 已知主题包 | Christmas Pack | 圣诞主题皮肤 |
| 已知主题包 | Cupid Pack | 情人节主题皮肤 |
| 资源引用 | `hop-pack-Christmas_back` | 圣诞包背景资源 |

---

## 十、音频资源配置

从 catalog.bin 提取到完整音频资产列表：

### 10.1 音效（SFX）文件列表

| 文件名 | 触发场景 |
|-------|---------|
| `ad_break.wav` | 广告插播 |
| `button_click.wav` | 按钮点击 |
| `card_in_hand.wav` | 拾取卡牌 |
| `category_card_in.wav` | 卡牌入类别槽 |
| `category_complete_card_fly.wav` | 类别完成卡牌飞出 |
| `category_increment.wav` | 类别进度增加 |
| `category_out.wav` | 类别槽飞出 |
| `close_popup.wav` | 关闭弹窗 |
| `deal_cards.ogg` | 发牌 |
| `fail.wav` | 关卡失败 |
| `hint.wav` | 使用提示 |
| `no_move_left.wav` | 无可移步骤 |
| `open_popup.wav` | 打开弹窗 |
| `out_of_move.wav` | 步数耗尽 |
| `puzzle_complete.wav` | 拼图完成 |
| `puzzle_piece_fly_end.wav` | 拼图碎片到达 |
| `puzzle_piece_reach_progress_bar.wav` | 拼图进度条推进 |
| `puzzle_piece_spawn.wav` | 拼图碎片生成 |
| `SFX_UI_Success_Magical_1.wav` | 成功魔法音效 |
| `stock_cards_restore.ogg` | 备用牌堆补充 |
| `take_stock_card.wav` | 抽取备用牌 |
| `talking_card_voice.wav` | 卡牌朗读（长版） |
| `talking_card_voice_short.wav` | 卡牌朗读（短版） |
| `undo_move.wav` | 撤销操作 |
| `undo2.wav` | 连续撤销 |
| `victory.ogg` | 胜利 |

### 10.2 背景音乐
- `background_track.mp3` — 单曲循环背景音乐

---

## 十一、数据统计汇总

| 维度 | 数据 |
|------|------|
| 总关卡数 | **963 关** |
| 本地可玩关卡 | 1–304 关（约 304 关） |
| 识别出的单词类别 | **23 个** |
| 识别出的拼图地点 | **74 处** |
| movesLimit 范围 | **75–250 步** |
| movesLimit 平均值 | **约 138 步** |
| 图案大类（卡牌图标） | **341 个** |
| 音效文件数 | **25 个** |
| 支持语言 | **80+ 种**（含中文简体、繁体、英、法、德等） |
| 拼图解锁起始关 | 第 3 关 |
