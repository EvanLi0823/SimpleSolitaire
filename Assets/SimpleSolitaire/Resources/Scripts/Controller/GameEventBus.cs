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
    }
}
