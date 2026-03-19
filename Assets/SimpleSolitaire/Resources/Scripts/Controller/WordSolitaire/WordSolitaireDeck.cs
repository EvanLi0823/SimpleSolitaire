using UnityEngine;

namespace SimpleSolitaire.Controller.WordSolitaire
{
    /// <summary>
    /// 牌堆类型
    /// </summary>
    public enum WordDeckType
    {
        Pack,          // 牌库
        Hand,          // 手牌区
        Column,        // 列区
        CategorySlot   // 分类槽
    }
    
    /// <summary>
    /// Word Solitaire 牌堆
    /// </summary>
    public class WordSolitaireDeck : Deck
    {
        [Header("Word Solitaire 牌堆属性")]
        public WordDeckType DeckType;          // 牌堆类型
        public string CategoryId;              // 类别ID（仅CategorySlot使用）
        public int TargetCardCount;            // 目标卡牌数量（仅CategorySlot使用）
        
        /// <summary>
        /// 当前已放置的卡牌数量
        /// </summary>
        public int CurrentCardCount => CardsArray.Count;
        
        /// <summary>
        /// 检查是否已完成（仅CategorySlot使用）
        /// </summary>
        public bool IsComplete => DeckType == WordDeckType.CategorySlot && 
                                  CurrentCardCount >= TargetCardCount;
        
        /// <summary>
        /// 检查是否可以接受卡牌
        /// </summary>
        public override bool AcceptCard(Card card)
        {
            WordSolitaireCard wordCard = card as WordSolitaireCard;
            if (wordCard == null) return false;
            
            switch (DeckType)
            {
                case WordDeckType.CategorySlot:
                    // 分类槽：类别必须匹配，或者是万能卡
                    if (wordCard.IsJoker)
                    {
                        return true;
                    }
                    return wordCard.CategoryId == CategoryId;
                    
                case WordDeckType.Column:
                    // 列区：总是接受（用于整理）
                    return true;
                    
                case WordDeckType.Hand:
                    // 手牌区：不接受外部卡牌
                    return false;
                    
                case WordDeckType.Pack:
                    // 牌库：不接受外部卡牌
                    return false;
                    
                default:
                    return false;
            }
        }
        
        /// <summary>
        /// 获取牌堆类型
        /// </summary>
        public override DeckType Type
        {
            get
            {
                switch (DeckType)
                {
                    case WordDeckType.Pack:
                        return DeckType.DECK_TYPE_PACK;
                    case WordDeckType.Hand:
                        return DeckType.DECK_TYPE_WASTE;
                    case WordDeckType.Column:
                        return DeckType.DECK_TYPE_BOTTOM;
                    case WordDeckType.CategorySlot:
                        return DeckType.DECK_TYPE_ACE;
                    default:
                        return DeckType.DECK_TYPE_BOTTOM;
                }
            }
        }
    }
}
