using TMPro;
using UnityEngine;

namespace SimpleSolitaire.Controller.Localization
{
    /// <summary>
    /// 伴侣组件版本的本地化 TMP 文本。
    /// 与 LocalizedTextMeshProUGUI（继承自 TMP）不同，
    /// 本组件作为独立 MonoBehaviour 挂载，驱动同一 GameObject 上的 TextMeshProUGUI 组件。
    /// 适用于无法替换基类的场景。
    /// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    [AddComponentMenu("UI/Localize Text (TMP Companion)")]
    public class LocalizeText : MonoBehaviour
    {
        [Tooltip("本地化 key，对应语言文件中的条目名称")]
        public string instanceID;

        private TextMeshProUGUI _textObject;
        private string _originalText;

        private void Awake()
        {
            _textObject = GetComponent<TextMeshProUGUI>();
        }

        private void OnEnable()
        {
            _originalText = _textObject.text;
            UpdateText();
            LocalizationManager.OnLanguageChanged += UpdateText;
        }

        private void OnDisable()
        {
            LocalizationManager.OnLanguageChanged -= UpdateText;
        }

        /// <summary>根据当前语言刷新显示文本。</summary>
        public void UpdateText()
        {
            if (_textObject == null || string.IsNullOrEmpty(instanceID)) return;
            _textObject.text = LocalizationManager.GetText(instanceID, _originalText);
        }
    }
}
