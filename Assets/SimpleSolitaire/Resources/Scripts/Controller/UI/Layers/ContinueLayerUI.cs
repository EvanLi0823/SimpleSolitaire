using System;
using UnityEngine;

namespace SimpleSolitaire.Controller.UI
{
    /// <summary>
    /// 继续上局游戏弹窗（ContinueGameLayer）的 UILayerBase 子类。
    /// LayerKey = "ContinueGameLayer"，Priority = Gameplay(80)，InteractType = Blocking。
    ///
    /// Yes/No 决策通过委托回调通知 GameManager，保持职责分离。
    /// </summary>
    public class ContinueLayerUI : UILayerBase
    {
        /// <summary>玩家点击"继续上局"（Yes）时触发。GameManager 订阅此事件执行 LoadGame()。</summary>
        public event Action OnContinueYes;

        /// <summary>玩家点击"开始新游戏"（No）时触发。GameManager 订阅此事件执行 NewGame()。</summary>
        public event Action OnContinueNo;

        protected override void OnLayerShow() { }
        protected override void OnLayerHide() { }

        // ── 按钮回调（Inspector 中 Button.onClick 绑定到此）─────────────────

        /// <summary>点击"继续"按钮。</summary>
        public void OnClickYes()
        {
            Hide(onComplete: () => OnContinueYes?.Invoke());
        }

        /// <summary>点击"新游戏"按钮。</summary>
        public void OnClickNo()
        {
            Hide(onComplete: () => OnContinueNo?.Invoke());
        }
    }
}
