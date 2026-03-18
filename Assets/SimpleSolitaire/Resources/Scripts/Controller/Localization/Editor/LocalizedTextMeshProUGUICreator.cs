using TMPro;
using UnityEditor;
using UnityEngine;

namespace SimpleSolitaire.Controller.Localization.Editor
{
    /// <summary>GameObject/UI 菜单中创建 LocalizedTextMeshProUGUI 对象的工具。</summary>
    public class LocalizedTextMeshProUGUICreator
    {
        [MenuItem("GameObject/UI/Localized TextMeshPro - Text (UI)", false, 2002)]
        private static void CreateLocalizedTextMeshProUGUI()
        {
            var go = new GameObject("Localized TextMeshPro");
            var localizedText = go.AddComponent<LocalizedTextMeshProUGUI>();

            localizedText.fontSize          = 32;
            localizedText.enableAutoSizing  = true;
            localizedText.fontSizeMin       = 16;
            localizedText.fontSizeMax       = 200;
            localizedText.alignment         = TextAlignmentOptions.Center;

            if (Selection.activeGameObject != null)
                go.transform.SetParent(Selection.activeGameObject.transform, false);

            Undo.RegisterCreatedObjectUndo(go, "Create Localized TextMeshPro");
            Selection.activeGameObject = go;
        }
    }
}
