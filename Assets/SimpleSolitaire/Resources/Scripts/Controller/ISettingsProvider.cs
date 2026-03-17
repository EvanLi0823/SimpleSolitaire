using SimpleSolitaire.Model.Enum;

namespace SimpleSolitaire.Controller
{
    /// <summary>
    /// 游戏设置读写接口。
    ///
    /// 作用：SettingLayerUI 通过此接口与 GameManager 交互，
    /// 而不依赖 GameManager 的具体类型，便于测试和多游戏模式扩展。
    ///
    /// 实现者：GameManager（所有游戏模式共用同一实现）。
    /// </summary>
    public interface ISettingsProvider
    {
        // ── 状态读取 ──────────────────────────────────────────────────────────
        bool SoundEnabled         { get; }
        bool AutoCompleteEnabled  { get; }
        bool HighlightDraggable   { get; }
        bool IsRightHand          { get; }
        OrientationType OrientationType { get; }

        // ── 设置切换 ──────────────────────────────────────────────────────────
        void ToggleSound();
        void ToggleHandOrientation();
        void ToggleHighlightDraggable();
        void ToggleAutoComplete();
        void CycleOrientationType();
    }
}
