using SimpleSolitaire.Controller;
using SimpleSolitaire.Model.Config;
using SimpleSolitaire.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleSolitaire.Controller.UI
{
    /// <summary>
    /// 胜利弹窗（WinLayer）的 UILayerBase 子类。
    /// LayerKey = "WinLayer"，Priority = System(100)，InteractType = Blocking。
    ///
    /// 子节点 Text 通过名称自动查找，跨节点引用通过场景搜索绑定，无需 Inspector 拖拽。
    /// </summary>
    public class WinLayerUI : UILayerBase
    {
        // ── 子节点 UI 组件（OnBindComponents 自动查找）────────────────────────
        private Text _timeWinLabel;
        private Text _scoreWinLabel;
        private Text _stepsWinLabel;

        // ── 跨节点依赖（场景唯一实例）────────────────────────────────────────
        private CongratulationManager _congratulationManager;
        private GameManager           _gameManager;

        private static readonly string BestScoreKey = "WinBestScore";

        // ── 组件绑定 ──────────────────────────────────────────────────────────

        /// <summary>
        /// 绑定子节点 Text 及跨节点引用。
        /// 节点名称以实际 Hierarchy 为准，如有差异请更新此处字符串。
        /// </summary>
        protected override void OnBindComponents()
        {
            _timeWinLabel  = this.Find<Text>("TimeWinLabel");
            _scoreWinLabel = this.Find<Text>("ScoreWinLabel");
            _stepsWinLabel = this.Find<Text>("StepsWinLabel");

            _congratulationManager = this.FindInScene<CongratulationManager>();
            _gameManager           = this.FindInScene<GameManager>();
        }

        // ── 弹窗生命周期 ──────────────────────────────────────────────────────

        protected override void OnLayerShow()
        {
            if (_gameManager == null) return;

            _congratulationManager?.CongratulationTextFill();

            int time  = _gameManager.TimeCount;
            int score = _gameManager.ScoreCount + (time > 0 ? Public.SCORE_NUMBER / time : 0);
            int steps = _gameManager.StepCount;

            if (_timeWinLabel  != null) _timeWinLabel.text  = "YOUR TIME: "  + FormatTime(time);
            if (_scoreWinLabel != null) _scoreWinLabel.text = "YOUR SCORE: " + score;
            if (_stepsWinLabel != null) _stepsWinLabel.text = "YOUR MOVES: " + steps;

            SaveBestScore(score);
        }

        protected override void OnLayerHide() { }

        // ── 内部工具 ──────────────────────────────────────────────────────────

        private static string FormatTime(int seconds)
        {
            int sec = seconds % 60;
            int min = seconds % 3600 / 60;
            return $"{min.ToString().PadLeft(2, '0')}:{sec.ToString().PadLeft(2, '0')}";
        }

        private static void SaveBestScore(int score)
        {
            if (!PlayerPrefs.HasKey(BestScoreKey) || score > PlayerPrefs.GetInt(BestScoreKey))
                PlayerPrefs.SetInt(BestScoreKey, score);
        }
    }
}
