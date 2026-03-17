using System;
using UnityEngine;

namespace SimpleSolitaire.Controller.UI
{
    /// <summary>
    /// 游戏模式选择弹窗（KlondikeGameLayer）的 UILayerBase 子类。
    /// LayerKey = "KlondikeGameLayer"，Priority = Gameplay(80)，InteractType = SemiModal。
    ///
    /// 负责初始化规则 Toggle 状态（KlondikeCardLogic.InitRuleToggles 通过事件触发）。
    /// Random / Replay / Close 三个按钮决策通过委托回调通知 GameManager。
    /// </summary>
    public class GameLayerUI : UILayerBase
    {
        /// <summary>显示时需要初始化规则 Toggle，由 KlondikeGameManager 订阅并调用 InitRuleToggles()。</summary>
        public event Action OnLayerShown;

        /// <summary>点击"随机新游戏"时触发。</summary>
        public event Action OnRandomClicked;

        /// <summary>点击"重玩"时触发。</summary>
        public event Action OnReplayClicked;

        protected override void OnLayerShow()
        {
            OnLayerShown?.Invoke();
        }

        protected override void OnLayerHide() { }

        // ── 按钮回调（Inspector 中 Button.onClick 绑定到此）─────────────────

        /// <summary>点击"随机新游戏"按钮。</summary>
        public void OnClickRandom()
        {
            // 通过 UILayerManager 关闭，保持堆栈状态一致；游戏逻辑由 GameManager 订阅 OnHideCompleted 处理
            UILayerManager.Instance?.Hide(GameLayerMediator.GameLayer);
            OnRandomClicked?.Invoke();
        }

        /// <summary>点击"重玩"按钮。</summary>
        public void OnClickReplay()
        {
            UILayerManager.Instance?.Hide(GameLayerMediator.GameLayer);
            OnReplayClicked?.Invoke();
        }

        /// <summary>点击"关闭"按钮。</summary>
        public void OnClickClose()
        {
            UILayerManager.Instance?.Hide(GameLayerMediator.GameLayer);
        }
    }
}
