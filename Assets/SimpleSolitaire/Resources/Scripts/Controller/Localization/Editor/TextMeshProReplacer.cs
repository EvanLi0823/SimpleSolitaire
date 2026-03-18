using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace SimpleSolitaire.Controller.Localization.Editor
{
    /// <summary>
    /// 批量将选中对象及其子对象中的 TextMeshProUGUI 替换为 LocalizedTextMeshProUGUI 的编辑器工具。
    /// 菜单路径：Tools/Replace TMP with LocalizedTMP
    /// </summary>
    public class TextMeshProReplacer : EditorWindow
    {
        [MenuItem("Tools/Replace TMP with LocalizedTMP")]
        private static void ReplaceTextMeshProUGUI()
        {
            GameObject[] selected = Selection.gameObjects;
            if (selected.Length == 0)
            {
                EditorUtility.DisplayDialog("无选中对象", "请先在 Hierarchy 中选择至少一个 GameObject。", "OK");
                return;
            }

            int count = 0;
            foreach (GameObject obj in selected)
            {
                foreach (TextMeshProUGUI tmp in obj.GetComponentsInChildren<TextMeshProUGUI>(true))
                {
                    // 只替换纯 TextMeshProUGUI，跳过已定制的子类
                    if (tmp.GetType() != typeof(TextMeshProUGUI)) continue;

                    // 尝试获取旧 LocalizeText 上的 instanceID
                    LocalizeText companion = tmp.GetComponent<LocalizeText>();
                    string instanceID = companion != null ? companion.instanceID : string.Empty;

                    // 保存所有属性
                    string                 text                  = tmp.text;
                    TMP_FontAsset          font                  = tmp.font;
                    Material               fontMaterial          = tmp.fontMaterial;
                    Color                  color                 = tmp.color;
                    FontStyles             fontStyle             = tmp.fontStyle;
                    float                  fontSize              = tmp.fontSize;
                    bool                   autoSizeTextContainer = tmp.autoSizeTextContainer;
                    bool                   enableAutoSizing      = tmp.enableAutoSizing;
                    float                  characterSpacing      = tmp.characterSpacing;
                    float                  wordSpacing           = tmp.wordSpacing;
                    float                  lineSpacing           = tmp.lineSpacing;
                    float                  paragraphSpacing      = tmp.paragraphSpacing;
                    TextAlignmentOptions   alignment             = tmp.alignment;
                    bool                   enableWordWrapping    = tmp.enableWordWrapping;
                    TextOverflowModes      overflowMode          = tmp.overflowMode;
                    bool                   isRTL                 = tmp.isRightToLeftText;
                    bool                   enableKerning         = tmp.enableKerning;
                    bool                   extraPadding          = tmp.extraPadding;
                    bool                   richText              = tmp.richText;

                    DestroyImmediate(tmp);
                    if (companion != null) DestroyImmediate(companion);

                    LocalizedTextMeshProUGUI newComp = obj.AddComponent<LocalizedTextMeshProUGUI>();
                    newComp.text                  = text;
                    newComp.font                  = font;
                    newComp.fontMaterial          = fontMaterial;
                    newComp.color                 = color;
                    newComp.fontStyle             = fontStyle;
                    newComp.fontSize              = fontSize;
                    newComp.autoSizeTextContainer = autoSizeTextContainer;
                    newComp.enableAutoSizing      = enableAutoSizing;
                    newComp.characterSpacing      = characterSpacing;
                    newComp.wordSpacing           = wordSpacing;
                    newComp.lineSpacing           = lineSpacing;
                    newComp.paragraphSpacing      = paragraphSpacing;
                    newComp.alignment             = alignment;
                    newComp.enableWordWrapping    = enableWordWrapping;
                    newComp.overflowMode          = overflowMode;
                    newComp.isRightToLeftText     = isRTL;
                    newComp.enableKerning         = enableKerning;
                    newComp.extraPadding          = extraPadding;
                    newComp.richText              = richText;

                    // instanceID 是 public field，直接赋值
                    newComp.instanceID = instanceID;

                    count++;
                }
            }

            EditorUtility.DisplayDialog("替换完成", $"已将 {count} 个 TextMeshProUGUI 替换为 LocalizedTextMeshProUGUI。", "OK");
        }
    }
}
