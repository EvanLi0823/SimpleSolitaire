using SimpleSolitaire.Model.Enum;
using SimpleSolitaire.Utility;
using UnityEngine.UI;

namespace SimpleSolitaire.Controller.UI
{
    /// <summary>
    /// 广告确认弹窗。
    /// 完全自管理 UI 状态，不再依赖 GameManager。
    /// WatchButton 直接调用 AdsManager，消除了与 GameManager 的间接耦合。
    /// </summary>
    public class AdsLayerUI : UILayerBase
    {
        // 文本内容保留为 public 字段，方便在 Inspector 中本地化修改
        public string NoAdsInfoText     = "DO YOU WANNA TO DEACTIVATE ALL ADS FOR THIS GAME SESSION? JUST WATCH LAST REWARD VIDEO AND INSTALL APP. THEN ADS WON'T DISTURB YOU AGAIN!";
        public string GetUndoAdsInfoText = "DO YOU WANNA TO GET FREE UNDO COUNTS? JUST WATCH REWARD VIDEO AND INSTALL APP. THEN UNDO WILL ADDED TO YOUR GAME SESSION!";

        private Text       _adsInfoText;
        private Text       _adsDidNotLoadText;
        private Text       _adsClosedTooEarlyText;
        private UnityEngine.GameObject _watchButton;

        private RewardAdsType _currentAdsType = RewardAdsType.None;

        // AdsLayerUI 直接持有 AdsManager，不再通过 GameManager 中转
        private AdsManager _adsManager;

        protected override void OnBindComponents()
        {
            _adsInfoText           = this.Find<Text>("AdsInfoText");
            _adsDidNotLoadText     = this.Find<Text>("AdsLoadedInfoText");
            _adsClosedTooEarlyText = this.Find<Text>("TooEarlyInfoText");

            // 直接查找 AdsManager，与 GameManager 解耦
            _adsManager = this.FindInScene<AdsManager>();

            var watchBtn = ComponentFinder.Find<Button>(transform, "WatchButton");
            _watchButton = watchBtn?.gameObject;
            watchBtn?.onClick.AddListener(OnWatchAdsClicked);

            ComponentFinder.Find<Button>(transform, "CloseButtonField")?.onClick.AddListener(OnClickClose);
            ComponentFinder.Find<Button>(transform, "BGBlocker")?.onClick.AddListener(OnClickClose);
        }

        protected override void OnLayerShow()
        {
            // OnLayerShow 负责初始化 UI 状态，GameManager 的 ShowAdsLayer 无需再操作这些字段
            SetInfoState(showInfo: true, showNotLoaded: false, showTooEarly: false, showWatchBtn: true);
            UpdateInfoText(_currentAdsType);
        }

        protected override void OnLayerHide() { }

        /// <summary>由 GameManager.ShowAdsLayer() 在显示弹窗前调用，传入广告类型。</summary>
        public void SetAdsType(RewardAdsType type)
        {
            _currentAdsType = type;
        }

        /// <summary>广告结果回调后，GameManager.OnRewardActionState 重新显示弹窗时调用此方法展示结果状态。</summary>
        public void ShowRewardResult(RewardAdsState state)
        {
            bool showNotLoaded = state == RewardAdsState.DID_NOT_LOADED;
            bool showTooEarly  = state == RewardAdsState.TOO_EARLY_CLOSE;
            SetInfoState(showInfo: false, showNotLoaded: showNotLoaded, showTooEarly: showTooEarly, showWatchBtn: false);
        }

        private void OnClickClose()
        {
            UILayerManager.Instance?.Hide(GameLayerMediator.AdsLayer);
        }

        /// <summary>WatchButton 点击：将成功/失败回调传入 AdsManager，结果由本层自行处理。</summary>
        private void OnWatchAdsClicked()
        {
            switch (_currentAdsType)
            {
                case RewardAdsType.GetUndo:
                    _adsManager?.ShowGetUndoAction(
                        onSuccess: OnAdSuccess,
                        onFailed:  OnAdFailed);
                    break;
                case RewardAdsType.NoAds:
                    _adsManager?.NoAdsAction(
                        onSuccess: OnAdSuccess,
                        onFailed:  OnAdFailed);
                    break;
            }
        }

        private void OnAdSuccess()
        {
            UILayerManager.Instance?.Hide(GameLayerMediator.AdsLayer);
        }

        private void OnAdFailed()
        {
            ShowRewardResult(RewardAdsState.DID_NOT_LOADED);
        }

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
            _watchButton?.SetActive(showWatchBtn);
        }
    }
}
