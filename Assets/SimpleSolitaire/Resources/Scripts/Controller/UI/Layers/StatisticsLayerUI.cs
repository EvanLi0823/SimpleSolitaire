using System;
using UnityEngine;

namespace SimpleSolitaire.Controller.UI
{
    /// <summary>
    /// 统计数据弹窗（StatisticsLayer）的 UILayerBase 子类。
    /// LayerKey = "StatisticsLayer"，Priority = Feature(60)，InteractType = SemiModal。
    ///
    /// 显示时通过事件通知外部刷新统计数据（如 KlondikeStatisticsController.InitRuleToggle）。
    /// 关闭后通过等待队列或事件回调由 GameLayerMediator 决定是否回到 SettingLayer。
    /// </summary>
    public class StatisticsLayerUI : UILayerBase
    {
        /// <summary>弹窗显示时触发，用于刷新统计数据 UI。GameManager 订阅此事件。</summary>
        public event Action OnRefreshRequested;

        protected override void OnLayerShow()
        {
            OnRefreshRequested?.Invoke();
        }

        protected override void OnLayerHide() { }

        // ── 按钮回调（Inspector 中 Button.onClick 绑定到此）─────────────────

        /// <summary>点击"关闭"按钮。</summary>
        public void OnClickClose()
        {
            UILayerManager.Instance?.Hide(GameLayerMediator.StatisticsLayer);
        }
    }
}
