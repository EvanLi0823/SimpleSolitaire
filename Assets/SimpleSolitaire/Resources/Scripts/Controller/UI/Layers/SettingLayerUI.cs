using SimpleSolitaire.Utility;
using UnityEngine.UI;

namespace SimpleSolitaire.Controller.UI
{
    /// <summary>
    /// 设置弹窗。
    ///
    /// 解耦改造：
    ///   - 以前依赖 GameManager 具体类型；现在依赖 ISettingsProvider 接口。
    ///   - 以前由 GameManager 手动调用 RefreshSettingLayer()；
    ///     现在订阅 GameEventBus.OnSettingsChanged 自动刷新，无需外部驱动。
    /// </summary>
    public class SettingLayerUI : UILayerBase
    {
        private SwitchSpriteComponent     _soundSwitcher;
        private SwitchSpriteComponent     _orientationSwitcher;
        private SwitchSpriteComponent     _highlightDraggableSwitcher;
        private SwitchSpriteComponent     _autoCompleteSwitcher;
        private TextSwitchSpriteComponent _screenOrientationSwitcher;

        // 依赖接口而非具体类型，消除与 GameManager 的强耦合
        private ISettingsProvider _settings;
        private GameLayerMediator _mediator;

        protected override void OnBindComponents()
        {
            _soundSwitcher              = ComponentFinder.Find<SwitchSpriteComponent>(transform, "Sound");
            _orientationSwitcher        = ComponentFinder.Find<SwitchSpriteComponent>(transform, "Hand");
            _highlightDraggableSwitcher = ComponentFinder.Find<SwitchSpriteComponent>(transform, "HighlightCards");
            _autoCompleteSwitcher       = ComponentFinder.Find<SwitchSpriteComponent>(transform, "AutoComplete");
            _screenOrientationSwitcher  = ComponentFinder.Find<TextSwitchSpriteComponent>(transform, "Orientation");

            // 通过接口查找，不再绑定到 GameManager 具体类型
            _settings = this.FindServiceInScene<ISettingsProvider>();
            _mediator = this.FindInScene<GameLayerMediator>();

            ComponentFinder.Find<Button>(transform, "CloseButtonField")?.onClick.AddListener(OnClickClose);
            ComponentFinder.Find<Button>(transform, "BGBlocker")?.onClick.AddListener(OnClickClose);
            ComponentFinder.Find<Button>(transform, "Rules")?.onClick.AddListener(OnClickRuleBtn);
            ComponentFinder.Find<Button>(transform, "Statistics")?.onClick.AddListener(OnClickStatisticBtn);

            if (_settings != null)
            {
                // 代码绑定的按钮改用接口方法，不再引用 GameManager 具体方法
                ComponentFinder.Find<Button>(transform, "Hand")?.onClick.AddListener(_settings.ToggleHandOrientation);
                ComponentFinder.Find<Button>(transform, "HighlightCards")?.onClick.AddListener(_settings.ToggleHighlightDraggable);
                ComponentFinder.Find<Button>(transform, "AutoComplete")?.onClick.AddListener(_settings.ToggleAutoComplete);
                ComponentFinder.Find<Button>(transform, "Orientation")?.onClick.AddListener(_settings.CycleOrientationType);
            }
        }

        // ── UILayerBase 生命周期 ───────────────────────────────────────────────

        protected override void OnLayerShow()
        {
            RefreshSwitchStates();
            // 弹窗显示期间订阅设置变更事件，实现自动刷新
            GameEventBus.OnSettingsChanged += RefreshSwitchStates;
        }

        protected override void OnLayerHide()
        {
            // 弹窗隐藏时注销，避免不可见时收到无效刷新
            GameEventBus.OnSettingsChanged -= RefreshSwitchStates;
        }

        /// <summary>刷新所有开关图片状态，从 ISettingsProvider 读取当前值。</summary>
        public void RefreshSwitchStates()
        {
            if (_settings == null) return;

            _soundSwitcher?.UpdateSwitchImg(_settings.SoundEnabled);
            _autoCompleteSwitcher?.UpdateSwitchImg(_settings.AutoCompleteEnabled);
            _orientationSwitcher?.UpdateSwitchImg(_settings.IsRightHand);
            _highlightDraggableSwitcher?.UpdateSwitchImg(_settings.HighlightDraggable);
            _screenOrientationSwitcher?.UpdateSwitchImg(_settings.OrientationType);
        }

        // ── 按钮回调 ──────────────────────────────────────────────────────────

        private void OnClickClose()
        {
            UILayerManager.Instance?.Hide(GameLayerMediator.SettingLayer);
        }

        private void OnClickRuleBtn()
        {
            _mediator?.ShowHowToPlayLayer(returnToSetting: true);
        }

        private void OnClickStatisticBtn()
        {
            _mediator?.ShowStatisticsLayer();
        }
    }
}
