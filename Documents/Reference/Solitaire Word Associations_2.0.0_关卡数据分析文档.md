# Solitaire Word Associations 2.0.0 关卡数据深度分析

> 数据来源：APK 内嵌 Unity TextAsset（UnityPy 解包），包含全量关卡 JSON + 本地化文本
> 分析日期：2026-03-17

---

## 一、关卡总览

| 指标 | 数据 |
|------|------|
| 正式关卡总数 | **999 关**（ID 2 ~ 1000） |
| 教程关卡数 | **2 关**（ID 0, 1，步数上限 999 / 无限制） |
| 关卡数据文件 | `levels_100` ~ `levels_1000`（每文件约 100 关） |
| 词语分类总数 | **5,550 个** |
| 关键词（单词牌）总数 | **42,811 个** |
| 支持语言 | 英语（en-US）、土耳其语（tr-TR），各按 100 关一包分批加载 |

---

## 二、关卡数据结构说明

每个关卡 JSON 对象字段如下：

```json
{
  "id": 2,                         // 关卡编号（2~1000）
  "moveCount": 70,                 // 最大允许步数
  "difficulty": "easy",            // 难度："easy" / "medium" / "hard"
  "foundationCount": 3,            // 基础堆数量（目标堆 = 分类数）
  "hasCategoryImages": {           // 各分类是否有配图
    "colors": false,
    "insects": true
  },
  "categoryKeywordCounts": {       // 各分类含多少张关键词牌
    "colors": 6,
    "insects": 6
  },
  "patterns": [{                   // 牌局布局（目前始终只有 1 个 pattern）
    "stockCards": [...],           // 发牌堆中的牌
    "columnCards": [[...], [...]]  // 各列的牌（从底到顶）
  }]
}
```

### 牌（Card）对象字段

```json
{
  "categoryName": "insects",   // 所属分类 ID
  "keyword": "ladybug",        // 关键词 ID（标题牌无此字段）
  "isTitleCard": false         // true = 分类标题牌（占一个基础堆位置）
}
```

**牌的类型：**
- **关键词牌（Keyword Card）**：`isTitleCard: false`，显示具体单词（如 "Ladybug"），需要归入对应分类的基础堆。
- **标题牌（Title Card / Category Card）**：`isTitleCard: true`，代表一个分类，只能放入空基础堆，是该分类关键词牌的归宿锚点。

---

## 三、难度体系

### 3.1 三档难度分布

| 难度 | 关卡数 | 占比 |
|------|--------|------|
| Easy（简单） | **200 关** | 20% |
| Medium（中等） | **600 关** | 60% |
| Hard（困难） | **199 关** | 约 20% |

### 3.2 各难度参数对比

| 参数 | Easy | Medium | Hard |
|------|------|--------|------|
| 最大步数（moveCount）| 70 ~ 75 | 108 ~ 134 | 156 ~ 230 |
| 平均步数 | **71** | **123** | **202** |
| 基础堆数量 | 全部 **3** 个 | 全部 **4** 个 | 全部 **4~5** 个 |
| 分类数（categoryKeywordCounts） | **6** 个 | **10** 个 | **12 / 14 / 15** 个 |
| 典型列数 | **3** 列 | **4** 列 | **4~5** 列 |

### 3.3 每 100 关难度节奏

每 100 关为一个批次，均保持 **固定比例**（20% Easy : 60% Medium : 20% Hard），说明关卡在批次内以难度交替排列，整体不存在线性加难趋势：

| 关卡区间 | Easy | Medium | Hard | 平均步数 | 平均分类数 |
|----------|------|--------|------|---------|-----------|
| 2 – 101 | 20 | 60 | 20 | 124 | 9.9 |
| 102 – 201 | 20 | 60 | 20 | 126 | 10.0 |
| 202 – 301 | 20 | 60 | 20 | 129 | 10.0 |
| 302 – 401 | 20 | 60 | 20 | 129 | 10.0 |
| 402 – 501 | 21 | 60 | 19 | 127 | 9.9 |
| 502 – 601 | 20 | 60 | 20 | 129 | 10.0 |
| 602 – 701 | 20 | 60 | 20 | 129 | 10.0 |
| 702 – 801 | 20 | 60 | 20 | 129 | 10.0 |
| 802 – 901 | 20 | 60 | 20 | 129 | 10.0 |
| 902 – 1000 | 19 | 60 | 20 | 130 | 10.0 |

---

## 四、关卡布局统计

### 4.1 基础堆数量分布

| 基础堆数 | 关卡数 | 说明 |
|---------|--------|------|
| 2 个 | 2 关 | 教程关卡 |
| 3 个 | **200 关** | 所有 Easy 关卡 |
| 4 个 | **640 关** | 大部分 Medium/Hard |
| 5 个 | **159 关** | 部分 Medium/Hard（约 16%）|

### 4.2 列数分布

| 列数 | 关卡数 |
|------|--------|
| 3 列 | **200 关**（Easy）|
| 4 列 | **653 关**（主要 Medium）|
| 5 列 | **146 关**（部分 Hard）|

### 4.3 牌数统计

| 指标 | 最小 | 最大 | 平均 |
|------|------|------|------|
| 全场总牌数 | 32 | 98 | **63.8** |
| 发牌堆（Stock）牌数 | 23 | 63 | **41.8** |
| 各列（Column）总牌数 | 9 | 35 | **22.0** |

### 4.4 分类配图比例

每个关卡中，部分分类会显示真实图片（如虫子、烘焙食物的实物图），大多数分类仅显示文字：

| 配图分类数量 | 关卡数 |
|------------|--------|
| 0 个配图 | **898 关**（89.9%）|
| 1 个配图 | 65 关 |
| 2 个配图 | 29 关 |
| 3 个配图 | 9 关 |

---

## 五、教程关卡详解

### 教程关卡 0（Tutorial Level 0）— 极简入门

| 参数 | 值 |
|------|----|
| 分类 | 颜色（colors: 2张）、水果（fruits: 2张）|
| 基础堆数 | 2 |
| 最大步数 | 999（无限制）|
| 难度 | easy |
| 布局 | 3 列，无发牌堆 |

**牌局布局：**
```
列1: [香蕉(banana/fruits)]
列2: [蓝色(blue/colors), 水果标题牌(fruits)]
列3: [苹果(apple/fruits), 颜色标题牌(colors), 红色(red/colors)]
```

**教程步骤提示（中文翻译）：**
1. 将"水果"分类标题牌拖入基础堆！
2. "香蕉"和它同组，也加入基础堆。
3. 将"红色"叠放在"蓝色"下方以分组牌堆。
4. 将分类标题牌放入空基础堆。
5. 把整组牌移入基础堆，完成该分类！
6. 移动最后一张牌，完成关卡！

---

### 教程关卡 1（Tutorial Level 1）— 规则深化

| 参数 | 值 |
|------|----|
| 分类 | 导航(navigation: 3张)、宠物(pets: 3张)、运动(sports: 2张)|
| 基础堆数 | 2 |
| 最大步数 | 999（无限制）|
| 发牌堆 | 有（足球 football、鸟 bird）|

**教程步骤提示（中文翻译）：**
1. 分类标题牌只能放入空基础堆。
2. 同分类的牌可以叠放在标题牌上方。
3. <span style="color:red">不能</span>在基础堆外的标题牌上叠放牌。
4. 但可以把标题牌移到自己的牌堆下方。
5. 将分组牌整体移入基础堆。
6. 在列之间移动牌来翻开朝下的牌。
7. 将牌移入基础堆，完成该分类。
8. 按顺序将标题牌和其他牌移入基础堆。
9. 用完牌时，从发牌堆抽取。
10. 将新抽的牌移到对应分类。
11. 再次点击发牌堆。
12. 移动最后一张牌，完成关卡！

---

## 六、词语分类系统

### 6.1 分类规模概览

| 指标 | 数值 |
|------|------|
| 全量唯一分类数 | **5,550 个** |
| 全量唯一关键词数 | **42,811 个** |
| 出现 1 次的分类（一次性） | 2,591 个（46.7%）|
| 出现 3+ 次的分类（常驻） | 1,037 个 |
| 出现 5+ 次的分类（核心） | 90 个 |

### 6.2 核心高频分类 TOP 100（按出现关卡数排序）

> 格式：**英文ID → 显示名称**（出现次数 × 关卡）（唯一关键词数）

| 排名 | 分类ID | 显示名称 | 出现关卡数 | 唯一词数 | 示例关键词（英文显示） |
|------|--------|----------|-----------|---------|---------------------|
| 1 | zodiac | Zodiac | 7 | 20 | Aquarius, Aries, Cancer, Gemini, Leo, Libra, Pisces, Scorpio... |
| 2 | pizza | Pizza | 7 | 18 | Anchovy, Calzone, Cheese, Crust, Dough, Hawaiian, Margherita... |
| 3 | fish | Fish | 7 | 22 | Bass, Carp, Cod, Eel, Fin, Gill, Hook, Lake, Net, Salmon... |
| 4 | bbq | Bbq | 6 | 24 | Brisket, Burger, Charcoal, Chicken, Flame, Grill, Marinade... |
| 5 | europe | Europe | 6 | 16 | Albania, Austria, Belgium, France, Germany, Italy, Spain... |
| 6 | berries | Berries | 6 | 19 | Acai, Blueberry, Boysen, Currant, Jam, Raspberry, Strawberry... |
| 7 | poetry | Poetry | 6 | 20 | Ballad, Elegy, Haiku, Imagery, Lyric, Meter, Rhyme, Sonnet... |
| 8 | drinks | Drinks | 6 | 15 | Absinthe, Cider, Coffee, Espresso, Juice, Lemonade, Tea... |
| 9 | baseball | Baseball | 6 | 14 | Ball, Base, Bat, Catcher, Field, Glove, Home run, Pitcher... |
| 10 | writers | Writers | 6 | 21 | Austen, Bronte, Camus, Dickens, Kafka, Shakespeare, Tolstoy... |
| 11 | muscles | Muscles | 6 | 28 | Abs, Biceps, Core, Deltoid, Glutes, Hamstring, Quadriceps... |
| 12 | japanese | Japanese | 6 | 17 | Anime, Bonsai, Geisha, Gyoza, Kimono, Manga, Matcha, Sushi... |
| 13 | dj | Dj | 6 | 15 | Beat, Booth, Club, Dance, Mix, Party, Rave, Sample, Setlist... |
| 14 | mexico | Mexico | 6 | 21 | Aztecs, Burrito, Cactus, Cancun, Chipotle, Fiesta, Taco... |
| 15 | locks | Locks | 6 | 13 | Bolt, Chain, Cylinder, Door, Key, Padlock, Secure, Tumbler... |
| 16 | birds | Birds | 6 | 22 | Beak, Canary, Dove, Eagle, Falcon, Feather, Finch, Owl... |
| 17 | gadget | Gadget | 6 | 19 | Battery, Camera, Device, Drone, Earbud, Gizmo, Headset, Phone... |
| 18 | optics | Optics | 5 | 14 | Aperture, Concave, Convex, Focus, Glasses, Lens, Monocle... |
| 19 | gym | Gym | 5 | 14 | Barbell, Cardio, Dumbbell, Exercise, Fitness, Lift, Strength... |
| 20 | math | Math | 5 | 21 | Algebra, Angle, Decimal, Divide, Formula, Geometry, Pi... |
| 21 | science | Science | 5 | 24 | Atom, Biology, Cell, Chemistry, Darwin, Edison, Einstein... |
| 22 | games | Games | 5 | 22 | Backgammon, Bingo, Chess, Dice, Domino, Go game, Poker... |
| 23 | ancient | Ancient | 5 | 17 | Babylon, Egypt, Era, History, Myth, Past, Rome, Ruins... |
| 24 | soccer | Soccer | 5 | 22 | Ball, Captain, Corner, Dribble, Field, Free kick, Goal... |
| 25 | mountain | Mountain | 5 | 17 | Alps, Andes, Climb, Denali, Eiger, Peak, Range, Ridge... |
| 26 | mythical | Mythical | 5 | 17 | Basilisk, Centaur, Chimera, Dragon, Fairy, Goblin, Griffin... |
| 27 | insects | Insects | 5 | 9 | Ant, Bee, Dragonfly, Fly, Ladybug, Mosquito, Moth, Wasp |
| 28 | beach | Beach | 5 | 9 | Coast, Sand, Sea, Shell, Shore, Summer, Sun, Tide, Wave |
| 29 | fruits | Fruits | 5 | 13 | Apple, Cherry, Fig, Grape, Kiwi, Mango, Melon, Orange... |
| 30 | chess | Chess | 5 | 11 | Bishop, Board, Checkmate, King, Knight, Pawn, Queen, Rook |
| 31 | weather | Weather | 5 | 15 | Cloudy, Fog, Forecast, Rain, Snow, Stormy, Sunny, Windy |
| 32 | bedroom | Bedroom | 5 | 11 | Alarm, Bed, Blanket, Chair, Closet, Lamp, Nightstand, Pillow |
| 33 | dog | Dog | 5 | 18 | Bark, Beagle, Bone, Collar, Commands, Corgi, Fetch, Labrador... |
| 34 | wedding | Wedding | 5 | 16 | Altar, Bride, Cake, Ceremony, Dress, Groom, Ring, Vow... |
| 35 | animals | Animals | 5 | 14 | Bear, Bird, Cat, Cow, Deer, Dog, Dolphin, Elephant, Lion... |
| 36 | ballet | Ballet | 5 | 15 | Barre, Dance, Dancer, Jete, Pirouette, Plie, Pointe, Pose... |
| 37 | bees | Bees | 5 | 16 | Apiary, Beehive, Beeswax, Buzz, Colony, Hive, Honey, Queen... |
| 38 | greek | Greek | 5 | 12 | Alpha, Beta, Delta, Epsilon, Gamma, Gyro, Hero, Iota, Myth |
| 39 | gemstone | Gemstone | 5 | 17 | Agate, Amber, Diamond, Emerald, Garnet, Jade, Opal, Ruby... |
| 40 | origami | Origami | 5 | 13 | Crane, Fold, Frog, Model, Paper, Shape, Swam... |
| 41 | urban | Urban | 5 | 16 | Avenue, Block, Building, City, Crowd, District, Graffiti... |
| 42 | swimming | Swimming | 5 | 11 | Beach, Dive, Float, Goggles, Lane, Pool, Stroke, Swim |
| 43 | lunar | Lunar | 5 | 8 | Crater, Light, Moon, Night, Orbit, Phase, Shadow, Silver |
| 44 | primates | Primates | 5 | 20 | Ape, Baboon, Bonobo, Chimp, Gibbon, Gorilla, Primate... |
| 45 | business | Business | 4 | 16 | Brand, CEO, Company, Market, Plan, Profit, Strategy... |
| 46 | circus | Circus | 4 | 18 | Acrobat, Clown, Juggler, Magician, Ring, Trapeze... |
| 47 | maps | Maps | 4 | 17 | Atlas, Chart, Globe, Guide, Legend, Map, Path, Route... |
| 48 | heraldry | Heraldry | 4 | 15 | Arms, Blazon, Coat, Crest, Emblem, Shield, Symbol... |
| 49 | red | Red | 4 | 16 | Blood, Crimson, Fire, Flamingo, Ruby, Scarlet, Wine... |
| 50 | dances | Dances | 4 | 18 | Ballet, Flamenco, Rumba, Salsa, Samba, Tango, Waltz... |

### 6.3 分类主题体系归纳

通过对 5,550 个分类的主题分析，可归纳出以下一级主题域：

| 主题域 | 代表分类（部分） |
|--------|----------------|
| 🍎 食物与饮食 | food, fruits, berries, bbq, pizza, drinks, bakery, dairy, snacks, sauces, dessert, candy... |
| 🐾 动物世界 | animals, birds, fish, insects, dog, cats, primates, horse, apes, bees, spider... |
| 🌍 地理人文 | europe, mexico, japan, greek, asia, africa, arctic, beach, mountain, landmarks... |
| 🎮 休闲娱乐 | games, circus, dj, arcade, sports, chess, baseball, soccer, ballet, dances... |
| 🔬 科学知识 | science, math, optics, atoms, physics, chemistry, anatomy, space, geology... |
| 🎨 艺术文化 | art, poetry, writers, origami, heraldry, music, crafts, theater, mythology... |
| 🏠 生活家居 | bedroom, furniture, kitchen, garden, bathroom, curtains, locks, appliances... |
| 💄 时尚穿着 | makeup, apparel, jewelry, hats, shirts, fashion... |
| 💡 知识词汇 | zodiac, alphabet, acronyms, antonyms, adverbs, grammar, language... |
| 🔮 神话奇幻 | mythical, ancient, alchemy, arcana, legend, dragon, magic... |
| 💪 运动健身 | gym, fitness, muscles, swimming, yoga, archery, ballet... |
| 🌿 自然生态 | garden, flowers, herbs, berries, trees, fossils, crystals... |

---

## 七、关卡示例解析

### 示例 1：Easy 关卡（关卡 2）

| 字段 | 值 |
|------|----|
| 难度 | Easy |
| 步数限制 | 70 步 |
| 基础堆 | 3 个 |
| 分类数 | 6 个 |
| 列数 | 3 列 |
| 总牌数 | 26 张（发牌堆 23 张 + 3 列共 6 张）|

**分类及关键词数量：**

| 分类 | 显示名 | 关键词数 | 有配图 |
|------|--------|---------|--------|
| colors | Colors | 6 | ❌ |
| insects | Insects | 6 | ✅ |
| bakery | Bakery | 4 | ✅ |
| furniture | Furniture | 4 | ❌ |
| continents | Continents | 3 | ❌ |
| sea | Sea | 3 | ❌ |

**发牌堆（部分）：** antarctica, bakery标题牌, moth, chair, wasp, africa, milk, sea标题牌, wave, yellow...
**列布局：**
- 列1：furniture标题牌 / yeast(bakery)
- 列2：colors标题牌 / pink(colors) / flour(bakery)
- 列3：ladybug(insects) / mosquito(insects) / table(furniture) / continents标题牌

---

### 示例 2：Medium 关卡（关卡 3）

| 字段 | 值 |
|------|----|
| 难度 | Medium |
| 步数限制 | 128 步 |
| 基础堆 | 4 个 |
| 分类数 | 10 个 |
| 列数 | 4 列 |
| 总牌数 | 57 张 |

**分类：** sports(7), weather(7), gemstones(7), fruits(6), animals(6), school(4), cold_drinks(4), flags(4), places(3), wooden(3)

---

### 示例 3：Hard 关卡（关卡 4）

| 字段 | 值 |
|------|----|
| 难度 | Medium（实为较复杂 Medium）|
| 步数限制 | 120 步 |
| 基础堆 | 4 个 |
| 分类数 | 10 个 |
| 总牌数 | 52 张 |

**分类：** shapes(3), bird(6), sharp(8), sauces(4), flowers(7), travel(4), bedroom(4), bugs(6), toys(3), snacks(7)

---

## 八、本地化文本体系

### 8.1 UI 文本键值（en-US 完整列表）

| 键名 | 英文原文 | 中文含义 |
|------|---------|---------|
| Moves | Moves | 步数 |
| Hint | Hint | 提示 |
| Undo | Undo | 撤销 |
| Settings | Settings | 设置 |
| Restart | Restart | 重新开始 |
| Level_Info | LEVEL {0} | 第 {0} 关 |
| Level_Number | Level {0} | 关卡 {0} |
| Level_Complete | Level {0} Perfect! | 第 {0} 关 完美！ |
| Level_Complete_Perfect | Perfect! | 完美！ |
| Continue_Level | Continue Level {0} | 继续第 {0} 关 |
| Out_Of_Moves | Out of Moves | 步数用尽 |
| Extra_Moves | +{0} Moves | +{0} 步 |
| Extra_Moves_With_Coin | +{0} Moves 💰{1} | 花 {1} 金币换 +{0} 步 |
| No_Moves_Left | No More Moves | 没有可用步数 |
| No_Hint_Available | No Hint Available | 暂无提示 |
| Daily_Bonus | Daily Bonus | 每日奖励 |
| Multiplier_Game_Earn | x{0} 💰 Reward Earned | 获得 x{0} 奖励 |
| Joker_Unlocked | Joker Unlocked! | 万能牌已解锁！ |
| How_To_Use | The joker card can be placed on top of any card... | 万能牌可放于任意牌上，任意牌也可放于其上 |
| Shop_Pack_Title | Premium Pack | 高级礼包 |
| IAP_Remove_Ads | REMOVE ADS | 移除广告 |
| Wrong_Move | Wrong Move | 错误移动 |
| Customize_Title | Customize | 自定义 |
| Background | Background | 桌面背景 |
| Deck | Deck | 牌背 |

### 8.2 错误提示文本（移牌规则）

| 场景 | 英文提示 | 中文含义 |
|------|---------|---------|
| 将关键词牌放到分类标题牌（非基础堆）上 | A category card can only go on top of words that match its category. | 分类标题牌只能叠在同类词汇上 |
| 将非标题牌放入空基础堆 | Only a category card can go into an empty foundation. | 空基础堆只接受分类标题牌 |
| 将不同分类的牌叠放 | You can only stack words from the same category. | 只能叠放同分类的词汇 |
| 在非基础堆的标题牌上叠放 | You can stack cards on a category card ONLY in the foundation. | 只有在基础堆中的标题牌上才能叠放 |

### 8.3 通知文本

| 场景 | 标题 | 内容 |
|------|------|------|
| 长时间未登录 | Long time no see... | Come and play Solitaire Connections! 🃏 |
| 一般唤回 | Are you bored? | — |

---

## 九、关卡设计规律总结

### 9.1 结构规律

1. **固定比例节奏**：每 100 关固定为 20% Easy + 60% Medium + 20% Hard，批次间无渐进加难。
2. **单一布局**：每关 `patterns` 数组始终只有 1 个布局，无随机变体。
3. **Easy 固定为 3 基础堆 / 3 列 / 6 分类**，结构最简，约 32~35 张牌。
4. **Medium 固定为 4 基础堆 / 4 列 / 10 分类**，约 50~65 张牌。
5. **Hard 为 4~5 基础堆 / 4~5 列 / 12~15 分类**，约 70~98 张牌。
6. 发牌堆平均持有全局约 **65% 的牌**，列中初始仅有约 35%。

### 9.2 分类词汇设计规律

1. **分类词汇量通常在 3~8 个**之间，避免过多或过少。
2. 每关同一分类不会重复出现（`hasCategoryImages` 字段各分类均唯一）。
3. **配图分类很少**（89.9% 关卡无配图），配图主要用于"动物""食物"等视觉感强的分类。
4. 5,550 个分类中约一半仅出现 1 次，说明游戏持续引入新鲜主题，避免重复感。
5. 高频核心分类（zodiac、pizza、fish、birds 等）每次出现时关键词集合不同，实现复用的同时保持新鲜感。

### 9.3 关键词词汇量

- 42,811 个唯一关键词分布于 5,550 个分类。
- 平均每个分类约 **7.7 个** 关键词。
- 最多的分类（如 muscles）拥有 **28 个**唯一关键词，但单次关卡中只取其中 4~8 个子集使用。

---

*分析工具：UnityPy 1.25.0 解包 Unity TextAsset，Python 统计计算*
*文档生成日期：2026-03-17*
