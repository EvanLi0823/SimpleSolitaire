using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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
    /// 手牌区卡牌排列方向
    /// </summary>
    public enum HandCardDirection
    {
        /// <summary>
        /// 正向：最新的一张牌在最左边（靠左）
        /// </summary>
        LeftToRight,
        
        /// <summary>
        /// 反向：最新的一张牌在最右边（靠右）
        /// </summary>
        RightToLeft
    }
    
    /// <summary>
    /// Word Solitaire 牌堆
    /// 继承自Deck基类,实现词语牌堆的特定逻辑
    /// </summary>
    public class WordSolitaireDeck : Deck, IPointerClickHandler
    {
        [Header("Word Solitaire 牌堆属性")]
        public WordDeckType DeckType;          // 牌堆类型
        public int CategoryId;              // 类别ID(仅CategorySlot使用)
        public int TargetCardCount;            // 目标卡牌数量(仅CategorySlot使用)
        public int ColumnIndex;                // 列索引(仅Column使用)

        [Header("手牌区配置")]
        public HandCardDirection HandDirection = HandCardDirection.LeftToRight;  // 手牌区卡牌排列方向
        public int MaxVisibleCards = 3;       // 手牌区最多显示卡牌数量

        /// <summary>
        /// 当前已放置的卡牌数量
        /// </summary>
        public int CurrentCardCount => CardsArray?.Count ?? 0;

        /// <summary>
        /// 检查是否已完成(仅CategorySlot使用)
        /// </summary>
        public bool IsComplete => DeckType == WordDeckType.CategorySlot &&
                                  CurrentCardCount >= TargetCardCount;

        /// <summary>
        /// 点击事件处理(隐藏Deck基类方法)
        /// 处理"恢复库存"文本点击
        /// </summary>
        public new void OnPointerClick(PointerEventData eventData)
        {
            // 如果是牌库类型,检查是否点击了"恢复库存"文本
            if (DeckType == WordDeckType.Pack)
            {
                // 检查是否点击在"恢复库存"文本上
                Transform restoreText = transform.Find("BackGround/Localized Text");
                if (restoreText != null && restoreText.gameObject.activeSelf)
                {
                    RectTransform restoreTextRect = restoreText.GetComponent<RectTransform>();
                    if (restoreTextRect != null && RectTransformUtility.RectangleContainsScreenPoint(restoreTextRect, eventData.position, eventData.pressEventCamera))
                    {
                        // 点击了"恢复库存"文本,执行恢复库存操作
                        if (CardLogicComponent is WordSolitaireCardLogic wordLogic)
                        {
                            wordLogic.RestorePackCards();
                        }
                        return;
                    }
                }
            }

            // 其他情况调用基类方法
            base.OnPointerClick(eventData);
        }
        
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
                    
                    // 分类槽为空时，只接受分类卡
                    if (CurrentCardCount == 0)
                    {
                        return wordCard.IsCategoryCard && wordCard.CategoryId == CategoryId;
                    }
                    
                    // 分类槽已有分类卡后，只接受同词组的单词卡
                    // 万能卡不能放入分类槽
                    if (wordCard.IsJoker)
                        return false;
                    
                    return wordCard.CategoryId == CategoryId;
                    
                case WordDeckType.Column:
                    // 列区：空白时只接受分类卡，有卡时接受同词组卡牌
                    if (CurrentCardCount == 0)
                    {
                        // 空白列区只接受分类卡作为起始
                        return wordCard.IsCategoryCard;
                    }
                    else
                    {
                        // 非空白列区：只接受同词组的卡牌（分类卡或单词卡）
                        WordSolitaireCard topCard = CardsArray[CurrentCardCount - 1] as WordSolitaireCard;
                        if (topCard == null) return false;
                        
                        // 万能卡不能放入列区
                        if (wordCard.IsJoker)
                            return false;
                        
                        // 分类卡只能放在同词组的单词卡上
                        if (wordCard.IsCategoryCard)
                            return topCard.CategoryId == wordCard.CategoryId;
                        
                        // 单词卡只能放在同词组的分类卡或单词卡上
                        return topCard.CategoryId == wordCard.CategoryId;
                    }
                    
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
        /// 规则：
        /// - 最多显示MaxVisibleCards张
        /// - 最新的一张牌在排列方向的尽头
        /// - 超过最大显示数量时，最外侧的牌隐藏
        /// </summary>
        private void UpdateHandCardsPosition()
        {
            float horizontalSpace = CardLogicComponent?.GetSpaceFromDictionary(DeckSpacesTypes.DECK_SPACE_HORIONTAL_WASTE) ?? 40f;
            Vector3 basePosition = transform.position;
            
            int visibleCount = Mathf.Min(CardsCount, MaxVisibleCards);
            bool isReverse = HandDirection == HandCardDirection.RightToLeft;
            
            for (int i = 0; i < CardsCount; i++)
            {
                WordSolitaireCard card = CardsArray[i] as WordSolitaireCard;
                if (card == null) continue;
                
                // 计算该卡牌在显示序列中的索引
                int displayIndex;
                bool isVisible;
                
                if (isReverse)
                {
                    // 反向：最新的一张在右边（索引越大越靠右）
                    // CardsArray中索引0是最旧的，CardsCount-1是最新的
                    int newestIndex = CardsCount - 1;
                    displayIndex = newestIndex - i;  // 最新的显示在最右边
                    
                    // 最右边MaxVisibleCards张显示，其余隐藏
                    isVisible = displayIndex < visibleCount && displayIndex >= 0;
                }
                else
                {
                    // 正向：最新的一张在左边（索引越小越靠左）
                    // CardsArray中索引0是最旧的，CardsCount-1是最新的
                    // 显示最新的visibleCount张
                    int newestIndex = CardsCount - 1;
                    int oldestVisibleIndex = newestIndex - visibleCount + 1;
                    
                    isVisible = i >= oldestVisibleIndex;
                    displayIndex = isVisible ? (i - oldestVisibleIndex) : 0;
                }
                
                // 设置卡牌位置和可见性
                if (isVisible)
                {
                    float xOffset = displayIndex * horizontalSpace;
                    Vector3 newPosition = new Vector3(basePosition.x + xOffset, basePosition.y, basePosition.z);
                    card.SetPosition(newPosition);
                    card.SetCardFace(true);  // 显示正面
                }
                else
                {
                    // 隐藏的卡牌移到远处并显示卡背
                    card.SetPosition(basePosition + new Vector3(1000, 0, 0));
                    card.SetCardFace(false);  // 显示卡背
                }
                
                card.transform.SetSiblingIndex(i);
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
            // 获取牌库堆叠间距（使用垂直封闭间距）
            float packSpace = CardLogicComponent?.GetSpaceFromDictionary(DeckSpacesTypes.DECK_SPACE_VERTICAL_BOTTOM_CLOSED) ?? 2f;
            Vector3 basePosition = transform.position;

            for (int i = 0; i < CardsCount; i++)
            {
                // 错位堆叠：每张牌向下移动一点，实现视觉堆叠效果
                Vector3 newPosition = new Vector3(
                    basePosition.x,
                    basePosition.y - i * packSpace,
                    basePosition.z
                );
                CardsArray[i].SetPosition(newPosition);
                CardsArray[i].transform.SetSiblingIndex(i);
            }

            // 更新"恢复库存"文本显示
            UpdateRestoreTextVisibility();
        }

        /// <summary>
        /// 更新恢复库存文本和背景显示
        /// </summary>
        private void UpdateRestoreTextVisibility()
        {
            // 只对牌库类型执行此逻辑
            if (DeckType != WordDeckType.Pack) return;

            // 背景状态：有牌时显示元素，无牌时隐藏
            bool shouldShow = HasCards;

            // 查找并控制各子元素的显示状态
            Transform cardBackStack = transform.Find("CardBackStack");
            if (cardBackStack != null)
            {
                cardBackStack.gameObject.SetActive(shouldShow);
            }

            Transform countLabelBack = transform.Find("CountLabelBack");
            if (countLabelBack != null)
            {
                countLabelBack.gameObject.SetActive(shouldShow);

                // 更新CountLabel文本，显示牌库中卡牌的数量
                Transform countLabel = countLabelBack.Find("CountLabel");
                if (countLabel != null)
                {
                    Text countLabelText = countLabel.GetComponent<Text>();
                    if (countLabelText != null)
                    {
                        countLabelText.text = CardsCount.ToString();
                    }
                }
            }

            Transform restoreText = transform.Find("BackGround/Localized Text");
            if (restoreText != null)
            {
                // "恢复库存"文本：有牌时隐藏,无牌时显示（与背景元素相反）
                restoreText.gameObject.SetActive(!shouldShow);
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
