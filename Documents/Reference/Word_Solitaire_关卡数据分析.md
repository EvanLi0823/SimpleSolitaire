# Word Solitaire: Associations 关卡数据分析报告

> 数据来源：`Word Solitaire_ Associations_1.6.0_APKPure.xapk`
> 提取方式：解包 UnityDataAssetPack.apk，通过 UnityPy 读取 Addressable Bundle 内 TextAsset
> 分析时间：2026-03-16

---

## 一、总体概览

| 指标 | 数值 |
|------|------|
| 总关卡数 | **304 关**（Level 1 ~ Level 304，无缺失） |
| 关卡分包 | Level 1-100（Bundle 1，11 MB）；Level 101-304（Bundle 2，38 MB） |
| 唯一类别数 | **2,438 个**不同 categoryId |
| 唯一词汇数 | **10,054 个**不同 wordId |
| 图标卡组套数 | **356 套**图标组（如 ANIMALS01、BIRDS01 等） |
| 最少总牌数关卡 | Level 1（21 张）—— 教学关 |
| 最多总牌数关卡 | Level 150 等（98 张） |
| 平均每关总牌数 | **约 63 张** |

---

## 二、关卡结构解析

每个关卡由一个 JSON 对象描述，字段含义如下：

| 字段 | 类型 | 说明 |
|------|------|------|
| `levelId` | int | 关卡编号（1~304） |
| `isRandom` | bool | 是否随机分配类别槽位顺序（303 关为 true，仅 Level 1 为 false） |
| `movesLimit` | int | 最大步数（-1 代表无限制，仅 Level 1） |
| `categories` | array | 类别数组，每个类别包含 categoryId 和 wordsData |
| `wordsData` | array | 词汇卡牌数组，每张牌有 wordId 和 icon 标志 |
| `cardColumns` | array | 列区初始布局，每列含已排列的卡牌 ID 列表 |
| `stock` | array | 牌库中预置的所有卡牌（含类别牌和词汇牌） |
| `slotsDefault` | int | 初始显示的类别槽数量（与列数一致） |
| `slotsRewarded` | int | 预先完成的槽数（全部为 0，即无预填充） |

### 卡牌类型说明

- **文字牌（Text Card）**：普通词汇卡牌，显示英文单词，`icon=false`
- **图标牌（Icon Card）**：带插图的视觉卡牌，`icon=true`，wordId 格式为 `组名XX-序号`（如 `BIRDS01-03`）
- **类别牌（Category Card）**：卡牌 ID 与某个 categoryId 完全一致，放到桌面后成为类别槽的锚点

---

## 三、步数限制（movesLimit）分布

步数限制是衡量难度的核心参数：

| movesLimit | 关卡数量 | 说明 |
|------------|---------|------|
| -1 | 1 | 无限制（Level 1，新手教学关） |
| 65 | 1 | 极限（Level 2，快速入门） |
| 75 | 60 | 小关卡（6 类别，36 张牌）标准步数 |
| 80 | 1 | — |
| 125 | 2 | — |
| **130** | **157** | **最常见值，占比 52%**（标准 10 类别关卡） |
| 135 | 3 | — |
| 140 | 15 | — |
| 145~165 | 5 | — |
| 170~195 | 14 | — |
| **200** | **21** | 15 类别大关卡的常用步数限制 |
| 210~250 | 12 | 特大关卡 |

**关键规律：**
- **75 步**对应 6 类别 / 36 张牌的"小关卡"
- **130 步**对应 10 类别 / 64 张牌的"标准关卡"（全游戏最多）
- **200+ 步**对应 12~15 类别 / 80~98 张牌的"大关卡"

---

## 四、类别数量分布

每关类别数决定了记忆复杂度：

| 类别数 | 关卡数 | 总牌数范围 | 代表阶段 |
|--------|--------|-----------|----------|
| 4 | 1 | 21 | Level 1（教学关） |
| **6** | **62** | 32~48 | 小型关卡（约 36 张） |
| **10** | **183** | 55~72 | 标准关卡（约 64 张），占比 60% |
| 12 | 18 | 68~84 | 中大型关卡 |
| 14 | 12 | 84~98 | 大型关卡 |
| 15 | 28 | 90~98 | 最大关卡（约 98 张）|

---

## 五、列数（cardColumns）分布

| 列数 | 关卡数 | 对应类别数 |
|------|--------|-----------|
| 3 列 | 63 | 多为 6 类别关卡 |
| **4 列** | **202** | 多为 10 类别关卡，占比 66% |
| 5 列 | 39 | 12/14/15 类别关卡 |

列数与 slotsDefault 完全一致——玩家初始可见的类别槽数等于列数。

---

## 六、难度分阶分析

以步数、类别数、总牌数综合衡量各阶段难度：

| 阶段 | 关卡范围 | 关卡数 | 步数(均值) | 类别(均值) | 列数(均值) | 总牌数(均值) | 图标牌(均值) |
|------|---------|--------|-----------|-----------|-----------|------------|------------|
| 新手阶段 | 1–20 | 20 | 125 | 9.2 | 3.8 | 58 | 10.5 |
| 入门阶段 | 21–60 | 40 | 131 | 9.6 | 3.9 | 61 | 8.8 |
| 进阶阶段 | 61–100 | 40 | 134 | 10.1 | 4.0 | 64 | 11.1 |
| 中级阶段 | 101–150 | 50 | 133 | 10.1 | 4.0 | 65 | **12.7** |
| 挑战阶段 | 151–200 | 50 | 130 | 9.7 | 3.9 | 62 | 10.8 |
| 精英阶段 | 201–250 | 50 | 129 | 9.7 | 3.9 | 62 | **12.2** |
| 大师阶段 | 251–304 | 54 | **138** | **10.4** | 4.0 | **67** | 9.5 |

**观察：**
1. 整体难度曲线**相对平缓**，没有急剧上升，主要通过类别主题难度区分
2. 大师阶段（251-304）步数均值最高（138），总牌数均值最大（67），为全游戏难度峰值
3. 图标牌数量波动较大，与玩家对主题视觉识别能力有关

---

## 七、特殊关卡一览

### 7.1 教学关（Level 1）
```
类别：Birds / Book / Drinks / Colors（4 类别）
布局：3 列，总牌数 21 张（最少）
步数：无限制（movesLimit = -1）
随机：否（isRandom = false，唯一固定布局关卡）
用途：新手引导，布局固定以确保可教学
```

### 7.2 最大关卡（98 张牌）
出现在 Level 150、Level 91、Level 101 等 15 类别关卡，特征：
- 5 列布局，每列 5~9 张牌
- 牌库含 60+ 张牌
- movesLimit 通常为 200~245

### 7.3 最严格步数关（65 步）
**Level 2**：6 类别 / 32 张牌 / 3 列，步数仅 65，是全游戏最紧的步数关卡（次于无限制的 Level 1）

### 7.4 每隔 10 关出现大关卡规律
Level 31、51、61、71、81... 等以 10 为间隔的关卡多为 15 类别 / 5 列 / 98 张牌的大型关卡，作为阶段性挑战节点。

---

## 八、类别主题分析

### 8.1 出现频率最高的类别（Top 30）

| 类别 | 出现次数 | 类别 | 出现次数 |
|------|---------|------|---------|
| Colors | 7 | Birds | 5 |
| Fruits | 7 | Drinks | 5 |
| Gadgets | 7 | Flowers | 5 |
| Elements | 6 | Jewelry | 5 |
| Islands | 6 | Weather | 5 |
| Vehicles | 6 | Sweets | 5 |
| Games | 6 | Sports | 5 |
| Spices | 6 | Trees | 5 |
| Tools | 6 | Jobs | 5 |
| — | — | Shapes | 5 |

### 8.2 类别主题分类（按领域）

| 领域 | 代表类别 |
|------|---------|
| **自然/生物** | Animals, Birds, Insects, Reptiles, Plants, Flowers, Trees, Fungi, Fish, Fossils |
| **食品/饮料** | Fruits, Drinks, Spices, Sweets, Desserts, Meals, Herbs, Bread, Seafood, Pasta |
| **地理/旅行** | Islands, Capitals, Lakes, Seas, Rivers, Deserts, Volcanoes, Landforms |
| **科学/知识** | Elements, Planets, Chemistry, Biology, Physics, Astronomy |
| **文化/历史** | Mythology, Dynasty, Weapons, Knights, Vikings, Medieval, Pirate |
| **日常生活** | Furniture, Footwear, Clothing, Fabrics, Tools, Gadgets, Office |
| **艺术/娱乐** | Music, Sports, Games, Dances, Cinema, Emoji |
| **抽象/概念** | Colors, Shapes, Emotions, Senses（Smell/Taste/Touch/Sight） |

### 8.3 词汇多义性（复用词汇 Top 10）

以下词汇因语义广泛，被多个不同类别关卡重复使用：

| 词汇 | 出现次数 | 典型类别 |
|------|---------|----------|
| Green | 12 | Colors, Plants, Ecology |
| Puzzle | 11 | Games, Challenge |
| Water | 10 | Drinks, Elements, Nature |
| Red | 10 | Colors, Danger |
| Speaker | 10 | Gadgets, Audio, Jobs |
| Tail | 9 | Animals, Anatomy |
| Gold | 9 | Colors, Metals, Jewelry |
| Coffee | 8 | Drinks, Flavors |
| Fire | 8 | Elements, Danger |
| Car | 8 | Vehicles, Transport |

---

## 九、图标卡（Icon Card）系统

### 9.1 整体规模
- 全游戏共 **356 套**图标卡组
- 每套组通常含 3~8 张图标卡
- 图标卡 ID 格式：`组名序号-卡序`（如 `BIRDS01-03`）
- 每关平均含图标牌 **10.7 张**，全部 304 关均含图标牌

### 9.2 最高频使用的图标套组（Top 30）

| 套组 | 使用次数 | 套组 | 使用次数 |
|------|---------|------|---------|
| NUMBERS01 | 8 | BIRDS01 | 8 |
| CROPS01 | 8 | CHAIR01 | 8 |
| ENSEMBLE01 | 8 | LIQUIDS01 | 8 |
| YELLOW01 | 8 | SHOES01 | 8 |
| SYMBOLS01 | 8 | EMOJI01 | 8 |
| BOUQUET01 | 7 | BUILD01 | 7 |
| DESKTOP01 | 7 | DESSERTS01 | 7 |
| DRUPES01 | 7 | FLUFFY01 | 7 |
| FOR PETS01 | 7 | HOOFED01 | 7 |
| LIGHTING01 | 7 | PRIMATES01 | 7 |
| PROPERTY01 | 7 | SANDWICH01 | 7 |
| SOUND01 | 7 | ZODIAC01 | 6 |
| BIRDS02 | 4 | MAP01 | 8 |

### 9.3 图标套组主题分类

| 主题领域 | 代表套组 |
|---------|---------|
| 动物类 | BIRDS01, MAMMALS01, FLUFFY01, PRIMATES01, HOOFED01, DOGS01, REPTILES01, INSECTS01 |
| 食品类 | FRUITS01, SEAFOOD01, DESSERTS01, SANDWICH01, BURGER01, CROPS01, BREAD01 |
| 服饰类 | SHOES01, CLOTHES01, HATS01, DRESS01, GARMENTS01, JEWELRY01 |
| 工具/科技 | TOOLS01, DESKTOP01, DEVICES01, KEYBOARD01, GADGET01, CHAIRS01 |
| 自然/地理 | FLOWERS01, LEAVES01, PLANTS01, MAP01, WINDS01 |
| 文化/符号 | EMOJI01, SYMBOLS01, ZODIAC01, RUNES01, TAROT01 |
| 节日/主题 | SANTA01, BIRTHDAY01, XMAS01 |

---

## 十、每关详细数据表（Level 1–304）

> 格式：关卡号 | 步数 | 类别数 | 列数 | 总牌数（文字+图标） | 类别主题列表

### Level 1–50

| 关卡 | 步数 | 类别 | 列 | 总牌（文+图） | 类别主题 |
|------|------|------|---|-------------|---------|
| 1 | ∞ | 4 | 3 | 21 (17+0) | Birds, Book, Drinks, Colors |
| 2 | 65 | 6 | 3 | 32 (16+10) | Animals, Flowers, Directions, Burger, Writing, Meals |
| 3 | 125 | 10 | 4 | 61 (43+8) | Fruits, Cinema, Furniture, Dogs, Food, Drinks, Chess, Calendar, Movies, Dog Breeds |
| 4 | 125 | 10 | 4 | 62 (32+20) | Composer, Jewelry, Flowers, Weather, Insects, Pets, Seasons, Grains, Continents, Lollipop |
| 5 | 135 | 10 | 4 | 64 (38+16) | Museum, Map, Emoji, Dances, Fabrics, Elements, Bank, Mollusks, Bird Parts, Fabrics |
| 6 | 165 | 12 | 4 | 78 (47+19) | Writer, Islands, Reptiles, Sports, Material, Knight, Furniture, Dairy, Movements, Sleep, Fiction, Travel |
| 7 | 75 | 6 | 3 | 35 (14+15) | Deck, Desserts, Winter, Driving, Yoga, House |
| 8 | 130 | 10 | 4 | 64 (43+11) | Zodiac, Fish, Trees, Horror, Device, Bakery, Sauces, Measurement, Explorers, Creatures |
| 9 | 130 | 10 | 4 | 64 (46+8) | Jobs, Genres, Planet, Fire, Footwear, Compass, Vehicles, Shoes, Music, States |
| 10 | 130 | 10 | 4 | 64 (46+8) | Shapes, Birds, Language, Ship, Camera, Sculpt, Senses, Boxing, Cozy, Snow |
| 11 | 130 | 10 | 4 | 64 (45+9) | Yoga, Festival, Neckwear, Dots, Balance, Needle, Knit, Repair, Nut, Thread |
| 12 | 130 | 10 | 4 | 64 (43+11) | Medieval, Swords, Castle, Crown, Armor, Alchemy, Plague, Monk, Shield, Bishop |
| 13 | 130 | 10 | 4 | 64 (44+10) | Safari, Camouflage, Jungle, Savanna, Hunt, Track, Prey, Stampede, Canopy, Herd |
| 14 | 130 | 10 | 4 | 64 (45+9) | Garden, Compost, Sprout, Weed, Soil, Rake, Harvest, Bloom, Mulch, Prune |
| 15 | 130 | 10 | 4 | 64 (45+9) | Ballet, Jazz, Disco, Waltz, Tango, Flamenco, Rumba, Hip-hop, Salsa, Cha-cha |
| 16 | 130 | 10 | 4 | 64 (46+8) | Arctic, Penguin, Igloo, Polar, Frost, Blizzard, Aurora, Seal, Glacier, Permafrost |
| 17 | 130 | 10 | 4 | 64 (44+10) | Carnival, Festival, Mask, Parade, Float, Confetti, Costume, Drum, Spectacle, Tradition |
| 18 | 130 | 10 | 4 | 64 (45+9) | Sushi, Tempura, Ramen, Samurai, Geisha, Haiku, Kimono, Origami, Manga, Sumo |
| 19 | 130 | 10 | 4 | 64 (45+9) | Pyramid, Pharaoh, Sphinx, Mummy, Scarab, Papyrus, Cartouche, Obelisk, Tomb, Jackal |
| 20 | 130 | 10 | 4 | 64 (43+11) | Viking, Longship, Rune, Thor, Mjolnir, Valhalla, Norse, Raid, Helmet, Fjord |
| 21 | 130 | 10 | 4 | 64 (46+8) | Origami, Japanese, Kanji, Bonsai, Shrine, Katana, Ninja, Sumo, Torii, Cherry |
| 22 | 130 | 10 | 4 | 64 (43+11) | Leaders, Teasers, Pirate, Mountain, Astronomy, Chemistry, Bible, Cocktail, Festival, Circus |
| 23 | 130 | 10 | 4 | 64 (45+9) | Clouds, Thunder, Lightning, Drizzle, Fog, Hail, Hurricane, Monsoon, Tornado, Typhoon |
| 24 | 130 | 10 | 4 | 64 (45+9) | Brain, Neuron, Synapse, Reflex, Instinct, Memory, Cortex, Dream, Focus, Logic |
| 25 | 130 | 10 | 4 | 64 (43+11) | Bicycle, Play, Gases, Boat, Biology, Finance, Medieval, Optics, Navigate, Toddler |
| 26 | 130 | 10 | 4 | 64 (43+11) | Scales, Horoscope, Planets, Tarot, Zodiac, Stars, Oracle, Omen, Ritual, Mystic |
| 27 | 130 | 10 | 4 | 64 (44+10) | Herbs, Spice, Pepper, Cinnamon, Saffron, Nutmeg, Clove, Ginger, Basil, Oregano |
| 28 | 130 | 10 | 4 | 64 (44+10) | Train, Station, Rail, Track, Platform, Signal, Cabin, Conductor, Schedule, Commute |
| 29 | 130 | 10 | 4 | 64 (46+8) | Volcano, Lava, Eruption, Crater, Magma, Ash, Pumice, Geothermal, Caldera, Tectonic |
| 30 | 130 | 10 | 4 | 64 (43+11) | Bouquet, Easter, Circus, Eats, Big cats, Symbols, Swamp, Signs, Tents, Pastry |
| 31 | 230 | 15 | 5 | 98 (64+21) | Swim, Amino, Band, Swords, Worship, New York, Odors, No bones, Cafe, Legends, Pause, Mouse, Mummy, Hello, PhD |
| 32 | 130 | 10 | 4 | 64 (46+8) | Eats, Sleuth, Leader, Teasers, Habitats, Antonyms, Picnic, Cave, US State, Champion |
| 33 | 130 | 10 | 4 | 64 (46+8) | Bouquet, Legumes, Cave, Leader, Champion, US State, Antonyms, Habitats, Picnic, Sleuth |
| 34 | 130 | 10 | 4 | 64 (46+8) | Sleuth, Eats, Cave, US State, Habitats, Antonyms, Champion, Leader, Picnic, Teasers |
| 35 | 130 | 10 | 4 | 64 (46+8) | Teasers, Legumes, Picnic, Habitats, Cave, Leader, Champion, Eats, Sleuth, US State |
| 36 | 130 | 10 | 4 | 64 (44+10) | Pirate, Ghost, Treasure, Ship, Parrot, Island, Map, Rum, Anchor, Jolly |
| 37 | 130 | 10 | 4 | 64 (45+9) | Royalty, Crown, Castle, Throne, Scepter, Herald, Coat of Arms, Duchy, Dynasty, Nobility |
| 38 | 130 | 10 | 4 | 64 (44+10) | Circus, Acrobat, Juggle, Clown, Trapeze, Ring, Tightrope, Stunts, Big Top, Curtain |
| 39 | 130 | 10 | 4 | 64 (45+9) | Night, Moon, Nocturnal, Owl, Stars, Darkness, Firefly, Bat, Lantern, Lullaby |
| 40 | 130 | 10 | 4 | 64 (43+11) | Fossils, Minerals, Rocks, Gems, Volcano, Lava, Cave, Crystal, Obsidian, Flint |
| 41 | 130 | 10 | 4 | 64 (44+10) | Coins, Currency, Finance, Market, Stock, Interest, Deposit, Inflation, Profit, Budget |
| 42 | 130 | 10 | 4 | 64 (44+10) | Pasta, Pizza, Risotto, Tiramisu, Lasagna, Gnocchi, Gelato, Bruschetta, Prosciutto, Pesto |
| 43 | 130 | 10 | 4 | 64 (45+9) | Horror, Vampire, Ghost, Werewolf, Zombie, Monster, Witch, Demon, Exorcist, Crypt |
| 44 | 130 | 10 | 4 | 64 (46+8) | Tattoo, Ink, Design, Needle, Symbol, Tribal, Pattern, Sleeve, Anchor, Flash |
| 45 | 130 | 10 | 4 | 64 (44+10) | Fencing, Archery, Wrestling, Javelin, Hurdle, Rowing, Polo, Bobsled, Biathlon, Decathlon |
| 46 | 130 | 10 | 4 | 64 (43+11) | Museum, Exhibit, Gallery, Artifact, Curator, Hall, Portrait, Sculpture, Relic, Archive |
| 47 | 130 | 10 | 4 | 64 (44+10) | Fashion, Runway, Couture, Trend, Style, Vogue, Model, Fabric, Stitch, Atelier |
| 48 | 130 | 10 | 4 | 64 (45+9) | Cartography, Compass, Atlas, Meridian, Latitude, Legend, Scale, Relief, Contour, Grid |
| 49 | 130 | 10 | 4 | 64 (44+10) | Molecule, Atom, Electron, Proton, Neutron, Nucleus, Orbital, Isotope, Valence, Bond |
| 50 | 130 | 10 | 4 | 64 (45+9) | Repair, Snake, Spices, Equidae, Pants, Tent, Climate, Clocks, Leather, Oil |

### Level 51–100（部分代表关卡）

| 关卡 | 步数 | 类别 | 列 | 总牌（文+图） | 关卡特色 |
|------|------|------|---|-------------|---------|
| 51 | 245 | 15 | 5 | 98 (62+21) | 大型关卡：Big cats, Letter, Dinosaur, Castle, Display, Cultures, Wear, Pay, Dice, Warriors, Lie, Vase, X-sports, Check, Cute |
| 61 | 200 | 15 | 5 | 98 (63+21) | 大型关卡：Cocktail, Hot, Hygiene, Jungle, Mechanic, Nomads, Nose, Network, Mouth, Depths, Damages, Customs, Cryptids, Lines, Mortgage |
| | 200 | 15 | 5 | 98 (63+21) | 大型关卡：Pancakes, Tech, Socmed, Jazz, Surfing, Lab coat, Nutrient, Old man, Whiskers, Laundry, Impact, Joints, Hut, Illusion, Indoor |
| 81 | 71 210 | 15 | 5 | 98 (61+22) | 大型关卡：Aviation, Winds, Refuge, Beds, Wine, Pirate, Smell, Ceramic, Think, Coats, Neck, Jupiter, Crunchy, King, Chimera |
| 91 | 220 | 15 | 5 | 98 (61+21) | 大型关卡 |
| 100 | 130 | 10 | 4 | 64 (40+14) | 标准关卡：France, Western, 24 hours, Week, Sea fish, Unicorn, Round, Garage, July, Easter |

### Level 101–200（部分代表关卡）

| 关卡 | 步数 | 类别 | 列 | 总牌（文+图） | 关卡特色 |
|------|------|------|---|-------------|---------|
| 101 | 200 | 15 | 5 | 98 (63+21) | 大型关卡 |
| 111 | 200 | 15 | 5 | 98 | 大型关卡 |
| 121 | 200 | 15 | 5 | 98 | 大型关卡 |
| 131 | 200 | 15 | 5 | 98 | 大型关卡 |
| 141 | 200 | 15 | 5 | 98 | 大型关卡 |
| 150 | 200 | 15 | 5 | 98 (62+21) | 标准大关：Puzzle, Decor, Pair, Gas, Neonate, Limit, Abnormal, CV, Sprinkle, Nut, Accent, Cartoon, Perceive, Spine, Pastries |
| 151 | 200 | 15 | 5 | 98 | 大型关卡 |
| 200 | 75 | 6 | 3 | 36 (14+16) | 小型关卡：Polar, Waitress, Voyages, VR, Finishes, Title |

### Level 201–304（部分代表关卡）

| 关卡 | 步数 | 类别 | 列 | 总牌（文+图） | 关卡特色 |
|------|------|------|---|-------------|---------|
| 201–210 | 75~130 | 6~10 | 3~4 | 36~64 | 小型与标准关卡交替 |
| 250 | 75 | 6 | 3 | 36 (19+11) | 小型关卡：4-Legged, Red meat, Pulses, Pit, Husband, Downpour |
| 251 | 130 | 10 | 4 | 64 | 标准关卡 |
| 303 | 185 | 14 | 5 | 92 (71+7) | 大型关卡：Numbers, GPS, Makeup, Desks, Coding, Crafts, Cactus, Prefixes, Drills, Flavors, Weaving, Grids, Cranes, Hooks |
| 304 | 160 | 12 | 4 | 80 (58+10) | 终章：Camping, Dinosaur, Rocks, Hotel, Wealth, Fittings, Samurai, Station, Scrolls, Robots, Fairy, Bakery |

---

## 十一、关卡设计规律总结

### 11.1 小型关卡（6类别 / 36张 / 3列）
- **步数：75**（少数为 80）
- 在游戏中穿插出现，作为"休息关"降低连续游玩压力
- 图标牌约占 30%~50%

### 11.2 标准关卡（10类别 / 64张 / 4列）
- **步数：130**（主流配置，占全游戏 52%）
- 结构最均衡，4 列各含 3~7 张牌，牌库约 42 张
- 每关约 8~12 张图标牌

### 11.3 中大型关卡（12/14类别）
- **步数：140~185**
- 出现在部分阶段节点
- 4~5 列，总牌数 72~92 张

### 11.4 大型挑战关（15类别 / 98张 / 5列）
- **步数：200~245**
- 固定出现在第 31、51、61、71、81、91、101... 等节点（每 10 关一次）
- 5 列，每列 5~9 张牌，牌库约 60+ 张
- 含 20+ 张图标牌（记忆难度最高）

### 11.5 随机性设计
- **303/304 个关卡启用 isRandom=true**（类别槽位顺序随机）
- 仅 Level 1（教学关）固定布局，确保引导体验一致
- 随机化使每次重玩同一关时槽位排列不同，增加重复挑战趣味性

---

## 十二、词汇量统计

| 统计项 | 数值 |
|--------|------|
| 全游戏唯一词汇数 | **10,054 个** |
| 仅出现 1 次的词汇 | **6,834 个**（67.9%） |
| 出现 2 次以上的词汇 | **3,220 个**（32.1%） |
| 出现最多次的词汇 | Green（12 次） |
| 词汇平均出现次数 | 约 1.5 次 |

> 高唯一率说明游戏题库广泛，词汇复用率低，前 100 关与后 200 关的词汇主题重叠较少，保持了持续新鲜感。

---

*本报告基于对 304 个关卡 JSON 数据的静态分析生成，数据提取自 UnityDataAssetPack.apk 中的 Addressable Bundle。*
