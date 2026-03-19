using SimpleSolitaire.Controller.NativeBridge;
using SimpleSolitaire.Controller.NativeBridge.Enums;
using SimpleSolitaire.Model.Enum;
using System;
using System.Collections;
using UnityEngine;

namespace SimpleSolitaire.Controller
{
    [Serializable]
    public class AdsIds
    {
        public string InterId;
        public string RewardId;
        public string BannerId;
    }

    public class AdsData
    {
        public AdsIds Ids;

        public bool IsTestADS;
        public bool IsBanner;
        public bool IsIntersitial;
        public bool IsReward;
        public bool IsHandeldAction;
    }

    public class AdsManager : MonoBehaviour
    {
        public Action<bool> BannerStateAction { get; set; }
        public Action<RewardAdsState, RewardAdsType> RewardAction { get; set; }

        [SerializeField] private GameManager _gameManagerComponent;
        [SerializeField] private UndoPerformer _undoPerformerComponent;
        [SerializeField] private OrientationManager _orientationManagerComponent;

        [SerializeField] private AdsIds _androidIds;
        [SerializeField] private AdsIds _iosIds;

        [Space(5f)] [SerializeField] private bool _intersitialRepeatCall;
        [SerializeField] private int _intersitialCallsBorder = 3;
        [SerializeField] private int _firstCallIntersitialTime;
        [SerializeField] private int _repeatIntersitialTime;

        private int _intersitialCallsCounter = 0;

        [SerializeField] private bool _isTestADS;
        [SerializeField] private bool _isBanner;
        [SerializeField] private bool _isIntersitial;
        [SerializeField] private bool _isReward;
        [SerializeField] private bool _isHandeldAction;

        public readonly string NoAdsKey = "NoAds";

        private RewardAdsType _lastShowingType = RewardAdsType.None;
        private RewardVideoStatus _lastRewardVideoStatus = RewardVideoStatus.None;

        // 每次调用携带的回调，广告结束后触发并清零
        private Action _pendingOnSuccess;
        private Action _pendingOnFailed;

        // 防重入标志
        private bool _isAdPlaying = false;

        private float _bannerHeight = 0;

        public float BannerHeight => _bannerHeight;

        private void OnEnable()
        {
            NativeBridgeManager.OnVideoPlayEnd   += HandleNativeBridgeVideoEnd;
            NativeBridgeManager.OnVideoPlayFailed += HandleNativeBridgeVideoFailed;
        }

        private void OnDisable()
        {
            NativeBridgeManager.OnVideoPlayEnd   -= HandleNativeBridgeVideoEnd;
            NativeBridgeManager.OnVideoPlayFailed -= HandleNativeBridgeVideoFailed;
        }

        private void Start()
        {
            InitializeADS();
        }

        private void OnDestroy()
        {
            HideBanner();
        }

        #region Requests ADS

        /// <summary>
        /// Banner 广告。NativeBridge 模式下暂不支持，保留接口供后续扩展。
        /// </summary>
        public void ShowBanner() { }

        /// <summary>
        /// Intersitial video request. NativeBridge 模式下由原生侧自行管理预加载，无需客户端请求。
        /// </summary>
        public void RequestInterstitial() { }

        /// <summary>
        /// Reward video request. NativeBridge 模式下由原生侧自行管理预加载，无需客户端请求。
        /// </summary>
        private void RequestRewardBasedVideo() { }

        #endregion

        #region Handlers

        #endregion

        #region Show/Hide ADS

        public void TryShowIntersitialByCounter()
        {
            if (++_intersitialCallsCounter >= _intersitialCallsBorder)
            {
                _intersitialCallsCounter = 0;
                ShowInterstitial();
            }
        }

        /// <summary>
        /// 通过 NativeBridge 显示插屏广告（协程方式，与激励视频保持一致）。
        /// </summary>
        public void ShowInterstitial()
        {
            if (IsHasKeyNoAds() || !_isIntersitial) return;
            StartCoroutine(LoadInterstitialVideo());
        }

        private IEnumerator LoadInterstitialVideo()
        {
            if (_isHandeldAction) AdsHandheld.Show();

            float waited = 0f;
            while (!NativeBridgeManager.Instance.IsInitSuccess() && waited < 3f)
            {
                waited += Time.deltaTime;
                yield return null;
            }

            if (_isHandeldAction) AdsHandheld.Hide();

            if (NativeBridgeManager.Instance.IsADReady(AdType.Interstitial))
            {
                NativeBridgeManager.Instance.SendMessageToPlatform(
                    BridgeMessageType.ShowVideo, (int)AdType.Interstitial);
            }
            else
            {
                Debug.LogWarning("[AdsManager] 插屏广告尚未就绪");
            }
        }

        /// <summary>
        /// 激励视频（通过 NoAdsAction / ShowGetUndoAction 触发，此入口保留兼容性）。
        /// </summary>
        public void ShowRewardBasedVideo() { }

        /// <summary>
        /// This method hide Smart banner from bottom of screen.
        /// </summary>
        public void HideBanner()
        {
            _bannerHeight = 0;
        }

        /// <summary>
        /// Show reward video. If user watch it the ads will disappear for current game session.
        /// </summary>
        public void NoAdsAction(Action onSuccess = null, Action onFailed = null)
        {
            if (_isAdPlaying) { onFailed?.Invoke(); return; }
            _lastShowingType  = RewardAdsType.NoAds;
            _pendingOnSuccess = onSuccess;
            _pendingOnFailed  = onFailed;
            _isAdPlaying      = true;
            StartCoroutine(LoadRewardedVideo(_lastShowingType));
        }

        /// <summary>
        /// Show reward video. If user watch it the free undo tries will be add for user.
        /// </summary>
        public void ShowGetUndoAction(Action onSuccess = null, Action onFailed = null)
        {
            if (_isAdPlaying) { onFailed?.Invoke(); return; }
            _lastShowingType  = RewardAdsType.GetUndo;
            _pendingOnSuccess = onSuccess;
            _pendingOnFailed  = onFailed;
            _isAdPlaying      = true;
            StartCoroutine(LoadRewardedVideo(_lastShowingType));
        }

        private IEnumerator LoadRewardedVideo(RewardAdsType _)
        {
            _lastRewardVideoStatus = RewardVideoStatus.None;

            if (_isHandeldAction) AdsHandheld.Show();

            // 等待 NativeBridge 初始化完成（最多等 3 秒）
            float waited = 0f;
            while (!NativeBridgeManager.Instance.IsInitSuccess() && waited < 3f)
            {
                waited += Time.deltaTime;
                yield return null;
            }

            if (_isHandeldAction) AdsHandheld.Hide();

            bool isReady = NativeBridgeManager.Instance.IsADReady(AdType.RewardVideo);
            _lastRewardVideoStatus = isReady ? RewardVideoStatus.Loaded : RewardVideoStatus.FailedToLoad;

            switch (_lastRewardVideoStatus)
            {
                case RewardVideoStatus.FailedToLoad:
                    InvokeAndClearCallbacks(success: false);
                    break;
                case RewardVideoStatus.Loaded:
                    // 发起播放；结果通过 NativeBridgeManager.OnVideoPlayEnd/OnVideoPlayFailed 回调
                    NativeBridgeManager.Instance.SendMessageToPlatform(
                        BridgeMessageType.ShowVideo, (int)AdType.RewardVideo);
                    break;
            }
        }
            
        #endregion

        #region EventsHandlers

        /// <summary>
        /// NativeBridge 视频播放成功回调（原生调用 ADPlayResult 后触发）。
        /// adType：0=激励视频 1=插屏 2=AdMob，仅处理激励视频类型。
        /// </summary>
        private void HandleNativeBridgeVideoEnd(int adType)
        {
            if (_lastShowingType == RewardAdsType.None) return;
            if (adType != (int)AdType.RewardVideo) return;
            RewardAction?.Invoke(RewardAdsState.CLOSE, _lastShowingType);
            OnRewardedUser();
            InvokeAndClearCallbacks(success: true);
        }

        /// <summary>
        /// NativeBridge 视频播放失败回调（原生调用 ADPlayResult isSuccess=false 后触发）。
        /// </summary>
        private void HandleNativeBridgeVideoFailed(int adType)
        {
            if (_lastShowingType == RewardAdsType.None) return;
            if (adType != (int)AdType.RewardVideo) return;
            RewardAction?.Invoke(RewardAdsState.DID_NOT_LOADED, _lastShowingType);
            InvokeAndClearCallbacks(success: false);
        }

        /// <summary>
        /// 触发本次调用携带的回调并清零，防止重复触发。
        /// </summary>
        private void InvokeAndClearCallbacks(bool success)
        {
            Action onSuccess = _pendingOnSuccess;
            Action onFailed  = _pendingOnFailed;
            _pendingOnSuccess = null;
            _pendingOnFailed  = null;
            _lastShowingType  = RewardAdsType.None;
            _isAdPlaying      = false;

            if (success) onSuccess?.Invoke();
            else         onFailed?.Invoke();
        }

        /// <summary>
        /// Reward actions by type of reward ads.
        /// </summary>
        public void OnRewardedUser()
        {
            switch (_lastShowingType)
            {
                case RewardAdsType.NoAds:
                    PlayerPrefs.SetInt(NoAdsKey, 1);
                    HideBanner();
                    _gameManagerComponent.OnNoAdsRewardedUser();
                    RequestRewardBasedVideo();
                    break;
                case RewardAdsType.GetUndo:
                    _gameManagerComponent.OnClickAdsCloseBtn();
                    _undoPerformerComponent.UpdateUndoCounts();
                    RequestRewardBasedVideo();
                    break;
            }
        }

        #endregion

        /// <summary>
        /// Initialize all active advertisment.
        /// </summary>
        public void InitializeADS()
        {
#if UNITY_STANDALONE || UNITY_WEBGL
            return;
#endif
            if (NativeBridgeManager.Instance == null)
            {
                Debug.LogWarning("[AdsManager] NativeBridgeManager 实例不存在，广告初始化跳过");
                return;
            }

            if (_isBanner)
                ShowBanner();

            if (_isIntersitial && _intersitialRepeatCall)
                InvokeRepeating(nameof(ShowInterstitial), _firstCallIntersitialTime, _repeatIntersitialTime);
        }

        /// <summary>
        /// Check for exist in player prefs key <see cref="NoAdsKey"/>
        /// </summary>
        /// <returns></returns>
        public bool IsHasKeyNoAds()
        {
#if UNITY_STANDALONE || UNITY_WEBGL
            return true;
#endif
            return PlayerPrefs.HasKey(NoAdsKey);
        }

    }
    
    public static class AdsHandheld
    {
        public static void Show()
        {
#if UNITY_IPHONE
            Handheld.SetActivityIndicatorStyle(UnityEngine.iOS.ActivityIndicatorStyle.Gray);
#elif UNITY_ANDROID
            Handheld.SetActivityIndicatorStyle(AndroidActivityIndicatorStyle.Small);
#endif
            
#if UNITY_IPHONE || UNITY_ANDROID
            Handheld.StartActivityIndicator();
#endif
        }

        public static void Hide()
        {
#if UNITY_IPHONE || UNITY_ANDROID
            Handheld.StopActivityIndicator();
#endif
        }
    }
}