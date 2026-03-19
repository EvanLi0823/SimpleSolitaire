using UnityEngine;

namespace SimpleSolitaire.Controller.WordSolitaire
{
    /// <summary>
    /// 关卡数据管理器
    /// 负责加载关卡数据和管理关卡进度
    /// </summary>
    public class LevelDataManager : MonoBehaviour
    {
        private const string CURRENT_LEVEL_KEY = "WordSolitaire_CurrentLevel";
        private const string UNLOCKED_LEVEL_KEY = "WordSolitaire_UnlockedLevel";
        
        private LevelData _currentLevel;
        private int _currentLevelId = 1;
        private int _unlockedLevelId = 1;
        
        /// <summary>
        /// 当前关卡
        /// </summary>
        public LevelData CurrentLevel => _currentLevel;
        
        /// <summary>
        /// 当前关卡ID
        /// </summary>
        public int CurrentLevelId => _currentLevelId;
        
        /// <summary>
        /// 已解锁的最大关卡ID
        /// </summary>
        public int UnlockedLevelId => _unlockedLevelId;
        
        private void Awake()
        {
            LoadProgress();
        }
        
        /// <summary>
        /// 获取当前关卡
        /// </summary>
        public LevelData GetCurrentLevel()
        {
            if (_currentLevel == null)
            {
                LoadLevel(_currentLevelId);
            }
            return _currentLevel;
        }
        
        /// <summary>
        /// 加载指定关卡
        /// </summary>
        public void LoadLevel(int levelId)
        {
            _currentLevelId = levelId;
            string path = $"Data/WordSolitaire/Levels/Level_{levelId:D2}";
            _currentLevel = Resources.Load<LevelData>(path);
            
            if (_currentLevel == null)
            {
                Debug.LogWarning($"[LevelDataManager] 关卡数据不存在: {path}");
                // 创建默认关卡数据
                _currentLevel = CreateDefaultLevel(levelId);
            }
            
            // 发布关卡变化事件
            GameEventBus.PublishLevelChanged(levelId);
            
            // 保存当前关卡
            PlayerPrefs.SetInt(CURRENT_LEVEL_KEY, levelId);
            PlayerPrefs.Save();
        }
        
        /// <summary>
        /// 进入下一关
        /// </summary>
        public void GoToNextLevel()
        {
            int nextLevel = _currentLevelId + 1;
            LoadLevel(nextLevel);
        }
        
        /// <summary>
        /// 解锁下一关
        /// </summary>
        public void UnlockNextLevel()
        {
            int nextLevel = _currentLevelId + 1;
            if (nextLevel > _unlockedLevelId)
            {
                _unlockedLevelId = nextLevel;
                PlayerPrefs.SetInt(UNLOCKED_LEVEL_KEY, _unlockedLevelId);
                PlayerPrefs.Save();
                Debug.Log($"[LevelDataManager] 解锁关卡: {_unlockedLevelId}");
            }
        }
        
        /// <summary>
        /// 检查关卡是否已解锁
        /// </summary>
        public bool IsLevelUnlocked(int levelId)
        {
            return levelId <= _unlockedLevelId;
        }
        
        /// <summary>
        /// 加载进度
        /// </summary>
        private void LoadProgress()
        {
            _currentLevelId = PlayerPrefs.GetInt(CURRENT_LEVEL_KEY, 1);
            _unlockedLevelId = PlayerPrefs.GetInt(UNLOCKED_LEVEL_KEY, 1);
        }
        
        /// <summary>
        /// 创建默认关卡数据
        /// </summary>
        private LevelData CreateDefaultLevel(int levelId)
        {
            var level = ScriptableObject.CreateInstance<LevelData>();
            level.LevelId = levelId;
            level.CardCount = 20;
            level.MaxMoves = 999;
            level.ColumnCount = 4;
            level.SlotCount = 3;
            level.CategoryIds = new[] { "animals", "fruits", "colors" };
            level.IsTutorial = levelId == 1;
            level.IsShowResultAd = true;
            level.IsShowMatchAd = false;
            return level;
        }
        
        /// <summary>
        /// 重置所有进度
        /// </summary>
        public void ResetProgress()
        {
            _currentLevelId = 1;
            _unlockedLevelId = 1;
            PlayerPrefs.SetInt(CURRENT_LEVEL_KEY, 1);
            PlayerPrefs.SetInt(UNLOCKED_LEVEL_KEY, 1);
            PlayerPrefs.Save();
            LoadLevel(1);
        }
    }
}
