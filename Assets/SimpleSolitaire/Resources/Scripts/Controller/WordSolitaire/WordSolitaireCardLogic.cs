using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimpleSolitaire.Model.Enum;
using UnityEngine;
using UnityEngine.UI;

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
        // public WordSolitaireOrientationManager OrientationManager;  // 方向管理器 - 已注释，暂时不需要考虑横屏

        // 牌堆容器引用（用于动态生成Deck）
        public Transform CategorySlotsContainer;    // 分类槽容器
        public Transform ColumnDecksContainer;      // 列区容器
        
        // 牌堆引用
        public WordSolitaireDeck HandDeck;           // 手牌区
        public WordSolitaireDeck[] ColumnDecks;      // 列区牌堆（动态生成）
        public WordSolitaireDeck[] CategorySlots;    // 分类槽（动态生成）
        
        [Header("预制体")]
        public GameObject DeckPrefab;                // Deck预制体（CategorySlot.prefab）
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
            
            if (_currentLevel == null)
            {
                Debug.LogError("[WordSolitaireCardLogic] 无法初始化关卡: levelData 为 null");
                return;
            }
            
            // 验证关卡数据
            if (!ValidateLevelData(_currentLevel))
            {
                Debug.LogError($"[WordSolitaireCardLogic] 关卡 {_currentLevel.LevelId} 数据验证失败，无法初始化游戏");
                _currentLevel = null;
                return;
            }
            
            LoadLevelWords();
        }
        
        /// <summary>
        /// 验证关卡数据的完整性
        /// </summary>
        private bool ValidateLevelData(LevelData level)
        {
            bool isValid = true;
            
            if (level.LevelId <= 0)
            {
                Debug.LogWarning($"[WordSolitaireCardLogic] LevelId 必须大于0，当前值: {level.LevelId}");
                isValid = false;
            }
            
            if (level.CardCount <= 0)
            {
                Debug.LogWarning($"[WordSolitaireCardLogic] CardCount 必须大于0，当前值: {level.CardCount}");
                isValid = false;
            }
            
            if (level.ColumnCount <= 0)
            {
                Debug.LogWarning($"[WordSolitaireCardLogic] ColumnCount 必须大于0，当前值: {level.ColumnCount}，使用默认值4");
                level.ColumnCount = 4;
            }
            
            if (level.SlotCount <= 0)
            {
                Debug.LogWarning($"[WordSolitaireCardLogic] SlotCount 必须大于0，当前值: {level.SlotCount}，使用默认值3");
                level.SlotCount = 3;
            }
            
            if (level.CategoryIds == null || level.CategoryIds.Length == 0)
            {
                Debug.LogError($"[WordSolitaireCardLogic] CategoryIds 不能为空");
                isValid = false;
            }
            
            // 验证InitialCardsPerColumn是否已配置
            if (level.InitialCardsPerColumn == null)
            {
                Debug.LogError($"[WordSolitaireCardLogic] InitialCardsPerColumn 未配置，无法分发卡牌到列区");
                isValid = false;
            }
            else if (level.InitialCardsPerColumn.Length != level.ColumnCount)
            {
                Debug.LogWarning($"[WordSolitaireCardLogic] InitialCardsPerColumn 长度 ({level.InitialCardsPerColumn.Length}) 与 ColumnCount ({level.ColumnCount}) 不匹配，可能导致部分列区无卡牌");
            }
            
            return isValid;
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
            if (_currentLevel == null)
            {
                Debug.LogError("[WordSolitaireCardLogic] 无法加载词库: _currentLevel 为 null");
                _currentLevelWords = new List<WordItem>();
                return;
            }
            
            if (_currentLevel.CategoryIds == null || _currentLevel.CategoryIds.Length == 0)
            {
                Debug.LogError("[WordSolitaireCardLogic] 无法加载词库: CategoryIds 为空");
                _currentLevelWords = new List<WordItem>();
                return;
            }
            
            if (WordDataManager == null)
            {
                Debug.LogError("[WordSolitaireCardLogic] WordDataManager 未配置");
                _currentLevelWords = new List<WordItem>();
                return;
            }
            
            _currentLevelWords = new List<WordItem>();
            int loadedCategories = 0;
            int loadedWords = 0;
            
            foreach (int categoryId in _currentLevel.CategoryIds)
            {
                WordCategoryData category = WordDataManager.GetCategoryById(categoryId);
                if (category == null)
                {
                    Debug.LogWarning($"[WordSolitaireCardLogic] 分类ID {categoryId} 不存在，跳过");
                    continue;
                }
                
                loadedCategories++;
                foreach (WordItem word in category.Words)
                {
                    _currentLevelWords.Add(word);
                    loadedWords++;
                }
            }
            
            Debug.Log($"[WordSolitaireCardLogic] 词库加载完成: {loadedCategories}/{_currentLevel.CategoryIds.Length} 个分类, {loadedWords} 个单词");
            
            if (_currentLevelWords.Count == 0)
            {
                Debug.LogError("[WordSolitaireCardLogic] 词库为空，无法初始化卡牌");
            }
        }
        
        /// <summary>
        /// 初始化卡牌逻辑（基类调用）
        /// </summary>
        public override void InitCardLogic()
        {
            // 如果关卡数据未初始化，跳过生成
            if (_currentLevel == null)
            {
                Debug.LogWarning("[WordSolitaireCardLogic] InitCardLogic: _currentLevel 为 null，跳过初始化");
                // 仍然调用基类初始化，避免其他游戏逻辑出错
                InitDeckCards();
                base.InitCardLogic();
                return;
            }

            // 动态生成分类槽和列区Deck（必须在基类InitCardLogic之前）
            GenerateDecks();

            // 初始化牌堆数组（必须在基类InitCardLogic之前）
            InitDeckCards();

            // 初始化所有牌堆数组（重写方法以动态计算数量）
            InitAllDeckArray();

            // 不需要调用基类InitCardLogic，因为：
            // 1. InitCardNodes() 是私有的，子类无法访问
            // 2. InitAllDeckArray() 已经手动调用
            // 3. UndoPerformerComponent.ResetUndoStates() 和 ParticleStars.Stop() 已经在后面处理
            // WordSolitaire有自己特殊的初始化流程

            // 初始化卡牌
            InitializeCards();
        }
        
        /// <summary>
        /// 初始化卡牌
        /// </summary>
        private void InitializeCards()
        {
            try
            {
                if (WordCardPrefab == null)
                {
                    Debug.LogError("[WordSolitaireCardLogic] WordCardPrefab 未配置，无法初始化卡牌");
                    return;
                }

                if (_currentLevel == null)
                {
                    Debug.LogError("[WordSolitaireCardLogic] _currentLevel 为 null，无法初始化卡牌");
                    return;
                }

                if (_currentLevelWords == null || _currentLevelWords.Count == 0)
                {
                    Debug.LogError("[WordSolitaireCardLogic] _currentLevelWords 为空，无法初始化卡牌");
                    return;
                }

                Debug.Log($"[WordSolitaireCardLogic] 开始初始化卡牌，共 {_currentLevelWords.Count} 个单词卡");
            
            // 1. 创建所有单词卡
            List<WordSolitaireCard> allCards = new List<WordSolitaireCard>();
            int failedCards = 0;
            
            foreach (WordItem wordItem in _currentLevelWords)
            {
                WordSolitaireCard card = CreateWordCard(wordItem);
                if (card != null)
                {
                    allCards.Add(card);
                }
                else
                {
                    Debug.LogError($"[WordSolitaireCardLogic] 创建卡牌失败: {wordItem?.WordId}");
                    failedCards++;
                }
            }
            
            Debug.Log($"[WordSolitaireCardLogic] 成功创建 {allCards.Count} 张卡牌，失败 {failedCards} 张");
            
            // 2. 为每个类别创建分类卡
            HashSet<int> categoryIds = new HashSet<int>();
            foreach (WordItem word in _currentLevelWords)
            {
                if (word != null)
                {
                    categoryIds.Add(word.CategoryId);
                }
            }
            
            foreach (int categoryId in categoryIds)
            {
                WordCategoryData category = WordDataManager.GetCategoryById(categoryId);
                if (category != null)
                {
                    WordItem categoryItem = category.CreateCategoryCardItem();
                    WordSolitaireCard categoryCard = CreateWordCard(categoryItem);
                    if (categoryCard != null)
                    {
                        // categoryCard的WordCardType已通过InitWithWordItem设置为CategoryCard
                        // IsCategoryCard属性会自动根据WordCardType计算，无需手动赋值
                        allCards.Add(categoryCard);
                    }
                }
            }
            
            // 3. 洗牌
            ShuffleCards(allCards);
            
            // 4. 分发卡牌到列区（根据 InitialCardsPerColumn 配置）
            int cardIndex = 0;
            int totalDistributed = 0;

            if (ColumnDecks != null && _currentLevel.InitialCardsPerColumn != null)
            {
                Debug.Log($"[WordSolitaireCardLogic] 准备分发卡牌: ColumnDecks.Length={ColumnDecks.Length}, InitialCardsPerColumn.Length={_currentLevel.InitialCardsPerColumn.Length}, allCards.Count={allCards.Count}");

                for (int col = 0; col < ColumnDecks.Length && col < _currentLevel.InitialCardsPerColumn.Length; col++)
                {
                    int cardsToDeal = _currentLevel.InitialCardsPerColumn[col];

                    if (cardsToDeal <= 0)
                    {
                        Debug.LogWarning($"[WordSolitaireCardLogic] 列区 {col} 的初始卡牌数量为0，跳过");
                        continue;
                    }

                    Debug.Log($"[WordSolitaireCardLogic] 开始分发 {cardsToDeal} 张卡牌到列区 {col}");

                    for (int i = 0; i < cardsToDeal && cardIndex < allCards.Count; i++)
                    {
                        if (col >= ColumnDecks.Length)
                        {
                            Debug.LogError($"[WordSolitaireCardLogic] 数组越界: col={col}, ColumnDecks.Length={ColumnDecks.Length}");
                            break;
                        }
                        Debug.Log($"[WordSolitaireCardLogic] 准备访问allCards[{cardIndex}], allCards.Count={allCards.Count}");
                        WordSolitaireCard card = allCards[cardIndex];
                        // 列区最下方（最后放置的）卡牌翻开
                        bool shouldFaceUp = (i == cardsToDeal - 1);
                        Debug.Log($"[WordSolitaireCardLogic] 分发卡牌: col={col}, cardIndex={cardIndex}, shouldFaceUp={shouldFaceUp}");
                        ColumnDecks[col].PushCard(card, shouldFaceUp, 0);
                        cardIndex++;
                        totalDistributed++;
                    }
                }

                Debug.Log($"[WordSolitaireCardLogic] 已分发 {totalDistributed} 张卡牌到列区，剩余 {allCards.Count - cardIndex} 张待处理");
            }
            else if (ColumnDecks != null && _currentLevel.InitialCardsPerColumn == null)
            {
                Debug.LogWarning("[WordSolitaireCardLogic] InitialCardsPerColumn 未配置，所有列区将保持空状态");
            }
            
            // 5. 剩余卡牌放入牌库
            Debug.Log($"[WordSolitaireCardLogic] 准备放入牌库: cardIndex={cardIndex}, allCards.Count={allCards.Count}, PackDeck={PackDeck != null}");
            Debug.Log($"[WordSolitaireCardLogic] 剩余卡牌数量: {allCards.Count - cardIndex}");
            for (int i = cardIndex; i < allCards.Count; i++)
            {
                Debug.Log($"[WordSolitaireCardLogic] 放入第 {i} 张卡牌到牌库");
                if (PackDeck != null)
                {
                    PackDeck?.PushCard(allCards[i], false, 0);
                }
            }
            Debug.Log($"[WordSolitaireCardLogic] 牌库卡牌放入完成，牌库当前数量: {PackDeck?.CardsCount ?? 0}");
            
            // 6. 更新所有牌堆显示
            PackDeck?.UpdateCardsPosition(false);
            if (ColumnDecks != null)
            {
                foreach (var deck in ColumnDecks)
                {
                    deck?.UpdateCardsPosition(false);
                }
            }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[WordSolitaireCardLogic] InitializeCards 发生错误: {ex.Message}\n{ex.StackTrace}");
            }
        }
        
        /// <summary>
        /// 创建词语卡牌
        /// </summary>
        private WordSolitaireCard CreateWordCard(WordItem wordItem)
        {
            if (WordCardPrefab == null)
            {
                Debug.LogError("[WordSolitaireCardLogic] WordCardPrefab 未配置，无法创建卡牌");
                return null;
            }
            
            GameObject cardObj = Instantiate(WordCardPrefab, transform);
            WordSolitaireCard card = cardObj.GetComponent<WordSolitaireCard>();
            
            // 如果预制体中没有WordSolitaireCard组件，则动态添加
            if (card == null)
            {
                card = cardObj.AddComponent<WordSolitaireCard>();
                Debug.LogWarning("[WordSolitaireCardLogic] WordCardPrefab 中没有 WordSolitaireCard 组件，已动态添加");
            }
            
            if (card != null)
            {
                card.InitWithWordItem(wordItem);
                card.CardLogicComponent = this;
            }
            else
            {
                Debug.LogError($"[WordSolitaireCardLogic] 创建WordSolitaireCard组件失败: {wordItem?.WordId}");
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
            if (CategorySlots != null && CategorySlots.Length > 0)
            {
                AceDeckArray = new Deck[CategorySlots.Length];
                for (int i = 0; i < CategorySlots.Length; i++)
                {
                    AceDeckArray[i] = CategorySlots[i];
                }
            }
            else
            {
                Debug.LogWarning("[WordSolitaireCardLogic] CategorySlots 为 null 或空，创建空的 AceDeckArray");
                AceDeckArray = new Deck[0];
            }

            // 初始化BottomDeckArray（列区）
            if (ColumnDecks != null && ColumnDecks.Length > 0)
            {
                BottomDeckArray = new Deck[ColumnDecks.Length];
                for (int i = 0; i < ColumnDecks.Length; i++)
                {
                    BottomDeckArray[i] = ColumnDecks[i];
                }
            }
            else
            {
                Debug.LogWarning("[WordSolitaireCardLogic] ColumnDecks 为 null 或空，创建空的 BottomDeckArray");
                BottomDeckArray = new Deck[0];
            }

            // 设置WasteDeck为HandDeck
            WasteDeck = HandDeck;

            // 牌库已在字段中初始化
        }

        /// <summary>
        /// 初始化所有牌堆数组（WordSolitaire 使用动态数量的牌堆，需要重写）
        /// </summary>
        protected override void InitAllDeckArray()
        {
            Debug.Log("[WordSolitaireCardLogic] InitAllDeckArray: 开始初始化");

            // 计算实际需要的牌堆数量
            int deckCount = 0;
            deckCount += AceDeckArray != null ? AceDeckArray.Length : 0;
            deckCount += BottomDeckArray != null ? BottomDeckArray.Length : 0;
            deckCount += WasteDeck != null ? 1 : 0;
            deckCount += PackDeck != null ? 1 : 0;

            Debug.Log($"[WordSolitaireCardLogic] InitAllDeckArray: AceDeckArray={AceDeckArray?.Length ?? 0}, BottomDeckArray={BottomDeckArray?.Length ?? 0}, WasteDeck={WasteDeck != null}, PackDeck={PackDeck != null}");

            // 创建AllDeckArray
            AllDeckArray = new Deck[deckCount];

            int j = 0;
            for (int i = 0; i < AceDeckArray.Length; i++)
            {
                AceDeckArray[i].Type = DeckType.DECK_TYPE_ACE;
                AllDeckArray[j++] = AceDeckArray[i];
            }

            for (int i = 0; i < BottomDeckArray.Length; i++)
            {
                BottomDeckArray[i].Type = DeckType.DECK_TYPE_BOTTOM;
                AllDeckArray[j++] = BottomDeckArray[i];
            }

            if (WasteDeck != null)
            {
                WasteDeck.Type = DeckType.DECK_TYPE_WASTE;
                AllDeckArray[j++] = WasteDeck;
            }

            if (PackDeck != null)
            {
                PackDeck.Type = DeckType.DECK_TYPE_PACK;
                AllDeckArray[j++] = PackDeck;
            }

            for (int i = 0; i < AllDeckArray.Length; i++)
            {
                if (AllDeckArray[i] != null)
                {
                    AllDeckArray[i].DeckNum = i;
                }
            }

            Debug.Log($"[WordSolitaireCardLogic] InitAllDeckArray: 总共 {deckCount} 个牌堆，初始化完成");
        }
        
        /// <summary>
        /// 设置牌库背景（WordSolitaire 不使用 PackDeck，所以重写此方法）
        /// </summary>
        protected override void SetPackDeckBg()
        {
            // WordSolitaire 不使用 PackDeck，跳过背景设置
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
                
                // 消耗步数
                ConsumeStep();
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
            
            // 收集要一起移动的卡牌（主卡 + 上方同词组卡牌）
            List<WordSolitaireCard> cardsToMove = CollectCardsToMove(card, originalDeck);
            
            // 从原牌堆移除所有要移动的卡牌
            if (originalDeck != null)
            {
                foreach (var c in cardsToMove)
                {
                    originalDeck.RemoveCard(c);
                }
                originalDeck.UpdateCardsPosition(false);
            }
            
            // 添加到目标牌堆
            foreach (var c in cardsToMove)
            {
                targetDeck.PushCard(c, true, 1);
            }
            targetDeck.UpdateCardsPosition(false);
            
            // 播放音效
            if (AudioCtrl != null)
            {
                AudioCtrl.Play(AudioController.AudioType.Move);
            }
            
            // 检查匹配（只检查主卡）
            if (targetDeck.DeckType == WordDeckType.CategorySlot)
            {
                OnCardMatchedToCategorySlot(card, targetDeck);
            }
            
            await Task.Yield();
        }
        
        /// <summary>
        /// 收集需要一起移动的卡牌
        /// 规则：拖拽的卡牌及其上方的同词组卡牌
        /// </summary>
        private List<WordSolitaireCard> CollectCardsToMove(WordSolitaireCard card, WordSolitaireDeck originalDeck)
        {
            List<WordSolitaireCard> cardsToMove = new List<WordSolitaireCard>();
            cardsToMove.Add(card); // 添加主卡
            
            if (originalDeck == null)
            {
                return cardsToMove;
            }
            
            // 找到卡牌在牌堆中的索引
            int cardIndex = originalDeck.CardsArray.IndexOf(card);
            if (cardIndex < 0)
            {
                return cardsToMove;
            }
            
            // 从最后一张牌开始，向上收集同词组的卡牌
            for (int i = cardIndex - 1; i >= 0; i--)
            {
                WordSolitaireCard aboveCard = originalDeck.CardsArray[i] as WordSolitaireCard;
                if (aboveCard != null && aboveCard.CategoryId == card.CategoryId)
                {
                    cardsToMove.Add(aboveCard);
                }
                else
                {
                    // 遇到不同词组的卡牌，停止收集
                    break;
                }
            }
            
            return cardsToMove;
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
        /// 借鉴 Klondike 机制：牌库有牌时发牌，牌库无牌时恢复库存
        /// 注意：恢复库存也需要消耗步数（根据产品文档）
        /// </summary>
        public override void OnClickPack()
        {
            // 牌库和手牌区都为空时，无法操作
            if (PackDeck == null || HandDeck == null) return;
            
            if (PackDeck.HasCards)
            {
                // 牌库有牌：发一张牌到手牌区
                Card card = PackDeck.Pop();
                if (card != null)
                {
                    HandDeck?.PushCard(card, true, 1);
                    HandDeck?.UpdateCardsPosition(false);
                    PackDeck?.UpdateCardsPosition(false);
                    
                    // 播放音效
                    if (AudioCtrl != null)
                    {
                        AudioCtrl.Play(AudioController.AudioType.MoveToWaste);
                    }
                    
                    // 消耗步数
                    ConsumeStep();
                    
                    // 检查牌库是否耗尽
                    if (!PackDeck.HasCards)
                    {
                        GameEventBus.PublishPackEmpty();
                    }
                }
            }
            else if (HandDeck.HasCards)
            {
                // 牌库无牌但有手牌：恢复库存（将手牌放回牌库），消耗步数
                RestorePackCards();
            }
            else
            {
                // 牌库和手牌区都空：播放错误音效
                if (AudioCtrl != null)
                {
                    AudioCtrl.Play(AudioController.AudioType.Error);
                }
            }
        }
        
        #endregion
        
        #region 辅助方法
        
        /// <summary>
        /// 消耗步数
        /// </summary>
        private void ConsumeStep()
        {
            if (GameManagerComponent is WordSolitaireGameManager wordManager)
            {
                wordManager.OnStepConsumed();
            }
        }
        
        /// <summary>
        /// 恢复库存 - 将手牌按初始顺序放回牌库
        /// 注意：此操作消耗1步（根据产品文档）
        /// </summary>
        public void RestorePackCards()
        {
            if (HandDeck == null || PackDeck == null) return;
            if (!HandDeck.HasCards) return;
            
            // 获取手牌区所有卡牌（从旧到新，保持顺序）
            List<Card> handCards = new List<Card>(HandDeck.CardsArray);
            
            // 从手牌区移除所有卡牌
            foreach (var card in handCards)
            {
                HandDeck.RemoveCard(card);
            }
            
            // 按初始顺序放回牌库（最旧的先放，在底部）
            foreach (var card in handCards)
            {
                PackDeck.PushCard(card, false, 0);  // 放入牌库时显示卡背
            }
            
            // 更新显示
            HandDeck.UpdateCardsPosition(false);
            PackDeck.UpdateCardsPosition(false);
            
            // 播放恢复音效
            if (AudioCtrl != null)
            {
                AudioCtrl.Play(AudioController.AudioType.MoveToPack);
            }
            
            // 消耗步数（根据产品文档要求）
            ConsumeStep();
        }
        
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
        
        /// <summary>
        /// 重写Shuffle方法
        /// WordSolitaire使用自己的卡牌初始化逻辑,不需要基类的Shuffle
        /// </summary>
        public override void Shuffle(bool bReplay)
        {
            // WordSolitaire的卡牌在InitCardLogic()中的InitializeCards()方法中已经生成和分发
            // 这里不需要做任何操作,避免基类Shuffle()中的数组越界问题
            
            // 如果是重新开始游戏,需要重新生成卡牌
            if (bReplay)
            {
                // 重新初始化卡牌
                InitializeCards();
            }
            
            // 更新提示系统
            if (HintManagerComponent != null)
            {
                HintManagerComponent.IsHintWasUsed = false;
            }
            
            // 标记游戏需要重置牌库
            IsNeedResetPack = true;
        }
        
        #endregion
        
        #region 动态生成Deck
        
        /// <summary>
        /// 动态生成分类槽和列区Deck
        /// </summary>
        private void GenerateDecks()
        {
            if (_currentLevel == null)
            {
                Debug.LogWarning("[WordSolitaireCardLogic] _currentLevel 为 null，跳过生成Deck");
                return;
            }
            
            if (DeckPrefab == null)
            {
                Debug.LogError("[WordSolitaireCardLogic] 无法生成Deck: DeckPrefab 未配置");
                return;
            }
            
            Debug.Log($"[WordSolitaireCardLogic] 开始生成Deck，关卡 {_currentLevel.LevelId}");
            
            // 清理现有Deck
            ClearExistingDecks();
            
            // 生成分类槽
            GenerateCategorySlots();
            
            // 生成列区Deck
            GenerateColumnDecks();
            
            // 应用自适应布局
            ApplyAdaptiveLayout();
            
            // 不调用 ApplyOrientationToDynamicElements，因为自适应布局已经设置了正确的位置
            
            Debug.Log($"[WordSolitaireCardLogic] Deck生成完成: {CategorySlots?.Length ?? 0} 个分类槽，{ColumnDecks?.Length ?? 0} 个列区");
        }
        
        /// <summary>
        /// 清理现有Deck
        /// </summary>
        private void ClearExistingDecks()
        {
            // 清理方向管理器中的动态元素 - 已注释，暂时不需要考虑横屏
            // if (OrientationManager != null)
            // {
            //     OrientationManager.ClearAllDynamicElements();
            // }

            if (CategorySlots != null)
            {
                foreach (var slot in CategorySlots)
                {
                    if (slot != null && slot.gameObject != null)
                    {
                        Destroy(slot.gameObject);
                    }
                }
            }

            if (ColumnDecks != null)
            {
                foreach (var deck in ColumnDecks)
                {
                    if (deck != null && deck.gameObject != null)
                    {
                        Destroy(deck.gameObject);
                    }
                }
            }

            CategorySlots = null;
            ColumnDecks = null;
        }
        
        /// <summary>
        /// 动态生成分类槽
        /// </summary>
        private void GenerateCategorySlots()
        {
            if (CategorySlotsContainer == null || _currentLevel.CategoryIds == null) return;
            
            List<WordSolitaireDeck> validSlots = new List<WordSolitaireDeck>();
            
            for (int i = 0; i < _currentLevel.CategoryIds.Length; i++)
            {
                int categoryId = _currentLevel.CategoryIds[i];
                
                // 验证分类ID是否存在于词库中
                WordCategoryData category = WordDataManager.GetCategoryById(categoryId);
                if (category == null)
                {
                    Debug.LogError($"[WordSolitaireCardLogic] 分类ID {categoryId} 不存在，跳过创建分类槽");
                    continue;
                }
                
                GameObject deckObj = Instantiate(DeckPrefab, CategorySlotsContainer);
                deckObj.name = $"CategorySlot_{category.CategoryName}";
                
                WordSolitaireDeck deck = deckObj.GetComponent<WordSolitaireDeck>();
                if (deck == null)
                {
                    deck = deckObj.AddComponent<WordSolitaireDeck>();
                }
                
                // 配置分类槽属性
                deck.DeckType = WordDeckType.CategorySlot;
                deck.CategoryId = categoryId;
                deck.TargetCardCount = _currentLevel.CategorySlotSize > 0 ? _currentLevel.CategorySlotSize : 5; // 默认5张
                deck.DeckNum = i;
                deck.CardLogicComponent = this;
                
                validSlots.Add(deck);

                // 注册到方向管理器（支持动态方向适配）- 已注释，暂时不需要考虑横屏
                // if (OrientationManager != null)
                // {
                //     OrientationElementKey key = WordSolitaireOrientationManager.GetCategorySlotKey(i);
                //     OrientationManager.RegisterDynamicElement(deckObj, key);
                //
                //     // 注册后立即禁用 AspectRatioFitter，使用手动布局
                //     var aspectRatioFitter = deckObj.GetComponent<AspectRatioFitter>();
                //     if (aspectRatioFitter != null)
                //     {
                //         aspectRatioFitter.enabled = false;
                //     }
                // }

                Debug.Log($"[WordSolitaireCardLogic] 成功创建分类槽: {category.CategoryName} (ID={categoryId}, 目标={deck.TargetCardCount})");
            }
            
            // 更新数组
            CategorySlots = validSlots.ToArray();
            
            // 验证至少有一个有效的分类槽
            if (CategorySlots.Length == 0)
            {
                Debug.LogError("[WordSolitaireCardLogic] 没有有效的分类槽，无法初始化游戏");
            }
            else
            {
                Debug.Log($"[WordSolitaireCardLogic] 成功创建 {CategorySlots.Length} 个分类槽");
            }
        }
        
        /// <summary>
        /// 动态生成列区Deck
        /// </summary>
        private void GenerateColumnDecks()
        {
            if (ColumnDecksContainer == null) return;
            
            int columnCount = _currentLevel.ColumnCount;
            if (columnCount <= 0) columnCount = 4; // 默认4列
            
            ColumnDecks = new WordSolitaireDeck[columnCount];
            
            for (int i = 0; i < columnCount; i++)
            {
                GameObject deckObj = Instantiate(DeckPrefab, ColumnDecksContainer);
                deckObj.name = $"ColumnDeck_{i}";
                
                WordSolitaireDeck deck = deckObj.GetComponent<WordSolitaireDeck>();
                if (deck == null)
                {
                    deck = deckObj.AddComponent<WordSolitaireDeck>();
                }
                
                // 配置列区属性
                deck.DeckType = WordDeckType.Column;
                deck.ColumnIndex = i;
                deck.DeckNum = i;
                deck.CardLogicComponent = this;
                
                ColumnDecks[i] = deck;

                // 注册到方向管理器（支持动态方向适配）- 已注释，暂时不需要考虑横屏
                // if (OrientationManager != null)
                // {
                //     OrientationElementKey key = WordSolitaireOrientationManager.GetColumnDeckKey(i);
                //     OrientationManager.RegisterDynamicElement(deckObj, key);
                //
                //     // 注册后立即禁用 AspectRatioFitter，使用手动布局
                //     var aspectRatioFitter = deckObj.GetComponent<AspectRatioFitter>();
                //     if (aspectRatioFitter != null)
                //     {
                //         aspectRatioFitter.enabled = false;
                //     }
                // }
            }

            // 应用方向到所有动态元素 - 已注释，暂时不需要考虑横屏
            // if (OrientationManager != null)
            // {
            //     OrientationManager.ApplyOrientationToDynamicElements();
            // }
        }
        
        /// <summary>
        /// 应用自适应布局（根据数量调整间距和缩放）
        /// 设计规格：
        /// - 容器宽度：720像素
        /// - 左右各留10像素间距
        /// - CategorySlot 分类槽尺寸：155x208像素
        /// - WordCard 卡牌尺寸：163x218像素
        /// <summary>
        /// 应用自适应布局
        /// 布局规则：
        /// - 分类槽和列区都是居中对齐
        /// - 两个元素之间的最大间距为 50
        /// - 锚点：(0.5, 1) - 上边缘居中
        /// </summary>
        private void ApplyAdaptiveLayout()
        {
            // 移除容器中的布局组件，使用手动计算位置
            RemoveLayoutComponents();

            // 设计常量
            const float CONTAINER_WIDTH = 720f;
            const float MARGIN = 10f;
            const float CATEGORY_WIDTH = 155f;    // CategorySlot 实际宽度
            const float CATEGORY_HEIGHT = 208f;   // CategorySlot 实际高度
            const float WORDCARD_WIDTH = 163f;   // WordCard 实际宽度
            const float WORDCARD_HEIGHT = 218f;  // WordCard 实际高度
            const float BASE_SPACING = 15f;
            const float MAX_SPACING = 50f;       // 最大间距限制
            const float VERTICAL_SPACING = 20f;  // 垂直间距

            // ============ 分类槽自适应布局 ============
            if (CategorySlotsContainer != null && CategorySlots != null && CategorySlots.Length > 0)
            {
                int slotCount = CategorySlots.Length;

                // 计算可用宽度
                float availableWidth = CONTAINER_WIDTH - 2 * MARGIN;

                // 计算每个分类槽的理想宽度（包括间距）
                float totalSpacingWidth = BASE_SPACING * (slotCount - 1);
                float availableForCards = availableWidth - totalSpacingWidth;
                float idealCardWidth = availableForCards / slotCount;

                // 计算缩放比例
                float scale = idealCardWidth / CATEGORY_WIDTH;

                // 限制缩放范围（0.5 ~ 1.0）
                scale = Mathf.Clamp(scale, 0.5f, 1.0f);

                // 如果4个或更少，使用固定比例1.0
                if (slotCount <= 4)
                {
                    scale = 1.0f;
                }

                // 计算实际间距（居中对齐，最大间距为50）
                float scaledCardWidth = CATEGORY_WIDTH * scale;
                float scaledSpacing = (availableWidth - scaledCardWidth * slotCount) / (slotCount - 1);

                // 限制间距在2-50之间
                float actualSpacing = Mathf.Clamp(scaledSpacing, 2f, MAX_SPACING);

                // 计算起始位置（居中对齐）
                float totalWidth = scaledCardWidth * slotCount + actualSpacing * (slotCount - 1);
                float startX = -totalWidth / 2f + scaledCardWidth / 2f;

                Debug.Log($"[ApplyAdaptiveLayout] 分类槽布局计算: totalWidth={totalWidth}, startX={startX}, scaledCardWidth={scaledCardWidth}, scaledSpacing={scaledSpacing}");

                // 动态设置每个分类槽的位置
                for (int i = 0; i < slotCount; i++)
                {
                    if (CategorySlots[i] != null)
                    {
                        var rectTransform = CategorySlots[i].transform as RectTransform;

                        // 禁用AspectRatioFitter以避免它覆盖我们设置的位置 - 已注释，暂时不需要考虑横屏
                        // var aspectRatioFitter = CategorySlots[i].GetComponent<AspectRatioFitter>();
                        // if (aspectRatioFitter != null)
                        // {
                        //     aspectRatioFitter.enabled = false;
                        // }

                        rectTransform.localScale = Vector3.one * scale;
                        float calculatedX = startX + i * (scaledCardWidth + actualSpacing);
                        rectTransform.anchoredPosition = new Vector2(calculatedX, 0);
                        Debug.Log($"[ApplyAdaptiveLayout] 分类槽 {i} 设置位置: {calculatedX}");
                    }
                }

                Debug.Log($"[ApplyAdaptiveLayout] 分类槽: 数量={slotCount}, 缩放={scale:F2}, 间距={actualSpacing:F1}");
            }

            // ============ 列区自适应布局 ============
            if (ColumnDecksContainer != null && ColumnDecks != null && ColumnDecks.Length > 0)
            {
                int columnCount = ColumnDecks.Length;
                
                // 计算可用宽度
                float availableWidth = CONTAINER_WIDTH - 2 * MARGIN;
                
                // 计算网格布局参数
                // 4列或更少：单行，4列
                // 5-8列：2行，每行4列
                // 9-12列：3行，每行4列
                int maxColumnsPerRow = 4;
                int columnsPerRow = Mathf.Min(columnCount, maxColumnsPerRow);
                int rows = Mathf.CeilToInt((float)columnCount / columnsPerRow);
                
                // 计算每列可用宽度（包括间距）
                float totalSpacingWidth = BASE_SPACING * (columnsPerRow - 1);
                float availableForColumns = availableWidth - totalSpacingWidth;
                float idealColumnWidth = availableForColumns / columnsPerRow;
                
                // 计算缩放比例
                float scale = idealColumnWidth / WORDCARD_WIDTH;
                
                // 限制缩放范围（0.4 ~ 1.0，列区可以更小）
                scale = Mathf.Clamp(scale, 0.4f, 1.0f);
                
                // 如果4列或更少，使用固定比例1.0
                if (columnCount <= 4)
                {
                    scale = 1.0f;
                    columnsPerRow = columnCount;  // 使用实际列数,而不是固定为4
                }

                // 计算实际间距（居中对齐，最大间距为50）
                float scaledCardWidth = WORDCARD_WIDTH * scale;
                float scaledCardHeight = WORDCARD_HEIGHT * scale;
                float scaledSpacing = (availableWidth - scaledCardWidth * columnsPerRow) / (columnsPerRow - 1);

                // 限制间距在2-50之间
                float actualSpacing = Mathf.Clamp(scaledSpacing, 2f, MAX_SPACING);

                // 计算每行的起始位置（居中对齐）
                // 注意：实际占用的宽度应该根据实际列数计算，而不是每行的最大列数
                int actualColumnsInCurrentRow = (rows == 1) ? columnCount : columnsPerRow;
                float actualRowWidth = scaledCardWidth * actualColumnsInCurrentRow + actualSpacing * (actualColumnsInCurrentRow - 1);
                float startX = -actualRowWidth / 2f + scaledCardWidth / 2f;
                float startY = 0; // 锚点是 (0.5, 1)，从上往下排列

                Debug.Log($"[ApplyAdaptiveLayout] 列区布局计算: actualRowWidth={actualRowWidth}, startX={startX}, columnsPerRow={columnsPerRow}, rows={rows}, actualColumnsInCurrentRow={actualColumnsInCurrentRow}, scaledCardWidth={scaledCardWidth}, actualSpacing={actualSpacing}");

                // 动态设置每个列区的位置
                for (int i = 0; i < columnCount; i++)
                {
                    if (ColumnDecks[i] != null)
                    {
                        var rectTransform = ColumnDecks[i].transform as RectTransform;

                        // 禁用AspectRatioFitter以避免它覆盖我们设置的位置 - 已注释，暂时不需要考虑横屏
                        // var aspectRatioFitter = ColumnDecks[i].GetComponent<AspectRatioFitter>();
                        // if (aspectRatioFitter != null)
                        // {
                        //     aspectRatioFitter.enabled = false;
                        // }

                        rectTransform.localScale = Vector3.one * scale;

                        // 计算行列索引
                        int row = i / columnsPerRow;
                        int col = i % columnsPerRow;

                        // 设置位置
                        float x = startX + col * (scaledCardWidth + actualSpacing);
                        float y = -row * (scaledCardHeight + VERTICAL_SPACING);
                        rectTransform.anchoredPosition = new Vector2(x, y);
                        Debug.Log($"[ApplyAdaptiveLayout] 列区 {i} 设置位置: row={row}, col={col}, x={x}, y={y}");
                    }
                }

                Debug.Log($"[ApplyAdaptiveLayout] 列区: 数量={columnCount}, 列/行={columnsPerRow}/{rows}, 缩放={scale:F2}, 间距={actualSpacing:F1}");
            }
        }
        /// <summary>
        /// 移除容器中的布局组件，使用手动计算位置
        /// </summary>
        private void RemoveLayoutComponents()
        {
            if (CategorySlotsContainer != null)
            {
                var hlg = CategorySlotsContainer.GetComponent<UnityEngine.UI.HorizontalLayoutGroup>();
                if (hlg != null)
                {
                    Destroy(hlg);
                }
            }
            
            if (ColumnDecksContainer != null)
            {
                var glg = ColumnDecksContainer.GetComponent<UnityEngine.UI.GridLayoutGroup>();
                if (glg != null)
                {
                    Destroy(glg);
                }
            }
        }
        
        #endregion
    }
}
