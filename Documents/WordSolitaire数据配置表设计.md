# WordSolitaire 数据配置表设计文档

> **文档目的**: 定义关卡、词汇、词组表的CSV配置格式,便于通过工具转换为Unity ScriptableObject
> **版本**: v1.0
> **创建日期**: 2026-03-19

---

## 一、概述

本文档定义了WordSolitaire游戏的CSV数据配置表格式,采用多语言Key设计,所有本地化文本通过`LocalizationManager`统一管理。

**设计原则**:
- CSV中只包含数据Key,不包含实际多语言文本
- 所有多语言文本集中在`Localization.csv`中管理
- 便于Excel/Google Sheets编辑和版本控制
- 支持一键转换为Unity ScriptableObject

---

## 二、词组表 (Categories.csv)

定义所有词组(类别)的基础信息。

### CSV格式示例

```csv
categoryId,nameKey,iconPath,difficulty,descriptionKey
animals,category_animals,category_icons/animals.png,easy,category_animals_desc
fruits,category_fruits,category_icons/fruits.png,easy,category_fruits_desc
colors,category_colors,category_icons/colors.png,easy,category_colors_desc
transport,category_transport,category_icons/transport.png,easy,category_transport_desc
animals_image,category_animals_image,category_icons/animals.png,image,category_animals_image_desc
```

### 字段说明

| 字段名 | 类型 | 必填 | 说明 |
|--------|------|------|------|
| `categoryId` | string | 是 | 词组唯一ID(数据库主键) |
| `nameKey` | string | 是 | 词组名称的多语言Key(如: `category_animals`) |
| `iconPath` | string | 是 | 类别图标路径(Unity Resources相对路径) |
| `difficulty` | string | 是 | 难度级别: `easy` / `medium` / `hard` / `image` |
| `descriptionKey` | string | 否 | 词组描述的多语言Key |

### 多语言Key命名规范

- **词组名称Key**: `category_{categoryId}`
- **词组描述Key**: `category_{categoryId}_desc`

### 使用示例

```csharp
// 通过LocalizationManager获取本地化文本
string categoryName = LocalizationManager.Get("category_animals");
string description = LocalizationManager.Get("category_animals_desc");
```

---

## 三、词汇表 (Words.csv)

定义所有单词卡牌及其所属词组。

### CSV格式示例

```csv
wordId,categoryId,textKey,cardType,imagePath,hintKey
animals_001,animals,word_dog,text,,hint_dog
animals_002,animals,word_cat,text,,hint_cat
animals_003,animals,word_elephant,text,,hint_elephant
animals_image_001,animals_image,word_dog,image,animal_images/dog.jpg,hint_dog
animals_image_002,animals_image,word_cat,image,animal_images/cat.jpg,hint_cat
fruits_001,fruits,word_apple,text,,hint_apple
fruits_002,fruits,word_banana,text,,hint_banana
colors_001,colors,word_red,text,,hint_red
colors_002,colors,word_blue,text,,hint_blue
colors_003,colors,word_yellow,text,,hint_yellow
```

### 字段说明

| 字段名 | 类型 | 必填 | 说明 |
|--------|------|------|------|
| `wordId` | string | 是 | 单词唯一ID(格式: `categoryId_序号`) |
| `categoryId` | string | 是 | 所属词组ID(关联Categories.csv) |
| `textKey` | string | 是 | 单词文本的多语言Key(如: `word_dog`) |
| `cardType` | string | 是 | 卡牌类型: `text`(文字) 或 `image`(图片) |
| `imagePath` | string | 条件 | 图片卡牌的图片路径(仅cardType=image时必填) |
| `hintKey` | string | 否 | 提示文本的多语言Key |

### 卡牌类型说明

| 类型 | 说明 | 示例 |
|------|------|------|
| `text` | 文字卡牌,显示单词文本 | `word_dog` → "狗" |
| `image` | 图片卡牌,显示图片 | 动物图片、水果图片 |

### 万能卡特殊配置

#### 在Categories.csv中配置万能卡类别

```csv
categoryId,nameKey,iconPath,difficulty,descriptionKey
joker,category_joker,category_icons/joker.png,easy,category_joker_desc
```

#### 在Words.csv中配置万能卡

```csv
wordId,categoryId,textKey,cardType,imagePath,hintKey
joker_001,joker,word_joker,joker,cards/joker.png,
joker_002,joker,word_joker,joker,cards/joker.png,
```

**万能卡配置规则**:
1. 必须创建categoryId为`joker`的类别
2. 万能卡的`cardType`必须为`joker`
3. 万能卡的`textKey`建议使用`word_joker`(所有万能卡共用同一文本)
4. 万能卡的`imagePath`必须填写,显示万能卡图标
5. 万能卡的`hintKey`通常为空(不需要额外提示)

### 多语言Key命名规范

- **单词文本Key**: `word_{wordId}`
- **提示文本Key**: `hint_{wordId}`

### 使用示例

```csharp
// 获取单词文本
string wordText = LocalizationManager.Get("word_dog"); // 返回"狗"或"dog"

// 获取提示文本
string hintText = LocalizationManager.Get("hint_dog"); // 返回"人类最好的朋友"
```

---

## 四、关卡表 (Levels.csv)

定义每个关卡的配置参数。

### CSV格式示例

```csv
levelId,cardCount,maxMoves,columnCount,slotCount,categoryIds,isTutorial,difficultyMix_easy,difficultyMix_medium,difficultyMix_hard,difficultyMix_image,initialHints,initialUndos,initialJokers,descriptionKey
1,30,999,4,5,animals|fruits|colors|transport|body_parts,true,100,0,0,0,3,3,1,level_1_desc
2,32,999,4,5,animals|fruits|colors|transport|clothes,true,100,0,0,0,3,3,1,level_2_desc
3,48,150,4,6,animals|fruits|colors|transport|food|drinks,false,80,20,0,0,3,2,1,level_3_desc
4,60,145,4,7,animals|fruits|colors|transport|food|drinks|vegetables,false,60,30,0,10,3,2,1,level_4_desc
5,68,200,5,8,animals|fruits|colors|transport|food|drinks|vegetables|clothes,false,50,30,10,10,3,2,1,level_5_desc
```

### 字段说明

| 字段名 | 类型 | 必填 | 说明 |
|--------|------|------|------|
| `levelId` | int | 是 | 关卡ID(从1开始) |
| `cardCount` | int | 是 | 卡牌总数(该关卡中所有卡牌数量) |
| `maxMoves` | int | 是 | 最大步数(999表示无限) |
| `columnCount` | int | 是 | 列区数量(如: 4) |
| `slotCount` | int | 是 | 分类槽数量(如: 5) |
| `categoryIds` | string | 是 | 涉及的词组ID列表(用 `\|` 分隔) |
| `isTutorial` | bool | 是 | 是否引导关卡(true/false) |
| `difficultyMix_easy` | int | 是 | Easy难度卡牌占比(0-100) |
| `difficultyMix_medium` | int | 是 | Medium难度卡牌占比(0-100) |
| `difficultyMix_hard` | int | 是 | Hard难度卡牌占比(0-100) |
| `difficultyMix_image` | int | 是 | Image难度(图片卡)占比(0-100) |
| `initialHints` | int | 否 | 初始提示道具数量(默认3) |
| `initialUndos` | int | 否 | 初始撤回道具数量(默认3) |
| `initialJokers` | int | 否 | 初始万能牌道具数量(默认1)<br>**注意**: 万能卡仅作为道具使用,不会出现在关卡卡牌中。玩家点击道具栏万能卡按钮后,下一次拖拽卡牌可放入任意分类槽
| `descriptionKey` | string | 否 | 关卡描述的多语言Key |

### 重要规则

1. **difficultyMix总和必须为100**: `easy + medium + hard + image = 100`
2. **categoryIds格式**: 多个categoryId用竖线 `\|` 分隔,如: `animals|fruits|colors`
3. **引导关卡**: `isTutorial=true`时,`maxMoves`应设置为999(无限步数)

### 多语言Key命名规范

- **关卡描述Key**: `level_{levelId}_desc`

### 使用示例

```csharp
// 获取关卡描述
string levelDesc = LocalizationManager.Get("level_1_desc"); // 返回"教学关卡1"
```

---

## 五、可选扩展表

### 5.1 关卡奖励表 (LevelRewards.csv)

定义每个关卡通关后的奖励。

```csv
levelId,coinsReward,unlockFeature,showInterstitialAd,descriptionKey
1,0,false,false,levelreward_1_desc
5,30,false,true,levelreward_5_desc
10,50,true,false,levelreward_10_desc
```

**字段说明**:
- `levelId`: 关卡ID
- `coinsReward`: 通关奖励金币(0表示无奖励)
- `unlockFeature`: 是否解锁新功能(true/false)
- `showInterstitialAd`: 通关后是否显示插屏广告(true/false)
- `descriptionKey`: 奖励描述的多语言Key

### 5.2 里程碑表 (Milestones.csv)

定义玩家达到特定关卡时的里程碑奖励。

```csv
milestoneId,levelId,rewardType,rewardAmount,descriptionKey
1,5,coins,50,milestone_5_desc
2,10,coins,100,milestone_10_desc
3,20,hints,5,milestone_20_desc
4,30,jokers,3,milestone_30_desc
```

**字段说明**:
- `milestoneId`: 里程碑ID
- `levelId`: 对应关卡数
- `rewardType`: 奖励类型(`coins`/`hints`/`undos`/`jokers`)
- `rewardAmount`: 奖励数量
- `descriptionKey`: 描述多语言Key

### 5.3 万能卡道具说明

**重要**: 万能卡仅作为道具使用,不会配置在关卡卡牌中。

#### 5.3.1 万能卡的使用方式

1. **道具来源**: 通过`Levels.csv`中的`initialJokers`字段配置初始数量
2. **使用方式**: 玩家点击底部道具栏的万能卡按钮
3. **效果**: 激活后,下一次拖拽任意卡牌可放入任意分类槽(该卡牌临时获得万能特性)
4. **消耗**: 每次使用消耗1个万能卡道具

#### 5.3.2 配置示例

在Levels.csv中配置初始万能卡数量:

```csv
levelId,cardCount,maxMoves,columnCount,slotCount,categoryIds,isTutorial,difficultyMix_easy,difficultyMix_medium,difficultyMix_hard,difficultyMix_image,initialHints,initialUndos,initialJokers,descriptionKey
1,30,999,4,5,animals|fruits|colors|transport|body_parts,true,100,0,0,0,3,3,1,level_1_desc
3,48,150,4,6,animals|fruits|colors|transport|food|drinks,false,80,20,0,0,3,2,2,level_3_desc
```

**说明**: 
- Level 1: 初始1个万能卡道具
- Level 3: 初始2个万能卡道具

#### 5.3.3 游戏内表现

- **道具栏显示**: 底部道具栏显示万能卡图标(🃏)和剩余数量
- **激活状态**: 点击后高亮显示,提示玩家已激活万能卡功能
- **使用效果**: 下一次拖拽任意卡牌时,所有分类槽都允许放置
- **使用后**: 万能卡数量减1,激活状态取消

#### 5.3.4 实现逻辑

```csharp
public class JokerItem : MonoBehaviour
{
    private bool isJokerActivated = false;
    
    public void ActivateJoker()
    {
        isJokerActivated = true;
        // 高亮显示激活状态
        UIManager.Instance.ShowJokerActivated();
    }
    
    public bool CanPlaceAnywhere()
    {
        if (isJokerActivated)
        {
            isJokerActivated = false;
            UIManager.Instance.HideJokerActivated();
            return true;
        }
        return false;
    }
}
```

---

## 六、多语言文本配置表 (Localization.csv)

所有多语言Key对应的实际文本,由LocalizationManager统一管理。

### CSV格式示例

```csv
key,en,zh,ja,ko,fr,es,de
category_animals,Animals,动物,動物,동물,Animaux,Animales,Tiere
category_animals_desc,Daily common animals,日常常见动物,日常的な動物,일상 동물,Animaux communs du quotidien,Animales comunes del día a día,Alltägliche Tiere
category_fruits,Fruits,水果,果物,과일,Fruits,Frutas,Früchte
word_dog,dog,狗,犬,개,chien,perro,Hund
hint_dog,Man's best friend,人类最好的朋友,人間の最良の友,인간의 가장 친한 친구,Le meilleur ami de l'homme,El mejor amigo del hombre,Des Menschen bester Freund
word_cat,cat,猫,猫,고양이,chat,gato,Katze
hint_cat,Small furry pet,毛茸茸的小宠物,小さな毛玉のようなペット,작고 털이 많은 애완 동물,Petit animal poilu,Peludo animalito,Kleines pelziges Haustier
level_1_desc,Tutorial Level 1,教学关卡1,チュートリアルレベル1,튜토리얼 레벨 1,Niveau tutoriel 1,Nivel tutorial 1,Tutorial Level 1
```

### 字段说明

| 字段名 | 类型 | 必填 | 说明 |
|--------|------|------|------|
| `key` | string | 是 | 多语言Key(唯一标识符) |
| `en` | string | 是 | 英文文本 |
| `zh` | string | 是 | 简体中文文本 |
| `ja` | string | 是 | 日语文本 |
| `ko` | string | 是 | 韩语文本 |
| `fr` | string | 否 | 法语文本(可选) |
| `es` | string | 否 | 西班牙语文本(可选) |
| `de` | string | 否 | 德语文本(可选) |

### Key命名规范总结

| Key类型 | 命名格式 | 示例 |
|---------|----------|------|
| 词组名称 | `category_{categoryId}` | `category_animals` |
| 词组描述 | `category_{categoryId}_desc` | `category_animals_desc` |
| 单词文本 | `word_{wordId}` | `word_dog` |
| 提示文本 | `hint_{wordId}` | `hint_dog` |
| 关卡描述 | `level_{levelId}_desc` | `level_1_desc` |
| 关卡奖励 | `levelreward_{levelId}_desc` | `levelreward_5_desc` |
| 里程碑 | `milestone_{levelId}_desc` | `milestone_10_desc` |

---

## 七、ScriptableObject数据结构

### 7.1 CategoryData.cs

```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "CategoryData", menuName = "WordSolitaire/Category Data")]
public class CategoryData : ScriptableObject
{
    [Header("基础信息")]
    public string categoryId;
    public string nameKey; // 多语言Key: category_{categoryId}
    
    [Header("视觉")]
    public Sprite icon;
    
    [Header("难度")]
    public string difficulty; // easy/medium/hard/image
    
    [Header("描述")]
    public string descriptionKey; // 多语言Key: category_{categoryId}_desc
}
```

### 7.2 WordData.cs

```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "WordData", menuName = "WordSolitaire/Word Data")]
public class WordData : ScriptableObject
{
    [Header("基础信息")]
    public string wordId;
    public string categoryId; // 关联CategoryData.categoryId
    
    [Header("文本")]
    public string textKey; // 多语言Key: word_{wordId}
    
    [Header("类型")]
    public string cardType; // text/image
    
    [Header("图片")]
    public Sprite image; // 仅cardType=image时使用
    
    [Header("提示")]
    public string hintKey; // 多语言Key: hint_{wordId}
}
```

### 7.3 LevelData.cs

```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "WordSolitaire/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("基础信息")]
    public int levelId;
    public string descriptionKey; // 多语言Key: level_{levelId}_desc
    
    [Header("关卡配置")]
    public int cardCount;
    public int maxMoves;
    public int columnCount;
    public int slotCount;
    public string[] categoryIds; // CategoryData.categoryId数组
    
    [Header("引导关卡")]
    public bool isTutorial;
    
    [Header("难度配比")]
    [Range(0, 100)]
    public int difficultyMixEasy;
    [Range(0, 100)]
    public int difficultyMixMedium;
    [Range(0, 100)]
    public int difficultyMixHard;
    [Range(0, 100)]
    public int difficultyMixImage;
    
    [Header("初始道具")]
    public int initialHints = 3;
    public int initialUndos = 3;
    public int initialJokers = 1;
}
```

### 7.4 LevelRewardData.cs (可选)

```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "LevelRewardData", menuName = "WordSolitaire/Level Reward Data")]
public class LevelRewardData : ScriptableObject
{
    public int levelId;
    public int coinsReward;
    public bool unlockFeature;
    public bool showInterstitialAd;
    public string descriptionKey;
}
```

### 7.5 MilestoneData.cs (可选)

```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "MilestoneData", menuName = "WordSolitaire/Milestone Data")]
public class MilestoneData : ScriptableObject
{
    public int milestoneId;
    public int levelId;
    public string rewardType; // coins/hints/undos/jokers
    public int rewardAmount;
    public string descriptionKey;
}
```

---

## 八、文件存放路径建议

```
Assets/SimpleSolitaire/Resources/Data/WordSolitaire/
├── CSV/                              # 原始CSV文件(编辑器使用)
│   ├── Categories.csv
│   ├── Words.csv
│   ├── Levels.csv
│   ├── LevelRewards.csv (可选)
│   ├── Milestones.csv (可选)
│   └── Localization.csv
│
└── ScriptableObjects/                # 生成的ScriptableObject(运行时读取)
    ├── Categories/
    │   ├── Animals.asset
    │   ├── Fruits.asset
    │   └── ...
    ├── Words/
    │   ├── Dog.asset
    │   ├── Cat.asset
    │   └── ...
    ├── Levels/
    │   ├── Level1.asset
    │   ├── Level2.asset
    │   └── ...
    ├── LevelRewards/ (可选)
    │   ├── Level1Reward.asset
    │   └── ...
    └── Milestones/ (可选)
        ├── Milestone1.asset
        └── ...
```

---

## 九、CSV转ScriptableObject工具开发要点

### 9.1 工具功能需求

开发一个Unity Editor工具,支持以下功能:

1. **一键转换**: 批量转换所有CSV文件为ScriptableObject
2. **增量更新**: 只转换修改过的CSV文件
3. **数据验证**: 转换前验证数据完整性和正确性
4. **错误提示**: 详细的错误信息和行号定位
5. **进度显示**: 转换进度条和状态提示

### 9.2 数据验证规则

转换时必须验证以下内容:

#### 基础验证
- [ ] CSV文件存在且格式正确
- [ ] 必填字段不为空
- [ ] 数据类型转换正确(int/string/bool)

#### 关联验证
- [ ] WordData中的`categoryId`必须在Categories.csv中存在
- [ ] LevelData中的`categoryIds`必须都有效
- [ ] LevelRewardData中的`levelId`必须在Levels.csv中存在

#### 数值验证
- [ ] LevelData中的`difficultyMix`总和必须为100
- [ ] 数值字段在合理范围内(int为正数,bool为true/false)

#### 路径验证
- [ ] `iconPath`和`imagePath`对应的资源文件存在
- [ ] `Localization.csv`中所有Key都存在

### 9.3 转换流程

```csharp
// 伪代码示例
public class CSVToSOConverter
{
    public void ConvertAll()
    {
        // 1. 加载所有CSV文件
        var categories = LoadCSV("Categories.csv");
        var words = LoadCSV("Words.csv");
        var levels = LoadCSV("Levels.csv");
        var localization = LoadCSV("Localization.csv");
        
        // 2. 数据验证
        ValidateData(categories, words, levels);
        
        // 3. 转换为ScriptableObject
        ConvertCategories(categories);
        ConvertWords(words);
        ConvertLevels(levels);
        
        // 4. 保存到磁盘
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
```

### 9.4 Editor工具界面

```csharp
public class WordSolitaireDataConverterWindow : EditorWindow
{
    [MenuItem("Tools/WordSolitaire/Convert CSV to SO")]
    public static void ShowWindow()
    {
        GetWindow<WordSolitaireDataConverterWindow>("CSV Converter");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("CSV to ScriptableObject Converter", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Convert All CSV"))
        {
            ConvertAll();
        }
        
        if (GUILayout.Button("Validate Data"))
        {
            ValidateAll();
        }
        
        // 显示转换日志
        DisplayLogs();
    }
}
```

---

## 十、使用示例

### 10.1 加载词组数据

```csharp
// 方式1: 通过Resources加载
CategoryData animalsData = Resources.Load<CategoryData>("Data/WordSolitaire/ScriptableObjects/Categories/Animals");
string categoryName = LocalizationManager.Get(animalsData.nameKey);

// 方式2: 通过数据管理器加载
public class WordDataManager : MonoBehaviour
{
    public List<CategoryData> categories;
    public List<WordData> words;
    public List<LevelData> levels;
    
    private void Awake()
    {
        // 加载所有ScriptableObject
        categories = new List<CategoryData>(Resources.LoadAll<CategoryData>("Data/WordSolitaire/ScriptableObjects/Categories"));
        words = new List<WordData>(Resources.LoadAll<WordData>("Data/WordSolitaire/ScriptableObjects/Words"));
        levels = new List<LevelData>(Resources.LoadAll<LevelData>("Data/WordSolitaire/ScriptableObjects/Levels"));
    }
}
```

### 10.2 获取本地化文本

```csharp
public class LocalizationManager
{
    private static Dictionary<string, string> currentLanguageTexts;
    
    // 初始化时加载Localization.csv
    public static void Initialize(SystemLanguage language)
    {
        // 从Resources加载Localization.csv
        TextAsset csv = Resources.Load<TextAsset>("Data/WordSolitaire/CSV/Localization");
        // 解析CSV并存储到currentLanguageTexts字典
    }
    
    // 根据key获取本地化文本
    public static string Get(string key)
    {
        if (currentLanguageTexts.TryGetValue(key, out string value))
            return value;
        
        Debug.LogWarning($"[Localization] Key not found: {key}");
        return key; // 返回key作为fallback
    }
}
```

---

## 十一、最佳实践

### 11.1 CSV编辑建议

1. **使用Excel或Google Sheets编辑**: 便于批量修改和数据验证
2. **版本控制**: 将CSV文件纳入Git版本控制
3. **备份**: 定期备份CSV文件
4. **注释**: 在CSV第一行添加字段说明注释(转换工具需跳过)

### 11.2 Key命名规范

1. **统一前缀**: 使用`category_`、`word_`、`hint_`、`level_`等前缀
2. **语义化**: Key应具有描述性,如`word_elephant`而不是`word_001`
3. **避免重复**: 确保Key在全局唯一

### 11.3 数据管理

1. **小批量测试**: 先配置少量数据测试,确认无误后再批量添加
2. **数据审查**: 定期检查数据完整性和一致性
3. **性能优化**: 关卡数量多时,考虑异步加载和分页加载

---

## 十二、附录

### 12.1 多语言Key完整示例

```csv
key,en,zh,ja,ko
category_animals,Animals,动物,動物,동물
category_animals_desc,Daily common animals,日常常见动物,日常的な動物,일상 동물
category_fruits,Fruits,水果,果物,과일
category_fruits_desc,Common fruits,常见水果,一般的な果物,일반 과일
word_dog,dog,狗,犬,개
hint_dog,Man's best friend,人类最好的朋友,人間の最良の友,인간의 가장 친한 친구
word_cat,cat,猫,猫,고양이
hint_cat,Small furry pet,毛茸茸的小宠物,小さな毛玉のようなペット,작고 털이 많은 애완 동물
word_elephant,elephant,大象,象,코끼리
hint_elephant,Largest land animal,最大的陆生动物,最大の陸上動物,가장 큰 육상 동물
level_1_desc,Tutorial Level 1,教学关卡1,チュートリアルレベル1,튜토리얼 레벨 1
level_2_desc,Tutorial Level 2,教学关卡2,チュートリアルレベル2,튜토리얼 레벨 2
level_3_desc,Easy Level 3,简单关卡3,簡単レベル3,쉬운 레벨 3
```

### 12.2 参考资源

- Unity ScriptableObject官方文档: https://docs.unity3d.com/Manual/class-ScriptableObject.html
- CSV格式规范: https://tools.ietf.org/html/rfc4180

---

*文档结束 - 数据配置表设计已固化*
