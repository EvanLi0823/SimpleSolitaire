using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleSolitaire.Controller.Localization.Editor
{
    /// <summary>
    /// GameObject/UI 菜单中创建 LocalizedText 对象，
    /// 以及右键将已有 Text 组件就地转换为 LocalizedText 的工具。
    /// </summary>
    public class LocalizedTextCreator
    {
        [MenuItem("GameObject/UI/Localized Text", false, 2001)]
        private static void CreateLocalizedText()
        {
            var go = new GameObject("Localized Text");
            var rectTransform = go.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(160, 30);

            var localizedText = go.AddComponent<LocalizedText>();
            localizedText.text      = "New Text";
            localizedText.font      = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            localizedText.fontSize  = 14;
            localizedText.alignment = TextAnchor.MiddleCenter;
            localizedText.color     = Color.black;

            // 挂载到选中对象下（如有 Canvas），否则找 / 创建 Canvas
            Canvas canvas = null;
            if (Selection.activeGameObject != null)
            {
                canvas = Selection.activeGameObject.GetComponentInParent<Canvas>();
                if (canvas != null)
                    go.transform.SetParent(Selection.activeGameObject.transform, false);
            }

            if (canvas == null)
            {
                canvas = Object.FindObjectOfType<Canvas>();
                if (canvas == null)
                {
                    var canvasGO = new GameObject("Canvas");
                    canvas = canvasGO.AddComponent<Canvas>();
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                    canvasGO.AddComponent<CanvasScaler>();
                    canvasGO.AddComponent<GraphicRaycaster>();

                    if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
                    {
                        var esGO = new GameObject("EventSystem");
                        esGO.AddComponent<UnityEngine.EventSystems.EventSystem>();
                        esGO.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                    }
                }
                go.transform.SetParent(canvas.transform, false);
            }

            Undo.RegisterCreatedObjectUndo(go, "Create Localized Text");
            Selection.activeGameObject = go;
        }

        /// <summary>将选中的 Text 组件就地替换为 LocalizedText（保留所有属性）。</summary>
        [MenuItem("CONTEXT/Text/Convert to Localized Text")]
        private static void ConvertToLocalizedText(MenuCommand command)
        {
            var original = (Text)command.context;
            var go = original.gameObject;

            // 保存原始属性
            var savedText               = original.text;
            var savedFont               = original.font;
            var savedFontSize           = original.fontSize;
            var savedFontStyle          = original.fontStyle;
            var savedLineSpacing        = original.lineSpacing;
            var savedSupportRichText    = original.supportRichText;
            var savedAlignment          = original.alignment;
            var savedAlignByGeometry    = original.alignByGeometry;
            var savedHorizontalOverflow = original.horizontalOverflow;
            var savedVerticalOverflow   = original.verticalOverflow;
            var savedResizeBestFit      = original.resizeTextForBestFit;
            var savedResizeMin          = original.resizeTextMinSize;
            var savedResizeMax          = original.resizeTextMaxSize;
            var savedColor              = original.color;
            var savedMaterial           = original.material;
            var savedRaycastTarget      = original.raycastTarget;

            Undo.RecordObject(go, "Convert to Localized Text");
            Undo.DestroyObjectImmediate(original);

            var localized = Undo.AddComponent<LocalizedText>(go);
            localized.text               = savedText;
            localized.font               = savedFont;
            localized.fontSize           = savedFontSize;
            localized.fontStyle          = savedFontStyle;
            localized.lineSpacing        = savedLineSpacing;
            localized.supportRichText    = savedSupportRichText;
            localized.alignment          = savedAlignment;
            localized.alignByGeometry    = savedAlignByGeometry;
            localized.horizontalOverflow = savedHorizontalOverflow;
            localized.verticalOverflow   = savedVerticalOverflow;
            localized.resizeTextForBestFit = savedResizeBestFit;
            localized.resizeTextMinSize  = savedResizeMin;
            localized.resizeTextMaxSize  = savedResizeMax;
            localized.color              = savedColor;
            localized.material           = savedMaterial;
            localized.raycastTarget      = savedRaycastTarget;
            localized.instanceID         = go.name;

            EditorUtility.SetDirty(go);
        }
    }
}
