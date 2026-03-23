using System;
using SimpleSolitaire.Model.Enum;

namespace SimpleSolitaire.Controller
{
    /// <summary>
    /// 轻量级静态事件总线，用于管理器之间的解耦通信。
    ///
    /// 使用规范：
    ///   - 发布者调用 Publish* 静态方法，内部已做 null 检查，无需外部判断。
    ///   - 订阅者在 OnEnable 中注册，在 OnDisable 中注销，防止内存泄漏。
    ///   - 事件参数使用基础类型（int/bool/enum），避免引入类间依赖。
    /// </summary>
    public static class GameEventBus
    {
        // ── HUD 数据事件 ──────────────────────────────────────────────────────

        /// <summary>分数变化时发布（值已截断为非负）。订阅者：GameHUDView。</summary>
        public static event Action<int> OnScoreChanged;

        /// <summary>计时器每秒递增时发布。订阅者：GameHUDView。</summary>
        public static event Action<int> OnTimeChanged;

        /// <summary>步数变化时发布。订阅者：GameHUDView。</summary>
        public static event Action<int> OnStepChanged;

        /// <summary>
        /// 初始化/恢复游戏时批量刷新 HUD（避免三次单独事件触发）。
        /// 参数：(score, time, steps)。订阅者：GameHUDView。
        /// </summary>
        public static event Action<int, int, int> OnHUDInitialized;

        // ── 设置变更事件 ──────────────────────────────────────────────────────

        /// <summary>
        /// 任意设置项（音效、方向、高亮、自动完成等）切换后发布。
        /// 订阅者：SettingLayerUI（刷新开关图片状态）。
        /// </summary>
        public static event Action OnSettingsChanged;

        // ── Word Solitaire 特有事件 ────────────────────────────────────────────

        /// <summary>金币数量变化时发布。参数：当前金币数量。订阅者：TopInfoBar。</summary>
        public static event Action<int> OnCoinsChanged;

        /// <summary>关卡变化时发布。参数：新关卡ID。订阅者：LevelManager, StatisticsController。</summary>
        public static event Action<int> OnLevelChanged;

        /// <summary>提示道具数量变化时发布。参数：剩余数量。订阅者：BottomToolBar。</summary>
        public static event Action<int> OnHintCountChanged;

        /// <summary>撤回道具数量变化时发布。参数：剩余数量。订阅者：BottomToolBar。</summary>
        public static event Action<int> OnUndoCountChanged;

        /// <summary>万能牌道具数量变化时发布。参数：剩余数量。订阅者：BottomToolBar。</summary>
        public static event Action<int> OnJokerCountChanged;

        /// <summary>卡牌成功匹配到分类槽时发布。参数：(categoryId, currentCount, targetCount)。订阅者：CategorySlotUI。</summary>
        public static event Action<int, int, int> OnCategoryMatched;

        /// <summary>牌库耗尽时发布。订阅者：GameManager。</summary>
        public static event Action OnPackEmpty;
        
        /// <summary>点击"恢复库存"按钮时发布。订阅者：GameManager。</summary>
        public static event Action OnRestorePackClicked;

        /// <summary>游戏胜利时发布。参数：(levelId, rewardCoins)。订阅者：WinLayerUI, StatisticsController。</summary>
        public static event Action<int, int> OnWordSolitaireWin;

        /// <summary>游戏失败时发布。参数：levelId。订阅者：LoseLayerUI。</summary>
        public static event Action<int> OnWordSolitaireLose;

        // ── 发布方法（统一 null 检查，避免调用方重复判断）────────────────────

        public static void PublishScoreChanged(int score)
            => OnScoreChanged?.Invoke(score);

        public static void PublishTimeChanged(int time)
            => OnTimeChanged?.Invoke(time);

        public static void PublishStepChanged(int steps)
            => OnStepChanged?.Invoke(steps);

        public static void PublishHUDInitialized(int score, int time, int steps)
            => OnHUDInitialized?.Invoke(score, time, steps);

        public static void PublishSettingsChanged()
            => OnSettingsChanged?.Invoke();

        // ── Word Solitaire 发布方法 ────────────────────────────────────────────

        public static void PublishCoinsChanged(int coins)
            => OnCoinsChanged?.Invoke(coins);

        public static void PublishLevelChanged(int levelId)
            => OnLevelChanged?.Invoke(levelId);

        public static void PublishHintCountChanged(int count)
            => OnHintCountChanged?.Invoke(count);

        public static void PublishUndoCountChanged(int count)
            => OnUndoCountChanged?.Invoke(count);

        public static void PublishJokerCountChanged(int count)
            => OnJokerCountChanged?.Invoke(count);

        public static void PublishCategoryMatched(int categoryId, int currentCount, int targetCount)
            => OnCategoryMatched?.Invoke(categoryId, currentCount, targetCount);

        public static void PublishPackEmpty()
            => OnPackEmpty?.Invoke();
        
        public static void PublishRestorePackClicked()
            => OnRestorePackClicked?.Invoke();

        public static void PublishWordSolitaireWin(int levelId, int rewardCoins)
            => OnWordSolitaireWin?.Invoke(levelId, rewardCoins);

        public static void PublishWordSolitaireLose(int levelId)
            => OnWordSolitaireLose?.Invoke(levelId);
    }
}
