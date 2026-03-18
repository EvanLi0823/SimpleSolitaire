# Solitaire Word Match 1.1.4 — 关卡数据深度分析

> 本文档基于对 APK 的静态分析，完整还原游戏关卡配置、词语内容库及关卡设计逻辑。

---

## 一、关卡系统总览

游戏关卡分为两类：**预定义关卡**（Tutorial 前6关，固定内容）和**随机生成关卡**（第7关起，按配置模板随机抽取词族）。第21关起循环复用第11-20关模板，实现**无限关卡**。

```
关卡 1-2  : 引导关（Tutorial），无限步数，固定内容，难度极低
关卡 3-6  : 引导关后期，引入步数限制，内容固定
关卡 7-20 : 随机生成关卡，难度逐步提升，步数/卡数/族数增加
关卡 21+  : 无限循环，复用关卡 11-20 的配置模板，内容每次随机
```

---

## 二、随机关卡配置详表（第1-20关）

| 关卡 | 总卡数 | 词族数 | 最大步数 | 布局 | 每族最少卡 | 难度词族配比 | 过关奖励 |
|:----:|:------:|:------:|:--------:|:----:|:----------:|------------|---------|
|  1 | 30 | 6 | ∞ | 3×3 | ≥3张 | easy:0% / 图片:0% / medium:0% / hard:0% | 金币×20 |
|  2 | 25 | 5 | ∞ | 3×3 | ≥3张 | easy:0% / 图片:0% / medium:0% / hard:0% | 金币×20 |
|  3 | 33 | 6 | 150 | 4×4 | ≥4张 | easy:0% / 图片:0% / medium:0% / hard:0% | 金币×20 |
|  4 | 33 | 6 | 150 | 4×4 | ≥3张 | easy:0% / 图片:0% / medium:0% / hard:0% | 金币×20 |
|  5 | 42 | 7 | 145 | 4×4 | ≥4张 | easy:0% / 图片:0% / medium:0% / hard:0% | 金币×20 |
|  6 | 48 | 7 | 145 | 4×4 | ≥4张 | easy:0% / 图片:0% / medium:0% / hard:0% | 金币×20 |
|  7 | 30 | 6 | 200 | 4×4 | ≥5张 | easy:50% / 图片:50% | 金币×20 + 提示×1 |
|  8 | 30 | 6 | 75 | 4×4 | ≥4张 | 图片:50% / easy:50% | 金币×20 |
|  9 | 54 | 10 | 120 | 4×4 | ≥4张 | easy:40% / 图片:30% / medium:20% / hard:10% | 金币×20 |
| 10 | 68 | 12 | 160 | 5×5 | ≥4张 | easy:15% / medium:15% / hard:25% / 图片:45% | 金币×30 + 撤回×1 |
| 11 | 30 | 6 | 75 | 3×3 | ≥4张 | easy:60% / 图片:40% | 金币×20 |
| 12 | 54 | 10 | 120 | 4×4 | ≥4张 | easy:40% / 图片:30% / medium:20% / hard:10% | 金币×20 + 提示×1 |
| 13 | 54 | 10 | 120 | 4×4 | ≥4张 | easy:40% / 图片:30% / medium:20% / hard:10% | 金币×20 |
| 14 | 68 | 12 | 200 | 4×4 | ≥4张 | 图片:40% / hard:30% / medium:20% / easy:10% | 金币×20 |
| 15 | 68 | 12 | 160 | 5×5 | ≥4张 | easy:15% / medium:15% / hard:25% / 图片:45% | 金币×20 + 撤回×1 |
| 16 | 30 | 6 | 75 | 4×4 | ≥4张 | 图片:50% / easy:50% | 金币×20 |
| 17 | 54 | 10 | 120 | 4×4 | ≥4张 | easy:40% / 图片:30% / medium:20% / hard:10% | 金币×20 + 提示×1 |
| 18 | 82 | 14 | 220 | 4×4 | ≥4张 | 图片:70% / hard:30% | 金币×30 |
| 19 | 54 | 10 | 120 | 4×4 | ≥4张 | easy:40% / 图片:30% / medium:20% / hard:10% | 金币×20 |
| 20 | 80 | 14 | 200 | 5×5 | ≥4张 | easy:15% / medium:15% / hard:25% / 图片:45% | 金币×30 + 撤回×1 |

> E=easy(简单) M=medium(中等) H=hard(困难) 图片=image类词族；均等=由引擎内部等权分配

---

## 三、预定义关卡内容（第1-6关，英语）

### 第2关
- 布局：3列 | 步数上限：∞ | 总卡数：20张 | 随机种子：3

| 词族ID | 词族名称 | 难度 | 本关卡数 | 词汇内容 |
|:------:|---------|:---:|:-------:|---------|
| 0 | Compass | easy | 5张 | North、East、South、West、Northwest、Northeast |
| 290 | Cakes | easy,image | 4张 | 图片卡（12张图） |
| 9 | Fruits | easy | 4张 | Apple、Banana、Cherry、Orange、Strawberry、Mango |
| 325 | Birds | easy,image | 3张 | 图片卡（23张图） |
| 7 | Shapes | easy | 4张 | Circle、Square、Triangle、Rectangle、Oval、Star |

### 第3关
- 布局：4列 | 步数上限：150 | 总卡数：27张 | 随机种子：1

| 词族ID | 词族名称 | 难度 | 本关卡数 | 词汇内容 |
|:------:|---------|:---:|:-------:|---------|
| 4 | Planets | easy | 5张 | Mercury、Venus、Earth、Mars、Jupiter、Saturn |
| 2 | Colors | easy | 5张 | Red、Blue、Yellow、Green、Orange、Purple |
| 1 | Job | easy | 4张 | Nurse、Driver、Teacher、Engineer、Plumber、Lawyer |
| 319 | Emojis | easy,image | 5张 | 图片卡（15张图） |
| 13 | Transport | easy | 4张 | Car、Bus、Train、Airplane、Bicycle、Boat |
| 410 | Flags | easy,image | 4张 | 图片卡（4张图） |

### 第4关
- 布局：4列 | 步数上限：150 | 总卡数：27张 | 随机种子：0

| 词族ID | 词族名称 | 难度 | 本关卡数 | 词汇内容 |
|:------:|---------|:---:|:-------:|---------|
| 35 | Clothing | easy | 6张 | Shirt、Pants、Dress、Skirt、Jacket、Sweater |
| 47 | Drinks | easy | 5张 | Water、Milk、Juice、Tea、Coffee、Soda |
| 21 | Mammals | easy | 3张 | Elephant、Lion、Tiger、Monkey、Bear、Wolf |
| 174 | Cinema | image,medium | 5张 | 图片卡（8张图） |
| 123 | World Wonders | medium | 3张 | Pyramid、Colosseum、Machu Picchu、Great Wall、Petra、Taj Mahal |
| 172 | Hats | image,easy | 5张 | 图片卡（9张图） |

### 第5关
- 布局：4列 | 步数上限：145 | 总卡数：35张 | 随机种子：0

| 词族ID | 词族名称 | 难度 | 本关卡数 | 词汇内容 |
|:------:|---------|:---:|:-------:|---------|
| 12 | Emotions | easy | 4张 | Joy、Anger、Fear、Sadness、Surprise、Disgust |
| 62 | Seas | hard | 5张 | Mediterranean、Caribbean、Baltic、Black Sea、Caspian (lake/sea)、Aegean |
| 290 | Cakes | easy,image | 5张 | 图片卡（12张图） |
| 56 | Ice Cream Flavors | easy | 8张 | Vanilla、Chocolate、Strawberry、Mint、Caramel、Coffee |
| 324 | Butterflies | medium,image | 5张 | 图片卡（8张图） |
| 94 | Trees | medium | 4张 | Oak、Maple、Pine、Willow、Birch、Cedar |
| 36 | Card Games | easy | 4张 | Poker、Bridge、Rummy、Solitaire、Blackjack、Go Fish |

### 第6关
- 布局：4列 | 步数上限：145 | 总卡数：41张 | 随机种子：7

| 词族ID | 词族名称 | 难度 | 本关卡数 | 词汇内容 |
|:------:|---------|:---:|:-------:|---------|
| 108 | Rivers | medium | 4张 | Nile、Amazon、Mississippi、Danube、Yangtze、Ganges |
| 30 | Insects | easy | 4张 | Ant、Bee、Fly、Mosquito、Beetle、Butterfly |
| 125 | Boats | medium | 5张 | Yacht、Sailboat、Canoe、Kayak、Rowboat、Cruise Ship |
| 102 | Capital Cities | medium | 8张 | London、Paris、Rome、Berlin、Tokyo、Cairo |
| 118 | Mythology | medium | 6张 | Zeus、Hera、Poseidon、Hades、Athena、Apollo |
| 300 | Pirates | hard,image | 8张 | 图片卡（13张图） |
| 330 | Toys | medium,image | 6张 | 图片卡（12张图） |

---

## 四、词语内容库完整分类（英语，411个词族）

每个词族包含15个关联词汇（图片词族则包含15张图片）。

### 4.1 简单文字（115个词族）
*纯文字卡，日常常见词汇，适合入门玩家*

| ID | 词族名称（英文） | 完整词汇列表 |
|:--:|----------------|------------|
| 0 | Compass | North | East | South | West | Northwest | Northeast | Southwest | Southeast | Bearing | Azimuth | Needle | Magnet | D… |
| 1 | Job | Nurse | Driver | Teacher | Engineer | Plumber | Lawyer | Chef | Doctor | Artist | Manager | Firefighter | Pilot | Acc… |
| 2 | Colors | Red | Blue | Yellow | Green | Orange | Purple | Black | White | Gray | Brown | Pink | Gold | Silver | Cyan | Magenta |
| 3 | Musical Instruments | Guitar | Piano | Violin | Drums | Trumpet | Cello | Flute | Saxophone | Harp | Clarinet | Oboe | Tuba | Banjo | Marim… |
| 4 | Planets | Mercury | Venus | Earth | Mars | Jupiter | Saturn | Uranus | Neptune | Pluto (dwarf) | Exoplanet | Orbit | Comet | As… |
| 5 | Kitchen Utensils | Fork | Spoon | Knife | Whisk | Spatula | Ladle | Rolling Pin | Colander | Peeler | Tongs | Measuring Cup | Grater | S… |
| 6 | Weather | Rain | Snow | Cloud | Wind | Fog | Storm | Sun | Thunder | Lightning | Hail | Breeze | Drizzle | Blizzard | Hurricane… |
| 7 | Shapes | Circle | Square | Triangle | Rectangle | Oval | Star | Diamond | Hexagon | Pentagon | Octagon | Sphere | Cube | Pyram… |
| 8 | Sports | Football | Basketball | Tennis | Golf | Baseball | Running | Swimming | Volleyball | Hockey | Boxing | Cycling | Skii… |
| 9 | Fruits | Apple | Banana | Cherry | Orange | Strawberry | Mango | Pineapple | Watermelon | Lemon | Blueberry | Raspberry | Kiwi… |
| 10 | Vegetables | Carrot | Potato | Tomato | Lettuce | Broccoli | Onion | Pepper | Corn | Spinach | Cabbage | Cucumber | Zucchini | Mus… |
| 11 | Body Parts | Head | Arm | Leg | Hand | Foot | Eye | Ear | Nose | Mouth | Neck | Shoulder | Finger | Toe | Chest | Stomach |
| 12 | Emotions | Joy | Anger | Fear | Sadness | Surprise | Disgust | Excitement | Calm | Love | Anxiety | Shame | Guilt | Envy | Hope … |
| 13 | Transport | Car | Bus | Train | Airplane | Bicycle | Boat | Ship | Motorcycle | Scooter | Subway | Taxi | Helicopter | Ferry | Lo… |
| 14 | Music | Disco | Pop | Jazz | Classical | Blues | Country | Hip Hop | Electronic | Reggae | Folk | Metal | Soul | Rock | Gospe… |
| 15 | School Subjects | Math | Science | History | English | Art | Geography | Music | Physics | Chemistry | Biology | Literature | Economics… |
| 16 | Garden | Flower | Tree | Bush | Shovel | Rake | Soil | Grass | Hose | Watering Can | Fence | Bench | Birdhouse | Seed | Compos… |
| 17 | Furniture | Chair | Table | Sofa | Bed | Desk | Lamp | Shelf | Cabinet | Dresser | Stool | Ottoman | Rug | Mirror | Clock | Wardrobe |
| 18 | Desserts | Cake | Pie | Cheesecake | Ice Cream | Pudding | Brownie | Donut | Muffin | Tart | Cookie | Sorbet | Custard | Gelato … |
| 19 | Board Games | Chess | Checkers | Monopoly | Scrabble | Clue | Risk | Go | Backgammon | Dominos | Poker | Bingo | Dice | Token | Car… |
| 20 | Birds | Robin | Sparrow | Eagle | Hawk | Pigeon | Owl | Penguin | Parrot | Canary | Crow | Swan | Duck | Goose | Vulture | Al… |
| 21 | Mammals | Elephant | Lion | Tiger | Monkey | Bear | Wolf | Deer | Fox | Rabbit | Mouse | Whale | Bat | Kangaroo | Horse | Zebra |
| 22 | Technology | Computer | Phone | Internet | Software | Hardware | Network | Chip | Pixel | Data | Server | Cloud | Tablet | Router … |
| 23 | Footwear | Shoe | Boot | Sandal | Sneaker | Heel | Slipper | Loafer | Moccasin | Clog | Trainer | Flip-Flop | Wellington | Pump … |
| 24 | Office Supplies | Pen | Paper | Stapler | Folder | Marker | Printer | Envelope | Scissors | Highlighter | Paperclip | Binder | Whiteboa… |
| 25 | Amusement Park | Rollercoaster | Ferris Wheel | Carousel | Ticket | Ride | Line | Cotton Candy | Game | Midway | Booth | Thrill | Admi… |
| 26 | Countries | Brazil | England | India | Russia | Canada | Mexico | Egypt | Nigeria | Japan | Korea | Australia | Germany | France … |
| 27 | Fast Food | Burger | Fries | Pizza | Taco | Sandwich | Hot Dog | Chicken | Shake | Soda | Doughnut | Wrap | Nuggets | Onion Rings… |
| 28 | Names | Michael | Jessica | Chris | Emily | David | Sarah | James | Anna | Robert | Laura | John | Maria | William | Nicole |… |
| 29 | Holidays | Christmas | Easter | Halloween | Thanksgiving | Diwali | Hanukkah | New Year | Valentine's | Birthday | Anniversary |… |
| 30 | Insects | Ant | Bee | Fly | Mosquito | Beetle | Butterfly | Moth | Cricket | Grasshopper | Wasp | Locust | Ladybug | Termite | … |
| 31 | Adjectives | Happy | Sad | Tall | Short | Fast | Slow | Big | Small | Red | Blue | Loud | Quiet | Bright | Dark | Smooth |
| 32 | Verbs | Run | Jump | Walk | Eat | Sleep | Speak | Think | Write | Read | Drive | Sing | Dance | Fly | Swim | Build |
| 33 | Materials | Wood | Metal | Plastic | Glass | Rubber | Paper | Ceramic | Concrete | Brick | Fabric | Foam | Resin | Clay | Stone |… |
| 34 | Tools | Hammer | Drill | Saw | Wrench | Pliers | Level | Screwdriver | Chisel | Ax | Measuring Tape | Vise | File | Clamp | S… |
| 35 | Clothing | Shirt | Pants | Dress | Skirt | Jacket | Sweater | Coat | Socks | Shoes | Hat | Scarf | Gloves | Belt | Tie | Jeans |
| 36 | Card Games | Poker | Bridge | Rummy | Solitaire | Blackjack | Go Fish | Hearts | Cribbage | Euchre | Pinochle | Deck | Hand | Shuf… |
| 37 | Weather Events | Rain | Snow | Hail | Thunderstorm | Blizzard | Hurricane | Tornado | Drought | Fog | Wind | Cyclone | Sleet | Rainbow… |
| 38 | Kitchen Appliances | Oven | Stove | Microwave | Toaster | Blender | Refrigerator | Dishwasher | Coffee Maker | Mixer | Grill | Kettle | Fo… |
| 39 | Musical Genres | Pop | Rock | Jazz | Blues | Classical | Folk | Metal | Hip-Hop | Reggae | EDM | Soul | Country | Opera | Punk | Disco |
| 40 | Prepositions | At | On | In | To | For | With | Under | Over | Through | About | Between | Among | Behind | Before | After |
| 41 | Farm Animals | Cow | Pig | Chicken | Sheep | Horse | Goat | Duck | Turkey | Goose | Donkey | Llama | Alpaca | Rabbit | Rooster | Calf |
| 42 | Sports Equipment | Ball | Bat | Racket | Club | Net | Stick | Helmet | Glove | Pads | Skates | Goggles | Hoop | Cleats | Goal | Shuttlecock |
| 43 | Building Parts | Wall | Floor | Ceiling | Roof | Door | Window | Foundation | Beam | Column | Staircase | Chimney | Balcony | Attic | … |
| 44 | US States | California | Texas | Florida | New York | Alaska | Hawaii | Illinois | Ohio | Pennsylvania | Michigan | Georgia | Nor… |
| 45 | Gardening Tools | Trowel | Rake | Shovel | Hoe | Pruners | Clippers | Wheelbarrow | Gloves | Fork | Edger | Sprinkler | Hand Cultivator… |
| 46 | Road Signs | Stop | Yield | Speed Limit | Warning | Curve | Detour | School Zone | Railroad | Merge | One Way | Do Not Enter | Ped… |
| 47 | Drinks | Water | Milk | Juice | Tea | Coffee | Soda | Beer | Wine | Cocktail | Smoothie | Lemonade | Cocoa | Energy Drink | Ci… |
| 48 | Writing Tools | Pen | Pencil | Keyboard | Paper | Ink | Quill | Typewriter | Marker | Chalk | Eraser | Notebook | Tablet | Stylus | C… |
| 49 | Body Actions | Breathe | Swallow | Blink | Cough | Sneeze | Yawn | Frown | Smile | Shiver | Sweat | Digest | Walk | Run | Hear | See |
| 50 | Shapes 2D | Circle | Square | Triangle | Rectangle | Oval | Hexagon | Pentagon | Octagon | Rhombus | Trapezoid | Parallelogram | … |
| 51 | Car Brands | Toyota | Honda | Ford | Chevrolet | BMW | Mercedes | Audi | Volkswagen | Hyundai | Kia | Nissan | Jeep | Tesla | Ferr… |
| 52 | Road Vehicles | Car | Truck | Bus | Motorcycle | Van | Scooter | Bicycle | Lorry | SUV | Sedan | Coupe | Hatchback | Convertible | Mi… |
| 53 | Sports Teams | Lakers | Yankees | Cowboys | Celtics | Barcelona | Real Madrid | Packers | Patriots | Bulls | Heat | Dodgers | Giants… |
| 54 | Currency | Dollar | Euro | Yen | Pound | Franc | Rupee | Peso | Yuan | Real | Ruble | Dinar | Shekel | Ringgit | Won | Shilling |
| 55 | Hair Styles | Braid | Bun | Ponytail | Bob | Pixie | Perm | Dreadlocks | Afro | Fade | Bangs | Updo | Buzzcut | Cornrows | Layers |… |
| 56 | Ice Cream Flavors | Vanilla | Chocolate | Strawberry | Mint | Caramel | Coffee | Pistachio | Mango | Butter Pecan | Cookie Dough | Cream … |
| 57 | Zoo Animals | Lion | Tiger | Monkey | Elephant | Giraffe | Panda | Bear | Zebra | Penguin | Rhino | Hippo | Gorilla | Snake | Flami… |
| 187 | Flowers | Rose | Tulip | Daisy | Sunflower | Orchid | Lily | Violet | Poppy | Lotus | Jasmine | Lavender | Peony |
| 188 | Dog Breeds | Labrador | Poodle | Bulldog | Beagle | Chihuahua | Boxer | Husky | Dalmatian | Pug | Collie | Terrier | Retriever |
| 190 | Forest Animals | Deer | Fox | Bear | Wolf | Squirrel | Owl | Rabbit | Moose | Badger | Raccoon | Woodpecker | Beaver |
| 191 | Jungle Animals | Monkey | Tiger | Parrot | Snake | Frog | Leopard | Gorilla | Sloth | Toucan | Jaguar | Chameleon | Piranha |
| 195 | Fitness | Gym | Run | Lift | Weight | Mat | Stretch | Sweat | Muscle | Train | Strong | Healthy | Yoga |
| 196 | Farm Life | Barn | Tractor | Hay | Fence | Field | Farmer | Silo | Plow | Crop | Scarecrow | Pitchfork | Bucket |
| 197 | Bathroom | Shower | Bathtub | Toilet | Sink | Towel | Soap | Mirror | Brush | Toothpaste | Shampoo | Paper | Mat |
| 198 | Bedroom | Bed | Pillow | Blanket | Wardrobe | Lamp | Mattress | Alarm | Dresser | Sheet | Pajamas | Hanger | Nightstand |
| 199 | Living Room | Sofa | Armchair | TV | Table | Carpet | Bookshelf | Fireplace | Painting | Remote | Cushion | Curtain | Vase |
| 200 | Kitchen Items | Fridge | Stove | Oven | Sink | Table | Chair | Kettle | Microwave | Toaster | Blender | Pan | Pot |
| 201 | Laundry | Washer | Dryer | Basket | Peg | Iron | Board | Detergent | Softener | Rack | Hanger | Fold | Stain |
| 202 | Cleaning | Broom | Mop | Bucket | Sponge | Rag | Vacuum | Detergent | Gloves | Brush | Duster | Bin | Bag |
| 204 | Baby Items | Diaper | Bottle | Crib | Stroller | Pacifier | Toy | Bib | Wipes | Highchair | Rattle | Blanket | Onesie |
| 205 | School Items | Backpack | Book | Notebook | Pencil | Pen | Eraser | Ruler | Desk | Board | Chalk | Glue | Scissors |
| 206 | Office Items | Computer | Desk | Chair | Printer | Phone | Paper | Stapler | File | Pen | Mouse | Screen | Lamp |
| 208 | Jewelry | Ring | Necklace | Bracelet | Earring | Diamond | Gold | Silver | Pearl | Watch | Pendant | Gem | Brooch |
| 209 | Accessories | Bag | Belt | Hat | Scarf | Gloves | Sunglasses | Wallet | Backpack | Umbrella | Tie | Watch | Handkerchief |
| 210 | Winter Clothes | Coat | Scarf | Gloves | Hat | Boots | Sweater | Jacket | Earmuffs | Mittens | Parka | Vest | Socks |
| 211 | Summer Clothes | T-shirt | Shorts | Dress | Skirt | Sandals | Hat | Swimsuit | Bikini | Cap | Sunglasses | Flip-flops | Tank Top |
| 212 | Footwear | Sneakers | Boots | Sandals | Heels | Slippers | Flats | Loafers | Clogs | Wedges | Cleats | Running Shoes | Socks |
| 214 | Hygiene | Soap | Shampoo | Deodorant | Razor | Comb | Brush | Floss | Lotion | Towel | Tissue | Perfume | Clippers |
| 217 | Breakfast | Toast | Egg | Cereal | Coffee | Milk | Juice | Pancake | Bacon | Yogurt | Fruit | Oatmeal | Waffle |
| 218 | Lunch | Sandwich | Salad | Soup | Burger | Pizza | Pasta | Wrap | Fruit | Water | Chips | Cookie | Soda |
| 219 | Dinner | Steak | Chicken | Fish | Rice | Potato | Vegetable | Pasta | Salad | Soup | Bread | Wine | Dessert |
| 220 | Bakery | Bread | Bun | Cake | Pie | Cookie | Donut | Muffin | Croissant | Bagel | Roll | Pastry | Tart |
| 221 | Fast Food | Burger | Fries | Pizza | Hot Dog | Taco | Nuggets | Soda | Shake | Onion Rings | Burrito | Wrap | Ketchup |
| 229 | Sweets | Candy | Chocolate | Cake | Cookie | Ice Cream | Donut | Lollipop | Gum | Brownie | Fudge | Toffee | Marshmallow |
| 230 | Drinks | Water | Milk | Juice | Soda | Tea | Coffee | Beer | Wine | Lemonade | Smoothie | Cider | Cocktail |
| 235 | Pizza | Cheese | Pepperoni | Mushroom | Sausage | Onion | Pepper | Olive | Ham | Pineapple | Bacon | Crust | Sauce |
| 237 | City | Street | Building | Car | Bus | Shop | Park | Sidewalk | Light | Sign | People | Traffic | Skyline |
| 238 | Park | Tree | Grass | Bench | Path | Playground | Fountain | Flower | Duck | Pond | Picnic | Bike | Walk |
| 239 | Beach Items | Sand | Towel | Umbrella | Sunscreen | Bucket | Spade | Ball | Surfboard | Cooler | Chair | Hat | Glasses |
| 241 | School | Classroom | Teacher | Student | Desk | Board | Book | Hallway | Gym | Library | Cafeteria | Bus | Bell |
| 245 | Train Station | Train | Track | Platform | Ticket | Conductor | Schedule | Passenger | Seat | Car | Whistle | Stop | Station |
| 246 | Post Office | Mail | Letter | Stamp | Box | Package | Clerk | Envelope | Address | Scale | Truck | Delivery | Zip Code |
| 248 | Supermarket | Cart | Food | Aisle | Shelf | Checkout | Cashier | Bag | Receipt | Produce | Dairy | Bakery | Frozen |
| 249 | Restaurant | Table | Chair | Menu | Waiter | Food | Drink | Chef | Kitchen | Bill | Tip | Plate | Fork |
| 250 | Cinema | Movie | Screen | Seat | Popcorn | Ticket | Soda | Candy | Dark | Projector | Film | Action | Comedy |
| 252 | Library | Book | Shelf | Table | Computer | Quiet | Librarian | Card | Read | Study | Desk | Chair | Silence |
| 254 | Circus | Tent | Clown | Acrobat | Lion | Elephant | Popcorn | Ring | Trapeze | Juggler | Ticket | Fun | Show |
| 255 | Farm | Barn | Field | Animal | Tractor | Farmer | Crop | Cow | Pig | Chicken | Horse | Hay | Fence |
| 256 | Zoo | Lion | Tiger | Bear | Monkey | Giraffe | Elephant | Zebra | Cage | Ticket | Keeper | Map | Shop |
| 262 | Music Class | Sing | Play | Note | Song | Instrument | Band | Rhythm | Beat | Listen | Teacher | Practice | Concert |
| 265 | Seasons | Spring | Summer | Autumn | Winter | Season | Weather | Sun | Snow | Rain | Leaf | Flower | Cold |
| 267 | Fairy Tales | Princess | Prince | Castle | Dragon | Witch | Magic | Fairy | King | Queen | Story | Once | End |
| 268 | Superheroes | Hero | Cape | Mask | Power | Fly | Strong | Save | Villain | Fight | City | Secret | Team |
| 270 | Pirates | Ship | Sea | Treasure | Map | Parrot | Sword | Hat | Patch | Hook | Island | Gold | Flag |
| 274 | Games | Board | Card | Dice | Piece | Win | Lose | Turn | Play | Fun | Rules | Player | Team |
| 275 | Party | Balloon | Cake | Gift | Music | Dance | Friend | Fun | Hat | Candle | Game | Food | Drink |
| 276 | Halloween | Ghost | Pumpkin | Witch | Bat | Cat | Candy | Costume | Night | Scary | Skeleton | Spider | Boo |
| 277 | Christmas | Tree | Santa | Gift | Star | Snow | Reindeer | Elf | Stocking | Light | Card | Family | Food |
| 278 | Easter | Bunny | Egg | Basket | Hunt | Candy | Spring | Chick | Flower | Chocolate | Sunday | Grass | Find |
| 279 | Valentine | Heart | Love | Card | Flower | Rose | Chocolate | Red | Pink | Cupid | Date | Kiss | Hug |
| 280 | Birthday | Cake | Candle | Gift | Party | Balloon | Age | Year | Happy | Song | Card | Wish | Blow |
| 282 | Fire Station | Truck | Hose | Ladder | Helmet | Firefighter | Siren | Pole | Water | Alarm | Axe | Rescue | Boots |
| 283 | Movies | Film | Actor | Star | Screen | Watch | Popcorn | Ticket | Show | Story | Funny | Sad | Action |
| 284 | Police Station | Badge | Car | Jail | Handcuffs | Uniform | Siren | Officer | Radio | Law | Help | Patrol | Blue |

### 4.2 中等文字（95个词族）
*纯文字卡，需要一定知识积累*

| ID | 词族名称（英文） | 完整词汇列表 |
|:--:|----------------|------------|
| 90 | Fictional Animals | Dragon | Unicorn | Griffin | Phoenix | Sphinx | Mermaid | Werewolf | Vampire (creature) | Yeti | Hydra | Chimera | Ba… |
| 91 | Books | Novel | Biography | Textbook | Mystery | Thriller | Fantasy | Science Fiction | Poetry | Reference | Cookbook | Histo… |
| 92 | Europe Countries | France | Germany | Italy | Spain | UK | Greece | Poland | Sweden | Norway | Ireland | Portugal | Switzerland | Belgiu… |
| 93 | Ocean Life | Whale | Dolphin | Shark | Coral | Fish | Octopus | Squid | Crab | Seal | Turtle | Starfish | Jellyfish | Seaweed | Ma… |
| 94 | Trees | Oak | Maple | Pine | Willow | Birch | Cedar | Elm | Redwood | Spruce | Ash | Holly | Palm | Fig | Cypress | Magnolia |
| 95 | Architecture | Building | Patio | Tower | Cathedral | Castle | Skyscraper | Pyramid | Dome | Arch | Column | Facade | Foundation | S… |
| 96 | Gems | Diamond | Ruby | Sapphire | Emerald | Amethyst | Topaz | Pearl | Opal | Garnet | Aquamarine | Peridot | Tourmaline | … |
| 97 | Photography | Camera | Lens | Shutter | Aperture | Flash | Tripod | Focus | Exposure | Film | Digital | Portrait | Landscape | Filt… |
| 98 | Cooking Verbs | Chop | Slice | Dice | Boil | Fry | Bake | Grill | Roast | Sauté | Stir | Whisk | Blend | Marinate | Simmer | Steam |
| 99 | Astronomy | Star | Moon | Sun | Planet | Galaxy | Nebula | Comet | Meteor | Telescope | Constellation | Black Hole | Light Year |… |
| 100 | Hand Tools | Hammer | Screwdriver | Pliers | Wrench | Saw | Drill | Tape Measure | Level | Chisel | Clamp | File | Vise | Ax | Sho… |
| 101 | Textile | Cotton | Silk | Wool | Linen | Polyester | Nylon | Rayon | Denim | Velvet | Fleece | Satin | Leather | Canvas | Burla… |
| 102 | Capital Cities | London | Paris | Rome | Berlin | Tokyo | Cairo | Beijing | Moscow | Sydney | Ottawa | Vienna | Madrid | Dublin | Seou… |
| 103 | Religions | Christianity | Islam | Judaism | Hinduism | Buddhism | Sikhism | Shinto | Taoism | Faith | Worship | Prayer | Temple … |
| 104 | Units of Measure | Meter | Liter | Gram | Second | Watt | Volt | Ampere | Ohm | Joule | Foot | Pound | Ounce | Degree | Mile | Gallon |
| 105 | Reptiles | Snake | Lizard | Turtle | Crocodile | Alligator | Chameleon | Gecko | Tortoise | Viper | Cobra | Iguana | Komodo Drag… |
| 106 | Time Periods | Century | Decade | Era | Epoch | Millennium | Age | Year | Month | Week | Day | Hour | Minute | Second | Antiquity | … |
| 107 | Natural Disasters | Earthquake | Volcano | Tsunami | Hurricane | Flood | Tornado | Wildfire | Drought | Landslide | Avalanche | Cyclone |… |
| 108 | Rivers | Nile | Amazon | Mississippi | Danube | Yangtze | Ganges | Rhine | Volga | Thames | Seine | Colorado | Murray | Congo … |
| 109 | Desert | Sahara | Gobi | Atacama | Namib | Arabian | Kalahari | Mojave | Sand | Dune | Cactus | Oasis | Heat | Arid | Camel | … |
| 110 | Solar System | Sun | Asteroid | Comet | Meteoroid | Kuiper Belt | Oort Cloud | Dwarf Planet | Terrestrial | Gas Giant | Satellite | … |
| 111 | Space Gear | Astronaut | Spacesuit | Helmet | Oxygen | Rocket | Shuttle | Satellite | Rover | Module | Docking | Control | Telesco… |
| 112 | World Cuisine | Pasta | Sushi | Taco | Curry | Pizza | Pho | Hummus | Paella | Burrito | Goulash | Dumpling | Stew | Kebab | Ramen | … |
| 113 | Mountains | Everest | K2 | Kilimanjaro | Denali | Alps | Andes | Rockies | Fuji | Summit | Peak | Range | Glacier | Altitude | Tr… |
| 114 | Adverbs | Quickly | Slowly | Loudly | Softly | Happily | Sadly | Always | Never | Often | Sometimes | Here | There | Up | Down … |
| 115 | Fairy Tales | Cinderella | Snow White | Rumpelstiltskin | Gingerbread Man | Goldilocks | Red Riding Hood | Sleeping Beauty | Prince… |
| 116 | The Senses | Sight | Sound | Smell | Taste | Touch | Hearing | Vision | Olfaction | Gustation | Feeling | Perception | Sensory | N… |
| 117 | Dinosaurs | T-Rex | Triceratops | Velociraptor | Brontosaurus | Stegosaurus | Fossil | Extinct | Jurassic | Cretaceous | Herbivor… |
| 118 | Mythology | Zeus | Hera | Poseidon | Hades | Athena | Apollo | Ares | Loki | Thor | Odin | Dragon | God | Hero | Legend | Prophecy |
| 119 | Spices | Salt | Pepper | Cinnamon | Clove | Nutmeg | Basil | Oregano | Thyme | Rosemary | Ginger | Garlic | Turmeric | Cumin |… |
| 120 | Dance | Ballet | Jazz | Tap | Hip Hop | Waltz | Salsa | Tango | Modern | Ballroom | Choreography | Stage | Routine | Step | R… |
| 121 | Art Mediums | Oil | Acrylic | Watercolor | Pastel | Charcoal | Pencil | Ink | Sculpture | Clay | Canvas | Brush | Palette | Fresco … |
| 122 | Money Terms | Coin | Banknote | Currency | Dollar | Euro | Yen | Pound | Credit | Debit | Wealth | Budget | Investment | Finance | … |
| 123 | World Wonders | Pyramid | Colosseum | Machu Picchu | Great Wall | Petra | Taj Mahal | Statue of Zeus | Lighthouse | Temple | Christ R… |
| 124 | Cooking Methods | Frying | Baking | Boiling | Grilling | Roasting | Steaming | Sautéing | Broiling | Braising | Simmering | Poaching | … |
| 125 | Boats | Yacht | Sailboat | Canoe | Kayak | Rowboat | Cruise Ship | Freighter | Tugboat | Submarine | Dinghy | Speedboat | Cat… |
| 126 | Coffee Drinks | Espresso | Latte | Cappuccino | Americano | Mocha | Macchiato | Cold Brew | Drip | Flat White | Frappe | Ristretto | … |
| 127 | Punctuation | Period | Comma | Question Mark | Exclamation | Semicolon | Colon | Quotation Mark | Apostrophe | Hyphen | Dash | Pare… |
| 128 | Office Roles | Manager | Assistant | Analyst | Director | Executive | Intern | Consultant | Clerk | Receptionist | Accountant | Admi… |
| 129 | Types of Hats | Fedora | Beanie | Baseball Cap | Sun Hat | Trilby | Beret | Cowboy Hat | Sombrero | Top Hat | Bonnet | Visor | Fascin… |
| 130 | Superpowers | Flight | Invisibility | Strength | Telepathy | Speed | Healing | Telekinesis | Elasticity | Time Travel | Force Field… |
| 131 | Psychology | Mind | Brain | Therapy | Emotion | Behavior | Cognition | Stress | Trauma | Perception | Subconscious | Memory | Pers… |
| 132 | Computer Parts | CPU | Monitor | Keyboard | Mouse | Motherboard | RAM | Hard Drive | Graphics Card | Power Supply | Fan | Speaker | We… |
| 133 | Fictional Weapons | Lightsaber | Wand | Phaser | Excalibur | Mjolnir | Batarang | Lasers | Swords | Arrows | Staff | Axe | Dagger | Bow |… |
| 134 | Medical Tools | Stethoscope | Syringe | Scalpel | Forceps | Thermometer | Bandage | X-ray | MRI | Sutures | Tourniquet | Mask | Glove… |
| 135 | Musical Notation | Note | Rest | Staff | Clef | Sharp | Flat | Natural | Bar | Rhythm | Tempo | Melody | Harmony | Chord | Measure | Key… |
| 136 | Fictional Races | Elf | Dwarf | Orc | Hobbit | Human | Vulcan | Klingon | Wookiee | Alien | Goblin | Gnome | Troll | Fairy | Centaur | … |
| 137 | Geometry Terms | Point | Line | Plane | Angle | Segment | Ray | Vertex | Perimeter | Area | Volume | Radius | Diameter | Circumference… |
| 138 | Photography Terms | Aperture | Shutter Speed | ISO | Exposure | Focus | Depth of Field | Composition | Tripod | Lens | Filter | Flash | Z… |
| 139 | Airlines | Delta | United | American | Lufthansa | Emirates | Qatar | British Airways | Air France | Qantas | Singapore | Turkis… |
| 140 | Fictional Objects | Ring | Stone | Lamp | Cloak | Sword | Orb | Grail | Mirror | Helmet | Book | Key | Potion | Amulet | Chest | Map |
| 141 | Computer Software | Windows | macOS | Linux | Browser | Word Processor | Spreadsheet | Photoshop | Antivirus | Operating System | Applica… |
| 142 | Types of Tea | Green | Black | Oolong | White | Herbal | Earl Grey | Chai | Darjeeling | Jasmine | Peppermint | Chamomile | Rooibos … |
| 143 | Dressing Garnish | Parsley | Cilantro | Mint | Chives | Basil | Lemon | Lime | Olive | Cherry | Whipped Cream | Sugar | Sprinkles | Topp… |
| 144 | Human Organs | Heart | Lungs | Brain | Liver | Kidneys | Stomach | Intestine | Skin | Spleen | Pancreas | Gallbladder | Bladder | Es… |
| 145 | The Middle Ages | Castle | Knight | King | Queen | Feudalism | Peasant | Plague | Crusade | Cathedral | Monastery | Joust | Armor | Fie… |
| 146 | Types of Wood | Oak | Maple | Pine | Cherry | Walnut | Birch | Mahogany | Teak | Ash | Cedar | Fir | Spruce | Balsa | Poplar | Ebony |
| 147 | Musical Terms | Tempo | Rhythm | Melody | Harmony | Pitch | Octave | Chord | Scale | Crescendo | Diminuendo | Forte | Piano | Staccat… |
| 148 | Gymnastics Moves | Flip | Twist | Vault | Cartwheel | Handstand | Split | Tumbling | Balance Beam | Uneven Bars | Rings | Pommel Horse |… |
| 149 | Types of Rocks | Igneous | Sedimentary | Metamorphic | Granite | Basalt | Limestone | Marble | Quartzite | Sandstone | Shale | Obsidia… |
| 189 | Cat Breeds | Persian | Siamese | Sphynx | Maine Coon | Ragdoll | Bengal | Siberian | Birman | Bombay | Manx | Burmese | Somali |
| 192 | Arctic Animals | Polar Bear | Penguin | Seal | Walrus | Arctic Fox | Snowy Owl | Beluga | Narwhal | Reindeer | Puffin | Orca | Hare |
| 193 | Recycling | Paper | Plastic | Glass | Metal | Bin | Sort | Reuse | Reduce | Earth | Can | Bottle | Box |
| 194 | Marine Life | Shark | Dolphin | Whale | Octopus | Jellyfish | Crab | Lobster | Seahorse | Starfish | Turtle | Seal | Clownfish |
| 203 | Garage | Car | Bike | Toolbox | Workbench | Ladder | Lawnmower | Tire | Oil | Drill | Saw | Paint | Box |
| 207 | Makeup | Lipstick | Mascara | Foundation | Shadow | Powder | Eyeliner | Blush | Brush | Polish | Remover | Mirror | Gloss |
| 213 | Hairstyles | Bob | Braid | Bun | Ponytail | Curls | Straight | Pixie | Mohawk | Afro | Bald | Bangs | Layers |
| 222 | Spa | Massage | Mask | Robe | Relax | Sauna | Steam | Towel | Lotion | Cucumber | Quiet | Oil | Stone |
| 224 | Berries | Strawberry | Blueberry | Raspberry | Blackberry | Cranberry | Gooseberry | Currant | Elderberry | Mulberry | Acai | G… |
| 225 | Nuts & Seeds | Almond | Walnut | Peanut | Cashew | Pistachio | Pecan | Hazelnut | Sunflower | Pumpkin | Chia | Sesame | Flax |
| 226 | Dairy | Milk | Cheese | Butter | Yogurt | Cream | Ice Cream | Cottage Cheese | Sour Cream | Whipped Cream | Ghee | Kefir | Whey |
| 227 | Meat | Beef | Pork | Chicken | Turkey | Lamb | Duck | Bacon | Ham | Sausage | Steak | Salami | Ribs |
| 228 | Seafood | Fish | Shrimp | Crab | Lobster | Clam | Oyster | Mussel | Squid | Octopus | Scallop | Tuna | Salmon |
| 231 | Coffee | Espresso | Latte | Cappuccino | Mocha | Americano | Macchiato | Iced Coffee | Black | Decaf | Roast | Bean | Mug |
| 232 | Tea | Green | Black | Herbal | Chai | Earl Grey | Mint | Chamomile | Jasmine | Oolong | Iced Tea | Bag | Pot |
| 233 | Spices | Salt | Pepper | Sugar | Cinnamon | Garlic | Ginger | Basil | Oregano | Chili | Curry | Cumin | Paprika |
| 234 | Pasta | Spaghetti | Penne | Fusilli | Macaroni | Lasagna | Ravioli | Linguine | Fettuccine | Rigatoni | Bowtie | Gnocchi | Sh… |
| 236 | Sauces | Ketchup | Mustard | Mayo | BBQ | Ranch | Salsa | Soy | Hot Sauce | Pesto | Gravy | Relish | Hummus |
| 240 | Camping Gear | Tent | Sleeping Bag | Flashlight | Fire | Backpack | Compass | Knife | Rope | Map | Stove | Canteen | Mat |
| 242 | University | Campus | Lecture | Professor | Student | Dorm | Degree | Exam | Library | Thesis | Club | Lab | Graduation |
| 243 | Hospital | Doctor | Nurse | Patient | Bed | Room | Medicine | Surgery | X-ray | Emergency | Waiting Room | Ambulance | Mask |
| 244 | Airport | Plane | Gate | Ticket | Passport | Security | Luggage | Pilot | Terminal | Runway | Flight | Arrival | Departure |
| 251 | Theater | Stage | Actor | Play | Curtain | Light | Costume | Audience | Applause | Script | Drama | Show | Seat |
| 253 | Museum | Art | Statue | Exhibit | History | Painting | Guide | Ticket | Display | Ancient | Dinosaur | Relic | Tour |
| 261 | Art | Paint | Draw | Color | Brush | Canvas | Clay | Sculpture | Paper | Pencil | Create | Design | Artist |
| 263 | Computers | Screen | Keyboard | Mouse | Internet | Game | Type | Code | File | Save | Click | Wifi | Laptop |
| 264 | Detectives | Clue | Magnifying Glass | Fingerprint | Mystery | Solve | Suspect | Crime | Evidence | Witness | Case | Spy | Track |
| 266 | Time | Second | Minute | Hour | Day | Week | Month | Year | Now | Later | Clock | Watch | Calendar |
| 269 | Robots | Metal | Wire | Battery | Chip | AI | Sensor | Motor | Code | Machine | Android | Cyborg | Gear |
| 271 | Wild West | Cowboy | Horse | Hat | Boot | Sheriff | Saloon | Desert | Cactus | Gun | Train | Gold | Town |
| 272 | Knights | Armor | Sword | Shield | Castle | King | Horse | Battle | Helmet | Lance | Honor | Quest | Dragon |
| 273 | Magic Tricks | Card | Hat | Wand | Rabbit | Dove | Coin | Illusion | Magician | Cape | Box | Secret | Trick |
| 281 | Wedding | Bride | Groom | Dress | Ring | Cake | Flower | Love | Kiss | Vow | Party | Dance | Guest |
| 285 | Sushi | Rice | Seaweed | Salmon | Tuna | Roll | Wasabi | Ginger | Chopsticks | Soy Sauce | Plate | Chef | Fish |
| 286 | Zodiac Signs | Aries | Taurus | Gemini | Cancer | Leo | Virgo | Libra | Scorpio | Sagittarius | Capricorn | Aquarius | Pisces |

### 4.3 困难文字（40个词族）
*纯文字卡，专业/冷僻词汇，极具挑战*

| ID | 词族名称（英文） | 完整词汇列表 |
|:--:|----------------|------------|
| 58 | Chemical Elements | Oxygen | Carbon | Gold | Silver | Iron | Hydrogen | Nitrogen | Neon | Helium | Calcium | Copper | Sodium | Chlorine |… |
| 59 | Precious Metals | Gold | Silver | Platinum | Palladium | Rhodium | Iridium | Osmium | Ruthenium | Alloy | Ore | Bullion | Karat | Ingot… |
| 60 | Military Ranks | General | Colonel | Major | Captain | Lieutenant | Sergeant | Corporal | Admiral | Commander | Cadet | Private | Chie… |
| 61 | Writing Styles | Fiction | Nonfiction | Essay | Poetry | Memoir | Journalism | Biography | Technical | Creative | Academic | Narrative… |
| 62 | Seas | Mediterranean | Caribbean | Baltic | Black Sea | Caspian (lake/sea) | Aegean | North Sea | Adriatic | Red Sea | Arabi… |
| 63 | Galaxy Items | Star | Nebula | Black Hole | Quasar | Pulsar | Supernova | Dark Matter | Andromeda | Milky Way | Cluster | Comet | Si… |
| 64 | Sea Predators | Shark | Orca | Barracuda | Moray Eel | Giant Squid | Bluefish | Piranha | Great White | Lionfish | Sea Lion | Leopard… |
| 65 | Body Systems | Skeletal | Muscular | Nervous | Circulatory | Respiratory | Digestive | Endocrine | Immune | Urinary | Lymphatic | Or… |
| 66 | Shapes 3D | Sphere | Cube | Pyramid | Cylinder | Cone | Prism | Torus | Dodecahedron | Tetrahedron | Vertex | Edge | Face | Volum… |
| 67 | Chemical Bonds | Covalent | Ionic | Metallic | Hydrogen | Van Der Waals | Atom | Electron | Molecule | Compound | Nucleus | Valence | … |
| 68 | Famous Painters | Picasso | Van Gogh | Monet | Leonardo | Rembrandt | Michelangelo | Dali | Raphael | Warhol | Klimt | Renoir | Degas |… |
| 69 | Fictional Cities | Gotham | Atlantis | Hogwarts | Metropolis | Narnia | Middle-earth | Oz | Wonderland | Springfield | Starfleet | Riven… |
| 70 | Constellations | Orion | Ursa Major | Cassiopeia | Andromeda | Lyra | Cygnus | Leo | Gemini | Taurus | Scorpius | Big Dipper | Crux | … |
| 71 | Literary Genres | Drama | Comedy | Tragedy | Satire | Epic | Romance | Gothic | Western | Horror | Bildungsroman | Parody | Fable | All… |
| 72 | African Countries | Nigeria | Egypt | South Africa | Kenya | Morocco | Ghana | Algeria | Ethiopia | Tanzania | Sudan | Uganda | Libya | Z… |
| 73 | Ocean Layers | Sunlight | Twilight | Midnight | Abyssal | Hadal | Surface | Deep Ocean | Trench | Zone | Photic | Aphotic | Thermocl… |
| 74 | Architecture Styles | Gothic | Baroque | Romanesque | Renaissance | Modern | Classical | Art Deco | Neoclassical | Contemporary | Victorian… |
| 75 | Types of Bridges | Beam | Arch | Truss | Suspension | Cable-stayed | Cantilever | Viaduct | Drawbridge | Footbridge | Pontoon | Span | P… |
| 76 | Ancient Empires | Roman | Greek | Egyptian | Persian | Ottoman | Aztec | Mayan | Inca | Mongol | Chinese | Babylonian | Carthaginian | … |
| 77 | Philosophers | Plato | Aristotle | Socrates | Kant | Nietzsche | Descartes | Confucius | Locke | Hume | Aquinas | Marx | Foucault | … |
| 78 | Chemical Lab | Beaker | Test Tube | Flask | Pipette | Bunsen Burner | Microscope | Safety Goggles | Centrifuge | Balance | Filter | … |
| 79 | Musical Chords | Major | Minor | Seventh | Diminished | Augmented | Suspended | Root | Inversion | Triad | Arpeggio | Progression | Ch… |
| 80 | Types of Glass | Tempered | Laminated | Borosilicate | Crystal | Fiber | Stained | Safety | Plate | Float | Pane | Sheet | Lens | Pris… |
| 81 | Weather Tools | Thermometer | Barometer | Anemometer | Rain Gauge | Hygrometer | Satellite | Radar | Weather Vane | Radiosonde | Fore… |
| 82 | Types of Bridges | Arch | Truss | Suspension | Beam | Cable-stayed | Cantilever | Viaduct | Drawbridge | Footbridge | Pontoon | Span | P… |
| 83 | Famous Battles | Hastings | Waterloo | Gettysburg | Marathon | Stalingrad | Midway | Thermopylae | Cannae | Bunker Hill | Agincourt | … |
| 84 | Fictional Pets | Hedwig | Snoopy | Pikachu | Toothless | Chewbacca | Garfield | Pluto | Lassie | Toto | Dobby | E.T. | Baloo | Artoo |… |
| 85 | Literary Devices | Metaphor | Simile | Alliteration | Hyperbole | Personification | Irony | Symbolism | Imagery | Onomatopoeia | Allusio… |
| 86 | Volcano Types | Shield | Composite | Cinder Cone | Lava Dome | Caldera | Stratovolcano | Dormant | Active | Extinct | Magma | Eruptio… |
| 87 | Art Techniques | Sketching | Shading | Blending | Perspective | Impressionism | Abstract | Pointillism | Collage | Drawing | Painting … |
| 88 | Photography Styles | Portrait | Landscape | Macro | Street | Abstract | Documentary | Fashion | Still Life | Fine Art | Candid | Wildlife … |
| 89 | Bridge Types | Beam | Arch | Truss | Suspension | Cable-stayed | Cantilever | Viaduct | Drawbridge | Footbridge | Pontoon | Span | P… |
| 215 | Fabrics | Cotton | Silk | Wool | Linen | Denim | Velvet | Leather | Nylon | Polyester | Satin | Lace | Fleece |
| 216 | Patterns | Stripe | Dot | Check | Plaid | Floral | Leopard | Zebra | Solid | Geometric | Abstract | Camo | Zigzag |
| 223 | Casino | Chip | Card | Dice | Slot | Table | Bet | Win | Lose | Money | Dealer | Wheel | Luck |
| 247 | Bank | Money | Teller | ATM | Vault | Check | Card | Account | Loan | Deposit | Safe | Cash | Coin |
| 257 | Math | Add | Subtract | Multiply | Divide | Number | Count | Shape | Graph | Fraction | Equal | Plus | Minus |
| 258 | Science | Experiment | Lab | Test | Atom | Energy | Force | Chemical | Biology | Physics | Space | Life | Study |
| 259 | History | Past | War | King | Queen | Date | Event | Ancient | Leader | Country | Map | Time | Change |
| 260 | Geography | Map | Earth | Land | Water | Mountain | River | Ocean | City | Country | North | South | East |

### 4.4 简单图片（60个词族）
*图片卡，图片内容直观，容易辨认*

| ID | 词族名称（英文） | 完整词汇列表 |
|:--:|----------------|------------|
| 150 | Christmas | 图片卡，共10张图片 |
| 151 | Space | 图片卡，共16张图片 |
| 152 | Drinks | 图片卡，共10张图片 |
| 153 | Animals | 图片卡，共10张图片 |
| 157 | France | 图片卡，共14张图片 |
| 158 | Glasses | 图片卡，共8张图片 |
| 161 | Games | 图片卡，共18张图片 |
| 162 | Chairs | 图片卡，共8张图片 |
| 163 | Fishs | 图片卡，共20张图片 |
| 165 | Leaves | 图片卡，共12张图片 |
| 171 | London | 图片卡，共16张图片 |
| 172 | Hats | 图片卡，共9张图片 |
| 173 | Dogs | 图片卡，共16张图片 |
| 175 | Cats | 图片卡，共16张图片 |
| 177 | Australia | 图片卡，共11张图片 |
| 182 | USA | 图片卡，共14张图片 |
| 183 | Trees | 图片卡，共32张图片 |
| 288 | Motos | 图片卡，共12张图片 |
| 290 | Cakes | 图片卡，共12张图片 |
| 296 | Dinosaurs | 图片卡，共13张图片 |
| 301 | Mushrooms | 图片卡，共23张图片 |
| 302 | Musicians | 图片卡，共9张图片 |
| 303 | Cars | 图片卡，共12张图片 |
| 304 | Flags | 图片卡，共30张图片 |
| 305 | Flags | 图片卡，共31张图片 |
| 306 | Volcanoes | 图片卡，共16张图片 |
| 313 | Italia | 图片卡，共12张图片 |
| 315 | Horses | 图片卡，共12张图片 |
| 316 | Greece | 图片卡，共11张图片 |
| 317 | Germany | 图片卡，共9张图片 |
| 319 | Emojis | 图片卡，共15张图片 |
| 320 | Emojis | 图片卡，共17张图片 |
| 322 | Circus | 图片卡，共10张图片 |
| 323 | Canada | 图片卡，共10张图片 |
| 325 | Birds | 图片卡，共23张图片 |
| 326 | Bicycles | 图片卡，共10张图片 |
| 327 | Bees | 图片卡，共11张图片 |
| 328 | Bears | 图片卡，共9张图片 |
| 332 | Candies | 图片卡，共11张图片 |
| 333 | Pizza | 图片卡，共9张图片 |
| 336 | Lighthouses | 图片卡，共9张图片 |
| 338 | Trains | 图片卡，共9张图片 |
| 379 | Police | 图片卡，共10张图片 |
| 380 | Plumber | 图片卡，共12张图片 |
| 381 | Photography | 图片卡，共10张图片 |
| 382 | Optician | 图片卡，共6张图片 |
| 383 | Laundromat | 图片卡，共15张图片 |
| 384 | Ice cream shop | 图片卡，共10张图片 |
| 385 | Firefighter | 图片卡，共11张图片 |
| 387 | Fairground | 图片卡，共15张图片 |
| 389 | Camper van | 图片卡，共10张图片 |
| 390 | Construction | 图片卡，共13张图片 |
| 391 | Boxing | 图片卡，共11张图片 |
| 392 | Bowling | 图片卡，共8张图片 |
| 398 | Aquarium | 图片卡，共13张图片 |
| 399 | Billiards | 图片卡，共12张图片 |
| 400 | Bakery | 图片卡，共16张图片 |
| 407 | Knights | 图片卡，共8张图片 |
| 408 | Lemon | 图片卡，共9张图片 |
| 410 | Flags | 图片卡，共4张图片 |

### 4.5 中等图片（82个词族）
*图片卡，需要一定知识才能识别图片主题*

| ID | 词族名称（英文） | 完整词汇列表 |
|:--:|----------------|------------|
| 154 | Pastas | 图片卡，共16张图片 |
| 155 | Fruits | 图片卡，共29张图片 |
| 156 | Japan | 图片卡，共16张图片 |
| 159 | Prohibitions | 图片卡，共9张图片 |
| 160 | Water | 图片卡，共14张图片 |
| 164 | Planets | 图片卡，共9张图片 |
| 166 | Sports | 图片卡，共9张图片 |
| 167 | Pool | 图片卡，共11张图片 |
| 168 | Myth | 图片卡，共16张图片 |
| 169 | Music | 图片卡，共14张图片 |
| 170 | Monuments | 图片卡，共16张图片 |
| 174 | Cinema | 图片卡，共8张图片 |
| 176 | Baby | 图片卡，共10张图片 |
| 181 | Monsters | 图片卡，共15张图片 |
| 185 | Gods | 图片卡，共16张图片 |
| 186 | Egypt | 图片卡，共17张图片 |
| 289 | Jobs | 图片卡，共20张图片 |
| 291 | Watches | 图片卡，共10张图片 |
| 292 | Sharks | 图片卡，共12张图片 |
| 294 | Ships | 图片卡，共11张图片 |
| 295 | Zodiac | 图片卡，共12张图片 |
| 297 | Cheeses | 图片卡，共15张图片 |
| 299 | Medieval | 图片卡，共10张图片 |
| 307 | Tools | 图片卡，共12张图片 |
| 308 | Rollercoaster | 图片卡，共10张图片 |
| 309 | Rollercoaster | 图片卡，共10张图片 |
| 310 | Princesses | 图片卡，共8张图片 |
| 311 | Penguins | 图片卡，共12张图片 |
| 314 | India | 图片卡，共10张图片 |
| 318 | Farming | 图片卡，共12张图片 |
| 324 | Butterflies | 图片卡，共8张图片 |
| 329 | Bags | 图片卡，共9张图片 |
| 330 | Toys | 图片卡，共12张图片 |
| 335 | Bridges | 图片卡，共10张图片 |
| 337 | Markets | 图片卡，共9张图片 |
| 339 | Weather | 图片卡，共12张图片 |
| 341 | Tattoo | 图片卡，共12张图片 |
| 342 | Aliens | 图片卡，共11张图片 |
| 343 | Bells | 图片卡，共9张图片 |
| 344 | Boots | 图片卡，共9张图片 |
| 345 | Castle | 图片卡，共12张图片 |
| 346 | Chocolate | 图片卡，共11张图片 |
| 347 | Eggs | 图片卡，共10张图片 |
| 348 | Fairies | 图片卡，共25张图片 |
| 349 | Fountain | 图片卡，共9张图片 |
| 350 | Gardening | 图片卡，共12张图片 |
| 351 | Gloves | 图片卡，共14张图片 |
| 352 | Helicopter | 图片卡，共9张图片 |
| 353 | Hospital | 图片卡，共11张图片 |
| 354 | Islands | 图片卡，共9张图片 |
| 355 | Laundry | 图片卡，共9张图片 |
| 356 | Library | 图片卡，共15张图片 |
| 357 | Lightning | 图片卡，共11张图片 |
| 358 | Milk | 图片卡，共6张图片 |
| 359 | Monkeys | 图片卡，共9张图片 |
| 360 | Ninjas | 图片卡，共12张图片 |
| 361 | Pharmacy | 图片卡，共11张图片 |
| 362 | Radio | 图片卡，共12张图片 |
| 363 | Rainbow | 图片卡，共16张图片 |
| 364 | Sewing | 图片卡，共13张图片 |
| 365 | Skating | 图片卡，共12张图片 |
| 366 | Skyscraper | 图片卡，共12张图片 |
| 367 | Snowman | 图片卡，共10张图片 |
| 368 | Socks | 图片卡，共15张图片 |
| 369 | Balloons | 图片卡，共15张图片 |
| 370 | Tea | 图片卡，共15张图片 |
| 371 | Tent | 图片卡，共9张图片 |
| 372 | Theater | 图片卡，共11张图片 |
| 373 | Yoga | 图片卡，共8张图片 |
| 374 | Winemaker | 图片卡，共12张图片 |
| 376 | Upholsterer | 图片卡，共10张图片 |
| 378 | Pottery | 图片卡，共9张图片 |
| 386 | Fencing | 图片卡，共16张图片 |
| 394 | Blacksmith | 图片卡，共6张图片 |
| 395 | Basketry | 图片卡，共20张图片 |
| 396 | Astronomy | 图片卡，共13张图片 |
| 397 | Archery | 图片卡，共14张图片 |
| 401 | Beekeeper | 图片卡，共13张图片 |
| 402 | China | 图片卡，共23张图片 |
| 403 | Vegetables | 图片卡，共16张图片 |
| 404 | Reptiles | 图片卡，共9张图片 |
| 405 | Spain | 图片卡，共17张图片 |

### 4.6 困难图片（19个词族）
*图片卡，专业图片内容，难度最高*

| ID | 词族名称（英文） | 完整词汇列表 |
|:--:|----------------|------------|
| 178 | Superheroes | 图片卡，共12张图片 |
| 179 | Safari | 图片卡，共16张图片 |
| 180 | Safari | 图片卡，共16张图片 |
| 184 | Halloween | 图片卡，共9张图片 |
| 287 | Martial arts | 图片卡，共16张图片 |
| 293 | Western | 图片卡，共13张图片 |
| 298 | Vikings | 图片卡，共8张图片 |
| 300 | Pirates | 图片卡，共13张图片 |
| 312 | Keys | 图片卡，共10张图片 |
| 321 | Coffees | 图片卡，共16张图片 |
| 331 | Ballet | 图片卡，共15张图片 |
| 334 | Airport | 图片卡，共11张图片 |
| 340 | Jewels | 图片卡，共16张图片 |
| 375 | Volcanology | 图片卡，共10张图片 |
| 377 | Seismology | 图片卡，共11张图片 |
| 388 | Deep sea diver | 图片卡，共9张图片 |
| 393 | Bookbinding | 图片卡，共13张图片 |
| 406 | Clock | 图片卡，共13张图片 |
| 409 | Mayans | 图片卡，共11张图片 |

---

## 五、多语言词库规模对比

| 语言 | 语言代码 | 词族数量 | 内容说明 |
|-----|:-------:|:-------:|---------|
| 法语 | fr | 796 | 内容最丰富，796个词族，含大量法语文化专属内容 |
| 英语 | en | 411 | 内容最完整，411个词族，含专属英语文化词族 |
| 意大利语 | it | 409 | 409个词族，接近英语规模 |
| 德语 | de | 370 | 370个词族 |
| 日语 | ja | 370 | 370个词族 |
| 俄语 | ru | 370 | 370个词族 |
| 西班牙语 | es | 260 | 260个词族 |
| 葡萄牙语 | pt | 260 | 260个词族 |

---

## 六、关卡奖励系统

### 6.1 关卡1-20累计奖励

- **累计金币：** 430 金币（平均每关 21.5 金币）
- **提示道具：** 共 3 个
- **撤回道具：** 共 3 个

### 6.2 里程碑关卡（含额外道具奖励）

| 关卡 | 金币奖励 | 额外道具 | 关卡特征 |
|:----:|:-------:|---------|---------|
| 第7关 | 20金币 | 提示×1 | 难度提升（30张卡/6族） |
| 第10关 | 30金币 | 撤回×1 | 升级为5×5布局，68张卡/12族 |
| 第12关 | 20金币 | 提示×1 | 难度提升（54张卡/10族） |
| 第15关 | 20金币 | 撤回×1 | 升级为5×5布局，68张卡/12族 |
| 第17关 | 20金币 | 提示×1 | 难度提升（54张卡/10族） |
| 第20关 | 30金币 | 撤回×1 | 升级为5×5布局，80张卡/14族 |

### 6.3 道具价格与获取速度

| 道具 | 金币价格 | 等效完成关数 | 初始持有 |
|-----|:-------:|:----------:|:-------:|
| 提示（Hint） | 300 | ≈14.0关 | 3 |
| 撤回（Undo） | 300 | ≈14.0关 | 3 |
| 小丑（Joker） | 300 | ≈14.0关 | 5 |
| 加10步（More Move） | 600 | ≈27.9关 | — |

---

## 七、关卡难度曲线分析

### 7.1 规模与步效比变化

```
  关卡    卡数    族数      步数     步/卡    卡/族  布局
────────────────────────────────────────────────────────────
 Lv 1    30     6    ∞        —      5.0  3基础×3堆
 Lv 2    25     5    ∞        —      5.0  3基础×3堆
 Lv 3    33     6     150    4.55    5.5  4基础×4堆
 Lv 4    33     6     150    4.55    5.5  4基础×4堆
 Lv 5    42     7     145    3.45    6.0  4基础×4堆
 Lv 6    48     7     145    3.02    6.9  4基础×4堆
 Lv 7    30     6     200    6.67    5.0  4基础×4堆
 Lv 8    30     6      75    2.50    5.0  4基础×4堆
 Lv 9    54    10     120    2.22    5.4  4基础×4堆
 Lv10    68    12     160    2.35    5.7  5基础×5堆
 Lv11    30     6      75    2.50    5.0  3基础×3堆
 Lv12    54    10     120    2.22    5.4  4基础×4堆
 Lv13    54    10     120    2.22    5.4  4基础×4堆
 Lv14    68    12     200    2.94    5.7  4基础×4堆
 Lv15    68    12     160    2.35    5.7  5基础×5堆
 Lv16    30     6      75    2.50    5.0  4基础×4堆
 Lv17    54    10     120    2.22    5.4  4基础×4堆
 Lv18    82    14     220    2.68    5.9  4基础×4堆
 Lv19    54    10     120    2.22    5.4  4基础×4堆
 Lv20    80    14     200    2.50    5.7  5基础×5堆
```

### 7.2 设计规律总结

| 阶段 | 关卡范围 | 核心特征 |
|------|:-------:|---------|
| 新手引导 | 1-2关 | 无步数限制，固定简单内容（30张/6族），完全引导 |
| 过渡期 | 3-6关 | 引入步数限制（≤150步），卡片逐渐增加至48张 |
| 图片入门 | 7-8关 | 首次大量出现图片卡（50%比例），卡片减少至30张降低压力 |
| 规模提升 | 9-10关 | 族数首次突破10个，第10关5×5布局里程碑 |
| 循环变化 | 11-20关 | 「小关（30张/6族）」与「大关（54-82张/10-14族）」交替 |
| 极限挑战 | 18关 | 全游戏最大规模：82张卡/14族，图片卡占70% |
| 无限循环 | 21+关 | 复用11-20关配置，词族每次随机抽取，无限可玩 |

---

## 附录：核心配置参数（config.json）

```json
  "stock_coins": 500,  // 初始金币
  "stock_hints": 3,  // 初始提示道具数
  "stock_undos": 3,  // 初始撤回道具数
  "stock_jokers": 5,  // 初始小丑道具数
  "price_hint": 300,  // 提示价格（金币）
  "price_undo": 300,  // 撤回价格（金币）
  "price_joker": 300,  // 小丑价格（金币）
  "price_more_move": 600,  // 加步价格（金币）
  "more_move_value": 10,  // 每次增加步数
  "free_coins_given_value": 100,  // 免费金币数量（看广告获取）
  "ad_disable_until_level": 5,  // 广告豁免关卡数（前N关无广告）
  "ad_disable_interstitial_during_gameplay_until_level": 20,  // 游戏过程无插屏广告的关卡数
  "ad_banner_delay_first_launch": 300,  // 首次启动横幅广告延迟（秒）
  "ad_banner_delay": 15,  // 横幅广告间隔（秒）
  "ad_interstitial_delay": 60,  // 插屏广告间隔（秒）
  "ad_interstitial_capping": 130,  // 插屏广告最大冷却（秒）
  "start_more_move_retries": 2,  // 可购买加步的最大次数
  "coins_per_moves_remaining": 0,  // 每剩余一步获得金币数
```
