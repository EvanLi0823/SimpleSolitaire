using UnityEngine;
using UnityEngine.UI;
using SimpleSolitaire.Controller.NativeBridge.Enums;
using SimpleSolitaire.Controller.NativeBridge.Models;

namespace SimpleSolitaire.Controller.NativeBridge
{
    /// <summary>
    /// NativeBridgeManager 使用示例
    /// 演示如何调用各类原生接口，以及如何订阅原生回调事件。
    ///
    /// 注意：此脚本仅供开发测试使用，生产环境请移除或禁用。
    /// </summary>
    public class NativeBridgeExample : MonoBehaviour
    {
        [Header("UI - 状态显示")]
        [SerializeField] private Text _statusText;
        [SerializeField] private Text _logText;

        [Header("UI - 操作按钮")]
        [SerializeField] private Button _initButton;
        [SerializeField] private Button _showVideoButton;
        [SerializeField] private Button _checkAdButton;
        [SerializeField] private Button _getParamsButton;
        [SerializeField] private Button _trackEventButton;

        private readonly System.Collections.Generic.Queue<string> _logQueue =
            new System.Collections.Generic.Queue<string>();
        private const int MaxLogLines = 10;

        #region Unity Lifecycle

        private void OnEnable()
        {
            NativeBridgeManager.OnVideoPlayEnd        += OnVideoPlayEnd;
            NativeBridgeManager.OnH5InitSuccess       += OnH5InitSuccess;
            NativeBridgeManager.OnH5Exit              += OnH5Exit;
            NativeBridgeManager.OnCommonParamReceived  += OnCommonParamReceived;
            NativeBridgeManager.OnCurrencySymbolReceived += OnCurrencySymbolReceived;
            NativeBridgeManager.OnWithdrawCompleted   += OnWithdrawCompleted;
        }

        private void OnDisable()
        {
            NativeBridgeManager.OnVideoPlayEnd        -= OnVideoPlayEnd;
            NativeBridgeManager.OnH5InitSuccess       -= OnH5InitSuccess;
            NativeBridgeManager.OnH5Exit              -= OnH5Exit;
            NativeBridgeManager.OnCommonParamReceived  -= OnCommonParamReceived;
            NativeBridgeManager.OnCurrencySymbolReceived -= OnCurrencySymbolReceived;
            NativeBridgeManager.OnWithdrawCompleted   -= OnWithdrawCompleted;
        }

        private void Start()
        {
            if (_initButton     != null) _initButton.onClick.AddListener(OnInitButtonClicked);
            if (_showVideoButton!= null) _showVideoButton.onClick.AddListener(OnShowVideoClicked);
            if (_checkAdButton  != null) _checkAdButton.onClick.AddListener(OnCheckAdClicked);
            if (_getParamsButton!= null) _getParamsButton.onClick.AddListener(OnGetParamsClicked);
            if (_trackEventButton != null) _trackEventButton.onClick.AddListener(OnTrackEventClicked);
        }

        #endregion

        #region Button Handlers

        private void OnInitButtonClicked()
        {
            bool isInit = NativeBridgeManager.Instance.IsInitSuccess();
            UpdateStatus(isInit ? "✓ 初始化成功" : "✗ 尚未初始化");
            LogMessage($"初始化状态: {isInit}");
        }

        private void OnShowVideoClicked()
        {
            bool ready = NativeBridgeManager.Instance.IsADReady(AdType.RewardVideo);
            if (ready)
            {
                NativeBridgeManager.Instance.SendMessageToPlatform(BridgeMessageType.ShowVideo);
                LogMessage("请求播放激励视频...");
            }
            else
            {
                LogMessage("激励视频尚未就绪");
            }
        }

        private void OnCheckAdClicked()
        {
            bool rewardReady = NativeBridgeManager.Instance.IsADReady(AdType.RewardVideo);
            bool interReady  = NativeBridgeManager.Instance.IsADReady(AdType.Interstitial);
            bool admobReady  = NativeBridgeManager.Instance.IsADReady(AdType.AdMob);
            LogMessage($"广告就绪 | 激励:{rewardReady} | 插屏:{interReady} | AdMob:{admobReady}");
        }

        private void OnGetParamsClicked()
        {
            CommonParamResponse param = NativeBridgeManager.Instance.GetCommonParam();
            if (param != null)
                LogMessage($"公共参数 | 语言:{param.language} 国家:{param.country} GK:{param.numberGK}");
            else
                LogMessage("公共参数尚未加载");

            string currency = NativeBridgeManager.Instance.GetCurrencySymbol();
            LogMessage($"货币符号: {currency}");
        }

        private void OnTrackEventClicked()
        {
            NativeBridgeManager.Instance.SendMessageToPlatform(
                BridgeMessageType.BuryPoint, "click_test_button", "example");
            LogMessage("埋点事件已发送: click_test_button");
        }

        #endregion

        #region 常用功能演示

        public void ShowPrivacyPolicy()
        {
            NativeBridgeManager.Instance.SendMessageToPlatform(BridgeMessageType.PrivacyPolicy);
            LogMessage("打开隐私政策");
        }

        public void ShowTermsOfUse()
        {
            NativeBridgeManager.Instance.SendMessageToPlatform(BridgeMessageType.TermsOfUse);
            LogMessage("打开使用条款");
        }

        public void ShowFeedback()
        {
            NativeBridgeManager.Instance.SendMessageToPlatform(BridgeMessageType.FeedBack);
            LogMessage("打开用户反馈");
        }

        public void ShowWithdraw()
        {
            NativeBridgeManager.Instance.ShowWithdrawInterface();
            LogMessage("打开提现界面");
        }

        public void ShowPromotion()
        {
            NativeBridgeManager.Instance.SendMessageToPlatform(BridgeMessageType.ShowPromotion);
            LogMessage("打开促销活动");
        }

        public void UpdateGameLevel(int level)
        {
            NativeBridgeManager.Instance.SendMessageToPlatform(BridgeMessageType.UpdateLevel, level);
            LogMessage($"上报当前关卡: {level}");
        }

        public void OnEnterGame()
        {
            NativeBridgeManager.Instance.SendMessageToPlatform(BridgeMessageType.EnterGame);
            LogMessage("通知原生：进入游戏");
        }

        public void GetUserAmount()
        {
            NativeBridgeManager.Instance.SendMessageToPlatform(BridgeMessageType.UserAmount);
            LogMessage("请求用户金额");
        }

        #endregion

        #region Event Callbacks

        private void OnVideoPlayEnd(int adType)  => LogMessage($"✓ 视频广告播放完毕，类型: {adType}");
        private void OnH5InitSuccess(bool ok)   => LogMessage($"H5 初始化: {ok}");
        private void OnH5Exit()                 => LogMessage("H5 界面已退出");
        private void OnWithdrawCompleted(int amt)=> LogMessage($"提现完成，扣除金额: {amt}");

        private void OnCommonParamReceived(CommonParamResponse param) =>
            LogMessage($"✓ 公共参数到达 | lang:{param.language} country:{param.country}");

        private void OnCurrencySymbolReceived(string symbol) =>
            LogMessage($"✓ 货币符号: {symbol}");

        #endregion

        #region Helpers

        private void UpdateStatus(string msg)
        {
            if (_statusText != null) _statusText.text = msg;
        }

        private void LogMessage(string msg)
        {
            Debug.Log($"[NativeBridgeExample] {msg}");
            _logQueue.Enqueue($"[{System.DateTime.Now:HH:mm:ss}] {msg}");
            while (_logQueue.Count > MaxLogLines) _logQueue.Dequeue();
            if (_logText != null)
                _logText.text = string.Join("\n", _logQueue);
        }

        #endregion
    }
}
