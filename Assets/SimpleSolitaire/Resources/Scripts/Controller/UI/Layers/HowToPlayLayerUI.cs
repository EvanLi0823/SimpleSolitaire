using SimpleSolitaire.Controller.Additional;
using SimpleSolitaire.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleSolitaire.Controller.UI
{
    public class HowToPlayLayerUI : UILayerBase
    {
        [SerializeField] private ScrollSnapRect _scrollSnap;
        [SerializeField] private RectTransform  _content;

        private HowToPlayManager _howToPlayManager;
        private bool _pagesGenerated;

        protected override void OnBindComponents()
        {
            _scrollSnap ??= GetComponentInChildren<ScrollSnapRect>(true);
            _content    ??= ComponentFinder.Find<RectTransform>(transform, "Content");

            _howToPlayManager = this.FindInScene<HowToPlayManager>();

            ComponentFinder.Find<Button>(transform, "CloseButtonField")?.onClick.AddListener(OnClickBack);
            ComponentFinder.Find<Button>(transform, "BGBlocker")?.onClick.AddListener(OnClickBack);
        }

        protected override void OnLayerShow()
        {
            if (!_pagesGenerated)
            {
                _howToPlayManager?.GeneratePagesInto(_content);
                _pagesGenerated = true;
                _scrollSnap?.Initialize();
            }

            _scrollSnap?.SetPage(0);
        }

        protected override void OnLayerHide() { }

        private void OnClickBack()
        {
            UILayerManager.Instance?.Hide(GameLayerMediator.HowToPlayLayer);
        }
    }
}
