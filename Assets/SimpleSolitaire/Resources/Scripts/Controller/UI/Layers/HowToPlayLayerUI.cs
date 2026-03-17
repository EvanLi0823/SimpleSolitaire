using SimpleSolitaire.Controller;
using SimpleSolitaire.Utility;
using UnityEngine;

namespace SimpleSolitaire.Controller.UI
{
    /// <summary>
    /// 游戏教程弹窗（HowToPlayLayer）的 UILayerBase 子类。
    /// LayerKey = "HowToPlayLayer"，Priority = Info(40)，InteractType = SemiModal。
    ///
    /// HowToPlayManager 通过场景搜索自动绑定，无需 Inspector 拖拽。
    /// </summary>
    public class HowToPlayLayerUI : UILayerBase
    {
        // ── 跨节点依赖（场景查找）────────────────────────────────────────────
        private HowToPlayManager _howToPlayManager;

        // ── 组件绑定 ──────────────────────────────────────────────────────────

        protected override void OnBindComponents()
        {
            _howToPlayManager = this.FindInScene<HowToPlayManager>();
        }

        // ── 弹窗生命周期 ──────────────────────────────────────────────────────

        protected override void OnLayerShow()
        {
            _howToPlayManager?.SetFirstPage();
        }

        protected override void OnLayerHide() { }

        // ── 按钮回调（Inspector Button.onClick 绑定到此）─────────────────────

        /// <summary>点击"返回"按钮。</summary>
        public void OnClickBack()
        {
            UILayerManager.Instance?.Hide(GameLayerMediator.HowToPlayLayer);
        }
    }
}
