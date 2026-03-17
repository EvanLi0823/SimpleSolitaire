using SimpleSolitaire.Controller.UI;
using SimpleSolitaire.Model.Config;
using SimpleSolitaire.Model.Enum;
using System;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SimpleSolitaire.Controller
{
    public abstract class GameManager : MonoBehaviour
    {
        [Header("Ads Components:")]
        public GameObject AdsBtn;
        [SerializeField]
        private GameObject _adsLayer;
        [SerializeField]
        private GameObject _watchButton;
        [SerializeField]
        private Text _adsInfoText;
        [SerializeField]
        private Text _adsDidNotLoadText;
        [SerializeField]
        private Text _adsClosedTooEarlyText;

        public string NoAdsInfoText = "DO YOU WANNA TO DEACTIVATE ALL ADS FOR THIS GAME SESSION? JUST WATCH LAST REWARD VIDEO AND INSTALL APP. THEN ADS WON'T DISTURB YOU AGAIN!";
        public string GetUndoAdsInfoText = "DO YOU WANNA TO GET FREE UNDO COUNTS? JUST WATCH REWARD VIDEO AND INSTALL APP. THEN UNDO WILL ADDED TO YOUR GAME SESSION!";

        [Header("Components:")]
        [SerializeField]
        protected CardLogic _cardLogic;
        [SerializeField]
        private AdsManager _adsManagerComponent;
        [SerializeField]
        private UndoPerformer _undoPerformComponent;
        [SerializeField]
        private AutoCompleteManager _autoCompleteComponent;
        [SerializeField]
        protected StatisticsController _statisticsComponent;
        [SerializeField]
        private HowToPlayManager _howToPlayComponent;
        [SerializeField]
        private OrientationManager _orientationManager;

        [Header("UI 弹窗管理器")]
        [SerializeField]
        protected UI.GameLayerMediator _layerMediator;

        [Header("Labels:")]
        [SerializeField]
        private Text _timeLabel;
        [SerializeField]
        private Text _scoreLabel;
        [SerializeField]
        private Text _stepsLabel;
        
        [Header("Settings:")]
        public bool UseLoadLastGameOption;

        public int TimeCount => _timeCount;
        public int StepCount => _stepCount;
        public int ScoreCount => _scoreCount;

        /// <summary>SettingLayerUI 读取当前音效开关状态。</summary>
        public bool SoundEnabled           => _soundEnable;
        /// <summary>SettingLayerUI 读取当前自动完成开关状态。</summary>
        public bool AutoCompleteEnabled    => _autoCompleteEnable;
        /// <summary>SettingLayerUI 读取当前高亮可拖动开关状态。</summary>
        public bool HighlightDraggable     => _cardLogic.HighlightDraggable;
        /// <summary>SettingLayerUI 读取当前左右手方向开关状态（右手为 true）。</summary>
        public bool IsRightHand            => _orientationManager.HandOrientation != HandOrientation.LEFT;
        /// <summary>SettingLayerUI 读取当前屏幕方向类型。</summary>
        public OrientationType OrientationType => _orientationManager.OrientationType;

        private readonly string _appearTrigger = "Appear";
        private readonly string _disappearTrigger = "Disappear";
        private readonly string _bestScoreKey = "WinBestScore";

        private int _timeCount;
        private int _stepCount;
        private int _scoreCount;
        private Coroutine _timeCoroutine;
        private AudioController _audioController;

        private RewardAdsType _currentAdsType = RewardAdsType.None;

        private bool _highlightDraggableEnable;
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

            _soundEnable = true;
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
            if (window == null)
            {
                return;
            }

            var anim = window.GetComponent<Animator>();

            if (anim == null)
            {
                return;
            }

            if (_audioController != null)
            {
                _audioController.Play(AudioController.AudioType.WindowOpen);
            }

            anim.SetTrigger(_appearTrigger);
        }

        /// <summary>
        /// Disappear window with animation.
        /// </summary>
        protected void DisappearWindow(GameObject window, Action onDisappear)
        {
            if (window == null)
            {
                return;
            }

            var anim = window.GetComponent<Animator>();

            if (anim == null)
            {
                return;
            }

            if (_audioController != null)
            {
                _audioController.Play(AudioController.AudioType.WindowClose);
            }

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

                // ShowContinueLayer() 已将弹窗注册到 UILayerManager 缓存，此处可安全获取并订阅事件
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

        /// <summary>玩家在 ContinueLayer 点击"继续上局"后的处理（由 ContinueLayerUI.OnContinueYes 事件驱动）。</summary>
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

        /// <summary>玩家在 ContinueLayer 点击"开始新游戏"后的处理（由 ContinueLayerUI.OnContinueNo 事件驱动）。</summary>
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
        /// <param name="offset"></param>
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
        /// Initialize first state of UI elements. And first timer state.
        /// </summary>
        private void InitMenuView(bool isLoadGame)
        {
            _timeCount = isLoadGame ? _undoPerformComponent.StatesData.Time : 0;
            SetTimeLabel(_timeCount);
            _stepCount = isLoadGame ? _undoPerformComponent.StatesData.Steps : 0;
            _stepsLabel.text = _stepCount.ToString();
            _scoreCount = isLoadGame ? _undoPerformComponent.StatesData.Score : 0;
            _scoreLabel.text = _scoreCount.ToString();
            StopGameTimer();
        }

        /// <summary>
        /// Deactivate <see cref="AdsBtn"/> button if we show <see cref="AdsManager.ShowRewardBasedVideo"/> this ads material.
        /// </summary>
        private void InitSettingBtns()
        {
            if (_adsManagerComponent.IsHasKeyNoAds())
            {
                AdsBtn.SetActive(false);
            }
        }

        /// <summary>
        /// Update <see cref="_timeLabel"/> view text.
        /// </summary>
        /// <param name="seconds"></param>
        private void SetTimeLabel(int seconds)
        {
            int sec = seconds % 60;
            int min = (seconds % 3600) / 60;
            _timeLabel.text = string.Format("{0,2}:{1,2}", min.ToString().PadLeft(2, '0'), sec.ToString().PadLeft(2, '0'));
        }

        /// <summary>
        /// Win game action.
        /// </summary>
        public void HasWinGame()
        {
            StopGameTimer();
            var score = _scoreCount + (_timeCount > 0 ? Public.SCORE_NUMBER / _timeCount : 0);

            if (_audioController != null)
            {
                _audioController.Play(AudioController.AudioType.Win);
            }

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

        /// <summary>
        /// Save to prefs best score if it need :)
        /// </summary>
        /// <param name="score">Score value</param>
        private void SetBestValuesToPrefs(int score)
        {
            if (!PlayerPrefs.HasKey(_bestScoreKey))
            {
                PlayerPrefs.SetInt(_bestScoreKey, score);
            }
            else
            {
                if (score > PlayerPrefs.GetInt(_bestScoreKey))
                {
                    PlayerPrefs.SetInt(_bestScoreKey, score);
                }
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
            _layerMediator?.ShowGameLayer(); // UILayerManager 负责 Show 动画及 CardLayer 隐藏
        }

        protected abstract void InitCardLogic();

        #region Continue Layer
        /// <summary>
        /// Click on play button in bottom setting layer.
        /// </summary>
        private void LoadGame()
        {
            InitSettingBtns();

            _cardLogic.InitCardLogic();

            _undoPerformComponent.LoadGame();

            _cardLogic.OnNewGameStart();

            InitMenuView(true);
        }

        /// <summary>
        /// Start new game.
        /// </summary>
        public void OnClickContinueNoBtn()
        {
            //Uncomment if you wanna clear last game when User click No button on Continue Layer.
            //_undoPerformComponent.DeleteLastGame();
            _cardLogic.InitCardLogic();
            _cardLogic.Shuffle(false);
            _layerMediator?.HideContinueLayer();
        }

        /// <summary>
        /// Continue last game.
        /// </summary>
        public void OnClickContinueYesBtn()
        {
            LoadGame();
            _layerMediator?.HideContinueLayer();
        }
        #endregion

        #region Exit Layer
        /// <summary>
        /// Click on Exit button.
        /// </summary>
        public void OnClickExitBtn()
        {
            _layerMediator?.ShowExitLayer();
        }

        /// <summary>
        /// Click No on exit dialog.
        /// </summary>
        public void OnClickExitNoBtn()
        {
            _layerMediator?.HideExitLayer();
        }

        /// <summary>
        /// Quit application. Exit game.
        /// </summary>
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

        /// <summary>
        /// Click on NoAds button.
        /// </summary>
        public void OnClickGetUndoAdsBtn()
        {
            _currentAdsType = RewardAdsType.GetUndo;
            ShowAdsLayer();
        }

        /// <summary>
        /// Click on NoAds button.
        /// </summary>
        public void OnClickNoAdsBtn()
        {
            _currentAdsType = RewardAdsType.NoAds;
            ShowAdsLayer();
        }

        /// <summary>
        /// Appearing ads layer with information about ads type.
        /// </summary>
        private void ShowAdsLayer()
        {
            UpdateAdsInfoText(_currentAdsType);
            _adsInfoText.enabled = true;
            _adsDidNotLoadText.enabled = false;
            _adsClosedTooEarlyText.enabled = false;
            _watchButton.SetActive(true);
            _layerMediator?.ShowAdsLayer();
        }

        /// <summary>
        /// Close <see cref="_adsLayer"/>.
        /// </summary>
        public void OnClickAdsCloseBtn()
        {
            _layerMediator?.HideAdsLayer();
        }

        /// <summary>
        /// Close <see cref="_adsLayer"/>.
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
        /// Call result of watched reward video.
        /// </summary>
        public void OnRewardActionState(RewardAdsState state, RewardAdsType type)
        {
            // 通过类型化接口获取 AdsLayerUI，避免依赖 GetTopLayer() 以及旧 Inspector 字段
            var adsLayerUI = UILayerManager.Instance?.GetLayer<AdsLayerUI>(GameLayerMediator.AdsLayer);
            if (adsLayerUI != null)
            {
                void OnAdsHidden()
                {
                    adsLayerUI.OnHideCompleted -= OnAdsHidden;
                    _layerMediator?.ShowAdsLayer();       // OnLayerShow() 先将状态重置为初始值
                    adsLayerUI.ShowRewardResult(state);   // 再覆写为实际结果状态（Show() 同步返回后执行）
                }
                adsLayerUI.OnHideCompleted += OnAdsHidden;
            }

            _layerMediator?.HideAdsLayer();
        }

        public void UpdateAdsInfoText(RewardAdsType type)
        {
            switch (type)
            {
                case RewardAdsType.NoAds:
                    _adsInfoText.text = NoAdsInfoText;
                    break;
                case RewardAdsType.GetUndo:
                    _adsInfoText.text = GetUndoAdsInfoText;
                    break;
            }
        }
        #endregion

        #region Rule Layer
        /// <summary>
        /// Click on rule button（从设置层进入 HowToPlay，关闭后自动返回设置层）。
        /// </summary>
        public void OnClickSettingLayerRuleBtn()
        {
            _layerMediator?.ShowHowToPlayLayer(returnToSetting: true);
        }

        /// <summary>
        /// Close HowToPlay layer（返回按钮）。
        /// </summary>
        public void OnClickHowToPlayBackBtn()
        {
            _layerMediator?.HideHowToPlayLayer();
        }
        #endregion

        #region Settings Layer
        /// <summary>
        /// Click on settings button.
        /// </summary>
        public void OnClickSettingBtn()
        {
            _layerMediator?.ShowSettingLayer();
        }

        /// <summary>
        /// Close setting layer.
        /// </summary>
        public void OnClickSettingLayerCloseBtn()
        {
            _layerMediator?.HideSettingLayer();
        }
        #endregion

        #region Statistics Layer
        /// <summary>
        /// Click on statistics button（先关 Setting，Statistics 进等待队列自动弹出）。
        /// </summary>
        public void OnClickStatisticBtn()
        {
            _layerMediator?.ShowStatisticsLayer();
        }

        /// <summary>
        /// Close statistics layer（触发虚方法链，子类可在关闭前执行额外逻辑）。
        /// </summary>
        public void OnClickStatisticLayerCloseBtn()
        {
            OnStatisticsLayerClosed();
        }

        /// <summary>
        /// 关闭统计层并返回设置层。子类可 override 在关闭前刷新规则开关等 UI 状态。
        /// </summary>
        protected virtual void OnStatisticsLayerClosed()
        {
            // 先订阅 OnHideCompleted，待 Statistics 动画结束（0.42s）后再显示 Setting，
            // 避免两个弹窗同时播放出入动画造成视觉重叠
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
        /// <summary>
        /// Click on random button.
        /// </summary>
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

        /// <summary>
        /// Click on replay button.
        /// </summary>
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

        /// <summary>
        /// 关闭游戏模式选择弹窗（GameLayer）。
        /// </summary>
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

        /// <summary>
        /// GameLayer 消失后的回调。UILayerManager 已自动管理 CardLayer 显隐，基类无操作。
        /// 子类可 override 处理附加面板（如 Pyramid/Tripeaks 的 _layoutsSettings）的联动逻辑。
        /// </summary>
        protected virtual void OnModalLayerDisappeared() { }
        #endregion

        /// <summary>
        /// Call action via time.
        /// </summary>
        /// <param name="action">Delegate.</param>
        /// <param name="time">Time for invoke.</param>
        /// <returns></returns>
        protected IEnumerator InvokeAction(Action action, float time)
        {
            yield return new WaitForSeconds(time);

            action?.Invoke();
        }

        /// <summary>
        /// Increase <see cref="_stepCount"/> value and start timer <see cref="GameTimer"/> if count == 1.
        /// </summary>
        public void CardMove()
        {
            _stepCount++;
            _statisticsComponent.IncreaseMovesAmount();

            _stepsLabel.text = _stepCount.ToString();
            if (_stepCount >= 1 && _timeCoroutine == null)
            {
                _timeCoroutine = StartCoroutine(GameTimer());
            }
        }

        /// <summary>
        /// Reset all view and states.
        /// </summary>
        public void RestoreInitialState()
        {
            InitMenuView(false);
        }

        /// <summary>
        /// Update score value <see cref="_scoreCount"/> and view text <see cref="_scoreLabel"/> on UI. 
        /// </summary>
        /// <param name="value">Score</param>
        public void AddScoreValue(int value)
        {
            _scoreCount += value;
            if (_scoreCount < 0)
            {
                _scoreCount = 0;
            }
            _scoreLabel.text = _scoreCount.ToString();
        }

        /// <summary>
        /// Click on sound switch button.
        /// </summary>
        public void OnClickSoundSwitch()
        {
            _soundEnable = !_soundEnable;

            if (_audioController != null)
            {
                _audioController.SetMute(!_soundEnable);
            }

            _layerMediator?.RefreshSettingLayer();
        }

        /// <summary>
        /// Click on hand orientation switch button.
        /// </summary>
        public void OnClickOrientationSwitch()
        {
            _orientationManager.SetHandOrientation(_orientationManager.HandOrientation == HandOrientation.RIGHT ? HandOrientation.LEFT : HandOrientation.RIGHT);
            _orientationManager.SetOrientation();
            _layerMediator?.RefreshSettingLayer();
        }

        /// <summary>
        /// Click on auto complete off/on switch button.
        /// </summary>
        public void OnClickAutoCompleteEnablingSwitch()
        {
            _autoCompleteEnable = !_autoCompleteEnable;
            _autoCompleteComponent.SetEnableAutoCompleteFeature(_autoCompleteEnable);
            _layerMediator?.RefreshSettingLayer();
        }

        /// <summary>
        /// Click on highlight draggable cards state turn off/on switch button.
        /// </summary>
        public void OnClickHighlightDraggableSwitch()
        {
            _cardLogic.HighlightDraggable = !_cardLogic.HighlightDraggable;
            for (int i = 0; i < _cardLogic.AllDeckArray.Length; i++)
            {
                _cardLogic.AllDeckArray[i].UpdateBackgroundColor();
            }
            _layerMediator?.RefreshSettingLayer();
        }

        /// <summary>
        /// Click on orientation state change switch button.
        /// </summary>
        public void OnClickOrientationStateSwitch()
        {
            var nextOrientationType = _orientationManager.OrientationType.Next();
            _orientationManager.SwitchOrientationType(nextOrientationType);
            _layerMediator?.RefreshSettingLayer();
        }

        /// <summary>
        /// Start game timer.
        /// </summary>
        private IEnumerator GameTimer()
        {
            while (true)
            {
                yield return new WaitForSeconds(1.0f);
                _timeCount++;
                if (_timeCount % 30 == 0)
                {
                    AddScoreValue(Public.SCORE_OVER_THIRTY_SECONDS_DECREASE);
                }
                SetTimeLabel(_timeCount);
            }
        }

        /// <summary>
        /// Stop game timer.
        /// </summary>
        private void StopGameTimer()
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
            {
                _cardLogic.SaveGameState(isTempState: true);
            }
            else
            {
                _undoPerformComponent.Undo(removeOnlyState: true);
            }
        }

        public void OnApplicationPause(bool state)
        {
            if (!_cardLogic.IsGameStarted)
            {
                Debug.LogWarning($"Game does not started.");
                return;
            }

            if (state)
            {
                _cardLogic.SaveGameState(isTempState: true);
            }
            else
            {
                _undoPerformComponent.Undo(removeOnlyState: true);
            }
        }
    }
}