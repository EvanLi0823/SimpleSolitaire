using SimpleSolitaire.Controller.UI;
using SimpleSolitaire.Model.Config;
using UnityEngine;

namespace SimpleSolitaire.Controller.WordSolitaire
{
    /// <summary>
    /// Word Solitaire 游戏管理器
    /// 负责游戏流程控制、步数管理、胜负处理和金币奖励
    /// </summary>
    public class WordSolitaireGameManager : GameManager
    {
        [Header("Word Solitaire 组件")]
        [SerializeField]
        private CoinManager _coinManager;
        
        [SerializeField]
        private LevelDataManager _levelDataManager;
        
        [Header("游戏配置")]
        [SerializeField]
        private WordSolitaireConfig _gameConfig;
        
        // 当前关卡数据
        private LevelData _currentLevel;
        
        // 当前剩余步数
        private int _remainingSteps;
        
        // 是否已触发第一个分类卡集齐广告
        private bool _hasShownMatchAd;
        
        /// <summary>
        /// 当前关卡数据
        /// </summary>
        public LevelData CurrentLevel => _currentLevel;
        
        /// <summary>
        /// 剩余步数
        /// </summary>
        public int RemainingSteps => _remainingSteps;
        
        /// <summary>
        /// 是否显示结算广告
        /// </summary>
        public bool IsShowResultAd => _currentLevel != null && _currentLevel.IsShowResultAd;
        
        /// <summary>
        /// 是否显示匹配广告
        /// </summary>
        public bool IsShowMatchAd => _currentLevel != null && _currentLevel.IsShowMatchAd;

        /// <summary>
        /// 初始化游戏
        /// </summary>
        protected override void InitializeGame()
        {
            base.InitializeGame();
            
            // 预加载词库数据（优化：提前加载避免首次使用时的延迟）
            var wordDataManager = FindObjectOfType<WordDataManager>();
            wordDataManager?.PreloadAllCategories();
            
            // 初始化金币管理器
            if (_coinManager == null)
            {
                _coinManager = gameObject.AddComponent<CoinManager>();
            }
            _coinManager.Initialize(_gameConfig != null ? _gameConfig.InitialCoins : 100);
            
            // 重置广告触发标记
            _hasShownMatchAd = false;
        }

        /// <summary>
        /// 初始化卡牌逻辑（由基类调用）
        /// </summary>
        protected override void InitCardLogic()
        {
            // 加载当前关卡
            if (_levelDataManager == null)
            {
                Debug.LogError("[WordSolitaireGameManager] _levelDataManager 未配置，无法加载关卡数据");
                _currentLevel = null;
                _remainingSteps = 999;
            }
            else
            {
                _currentLevel = _levelDataManager.GetCurrentLevel();
                if (_currentLevel == null)
                {
                    Debug.LogError("[WordSolitaireGameManager] 无法加载当前关卡数据");
                    _remainingSteps = 999;
                }
                else
                {
                    _remainingSteps = _currentLevel.MaxMoves;
                    Debug.Log($"[WordSolitaireGameManager] 关卡 {_currentLevel.LevelId} 加载完成，最大步数: {_remainingSteps}");
                }
            }
            
            // 初始化卡牌逻辑
            if (_cardLogic is WordSolitaireCardLogic wordCardLogic)
            {
                wordCardLogic.InitializeLevel(_currentLevel);
            }
            
            // 重置广告触发标记
            _hasShownMatchAd = false;
            
            // 发布步数初始化事件
            GameEventBus.PublishStepChanged(_remainingSteps);
        }

        /// <summary>
        /// 步数消耗事件
        /// 由 CardLogic 在卡牌移动后调用
        /// </summary>
        public void OnStepConsumed()
        {
            // 增加已走步数
            _stepCount++;
            
            // 减少剩余步数（如果不是无限步数）
            if (_currentLevel != null && _currentLevel.MaxMoves != 999)
            {
                _remainingSteps--;
            }
            
            // 发布步数变化事件
            GameEventBus.PublishStepChanged(_remainingSteps);
            
            // 检查失败条件
            if (_currentLevel != null && _currentLevel.MaxMoves != 999 && _remainingSteps <= 0)
            {
                // 延迟检查，确保所有移动完成
                Invoke(nameof(CheckLoseCondition), 0.1f);
            }
        }

        /// <summary>
        /// 检查失败条件
        /// </summary>
        private void CheckLoseCondition()
        {
            if (_cardLogic is WordSolitaireCardLogic wordCardLogic)
            {
                if (!wordCardLogic.CheckWinCondition())
                {
                    OnLose();
                }
            }
        }

        /// <summary>
        /// 过关处理
        /// </summary>
        public override void HasWinGame()
        {
            StopGameTimer();
            
            // 播放胜利音效
            var audioController = AudioController.Instance;
            audioController?.Play(AudioController.AudioType.Win);
            
            // 计算金币奖励
            int reward = CalculateLevelReward();
            
            // 发放金币奖励
            if (_coinManager != null && reward > 0)
            {
                _coinManager.AddCoins(reward);
            }
            
            // 解锁下一关
            _levelDataManager?.UnlockNextLevel();
            
            // 显示胜利弹窗
            _layerMediator?.ShowWinLayer();
            
            // 更新统计数据
            _statisticsComponent?.IncreasePlayedGamesTime(_timeCount);
            _statisticsComponent?.GetAverageGameTime();
            _statisticsComponent?.IncreaseScoreAmount(_scoreCount);
            _statisticsComponent?.IncreaseWonGamesAmount();
            _statisticsComponent?.SetBestWinTime(_timeCount);
            _statisticsComponent?.SetBestWinMoves(_stepCount);
            
            // 显示插屏广告（根据配置）
            if (IsShowResultAd && _adsManagerComponent != null)
            {
                _adsManagerComponent.ShowInterstitial();
            }
            
            // 发布胜利事件
            GameEventBus.PublishWordSolitaireWin(_currentLevel?.LevelId ?? 0, reward);
        }

        /// <summary>
        /// 失败处理
        /// </summary>
        protected void OnLose()
        {
            StopGameTimer();
            
            // 播放失败音效
            var audioController = AudioController.Instance;
            audioController?.Play(AudioController.AudioType.Error);
            
            // 显示失败弹窗
            _layerMediator?.ShowGameLayer();
            
            // 更新统计数据
            _statisticsComponent?.IncreasePlayedGamesTime(_timeCount);
            
            // 发布失败事件
            GameEventBus.PublishWordSolitaireLose(_currentLevel?.LevelId ?? 0);
        }

        /// <summary>
        /// 计算关卡奖励
        /// </summary>
        public int CalculateLevelReward()
        {
            if (_gameConfig == null) return 0;
            
            int baseReward = _gameConfig.NormalLevelReward;
            
            // 检查是否是里程碑关卡
            if (_currentLevel != null && _gameConfig.MilestoneLevels != null)
            {
                if (_gameConfig.MilestoneLevels.Contains(_currentLevel.LevelId))
                {
                    baseReward = _gameConfig.MilestoneLevelReward;
                }
            }
            
            // 根据剩余步数计算额外奖励
            int bonusReward = 0;
            if (_currentLevel != null && _currentLevel.MaxMoves != 999 && _remainingSteps > 0)
            {
                // 每剩余一步获得1金币奖励
                bonusReward = _remainingSteps;
            }
            
            return baseReward + bonusReward;
        }

        /// <summary>
        /// 卡牌匹配成功回调
        /// 由 WordSolitaireCardLogic 调用
        /// </summary>
        public void OnCardMatched(int categoryId, int currentCount, int targetCount)
        
        {
            // 增加分数
            AddScoreValue(Public.SCORE_NUMBER);
            
            // 发布分类匹配事件
            GameEventBus.PublishCategoryMatched(categoryId, currentCount, targetCount);
            
            // 检查是否是第一个完成的分类槽
            if (IsShowMatchAd && !_hasShownMatchAd && currentCount >= targetCount)
            {
                _hasShownMatchAd = true;
                _adsManagerComponent?.ShowInterstitial();
            }
        }

        /// <summary>
        /// 牌库耗尽回调
        /// 由 WordSolitaireCardLogic 调用
        /// </summary>
        public void OnPackEmpty()
        {
            GameEventBus.PublishPackEmpty();
        }

        /// <summary>
        /// 重新开始当前关卡
        /// </summary>
        public void RestartCurrentLevel()
        {
            _undoPerformComponent?.ResetUndoStates();
            InitCardLogic();
            
            if (_cardLogic != null)
            {
                _cardLogic.InitCardLogic();
                _cardLogic.Shuffle(false);
            }
            
            InitMenuView(false);
            _statisticsComponent?.IncreasePlayedGamesAmount();
        }

        /// <summary>
        /// 进入下一关
        /// </summary>
        public void GoToNextLevel()
        {
            if (_levelDataManager != null)
            {
                _levelDataManager.GoToNextLevel();
                RestartCurrentLevel();
            }
        }

        /// <summary>
        /// 开始下一关（UI调用的别名）
        /// </summary>
        public void StartNextLevel()
        {
            GoToNextLevel();
        }
        
        /// <summary>
        /// 重新开始游戏（UI调用的别名）
        /// </summary>
        public void RestartGame()
        {
            RestartCurrentLevel();
        }
        
        /// <summary>
        /// 返回主菜单
        /// </summary>
        public void ReturnToMainMenu()
        {
            // TODO: 实现返回主菜单逻辑
            Debug.Log("[WordSolitaireGameManager] 返回主菜单");
        }
        
        /// <summary>
        /// 获取提示（消耗道具）
        /// </summary>
        public void UseHint()
        {
            if (_coinManager != null)
            {
                // 这里可以实现消耗金币获取提示的逻辑
                // 或者检查是否有免费提示次数
            }
            
            // 调用提示系统
            if (_cardLogic is WordSolitaireCardLogic wordCardLogic)
            {
                wordCardLogic.ShowHint();
            }
        }

        /// <summary>
        /// 使用万能牌（消耗道具）
        /// </summary>
        public bool ActivateJoker()
        {
            // 检查是否有万能牌
            // 这里可以实现消耗金币或道具使用万能牌的逻辑
            return false;
        }
        
        /// <summary>
        /// 使用撤销（消耗道具）
        /// </summary>
        public bool UseUndo()
        {
            // 检查是否有撤销次数
            // 这里可以实现消耗金币或道具使用撤销的逻辑
            return false;
        }
    }
}
