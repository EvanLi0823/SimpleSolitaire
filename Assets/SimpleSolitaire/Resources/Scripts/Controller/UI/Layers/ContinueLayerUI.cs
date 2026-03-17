using System;
using SimpleSolitaire.Utility;
using UnityEngine.UI;

namespace SimpleSolitaire.Controller.UI
{
    public class ContinueLayerUI : UILayerBase
    {
        public event Action OnContinueYes;
        public event Action OnContinueNo;

        protected override void OnBindComponents()
        {
            ComponentFinder.Find<Button>(transform, "YesButton")?.onClick.AddListener(OnClickYes);
            ComponentFinder.Find<Button>(transform, "NoButton")?.onClick.AddListener(OnClickNo);
            ComponentFinder.Find<Button>(transform, "BGBlocker")?.onClick.AddListener(OnClickNo);
        }

        protected override void OnLayerShow() { }
        protected override void OnLayerHide() { }

        private void OnClickYes()
        {
            OnContinueYes?.Invoke();
            UILayerManager.Instance?.Hide(GameLayerMediator.ContinueGameLayer);
        }

        private void OnClickNo()
        {
            OnContinueNo?.Invoke();
            UILayerManager.Instance?.Hide(GameLayerMediator.ContinueGameLayer);
        }
    }
}
