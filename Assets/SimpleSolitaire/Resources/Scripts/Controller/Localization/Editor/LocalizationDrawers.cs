using UnityEditor;
using UnityEngine;

namespace SimpleSolitaire.Controller.Localization.Editor
{
    /// <summary>LocalizationIndex 的属性绘制器，在 Inspector 中显示行索引和提示信息。</summary>
    [CustomPropertyDrawer(typeof(LocalizationIndex))]
    public class LocalizationDrawerUIE : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = EditorGUIUtility.singleLineHeight;

            var indexRect = position;
            indexRect.width = 50;
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PropertyField(indexRect, property.FindPropertyRelative("index"), new GUIContent("", "行索引"));

            var hintRect = position;
            hintRect.xMin   = indexRect.xMax + 4;
            hintRect.width  = 300;
            EditorGUI.LabelField(hintRect, "语言文件路径：Resources/Localization/");

            EditorGUI.EndProperty();
        }
    }

    /// <summary>LocalizationIndexFolder 的折叠属性绘制器，同时显示正常描述和失败描述。</summary>
    [CustomPropertyDrawer(typeof(LocalizationIndexFolder))]
    public class LocalizationFoldDrawer : PropertyDrawer
    {
        private const float INDENT = 5f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position.height = EditorGUIUtility.singleLineHeight;
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, "文本");

            if (property.isExpanded)
            {
                position.y += EditorGUIUtility.singleLineHeight;
                DrawLabeledField(position, property, "description", "描述");

                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                DrawLabeledField(position, property, "failed", "失败描述");
            }

            EditorGUI.EndProperty();
        }

        private static void DrawLabeledField(Rect position, SerializedProperty property, string field, string labelText)
        {
            position.x += INDENT;
            var labelRect = position;
            labelRect.width = 70;
            EditorGUI.LabelField(labelRect, labelText);

            var fieldRect = position;
            fieldRect.xMin = labelRect.xMax + 4;
            EditorGUI.PropertyField(fieldRect, property.FindPropertyRelative(field), GUIContent.none);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.isExpanded
                ? EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing * 2
                : EditorGUIUtility.singleLineHeight;
        }
    }
}
