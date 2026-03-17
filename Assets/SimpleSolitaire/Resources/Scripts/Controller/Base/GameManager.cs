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
        private CongratulationManager _congratManagerComponent;
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

        [Header("Layers:")]
        [SerializeField]
        protected GameObject _gameLayer;
        [SerializeField]
        protected GameObject _cardLayer;
        [SerializeField]
        [System.Obsolete("请使用 _layerMediator.ShowWinLayer()，此字段将在阶段二迁移后移除")]
        private GameObject _winLayer;
        [SerializeField]
        [System.Obsolete("请使用 _layerMediator.ShowSettingLayer()，此字段将在阶段二迁移后移除")]
        private GameObject _settingLayer;
        [SerializeField]
        [System.Obsolete("请使用 _layerMediator.ShowStatisticsLayer()，此字段将在阶段二迁移后移除")]
        private GameObject _statisticLayer;
        [SerializeField]
        [System.Obsolete("请使用 _layerMediator.ShowExitLayer()，此字段将在阶段二迁移后移除")]
        private GameObject _exitLayer;
        [SerializeField]
        [System.Obsolete("请使用 _layerMediator.ShowContinueLayer()，此字段将在阶段二迁移后移除")]
        private GameObject _continueLayer;
        [SerializeField]
        [System.Obsolete("请使用 _layerMediator.ShowHowToPlayLayer()，此字段将在阶段二迁移后移除")]
        private GameObject _howToPlayLayer;

        [Header("Labels:")]
        [SerializeField]
        private Text _timeLabel;
        [SerializeField]
        private Text _scoreLabel;
        [SerializeField]
        private Text _stepsLabel;
        [SerializeField]
        private Text _timeWinLabel;
        [SerializeField]
        private Text _scoreWinLabel;
        [SerializeField]
        private Text _stepsWinLabel;

        [Header("Switchers:")]
        [SerializeField]
        private SwitchSpriteComponent _soundSwitcher;
        [SerializeField]
        private SwitchSpriteComponent _autoCompleteSwitcher;
        [SerializeField]
        private SwitchSpriteComponent _orientationSwitcher;
        [SerializeField]
        private SwitchSpriteComponent _highlightDraggableSwitcher;
        [SerializeField]
        private TextSwitchSpriteComponent _screenOrientationSwitcher;
        
        [Header("Settings:")]
        public bool UseLoadLastGameOption;

        public int TimeCount => _timeCount;
        public int StepCount => _stepCount;
        public int ScoreCount => _scoreCount;

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
            InitOrientationStateSwitch();
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
            }
            else
            {
                _cardLogic.InitCardLogic();
                _cardLogic.Shuffle(false);
                InitMenuView(false);
            }
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
            _congratManagerComponent.CongratulationTextFill();
            var score = _scoreCount + Public.SCORE_NUMBER / _timeCount;
            _timeWinLabel.text = "YOUR TIME: " + _timeLabel.text;
            _scoreWinLabel.text = "YOUR SCORE: " + score;
            _stepsWinLabel.text = "YOUR MOVES: " + _stepCount;

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
            _cardLayer.SetActive(false);
            AppearGameLayer();
        }

        protected void AppearGameLayer()
        {
            _gameLayer.SetActive(true);
            InitCardLogic();
            AppearWindow(_gameLayer);
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
            bool infoText    = false;
            bool closedText  = state == RewardAdsState.TOO_EARLY_CLOSE;
            bool notLoadText = state == RewardAdsState.DID_NOT_LOADED;

            // 先隐藏广告层，动画完成后用结果状态重新显示
            var adsLayerBase = UI.UILayerManager.Instance?.GetTopLayer();
            if (adsLayerBase != null)
            {
                void OnAdsHidden()
                {
                    adsLayerBase.OnHideCompleted -= OnAdsHidden;
                    _adsInfoText.enabled          = infoText;
                    _adsDidNotLoadText.enabled     = notLoadText;
                    _adsClosedTooEarlyText.enabled = closedText;
                    _watchButton.SetActive(false);
                    _layerMediator?.ShowAdsLayer();
                }
                adsLayerBase.OnHideCompleted += OnAdsHidden;
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
            _layerMediator?.HideStatisticsLayer();
            _layerMediator?.ShowSettingLayer();
        }
        #endregion

        #region Game Layer
        /// <summary>
        /// Click on random button.
        /// </summary>
        public void OnClickModalRandom()
        {
            DisappearWindow(_gameLayer, OnWindowDisappeared);

            void OnWindowDisappeared()
            {
                _cardLogic.OnNewGameStart();
                _statisticsComponent.IncreasePlayedGamesAmount();
                _gameLayer.SetActive(false);
                _cardLayer.SetActive(true);
                _cardLogic.Shuffle(false);
                _undoPerformComponent.ResetUndoStates();
            }
        }

        /// <summary>
        /// Click on replay button.
        /// </summary>
        public void OnClickModalReplay()
        {
            DisappearWindow(_gameLayer, OnWindowDisappeared);

            void OnWindowDisappeared()
            {
                _cardLogic.OnNewGameStart();
                _statisticsComponent.IncreasePlayedGamesAmount();
                _gameLayer.SetActive(false);
                _cardLayer.SetActive(true);
                _cardLogic.Shuffle(true);
                _undoPerformComponent.ResetUndoStates();
            }
        }

        /// <summary>
        /// Close <see cref="_gameLayer"/>.
        /// </summary>
        public void OnClickModalClose()
        {
            DisappearWindow(_gameLayer, OnModalLayerDisappeared);
        }

        protected virtual void OnModalLayerDisappeared()
        {
            _gameLayer.SetActive(false);
            _cardLayer.SetActive(true);
        }
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
            _soundSwitcher.UpdateSwitchImg(_soundEnable);

            if (_audioController != null)
            {
                _audioController.SetMute(!_soundEnable);
            }
        }

        /// <summary>
        /// Click on hand orientation switch button.
        /// </summary>
        public void OnClickOrientationSwitch()
        {
            _orientationManager.SetHandOrientation(_orientationManager.HandOrientation == HandOrientation.RIGHT ? HandOrientation.LEFT : HandOrientation.RIGHT);
            _orientationManager.SetOrientation();
            
            _orientationSwitcher.UpdateSwitchImg(_orientationManager.HandOrientation != HandOrientation.LEFT);

        }

        /// <summary>
        /// Click on auto complete off/on switch button.
        /// </summary>
        public void OnClickAutoCompleteEnablingSwitch()
        {
            _autoCompleteEnable = !_autoCompleteEnable;
            _autoCompleteComponent.SetEnableAutoCompleteFeature(_autoCompleteEnable);
            _autoCompleteSwitcher.UpdateSwitchImg(_autoCompleteEnable);
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
            _highlightDraggableSwitcher.UpdateSwitchImg(_cardLogic.HighlightDraggable);
        }

        /// <summary>
        /// Click on orientation state change switch button.
        /// </summary>
        public void OnClickOrientationStateSwitch()
        {
            var currentOrientationType = _orientationManager.OrientationType;
            var nextOrientationType = currentOrientationType.Next();
            _orientationManager.SwitchOrientationType(nextOrientationType);
            _screenOrientationSwitcher.UpdateSwitchImg(nextOrientationType);
        }
        
        public void InitOrientationStateSwitch()
        {
            var currentOrientationType = _orientationManager.OrientationType;
            _screenOrientationSwitcher.UpdateSwitchImg(currentOrientationType);
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