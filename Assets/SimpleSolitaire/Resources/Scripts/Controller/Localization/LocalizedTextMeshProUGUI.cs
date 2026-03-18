using TMPro;
using UnityEngine;

namespace SimpleSolitaire.Controller.Localization
{
    /// <summary>
    /// 继承自 TextMeshProUGUI 的本地化文本组件。
    /// 在 Inspector 中设置 instanceID（本地化 key），组件会自动在语言切换时刷新显示文本。
    /// </summary>
    public class LocalizedTextMeshProUGUI : TextMeshProUGUI
    {
        [SerializeField]
        [Tooltip("本地化 key，对应语言文件中的条目名称")]
        public string instanceID;

        private string _originalText;

        protected override void OnEnable()
        {
            base.OnEnable();
            _originalText = text;
            UpdateText();
            LocalizationManager.OnLanguageChanged += UpdateText;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            LocalizationManager.OnLanguageChanged -= UpdateText;
        }

        /// <summary>根据当前语言刷新显示文本。</summary>
        public void UpdateText()
        {
            if (string.IsNullOrEmpty(instanceID)) return;

            string newText = LocalizationManager.GetText(instanceID, _originalText);
            if (text != newText)
                text = newText;
        }
    }
}
