using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Newtonsoft.Json;
using SimpleSolitaire.Controller.NativeBridge.Enums;
using SimpleSolitaire.Controller.NativeBridge.Models;

namespace SimpleSolitaire.Controller.NativeBridge
{
    /// <summary>
    /// Unity 与原生平台交互的桥接管理器（Solitaire 版本）
    /// 实现 Unity C# 代码与 Android/iOS 原生代码的双向通信。
    ///
    /// 使用方式：
    ///   - 挂载至场景中的 GameObject，或由 SingletonBehaviour 自动创建
    ///   - 在 Resources/Settings/ 下创建 NativeBridgeSettings.asset 配置文件
    ///   - 订阅 OnVideoPlayEnd、OnCommonParamReceived 等事件以响应原生回调
    /// </summary>
    public class NativeBridgeManager : SingletonBehaviour<NativeBridgeManager>
    {
        #region iOS Native Interface
#if UNITY_IOS && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string callNative(string msg);
#endif
        #endregion

        #region Private Fields

        private NativeBridgeSettings _settings;

#if UNITY_ANDROID && !UNITY_EDITOR
        private AndroidJavaObject _androidJavaObject;
#endif

        // 初始化状态标志
        private bool _commonParamReturn    = false;
        private bool _isUnifyCurrencyRef   = false;
        private bool _h5InitResult         = false;
        private bool _isWhitePackage       = false;
        private int  _h5UserType           = 0;
        private int  _notifyReward         = 0;

        // 缓存数据
        private CommonParamResponse _commonParam;
        private string _currencySymbol = "$";

        // 消息处理器字典
        private Dictionary<BridgeMessageType, Action<Dictionary<string, object>>> _messageHandlers;

        // 消息类型 → 原生方法名映射
        private readonly Dictionary<BridgeMessageType, string> _methodNameMap =
            new Dictionary<BridgeMessageType, string>
            {
                { BridgeMessageType.CommonParam,      "getCommonParm"      },
                { BridgeMessageType.PrivacyPolicy,    "PrivacyPolicy"      },
                { BridgeMessageType.TermsOfUse,       "TermsofUse"         },
                { BridgeMessageType.ShowWithdraw,     "showWithdraw"       },
                { BridgeMessageType.BuryPoint,        "buryPoint"          },
                { BridgeMessageType.ShowVideo,        "showVideo"          },
                { BridgeMessageType.RequestIsWhiteBao,"requestIsWhiteBao"  },
                { BridgeMessageType.GetUnifyCurrency, "getUnifyCurrency"   },
                { BridgeMessageType.FeedBack,         "feedback"           },
                { BridgeMessageType.ShowPromotion,    "showPromotion"      },
                { BridgeMessageType.EnterGame,        "enterGame"          },
                { BridgeMessageType.UpdateLevel,      "updateLevel"        },
                { BridgeMessageType.UserAmount,       "userAmount"         },
                { BridgeMessageType.IsInterADReady,   "isInterReady"       },
                { BridgeMessageType.IsRewardADReady,  "isRewardReady"      },
                { BridgeMessageType.IsAdMobADReady,   "isAdMobReady"       },
                { BridgeMessageType.ShowWithdrawGuide,"showWithdrawGuide"  },
                { BridgeMessageType.IsWithdrawReward, "isWithdrawReward"   }
            };

        #endregion

        #region Public Events

        /// <summary>视频广告播放成功结束事件（参数：adType 整型值）</summary>
        public static event Action<int> OnVideoPlayEnd;

        /// <summary>视频广告播放失败事件（参数：adType 整型值）</summary>
        public static event Action<int> OnVideoPlayFailed;

        /// <summary>H5 初始化成功事件（参数：是否成功）</summary>
        public static event Action<bool> OnH5InitSuccess;

        /// <summary>H5 退出事件</summary>
        public static event Action OnH5Exit;

        /// <summary>公共参数接收事件</summary>
        public static event Action<CommonParamResponse> OnCommonParamReceived;

        /// <summary>货币符号接收事件</summary>
        public static event Action<string> OnCurrencySymbolReceived;

        /// <summary>用户完成提现事件（参数：扣除金额）</summary>
        public static event Action<int> OnWithdrawCompleted;

        #endregion

        #region Lifecycle

        public override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            OnInit();
        }

        /// <summary>加载 NativeBridgeSettings 配置</summary>
        private void LoadSettings()
        {
            _settings = Resources.Load<NativeBridgeSettings>("Settings/NativeBridgeSettings");
            if (_settings == null)
            {
                Debug.LogError("[NativeBridge] 未找到 NativeBridgeSettings，" +
                    "请通过 Assets > Create > Simple Solitaire > Settings > Native Bridge Settings 创建，" +
                    "并保存至 Resources/Settings/ 目录。");
            }
        }

        public void OnInit()
        {
            Debug.Log("[NativeBridge] 开始初始化...");

            LoadSettings();

            if (_settings != null && _settings.enableNativeBridge)
            {
                InitializeHandlers();
                if (_settings.autoInitialize)
                    Init();
                else
                    Debug.Log("[NativeBridge] 自动初始化已禁用，请手动调用 Init()。");
            }
            else
            {
                Debug.LogWarning("[NativeBridge] Native Bridge 已禁用或配置文件缺失");
            }
        }

        /// <summary>初始化平台连接及自动调用接口</summary>
        private void Init()
        {
            if (_settings == null || !_settings.ValidateSettings())
            {
                Debug.LogError("[NativeBridge] 配置无效，初始化中止");
                return;
            }

            _settings.LogDebug("初始化中...");

#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                _androidJavaObject = new AndroidJavaObject(_settings.androidPackageName);
                Debug.Log($"[NativeBridge] ✓ Android 初始化成功（包名: {_settings.androidPackageName}）");
            }
            catch (Exception e)
            {
                _settings.LogError($"Android 初始化失败: {e.Message}");
                return;
            }
#elif UNITY_IOS && !UNITY_EDITOR
            Debug.Log($"[NativeBridge] ✓ iOS 初始化成功（方法名: {_settings.iOSMethodName}）");
#endif

#if !UNITY_EDITOR
            if (_settings.autoRequestCommonParams)
                SendMessageToPlatform(BridgeMessageType.CommonParam);
            if (_settings.autoRequestWhiteBao)
                SendMessageToPlatform(BridgeMessageType.RequestIsWhiteBao);
            if (_settings.autoRequestCurrency)
                SendMessageToPlatform(BridgeMessageType.GetUnifyCurrency);
#else
            if (_settings.mockResponseInEditor)
            {
                _commonParamReturn  = true;
                _isUnifyCurrencyRef = true;
                Debug.Log("[NativeBridge] ✓ 编辑器模式初始化成功（Mock 已启用）");
            }
            else
            {
                Debug.Log("[NativeBridge] ✓ 编辑器模式初始化成功（Mock 已禁用）");
            }
#endif

            Debug.Log($"[NativeBridge] ======================================");
            Debug.Log($"[NativeBridge] 初始化完成 | 平台: {GetCurrentPlatformName()}");
            Debug.Log($"[NativeBridge] ======================================");
        }

        private string GetCurrentPlatformName()
        {
#if UNITY_EDITOR
            return "Unity Editor";
#elif UNITY_ANDROID
            return "Android";
#elif UNITY_IOS
            return "iOS";
#else
            return "Unknown";
#endif
        }

        /// <summary>注册所有消息处理器</summary>
        private void InitializeHandlers()
        {
            _messageHandlers = new Dictionary<BridgeMessageType, Action<Dictionary<string, object>>>
            {
                { BridgeMessageType.CommonParam,       HandleCommonParam       },
                { BridgeMessageType.PrivacyPolicy,     HandlePrivacyPolicy     },
                { BridgeMessageType.TermsOfUse,        HandleTermsOfUse        },
                { BridgeMessageType.ShowWithdraw,      HandleShowWithdraw      },
                { BridgeMessageType.BuryPoint,         HandleBuryPoint         },
                { BridgeMessageType.ShowVideo,         HandleShowVideo         },
                { BridgeMessageType.RequestIsWhiteBao, HandleRequestIsWhiteBao },
                { BridgeMessageType.GetUnifyCurrency,  HandleGetUnifyCurrency  },
                { BridgeMessageType.FeedBack,          HandleFeedBack          },
                { BridgeMessageType.ShowPromotion,     HandleShowPromotion     },
                { BridgeMessageType.EnterGame,         HandleEnterGame         },
                { BridgeMessageType.UpdateLevel,       HandleUpdateLevel       },
                { BridgeMessageType.UserAmount,        HandleUserAmount        },
                { BridgeMessageType.IsRewardADReady,   HandleAdReadyQuery      },
                { BridgeMessageType.IsInterADReady,    HandleAdReadyQuery      },
                { BridgeMessageType.IsAdMobADReady,    HandleAdReadyQuery      }
            };
        }

        private void OnDestroy()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (_androidJavaObject != null)
            {
                _androidJavaObject.Dispose();
                _androidJavaObject = null;
            }
#endif
        }

        #endregion

        #region Public API - Unity 主动调用原生

        /// <summary>向原生平台发送消息（异步，无需返回值）</summary>
        public void SendMessageToPlatform(BridgeMessageType type,
                                          object param1 = null,
                                          object param2 = null)
        {
            if (_settings == null || !_settings.enableNativeBridge)
            {
                Debug.LogWarning("[NativeBridge] Native Bridge 未启用或配置缺失");
                return;
            }

            if (type == BridgeMessageType.None)
            {
                _settings.LogWarning("不能发送 None 类型消息");
                return;
            }

            if (!_methodNameMap.TryGetValue(type, out string methodName))
            {
                _settings.LogError($"找不到 {type} 对应的方法名映射");
                return;
            }

            var message = new BridgeMessage(methodName, param1, param2);
            string jsonData = JsonConvert.SerializeObject(message);

            _settings.LogDebug($"SendMessageToPlatform: {methodName} | data: {jsonData}");

            string result = GetMessageFromPlatform(jsonData);
            if (!string.IsNullOrEmpty(result))
                DecodeMessage(result, type);
        }

        /// <summary>向原生平台发送消息并同步获取结果</summary>
        public Dictionary<string, object> SendMessageAndGetResult(BridgeMessageType type,
                                                                   object param1 = null,
                                                                   object param2 = null)
        {
            if (_settings == null || !_settings.enableNativeBridge) return null;
            if (type == BridgeMessageType.None) return null;

            if (!_methodNameMap.TryGetValue(type, out string methodName))
            {
                _settings.LogError($"找不到 {type} 对应的方法名映射");
                return null;
            }

            var message = new BridgeMessage(methodName, param1, param2);
            string jsonData = JsonConvert.SerializeObject(message);

            _settings.LogDebug($"SendMessageAndGetResult: {methodName} | data: {jsonData}");

            string result = GetMessageFromPlatform(jsonData);
            if (!string.IsNullOrEmpty(result))
            {
                try
                {
                    return JsonConvert.DeserializeObject<Dictionary<string, object>>(result);
                }
                catch (Exception e)
                {
                    _settings.LogError($"解析结果失败: {e.Message}");
                }
            }
            return null;
        }

        /// <summary>调用原生方法并返回字符串结果</summary>
        private string GetMessageFromPlatform(string jsonData)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (_androidJavaObject != null)
            {
                try { return _androidJavaObject.Call<string>(_settings.androidMethodName, jsonData); }
                catch (Exception e) { _settings.LogError($"Android 调用失败: {e.Message}"); return null; }
            }
#elif UNITY_IOS && !UNITY_EDITOR
            try { return callNative(jsonData); }
            catch (Exception e) { _settings.LogError($"iOS 调用失败: {e.Message}"); return null; }
#endif
            // 编辑器模式返回模拟数据
            if (_settings != null && _settings.mockResponseInEditor)
                return GetMockResponse(jsonData);
            return "{}";
        }

        /// <summary>编辑器模拟响应</summary>
        private static string GetMockResponse(string jsonData)
        {
#if UNITY_EDITOR
            var message = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonData);
            if (message != null && message.ContainsKey("m"))
            {
                switch (message["m"].ToString())
                {
                    case "getCommonParm":   return "{\"language\":\"en\",\"country\":\"us\",\"numberGK\":1}";
                    case "getUnifyCurrency":return "{\"amount\":\"$\"}";
                    case "isRewardReady":
                    case "isInterReady":
                    case "isAdMobReady":    return "{\"amount\":1}";
                    default:                return "{}";
                }
            }
#endif
            return "{}";
        }

        #endregion

        #region Public API - 原生回调 Unity

        /// <summary>H5 初始化结果回调（由原生调用）</summary>
        public void H5InitResult(string msg)
        {
            _settings?.LogDebug($"H5InitResult: {msg}");
            var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(msg);
            if (data != null && data.ContainsKey("amount"))
            {
                _h5UserType   = Convert.ToInt32(data["amount"]);
                _h5InitResult = true;
                OnH5InitSuccess?.Invoke(true);
            }
        }

        /// <summary>H5 增加现金回调（由原生调用）</summary>
        public void H5AddCash(string msg)
        {
            _settings?.LogDebug($"H5AddCash: {msg}");
            var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(msg);
            if (data != null && data.ContainsKey("amount"))
            {
                float amount = Convert.ToSingle(data["amount"]);
                // TODO: 对接 Solitaire 货币系统
                _settings?.LogDebug($"现金增加: {amount}");
            }
        }

        /// <summary>H5 状态变化（由原生调用）</summary>
        public void H5State(string msg)
        {
            _settings?.LogDebug($"H5State: {msg}");
            OnH5Exit?.Invoke();
        }

        /// <summary>设置屏幕方向（由原生调用）</summary>
        public void SetOrientation(string msg)
        {
            _settings?.LogDebug($"SetOrientation: {msg}");
            var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(msg);
            if (data != null && data.ContainsKey("amount"))
            {
                int orientation = Convert.ToInt32(data["amount"]);
                Screen.orientation = orientation == 0
                    ? UnityEngine.ScreenOrientation.LandscapeLeft
                    : UnityEngine.ScreenOrientation.Portrait;
            }
        }

        /// <summary>广告播放结果回调（由原生调用）</summary>
        public void ADPlayResult(string msg)
        {
            _settings?.LogDebug($"ADPlayResult: {msg}");
            try
            {
                var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(msg);
                if (data == null) return;

                int  adType   = data.ContainsKey("amount") ? Convert.ToInt32(data["amount"]) : 0;
                bool isSuccess = true;

                if      (data.ContainsKey("success")) isSuccess = Convert.ToBoolean(data["success"]);
                else if (data.ContainsKey("result"))  isSuccess = Convert.ToBoolean(data["result"]);
                else if (data.ContainsKey("status"))  isSuccess = Convert.ToInt32(data["status"]) == 1;

                if (isSuccess)
                {
                    OnVideoPlayEnd?.Invoke(adType);
                    _settings?.LogDebug($"广告播放成功，类型: {adType}");
                }
                else
                {
                    string errMsg = data.ContainsKey("error")   ? data["error"].ToString()   :
                                    data.ContainsKey("message") ? data["message"].ToString() : "";
                    _settings?.LogWarning($"广告播放失败，类型: {adType}，错误: {errMsg}");
                    OnVideoPlayFailed?.Invoke(adType);
                }
            }
            catch (Exception ex)
            {
                _settings?.LogError($"ADPlayResult 解析异常: {ex.Message}");
            }
        }

        /// <summary>通知奖励到达（由原生调用）</summary>
        public void NotifyReward()
        {
            _settings?.LogDebug("NotifyReward");
            _notifyReward++;
        }

        /// <summary>用户已提现回调（由原生调用）</summary>
        public void WithdrawAction(string msg)
        {
            _settings?.LogDebug($"WithdrawAction: {msg}");
            var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(msg);
            if (data == null) return;

            int amount = 0;
            if (data.ContainsKey("amount"))
                amount = Convert.ToInt32(data["amount"]);

            // 通知业务层处理提现后的扣款/积分逻辑
            OnWithdrawCompleted?.Invoke(amount);

            // TODO: 对接 Solitaire 具体的积分扣减逻辑
            // 示例：PlayerPrefs.SetInt("coins", PlayerPrefs.GetInt("coins", 0) - amount);
        }

        #endregion

        #region Public Query API

        /// <summary>检查初始化是否完成</summary>
        public bool IsInitSuccess()
        {
#if UNITY_EDITOR
            return _settings != null && _settings.mockResponseInEditor || (_commonParamReturn && _isUnifyCurrencyRef);
#else
            return _commonParamReturn && _isUnifyCurrencyRef;
#endif
        }

        /// <summary>查询指定类型广告是否就绪</summary>
        public bool IsADReady(AdType type)
        {
#if UNITY_EDITOR
            return true;
#else
            BridgeMessageType msgType = type switch
            {
                AdType.RewardVideo  => BridgeMessageType.IsRewardADReady,
                AdType.Interstitial => BridgeMessageType.IsInterADReady,
                AdType.AdMob        => BridgeMessageType.IsAdMobADReady,
                _                   => BridgeMessageType.None
            };

            if (msgType == BridgeMessageType.None) return false;

            var result = SendMessageAndGetResult(msgType);
            if (result != null && result.ContainsKey("amount"))
            {
                if (int.TryParse(result["amount"].ToString(), out int val))
                    return val == 1;
            }
            return false;
#endif
        }

        /// <summary>是否可以显示 H5 界面</summary>
        public bool CheckCanShowH5()  => _h5InitResult;

        /// <summary>获取 H5 用户类型</summary>
        public int GetH5UserType()    => _h5UserType;

        /// <summary>是否有待处理的通知奖励</summary>
        public bool HaveNotifyReward() => _notifyReward > 0;

        /// <summary>消耗一次通知奖励</summary>
        public void PushNotifyReward() { if (_notifyReward > 0) _notifyReward--; }

        /// <summary>获取公共参数缓存</summary>
        public CommonParamResponse GetCommonParam() => _commonParam;

        /// <summary>获取货币符号</summary>
        public string GetCurrencySymbol() => _currencySymbol;

        /// <summary>是否为白包</summary>
        public bool IsWhitePackage() => _isWhitePackage;

        #endregion

        #region Withdraw Interface

        /// <summary>打开提现界面（自动收集 Solitaire 游戏数据）</summary>
        public void ShowWithdrawInterface()
        {
            ShowWithdrawInterface(CollectWithdrawParams());
        }

        /// <summary>打开提现界面（使用提供的参数）</summary>
        public void ShowWithdrawInterface(WithdrawParams withdrawParams)
        {
            if (withdrawParams == null)
                withdrawParams = CollectWithdrawParams();

            string jsonParams = JsonConvert.SerializeObject(withdrawParams);
            _settings?.LogDebug($"ShowWithdraw 参数: {jsonParams}");
            SendMessageToPlatform(BridgeMessageType.ShowWithdraw, jsonParams);
        }

        /// <summary>打开提现界面（直接传入具体字段）</summary>
        public void ShowWithdrawInterface(string currentAmount, string currentCoin,
            string currentLevel, string adCount, string currentScore, string gamesWon)
        {
            ShowWithdrawInterface(new WithdrawParams
            {
                CurrentAmount = currentAmount,
                CurrentCoin   = currentCoin,
                CurrentLevel  = currentLevel,
                AdCount       = adCount,
                CurrentScore  = currentScore,
                GamesWon      = gamesWon
            });
        }

        /// <summary>从 Solitaire PlayerPrefs 收集提现参数</summary>
        private WithdrawParams CollectWithdrawParams()
        {
            var p = new WithdrawParams();

            // 货币/积分（各游戏模式的公共 PlayerPrefs key 可根据实际情况调整）
            p.CurrentAmount = PlayerPrefs.GetInt("Coins", 0).ToString();
            p.CurrentCoin   = p.CurrentAmount;

            // 历史最高分（示例 key，按实际 StatisticsController 的 key 替换）
            p.CurrentScore  = PlayerPrefs.GetInt("BestScore", 0).ToString();

            // 胜利局数
            p.GamesWon      = PlayerPrefs.GetInt("WonGames", 0).ToString();

            // 当前游戏模式关卡/进度（无关卡纸牌游戏可传0）
            p.CurrentLevel  = "0";

            // 看广告次数（TODO: 对接广告统计模块）
            p.AdCount       = "0";

            _settings?.LogDebug($"提现参数: Amount={p.CurrentAmount}, Score={p.CurrentScore}, WonGames={p.GamesWon}");
            return p;
        }

        #endregion

        #region Message Decoding

        private void DecodeMessage(string msg, BridgeMessageType requestType = BridgeMessageType.None)
        {
            if (string.IsNullOrEmpty(msg)) return;
            _settings?.LogDebug($"DecodeMessage: {msg}");

            try
            {
                var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(msg);
                if (data != null)
                    ProcessMessage(data, requestType);
            }
            catch (Exception e)
            {
                _settings?.LogError($"消息解码失败: {e.Message}");
            }
        }

        private void ProcessMessage(Dictionary<string, object> data, BridgeMessageType requestType)
        {
            if (data.ContainsKey("m"))
            {
                string methodName = data["m"].ToString();
                var type = GetMessageTypeByMethodName(methodName);
                if (type != BridgeMessageType.None && _messageHandlers.ContainsKey(type))
                    _messageHandlers[type]?.Invoke(data);
            }
            else
            {
                HandleDirectMessage(data, requestType);
            }
        }

        private BridgeMessageType GetMessageTypeByMethodName(string methodName)
        {
            foreach (var kvp in _methodNameMap)
                if (kvp.Value == methodName) return kvp.Key;
            return BridgeMessageType.None;
        }

        private void HandleDirectMessage(Dictionary<string, object> data, BridgeMessageType requestType)
        {
            if (requestType != BridgeMessageType.None && _messageHandlers.ContainsKey(requestType))
            {
                _settings?.LogDebug($"直接响应处理: {requestType}");
                _messageHandlers[requestType]?.Invoke(data);
            }
            else
            {
                _settings?.LogWarning($"收到无法路由的直接消息: {JsonConvert.SerializeObject(data)}");
            }
        }

        #endregion

        #region Message Handlers

        private void HandleCommonParam(Dictionary<string, object> data)
        {
            _commonParam = new CommonParamResponse();
            if (data.TryGetValue("language", out object lang))    _commonParam.language = lang.ToString();
            if (data.TryGetValue("country",  out object country)) _commonParam.country  = country.ToString();
            if (data.TryGetValue("numberGK", out object gk))      _commonParam.numberGK = Convert.ToInt32(gk);

            _commonParamReturn = true;
            OnCommonParamReceived?.Invoke(_commonParam);
            _settings?.LogDebug($"公共参数: lang={_commonParam.language}, country={_commonParam.country}, gk={_commonParam.numberGK}");
        }

        private void HandleGetUnifyCurrency(Dictionary<string, object> data)
        {
            if (data.TryGetValue("amount", out object amount))
            {
                _currencySymbol     = amount.ToString();
                _isUnifyCurrencyRef = true;
                OnCurrencySymbolReceived?.Invoke(_currencySymbol);
                _settings?.LogDebug($"货币符号: {_currencySymbol}");
            }
        }

        private void HandleRequestIsWhiteBao(Dictionary<string, object> data)
        {
            if (data.TryGetValue("amount", out object amount))
            {
                _isWhitePackage = amount.ToString() == "1";
                _settings?.LogDebug($"是否白包: {_isWhitePackage}");
            }
        }

        private void HandleShowVideo(Dictionary<string, object> data)
        {
            int adType = data != null && data.ContainsKey("amount")
                ? Convert.ToInt32(data["amount"]) : 0;
            OnVideoPlayEnd?.Invoke(adType);
            _settings?.LogDebug("视频播放结束");
        }

        private void HandleAdReadyQuery(Dictionary<string, object> data)
        {
            if (data != null && data.ContainsKey("amount"))
                _settings?.LogDebug($"广告就绪查询结果: {data["amount"]}");
        }

        private void HandlePrivacyPolicy(Dictionary<string, object> data)    => _settings?.LogDebug("隐私政策已处理");
        private void HandleTermsOfUse(Dictionary<string, object> data)       => _settings?.LogDebug("使用条款已处理");
        private void HandleShowWithdraw(Dictionary<string, object> data)     => _settings?.LogDebug("提现界面已处理");
        private void HandleBuryPoint(Dictionary<string, object> data)        => _settings?.LogDebug("埋点统计已处理");
        private void HandleFeedBack(Dictionary<string, object> data)         => _settings?.LogDebug("用户反馈已处理");
        private void HandleShowPromotion(Dictionary<string, object> data)    => _settings?.LogDebug("促销活动已处理");
        private void HandleEnterGame(Dictionary<string, object> data)        => _settings?.LogDebug("进入游戏已处理");
        private void HandleUpdateLevel(Dictionary<string, object> data)      => _settings?.LogDebug("更新等级已处理");
        private void HandleUserAmount(Dictionary<string, object> data)       => _settings?.LogDebug("用户金额已处理");

        #endregion
    }
}
