using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;

namespace SimpleSolitaire.Controller.Localization.Editor
{
    /// <summary>LocalizedTextMeshProUGUI 组件的自定义 Inspector，在 Key 字段变化时即时预览翻译结果。</summary>
    [CustomEditor(typeof(LocalizedTextMeshProUGUI), true)]
    [CanEditMultipleObjects]
    public class LocalizedTextMeshProUGUIEditor : TMP_EditorPanelUI
    {
        private SerializedProperty _instanceIDProp;
        private LocalizedTextMeshProUGUI _localizedText;

        protected override void OnEnable()
        {
            base.OnEnable();
            _instanceIDProp = serializedObject.FindProperty("instanceID");
            _localizedText  = (LocalizedTextMeshProUGUI)target;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(_instanceIDProp, new GUIContent("Localization Key"));
            if (EditorGUI.EndChangeCheck() || Event.current.type == EventType.Layout)
            {
                serializedObject.ApplyModifiedProperties();
                PreviewLocalizedText();
            }

            EditorGUILayout.Space();
            base.OnInspectorGUI();

            if (serializedObject.ApplyModifiedProperties())
                PreviewLocalizedText();
        }

        private void PreviewLocalizedText()
        {
            if (_localizedText == null || string.IsNullOrEmpty(_localizedText.instanceID)) return;

            string localized = LocalizationManager.GetText(_localizedText.instanceID, _localizedText.text);
            if (_localizedText.text == localized) return;

            _localizedText.text = localized;

            if (PrefabUtility.IsPartOfPrefabInstance(_localizedText))
                PrefabUtility.RecordPrefabInstancePropertyModifications(_localizedText);

            EditorUtility.SetDirty(_localizedText);
        }
    }
}
