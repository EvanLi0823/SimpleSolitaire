# Solitaire Word Associations 0.4.0 关卡数据分析文档

## 数据来源

| 文件 | 内容 |
|------|------|
| `Levels.json` | 141 个关卡的完整配置（步数、显示牌组数、候选牌组池） |
| `DecksText.json` | 1086 个文字牌组定义 |
| `DecksImg.json` | 230 个图片牌组定义 |
| `Parameters.json` | 游戏全局参数 |

来源路径：`assets/aa/Android/configs_assets_all_*.bundle`（通过 UnityPy 解析）

---

## 一、关卡总览

| 指标 | 数值 |
|------|------|
| 关卡总数 | **141** |
| 教程关卡（无步数限制） | 1（第1关） |
| 正式关卡（有步数限制） | 140（第2–141关） |
| 步数范围 | 65 – 220 步 |
| 平均步数（正式关卡） | ~118 步 |
| 文字牌组总数 | **1086** |
| 图片牌组总数 | **230** |
| 牌组总计 | **1316** |

---

## 二、关卡结构说明

每个关卡包含三个核心参数：

| 参数 | 含义 |
|------|------|
| `moves` | 步数上限（-1 = 无限制，即教程关） |
| `decksCount` | 本关实际**展示给玩家**的牌组数量 |
| `decks`（候选池） | 可选牌组列表，游戏每次从中**随机抽取** `decksCount` 个展示，提供重玩变化性 |

候选池大小：4–15 个牌组（通常远大于实际展示数），带来不同的重玩体验。

---

## 三、难度分布

### 3.1 按展示牌组数分布

| 展示牌组数 | 关卡数 | 占比 | 典型步数范围 |
|-----------|--------|------|-------------|
| 3 个牌组（简单） | 41 | 29.1% | 65–100 步 |
| 4 个牌组（标准） | 88 | 62.4% | 95–165 步 |
| 5 个牌组（超难） | 12 | 8.5% | 180–220 步 |

### 3.2 难度类型细分

| 难度类型 | 展示牌组数 | 步数约 | 关卡数 | 占比 |
|---------|-----------|--------|--------|------|
| 教程（Tutorial） | 3 | 无限制 | 1 | 0.7% |
| 简单（Easy） | 3 | 65–100 | 40 | 28.4% |
| 标准（Medium） | 4 | 95–135 | 63 | 44.7% |
| 中难（Med-Hard） | 4 | 136–165 | 25 | 17.7% |
| 超难（MEGA） | 5 | 180–220 | 12 | 8.5% |

### 3.3 步数频率分布（前10）

| 步数 | 出现关卡数 | 代表关卡 |
|------|-----------|---------|
| 72 | 14 | L043, L048, L060, L068... |
| 120 | 13 | L023, L041, L046, L051... |
| 75 | 11 | L002, L012, L017, L050... |
| 180 | 8 | L067, L077, L087, L097... |
| 115 | 8 | L040, L061, L076, L086... |
| 145 | 7 | L014, L042, L072, L082... |
| 73 | 6 | L043, L060, L063, L070... |
| 130 | 6 | L008, L013, L033, L044... |
| 110 | 6 | L030, L055, L065, L081... |
| 125 | 5 | L019, L028, L035, L049... |

---

## 四、关卡排列规律

### 4.1 早期引导阶段（L1–L25）

前25关为渐进式引导，难度节奏如下：

| 关卡 | 步数 | 牌组数 | 特点 |
|------|------|--------|------|
| L001 | 无限 | 3 | 教程关，4个候选牌组 |
| L002 | 75 | 3 | 首个正式关（简单） |
| L003–L004 | 150 | 4 | 首次出现4牌组 |
| L006 | 200 | 4 | 早期高难关（12候选池） |
| L007 | 70 | 3 | 难度重置 |
| L011 | 210 | 4 | 第二个高难关 |
| L016 | 215 | 4 | 第三个高难关 |
| L021 | 165 | 4 | 首个165步关卡 |

### 4.2 正式阶段（L26–L141）：10关大循环

从 L26 起形成稳定的 **10关大循环模式**：

| 循环内位置 | 难度类型 | 展示牌组数 | 步数约 |
|-----------|---------|-----------|--------|
| +0（循环起点） | **超难（MEGA）** | **5** | **180–220** |
| +1 | 简单 | 3 | 68–75 |
| +2 | 标准 | 4 | 110–130 |
| +3 | 简单 | 3 | 68–75 |
| +4 | 标准/中难 | 4 | 100–130 |
| +5 | 标准/中难 | 4 | 100–120 |
| +6 | 简单 | 3 | 70–75 |
| +7 | 标准 | 4 | 115–130 |
| +8 | 简单 | 3 | 72–75 |
| +9 | 标准 | 4 | 110–120 |

**MEGA关卡完整列表（共12个）：**

| 关卡 | 步数 | 展示牌组数 | 候选池 |
|------|------|-----------|--------|
| L026 | 200 | 5 | 14 |
| L036 | 202 | 5 | 14 |
| L047 | 185 | 5 | 14 |
| L057 | 190 | 5 | 14 |
| L067 | 180 | 5 | 14 |
| L077 | 180 | 5 | 14 |
| L087 | 180 | 5 | 15 |
| L097 | **220** | 5 | 15 |
| L107 | 180 | 5 | 15 |
| L117 | 180 | 5 | 15 |
| L127 | 180 | 5 | 15 |
| L137 | 180 | 5 | 15 |

> **规律**：L047起步数稳定在180，L087起候选池从14扩大到15，L097为全游戏最高步数（220步）。

---

## 五、完整关卡列表（L001–L141）

> 格式：`关卡号 | 步数 | 展示牌组数 | 候选池大小 | 候选牌组名称`

| 关卡 | 步数 | 牌组数 | 候选池 | 候选牌组 |
|------|------|--------|--------|---------|
| L001 | ∞ | 3 | 4 | Seasons, Space, Food, Countries |
| L002 | 75 | 3 | 6 | Insects_img, Hobbies, Drinks_img, Toys, Pets, Bathroom |
| L003 | 150 | 4 | 10 | Animals, Home, Feelings, Accessories_img, Travel, Directions_img, Music, Shopping, Clothes, Construction |
| L004 | 150 | 4 | 10 | Body, Beach, Football, Sweets_img, Furniture_img, Stationery_img, Holidays, Dishes, Jobs, Gadgets |
| L005 | 135 | 4 | 10 | Writer, Health, Kitchen, Face_img, Transport, Light, Farm, Shapes_img, Bakery, Materials |
| L006 | 200 | 4 | 12 | Desert, Crafts, Jewelry_img, Forest, Planets_img, Birds, Sky, Fruits, Mountains, Magic, Calm, SeaCreatures_img |
| L007 | 70 | 3 | 6 | Vegetables_img, Sweeteners, Camping, Frosty, Soft, Zodiac_img |
| L008 | 130 | 4 | 9 | Childhood, Books, Foliage_img, Gold, Silence, City, Household, Keyboard, Emoji_img |
| L009 | 130 | 4 | 10 | Spices_img, Headwear_img, TeaTime, Punctuation, Dogs, MusicGenres, Sparkles, Medieval, Math, Colours |
| L010 | 150 | 4 | 10 | Social, Chemistry, Literature, Herbs, Office, Shades, Greek_img, Vintage, Olympics, Grinding |
| L011 | 210 | 4 | 12 | Nails, Deserts, Capitals, Philosophers, Wildlife, Vitamins_img, Atmosphere, Halloween_img, Qualities, Energy, AncientJapan, Relatives |
| L012 | 75 | 3 | 6 | Nutrition, Berries_img, Rivers, Africa_img, Fuel, Numbers |
| L013 | 130 | 4 | 9 | Circus, Babies, China, Trees, Marine, Thanksgiving, Archaeology, Instruments_img, Tickets |
| L014 | 145 | 4 | 10 | Islands, Picnic_img, BoardGames, Diet, Swing, Eggs, Statistics, Persia, Cheeses, Concert |
| L015 | 145 | 4 | 9 | Lakes, Fossils, Birthday_img, Gaming, Poultry, Philosophy, Pasta, Airport, Sleeping |
| L016 | 215 | 4 | 12 | Romans, GameRoles, Microbiology, Italian, Doorways, Ballet, Muscles, Climate, Soil, Nocturnal_img, Christmas_img, Gambling |
| L017 | 75 | 3 | 6 | Easter_img, Existentialism, JapanFood_img, Earth, Tournament, Agriculture |
| L018 | 135 | 4 | 10 | Sauces, Biomes, Trains, 1001Nights, OceanScience, Poker, Meat, Asia_img, NewYear, Nailart_img |
| L019 | 125 | 4 | 9 | Hair_img, Grilling, Orient, Legumes_img, Seas, Celebration, Workout, Evolution, StoneAge |
| L020 | 125 | 4 | 8 | Dices_img, Biodiversity, Volcanoes, Spring, Scandi, Flexibility, Garden_img, Latin |
| L021 | 165 | 4 | 11 | Mesopotamia, Genetics, Autumn, Notes_img, Endurance, Tennis, Metro, WaterGods, Ryan, Skeleton, Casino |
| L022 | 70 | 3 | 6 | AncientGreece, Blood, Milk, SoilLife_img, Window, Houseplants |
| L023 | 120 | 4 | 9 | Percussion, Immunology, Heroes, Tom, Potato, Apples, AncientIndia, CardGames, Predators_img |
| L024 | 120 | 4 | 9 | Vocal, Decks, Nuts_img, Pear, Emma, Medals_img, Cycling, Intelligence, Bud |
| L025 | 135 | 4 | 10 | Neuroscience, Peach, Pollination, Attractions_img, Outdoor, Hygge, Aztecs, Chris, Dinosaurs_img, Hollywood |
| **L026** | **200** | **5** | **14** | Grapes, Laboratory, Celebrities, Adventure, Cooking, Artists, Architecture, Maya, Sale, Wedding, Baseball, FengShui_img, Extreme, Patterns_img |
| L027 | 68 | 3 | 6 | Cityscape, Cherry, Embroidery, Esports, Incas, Landmarks_img |
| L028 | 125 | 4 | 10 | Citrus, Textile, AncientCities, Drinkware_img, OldTown, Interview, Biotechnology, Weather, Cacti_img, HR |
| L029 | 68 | 3 | 6 | AncientEgypt, San, Anatomy, Construction_1, Piercing, America_img |
| L030 | 110 | 4 | 9 | Cognition, Mushrooms_img, Roadworks, Tomatoes, Poetry, Spartans, Trigonometry, Cowboy, Salads |
| L031 | 160 | 4 | 11 | BeautyAndTheBeast, Hills, Park, Fish_img, SaintPatrick, Memory, PoetryStyles, Chess_img, Dressings, Rock, Pirates |
| L032 | 65 | 3 | 6 | Jack, Onions, Functions, Arabia_img, Vinyl, Beer |
| L033 | 130 | 4 | 9 | Rapuntzel, Soundeffects, Pencils, Satellites, Neighborhoods, Scotland, Vision, Anna, Decor_img |
| L034 | 95 | 4 | 9 | Public, Paint, RoadSigns_img, SnowWhite, Lily, Buttons_img, Maternity, Star, Physics |
| L035 | 125 | 4 | 10 | Cultural, Brushes, LightEffects, Gestures_img, Cinderella, Stars, Teams, Magnetism, EvenToed_img, Corn |
| **L036** | **202** | **5** | **14** | Composition, Max, Arcana, Clouds, Cyberpunk, Cars_1, CafeMood, SleepingBeauty, Wall, Night, Care, Europe_img, Currency, Tetris_img |
| L037 | 70 | 3 | 6 | Framing, Trust, Artwork, Cargo, Coats, TropicalFlowers_img |
| L038 | 128 | 4 | 10 | PhotoEffects, Leo, Wealth, Succulents_img, NightCafe, LittleMermaid, Frost, Symbols, Makeup_img, Provence |
| L039 | 75 | 3 | 6 | Interiors, Aladdin, Smell, Rain, Strength, Elements_img |
| L040 | 115 | 4 | 9 | Perspective, Antarctic_img, StreetArt, Future, NASA, Roofs, Trade, Sofia, Wind |
| L041 | 120 | 4 | 9 | Cocktails, OddToed_img, Optics, Unity, Wisdom, Tarot, Dentist, Belts, Taste |
| L042 | 145 | 4 | 11 | BookCafe, Market, Royal, Savanna_img, Myths, Boats_1, Snow, Marble, IceCream_1, Sockets_1, Joy |
| L043 | 70 | 3 | 6 | Fog, Glass, Candies, Stamps_img, Ties, Mining |
| L044 | 130 | 4 | 9 | Goods, Ships_1, Rituals, Shadows, Oils, Kate, Freedom, Shells_1, Cookies_img |
| L045 | 100 | 4 | 9 | Geology, Workshop, Bakery_2_img, Coast, Hope, Rodents_img, Crystals_1, Laundry, Signals |
| L046 | 120 | 4 | 10 | Love, Courage, Toppings, Pancakes_img, Metal, ColorEffects, Astronauts, Tiles, Hairstyles_img, Dreams |
| **L047** | **185** | **5** | **14** | Tradition, Spaceship, Bread, Touch, Harbor, Skiing, Bridges, Barber, Labyrinths, Drawing, Alloys, Pastry_img, Bags, Mahjong_img |
| L048 | 70 | 3 | 6 | Peace, Skates, Minerals, Creatures, Desserts_1, Cars_img |
| L049 | 125 | 4 | 10 | Waves, Facade, Pastry_1, Reptiles_img, Grooming, ScreenEffects, Korea, USAStates, Arctic_img, Stairs |
| L050 | 75 | 3 | 6 | Perfume, Asia_1, Cakes, USCities, Beauty, Blooming_img |
| L051 | 120 | 4 | 9 | Thailand, Desserts_img, Mess, Shrines, Lost, Hotdog, Success, Fur, Canals |
| L052 | 150 | 4 | 11 | Spirit, Sauna, Fashion, Footprints_img, Paramedic, Tundra, Hiccups, Archery, Clock, Sicily, Plateau |
| L053 | 75 | 3 | 6 | Outfits, Fantasy, Taiga, Fair_img, Sneezes, Paris |
| L054 | 130 | 4 | 9 | Architect, Steppe, Rescuer, ArabSweets, Balkans, News, Valentine, SeaFiish, Sandwiches_img |
| L055 | 110 | 4 | 9 | Karate, Legends, Noses_img, Fillings, Prairie, Crystals_img, Itchy, Eskimos, Planner |
| L056 | 120 | 4 | 10 | Meadow, Romance, Turkey, IceCream_img, Composers, Sun, SeaBirds, Nomads, Gummies_img, Style |
| **L057** | **190** | **5** | **14** | Greece, Heath, Wetland, Judo, Ceremony, Creams, Masquerade, Mail, Robert, Hypermarket, ICU, Flacon_img, Dali, Dragons_img |
| L058 | 75 | 3 | 6 | Boxing, Venice, Habits, Physics_1, Dynasties, PinkEmoji_img |
| L059 | 125 | 4 | 10 | Morocco, Carnival, Cheesecream, Tattoes_img, Foodcourt, Kings, Beasts, Fencing, Rainforest_img, VanGogh |
| L060 | 73 | 3 | 6 | Wyverns, Rio, Calendar, WinterGames_img, Carpet, Domino_img |
| L061 | 115 | 4 | 9 | Industry, GreenEmoji_img, Burlesque, Georgia, Nobel, Oscars, StreetFood, Dresses, Radiation |
| L062 | 150 | 4 | 11 | Snacks, Puppy, London, Butterflies_img, Repair, Villains, Broadway, Seafood, Courts, Meetings, Vinegar |
| L063 | 73 | 3 | 6 | Pharmacy, Coffee, Malta, Cards_img, Broth, Biathlon |
| L064 | 120 | 4 | 9 | Athletics, Appetizers, Dunes, Wizards, Midwife, Evidence, Rice, Vet, Moths_img |
| L065 | 105 | 4 | 9 | Routine, Women, Beetles_img, Skirts, Soup, Jungle_img, TV, Toddler, Gym |
| L066 | 125 | 4 | 10 | Westeros, Frozen, Surfing, TropicalFruits_img, SmartHome, Amsterdam, Aquarium, Cat, Shells_img, Shed |
| **L067** | **180** | **5** | **14** | Firefighter, Cyprus, Dog, Grinch, Surgeon, Risotto, Negotiation, Butterflies_1, Tickles, Diaspora, Superpowers, Venomous_img, Detectives, Boats_img |
| L068 | 72 | 3 | 6 | Agatha, Nurse, DC, Bavaria, Highland, Headphones_img |
| L069 | 120 | 4 | 10 | Soda, Rowing, Dumplings, Spiders_img, Training, Cookies_1, TV_1, Sherlock, Whisky_img, HarryPotter |
| L070 | 73 | 3 | 6 | Marvel, Swimming, Vienna, Caterpillars_img, Chocolate, Caution_img |
| L071 | 120 | 4 | 9 | Bond, Pollinators_img, Talkshow, Casserole, Conflict, Lawyer, Pickles, Bakery_1, Citizenship |
| L072 | 145 | 4 | 11 | Queens, Bowls, Folk, Leaves_img, Socks, Exams, Ethics, Preserves, Witnesses, Spa, Coworking |
| L073 | 73 | 3 | 6 | AI, ActiveWear, Mafia, Storage_img, Freelance, Ghosts |
| L074 | 120 | 4 | 9 | Digital, Wellness, CottageCheese, Juices, Jumanji, Underwear, Psychology, Career, Mexican_img |
| L075 | 100 | 4 | 9 | Noir, Towels, Ships_img, Rights, Hamsters, Sockets_img, Narnia, Yogurt, FolkDance |
| L076 | 115 | 4 | 10 | Hacking, Smoothies, MiddleEarth, WhiteEmoji_img, Triada, Cemetery, Hotel, University, FastFood_img, Warlords |
| **L077** | **180** | **5** | **14** | Cats, Hostel, Graduation, Remote, Swimwear, Noodles, Canned, Moths_1, Afterlife, Stew, Epilation, BeigeEmoji_img, SWAT, Umbrella_img |
| L078 | 73 | 3 | 6 | Cartel, Resort, Honey, Midsummer, Internships, BlueEmoji_img |
| L079 | 118 | 4 | 10 | Harvest, Cruise, College, Glasses_img, Startup, Templars, Angels, Mukbang, Balls_img, Priest |
| L080 | 72 | 3 | 6 | Demons, ASMR, Yakuza, Vehicles_img, Anime, Shoes_img |
| L081 | 110 | 4 | 9 | Fields, Aircraft_img, Exorcism, Milkshakes, Underworld, Odyssey, Ferment, Talkshows, Pantry |
| L082 | 145 | 4 | 12 | Tolstoy, Shirts, Theatre, Hats_img, Rhetoric, Plumber, Marinade, Moon, Duties, Paella, Stations, Pines |
| L083 | 72 | 3 | 6 | Tragedy, Wardrobe, Pushkin, PeaceSymbols_img, Spruces, Playwrights |
| L084 | 120 | 4 | 9 | Comedy, Engineer, Sweaters, Airports, Semantics, Bark, Settlements, Joints, Friends_img |
| L085 | 110 | 4 | 10 | Drama, Stylistics, Medicine_img, Law, Golf, Tundra_1_img, Poets, Suits, Ligaments, India |
| L086 | 115 | 4 | 11 | JaneAusten, Electrician, Symphony, Chair_img, Gypsy, Cricket, Narrative, Eclipse, Airlines, Bushes_img, Parade |
| **L087** | **180** | **5** | **15** | Farce, Tendons, Carpenter, Regions, Rugby, Gladiators, Democracy, Beetles_1, Dickens, Fork, Corrida, Coins, SummerGames_img, Cinema, Cone_img |
| L088 | 72 | 3 | 7 | Dostoevsky, Satire, Opera, Legions, SuperBowl, Museums, TheSimpsons_img |
| L089 | 120 | 4 | 11 | Novelists, NFL, Mechanic, China_1_img, Antiques, Conquistador, Collectibles, Eurovision, Hangover, Laundry_1_img, Granny |
| L090 | 75 | 3 | 7 | Weekend, Girlsbands, Colonization, Playoffs, Formula1_img, Lonely, Eyes_img |
| L091 | 115 | 4 | 10 | Block, Teeth_img, Keys, Welder, Daytime, Rust, BachelorParty, Migration, Police, Galleries |
| L092 | 155 | 4 | 12 | Gases, BurningMan, Decay, Paths_img, Constellation_img, Opposites, Machinery, Earthpit, Gourmet, Trials, Mountaineer, Mulan |
| L093 | 93 | 3 | 6 | Erosion, Esmeralda, Injuries, Architecture_img, Fishing, Oktoberfest |
| L094 | 145 | 4 | 9 | Wilderness, SportsGear_img, Transitions, FirstAid, Hiking, Shelter, Diwali, SaintPetersburg, Fasteners_img |
| L095 | 125 | 4 | 10 | Cracks, Pocahontas, Electrical_img, Theatres, LaTomatina, Mirrors_img, Reception, LateSnack, Language, Tulips_1 |
| L096 | 135 | 4 | 11 | NationalParks, Designers, Reactions, Lanterns_img, Etiquette, Religion, InsideOut, NightOut, Hunting, Locks_img, Speech |
| **L097** | **220** | **5** | **15** | Glastonbury, SpainCrown, Clubbing, TidePools_1, Nightmares, ToyStory, Mouth, Forensics, Translation, Wonders, Cathedrals, SpiceMixes, Masks_img, Spoon, MailItems_img |
| L098 | 110 | 3 | 7 | Festivals, Pilgrimage, DeepSea_1, Reading, Phobias, Units_1, Knives_img |
| L099 | 140 | 4 | 11 | Orchids_1, Baroque, Ratatouille, Clocks_img, Runways, States, Abbeys, Lips, Romanovs, Sewing_img, Bilingual |
| L100 | 90 | 3 | 7 | FrenchCrown, ForestCore_1, Temples, MotherTeresa, Crowns_img, Moana, Fossils_1_img |
| L101 | 115 | 4 | 10 | FormalDinner, Pizza_img, FashionWeek, Toolboxes_img, Disneyland, Cycles, LakeShores_1, Metalwork, Puzzles, Guinness |
| L102 | 145 | 4 | 12 | PeterI, Roses_1, LionKing, Brooms_img, Relics_img, Gothic, Angles, Momentum, Borders, Stadium, Afterparty, HolyEmpire |
| L103 | 72 | 3 | 6 | Coachella, BritishCrown, Disney, ForestCore_img, Chaos, Renaissance |
| L104 | 120 | 4 | 9 | DryFruits_1, CoralReef_img, Identity, Alpine, ToneVoice, Bias, Constants, UFO, MapSymbols_img |
| L105 | 110 | 4 | 10 | Monks, FairyTales, Skins_img, Cognac, JewelryBox, Salad_img, SeedPods, Imbalance, Fragments, Reflections |
| L106 | 115 | 4 | 11 | Pixar, Gala, Layers, ToolKit_img, Hollows, Surfaces, Supports, Pollens, LogicSigns, Mangroves_img, Aromas |
| **L107** | **180** | **5** | **15** | Roots, Archetypes, Aliens, Mangroves_1, Thresholds, Bindings, Habsburgs, Typography, Postures, Arena, Thorns, Beacons, OceanBirds_img, Riddles, Units_img |
| L108 | 72 | 3 | 7 | Aging, Handmade, Documents, Zippers, Courier, RiverDelta, Orchids_img |
| L109 | 120 | 4 | 11 | Checklists, SecondHand, Floodplains_1, BabyKit_img, Tomorrowland, Janitor, Ironing, Hems, Enclaves, Ice_img, Censorship |
| L110 | 75 | 3 | 7 | Diasporas, ADHD, Locksmith, Red, Sandwich_img, Elections, Wardrobe_1_img |
| L111 | 115 | 4 | 10 | Amber, TeaSet_img, Alliances, BeachBag_img, Sanctions, Selfie, Scooter, Equations, SleepStates, Cuisine |
| L112 | 145 | 4 | 12 | Protests, SpiderMan, Skateboard, IceCream_2_img, Roses_img, ForestEdge_1, Fonts, UFC, Vectors, Knife, Referendum, Laziness |
| L113 | 72 | 3 | 6 | MediaPower, Bikers, Batman, LakeShores_img, BadHabits, Autonomies |
| L114 | 120 | 4 | 9 | Opposition, SeaPlants_img, Joker, Wars, Messengers, Titanic, Violet, Brain, FruitGarden_img |
| L115 | 110 | 4 | 10 | Superman, Napoleon, ArtKit_img, Monasteries, Mosses, MagicItems_img, Utopia, Shrek, Sportcars, ElonMusk |
| L116 | 115 | 4 | 11 | Moses, Ideologies, JoanOfArc, RepairSet_img, TechLeaders, TwinPeaks, Blue, Ficuses, Cyberlaws, Floodplains_img, Football_1 |
| **L117** | **180** | **5** | **15** | IronMan, Generals, Dystopias, Paradise, Karting, SeaRoutes, MapSymbols_1, Fractions, Ages, Cinema_1, Menstruation, Balcony, DeepSea_img, Mysteries, Cake_img |
| L118 | 72 | 3 | 7 | Empires, Explorers, Churchill, Yellow, Latin_1, DavidLynch, Grass_img |
| L119 | 120 | 4 | 11 | Green, WorldWarTwo, Civilization, WaterPlants_img, Merchants, Porch, Rally, Chaplin, Basketball, Sushi_1_img, Jails |
| L120 | 75 | 3 | 7 | Motocycle, Pink, Monroe, Volleyball, Taco_img, Ports, Tulips_img |
| L121 | 115 | 4 | 10 | Caravans, GardenSet_img, Hockey, MoonCycle_img, Cameron, PinUp, CandyColors, Sommelier, Phenomena, Holi |
| L122 | 145 | 4 | 12 | EarthTones, Derbies, GoldenAge, Alpine_img, Hamburger_img, TradeRoutes, Songkran, FamousTrials, Tracking, Predicates, SuperModels, Revolutions |
| L123 | 72 | 3 | 6 | Black, CreditCard, PopIcons, Breakfast_img, Treaties, Nolan |
| L124 | 120 | 4 | 9 | Ultras, Soup_1_img, Remarque, Orange, ChildGames, Ceasefire, Soap, RockIcons, Alchemy_img |
| L125 | 110 | 4 | 10 | Outlaws, Spielberg, CarKit_img, Tongue, Gray, Orchard_img, Coups, Present, SchoolGames, Divas |
| L126 | 115 | 4 | 11 | Purple, Dracula, Courtroom, SweetTreat_img, Handball, SilentCinema, Thirst, Tent, Bikes, TidePools_img, Dionis |
| **L127** | **180** | **5** | **15** | Metallic, Remark, Kubrick, DarkTales, Eating, WaterPolo, Writing, MathProofs, Vigilantes, Marketplaces, Past, Milestones, DryFruits_img, MTVEra, CoffeeSet_img |
| L128 | 72 | 3 | 7 | Hitchcock, Curses, Plazas, Future_1, Innovations, HeistFilms, Backpack_img |
| L129 | 120 | 4 | 11 | Beige, Chase, Prophecies, FirstAid_1_img, Anarchy, Goalies, Y2KEra, SciFi, Manners, Lunchbox_img, Hedgehogs |
| L130 | 75 | 3 | 7 | SocialCodes, Capybaras, Wings, Transfers, SportsBag_img, White, Totems_img |
| L131 | 115 | 4 | 10 | Brown, Feathers_img, Tarantino, MakeupBag_img, Academies, Dresscode, Boredom, Calligraphy, LegalTerms, Symptoms |
| L132 | 145 | 4 | 12 | Scorsese, Fatigue, Cheating, Egg_img, GiftBox_img, Disciplines, Diagnosis, SocialPets, Couture, Constitution, Eyes_1, Bullying |
| L133 | 72 | 3 | 6 | MentalHealth, Gossip, Methods, PetCare_img, AnimalRoles, Diving |
| L134 | 120 | 4 | 9 | SelfEsteem, Pancake_img, Symbiosis, Theories, Cuba, Dashboards, Winehouse, BritishPop, DeskSet_img |
| L135 | 110 | 4 | 10 | VintageCars, Sins, TravelKit_img, Wellbeing, WarGods, AstroTools_img, Fincher, PuertoRico, BreakTime, Nintendo |
| L136 | 115 | 4 | 11 | Teachers, Thriller, InternetLife, SeaVoyage_img, Jamaica, Pokemon, SpyGames, Xbox, Science, ForestEdge_img, Coppola |
| **L137** | **180** | **5** | **15** | Homework, MindGames, Rodriguez, Achievements, Sega, Mystery, Banquet, Dreiser, ColdWar, Apple, Prey, Honorifics, WildOcean_img, Halal, CampingKit_img |
| L138 | 72 | 3 | 7 | Gangsters, Tarkovsky, Hawaii, Suspense, Arcade, Illusions, Nests_img |
| L139 | 120 | 4 | 11 | LatinActress, RetroGames, Bollywood, HerbGarden_img, NuclearAge, Google, USWriters, Nobility, FoodChain, SchoolBag_img, Ramadan |
| L140 | 75 | 3 | 7 | Caribbean, PlayStation, SmartPhones, ActionMovies, WildGreens_img, Musicals, Handles_img |
| L141 | 115 | 4 | 10 | WebCulture, Medicinal_img, AlPacino, Mechanisms_img, SuperPowers, Fitzgerald, Ecosystems, Hercules, Symbolism, Coordinates |

---

## 六、牌组内容主题分类

### 6.1 文字牌组（1086 个）按主题领域分类

| 领域 | 代表牌组（英文原名） | 数量估计 |
|------|-------------------|---------|
| **自然·地理** | Seasons, Space, Desert, Forest, Birds, Mountains, Rivers, Seas, Islands, Lakes, Deserts, Volcanoes, Atmosphere, Earth, Climate, Biomes, Biodiversity, Tundra, Taiga, Steppe, Prairie, Plateau, Meadow, Heath, Wetland, Fog, Rain, Wind, Snow, Frost, Arctic, Antarctic | ~80 |
| **历史·文化** | AncientGreece, AncientEgypt, AncientIndia, AncientJapan, Romans, Spartans, Mesopotamia, Aztecs, Maya, Incas, StoneAge, Medieval, Renaissance, Baroque, Gothic, 1001Nights, China, Persia, Orient, AncientCities, Vikings, Romanovs, Habsburgs, FrenchCrown, BritishCrown, SpainCrown, Napoleon, Churchill, JoanOfArc, PeterI, ColdWar, WorldWarTwo | ~150 |
| **流行文化·娱乐** | Cinema, Hollywood, Disney, Pixar, Marvel, DC, HarryPotter, Westeros, MiddleEarth, Narnia, Jumanji, Bond, Sherlock, Agatha, Batman, Superman, SpiderMan, IronMan, LionKing, ToyStory, InsideOut, Mulan, Aladdin, Cinderella, BeautyAndTheBeast, LittleMermaid, SnowWhite, Rapuntzel, Moana, Pocahontas, Pokemon, Nintendo, Xbox, PlayStation, Sega | ~100 |
| **电影·导演** | TwinPeaks, Hitchcock, Tarantino, Nolan, Spielberg, Coppola, Kubrick, Scorsese, Fincher, DavidLynch, Tarkovsky, Rodriguez, Dreiser, Remarque, Chaplin | ~30 |
| **文学·作家** | Tolstoy, Pushkin, Dostoevsky, Dickens, JaneAusten, Novelists, Poets, Playwrights, USWriters, Fitzgerald | ~30 |
| **美食·饮食** | Food, Drinks, Bakery, Desserts, Pizza, Sushi, Pasta, Sauces, Soup, Salads, Seafood, Meat, Cheese, Coffee, Beer, Cocktails, Pancakes, Sandwiches, Hotdog, Chocolate, Candies, ArabSweets, SpiceMixes, Vinegar, Pickles, Ferment, Dumplings, Noodles, Rice, Paella, Risotto, Casserole, Stew, Broth | ~80 |
| **运动·体育** | Football, Basketball, Tennis, Golf, Boxing, Rugby, Cricket, Hockey, Swimming, Surfing, Skiing, Cycling, Rowing, Athletics, Archery, Fencing, Judo, Karate, Biathlon, SuperBowl, NFL, Eurovision, Formula1 | ~50 |
| **时尚·设计** | Fashion, Accessories, Jewelry, Clothes, Hairstyles, Makeup, Perfume, Dresses, Suits, Socks | ~30 |
| **科技·现代** | AI, Hacking, SmartHome, Gadgets, Digital, Cyberpunk, ElonMusk, Google, InternetLife, WebCulture | ~20 |
| **社会·职业** | Jobs, Health, Law, Medicine, Doctors, Lawyer, Engineer, Architect, Firefighter, Police | ~30 |
| **心理·哲学** | Philosophy, Psychology, Existentialism, Cognition, Intelligence, Neuroscience, Wisdom | ~20 |
| **节日·庆典** | Christmas, Birthday, Halloween, Easter, NewYear, SaintPatrick, Thanksgiving, Diwali, Ramadan, Holi, LaTomatina, Songkran | ~20 |
| **颜色系列** | Red, Orange, Yellow, Green, Blue, Purple, Pink, Black, White, Gray, Brown, Beige, Metallic | ~15 |
| **表情包·Emoji** | Emoji, PinkEmoji, GreenEmoji, WhiteEmoji, BeigeEmoji, BlueEmoji | ~10 |
| **其他** | 其余牌组（家居、情感、数学、语言学等）| ~370 |

### 6.2 图片牌组（230 个）

图片牌组名称均带 `_img` 后缀（或数字变体如 `_1_img`），内容与文字牌组对应，以图片形式展示单词，例如：

- `Insects_img`、`Drinks_img`、`Accessories_img`、`Jewelry_img`
- `Planets_img`、`SeaCreatures_img`、`Headwear_img`、`Spices_img`
- `Halloween_img`、`Christmas_img`、`Easter_img`、`Birthday_img`
- `Dinosaurs_img`、`Reptiles_img`、`Rodents_img`、`Butterflies_img`
- `Formula1_img`、`Cars_img`、`Vehicles_img`、`Aircraft_img`
- `Zodiac_img`、`Constellation_img`、`MoonCycle_img`

---

## 七、游戏参数（Parameters.json）

| 参数 | 值 | 说明 |
|------|----|------|
| 初始金币 | 500 | 新玩家起始金币 |
| 撤销（Undo）价格 | 125 金币 | 每次使用 |
| 移动推荐（Move Recommendation）价格 | 150 金币 | 每次使用 |
| 清除分类（Clear Category）价格 | 1000 金币 | 每次使用 |
| 万能牌（Joker）价格 | 500 金币 | 每次使用 |
| 额外步数（Extra Moves）价格 | 500 金币 | 步数耗尽时续命 |
| 关卡完成金币奖励 | 50 金币 | 通关基础奖励 |
| 看广告通关奖励 | 150 金币 | 看广告后额外奖励 |
| 剩余步数→金币 | 1:1 | 通关时剩余步数转化为金币 |
| 显示广告最低关卡 | 第4关 | 前3关不显示广告 |
| 插屏广告间隔 | 180 秒 | 相邻两次插屏广告最短间隔 |
| 触发广告最少步数消耗 | 50 步 | 玩家至少走50步才可能触发广告 |
| 触发广告剩余步数上限 | 20 步 | 剩余步数少于20步时才触发广告 |

---

## 八、关键数据洞察

1. **内容规模巨大**：1316 个牌组（1086 文字 + 230 图片），远超同类游戏，为重玩提供极大多样性。

2. **随机候选池机制**：每关候选池（4–15个牌组）远大于实际展示数（3–5个），同一关多次游玩主题不同，显著提升重玩价值。

3. **MEGA关节奏设计**：从第26关起，每10关设置1个5牌组超难关，节奏感强，给玩家清晰的挑战感。

4. **道具价格梯度明显**：
   - 撤销（125）< 移动推荐（150）< 万能牌（500）= 额外步数（500）< 清除分类（1000）
   - 清除分类是最昂贵道具，价格是撤销的8倍，体现其对游戏局面的决定性影响。

5. **广告触发条件精准**：只有玩家走了足够多步（≥50步）且剩余步数不多（≤20步）时才触发插屏广告，避免过早打扰，体验友好。

6. **最难关为L097**：全游戏唯一220步关卡，需展示5组牌组，候选池15个。

7. **主题覆盖极广**：从自然地理到流行文化，从历史人物到现代科技，内容触达全球受众。颜色系列（13种颜色主题）为0.4.0新增特色。

---

*文档生成日期：2026-03-17*
*分析来源：APK 静态逆向分析（Unity Addressables Bundle → UnityPy 解析 → JSON 数据提取）*
