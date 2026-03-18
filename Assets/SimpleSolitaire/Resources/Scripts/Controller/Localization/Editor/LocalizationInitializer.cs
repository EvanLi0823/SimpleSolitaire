using UnityEditor;

namespace SimpleSolitaire.Controller.Localization.Editor
{
    /// <summary>
    /// 编辑器启动时初始化本地化系统（目前保留为空，按需启用）。
    /// 若需要在非运行时模式下预览翻译文本，取消注释下方代码。
    /// </summary>
    [InitializeOnLoad]
    public class LocalizationInitializer
    {
        // static LocalizationInitializer()
        // {
        //     EditorApplication.delayCall += Initialize;
        // }
        //
        // static void Initialize()
        // {
        //     if (!Application.isPlaying)
        //     {
        //         LocalizationManager.InitializeLocalization();
        //     }
        // }
    }
}
