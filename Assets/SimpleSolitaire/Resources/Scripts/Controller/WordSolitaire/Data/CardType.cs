using UnityEngine;

namespace SimpleSolitaire.Controller.WordSolitaire
{
    /// <summary>
    /// 卡牌类型枚举 - 定义单词卡牌的可视化类型
    /// </summary>
    public enum CardType
    {
        /// <summary>
        /// 文字卡牌 - 显示单词文本
        /// </summary>
        Text,
        
        /// <summary>
        /// 图片卡牌 - 显示图片内容
        /// </summary>
        Image,
        
        /// <summary>
        /// 万能卡 - 可放入任意分类槽
        /// </summary>
        Joker
    }
}
