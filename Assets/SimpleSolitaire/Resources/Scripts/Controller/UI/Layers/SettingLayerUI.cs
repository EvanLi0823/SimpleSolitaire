using SimpleSolitaire.Utility;
using UnityEngine.UI;

namespace SimpleSolitaire.Controller.UI
{
    public class SettingLayerUI : UILayerBase
    {
        private SwitchSpriteComponent     _soundSwitcher;
        private SwitchSpriteComponent     _orientationSwitcher;
        private SwitchSpriteComponent     _highlightDraggableSwitcher;
        private SwitchSpriteComponent     _autoCompleteSwitcher;
        private TextSwitchSpriteComponent _screenOrientationSwitcher;

        private GameManager       _gameManager;
        private GameLayerMediator _mediator;

        protected override void OnBindComponents()
        {
            _soundSwitcher              = ComponentFinder.Find<SwitchSpriteComponent>(transform, "Sound");
            _orientationSwitcher        = ComponentFinder.Find<SwitchSpriteComponent>(transform, "Hand");
            _highlightDraggableSwitcher = ComponentFinder.Find<SwitchSpriteComponent>(transform, "HighlightCards");
            _autoCompleteSwitcher       = ComponentFinder.Find<SwitchSpriteComponent>(transform, "AutoComplete");
            _screenOrientationSwitcher  = ComponentFinder.Find<TextSwitchSpriteComponent>(transform, "Orientation");

            _gameManager = this.FindInScene<GameManager>();
            _mediator    = this.FindInScene<GameLayerMediator>();

            ComponentFinder.Find<Button>(transform, "CloseButtonField")?.onClick.AddListener(OnClickClose);
            ComponentFinder.Find<Button>(transform, "BGBlocker")?.onClick.AddListener(OnClickClose);
            ComponentFinder.Find<Button>(transform, "Rules")?.onClick.AddListener(OnClickRuleBtn);
            ComponentFinder.Find<Button>(transform, "Statistics")?.onClick.AddListener(OnClickStatisticBtn);

            if (_gameManager != null)
            {
                ComponentFinder.Find<Button>(transform, "Hand")?.onClick.AddListener(_gameManager.OnClickOrientationSwitch);
                ComponentFinder.Find<Button>(transform, "HighlightCards")?.onClick.AddListener(_gameManager.OnClickHighlightDraggableSwitch);
                ComponentFinder.Find<Button>(transform, "AutoComplete")?.onClick.AddListener(_gameManager.OnClickAutoCompleteEnablingSwitch);
                ComponentFinder.Find<Button>(transform, "Orientation")?.onClick.AddListener(_gameManager.OnClickOrientationStateSwitch);
            }
        }

        protected override void OnLayerShow()
        {
            RefreshSwitchStates();
        }

        protected override void OnLayerHide() { }

        public void RefreshSwitchStates()
        {
            if (_gameManager == null) return;

            _soundSwitcher?.UpdateSwitchImg(_gameManager.SoundEnabled);
            _autoCompleteSwitcher?.UpdateSwitchImg(_gameManager.AutoCompleteEnabled);
            _orientationSwitcher?.UpdateSwitchImg(_gameManager.IsRightHand);
            _highlightDraggableSwitcher?.UpdateSwitchImg(_gameManager.HighlightDraggable);
            _screenOrientationSwitcher?.UpdateSwitchImg(_gameManager.OrientationType);
        }

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
