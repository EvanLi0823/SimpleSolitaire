using SimpleSolitaire.Model.Enum;
using SimpleSolitaire.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleSolitaire.Controller.UI
{
    public class AdsLayerUI : UILayerBase
    {
        private Text      _adsInfoText;
        private Text      _adsDidNotLoadText;
        private Text      _adsClosedTooEarlyText;
        private GameObject _watchButton;

        public string NoAdsInfoText     = "DO YOU WANNA TO DEACTIVATE ALL ADS FOR THIS GAME SESSION? JUST WATCH LAST REWARD VIDEO AND INSTALL APP. THEN ADS WON'T DISTURB YOU AGAIN!";
        public string GetUndoAdsInfoText = "DO YOU WANNA TO GET FREE UNDO COUNTS? JUST WATCH REWARD VIDEO AND INSTALL APP. THEN UNDO WILL ADDED TO YOUR GAME SESSION!";

        private RewardAdsType _currentAdsType = RewardAdsType.None;

        private GameManager _gameManager;

        protected override void OnBindComponents()
        {
            _adsInfoText           = this.Find<Text>("AdsInfoText");
            _adsDidNotLoadText     = this.Find<Text>("AdsLoadedInfoText");
            _adsClosedTooEarlyText = this.Find<Text>("TooEarlyInfoText");

            _gameManager = this.FindInScene<GameManager>();

            var watchBtn = ComponentFinder.Find<Button>(transform, "WatchButton");
            _watchButton = watchBtn?.gameObject;
            if (_gameManager != null)
                watchBtn?.onClick.AddListener(_gameManager.OnWatchAdsBtnClick);

            ComponentFinder.Find<Button>(transform, "CloseButtonField")?.onClick.AddListener(OnClickClose);
            ComponentFinder.Find<Button>(transform, "BGBlocker")?.onClick.AddListener(OnClickClose);
        }

        protected override void OnLayerShow()
        {
            SetInfoState(showInfo: true, showNotLoaded: false, showTooEarly: false, showWatchBtn: true);
            UpdateInfoText(_currentAdsType);
        }

        protected override void OnLayerHide() { }

        public void SetAdsType(RewardAdsType type)
        {
            _currentAdsType = type;
        }

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
