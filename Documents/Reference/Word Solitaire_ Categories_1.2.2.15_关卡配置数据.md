# Word Solitaire: Categories — 关卡配置数据分析

> 数据来源：APK 内嵌 Unity Asset Bundle（`defaultlocalgroup_assets_definitions/en/1-20.bundle`）及 Addressables 目录

---

## 一、总体关卡规模

| 指标 | 数值 |
|------|------|
| **总关卡数** | **455 关** |
| 本地预置关卡（随包内嵌） | Level 1 – 20（共 20 关） |
| 远程按需下载关卡 | Level 21 – 455（共 435 关，分批加载） |
| 远程分包规模 | 每 20 关一个 bundle，共 23 个分包 |
| 支持语言 | 英文（EN）、德文（DE）、俄文（RU） |

### 远程分包列表（Level 21–455）

| 分包 | 关卡范围 |
|------|----------|
| 21-40.bundle | Level 21 – 40 |
| 41-60.bundle | Level 41 – 60 |
| 61-80.bundle | Level 61 – 80 |
| 81-100.bundle | Level 81 – 100 |
| 101-120.bundle | Level 101 – 120 |
| 121-140.bundle | Level 121 – 140 |
| 141-160.bundle | Level 141 – 160 |
| 161-180.bundle | Level 161 – 180 |
| 181-200.bundle | Level 181 – 200 |
| 201-220.bundle | Level 201 – 220 |
| 221-240.bundle | Level 221 – 240 |
| 241-260.bundle | Level 241 – 260 |
| 261-280.bundle | Level 261 – 280 |
| 281-300.bundle | Level 281 – 300 |
| 301-320.bundle | Level 301 – 320 |
| 321-340.bundle | Level 321 – 340 |
| 341-360.bundle | Level 341 – 360 |
| 361-380.bundle | Level 361 – 380 |
| 381-400.bundle | Level 381 – 400 |
| 401-420.bundle | Level 401 – 420 |
| 421-440.bundle | Level 421 – 440 |
| 441-455.bundle | Level 441 – 455 |

---

## 二、关卡结构说明

每一关由若干个**词汇类别（Category）**组成，每个类别包含：
- **类别名称**（如 Sweets、Music）
- **类别 ID**（数据库主键）
- **类别定义**（一句话描述该类别含义）
- **词汇列表**（3–8 个词，每词附带英文释义）

随着关卡推进：
- 早期关卡（1–5）：每关 3–5 个类别，约 11–25 个词
- 中期关卡（6–12）：每关 6 个类别，约 30–33 个词
- 后期关卡（13–20）：每关 5–8 个类别，约 27–44 个词，词汇难度逐步上升

---

## 三、Level 1–20 详细配置

### Level 1（3 类别 / 11 词）

#### Sweets（ID: 1）
> Confections that have sugar as a principal ingredient.（以糖为主要成分的糖果）

| 单词 | 英文释义 |
|------|----------|
| Candy | A sweet food item made with sugar or syrup, often flavored and colored. |
| Gummy | A chewy gelatin-based treat available in various shapes and flavors. |
| Lollipop | A hard confection mounted on a stick for sucking or licking. |

#### Sky（ID: 4）
> The region of the atmosphere and outer space seen from the earth.（地球所见的大气层与外太空区域）

| 单词 | 英文释义 |
|------|----------|
| Cloud | A visible mass of condensed water vapor floating in the atmosphere. |
| Sun | The star around which the earth orbits, providing light and heat. |
| Moon | The natural satellite of the earth, visible chiefly at night by reflected light. |
| Star | A fixed luminous point in the night sky that is a large, remote incandescent body. |

#### Mind（ID: 8）
> The element of a person that enables them to be aware of the world and their experiences, to think, and to feel.（使人感知世界、思考和感受的心理要素）

| 单词 | 英文释义 |
|------|----------|
| Memory | The faculty by which information is encoded, stored, and retrieved. |
| Focus | The center of interest or activity; the state of having clear visual definition. |
| Logic | Reasoning conducted or assessed according to strict principles of validity. |
| Idea | A thought or suggestion as to a possible course of action. |

---

### Level 2（4 类别 / 21 词）

#### Artroom（ID: 173）
> A room used for creating art.（用于艺术创作的房间）

| 单词 | 英文释义 |
|------|----------|
| Brush | An implement with a handle, consisting of bristles, hair, or wire set into a block, used for painting. |
| Paint | A colored substance that is spread over a surface and dries to leave a thin decorative or protective coating. |
| Ink | A colored fluid used for writing, drawing, printing, or duplicating. |
| Canvas | A strong, coarse unbleached cloth used as a surface for oil painting. |
| Stool | A seat without a back or arms, typically resting on three or four legs or on a single pedestal. |
| Frame | A rigid structure that surrounds or encloses a picture. |
| Shelf | A flat length of wood or rigid material, attached to a wall or forming part of piece of furniture. |

#### Shoes（ID: 12）
> Coverings for the foot.（覆盖脚部的物品）

| 单词 | 英文释义 |
|------|----------|
| Sneaker | A soft shoe worn for sports or casual occasions. |
| Boot | A sturdy item of footwear covering the foot, the ankle, and sometimes the leg. |
| Sandal | A light shoe with either an openwork upper or straps attaching the sole to the foot. |
| Loafer | A leather slip-on shoe shaped like a moccasin, with a low flat heel. |

#### Coffee（ID: 16）
> A hot drink made from the roasted and ground seeds of a tropical shrub.（由热带灌木种子烘焙磨粉制成的热饮）

| 单词 | 英文释义 |
|------|----------|
| Latte | A beverage made with espresso and hot steamed milk. |
| Mocha | A chocolate-flavored variant of a latte. |
| Americano | A drink made by diluting an espresso with hot water. |
| Cortado | A beverage consisting of espresso mixed with a roughly equal amount of warm milk. |
| Espresso | Strong black liquid made by forcing steam through ground beans. |

#### Travel（ID: 17）
> The action of making a journey, typically of some length or abroad.（旅行的行为，通常指较长距离或出国）

| 单词 | 英文释义 |
|------|----------|
| Map | A diagrammatic representation of an area of land or sea showing physical features. |
| Guide | A person who advises or shows the way to others. |
| Visa | An endorsement on a passport indicating that the holder is allowed to enter or stay in a country. |
| Passport | An official document issued by a government, certifying the holder's identity and citizenship. |
| Ticket | A certificate or token showing that a fare or admission fee has been paid. |

---

### Level 3（5 类别 / 25 词）

#### Volcano（ID: 3）
> A rupture in the crust of a planetary-mass object that allows hot lava, ash, and gases to escape.（行星地壳裂口，使熔岩、火山灰和气体喷出）

| 单词 | 英文释义 |
|------|----------|
| Lava | Molten rock that has been expelled from the interior of a terrestrial planet. |
| Crater | A bowl-shaped depression at the top of a volcano or caused by an impact. |
| Ash | Fine fragments of rock, minerals, and volcanic glass created during eruptions. |
| Magma | Molten or semi-molten natural material found beneath the surface of the Earth. |

#### Music（ID: 246）
> The art or science of combining vocal or instrumental sounds.（结合声乐或器乐声音的艺术或科学）

| 单词 | 英文释义 |
|------|----------|
| Piano | A large keyboard musical instrument with a wooden case enclosing a soundboard and metal strings. |
| Cello | A bass instrument of the violin family, held upright on the floor between the legs of the seated player. |
| Drum | A percussion instrument sounded by being struck with sticks or the hands. |
| Flute | A wind instrument made from a tube with holes that are stopped by the fingers or keys. |
| Guitar | A stringed musical instrument with a fretted fingerboard, typically incurved sides, and six or twelve strings. |
| Violin | A stringed musical instrument of treble pitch, played with a horsehair bow. |
| Harp | A musical instrument, roughly triangular in shape, consisting of a frame supporting a series of parallel strings. |

#### Snacks（ID: 21）
> Small amounts of food eaten between meals.（正餐之间食用的少量食物）

| 单词 | 英文释义 |
|------|----------|
| Chips | Thin slices of potato that have been fried or baked until crisp. |
| Cracker | A thin, dry biscuit, typically eaten with cheese or savory toppings. |
| Nuts | Fruits consisting of a hard or tough shell around an edible kernel. |
| Nachos | A dish of tortilla chips topped with melted cheese and often other ingredients. |
| Cookie | A sweet baked treat, usually flat and round. |

#### Phone（ID: 23）
> A device used for voice communication over a distance.（用于远距离语音通信的设备）

| 单词 | 英文释义 |
|------|----------|
| SMS | A system for sending short text messages to mobile devices. |
| App | A software program designed to run on a mobile device. |
| Network | A group or system of interconnected people or things. |
| Caller | A person who initiates a telephone conversation. |
| Ringer | A component that produces a sound to alert of an incoming call. |

#### Film（ID: 9）
> A motion picture or movie.（一部电影）

| 单词 | 英文释义 |
|------|----------|
| Shot | A series of frames that runs for an uninterrupted period of time. |
| Scene | A sequence of continuous action in a play, movie, opera, or book. |
| Reel | A cylinder on which flexible materials like celluloid can be wound. |
| Retro | Imitative of a style, fashion, or design from the recent past. |

---

### Level 4（5 类别 / 23 词）

#### Pizza（ID: 5）
> A savory dish of Italian origin consisting of a flattened round base of dough topped with tomatoes, cheese, and often other ingredients.

| 单词 | 英文释义 |
|------|----------|
| Slice | A wedge-shaped piece cut from a round pie or similar dish. |
| Crust | The hard outer layer of a loaf of bread or pastry. |
| Sauce | A liquid or semi-liquid substance served with food to add moistness and flavor. |
| Cheese | A dairy product derived from milk, produced in a wide range of flavors and textures. |

#### Train（ID: 11）
> A series of connected vehicles that run along a railway track and transport people or freight.

| 单词 | 英文释义 |
|------|----------|
| Rail | A bar or series of bars, typically fixed on upright supports, serving as part of a track. |
| Coach | A railway carriage or passenger car. |
| Steam | The vapor into which water is converted when heated, used as power for early engines. |
| Track | A continuous line of rails on a railway. |
| Depot | A building where supplies or vehicles, especially locomotives and buses, are housed and maintained. |

#### Delight（ID: 381）
> A profound sense of pleasure and contentment.（深切的愉悦与满足感）

| 单词 | 英文释义 |
|------|----------|
| Bliss | A profound state of perfect happiness, characterized by complete contentment and spiritual well-being. |
| Glee | A buoyant and unrestrained feeling of jubilant happiness. |
| Elation | A powerful emotion characterized by a feeling of triumphant joy and exhilaration. |
| Rapture | A state of overwhelming, intense joy and spiritual transport. |

#### Nature（ID: 31）
> The physical world collectively, including plants, animals, the landscape, and other features.

| 单词 | 英文释义 |
|------|----------|
| Tree | A woody perennial plant, typically having a single stem or trunk growing to a considerable height. |
| Leaf | A flattened structure of a higher plant, typically green and blade-like. |
| Rock | The solid mineral material forming part of the surface of the earth. |
| Flower | The seed-bearing part of a plant, consisting of reproductive organs. |
| Mushroom | A fungal growth that typically takes the form of a domed cap on a stalk. |

#### Money（ID: 39）
> A current medium of exchange in the form of coins and banknotes.

| 单词 | 英文释义 |
|------|----------|
| Coin | A flat, typically round piece of metal with an official stamp, used as currency. |
| Wallet | A pocket-sized, flat, folding case for holding money and plastic cards. |
| Salary | A fixed regular payment made by an employer to an employee. |
| Cash | Currency in the form of coins or banknotes. |
| Bank | A financial establishment that invests money deposited by customers. |

---

### Level 5（5 类别 / 25 词）

#### Hotel（ID: 94）
> An establishment providing accommodations, meals, and other services for travelers.

| 单词 | 英文释义 |
|------|----------|
| Lobby | A room near the entrance of a public building. |
| Mini-bar | A small refrigerator in a hotel room containing beverages and snacks. |
| Porter | A person employed to carry luggage and other loads. |
| Maid | A hotel employee responsible for cleaning and maintaining guest rooms. |
| Key card | A plastic card holding a physical or digital pattern that opens a door. |

#### Insect（ID: 493）
> A small arthropod animal that has six legs and generally one or two pairs of wings.

| 单词 | 英文释义 |
|------|----------|
| Ant | A small insect, typically having a sting and living in a complex social colony. |
| Bee | A stinging winged insect that collects nectar and pollen, produces wax and honey. |
| Fly | An insect with a single pair of transparent wings and sucking mouthparts. |
| Mosquito | A slender long-legged fly with aquatic larvae. |
| Beetle | An insect of a large order distinguished by having forewings modified into hard wing cases. |

#### Toys（ID: 19）
> Objects for a child to play with, typically a model or miniature replica of something.

| 单词 | 英文释义 |
|------|----------|
| Doll | A small model of a human figure, typically a baby or child. |
| Ball | A solid or hollow spherical object that is kicked, thrown, or hit in a game. |
| Block | A solid piece of hard material with flat surfaces, used for building. |
| Teddy | A soft, stuffed bear used as a plaything. |
| Train | A miniature replica of a railway vehicle for children. |

#### Village（ID: 38）
> A group of houses and associated buildings, larger than a hamlet and smaller than a town, situated in a rural area.

| 单词 | 英文释义 |
|------|----------|
| Well | A shaft sunk into the ground to obtain water, oil, or gas. |
| Barn | A large building used for storing grain, hay, or straw or for housing livestock. |
| Mill | A building equipped with machinery for grinding grain into flour. |
| Lane | A narrow road, especially in the country. |
| House | A building for human habitation, especially one lived in by a family. |

#### Puzzle（ID: 424）
> A game, toy, or problem designed to test ingenuity or knowledge.

| 单词 | 英文释义 |
|------|----------|
| Jigsaw | A machine saw with a fine blade enabling it to cut curved lines. |
| Crossword | A puzzle consisting of a grid of squares where words crossing vertically and horizontally are written. |
| Sudoku | A puzzle in which missing numbers are to be filled into a 9 by 9 grid. |
| Maze | A network of paths and hedges designed as a puzzle through which one has to find a way. |
| Trivia | Details, considerations, or pieces of information of little importance or value. |

---

### Level 6（6 类别 / 31 词）

| 类别 | ID | 词汇 |
|------|----|------|
| Prairie（草原） | 7 | Bison、Coyote、Herd、Plain |
| Babies（婴儿） | 20 | Crib、Rattle、Bib、Pram、Stroller |
| Pottery（陶艺） | 301 | Clay、Firing、Kiln、Glaze、Potter、Ceramic |
| Shop（商店） | 43 | Aisle、Counter、Receipt、Sales、Bag |
| Myth（神话） | 49 | Centaur、Troll、Nymph、Titan、Hydra |
| Drums（鼓） | 485 | Snare、Bass、Hi-hat、Rhythm、Beat、Cymbal |

---

### Level 7（6 类别 / 31 词）

| 类别 | ID | 词汇 |
|------|----|------|
| Bowling（保龄球） | 184 | Lane、Ball、Score、Tenpin、Strike、Frame |
| Nut（坚果） | 431 | Almond、Cashew、Walnut、Hazelnut、Pistachio、Pecan |
| Teens say（青少年网络语） | 3791 | Bestie、Crush、Rofl、Imba、FOMO |
| Galaxy（星系） | 48 | Asteroid、Gravity、Nebula、Meteor、UFO |
| Castle（城堡） | 51 | Tower、Moat、Gate、Dungeon |
| Gallery（画廊） | 64 | Art、Auction、Painting、Curator、Sculpture |

---

### Level 8（6 类别 / 32 词）

| 类别 | ID | 词汇 |
|------|----|------|
| Spa（水疗） | 289 | Sauna、Pool、Massage、Bath、Hot tub |
| Desert（沙漠） | 18 | Dune、Oasis、Camel、Sand、Mirage |
| Laptop（笔记本电脑） | 22 | Screen、Mouse、Cable、Touchpad、Charger |
| Clothes（服装） | 32 | Hat、Shirt、Pants、Skirt、Scarf、Socks、Gloves |
| Ship（船只） | 53 | Sail、Mast、Hull、Deck、Rope |
| Clinic（诊所） | 59 | Patient、Pills、Nurse、Doctor、Test |

---

### Level 9（6 类别 / 30 词）

| 类别 | ID | 词汇 |
|------|----|------|
| Metals（金属） | 13 | Iron、Zinc、Copper、Silver、Nickel |
| City（城市） | 24 | Park、Plaza、Bridge、Metro、Street |
| Weather（天气） | 29 | Sun、Cloud、Rain、Snow、Wind |
| Jungle（丛林） | 46 | Tiger、Liana、Tree、Snake、Monkey |
| Game（桌游） | 425 | Chess、Jenga、Dice、Domino、Ludo |
| Library（图书馆） | 63 | Catalog、Silence、Tome、Novel、Fiction |

---

### Level 10（6 类别 / 33 词）

| 类别 | ID | 词汇 |
|------|----|------|
| Desserts（甜点） | 33 | Cake、Cookie、Donut、Muffin、IceCream、Pie、Pudding |
| Mineral（矿物） | 419 | Crystal、Granite、Calcite、Geology、Malachite |
| Skywatch（观星） | 154 | Lens、Scope、Star、Eyepiece、Night、Moon |
| Zoo（动物园） | 56 | Flamingo、Lion、Pinguin、Ape、Zebra |
| Social（社交网络） | 68 | Chat、Post、Feed、Group、Meme |
| Cards（扑克牌） | 69 | Poker、Joker、Trick、Spade、Suit |

---

### Level 11（6 类别 / 32 词）

| 类别 | ID | 词汇 |
|------|----|------|
| School（学校） | 30 | Book、Pencil、Eraser、Ruler、Backpack |
| Fabric（织物） | 420 | Cotton、Silk、Wool、Linen、Velvet |
| Dinner（晚餐） | 77 | Sushi、Roast、Salad、Stew、Pasta |
| Garage（车库） | 82 | Car、Tire、Tool、Shelf、Jack |
| Sports（运动） | 90 | Football、Tennis、Boxing、Rugby、Hockey、Judo |
| Rivers（河流） | 93 | Nile、Amazon、Rhine、Seine、Thames、Ganges |

---

### Level 12（6 类别 / 31 词）

| 类别 | ID | 词汇 |
|------|----|------|
| Geometry（几何） | 26 | Square、Oval、Circle、Triangle、Trapezoid |
| Coast（海岸） | 45 | Cliff、Bluff、Bay、Inlet、Cove |
| Internet（互联网） | 67 | Browser、Modem、AI、URL、Wi-Fi |
| Bedroom（卧室） | 78 | Bed、Pillow、Pajamas、Closet、Quilt |
| Yard（庭院） | 81 | Patio、Fence、Shed、Grill、Mower |
| Bakery（烘焙坊） | 84 | Bread、Bagel、Donut、Muffin、Bun、Baguette |

---

### Level 13（6 类别 / 30 词）

| 类别 | ID | 词汇 |
|------|----|------|
| Soup（汤） | 6 | Broth、Chowder、Bisque、Gazpacho |
| Space（太空） | 34 | Planet、Star、Moon、Comet、Asteroid、Rocket |
| Royalty（皇室） | 52 | Crown、Throne、King、Prince、Queen |
| Photo（摄影） | 70 | Print、Flash、Frame、Camera、Album |
| Reptiles（爬行动物） | 74 | Snake、Gecko、Turtle、Lizard、Viper |
| Sawmill（锯木厂） | 219 | Log、Blade、Bark、Lumber、Timber |

---

### Level 14（5 类别 / 30 词）

| 类别 | ID | 词汇 |
|------|----|------|
| Garden（花园） | 35 | WateringCan、Shovel、Rake、Butterfly、Bee、Ladybug |
| Circus（马戏团） | 97 | Juggler、Tent、Arena、Acrobat、Tightrope、Magician |
| Palm（手掌） | 2831 | Fist、Finger、Palmistry、High five、Grip、Slap |
| Ocean（海洋） | 88 | Coral、Whale、Shark、Dolphin、Kraken、Reef |
| Time（时间） | 102 | Clock、Watches、Timer、Alarm、Hour、Week |

---

### Level 15（5 类别 / 27 词）

| 类别 | ID | 词汇 |
|------|----|------|
| Bathroom（浴室） | 36 | Soap、Brush、Toothbrush、Towel、Shower、Comb |
| Veggies（蔬菜） | 92 | Carrot、Spinach、Pepper、Radish、Peas、Garlic |
| Math（数学） | 62 | Angle、Ratio、Sum、Graph、Theorem |
| Soil（土壤） | 73 | Dirt、Silt、Loam、Erosion、Gravel |
| Lab（实验室） | 60 | Test、Flask、Probe、Slide、Tube |

---

### Level 16（6 类别 / 32 词）

| 类别 | ID | 词汇 |
|------|----|------|
| Clothing（服饰） | 91 | Shirt、Skirt、Jacket、Coat、Dress、Hoodie |
| Beach（海滩） | 37 | Umbrella、Bucket、Shovel、Towel、Seashell、Crab |
| Sweet（甜食） | 434 | Jam、Chocolate、Toffee、Fudge、Honey |
| Jobs（职业） | 110 | Surgeon、Miner、Lawyer、Actor、Dentist、Clerk、Plumber、Writer |
| Ballet（芭蕾） | 112 | Tutu、Pointe、Barre |
| Pottery（陶器） | 113 | Clay、Wheel、Vase、Glaze |

---

### Level 17（7 类别 / 34 词）

| 类别 | ID | 词汇 |
|------|----|------|
| Magic（魔法） | 50 | Spell、Charm、Wand、Rune、Mana |
| Code（编程） | 66 | Script、Bug、Algorithm、Binary、Testing |
| Park（公园） | 95 | Gazebo、Pond、Swing、Pathway、Picnic |
| Surgery（手术） | 115 | Mask、Scalpel、Suture、Forceps |
| Church（教堂） | 116 | Altar、Cross、Choir、Pew |
| Stadium（体育场） | 117 | Bleachers、Turf、Fans、Score |
| Flowers（花卉） | 261 | Rose、Lily、Tulip、Poppy、Daisy、Peony、Aster |

---

### Level 18（8 类别 / 44 词）

| 类别 | ID | 词汇 |
|------|----|------|
| Swimming（游泳） | 1113 | Goggles、Pool、Lane、Dive、Freestyle、Butterfly、Swimsuit |
| Barber（理发店） | 153 | Trimmer、Razor、Comb、Brush、Lather、Apron |
| Piano（钢琴） | 292 | Key、String、Note、Pedal |
| Army（军队） | 100 | Tank、Squad、Combat、Platoon、Base、Battle |
| Cattle（牛群） | 120 | Bull、Cow、Calf、Steer |
| Carpet（地毯） | 126 | Loom、Yarn、Pile、Pattern |
| Silver（银器） | 1319 | Utensils、Shine、Bracelet、Ring、Spoon、Coin |
| Sniff（嗅觉） | 2917 | Nose、Smell、Whiff、Aroma、Scent、Breath、Odor |

---

### Level 19（7 类别 / 37 词）

| 类别 | ID | 词汇 |
|------|----|------|
| Suburbs（郊区） | 55 | House、Yard、Fence、Lawn、Patio、Sidewalk |
| Morning（早晨） | 76 | Egg、Toast、Cereal、Bacon、Coffee |
| Temple（寺庙） | 125 | Monk、Deity、Idol、Shrine |
| Forest（森林） | 105 | Pine、Maple、Birch、Fir、Spruce、Cedar |
| Pharmacy（药店） | 134 | Pill、Dose、Aspirin、Bandage |
| Aquarium（水族馆） | 135 | Filter、Fish、Coral、Water、Stone |
| Spaceship（宇宙飞船） | 1789 | Alien、Cockpit、UFO、Shuttle、Astronaut、Zero-G、Orbit |

---

### Level 20（7 类别 / 39 词）

| 类别 | ID | 词汇 |
|------|----|------|
| Ores（矿石） | 72 | Quartz、Mica、Talc、Pyrite、Gypsum |
| Fruit（水果） | 108 | Apple、Grape、Mango、Peach、Lemon、Melon、Banana |
| Workshop（工坊） | 132 | Pliers、File、Mallet、Clamp、Vise |
| Acorn（橡果） | 3740 | Jay、Oak、Squirrel、Boar、Storage、Autumn、Seed |
| Harpsich（大键琴） | 204 | Keys、Strings、Baroque |
| Arctic（北极） | 47 | Igloo、Narwhal、Ice、Seal、Frost |
| Tilt（倾斜） | 3152 | Lean、Move、Side、Pinball、Incline、Shift、Slant |

---

## 四、关卡难度递进规律

| 关卡区间 | 每关类别数 | 每关词汇数 | 特征 |
|----------|------------|------------|------|
| Level 1–2 | 3–4 | 11–21 | 入门，类别简单（天空、糖果、咖啡） |
| Level 3–5 | 5 | 23–25 | 类别扩展，引入科学（火山）、技术（手机）、文化（电影）主题 |
| Level 6–10 | 6 | 30–33 | 词汇量稳定在 30+ 词，引入神话、天文、社交等复杂主题 |
| Level 11–15 | 5–6 | 27–32 | 引入地理（大河）、互联网、数学等学科类词汇 |
| Level 16–20 | 6–8 | 32–44 | 最多 8 个类别/关，词汇涵盖医疗、艺术、嗅觉、太空等高级主题 |

---

## 五、类别 ID 分布特征

从 ID 分布可推测游戏内容数据库的演进历史：

| ID 范围 | 代表类别 | 推测含义 |
|---------|----------|----------|
| 1–100 | Sweets、Sky、Train、Nature | 早期核心类别，覆盖日常生活 |
| 100–300 | Artroom、Bowling、Spa | 中期扩充，引入文化休闲场景 |
| 300–500 | Pottery、Puzzle、Nuts | 专题深化，细化特定领域词汇 |
| 500+ | Insect、Drums、Teens say | 新增扩展内容，含网络语言等现代主题 |
| 1000+ | Swimming、Silver | 后续更新包内容 |
| 2000+ | Teens say（3791）、Palm（2831） | 最新批次内容，含流行文化词汇 |

---

## 六、全游戏词汇类别汇总（135+ 类）

按主题领域分组（来自 Addressables 图集目录）：

### 自然与地理（30类）
Sky、Volcano、Nature、Desert、Jungle、Arctic、Galaxy、Space、River、Coast、Ocean、Prairie、Soil、Forest、Savanna、Tundra、Lagoon、Swamp、Polder、Seafront、Island、Cave、Arboretum、Botany、Botanics、Wildlife、Harvest、Agronomy、Forestry、Campsite

### 动植物（15类）
Animals、Reptiles、Fish、Cattle、Insects、Zoo、Safari、Aquarium、Fruit、Flowers、Orchard、Garden、Pets、Acorn、Cattle

### 食物与饮品（15类）
Sweets、Snacks、Coffee、Pizza、Desserts、Bakery、Cooking、Dining、Drinks、Morning、Canteen、Larder、Salad、Tajine、Kitchen

### 日常生活（20类）
Clothes、Clothing、Shoes、Bathroom、Bedroom、Wardrobe、Laundry、Playroom、Nursery、Toys、School、Village、Suburbs、Terrace、Mailbox、Garage、Yard、Market、Shop、Boutique

### 文化与艺术（15类）
Music、Film、Gallery、Ballet、Artroom、Puppet、Carnival、Festival、Bowling、Circus、Magic、Royalty、Ceremony、Orchestra、Ritual

### 科学与技术（15类）
Math、Geometry、Science、Chemistry、Lab、Minerals、Metals、Ores、Laptop、Internet、Code、Pharmacy、Medicine、Hospital、Clinic

### 地域文化（12类）
Japan、Kimono、Kabuki、Ikebana、Zen、Tango、Samba、Yodel、Gothic、Totem、Psalms、Tajine

### 职业与场所（12类）
Army、Forge、Barbers、Workshop、Pottery、Plumbing、Shipyard、Fishing、Sailing、Farmwork、Academy、Embassy

### 其他特色类别（10类）
Teens say（网络语言）、Delight（情感词汇）、Mind（心理词汇）、Sniff（嗅觉词汇）、Palm（手势词汇）、Tilt（方向词汇）、Acorn（自然生态）、Nut（坚果详解）、Myth（神话生物）、Cards（扑克词汇）
