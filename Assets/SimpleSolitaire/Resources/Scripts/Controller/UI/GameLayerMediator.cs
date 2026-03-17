using UnityEngine;

namespace SimpleSolitaire.Controller.UI
{
    /// <summary>
    /// GameManager 与 UILayerManager 之间的中介者。
    ///
    /// 职责：
    ///   1. 集中定义所有弹窗的 LayerKey 常量，消除魔法字符串
    ///   2. 封装每个弹窗的 Show/Hide，统一转发给 UILayerManager
    ///   3. 处理需要弹窗切换的复合流程（如：关统计 → 开设置）
    ///   4. 保持 GameManager 现有 public 方法签名不变（Button.onClick 绑定无需修改）
    ///
    /// 挂载位置：与 GameManager 相同的 GameObject（Managers/GameManager）。
    /// </summary>
    public class GameLayerMediator : MonoBehaviour
    {
        // ── LayerKey 常量（与各弹窗挂载脚本的类名保持一致，即 GetType().Name）────
        public const string WinLayer          = "WinLayerUI";
        public const string ExitLayer         = "ExitLayerUI";
        public const string ContinueGameLayer = "ContinueLayerUI";
        public const string GameLayer         = "GameLayerUI";
        public const string AdsLayer          = "AdsLayerUI";
        public const string StatisticsLayer   = "StatisticsLayerUI";
        public const string SettingLayer      = "SettingLayerUI";
        public const string HowToPlayLayer    = "HowToPlayLayerUI";

        // ── 依赖引用 ──────────────────────────────────────────────────────────
        [SerializeField] private UILayerManager _layerManager;

        private void Awake()
        {
            if (_layerManager == null)
                _layerManager = UILayerManager.Instance;
        }

        // ── Win Layer ──────────────────────────────────────────────────────────
        public void ShowWinLayer() => _layerManager.Show(WinLayer);
        public void HideWinLayer() => _layerManager.Hide(WinLayer);

        // ── Exit Layer ─────────────────────────────────────────────────────────
        public void ShowExitLayer() => _layerManager.Show(ExitLayer);
        public void HideExitLayer() => _layerManager.Hide(ExitLayer);

        // ── Continue Layer ─────────────────────────────────────────────────────
        public void ShowContinueLayer() => _layerManager.Show(ContinueGameLayer);
        public void HideContinueLayer() => _layerManager.Hide(ContinueGameLayer);

        // ── Game Layer（游戏模式选择弹窗）─────────────────────────────────────
        public void ShowGameLayer() => _layerManager.Show(GameLayer);
        public void HideGameLayer() => _layerManager.Hide(GameLayer);

        // ── Ads Layer ──────────────────────────────────────────────────────────
        public void ShowAdsLayer() => _layerManager.Show(AdsLayer);
        public void HideAdsLayer() => _layerManager.Hide(AdsLayer);

        // ── Statistics Layer ───────────────────────────────────────────────────
        /// <summary>
        /// 显示统计弹窗。
        /// 若 SettingLayer 当前可见，则先关闭它，统计弹窗进入等待队列，
        /// SettingLayer 动画结束后自动弹出（利用 UILayerManager 等待队列机制）。
        /// </summary>
        public void ShowStatisticsLayer()
        {
            if (_layerManager.IsShowing(SettingLayer))
                _layerManager.Hide(SettingLayer);

            _layerManager.Show(StatisticsLayer);
        }

        public void HideStatisticsLayer() => _layerManager.Hide(StatisticsLayer);

        // ── Setting Layer ──────────────────────────────────────────────────────
        public void ShowSettingLayer() => _layerManager.Show(SettingLayer);
        public void HideSettingLayer() => _layerManager.Hide(SettingLayer);

        // ── How To Play Layer ──────────────────────────────────────────────────
        /// <summary>
        /// 显示 HowToPlay 弹窗。
        /// </summary>
        /// <param name="returnToSetting">
        /// 为 true 时：先关闭 SettingLayer，HowToPlay 关闭后自动重新打开 Setting。
        /// 对应原 OnClickSettingLayerRuleBtn 的复合流程。
        /// </param>
        public void ShowHowToPlayLayer(bool returnToSetting = false)
        {
            if (returnToSetting && _layerManager.IsShowing(SettingLayer))
            {
                _layerManager.Hide(SettingLayer);
            }

            _layerManager.Show(HowToPlayLayer);

            if (returnToSetting)
            {
                // 订阅 HideCompleted 事件：HowToPlay 关闭后自动回到 Setting
                var howToPlayLayer = _layerManager.GetTopLayer();
                if (howToPlayLayer != null)
                {
                    // 使用局部函数避免 lambda 捕获引用问题
                    void OnHowToPlayHidden()
                    {
                        howToPlayLayer.OnHideCompleted -= OnHowToPlayHidden;
                        ShowSettingLayer();
                    }

                    howToPlayLayer.OnHideCompleted += OnHowToPlayHidden;
                }
            }
        }

        public void HideHowToPlayLayer() => _layerManager.Hide(HowToPlayLayer);

        // ── Setting 刷新（供 GameManager 开关按钮回调后触发）────────────────
        /// <summary>
        /// 通知 SettingLayerUI 刷新所有开关图片状态。
        /// 仅在 SettingLayer 处于缓存中时有效（弹窗未实例化则无操作）。
        /// </summary>
        public void RefreshSettingLayer()
        {
            _layerManager.GetLayer<SettingLayerUI>(SettingLayer)?.RefreshSwitchStates();
        }

        // ── CardLayer 手动控制（特殊情况）────────────────────────────────────
        public void ShowCardLayer() => _layerManager.SetMainLayerVisible(true);
        public void HideCardLayer() => _layerManager.SetMainLayerVisible(false);
    }
}
