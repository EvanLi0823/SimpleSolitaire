#if UNITY_EDITOR
using UnityEditor;
#endif

using SimpleSolitaire.Utility;
using UnityEngine.UI;

namespace SimpleSolitaire.Controller.UI
{
    public class ExitLayerUI : UILayerBase
    {
        private CardLogic _cardLogic;

        protected override void OnBindComponents()
        {
            _cardLogic = this.FindInScene<CardLogic>();

            ComponentFinder.Find<Button>(transform, "YesButton")?.onClick.AddListener(OnClickYes);
            ComponentFinder.Find<Button>(transform, "NoButton")?.onClick.AddListener(OnClickNo);
            ComponentFinder.Find<Button>(transform, "BGBlocker")?.onClick.AddListener(OnClickNo);
        }

        protected override void OnLayerShow() { }
        protected override void OnLayerHide() { }

        private void OnClickYes()
        {
            _cardLogic?.SaveGameState(isTempState: true);

            void OnHidden()
            {
                OnHideCompleted -= OnHidden;
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
            OnHideCompleted += OnHidden;
            UILayerManager.Instance?.Hide(GameLayerMediator.ExitLayer);
        }

        private void OnClickNo()
        {
            UILayerManager.Instance?.Hide(GameLayerMediator.ExitLayer);
        }
    }
}
