using System;
using UnityEngine;

namespace SimpleSolitaire.Controller.Localization
{
    /// <summary>语言与文本的简单键值对，用于 Inspector 列表配置。</summary>
    [Serializable]
    public class LanguageObject
    {
        public SystemLanguage language;
        public string text;
    }
}
