using SimpleSolitaire.Model.Enum;
using SimpleSolitaire.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleSolitaire.Controller.UI
{
    /// <summary>
    /// 广告弹窗（AdsLayer）的 UILayerBase 子类。
    /// LayerKey = "AdsLayer"，Priority = Feature(60)，InteractType = Blocking。
    ///
    /// 子节点 Text / GameObject 通过名称自动查找，无需 Inspector 拖拽。
    /// </summary>
    public class AdsLayerUI : UILayerBase
    {
        // ── 子节点 UI 组件（OnBindComponents 自动查找）────────────────────────
        private Text      _adsInfoText;
        private Text      _adsDidNotLoadText;
        private Text      _adsClosedTooEarlyText;
        private GameObject _watchButton;

        // ── 配置文案（可在 Inspector 中覆盖）────────────────────────────────
        public string NoAdsInfoText     = "DO YOU WANNA TO DEACTIVATE ALL ADS FOR THIS GAME SESSION? JUST WATCH LAST REWARD VIDEO AND INSTALL APP. THEN ADS WON'T DISTURB YOU AGAIN!";
        public string GetUndoAdsInfoText = "DO YOU WANNA TO GET FREE UNDO COUNTS? JUST WATCH REWARD VIDEO AND INSTALL APP. THEN UNDO WILL ADDED TO YOUR GAME SESSION!";

        private RewardAdsType _currentAdsType = RewardAdsType.None;

        // ── 组件绑定 ──────────────────────────────────────────────────────────

        /// <summary>
        /// 绑定广告弹窗内的子节点组件。节点名称以实际 Hierarchy 为准。
        /// </summary>
        protected override void OnBindComponents()
        {
            _adsInfoText           = this.Find<Text>("AdsInfoText");
            _adsDidNotLoadText     = this.Find<Text>("AdsDidNotLoadText");
            _adsClosedTooEarlyText = this.Find<Text>("AdsClosedTooEarlyText");

            // WatchButton 是 GameObject，通过 Transform 查找后取 .gameObject
            Transform watchBtnTransform = this.Find<Transform>("WatchButton");
            _watchButton = watchBtnTransform != null ? watchBtnTransform.gameObject : null;
        }

        // ── 弹窗生命周期 ──────────────────────────────────────────────────────

        protected override void OnLayerShow()
        {
            SetInfoState(showInfo: true, showNotLoaded: false, showTooEarly: false, showWatchBtn: true);
            UpdateInfoText(_currentAdsType);
        }

        protected override void OnLayerHide() { }

        // ── 公开 API ──────────────────────────────────────────────────────────

        /// <summary>设置广告类型（在 UILayerManager.Show 之前调用）。</summary>
        public void SetAdsType(RewardAdsType type)
        {
            _currentAdsType = type;
        }

        /// <summary>显示广告观看结果反馈。</summary>
        public void ShowRewardResult(RewardAdsState state)
        {
            bool showNotLoaded = state == RewardAdsState.DID_NOT_LOADED;
            bool showTooEarly  = state == RewardAdsState.TOO_EARLY_CLOSE;
            SetInfoState(showInfo: false, showNotLoaded: showNotLoaded, showTooEarly: showTooEarly, showWatchBtn: false);
        }

        // ── 按钮回调 ──────────────────────────────────────────────────────────

        /// <summary>关闭广告弹窗。</summary>
        public void OnClickClose()
        {
            UILayerManager.Instance?.Hide(GameLayerMediator.AdsLayer);
        }

        // ── 内部工具 ──────────────────────────────────────────────────────────

        private void UpdateInfoText(RewardAdsType type)
        {
            if (_adsInfoText == null) return;
            _adsInfoText.text = type == RewardAdsType.NoAds ? NoAdsInfoText : GetUndoAdsInfoText;
        }

        private void SetInfoState(bool showInfo, bool showNotLoaded, bool showTooEarly, bool showWatchBtn)
        {
            if (_adsInfoText           != null) _adsInfoText.enabled           = showInfo;
            if (_adsDidNotLoadText     != null) _adsDidNotLoadText.enabled     = showNotLoaded;
            if (_adsClosedTooEarlyText != null) _adsClosedTooEarlyText.enabled = showTooEarly;
            if (_watchButton           != null) _watchButton.SetActive(showWatchBtn);
        }
    }
}
