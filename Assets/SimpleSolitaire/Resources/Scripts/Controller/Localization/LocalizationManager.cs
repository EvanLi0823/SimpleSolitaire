using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimpleSolitaire.Controller.Localization
{
    /// <summary>
    /// 本地化管理器。
    ///
    /// 职责：
    ///   - 启动时根据系统语言或用户保存的偏好加载对应 .txt 语言文件
    ///   - 提供 GetText(key, defaultText) 静态接口供全局查询翻译文本
    ///   - 运行时切换语言并通过 OnLanguageChanged 事件通知所有订阅者
    ///
    /// 语言文件路径约定：
    ///   Assets/SimpleSolitaire/Resources/Localization/{Language}.txt
    ///   例：Localization/English.txt、Localization/ChineseSimplified.txt
    ///
    /// 文件格式（每行一条）：
    ///   Key : Value
    ///
    /// 用法示例：
    ///   string text = LocalizationManager.GetText("btn_play", "Play");
    ///   LocalizationManager.ChangeLanguage(SystemLanguage.ChineseSimplified);
    /// </summary>
    public class LocalizationManager : MonoBehaviour
    {
        // ── 单例 ──────────────────────────────────────────────────────────────
        private static LocalizationManager _instance;

        public static LocalizationManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<LocalizationManager>();
                return _instance;
            }
        }

        // ── 状态 ──────────────────────────────────────────────────────────────
        private static Dictionary<string, string> _dic;
        private static SystemLanguage _currentLanguage;
        private static bool _useSystemLanguage = true;
        private static SystemLanguage _overrideLanguage = SystemLanguage.English;

        // PlayerPrefs 键
        private const string LANGUAGE_PREF_KEY      = "GameLanguage";
        private const string USE_SYSTEM_LANGUAGE_KEY = "UseSystemLanguage";

        // ── 事件 ──────────────────────────────────────────────────────────────
        /// <summary>语言切换完成时触发，订阅者负责刷新自身文本显示。</summary>
        public static event Action OnLanguageChanged;

        // ── Unity 生命周期 ────────────────────────────────────────────────────

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeLocalization();
        }

        // ── 初始化 ────────────────────────────────────────────────────────────

        /// <summary>初始化本地化系统，读取偏好并加载对应语言。可从外部静态调用（如 LanguageSelectionGame）。</summary>
        public static void InitializeLocalization()
        {
            LoadLanguageSettings();
            LoadLanguage(GetTargetLanguage());
        }

        private static void LoadLanguageSettings()
        {
            _useSystemLanguage = PlayerPrefs.GetInt(USE_SYSTEM_LANGUAGE_KEY, 1) == 1;

            if (PlayerPrefs.HasKey(LANGUAGE_PREF_KEY))
            {
                string saved = PlayerPrefs.GetString(LANGUAGE_PREF_KEY, "English");
                if (Enum.TryParse<SystemLanguage>(saved, out SystemLanguage lang))
                    _overrideLanguage = lang;
            }
        }

        private static void SaveLanguageSettings()
        {
            PlayerPrefs.SetInt(USE_SYSTEM_LANGUAGE_KEY, _useSystemLanguage ? 1 : 0);
            PlayerPrefs.SetString(LANGUAGE_PREF_KEY, _overrideLanguage.ToString());
            PlayerPrefs.Save();
        }

        private static SystemLanguage GetTargetLanguage()
        {
            return _useSystemLanguage ? Application.systemLanguage : _overrideLanguage;
        }

        // ── 语言加载 ──────────────────────────────────────────────────────────

        /// <summary>加载指定语言的文本文件并填充翻译字典，加载完成后触发 OnLanguageChanged 事件。</summary>
        public static void LoadLanguage(SystemLanguage language)
        {
            _currentLanguage = language;

            TextAsset txt = Resources.Load<TextAsset>($"Localization/{language}");
            if (txt == null)
            {
                Debug.LogWarning($"[LocalizationManager] 未找到语言文件 Localization/{language}，回退到 English。");
                txt = Resources.Load<TextAsset>("Localization/English");
                _currentLanguage = SystemLanguage.English;
            }

            _dic = new Dictionary<string, string>();
            if (txt != null)
            {
                string[] lines = txt.text.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in lines)
                {
                    string[] parts = line.Split(new[] { ':' }, 2);
                    if (parts.Length == 2)
                        _dic[parts[0].Trim()] = parts[1].Trim();
                }
            }

            OnLanguageChanged?.Invoke();
        }

        // ── 公共 API ──────────────────────────────────────────────────────────

        /// <summary>
        /// 按 key 查询当前语言的翻译文本，key 不存在时返回 defaultText。
        /// 字典未初始化时自动按当前语言加载。
        /// </summary>
        public static string GetText(string key, string defaultText = "")
        {
            if (_dic == null || _dic.Count == 0)
            {
                if (_currentLanguage == SystemLanguage.Unknown)
                    _currentLanguage = GetTargetLanguage();
                LoadLanguage(_currentLanguage);
            }

            if (_dic.TryGetValue(key, out string value) && !string.IsNullOrEmpty(value))
                return value;

            return defaultText;
        }

        /// <summary>运行时切换语言，保存偏好并通知所有订阅者。</summary>
        public static void ChangeLanguage(SystemLanguage language)
        {
            _useSystemLanguage = false;
            _overrideLanguage  = language;
            SaveLanguageSettings();
            _dic = null;
            LoadLanguage(language);
        }

        /// <summary>设置是否跟随系统语言；切换后若目标语言发生变化则立即重新加载。</summary>
        public static void SetUseSystemLanguage(bool useSystemLanguage)
        {
            _useSystemLanguage = useSystemLanguage;
            SaveLanguageSettings();
            SystemLanguage target = GetTargetLanguage();
            if (_currentLanguage != target)
                LoadLanguage(target);
        }

        /// <summary>
        /// 刷新场景中所有 LocalizedText 和 LocalizedTextMeshProUGUI 组件的显示文本。
        /// 通常在语言切换后调用。
        /// </summary>
        public static void RefreshAllLocalizedTexts()
        {
            var tmpTexts = FindObjectsOfType<LocalizedTextMeshProUGUI>();
            foreach (var t in tmpTexts)
            {
                if (t != null && t.gameObject.activeInHierarchy)
                    t.UpdateText();
            }

            var legacyTexts = FindObjectsOfType<LocalizedText>();
            foreach (var t in legacyTexts)
            {
                if (t != null && t.gameObject.activeInHierarchy)
                    t.UpdateText();
            }
        }

        /// <summary>返回当前生效的语言。</summary>
        public static SystemLanguage GetCurrentLanguage() => _currentLanguage;

        /// <summary>是否跟随系统语言。</summary>
        public static bool IsUsingSystemLanguage() => _useSystemLanguage;

        /// <summary>检测指定语言是否存在对应的语言文件。</summary>
        public static bool IsLanguageSupported(SystemLanguage language)
            => Resources.Load<TextAsset>($"Localization/{language}") != null;

        /// <summary>
        /// 扫描已知语言列表，返回实际有文件的语言数组。
        /// English 排在首位，其余按字母序排列。
        /// </summary>
        public static SystemLanguage[] GetSupportedLanguages()
        {
            var mapping = new Dictionary<string, SystemLanguage>
            {
                { "English",            SystemLanguage.English },
                { "ChineseSimplified",  SystemLanguage.ChineseSimplified },
                { "ChineseTraditional", SystemLanguage.ChineseTraditional },
                { "French",             SystemLanguage.French },
                { "German",             SystemLanguage.German },
                { "Spanish",            SystemLanguage.Spanish },
                { "Japanese",           SystemLanguage.Japanese },
                { "Korean",             SystemLanguage.Korean },
                { "Russian",            SystemLanguage.Russian },
                { "Portuguese",         SystemLanguage.Portuguese },
                { "Italian",            SystemLanguage.Italian },
                { "Dutch",              SystemLanguage.Dutch },
                { "Polish",             SystemLanguage.Polish },
                { "Turkish",            SystemLanguage.Turkish },
                { "Arabic",             SystemLanguage.Arabic },
                { "Thai",               SystemLanguage.Thai },
                { "Vietnamese",         SystemLanguage.Vietnamese },
                { "Indonesian",         SystemLanguage.Indonesian },
                { "Czech",              SystemLanguage.Czech },
                { "Romanian",           SystemLanguage.Romanian },
            };

            var supported = new List<SystemLanguage>();
            foreach (var kvp in mapping)
            {
                if (Resources.Load<TextAsset>($"Localization/{kvp.Key}") != null)
                    supported.Add(kvp.Value);
            }

            var result = supported.Where(l => l == SystemLanguage.English).ToList();
            result.AddRange(supported.Where(l => l != SystemLanguage.English).OrderBy(l => l.ToString()));
            return result.ToArray();
        }

        // ── Inspector 辅助 ────────────────────────────────────────────────────

        [ContextMenu("强制刷新当前语言")]
        private void ForceRefresh()
        {
            LoadLanguage(_currentLanguage);
        }
    }
}
