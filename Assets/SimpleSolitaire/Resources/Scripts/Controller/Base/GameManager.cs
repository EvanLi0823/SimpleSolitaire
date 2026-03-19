using SimpleSolitaire.Controller.UI;
using SimpleSolitaire.Model.Config;
using SimpleSolitaire.Model.Enum;
using System;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace SimpleSolitaire.Controller
{
    public abstract class GameManager : MonoBehaviour, ISettingsProvider
    {
        [Header("Ads Components:")]
        public GameObject AdsBtn;

        [Header("Components:")]
        [SerializeField]
        protected CardLogic _cardLogic;
        [SerializeField]
        protected AdsManager _adsManagerComponent;
        [SerializeField]
        protected UndoPerformer _undoPerformComponent;
        [SerializeField]
        protected AutoCompleteManager _autoCompleteComponent;
        [SerializeField]
        protected StatisticsController _statisticsComponent;
        [SerializeField]
        protected HowToPlayManager _howToPlayComponent;
        [SerializeField]
        protected OrientationManager _orientationManager;

        [Header("UI 弹窗管理器")]
        [SerializeField]
        protected UI.GameLayerMediator _layerMediator;

        [Header("Settings:")]
        public bool UseLoadLastGameOption;

        public int TimeCount  => _timeCount;
        public int StepCount  => _stepCount;
        public int ScoreCount => _scoreCount;
        public bool IsPause { get => _isPause; set => _isPause = value; }

        // ── ISettingsProvider 状态读取 ────────────────────────────────────────
        public bool SoundEnabled        => _soundEnable;
        public bool AutoCompleteEnabled => _autoCompleteEnable;
        public bool HighlightDraggable  => _cardLogic.HighlightDraggable;
        public bool IsRightHand         => _orientationManager.HandOrientation != HandOrientation.LEFT;
        public OrientationType OrientationType => _orientationManager.OrientationType;

        private readonly string _appearTrigger    = "Appear";
        private readonly string _disappearTrigger = "Disappear";
        private readonly string _bestScoreKey     = "WinBestScore";

        protected int _timeCount;
        protected int _stepCount;
        protected int _scoreCount;
        protected bool _isPause;
        private Coroutine _timeCoroutine;
        private AudioController _audioController;

        private RewardAdsType _currentAdsType = RewardAdsType.None;

        private bool _soundEnable;
        private bool _autoCompleteEnable;

        protected float _windowAnimationTime = 0.42f;

        private void Awake()
        {
            InitializeGame();
        }

        /// <summary>
        /// Initialize game structure.
        /// </summary>
        protected virtual void InitializeGame()
        {
            Application.targetFrameRate = 300;

            _undoPerformComponent.Initialize();

            _soundEnable        = true;
            _autoCompleteEnable = true;

            _adsManagerComponent.RewardAction += OnRewardActionState;
            _cardLogic.SubscribeEvents();
            _audioController = AudioController.Instance;
        }

        private void Start()
        {
            InitGameState();
        }

        /// <summary>
        /// Appear window with animation.
        /// </summary>
        protected void AppearWindow(GameObject window)
        {
            if (window == null) return;

            var anim = window.GetComponent<Animator>();
            if (anim == null) return;

            _audioController?.Play(AudioController.AudioType.WindowOpen);
            anim.SetTrigger(_appearTrigger);
        }

        /// <summary>
        /// Disappear window with animation.
        /// </summary>
        protected void DisappearWindow(GameObject window, Action onDisappear)
        {
            if (window == null) return;

            var anim = window.GetComponent<Animator>();
            if (anim == null) return;

            _audioController?.Play(AudioController.AudioType.WindowClose);
            anim.SetTrigger(_disappearTrigger);

            StartCoroutine(InvokeAction(onDisappear, _windowAnimationTime));
        }

        /// <summary>
        /// Show how to play window with animation.
        /// </summary>
        public void ShowHowToPlayLayer()
        {
            _layerMediator?.ShowHowToPlayLayer();
        }

        /// <summary>
        /// Hide how to play layer with animation.
        /// </summary>
        public void HideHowToPlayLayer()
        {
            _layerMediator?.HideHowToPlayLayer();
        }

        /// <summary>
        /// Init new game state or show continue game panel.
        /// </summary>
        private void InitGameState()
        {
            InitSettingBtns();

            if (UseLoadLastGameOption && _howToPlayComponent.IsHasKey() && _undoPerformComponent.IsHasGame())
            {
                _layerMediator?.ShowContinueLayer();

                var continueLayer = UILayerManager.Instance?.GetLayer<ContinueLayerUI>(GameLayerMediator.ContinueGameLayer);
                if (continueLayer != null)
                {
                    continueLayer.OnContinueYes += OnContinueLayerYes;
                    continueLayer.OnContinueNo  += OnContinueLayerNo;
                }
            }
            else
            {
                _cardLogic.InitCardLogic();
                _cardLogic.Shuffle(false);
                InitMenuView(false);
            }
        }

        private void OnContinueLayerYes()
        {
            var continueLayer = UILayerManager.Instance?.GetLayer<ContinueLayerUI>(GameLayerMediator.ContinueGameLayer);
            if (continueLayer != null)
            {
                continueLayer.OnContinueYes -= OnContinueLayerYes;
                continueLayer.OnContinueNo  -= OnContinueLayerNo;
            }
            LoadGame();
        }

        private void OnContinueLayerNo()
        {
            var continueLayer = UILayerManager.Instance?.GetLayer<ContinueLayerUI>(GameLayerMediator.ContinueGameLayer);
            if (continueLayer != null)
            {
                continueLayer.OnContinueYes -= OnContinueLayerYes;
                continueLayer.OnContinueNo  -= OnContinueLayerNo;
            }
            _cardLogic.InitCardLogic();
            _cardLogic.Shuffle(false);
            InitMenuView(false);
        }

        /// <summary>
        /// Change position of bottom panel. Used for ads banner.
        /// </summary>
        public void OnNoAdsRewardedUser()
        {
            OnClickAdsCloseBtn();
            AdsBtn.SetActive(false);
        }

        private void OnDestroy()
        {
            _adsManagerComponent.RewardAction -= OnRewardActionState;
            _cardLogic.UnsubscribeEvents();
        }

        /// <summary>
        /// 初始化/恢复游戏的 HUD 状态，通过 GameEventBus 批量通知 GameHUDView。
        /// </summary>
        protected void InitMenuView(bool isLoadGame)
        {
            _timeCount  = isLoadGame ? _undoPerformComponent.StatesData.Time  : 0;
            _stepCount  = isLoadGame ? _undoPerformComponent.StatesData.Steps : 0;
            _scoreCount = isLoadGame ? _undoPerformComponent.StatesData.Score : 0;
            StopGameTimer();

            // 通过 EventBus 批量更新 HUD，GameManager 无需持有 Text 组件引用
            GameEventBus.PublishHUDInitialized(_scoreCount, _timeCount, _stepCount);
        }

        /// <summary>
        /// Deactivate <see cref="AdsBtn"/> button if we show reward ads.
        /// </summary>
        private void InitSettingBtns()
        {
            if (_adsManagerComponent.IsHasKeyNoAds())
            {
                AdsBtn.SetActive(false);
            }
        }

        /// <summary>
        /// Win game action.
        /// </summary>
        public virtual void HasWinGame()
        {
            StopGameTimer();
            var score = _scoreCount + (_timeCount > 0 ? Public.SCORE_NUMBER / _timeCount : 0);

            _audioController?.Play(AudioController.AudioType.Win);

            SetBestValuesToPrefs(score);

            _layerMediator?.ShowWinLayer();

            _statisticsComponent.IncreasePlayedGamesTime(_timeCount);
            _statisticsComponent.GetAverageGameTime();
            _statisticsComponent.IncreaseScoreAmount(score);
            _statisticsComponent.IncreaseWonGamesAmount();
            _statisticsComponent.SetBestWinTime(_timeCount);
            _statisticsComponent.SetBestWinMoves(_stepCount);

            _adsManagerComponent.ShowInterstitial();
        }

        private void SetBestValuesToPrefs(int score)
        {
            if (!PlayerPrefs.HasKey(_bestScoreKey))
            {
                PlayerPrefs.SetInt(_bestScoreKey, score);
            }
            else
            {
                if (score > PlayerPrefs.GetInt(_bestScoreKey))
                    PlayerPrefs.SetInt(_bestScoreKey, score);
            }
        }

        /// <summary>
        /// Click on new game button.
        /// </summary>
        public void OnClickWinNewGame()
        {
            _layerMediator?.HideWinLayer();
            _cardLogic.Shuffle(false);
            _undoPerformComponent.ResetUndoStates();
            _statisticsComponent.IncreasePlayedGamesAmount();
        }

        /// <summary>
        /// Click on play button in bottom setting layer.
        /// </summary>
        public void OnClickPlayBtn()
        {
            AppearGameLayer();
        }

        protected void AppearGameLayer()
        {
            InitCardLogic();
            _layerMediator?.ShowGameLayer();
        }

        protected abstract void InitCardLogic();

        #region Continue Layer

        private void LoadGame()
        {
            InitSettingBtns();
            _cardLogic.InitCardLogic();
            _undoPerformComponent.LoadGame();
            _cardLogic.OnNewGameStart();
            InitMenuView(true);
        }

        public void OnClickContinueNoBtn()
        {
            _cardLogic.InitCardLogic();
            _cardLogic.Shuffle(false);
            _layerMediator?.HideContinueLayer();
        }

        public void OnClickContinueYesBtn()
        {
            LoadGame();
            _layerMediator?.HideContinueLayer();
        }

        #endregion

        #region Exit Layer

        public void OnClickExitBtn()
        {
            _layerMediator?.ShowExitLayer();
        }

        public void OnClickExitNoBtn()
        {
            _layerMediator?.HideExitLayer();
        }

        public void OnClickExitYesBtn()
        {
#if UNITY_EDITOR
            _cardLogic.SaveGameState(isTempState: true);
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        #endregion

        #region Ads Layer

        public void OnClickGetUndoAdsBtn()
        {
            _currentAdsType = RewardAdsType.GetUndo;
            ShowAdsLayer();
        }

        public void OnClickNoAdsBtn()
        {
            _currentAdsType = RewardAdsType.NoAds;
            ShowAdsLayer();
        }

        /// <summary>
        /// 显示广告弹窗：将广告类型传给 AdsLayerUI，由其自行管理 UI 状态。
        /// </summary>
        private void ShowAdsLayer()
        {
            UILayerManager.Instance?.GetLayer<AdsLayerUI>(GameLayerMediator.AdsLayer)
                ?.SetAdsType(_currentAdsType);
            _layerMediator?.ShowAdsLayer();
        }

        public void OnClickAdsCloseBtn()
        {
            _layerMediator?.HideAdsLayer();
        }

        /// <summary>
        /// Watch Ads 按钮点击（由 AdsLayerUI 的 WatchButton 绑定，保留供 Inspector 回退兼容）。
        /// </summary>
        public void OnWatchAdsBtnClick()
        {
            switch (_currentAdsType)
            {
                case RewardAdsType.GetUndo:
                    _adsManagerComponent.ShowGetUndoAction();
                    break;
                case RewardAdsType.NoAds:
                    _adsManagerComponent.NoAdsAction();
                    break;
            }
        }

        /// <summary>
        /// 广告奖励回调：隐藏当前广告层，播放关闭动画后重新弹出并展示结果状态。
        /// </summary>
        public void OnRewardActionState(RewardAdsState state, RewardAdsType type)
        {
            var adsLayerUI = UILayerManager.Instance?.GetLayer<AdsLayerUI>(GameLayerMediator.AdsLayer);
            if (adsLayerUI != null)
            {
                void OnAdsHidden()
                {
                    adsLayerUI.OnHideCompleted -= OnAdsHidden;
                    _layerMediator?.ShowAdsLayer();
                    adsLayerUI.ShowRewardResult(state);
                }
                adsLayerUI.OnHideCompleted += OnAdsHidden;
            }
            _layerMediator?.HideAdsLayer();
        }

        #endregion

        #region Rule Layer

        public void OnClickSettingLayerRuleBtn()
        {
            _layerMediator?.ShowHowToPlayLayer(returnToSetting: true);
        }

        public void OnClickHowToPlayBackBtn()
        {
            _layerMediator?.HideHowToPlayLayer();
        }

        #endregion

        #region Settings Layer

        public void OnClickSettingBtn()
        {
            _layerMediator?.ShowSettingLayer();
        }

        public void OnClickSettingLayerCloseBtn()
        {
            _layerMediator?.HideSettingLayer();
        }

        #endregion

        #region Statistics Layer

        public void OnClickStatisticBtn()
        {
            _layerMediator?.ShowStatisticsLayer();
        }

        public void OnClickStatisticLayerCloseBtn()
        {
            OnStatisticsLayerClosed();
        }

        protected virtual void OnStatisticsLayerClosed()
        {
            var statsLayer = UILayerManager.Instance?.GetLayer<UILayerBase>(GameLayerMediator.StatisticsLayer);
            if (statsLayer != null)
            {
                void OnHidden()
                {
                    statsLayer.OnHideCompleted -= OnHidden;
                    _layerMediator?.ShowSettingLayer();
                }
                statsLayer.OnHideCompleted += OnHidden;
            }
            _layerMediator?.HideStatisticsLayer();
        }

        #endregion

        #region Game Layer

        public void OnClickModalRandom()
        {
            var gameLayerUI = UILayerManager.Instance?.GetLayer<GameLayerUI>(GameLayerMediator.GameLayer);
            if (gameLayerUI != null)
            {
                void OnHidden()
                {
                    gameLayerUI.OnHideCompleted -= OnHidden;
                    _cardLogic.OnNewGameStart();
                    _statisticsComponent.IncreasePlayedGamesAmount();
                    _cardLogic.Shuffle(false);
                    _undoPerformComponent.ResetUndoStates();
                }
                gameLayerUI.OnHideCompleted += OnHidden;
            }
            _layerMediator?.HideGameLayer();
        }

        public void OnClickModalReplay()
        {
            var gameLayerUI = UILayerManager.Instance?.GetLayer<GameLayerUI>(GameLayerMediator.GameLayer);
            if (gameLayerUI != null)
            {
                void OnHidden()
                {
                    gameLayerUI.OnHideCompleted -= OnHidden;
                    _cardLogic.OnNewGameStart();
                    _statisticsComponent.IncreasePlayedGamesAmount();
                    _cardLogic.Shuffle(true);
                    _undoPerformComponent.ResetUndoStates();
                }
                gameLayerUI.OnHideCompleted += OnHidden;
            }
            _layerMediator?.HideGameLayer();
        }

        public void OnClickModalClose()
        {
            var gameLayerUI = UILayerManager.Instance?.GetLayer<GameLayerUI>(GameLayerMediator.GameLayer);
            if (gameLayerUI != null)
            {
                void OnHidden()
                {
                    gameLayerUI.OnHideCompleted -= OnHidden;
                    OnModalLayerDisappeared();
                }
                gameLayerUI.OnHideCompleted += OnHidden;
            }
            _layerMediator?.HideGameLayer();
        }

        protected virtual void OnModalLayerDisappeared() { }

        #endregion

        protected IEnumerator InvokeAction(Action action, float time)
        {
            yield return new WaitForSeconds(time);
            action?.Invoke();
        }

        /// <summary>
        /// 步数 +1，首次操作后启动计时器。
        /// </summary>
        public void CardMove()
        {
            _stepCount++;
            _statisticsComponent.IncreaseMovesAmount();
            GameEventBus.PublishStepChanged(_stepCount);

            if (_stepCount >= 1 && _timeCoroutine == null)
                _timeCoroutine = StartCoroutine(GameTimer());
        }

        /// <summary>
        /// Reset all view and states.
        /// </summary>
        public void RestoreInitialState()
        {
            InitMenuView(false);
        }

        /// <summary>
        /// 更新分数并通过 EventBus 通知 GameHUDView。
        /// </summary>
        public void AddScoreValue(int value)
        {
            _scoreCount += value;
            if (_scoreCount < 0) _scoreCount = 0;
            GameEventBus.PublishScoreChanged(_scoreCount);
        }

        // ── ISettingsProvider 设置切换实现 ───────────────────────────────────

        public void OnClickSoundSwitch()
        {
            _soundEnable = !_soundEnable;
            _audioController?.SetMute(!_soundEnable);
            GameEventBus.PublishSettingsChanged();
        }

        public void OnClickOrientationSwitch()
        {
            _orientationManager.SetHandOrientation(
                _orientationManager.HandOrientation == HandOrientation.RIGHT
                    ? HandOrientation.LEFT
                    : HandOrientation.RIGHT);
            _orientationManager.SetOrientation();
            GameEventBus.PublishSettingsChanged();
        }

        public void OnClickAutoCompleteEnablingSwitch()
        {
            _autoCompleteEnable = !_autoCompleteEnable;
            _autoCompleteComponent.SetEnableAutoCompleteFeature(_autoCompleteEnable);
            GameEventBus.PublishSettingsChanged();
        }

        public void OnClickHighlightDraggableSwitch()
        {
            _cardLogic.HighlightDraggable = !_cardLogic.HighlightDraggable;
            for (int i = 0; i < _cardLogic.AllDeckArray.Length; i++)
                _cardLogic.AllDeckArray[i].UpdateBackgroundColor();
            GameEventBus.PublishSettingsChanged();
        }

        public void OnClickOrientationStateSwitch()
        {
            var nextOrientationType = _orientationManager.OrientationType.Next();
            _orientationManager.SwitchOrientationType(nextOrientationType);
            GameEventBus.PublishSettingsChanged();
        }

        // ISettingsProvider 接口显式实现（委托给已有 public 方法）
        void ISettingsProvider.ToggleSound()               => OnClickSoundSwitch();
        void ISettingsProvider.ToggleHandOrientation()     => OnClickOrientationSwitch();
        void ISettingsProvider.ToggleHighlightDraggable()  => OnClickHighlightDraggableSwitch();
        void ISettingsProvider.ToggleAutoComplete()        => OnClickAutoCompleteEnablingSwitch();
        void ISettingsProvider.CycleOrientationType()      => OnClickOrientationStateSwitch();

        // ── 计时器 ────────────────────────────────────────────────────────────

        private IEnumerator GameTimer()
        {
            while (true)
            {
                yield return new WaitForSeconds(1.0f);
                _timeCount++;
                if (_timeCount % 30 == 0)
                    AddScoreValue(Public.SCORE_OVER_THIRTY_SECONDS_DECREASE);
                GameEventBus.PublishTimeChanged(_timeCount);
            }
        }

        protected void StopGameTimer()
        {
            if (_timeCoroutine != null)
            {
                StopCoroutine(_timeCoroutine);
                _timeCoroutine = null;
            }
        }

        public void OnApplicationFocus(bool state)
        {
            if (!_cardLogic.IsGameStarted)
            {
                Debug.LogWarning($"Game does not started.");
                return;
            }

            if (!state)
                _cardLogic.SaveGameState(isTempState: true);
            else
                _undoPerformComponent.Undo(removeOnlyState: true);
        }

        public void OnApplicationPause(bool state)
        {
            if (!_cardLogic.IsGameStarted)
            {
                Debug.LogWarning($"Game does not started.");
                return;
            }

            if (state)
                _cardLogic.SaveGameState(isTempState: true);
            else
                _undoPerformComponent.Undo(removeOnlyState: true);
        }
    }
}
