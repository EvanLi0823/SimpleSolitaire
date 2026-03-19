using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SimpleSolitaire.Model;

namespace SimpleSolitaire.Controller.WordSolitaire
{
    /// <summary>
    /// Word Solitaire 提示系统
    /// 负责查找可匹配卡牌并高亮提示
    /// </summary>
    public class WordSolitaireHintManager : HintManager
    {
        /// <summary>
        /// 提示动画协程
        /// </summary>
        private IEnumerator _hintCoroutine;

        /// <summary>
        /// 当前高亮的卡牌
        /// </summary>
        private Card _highlightedCard;

        /// <summary>
        /// 目标牌堆
        /// </summary>
        private Deck _targetDeck;

        /// <summary>
        /// 提示动画实现
        /// </summary>
        /// <param name="data">提示数据</param>
        protected override IEnumerator HintTranslate(HintData data)
        {
            IsHintProcess = true;

            // 获取提示列表
            List<HintElement> hints = data.Type == HintType.AutoComplete ? AutoCompleteHints : Hints;
            
            if (data.Type == HintType.AutoComplete)
            {
                CurrentHintIndex = 0;
            }

            // 如果指定了卡牌，查找对应的提示索引
            if (data.Card != null)
            {
                CurrentHintIndex = hints.FindIndex(x => x.HintCard == data.Card);
            }

            // 检查是否找到有效提示
            if (data.Card != null && CurrentHintIndex == -1)
            {
                AudioController audioCtrl = AudioController.Instance;
                if (audioCtrl != null)
                {
                    audioCtrl.Play(AudioController.AudioType.Error);
                }

                Debug.LogWarning($"[WordSolitaireHintManager] 该卡牌没有可匹配的提示: {data.Card.CardNumber}");
                IsHintProcess = false;
                CurrentHintIndex = 0;
                yield break;
            }

            // 执行提示动画
            float t = 0f;
            Card hintCard = hints[CurrentHintIndex].HintCard;
            hintCard.Deck.UpdateCardsPosition(false);

            // 保存原始层级
            CurrentHintSiblingIndex = hintCard.transform.GetSiblingIndex();

            // 将卡牌置顶
            hintCard.Deck.SetCardsToTop(hintCard);

            // 保存高亮卡牌引用
            _highlightedCard = hintCard;
            _targetDeck = hints[CurrentHintIndex].DestinationDeck;

            // 动画移动
            while (t < 1)
            {
                t += Time.deltaTime / data.HintTime;
                hintCard.transform.position = Vector3.Lerp(
                    hints[CurrentHintIndex].FromPosition,
                    hints[CurrentHintIndex].ToPosition,
                    t
                );

                yield return new WaitForEndOfFrame();

                hints[CurrentHintIndex].HintCard.Deck.SetPositionFromCard(
                    hintCard,
                    hintCard.transform.position.x,
                    hintCard.transform.position.y
                );
            }

            // 如果是普通提示模式，返回原位
            if (IsHasHint() && data.Type == HintType.Hint)
            {
                hintCard.Deck.UpdateCardsPosition(false);
                hintCard.transform.position = hints[CurrentHintIndex].FromPosition;
                hintCard.transform.SetSiblingIndex(CurrentHintSiblingIndex);
                
                // 切换到下一个提示
                CurrentHintIndex = CurrentHintIndex >= hints.Count - 1 ? 0 : CurrentHintIndex + 1;
            }

            // 如果是点击或自动完成，执行放置
            if (data.Type != HintType.Hint)
            {
                _cardLogicComponent.OnDragEnd(hintCard);
            }

            IsHintProcess = false;
        }

        /// <summary>
        /// 生成提示列表
        /// </summary>
        /// <param name="isAutoComplete">是否为自动完成模式</param>
        protected override void GenerateHints(bool isAutoComplete = false)
        {
            CurrentHintIndex = 0;
            AutoCompleteHints = new List<HintElement>();
            Hints = new List<HintElement>();

            if (IsAvailableForMoveCardArray.Count == 0)
            {
                ActivateHintButton(false);
                ActivateAutoCompleteHintButton(false);
                return;
            }

            // 遍历所有可移动的卡牌
            foreach (var card in IsAvailableForMoveCardArray)
            {
                if (card == null) continue;

                WordSolitaireCard wordCard = card as WordSolitaireCard;
                if (wordCard == null) continue;

                // 查找可放置的分类槽
                foreach (var deck in _cardLogicComponent.AllDeckArray)
                {
                    WordSolitaireDeck wordDeck = deck as WordSolitaireDeck;
                    if (wordDeck == null) continue;

                    // 只考虑分类槽
                    if (wordDeck.DeckType != WordDeckType.CategorySlot)
                        continue;

                    // 检查是否可以放置
                    if (deck.AcceptCard(card))
                    {
                        // 获取目标位置
                        Vector3 targetPosition = GetTargetPosition(deck);

                        // 添加到提示列表
                        var hintElement = new HintElement(
                            card,
                            card.transform.position,
                            targetPosition,
                            deck
                        );

                        Hints.Add(hintElement);

                        // 如果是自动完成模式且满足条件，添加到自动完成列表
                        if (isAutoComplete && IsValidForAutoComplete(wordCard, wordDeck))
                        {
                            AutoCompleteHints.Add(hintElement);
                        }
                    }
                }
            }

            // 激活提示按钮
            ActivateHintButton(IsHasHint());
            ActivateAutoCompleteHintButton(IsHasAutoCompleteHint());

            Debug.Log($"[WordSolitaireHintManager] 生成提示: {Hints.Count}个, 自动完成: {AutoCompleteHints.Count}个");
        }

        /// <summary>
        /// 获取目标位置
        /// </summary>
        /// <param name="deck">目标牌堆</param>
        /// <returns>目标位置</returns>
        private Vector3 GetTargetPosition(Deck deck)
        {
            Card topCard = deck.GetTopCard();
            if (topCard != null)
            {
                // 如果有顶部卡牌，放在其上方
                float spaceY = _cardLogicComponent.GetSpaceFromDictionary(DeckSpacesTypes.DECK_SPACE_VERTICAL_ACE);
                return topCard.transform.position + new Vector3(0, spaceY, 0);
            }
            else
            {
                // 否则放在牌堆位置
                return deck.transform.position;
            }
        }

        /// <summary>
        /// 检查是否适合自动完成
        /// </summary>
        /// <param name="card">卡牌</param>
        /// <param name="targetDeck">目标牌堆</param>
        /// <returns>是否适合</returns>
        private bool IsValidForAutoComplete(WordSolitaireCard card, WordSolitaireDeck targetDeck)
        {
            // 检查该类别槽是否已有卡牌
            // 如果有，优先自动完成
            if (targetDeck.HasCards)
            {
                return true;
            }

            // 检查是否还有其他可匹配的卡牌在同一类别
            var sameCategoryCards = _cardLogicComponent.CardsArray
                .Where(c => c is WordSolitaireCard wc && wc.CategoryId == card.CategoryId && c != card)
                .ToList();

            // 如果该类别还有其他卡牌，暂时不自动完成
            return sameCategoryCards.Count == 0;
        }

        /// <summary>
        /// 查找有效移动
        /// </summary>
        /// <returns>可移动的卡牌，如果没有则返回null</returns>
        public Card FindValidMove()
        {
            UpdateAvailableForDragCards();
            GenerateHints();

            if (Hints.Count > 0)
            {
                return Hints[0].HintCard;
            }

            return null;
        }

        /// <summary>
        /// 高亮提示
        /// </summary>
        public void HighlightHint()
        {
            if (IsHintProcess)
            {
                // 如果正在提示中，重置并显示下一个
                ResetHint();
            }

            HintButtonAction();
        }

        /// <summary>
        /// 高亮指定卡牌
        /// </summary>
        /// <param name="card">要高亮的卡牌</param>
        public void HighlightCard(Card card)
        {
            if (card == null) return;

            var data = new HintData(
                hintTime: TapToPlaceTranslateTime,
                type: HintType.CardClick,
                card: card,
                hintButtonPressed: false
            );

            Hint(data);
        }

        /// <summary>
        /// 重置提示
        /// </summary>
        public new void ResetHint()
        {
            base.ResetHint();
            _highlightedCard = null;
            _targetDeck = null;
        }

        /// <summary>
        /// 检查指定卡牌是否有提示
        /// </summary>
        /// <param name="card">卡牌</param>
        /// <returns>是否有提示</returns>
        public bool HasHintForCard(Card card)
        {
            if (card == null) return false;
            return Hints.Any(x => x.HintCard == card);
        }

        /// <summary>
        /// 获取提示数量
        /// </summary>
        /// <returns>提示数量</returns>
        public int GetHintCount()
        {
            return Hints.Count;
        }

        /// <summary>
        /// 获取第一个提示的目标牌堆
        /// </summary>
        /// <returns>目标牌堆</returns>
        public Deck GetFirstHintTargetDeck()
        {
            if (Hints.Count > 0)
            {
                return Hints[0].DestinationDeck;
            }
            return null;
        }

        /// <summary>
        /// 更新可拖拽卡牌列表（重写以适配Word Solitaire逻辑）
        /// </summary>
        /// <param name="isAutoComplete">是否为自动完成</param>
        public override void UpdateAvailableForDragCards(bool isAutoComplete = false)
        {
            IsAvailableForMoveCardArray = new List<Card>();

            Card[] cards = _cardLogicComponent.CardsArray;

            for (int i = 0; i < cards.Length; i++)
            {
                if (cards[i] == null) continue;

                // 只考虑可拖拽的卡牌
                if (!cards[i].IsDraggable)
                    continue;

                // 获取卡牌所在牌堆
                WordSolitaireDeck deck = cards[i].Deck as WordSolitaireDeck;
                if (deck == null) continue;

                // 只考虑手牌区和列区的卡牌
                if (deck.DeckType == WordDeckType.Hand || deck.DeckType == WordDeckType.Column)
                {
                    // 如果是列区，只取最顶部的卡牌
                    if (deck.DeckType == WordDeckType.Column)
                    {
                        if (deck.GetTopCard() == cards[i])
                        {
                            IsAvailableForMoveCardArray.Add(cards[i]);
                        }
                    }
                    else
                    {
                        IsAvailableForMoveCardArray.Add(cards[i]);
                    }
                }
            }

            GenerateHints(isAutoComplete);
        }
    }
}
