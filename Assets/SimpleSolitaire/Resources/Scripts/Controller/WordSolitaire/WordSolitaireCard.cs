using UnityEngine;

namespace SimpleSolitaire.Controller.WordSolitaire
{
    /// <summary>
    /// 卡牌类型
    /// </summary>
    public enum CardVisualType
    {
        Text,      // 文字卡牌
        Image,     // 图片卡牌
        Joker      // 万能卡
    }
    
    /// <summary>
    /// Word Solitaire 词语卡牌
    /// </summary>
    public class WordSolitaireCard : Card
    {
        [Header("词语卡牌属性")]
        public string WordId;                  // 单词ID
        public string CategoryId;              // 所属类别ID
        public CardVisualType VisualType;      // 卡牌视觉类型
        public Sprite WordImage;               // 单词图片（仅图片卡使用）
        
        /// <summary>
        /// 是否是万能卡
        /// </summary>
        public bool IsJoker => VisualType == CardVisualType.Joker;
        
        /// <summary>
        /// 获取卡牌类型名称
        /// </summary>
        public override string GetTypeName()
        {
            return $"Word_{CategoryId}_{WordId}";
        }
    }
}
