using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;

namespace SimpleSolitaire.Controller.Localization
{
    /// <summary>
    /// 基于 TMP_Dropdown 的语言选择 UI 组件。
    /// 将此脚本挂载到含有 TMP_Dropdown 组件的 GameObject 上，
    /// 并将 OnChangeLanguage() 绑定到 Dropdown 的 OnValueChanged 事件。
    /// </summary>
    [RequireComponent(typeof(TMP_Dropdown))]
    public class LanguageSelectionGame : MonoBehaviour
    {
        [Tooltip("额外支持的语言（CultureInfo 代码 + 显示名称），用于补充 Resources/Localization/ 目录外的语言。")]
        public CultureTuple[] extraLanguages;

        private List<CultureInfo> _cultures;
        private TMP_Dropdown _dropdown;

        private void Start()
        {
            _dropdown = GetComponent<TMP_Dropdown>();

            // 加载 Resources/Localization/ 下所有语言文件
            TextAsset[] files = Resources.LoadAll<TextAsset>("Localization/");

            // 筛选出文件名与 CultureInfo.DisplayName 匹配的语言
            _cultures = CultureInfo.GetCultures(CultureTypes.AllCultures)
                .Where(c => files.Any(f => f.name == c.DisplayName))
                .ToList();

            // 追加额外语言
            _cultures.AddRange(extraLanguages.Select(e => new CultureInfo(e.culture)));

            // 填充下拉选项
            _dropdown.options = _cultures
                .Select(c => new TMP_Dropdown.OptionData(c.Name.ToUpper()))
                .ToList();

            // 初始化本地化系统并同步当前语言到下拉框
            LocalizationManager.InitializeLocalization();
            SystemLanguage current = LocalizationManager.GetCurrentLanguage();

            CultureInfo matched = _cultures.FirstOrDefault(c => c.EnglishName == current.ToString());
            if (matched != null)
            {
                _dropdown.captionText.text = matched.Name.ToUpper();
                _dropdown.value = _cultures.IndexOf(matched);
            }
        }

        /// <summary>绑定到 Dropdown.OnValueChanged，切换到用户选择的语言。</summary>
        public void OnChangeLanguage()
        {
            CultureInfo selected = GetSelectedCulture();
            if (selected == null) return;

            if (Enum.TryParse<SystemLanguage>(selected.EnglishName, out SystemLanguage lang))
                LocalizationManager.LoadLanguage(lang);
        }

        private CultureInfo GetSelectedCulture()
        {
            if (_cultures == null || _dropdown.value < 0 || _dropdown.value >= _cultures.Count)
                return null;
            return _cultures[_dropdown.value];
        }
    }

    /// <summary>额外语言配置（CultureInfo 代码与显示名称）。</summary>
    [Serializable]
    public struct CultureTuple
    {
        [Tooltip("CultureInfo 代码，如 \"fil\"、\"ms\"")]
        public string culture;

        [Tooltip("显示名称")]
        public string name;
    }
}
