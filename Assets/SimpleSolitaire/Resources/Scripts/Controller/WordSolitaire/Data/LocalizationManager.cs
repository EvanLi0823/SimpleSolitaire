using System.Collections.Generic;
using UnityEngine;

namespace SimpleSolitaire.Controller.WordSolitaire
{
    /// <summary>
    /// 多语言管理器 - 负责加载和管理本地化文本
    /// </summary>
    public class LocalizationManager : MonoBehaviour
    {
        // ── 单例模式 ──────────────────────────────────────────────────────────
        public static LocalizationManager Instance { get; private set; }

        // ── 常量定义 ──────────────────────────────────────────────────────────
        public const string LANGUAGE_KEY = "WordSolitaire_Language";
        public const string DEFAULT_LANGUAGE = "en";

        // ── Inspector配置 ────────────────────────────────────────────────────
        [Header("数据路径配置")]
        [SerializeField] private string _localizationPath = "Data/WordSolitaire/Localization";

        [Header("默认语言")]
        [SerializeField] private SystemLanguage _defaultLanguage = SystemLanguage.English;

        // ── 内部数据结构 ──────────────────────────────────────────────────────
        /// <summary>当前语言的所有文本字典：Key = textKey, Value = localizedText</summary>
        private Dictionary<string, string> _currentTexts;

        /// <summary>当前语言代码</summary>
        private string _currentLanguageCode;

        /// <summary>当前系统语言</summary>
        private SystemLanguage _currentLanguage;

        /// <summary>是否已加载</summary>
        private bool _isLoaded;

        // ── 公开属性 ──────────────────────────────────────────────────────────
        public SystemLanguage CurrentLanguage => _currentLanguage;
        public string CurrentLanguageCode => _currentLanguageCode;

        // ── Unity生命周期 ─────────────────────────────────────────────────────
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            _currentTexts = new Dictionary<string, string>();
            _isLoaded = false;
        }

        private void Start()
        {
            // 加载保存的语言设置，或使用默认语言
            LoadSavedLanguage();
            LoadLocalization(_currentLanguage);
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        // ── 核心API ───────────────────────────────────────────────────────────

        /// <summary>
        /// 获取本地化文本
        /// </summary>
        /// <param name="key">文本Key</param>
        /// <returns>本地化文本，不存在返回Key本身</returns>
        public string Get(string key)
        {
            if (!_isLoaded)
            {
                LoadLocalization(_currentLanguage);
            }

            if (string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }

            if (_currentTexts.TryGetValue(key, out string value))
            {
                return value;
            }

            Debug.LogWarning($"[LocalizationManager] Key not found: {key}");
            return key; // fallback返回Key本身
        }

        /// <summary>
        /// 获取本地化文本（带格式化参数）
        /// </summary>
        public string GetFormat(string key, params object[] args)
        {
            string text = Get(key);
            return string.Format(text, args);
        }

        /// <summary>
        /// 切换语言
        /// </summary>
        /// <param name="language">目标语言</param>
        public void SetLanguage(SystemLanguage language)
        {
            if (_currentLanguage == language && _isLoaded)
            {
                return;
            }

            _currentLanguage = language;
            _currentLanguageCode = ConvertToLanguageCode(language);
            
            LoadLocalization(language);
            SaveLanguageSetting();

            // 发布语言变更事件
            GameEventBus.PublishSettingsChanged();
        }

        /// <summary>
        /// 切换语言（通过语言代码）
        /// </summary>
        public void SetLanguage(string languageCode)
        {
            SystemLanguage language = ConvertToSystemLanguage(languageCode);
            SetLanguage(language);
        }

        /// <summary>
        /// 检查Key是否存在
        /// </summary>
        public bool HasKey(string key)
        {
            if (!_isLoaded) return false;
            return _currentTexts.ContainsKey(key);
        }

        /// <summary>
        /// 获取所有可用的语言列表
        /// </summary>
        public List<SystemLanguage> GetAvailableLanguages()
        {
            // 返回支持的语言列表
            return new List<SystemLanguage>
            {
                SystemLanguage.English,
                SystemLanguage.ChineseSimplified,
                SystemLanguage.ChineseTraditional,
                SystemLanguage.Japanese,
                SystemLanguage.Korean,
                SystemLanguage.French,
                SystemLanguage.Spanish,
                SystemLanguage.German
            };
        }

        // ── 内部方法 ──────────────────────────────────────────────────────────

        /// <summary>
        /// 加载指定语言的本地化数据
        /// </summary>
        private void LoadLocalization(SystemLanguage language)
        {
            _currentTexts.Clear();
            _currentLanguageCode = ConvertToLanguageCode(language);

            // 尝试从Resources加载CSV文件
            TextAsset csvAsset = Resources.Load<TextAsset>($"{_localizationPath}/{_currentLanguageCode}");
            
            if (csvAsset == null)
            {
                // 回退到默认语言
                Debug.LogWarning($"[LocalizationManager] 未找到语言文件: {_currentLanguageCode}，使用默认语言");
                _currentLanguageCode = DEFAULT_LANGUAGE;
                csvAsset = Resources.Load<TextAsset>($"{_localizationPath}/{DEFAULT_LANGUAGE}");
            }

            if (csvAsset != null)
            {
                ParseCSV(csvAsset.text);
            }
            else
            {
                Debug.LogError($"[LocalizationManager] 无法加载任何本地化文件");
            }

            _isLoaded = true;
            Debug.Log($"[LocalizationManager] 已加载语言: {_currentLanguageCode}，共 {_currentTexts.Count} 条文本");
        }

        /// <summary>
        /// 解析CSV文本
        /// </summary>
        private void ParseCSV(string csvText)
        {
            if (string.IsNullOrEmpty(csvText)) return;

            string[] lines = csvText.Split('\n');
            
            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();
                if (string.IsNullOrEmpty(trimmedLine)) continue;
                if (trimmedLine.StartsWith("key,")) continue; // 跳过表头

                string[] parts = trimmedLine.Split(',');
                if (parts.Length >= 2)
                {
                    string key = parts[0].Trim();
                    string value = parts[1].Trim();
                    
                    // 处理引号包裹的文本
                    if (value.StartsWith("\"") && value.EndsWith("\""))
                    {
                        value = value.Substring(1, value.Length - 2);
                    }
                    
                    _currentTexts[key] = value;
                }
            }
        }

        /// <summary>
        /// 将SystemLanguage转换为语言代码
        /// </summary>
        private string ConvertToLanguageCode(SystemLanguage language)
        {
            switch (language)
            {
                case SystemLanguage.English: return "en";
                case SystemLanguage.ChineseSimplified: return "zh";
                case SystemLanguage.ChineseTraditional: return "zh-TW";
                case SystemLanguage.Japanese: return "ja";
                case SystemLanguage.Korean: return "ko";
                case SystemLanguage.French: return "fr";
                case SystemLanguage.Spanish: return "es";
                case SystemLanguage.German: return "de";
                case SystemLanguage.Russian: return "ru";
                case SystemLanguage.Portuguese: return "pt";
                case SystemLanguage.Italian: return "it";
                default: return DEFAULT_LANGUAGE;
            }
        }

        /// <summary>
        /// 将语言代码转换为SystemLanguage
        /// </summary>
        private SystemLanguage ConvertToSystemLanguage(string languageCode)
        {
            switch (languageCode.ToLower())
            {
                case "en": return SystemLanguage.English;
                case "zh":
                case "zh-cn": return SystemLanguage.ChineseSimplified;
                case "zh-tw":
                case "zh-hk": return SystemLanguage.ChineseTraditional;
                case "ja":
                case "jp": return SystemLanguage.Japanese;
                case "ko":
                case "kr": return SystemLanguage.Korean;
                case "fr": return SystemLanguage.French;
                case "es": return SystemLanguage.Spanish;
                case "de": return SystemLanguage.German;
                case "ru": return SystemLanguage.Russian;
                case "pt": return SystemLanguage.Portuguese;
                case "it": return SystemLanguage.Italian;
                default: return _defaultLanguage;
            }
        }

        /// <summary>
        /// 加载保存的语言设置
        /// </summary>
        private void LoadSavedLanguage()
        {
            string savedLanguage = PlayerPrefs.GetString(LANGUAGE_KEY, string.Empty);
            if (!string.IsNullOrEmpty(savedLanguage))
            {
                _currentLanguage = ConvertToSystemLanguage(savedLanguage);
            }
            else
            {
                // 使用系统语言
                _currentLanguage = Application.systemLanguage;
            }
            _currentLanguageCode = ConvertToLanguageCode(_currentLanguage);
        }

        /// <summary>
        /// 保存语言设置
        /// </summary>
        private void SaveLanguageSetting()
        {
            PlayerPrefs.SetString(LANGUAGE_KEY, _currentLanguageCode);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// 清除缓存
        /// </summary>
        public void ClearCache()
        {
            _currentTexts.Clear();
            _isLoaded = false;
        }
    }
}
