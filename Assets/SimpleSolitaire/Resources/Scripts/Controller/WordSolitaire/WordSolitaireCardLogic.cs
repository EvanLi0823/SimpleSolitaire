using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace SimpleSolitaire.Controller.WordSolitaire
{
    /// <summary>
    /// Word Solitaire 核心游戏逻辑
    /// 继承自CardLogic基类，实现词语接龙的核心玩法
    /// </summary>
    public class WordSolitaireCardLogic : CardLogic
    {
        [Header("Word Solitaire 组件")]
        public WordDataManager WordDataManager;
        public LevelDataManager LevelDataManager;
        
        // 牌堆引用
        public WordSolitaireDeck HandDeck;           // 手牌区
        public WordSolitaireDeck[] ColumnDecks;      // 列区牌堆
        public WordSolitaireDeck[] CategorySlots;    // 分类槽
        
        [Header("预制体")]
        public GameObject WordCardPrefab;            // 词语卡牌预制体
        
        // 当前关卡数据
        private LevelData _currentLevel;
        private List<WordItem> _currentLevelWords;
        private bool _isGameOver;
        
        /// <summary>
        /// 当前关卡
        /// </summary>
        public LevelData CurrentLevel => _currentLevel;
        
        /// <summary>
        /// 是否游戏结束
        /// </summary>
        public bool IsGameOver => _isGameOver;
        
        /// <summary>
        /// 卡牌数量（基类要求）
        /// </summary>
        protected override int CardNums => _currentLevel?.CardCount ?? 0;
        
        #region 初始化
        
        /// <summary>
        /// 初始化关卡
        /// </summary>
        public void InitializeLevel(LevelData levelData)
        {
            _currentLevel = levelData;
            _isGameOver = false;
            
            if (_currentLevel != null)
            {
                LoadLevelWords();
            }
        }
        
        /// <summary>
        /// 根据关卡ID初始化关卡（撤销系统使用）
        /// </summary>
        /// <param name="levelId">关卡ID</param>
        public void InitializeLevelById(int levelId)
        {
            if (LevelDataManager == null) return;
            
            LevelDataManager.LoadLevel(levelId);
            LevelData levelData = LevelDataManager.CurrentLevel;
            if (levelData != null)
            {
                InitializeLevel(levelData);
            }
        }
        
        /// <summary>
        /// 加载关卡词汇
        /// </summary>
        private void LoadLevelWords()
        {
            _currentLevelWords = new List<WordItem>();
            
            if (WordDataManager == null || _currentLevel == null) return;
            
            foreach (string categoryId in _currentLevel.CategoryIds)
            {
                WordCategoryData category = WordDataManager.GetCategoryById(categoryId);
                if (category == null) continue;
                
                foreach (WordItem word in category.Words)
                {
                    _currentLevelWords.Add(word);
                }
            }
        }
        
        /// <summary>
        /// 初始化卡牌逻辑（基类调用）
        /// </summary>
        public override void InitCardLogic()
        {
            base.InitCardLogic();
            
            if (_currentLevel != null)
            {
                InitializeCards();
            }
        }
        
        /// <summary>
        /// 初始化卡牌
        /// </summary>
        private void InitializeCards()
        {
            if (WordCardPrefab == null) return;
            
            // 创建所有卡牌
            List<WordSolitaireCard> allCards = new List<WordSolitaireCard>();
            
            foreach (WordItem wordItem in _currentLevelWords)
            {
                WordSolitaireCard card = CreateWordCard(wordItem);
                if (card != null)
                {
                    allCards.Add(card);
                }
            }
            
            // 洗牌
            ShuffleCards(allCards);
            
            // 分发卡牌到牌库
            foreach (WordSolitaireCard card in allCards)
            {
                PackDeck?.PushCard(card, false, 0);
            }
            
            // 更新牌库显示
            PackDeck?.UpdateCardsPosition(false);
        }
        
        /// <summary>
        /// 创建词语卡牌
        /// </summary>
        private WordSolitaireCard CreateWordCard(WordItem wordItem)
        {
            if (WordCardPrefab == null) return null;
            
            GameObject cardObj = Instantiate(WordCardPrefab, transform);
            WordSolitaireCard card = cardObj.GetComponent<WordSolitaireCard>();
            
            if (card != null)
            {
                card.InitWithWordItem(wordItem);
                card.CardLogicComponent = this;
            }
            
            return card;
        }
        
        /// <summary>
        /// 洗牌
        /// </summary>
        private void ShuffleCards(List<WordSolitaireCard> cards)
        {
            System.Random rng = new System.Random();
            int n = cards.Count;
            
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                WordSolitaireCard temp = cards[k];
                cards[k] = cards[n];
                cards[n] = temp;
            }
        }
        
        /// <summary>
        /// 初始化牌堆数组（基类要求）
        /// </summary>
        protected override void InitDeckCards()
        {
            // 初始化AceDeckArray（分类槽）
            AceDeckArray = new Deck[CategorySlots.Length];
            for (int i = 0; i < CategorySlots.Length; i++)
            {
                AceDeckArray[i] = CategorySlots[i];
            }
            
            // 初始化BottomDeckArray（列区）
            BottomDeckArray = new Deck[ColumnDecks.Length];
            for (int i = 0; i < ColumnDecks.Length; i++)
            {
                BottomDeckArray[i] = ColumnDecks[i];
            }
            
            // 设置WasteDeck为HandDeck
            WasteDeck = HandDeck;
            
            // 牌库已在字段中初始化
        }
        
        #endregion
        
        #region 游戏逻辑
        
        /// <summary>
        /// 卡牌拖动结束处理（基类要求）
        /// </summary>
        public override async Task OnDragEnd(Card card)
        {
            WordSolitaireCard wordCard = card as WordSolitaireCard;
            if (wordCard == null) return;
            
            // 查找重叠的牌堆
            WordSolitaireDeck targetDeck = FindOverlappingDeck(wordCard);
            
            if (targetDeck != null && targetDeck.AcceptCard(wordCard))
            {
                // 执行放置
                await PlaceCardToDeck(wordCard, targetDeck);
            }
            else
            {
                // 返回原位
                ReturnCardToOriginalDeck(wordCard);
            }
        }
        
        /// <summary>
        /// 查找重叠的牌堆
        /// </summary>
        private WordSolitaireDeck FindOverlappingDeck(WordSolitaireCard card)
        {
            WordSolitaireDeck bestDeck = null;
            float bestOverlap = 0f;
            
            // 检查所有牌堆
            CheckDeckOverlap(card, PackDeck as WordSolitaireDeck, ref bestDeck, ref bestOverlap);
            CheckDeckOverlap(card, HandDeck, ref bestDeck, ref bestOverlap);
            
            if (ColumnDecks != null)
            {
                foreach (var deck in ColumnDecks)
                {
                    CheckDeckOverlap(card, deck, ref bestDeck, ref bestOverlap);
                }
            }
            
            if (CategorySlots != null)
            {
                foreach (var deck in CategorySlots)
                {
                    CheckDeckOverlap(card, deck, ref bestDeck, ref bestOverlap);
                }
            }
            
            return bestDeck;
        }
        
        /// <summary>
        /// 检查牌堆重叠
        /// </summary>
        private void CheckDeckOverlap(WordSolitaireCard card, WordSolitaireDeck deck, 
            ref WordSolitaireDeck bestDeck, ref float bestOverlap)
        {
            if (deck == null || deck == card.Deck) return;
            
            float overlap = CalculateOverlap(card, deck);
            if (overlap > bestOverlap)
            {
                bestOverlap = overlap;
                bestDeck = deck;
            }
        }
        
        /// <summary>
        /// 计算重叠面积
        /// </summary>
        private float CalculateOverlap(WordSolitaireCard card, WordSolitaireDeck deck)
        {
            RectTransform cardRect = card.GetComponent<RectTransform>();
            RectTransform deckRect = deck.GetComponent<RectTransform>();
            
            if (cardRect == null || deckRect == null) return 0f;
            
            Vector3 cardPos = cardRect.position;
            Vector3 deckPos = deckRect.position;
            
            float cardWidth = cardRect.rect.width * cardRect.lossyScale.x;
            float cardHeight = cardRect.rect.height * cardRect.lossyScale.y;
            float deckWidth = deckRect.rect.width * deckRect.lossyScale.x;
            float deckHeight = deckRect.rect.height * deckRect.lossyScale.y;
            
            float overlapX = Mathf.Max(0, Mathf.Min(cardPos.x + cardWidth / 2, deckPos.x + deckWidth / 2) 
                - Mathf.Max(cardPos.x - cardWidth / 2, deckPos.x - deckWidth / 2));
            float overlapY = Mathf.Max(0, Mathf.Min(cardPos.y + cardHeight / 2, deckPos.y + deckHeight / 2) 
                - Mathf.Max(cardPos.y - cardHeight / 2, deckPos.y - deckHeight / 2));
            
            return overlapX * overlapY;
        }
        
        /// <summary>
        /// 放置卡牌到牌堆
        /// </summary>
        private async Task PlaceCardToDeck(WordSolitaireCard card, WordSolitaireDeck targetDeck)
        {
            WordSolitaireDeck originalDeck = card.Deck as WordSolitaireDeck;
            
            // 从原牌堆移除
            if (originalDeck != null)
            {
                originalDeck.RemoveCard(card);
                originalDeck.UpdateCardsPosition(false);
            }
            
            // 添加到目标牌堆
            targetDeck.PushCard(card, true, 1);
            targetDeck.UpdateCardsPosition(false);
            
            // 播放音效
            if (AudioCtrl != null)
            {
                AudioCtrl.Play(AudioController.AudioType.Move);
            }
            
            // 检查匹配
            if (targetDeck.DeckType == WordDeckType.CategorySlot)
            {
                OnCardMatchedToCategorySlot(card, targetDeck);
            }
            
            await Task.Yield();
        }
        
        /// <summary>
        /// 卡牌返回原位
        /// </summary>
        private void ReturnCardToOriginalDeck(WordSolitaireCard card)
        {
            if (card.Deck != null)
            {
                card.Deck.UpdateCardsPosition(false);
            }
        }
        
        /// <summary>
        /// 卡牌匹配到分类槽
        /// </summary>
        private void OnCardMatchedToCategorySlot(WordSolitaireCard card, WordSolitaireDeck slot)
        {
            // 发布匹配成功事件
            GameEventBus.PublishCategoryMatched(card.CategoryId, slot.CurrentCardCount, slot.TargetCardCount);
            
            // 检查分类槽是否完成
            if (slot.IsComplete)
            {
                // TODO: 添加分类完成事件
                // GameEventBus.PublishCategoryCompleted(card.CategoryId);
                
                // 播放完成音效
                if (AudioCtrl != null)
                {
                    AudioCtrl.Play(AudioController.AudioType.MoveToAce);
                }
            }
            
            // 检查胜利条件
            CheckWinCondition();
        }
        
        /// <summary>
        /// 尝试自动匹配卡牌
        /// </summary>
        public void TryAutoMatchCard(WordSolitaireCard card)
        {
            if (card.WordCardType == global::SimpleSolitaire.Controller.WordSolitaire.CardType.Joker) return;
            
            // 查找对应的分类槽
            if (CategorySlots != null)
            {
                foreach (var slot in CategorySlots)
                {
                    if (slot.CategoryId == card.CategoryId && slot.AcceptCard(card))
                    {
                        PlaceCardToDeck(card, slot);
                        return;
                    }
                }
            }
        }
        
        /// <summary>
        /// 检查胜利条件
        /// </summary>
        public bool CheckWinCondition()
        {
            if (CategorySlots == null || CategorySlots.Length == 0)
                return false;
            
            // 所有分类槽都必须完成
            foreach (var slot in CategorySlots)
            {
                if (slot == null) continue;
                if (!slot.IsComplete)
                    return false;
            }
            
            // 触发胜利
            OnWinGame();
            return true;
        }
        
        /// <summary>
        /// 胜利处理
        /// </summary>
        private void OnWinGame()
        {
            IsGameStarted = false;
            _isGameOver = true;
            
            if (GameManagerComponent is WordSolitaireGameManager wordManager)
            {
                wordManager.HasWinGame();
            }
        }
        
        #endregion
        
        #region 牌库操作
        
        /// <summary>
        /// 点击牌库（基类要求）
        /// </summary>
        public override void OnClickPack()
        {
            if (PackDeck == null || !PackDeck.HasCards) return;
            
            // 从牌库发一张牌到手牌区
            Card card = PackDeck.Pop();
            if (card != null)
            {
                HandDeck?.PushCard(card, true, 1);
                HandDeck?.UpdateCardsPosition(false);
                
                // 播放音效
                if (AudioCtrl != null)
                {
                    AudioCtrl.Play(AudioController.AudioType.Move);
                }
            }
            
            // 检查牌库是否耗尽
            if (!PackDeck.HasCards)
            {
                GameEventBus.PublishPackEmpty();
            }
        }
        
        #endregion
        
        #region 辅助方法
        
        /// <summary>
        /// 显示提示
        /// </summary>
        public void ShowHint()
        {
            if (HintManagerComponent is WordSolitaireHintManager wordHintManager)
            {
                wordHintManager.HighlightHint();
            }
        }
        
        /// <summary>
        /// 获取所有牌堆（基类要求）
        /// </summary>
        public override Deck[] AllDeckArray
        {
            get
            {
                List<Deck> decks = new List<Deck>();
                
                if (PackDeck != null)
                    decks.Add(PackDeck);
                
                if (HandDeck != null)
                    decks.Add(HandDeck);
                
                if (ColumnDecks != null)
                    decks.AddRange(ColumnDecks);
                
                if (CategorySlots != null)
                    decks.AddRange(CategorySlots);
                
                return decks.ToArray();
            }
        }
        
        /// <summary>
        /// 事件订阅（基类要求）
        /// </summary>
        public override void SubscribeEvents()
        {
            // 订阅需要的事件
        }
        
        /// <summary>
        /// 事件取消订阅（基类要求）
        /// </summary>
        public override void UnsubscribeEvents()
        {
            // 取消订阅事件
        }
        
        /// <summary>
        /// 新游戏开始（基类要求）
        /// </summary>
        public override void OnNewGameStart()
        {
            _isGameOver = false;
        }
        
        #endregion
    }
}
