using UnityEngine;

namespace SimpleSolitaire.Controller.Localization
{
    /// <summary>
    /// 游戏支持的语言枚举（替代Unity的SystemLanguage）。
    /// 包含 Unity SystemLanguage 未收录的 Filipino、Malay、PortugueseBR、Hindi 等语言。
    /// </summary>
    public enum GameLanguage
    {
        English,
        Czech,
        Dutch,
        French,
        German,
        Italian,
        Polish,
        Romanian,
        Spanish,
        Portuguese,
        PortugueseBR,
        Japanese,
        Korean,
        Chinese,
        ChineseTraditional,
        Hindi,
        Indonesian,
        Malay,
        Filipino,
        Thai,
        Vietnamese,
        Russian,
        Turkish,
        Arabic,
        Hebrew,
    }

    /// <summary>语言元信息（ISO 代码、本地名称、是否 RTL 等）。</summary>
    [System.Serializable]
    public class LanguageInfo
    {
        public GameLanguage language;
        public string languageCode;  // ISO 639-1，如 "en"、"zh"
        public string countryCode;   // ISO 3166-1，如 "US"、"CN"
        public string nativeName;    // 母语名称
        public string englishName;   // 英文名称
        public bool isRTL;           // 是否从右到左（阿拉伯语、希伯来语）

        public LanguageInfo(GameLanguage lang, string langCode, string country,
                            string native, string english, bool rtl = false)
        {
            language     = lang;
            languageCode = langCode;
            countryCode  = country;
            nativeName   = native;
            englishName  = english;
            isRTL        = rtl;
        }
    }

    /// <summary>语言枚举与 Unity SystemLanguage / ISO 代码之间的转换工具类。</summary>
    public static class GameLanguageHelper
    {
        public static LanguageInfo GetLanguageInfo(GameLanguage language)
        {
            switch (language)
            {
                case GameLanguage.English:            return new LanguageInfo(language, "en",    "US", "English",               "English");
                case GameLanguage.Czech:              return new LanguageInfo(language, "cs",    "CZ", "Čeština",               "Czech");
                case GameLanguage.Dutch:              return new LanguageInfo(language, "nl",    "NL", "Nederlands",            "Dutch");
                case GameLanguage.French:             return new LanguageInfo(language, "fr",    "FR", "Français",              "French");
                case GameLanguage.German:             return new LanguageInfo(language, "de",    "DE", "Deutsch",               "German");
                case GameLanguage.Italian:            return new LanguageInfo(language, "it",    "IT", "Italiano",              "Italian");
                case GameLanguage.Polish:             return new LanguageInfo(language, "pl",    "PL", "Polski",                "Polish");
                case GameLanguage.Romanian:           return new LanguageInfo(language, "ro",    "RO", "Română",               "Romanian");
                case GameLanguage.Spanish:            return new LanguageInfo(language, "es",    "ES", "Español",              "Spanish");
                case GameLanguage.Portuguese:         return new LanguageInfo(language, "pt",    "PT", "Português",            "Portuguese");
                case GameLanguage.PortugueseBR:       return new LanguageInfo(language, "pt-BR", "BR", "Português (Brasil)",  "Portuguese (Brazil)");
                case GameLanguage.Japanese:           return new LanguageInfo(language, "ja",    "JP", "日本語",               "Japanese");
                case GameLanguage.Korean:             return new LanguageInfo(language, "ko",    "KR", "한국어",               "Korean");
                case GameLanguage.Chinese:            return new LanguageInfo(language, "zh",    "CN", "简体中文",              "Chinese (Simplified)");
                case GameLanguage.ChineseTraditional: return new LanguageInfo(language, "zh-TW", "TW", "繁體中文",             "Chinese (Traditional)");
                case GameLanguage.Hindi:              return new LanguageInfo(language, "hi",    "IN", "हिन्दी",           "Hindi");
                case GameLanguage.Indonesian:         return new LanguageInfo(language, "id",    "ID", "Bahasa Indonesia",     "Indonesian");
                case GameLanguage.Malay:              return new LanguageInfo(language, "ms",    "MY", "Bahasa Melayu",        "Malay");
                case GameLanguage.Filipino:           return new LanguageInfo(language, "fil",   "PH", "Filipino",             "Filipino");
                case GameLanguage.Thai:               return new LanguageInfo(language, "th",    "TH", "ไทย",                 "Thai");
                case GameLanguage.Vietnamese:         return new LanguageInfo(language, "vi",    "VN", "Tiếng Việt",          "Vietnamese");
                case GameLanguage.Russian:            return new LanguageInfo(language, "ru",    "RU", "Русский",              "Russian");
                case GameLanguage.Turkish:            return new LanguageInfo(language, "tr",    "TR", "Türkçe",              "Turkish");
                case GameLanguage.Arabic:             return new LanguageInfo(language, "ar",    "SA", "العربية",             "Arabic",  true);
                case GameLanguage.Hebrew:             return new LanguageInfo(language, "he",    "IL", "עברית",               "Hebrew",  true);
                default:                              return GetLanguageInfo(GameLanguage.English);
            }
        }

        /// <summary>根据 ISO 639-1 语言代码获取对应 GameLanguage。</summary>
        public static GameLanguage GetLanguageFromCode(string languageCode)
        {
            if (string.IsNullOrEmpty(languageCode)) return GameLanguage.English;

            string mainLang = languageCode.Split('-')[0].ToLower();

            if (languageCode.ToLower() == "pt-br")                                          return GameLanguage.PortugueseBR;
            if (languageCode.ToLower().Contains("zh-tw") || languageCode.ToLower().Contains("zh-hk")) return GameLanguage.ChineseTraditional;

            switch (mainLang)
            {
                case "en":  return GameLanguage.English;
                case "cs":  return GameLanguage.Czech;
                case "nl":  return GameLanguage.Dutch;
                case "fr":  return GameLanguage.French;
                case "de":  return GameLanguage.German;
                case "it":  return GameLanguage.Italian;
                case "pl":  return GameLanguage.Polish;
                case "ro":  return GameLanguage.Romanian;
                case "es":  return GameLanguage.Spanish;
                case "pt":  return GameLanguage.Portuguese;
                case "ja":  return GameLanguage.Japanese;
                case "ko":  return GameLanguage.Korean;
                case "zh":  return GameLanguage.Chinese;
                case "hi":  return GameLanguage.Hindi;
                case "id":  return GameLanguage.Indonesian;
                case "ms":  return GameLanguage.Malay;
                case "fil": return GameLanguage.Filipino;
                case "tl":  return GameLanguage.Filipino; // Tagalog 映射到 Filipino
                case "th":  return GameLanguage.Thai;
                case "vi":  return GameLanguage.Vietnamese;
                case "ru":  return GameLanguage.Russian;
                case "tr":  return GameLanguage.Turkish;
                case "ar":  return GameLanguage.Arabic;
                case "he":  return GameLanguage.Hebrew;
                default:    return GameLanguage.English;
            }
        }

        /// <summary>将 Unity SystemLanguage 转换为 GameLanguage。</summary>
        public static GameLanguage FromSystemLanguage(SystemLanguage systemLanguage)
        {
            switch (systemLanguage)
            {
                case SystemLanguage.English:            return GameLanguage.English;
                case SystemLanguage.Czech:              return GameLanguage.Czech;
                case SystemLanguage.Dutch:              return GameLanguage.Dutch;
                case SystemLanguage.French:             return GameLanguage.French;
                case SystemLanguage.German:             return GameLanguage.German;
                case SystemLanguage.Italian:            return GameLanguage.Italian;
                case SystemLanguage.Polish:             return GameLanguage.Polish;
                case SystemLanguage.Romanian:           return GameLanguage.Romanian;
                case SystemLanguage.Spanish:            return GameLanguage.Spanish;
                case SystemLanguage.Portuguese:         return GameLanguage.Portuguese;
                case SystemLanguage.Japanese:           return GameLanguage.Japanese;
                case SystemLanguage.Korean:             return GameLanguage.Korean;
                case SystemLanguage.Chinese:            return GameLanguage.Chinese;
                case SystemLanguage.ChineseSimplified:  return GameLanguage.Chinese;
                case SystemLanguage.ChineseTraditional: return GameLanguage.ChineseTraditional;
                case SystemLanguage.Indonesian:         return GameLanguage.Indonesian;
                case SystemLanguage.Thai:               return GameLanguage.Thai;
                case SystemLanguage.Vietnamese:         return GameLanguage.Vietnamese;
                case SystemLanguage.Russian:            return GameLanguage.Russian;
                case SystemLanguage.Turkish:            return GameLanguage.Turkish;
                case SystemLanguage.Arabic:             return GameLanguage.Arabic;
                case SystemLanguage.Hebrew:             return GameLanguage.Hebrew;
                default:                                return GameLanguage.English;
            }
        }

        /// <summary>将 GameLanguage 转换回 Unity SystemLanguage（不支持的语言回退到 English）。</summary>
        public static SystemLanguage ToSystemLanguage(GameLanguage gameLanguage)
        {
            switch (gameLanguage)
            {
                case GameLanguage.English:            return SystemLanguage.English;
                case GameLanguage.Czech:              return SystemLanguage.Czech;
                case GameLanguage.Dutch:              return SystemLanguage.Dutch;
                case GameLanguage.French:             return SystemLanguage.French;
                case GameLanguage.German:             return SystemLanguage.German;
                case GameLanguage.Italian:            return SystemLanguage.Italian;
                case GameLanguage.Polish:             return SystemLanguage.Polish;
                case GameLanguage.Romanian:           return SystemLanguage.Romanian;
                case GameLanguage.Spanish:            return SystemLanguage.Spanish;
                case GameLanguage.Portuguese:         return SystemLanguage.Portuguese;
                case GameLanguage.PortugueseBR:       return SystemLanguage.Portuguese;
                case GameLanguage.Japanese:           return SystemLanguage.Japanese;
                case GameLanguage.Korean:             return SystemLanguage.Korean;
                case GameLanguage.Chinese:            return SystemLanguage.ChineseSimplified;
                case GameLanguage.ChineseTraditional: return SystemLanguage.ChineseTraditional;
                case GameLanguage.Indonesian:         return SystemLanguage.Indonesian;
                case GameLanguage.Thai:               return SystemLanguage.Thai;
                case GameLanguage.Vietnamese:         return SystemLanguage.Vietnamese;
                case GameLanguage.Russian:            return SystemLanguage.Russian;
                case GameLanguage.Turkish:            return SystemLanguage.Turkish;
                case GameLanguage.Arabic:             return SystemLanguage.Arabic;
                case GameLanguage.Hebrew:             return SystemLanguage.Hebrew;
                // Unity 不支持的语言回退到 English
                case GameLanguage.Hindi:
                case GameLanguage.Malay:
                case GameLanguage.Filipino:
                default: return SystemLanguage.English;
            }
        }
    }
}
