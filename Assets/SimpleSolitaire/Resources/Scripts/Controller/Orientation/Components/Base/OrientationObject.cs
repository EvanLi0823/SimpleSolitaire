using UnityEngine;

namespace SimpleSolitaire.Controller
{
    public abstract class OrientationObject : MonoBehaviour
    {
        [SerializeField] protected OrientationManager _orientationManager;

        protected void Awake()
        {
            _orientationManager.OnOrientationChanged += OnOrientationChanged;
        }

        protected void OnEnable()
        {
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
            _orientationManager.OnOrientationChanged -= OnOrientationChanged;
        }

        protected void OnOrientationChanged(ScreenOrientation obj)
            => DoAction(obj.ToOrientationScreen());

        public abstract void DoAction(OrientationScreen screen);
    }
}