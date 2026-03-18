using System;
using UnityEngine;

namespace SimpleSolitaire.Controller.Localization
{
    /// <summary>本地化索引数据，存储默认文本与行索引。</summary>
    [Serializable]
    public class LocalizationIndex
    {
        [Tooltip("默认文本（key 未找到时使用）")]
        public string text;

        [Tooltip("本地化文件中的行索引")]
        public int index;
    }

    /// <summary>LocalizationIndex 的分组容器，包含正常描述和失败描述两条记录。</summary>
    [Serializable]
    public class LocalizationIndexFolder
    {
        [Tooltip("正常描述")]
        public LocalizationIndex description;

        [Tooltip("失败描述")]
        public LocalizationIndex failed;
    }
}
