using System;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleSolitaire.Controller.UI
{
    /// <summary>
    /// 单个弹窗的静态配置项。
    /// </summary>
    [Serializable]
    public class UILayerConfigItem
    {
        [Tooltip("弹窗唯一 Key，即挂载脚本的类名（UILayerBase.GetType().Name）")]
        public string LayerKey;

        [Tooltip("弹窗业务优先级")]
        public LayerPriority Priority;

        [Tooltip("动画类型：Standard=Animator Appear/Disappear，Instant=无动画")]
        public LayerAnimationType AnimationType;

        [Tooltip("弹窗 GameObject 引用（用于 Inspector 可视化，运行时以场景中 UILayerBase 注册为准）")]
        public GameObject LayerGameObject;
    }

    /// <summary>
    /// 弹窗配置 ScriptableObject。
    /// 作为设计期文档，集中查看和校验项目所有弹窗配置。
    /// 运行时弹窗优先级以 UILayerBase 组件上的 SerializeField 为权威来源。
    ///
    /// 创建方式：Project 右键 → Create → SimpleSolitaire/UI Layer Config
    /// </summary>
    [CreateAssetMenu(
        fileName = "UILayerConfig",
        menuName = "SimpleSolitaire/UI Layer Config",
        order = 200)]
    public class UILayerConfig : ScriptableObject
    {
        [Header("当前项目所有弹窗配置清单")]
        public List<UILayerConfigItem> Layers = new List<UILayerConfigItem>
        {
            // ── System (100) ────────────────────────────────────────────────────
            new UILayerConfigItem
            {
                LayerKey      = "WinLayerUI",
                Priority      = LayerPriority.System,
                AnimationType = LayerAnimationType.Standard,
            },
            new UILayerConfigItem
            {
                LayerKey      = "ExitLayerUI",
                Priority      = LayerPriority.System,
                AnimationType = LayerAnimationType.Standard,
            },

            // ── Gameplay (80) ───────────────────────────────────────────────────
            new UILayerConfigItem
            {
                LayerKey      = "ContinueLayerUI",
                Priority      = LayerPriority.Gameplay,
                AnimationType = LayerAnimationType.Standard,
            },
            new UILayerConfigItem
            {
                LayerKey      = "GameLayerUI",
                Priority      = LayerPriority.Gameplay,
                AnimationType = LayerAnimationType.Standard,
            },

            // ── Feature (60) ────────────────────────────────────────────────────
            new UILayerConfigItem
            {
                LayerKey      = "AdsLayerUI",
                Priority      = LayerPriority.Feature,
                AnimationType = LayerAnimationType.Standard,
            },
            new UILayerConfigItem
            {
                LayerKey      = "StatisticsLayerUI",
                Priority      = LayerPriority.Feature,
                AnimationType = LayerAnimationType.Standard,
            },
            new UILayerConfigItem
            {
                LayerKey      = "SettingLayerUI",
                Priority      = LayerPriority.Feature,
                AnimationType = LayerAnimationType.Standard,
            },

            // ── Info (40) ───────────────────────────────────────────────────────
            new UILayerConfigItem
            {
                LayerKey      = "HowToPlayLayerUI",
                Priority      = LayerPriority.Info,
                AnimationType = LayerAnimationType.Standard,
            },
        };

        /// <summary>
        /// 根据 Key 查找配置项（编辑器工具/调试用）。
        /// </summary>
        public UILayerConfigItem FindByKey(string key)
        {
            return Layers?.Find(x => x.LayerKey == key);
        }
    }
}
