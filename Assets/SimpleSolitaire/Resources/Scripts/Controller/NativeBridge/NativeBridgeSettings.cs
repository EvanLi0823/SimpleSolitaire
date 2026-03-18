using UnityEngine;

namespace SimpleSolitaire.Controller.NativeBridge
{
    /// <summary>
    /// NativeBridge 配置设置（ScriptableObject）
    /// 通过 Assets > Create > Simple Solitaire > Settings > Native Bridge Settings 创建
    /// 保存至 Assets/SimpleSolitaire/Resources/Settings/NativeBridgeSettings.asset
    /// </summary>
    [CreateAssetMenu(fileName = "NativeBridgeSettings",
                     menuName = "Simple Solitaire/Settings/Native Bridge Settings")]
    public class NativeBridgeSettings : ScriptableObject
    {
        [Header("General Settings")]
        [Tooltip("是否启用 Native Bridge 功能")]
        public bool enableNativeBridge = true;

        [Header("Android Configuration")]
        [Tooltip("Android 原生类的完整包名路径")]
        public string androidPackageName = "com.simplesolitaire.game.NativeBridge";

        [Tooltip("Android 原生方法名")]
        public string androidMethodName = "callUnity";

        [Header("iOS Configuration")]
        [Tooltip("iOS 原生方法名")]
        public string iOSMethodName = "callNative";

        [Header("Debug Settings")]
        [Tooltip("是否在控制台输出调试日志")]
        public bool enableDebugLogs = true;

        [Tooltip("是否在编辑器中模拟原生响应")]
        public bool mockResponseInEditor = true;

        [Header("Initialization")]
        [Tooltip("是否在启动时自动初始化")]
        public bool autoInitialize = true;

        [Tooltip("启动时自动请求公共参数")]
        public bool autoRequestCommonParams = true;

        [Tooltip("启动时自动请求白包状态")]
        public bool autoRequestWhiteBao = true;

        [Tooltip("启动时自动请求货币符号")]
        public bool autoRequestCurrency = true;

        [Header("Timeout Settings")]
        [Tooltip("原生调用超时时间（秒）")]
        [Range(1f, 30f)]
        public float nativeCallTimeout = 5f;

        /// <summary>验证设置有效性</summary>
        public bool ValidateSettings()
        {
            if (string.IsNullOrEmpty(androidPackageName))
            {
                Debug.LogError("[NativeBridgeSettings] Android package name is empty!");
                return false;
            }

            if (string.IsNullOrEmpty(androidMethodName))
            {
                Debug.LogError("[NativeBridgeSettings] Android method name is empty!");
                return false;
            }

            return true;
        }

        /// <summary>获取当前平台对应的方法名</summary>
        public string GetPlatformMethodName()
        {
#if UNITY_ANDROID
            return androidMethodName;
#elif UNITY_IOS
            return iOSMethodName;
#else
            return string.Empty;
#endif
        }

        public void LogDebug(string message)
        {
            if (enableDebugLogs)
                Debug.Log($"[NativeBridge] {message}");
        }

        public void LogWarning(string message)
        {
            if (enableDebugLogs)
                Debug.LogWarning($"[NativeBridge] {message}");
        }

        public void LogError(string message)
        {
            Debug.LogError($"[NativeBridge] {message}");
        }
    }
}
