namespace SimpleSolitaire.Controller.UI
{
    /// <summary>
    /// 弹窗业务优先级枚举。
    /// 数值越大优先级越高，高优先级弹窗可压栈覆盖低优先级弹窗。
    /// </summary>
    public enum LayerPriority
    {
        Notify   = 20,   // 轻提示（未来扩展用）
        Info     = 40,   // 纯信息弹窗：HowToPlay
        Feature  = 60,   // 功能性弹窗：广告、统计、可视化设置
        Gameplay = 80,   // 游戏流程弹窗：GameLayer、ContinueGame
        System   = 100,  // 系统级弹窗：胜利、退出确认
    }

    /// <summary>
    /// 弹窗动画类型：是否播放 Animator 进出场动画。
    /// </summary>
    public enum LayerAnimationType
    {
        /// <summary>使用 Appear/Disappear Animator 触发器（默认）。</summary>
        Standard = 0,

        /// <summary>无动画，直接 SetActive 切换（调试或极简弹窗）。</summary>
        Instant  = 1,
    }
}
