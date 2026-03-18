using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimpleSolitaire.Controller.Localization
{
    /// <summary>
    /// 运行时语言选择组件。
    /// 挂载到场景中任意 GameObject 上，Start 时根据设置自动应用语言。
    /// 也可在 Inspector 中通过右键菜单或代码调用 SetLanguage() 手动切换。
    /// </summary>
    [AddComponentMenu("Localization/Language Selector")]
    public class LanguageSelector : MonoBehaviour
    {
        [Header("语言设置")]
        [SerializeField]
        [Tooltip("是否跟随系统语言")]
        private bool useSystemLanguage = true;

        [SerializeField]
        [Tooltip("手动指定的目标语言（useSystemLanguage = false 时生效）")]
        private SystemLanguage selectedLanguage = SystemLanguage.English;

        [Header("运行时设置")]
        [SerializeField]
        [Tooltip("Start 时自动应用语言设置")]
        private bool applyOnStart = true;

        // ── 只读调试字段（由 ReadOnlyAttribute 在 Inspector 中禁用编辑）──────
        [Header("调试信息（只读）")]
        [SerializeField]
        [ReadOnly]
        private SystemLanguage currentLanguage = SystemLanguage.English;

        [SerializeField]
        [ReadOnly]
        private bool isLanguageSupported = true;

        // ── Unity 生命周期 ────────────────────────────────────────────────────

        private void Start()
        {
            if (applyOnStart)
                ApplyLanguageSettings();

            UpdateDebugInfo();
        }

        // ── 公共 API ──────────────────────────────────────────────────────────

        /// <summary>根据当前字段配置应用语言设置。</summary>
        public void ApplyLanguageSettings()
        {
            if (useSystemLanguage)
                LocalizationManager.SetUseSystemLanguage(true);
            else
                LocalizationManager.ChangeLanguage(selectedLanguage);

            UpdateDebugInfo();
        }

        /// <summary>设置是否跟随系统语言。</summary>
        public void SetUseSystemLanguage(bool use)
        {
            useSystemLanguage = use;
            if (use)
                LocalizationManager.SetUseSystemLanguage(true);
            UpdateDebugInfo();
        }

        /// <summary>切换到指定语言。</summary>
        public void SetLanguage(SystemLanguage language)
        {
            selectedLanguage = language;
            useSystemLanguage = false;
            LocalizationManager.ChangeLanguage(language);
            UpdateDebugInfo();
        }

        /// <summary>通过下拉框索引切换语言（索引对应 GetSupportedLanguages() 数组）。</summary>
        public void SetLanguageByIndex(int index)
        {
            SystemLanguage[] supported = LocalizationManager.GetSupportedLanguages();
            if (index >= 0 && index < supported.Length)
                SetLanguage(supported[index]);
        }

        /// <summary>返回受支持语言的名称列表，可直接绑定到 Dropdown.options。</summary>
        public List<string> GetLanguageNames()
        {
            return LocalizationManager.GetSupportedLanguages().Select(l => l.ToString()).ToList();
        }

        /// <summary>返回当前语言在支持列表中的索引。</summary>
        public int GetCurrentLanguageIndex()
        {
            SystemLanguage[] supported = LocalizationManager.GetSupportedLanguages();
            SystemLanguage current = LocalizationManager.GetCurrentLanguage();
            for (int i = 0; i < supported.Length; i++)
            {
                if (supported[i] == current) return i;
            }
            return 0;
        }

        /// <summary>强制刷新场景中所有本地化文本组件。</summary>
        public void RefreshAllTexts()
        {
            LocalizationManager.RefreshAllLocalizedTexts();
        }

        // ── 内部 ──────────────────────────────────────────────────────────────

        private void UpdateDebugInfo()
        {
            currentLanguage     = LocalizationManager.GetCurrentLanguage();
            isLanguageSupported = LocalizationManager.IsLanguageSupported(currentLanguage);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying)
                ApplyLanguageSettings();
        }
#endif
    }

    // ── ReadOnly 属性（Inspector 只读显示）────────────────────────────────────

    /// <summary>标记 SerializeField 字段在 Inspector 中只读显示。</summary>
    public class ReadOnlyAttribute : UnityEngine.PropertyAttribute { }

#if UNITY_EDITOR
    [UnityEditor.CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : UnityEditor.PropertyDrawer
    {
        public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            UnityEditor.EditorGUI.PropertyField(position, property, label);
            GUI.enabled = true;
        }
    }
#endif
}
