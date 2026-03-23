# WordSolitaire CSV数据配置说明

> 本目录包含WordSolitaire游戏的CSV配置数据，用于定义关卡、词组、词汇和多语言文本。

## 文件说明

### 1. Categories.csv - 词组类别表

定义所有词组(类别)的基础信息。

| 字段 | 类型 | 说明 |
|------|------|------|
| categoryId | string | 词组唯一ID |
| nameKey | string | 词组名称的多语言Key |
| iconPath | string | 类别图标路径 |
| difficulty | string | 难度级别(easy/medium/hard/image) |

### 2. Words.csv - 词汇表

定义所有单词卡牌及其所属词组。

| 字段 | 类型 | 说明 |
|------|------|------|
| wordId | string | 单词唯一ID |
| categoryId | string | 所属词组ID |
| textKey | string | 单词文本的多语言Key |
| cardType | string | 卡牌类型(Text/Image/Joker) |
| imagePath | string | 图片路径(仅Image类型) |

### 3. Levels.csv - 关卡表

定义每个关卡的配置参数。

| 字段 | 类型 | 说明 |
|------|------|------|
| levelId | int | 关卡ID |
| cardCount | int | 卡牌总数 |
| maxMoves | int | 最大步数(999表示无限) |
| columnCount | int | 列区数量 |
| slotCount | int | 分类槽数量 |
| categoryIds | string | 涉及的词组ID(用|分隔) |
| isTutorial | bool | 是否引导关卡 |
| isShowResultAd | bool | 关卡结算时是否播放插屏广告 |
| isShowMatchAd | bool | 第一个分类卡集齐时是否播放插屏广告 |

### 4. Localization.csv - 多语言文本表

所有多语言Key对应的实际文本。

| 字段 | 类型 | 说明 |
|------|------|------|
| key | string | 多语言Key |
| en | string | 英文文本 |
| zh | string | 简体中文文本 |
| ja | string | 日语文本 |
| ko | string | 韩语文本 |

## 当前数据规模

- **词组类别**: 10个基础类别
- **词汇数量**: 50个单词(每类别5个)
- **关卡数量**: 5个关卡(2个引导关卡 + 3个正式关卡)
- **支持语言**: 英语、简体中文、日语、韩语

## MVP版本目标(扩展)

根据产品文档，MVP版本需要扩展至：
- 30个类别
- 300个词汇(每类别10个)
- 20个关卡

## 使用说明

这些CSV文件需要通过Unity Editor工具转换为ScriptableObject，具体转换工具开发可参考 `Documents/WordSolitaire数据配置表设计.md`。
