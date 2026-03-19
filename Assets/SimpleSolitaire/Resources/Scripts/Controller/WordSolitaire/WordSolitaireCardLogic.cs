using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimpleSolitaire.Controller.WordSolitaire
{
    /// <summary>
    /// Word Solitaire 核心游戏逻辑
    /// </summary>
    public class WordSolitaireCardLogic : CardLogic
    {
        [Header("Word Solitaire 组件")]
        public WordDataManager WordDataManager;
        public LevelDataManager LevelDataManager;
        
        [Header("牌堆引用")]
        public WordSolitaireDeck PackDeck;           // 牌库
        public WordSolitaireDeck HandDeck;           // 手牌区
        public WordSolitaireDeck[] ColumnDecks;      // 列区牌堆
        public WordSolitaireDeck[] CategorySlots;    // 分类槽
        
        // 当前关卡数据
        private LevelData _currentLevel;
        
        /// <summary>
        /// 当前关卡
        /// </summary>
        public LevelData CurrentLevel => _currentLevel;
        
        /// <summary>
        /// 卡牌编号数组（用于撤销）
        /// </summary>
        public int[] CardNumberArray { get; private set; }
        
        /// <summary>
        /// 初始化关卡
        /// </summary>
        public void InitializeLevel(LevelData levelData)
        {
            _currentLevel = levelData;
        }
        
        /// <summary>
        /// 加载指定关卡
        /// </summary>
        public void LoadLevel(int levelId)
        {
            if (LevelDataManager != null)
            {
                LevelDataManager.LoadLevel(levelId);
                _currentLevel = LevelDataManager.GetCurrentLevel();
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
            
            return true;
        }
        
        /// <summary>
        /// 显示提示
        /// </summary>
        public void ShowHint()
        {
            if (HintComponent is WordSolitaireHintManager wordHintManager)
            {
                wordHintManager.HighlightHint();
            }
        }
        
        /// <summary>
        /// 初始化特定卡牌编号（用于撤销恢复）
        /// </summary>
        public void InitSpecificCardNums(int[] cardNums)
        {
            CardNumberArray = cardNums;
        }
        
        /// <summary>
        /// 设置规则（兼容基类）
        /// </summary>
        public void SetRuleImmediately(int rule)
        {
            // Word Solitaire 不需要规则设置
        }
        
        /// <summary>
        /// 获取所有牌堆
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
    }
}
