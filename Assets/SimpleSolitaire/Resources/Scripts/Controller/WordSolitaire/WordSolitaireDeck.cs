using UnityEngine;
using System.Collections.Generic;
using SimpleSolitaire.Model.Enum;

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
    /// 继承自Deck基类，实现词语牌堆的特定逻辑
    /// </summary>
    public class WordSolitaireDeck : Deck
    {
        [Header("Word Solitaire 牌堆属性")]
        public WordDeckType DeckType;          // 牌堆类型
        public string CategoryId;              // 类别ID（仅CategorySlot使用）
        public int TargetCardCount;            // 目标卡牌数量（仅CategorySlot使用）
        public int ColumnIndex;                // 列索引（仅Column使用）
        
        /// <summary>
        /// 当前已放置的卡牌数量
        /// </summary>
        public int CurrentCardCount => CardsArray?.Count ?? 0;
        
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
                    // 分类槽已满，不接受
                    if (IsComplete)
                        return false;
                    // 分类槽：类别必须匹配，或者是万能卡
                    if (wordCard.IsJoker)
                        return true;
                    return wordCard.CategoryId == CategoryId;
                    
                case WordDeckType.Column:
                    // 列区：总是接受（用于整理）
                    return true;
                    
                case WordDeckType.Hand:
                    // 手牌区：只接受来自牌库的卡牌
                    return card.Deck != null && card.Deck.Type == Model.Enum.DeckType.DECK_TYPE_PACK;
                    
                case WordDeckType.Pack:
                    // 牌库：不接受外部卡牌
                    return false;
                    
                default:
                    return false;
            }
        }
        
        /// <summary>
        /// 更新卡牌位置
        /// </summary>
        public override void UpdateCardsPosition(bool firstTime)
        {
            if (!HasCards) return;
            
            switch (DeckType)
            {
                case WordDeckType.Column:
                    UpdateColumnCardsPosition();
                    break;
                case WordDeckType.Hand:
                    UpdateHandCardsPosition();
                    break;
                case WordDeckType.CategorySlot:
                    UpdateCategorySlotCardsPosition();
                    break;
                case WordDeckType.Pack:
                    // 牌库卡牌堆叠在一起
                    UpdatePackCardsPosition();
                    break;
            }
            
            UpdateCardsActiveStatus();
            UpdateDraggableStatus();
        }
        
        /// <summary>
        /// 更新列区卡牌位置（垂直堆叠）
        /// </summary>
        private void UpdateColumnCardsPosition()
        {
            float verticalSpace = CardLogicComponent?.GetSpaceFromDictionary(DeckSpacesTypes.DECK_SPACE_VERTICAL_BOTTOM_OPENED) ?? 30f;
            Vector3 basePosition = transform.position;
            
            for (int i = 0; i < CardsCount; i++)
            {
                Vector3 newPosition = new Vector3(basePosition.x, basePosition.y - i * verticalSpace, basePosition.z);
                CardsArray[i].SetPosition(newPosition);
                CardsArray[i].transform.SetSiblingIndex(i);
            }
        }
        
        /// <summary>
        /// 更新手牌区卡牌位置（水平堆叠）
        /// </summary>
        private void UpdateHandCardsPosition()
        {
            float horizontalSpace = CardLogicComponent?.GetSpaceFromDictionary(DeckSpacesTypes.DECK_SPACE_HORIONTAL_WASTE) ?? 40f;
            Vector3 basePosition = transform.position;
            
            // 只显示最多3张手牌
            int displayCount = Mathf.Min(CardsCount, 3);
            int startIndex = Mathf.Max(0, CardsCount - 3);
            
            for (int i = 0; i < CardsCount; i++)
            {
                if (i >= startIndex)
                {
                    int displayIndex = i - startIndex;
                    Vector3 newPosition = new Vector3(basePosition.x + displayIndex * horizontalSpace, basePosition.y, basePosition.z);
                    CardsArray[i].SetPosition(newPosition);
                }
                CardsArray[i].transform.SetSiblingIndex(i);
            }
        }
        
        /// <summary>
        /// 更新分类槽卡牌位置
        /// </summary>
        private void UpdateCategorySlotCardsPosition()
        {
            Vector3 basePosition = transform.position;
            
            for (int i = 0; i < CardsCount; i++)
            {
                // 分类槽中的卡牌轻微堆叠
                float offset = i * 2f;
                Vector3 newPosition = new Vector3(basePosition.x, basePosition.y + offset, basePosition.z - offset * 0.1f);
                CardsArray[i].SetPosition(newPosition);
                CardsArray[i].transform.SetSiblingIndex(i);
            }
        }
        
        /// <summary>
        /// 更新牌库卡牌位置
        /// </summary>
        private void UpdatePackCardsPosition()
        {
            Vector3 basePosition = transform.position;
            
            for (int i = 0; i < CardsCount; i++)
            {
                CardsArray[i].SetPosition(basePosition);
                CardsArray[i].transform.SetSiblingIndex(i);
            }
        }
        
        /// <summary>
        /// 更新卡牌可拖动状态
        /// </summary>
        public override void UpdateDraggableStatus()
        {
            for (int i = 0; i < CardsCount; i++)
            {
                WordSolitaireCard card = CardsArray[i] as WordSolitaireCard;
                if (card == null) continue;
                
                switch (DeckType)
                {
                    case WordDeckType.Column:
                        // 列区只有最后一张牌可拖动
                        card.IsDraggable = (i == CardsCount - 1);
                        break;
                    case WordDeckType.Hand:
                        // 手牌区只有最后一张可拖动
                        card.IsDraggable = (i == CardsCount - 1);
                        break;
                    case WordDeckType.CategorySlot:
                    case WordDeckType.Pack:
                        // 分类槽和牌库中的牌不可拖动
                        card.IsDraggable = false;
                        break;
                }
            }
        }
        
        /// <summary>
        /// 更新背景颜色
        /// </summary>
        public override void UpdateBackgroundColor()
        {
            // Word Solitaire中背景颜色由卡牌自身管理
        }
        
        /// <summary>
        /// 获取牌堆类型
        /// </summary>
        public override Model.Enum.DeckType Type
        {
            get
            {
                switch (DeckType)
                {
                    case WordDeckType.Pack:
                        return Model.Enum.DeckType.DECK_TYPE_PACK;
                    case WordDeckType.Hand:
                        return Model.Enum.DeckType.DECK_TYPE_WASTE;
                    case WordDeckType.Column:
                        return Model.Enum.DeckType.DECK_TYPE_BOTTOM;
                    case WordDeckType.CategorySlot:
                        return Model.Enum.DeckType.DECK_TYPE_ACE;
                    default:
                        return Model.Enum.DeckType.DECK_TYPE_BOTTOM;
                }
            }
        }
    }
}
