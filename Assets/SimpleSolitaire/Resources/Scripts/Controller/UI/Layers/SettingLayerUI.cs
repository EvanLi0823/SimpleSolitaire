using System;
using SimpleSolitaire.Utility;
using UnityEngine;

namespace SimpleSolitaire.Controller.UI
{
    /// <summary>
    /// 视觉/设置弹窗（VisualizeSettings）的 UILayerBase 子类。
    /// LayerKey = "VisualizeSettings"，Priority = Feature(60)，InteractType = SemiModal。
    ///
    /// GameLayerMediator 通过场景搜索自动绑定，无需 Inspector 拖拽。
    /// 原先 OnClickRuleBtn / OnClickStatisticBtn 中的 GetComponent + FindObjectOfType 双重查找
    /// 统一收归到 OnBindComponents，仅执行一次。
    /// </summary>
    public class SettingLayerUI : UILayerBase
    {
        /// <summary>弹窗显示时触发，用于刷新所有开关状态。GameManager 订阅此事件。</summary>
        public event Action OnRefreshRequested;

        // ── 跨节点依赖（场景查找）────────────────────────────────────────────
        private GameLayerMediator _mediator;

        // ── 组件绑定 ──────────────────────────────────────────────────────────

        protected override void OnBindComponents()
        {
            _mediator = this.FindInScene<GameLayerMediator>();
        }

        // ── 弹窗生命周期 ──────────────────────────────────────────────────────

        protected override void OnLayerShow()
        {
            OnRefreshRequested?.Invoke();
        }

        protected override void OnLayerHide() { }

        // ── 按钮回调（Inspector Button.onClick 绑定到此）─────────────────────

        /// <summary>点击"关闭"按钮。</summary>
        public void OnClickClose()
        {
            UILayerManager.Instance?.Hide(GameLayerMediator.SettingLayer);
        }

        /// <summary>
        /// 点击"规则"按钮：关闭 Setting → 打开 HowToPlay → 关闭后回到 Setting。
        /// </summary>
        public void OnClickRuleBtn()
        {
            _mediator?.ShowHowToPlayLayer(returnToSetting: true);
        }

        /// <summary>
        /// 点击"统计"按钮：关闭 Setting → 打开 Statistics。
        /// </summary>
        public void OnClickStatisticBtn()
        {
            _mediator?.ShowStatisticsLayer();
        }
    }
}
