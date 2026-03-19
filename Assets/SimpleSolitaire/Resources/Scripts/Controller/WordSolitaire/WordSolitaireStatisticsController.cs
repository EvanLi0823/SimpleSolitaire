using System;
using UnityEngine;

namespace SimpleSolitaire.Controller.WordSolitaire
{
    /// <summary>
    /// Word Solitaire 统计控制器
    /// 负责记录关卡最佳成绩、通关次数、最快用时等统计数据
    /// </summary>
    public class WordSolitaireStatisticsController : StatisticsController
    {
        /// <summary>
        /// 统计存储键前缀
        /// </summary>
        protected override string StatisticPrefs => "STATISTICS_WORDSOLITAIRE";

        /// <summary>
        /// 关卡统计存储键前缀
        /// </summary>
        private const string LEVEL_STATS_PREFIX = "WordSolitaire_LevelStats_";

        /// <summary>
        /// 当前关卡最佳时间（秒）
        /// </summary>
        private int _currentLevelBestTime;

        /// <summary>
        /// 当前关卡最佳步数
        /// </summary>
        private int _currentLevelBestMoves;

        /// <summary>
        /// 当前关卡通关次数
        /// </summary>
        private int _currentLevelClearCount;

        /// <summary>
        /// 卡牌逻辑组件引用
        /// </summary>
        private WordSolitaireCardLogic Logic => _cardLogicComponent as WordSolitaireCardLogic;

        /// <summary>
        /// 当前关卡ID
        /// </summary>
        private int CurrentLevelId => Logic?.CurrentLevel?.LevelId ?? 0;

        /// <summary>
        /// 唤醒时初始化
        /// </summary>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            
            // 订阅关卡变化事件
            GameEventBus.OnLevelChanged += OnLevelChanged;
        }

        /// <summary>
        /// 销毁时取消订阅
        /// </summary>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();
            
            // 取消订阅关卡变化事件
            GameEventBus.OnLevelChanged -= OnLevelChanged;
        }

        /// <summary>
        /// 关卡变化回调
        /// </summary>
        /// <param name="levelId">新关卡ID</param>
        private void OnLevelChanged(int levelId)
        {
            // 加载新关卡的统计数据
            LoadLevelStats(levelId);
        }

        /// <summary>
        /// 保存统计到PlayerPrefs
        /// </summary>
        protected override void SaveStatisticInPrefs()
        {
            // 保存全局统计
            SaveByRule(StatisticPrefs);

            // 保存当前关卡统计
            if (CurrentLevelId > 0)
            {
                SaveLevelStats(CurrentLevelId);
            }
        }

        /// <summary>
        /// 从PlayerPrefs获取统计
        /// </summary>
        protected override void GetStatisticFromPrefs()
        {
            // 获取全局统计
            GetByRule(StatisticPrefs);

            // 获取当前关卡统计
            if (CurrentLevelId > 0)
            {
                LoadLevelStats(CurrentLevelId);
            }
        }

        /// <summary>
        /// 保存关卡统计
        /// </summary>
        /// <param name="levelId">关卡ID</param>
        private void SaveLevelStats(int levelId)
        {
            string key = GetLevelStatsKey(levelId);
            string stats = string.Format("{0}/{1}/{2}", 
                _currentLevelBestTime, 
                _currentLevelBestMoves, 
                _currentLevelClearCount);
            
            PlayerPrefs.SetString(key, stats);
        }

        /// <summary>
        /// 加载关卡统计
        /// </summary>
        /// <param name="levelId">关卡ID</param>
        private void LoadLevelStats(int levelId)
        {
            string key = GetLevelStatsKey(levelId);
            
            if (PlayerPrefs.HasKey(key))
            {
                string stats = PlayerPrefs.GetString(key);
                string[] parts = stats.Split('/');
                
                if (parts.Length >= 3)
                {
                    _currentLevelBestTime = int.Parse(parts[0]);
                    _currentLevelBestMoves = int.Parse(parts[1]);
                    _currentLevelClearCount = int.Parse(parts[2]);
                }
            }
            else
            {
                // 初始化默认值
                _currentLevelBestTime = 0;
                _currentLevelBestMoves = 0;
                _currentLevelClearCount = 0;
            }
        }

        /// <summary>
        /// 获取关卡统计存储键
        /// </summary>
        /// <param name="levelId">关卡ID</param>
        /// <returns>存储键</returns>
        private string GetLevelStatsKey(int levelId)
        {
            return $"{LEVEL_STATS_PREFIX}{levelId}";
        }

        /// <summary>
        /// 设置关卡最佳时间
        /// </summary>
        /// <param name="time">时间（秒）</param>
        public void SetLevelBestTime(int time)
        {
            if (CurrentLevelId <= 0) return;

            // 如果当前记录为0或新记录更好（时间更短）
            if (_currentLevelBestTime == 0 || time < _currentLevelBestTime)
            {
                _currentLevelBestTime = time;
                SaveLevelStats(CurrentLevelId);
                
                Debug.Log($"[WordSolitaireStatisticsController] 关卡{CurrentLevelId}新最佳时间: {time}秒");
            }
        }

        /// <summary>
        /// 设置关卡最佳步数
        /// </summary>
        /// <param name="moves">步数</param>
        public void SetLevelBestMoves(int moves)
        {
            if (CurrentLevelId <= 0) return;

            // 如果当前记录为0或新记录更好（步数更少）
            if (_currentLevelBestMoves == 0 || moves < _currentLevelBestMoves)
            {
                _currentLevelBestMoves = moves;
                SaveLevelStats(CurrentLevelId);
                
                Debug.Log($"[WordSolitaireStatisticsController] 关卡{CurrentLevelId}新最佳步数: {moves}步");
            }
        }

        /// <summary>
        /// 增加关卡通关次数
        /// </summary>
        public void IncreaseLevelClearCount()
        {
            if (CurrentLevelId <= 0) return;

            _currentLevelClearCount++;
            SaveLevelStats(CurrentLevelId);
            
            Debug.Log($"[WordSolitaireStatisticsController] 关卡{CurrentLevelId}通关次数: {_currentLevelClearCount}");
        }

        /// <summary>
        /// 获取关卡最佳时间
        /// </summary>
        /// <param name="levelId">关卡ID</param>
        /// <returns>最佳时间（秒），0表示无记录</returns>
        public int GetLevelBestTime(int levelId)
        {
            string key = GetLevelStatsKey(levelId);
            
            if (PlayerPrefs.HasKey(key))
            {
                string stats = PlayerPrefs.GetString(key);
                string[] parts = stats.Split('/');
                
                if (parts.Length >= 1)
                {
                    return int.Parse(parts[0]);
                }
            }
            
            return 0;
        }

        /// <summary>
        /// 获取关卡最佳步数
        /// </summary>
        /// <param name="levelId">关卡ID</param>
        /// <returns>最佳步数，0表示无记录</returns>
        public int GetLevelBestMoves(int levelId)
        {
            string key = GetLevelStatsKey(levelId);
            
            if (PlayerPrefs.HasKey(key))
            {
                string stats = PlayerPrefs.GetString(key);
                string[] parts = stats.Split('/');
                
                if (parts.Length >= 2)
                {
                    return int.Parse(parts[1]);
                }
            }
            
            return 0;
        }

        /// <summary>
        /// 获取关卡通关次数
        /// </summary>
        /// <param name="levelId">关卡ID</param>
        /// <returns>通关次数</returns>
        public int GetLevelClearCount(int levelId)
        {
            string key = GetLevelStatsKey(levelId);
            
            if (PlayerPrefs.HasKey(key))
            {
                string stats = PlayerPrefs.GetString(key);
                string[] parts = stats.Split('/');
                
                if (parts.Length >= 3)
                {
                    return int.Parse(parts[2]);
                }
            }
            
            return 0;
        }

        /// <summary>
        /// 检查关卡是否有记录
        /// </summary>
        /// <param name="levelId">关卡ID</param>
        /// <returns>是否有记录</returns>
        public bool HasLevelStats(int levelId)
        {
            return PlayerPrefs.HasKey(GetLevelStatsKey(levelId));
        }

        /// <summary>
        /// 清除关卡统计
        /// </summary>
        /// <param name="levelId">关卡ID，0表示清除所有</param>
        public void ClearLevelStats(int levelId = 0)
        {
            if (levelId > 0)
            {
                // 清除指定关卡
                PlayerPrefs.DeleteKey(GetLevelStatsKey(levelId));
                Debug.Log($"[WordSolitaireStatisticsController] 已清除关卡{levelId}统计");
            }
            else
            {
                // 清除所有关卡统计
                for (int i = 1; i <= 100; i++)
                {
                    if (PlayerPrefs.HasKey(GetLevelStatsKey(i)))
                    {
                        PlayerPrefs.DeleteKey(GetLevelStatsKey(i));
                    }
                }
                Debug.Log("[WordSolitaireStatisticsController] 已清除所有关卡统计");
            }
        }

        /// <summary>
        /// 游戏计时器
        /// </summary>
        protected override void GameTimer()
        {
            base.GameTimer();
        }

        /// <summary>
        /// 增加游戏次数
        /// </summary>
        public override void IncreasePlayedGamesAmount()
        {
            base.IncreasePlayedGamesAmount();
        }

        /// <summary>
        /// 获取关卡统计信息
        /// </summary>
        /// <param name="levelId">关卡ID</param>
        /// <returns>统计信息字符串</returns>
        public string GetLevelStatsInfo(int levelId)
        {
            int bestTime = GetLevelBestTime(levelId);
            int bestMoves = GetLevelBestMoves(levelId);
            int clearCount = GetLevelClearCount(levelId);

            string timeStr = bestTime > 0 ? $"{bestTime / 60:D2}:{bestTime % 60:D2}" : "--:--";
            string movesStr = bestMoves > 0 ? bestMoves.ToString() : "--";

            return $"最佳时间: {timeStr}, 最佳步数: {movesStr}, 通关次数: {clearCount}";
        }

        /// <summary>
        /// 获取当前关卡统计信息
        /// </summary>
        /// <returns>统计信息</returns>
        public LevelStats GetCurrentLevelStats()
        {
            return new LevelStats
            {
                LevelId = CurrentLevelId,
                BestTime = _currentLevelBestTime,
                BestMoves = _currentLevelBestMoves,
                ClearCount = _currentLevelClearCount
            };
        }
    }

    /// <summary>
    /// 关卡统计信息结构
    /// </summary>
    [Serializable]
    public struct LevelStats
    {
        /// <summary>
        /// 关卡ID
        /// </summary>
        public int LevelId;

        /// <summary>
        /// 最佳时间（秒）
        /// </summary>
        public int BestTime;

        /// <summary>
        /// 最佳步数
        /// </summary>
        public int BestMoves;

        /// <summary>
        /// 通关次数
        /// </summary>
        public int ClearCount;

        /// <summary>
        /// 格式化最佳时间
        /// </summary>
        public string FormattedBestTime => BestTime > 0 
            ? $"{BestTime / 60:D2}:{BestTime % 60:D2}" 
            : "--:--";

        /// <summary>
        /// 格式化最佳步数
        /// </summary>
        public string FormattedBestMoves => BestMoves > 0 
            ? BestMoves.ToString() 
            : "--";
    }
}
