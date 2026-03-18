using System.Text.RegularExpressions;
using UnityEngine;

namespace SimpleSolitaire.Controller.Localization
{
    /// <summary>
    /// 翻译文本中的占位符替换工具。
    ///
    /// 支持的占位符格式：{key}
    /// 查找顺序：
    ///   1. PlayerPrefs 中是否存有对应 key 的值
    ///   2. 无法匹配则保留原占位符原样输出（便于排查）
    ///
    /// 如需扩展业务占位符（如当前分数、关卡等），
    /// 在 GetPlaceholderValue 的 switch 中添加对应 case 即可。
    /// </summary>
    public static class PlaceholderManager
    {
        /// <summary>根据占位符 key 返回对应值。</summary>
        public static string GetPlaceholderValue(string placeholderKey)
        {
            switch (placeholderKey)
            {
                // ── 在此扩展游戏业务占位符 ──────────────────────────────────
                // 示例：
                // case "playerName":
                //     return PlayerPrefs.GetString("PlayerName", "Player");

                default:
                    // 回退：从 PlayerPrefs 读取
                    if (PlayerPrefs.HasKey(placeholderKey))
                        return PlayerPrefs.GetString(placeholderKey);

                    // 无法匹配则保留原占位符
                    return "{" + placeholderKey + "}";
            }
        }

        /// <summary>将文本中所有 {key} 形式的占位符替换为对应值。</summary>
        public static string ReplacePlaceholders(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            return Regex.Replace(input, @"\{(\w+)\}", match =>
                GetPlaceholderValue(match.Groups[1].Value));
        }
    }
}
