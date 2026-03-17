using UnityEngine;
using UnityEngine.UI;

namespace SimpleSolitaire.Controller.UI
{
    /// <summary>
    /// 游戏 HUD 视图组件，负责更新顶部 time / score / steps 三个标签。
    ///
    /// 通过 GameEventBus 订阅数据变化，与 GameManager 完全解耦——
    /// HUD 只关心"数据是什么"，不关心"谁改变了数据"。
    ///
    /// 挂载位置：Canvas/Screen/Top 节点（或其子节点）。
    /// 在 Inspector 中拖拽三个 Text 组件即可，无需其他引用。
    /// </summary>
    public class GameHUDView : MonoBehaviour
    {
        [SerializeField] private Text _timeLabel;
        [SerializeField] private Text _scoreLabel;
        [SerializeField] private Text _stepsLabel;

        // ── Unity 生命周期 ────────────────────────────────────────────────────

        private void OnEnable()
        {
            GameEventBus.OnScoreChanged   += SetScore;
            GameEventBus.OnTimeChanged    += SetTime;
            GameEventBus.OnStepChanged    += SetSteps;
            GameEventBus.OnHUDInitialized += OnHUDInitialized;
        }

        private void OnDisable()
        {
            GameEventBus.OnScoreChanged   -= SetScore;
            GameEventBus.OnTimeChanged    -= SetTime;
            GameEventBus.OnStepChanged    -= SetSteps;
            GameEventBus.OnHUDInitialized -= OnHUDInitialized;
        }

        // ── 事件处理 ─────────────────────────────────────────────────────────

        private void SetScore(int score)
        {
            if (_scoreLabel != null) _scoreLabel.text = score.ToString();
        }

        private void SetTime(int seconds)
        {
            if (_timeLabel == null) return;
            int sec = seconds % 60;
            int min = (seconds % 3600) / 60;
            _timeLabel.text = $"{min.ToString().PadLeft(2, '0')}:{sec.ToString().PadLeft(2, '0')}";
        }

        private void SetSteps(int steps)
        {
            if (_stepsLabel != null) _stepsLabel.text = steps.ToString();
        }

        /// <summary>初始化/恢复游戏时批量刷新三个标签，避免三次独立事件的视觉闪烁。</summary>
        private void OnHUDInitialized(int score, int time, int steps)
        {
            SetScore(score);
            SetTime(time);
            SetSteps(steps);
        }
    }
}
