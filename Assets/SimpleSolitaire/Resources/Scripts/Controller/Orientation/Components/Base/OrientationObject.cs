using UnityEngine;

namespace SimpleSolitaire.Controller
{
    public abstract class OrientationObject : MonoBehaviour
    {
        [SerializeField] protected OrientationManager _orientationManager;

        protected void Awake()
        {
            if (_orientationManager == null) return;
            _orientationManager.OnOrientationChanged += OnOrientationChanged;
        }

        protected void OnEnable()
        {
            if (_orientationManager == null) return;
            var orientation = _orientationManager.OrientationContainer.CurrentOrientation;
            if (orientation == null)
            {
                return;
            }

            var screen = orientation.ScrOrientation;
            
            DoAction(screen);
        }

        protected void OnDestroy()
        {
            if (_orientationManager == null) return;
            _orientationManager.OnOrientationChanged -= OnOrientationChanged;
        }

        protected void OnOrientationChanged(ScreenOrientation obj)
            => DoAction(obj.ToOrientationScreen());

        public abstract void DoAction(OrientationScreen screen);
    }
}