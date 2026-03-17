#if UNITY_EDITOR
using UnityEditor;
#endif

using SimpleSolitaire.Controller;
using SimpleSolitaire.Utility;
using UnityEngine;

namespace SimpleSolitaire.Controller.UI
{
    /// <summary>
    /// 退出确认弹窗（ExitLayer）的 UILayerBase 子类。
    /// LayerKey = "ExitLayer"，Priority = System(100)，InteractType = Blocking。
    ///
    /// CardLogic 通过场景搜索自动绑定，无需 Inspector 拖拽。
    /// </summary>
    public class ExitLayerUI : UILayerBase
    {
        // ── 跨节点依赖（场景查找）────────────────────────────────────────────
        private CardLogic _cardLogic;

        // ── 组件绑定 ──────────────────────────────────────────────────────────

        protected override void OnBindComponents()
        {
            _cardLogic = this.FindInScene<CardLogic>();
        }

        protected override void OnLayerShow() { }
        protected override void OnLayerHide() { }

        // ── 按钮回调（Inspector Button.onClick 绑定到此）─────────────────────

        /// <summary>点击"确认退出"按钮。</summary>
        public void OnClickYes()
        {
            _cardLogic?.SaveGameState(isTempState: true);

            Hide(onComplete: () =>
            {
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            });
        }

        /// <summary>点击"取消"按钮。</summary>
        public void OnClickNo()
        {
            UILayerManager.Instance?.Hide(GameLayerMediator.ExitLayer);
        }
    }
}
