using UnityEngine;
using UnityEngine.UI;

namespace SimpleSolitaire.Controller
{
    public class DeckSizeManager : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;

        [Header("Correctly deck settings:")]
        [SerializeField] private HandOrientationElement _portraitDeckElement;
        [SerializeField] private HandOrientationElement _landscapeDeckElement;

        private Vector2 _portraitDeckSize;
        private Vector2 _landscapeDeckSize;
        private float _landscapeOrientationScaleDifference;
        private float _scaleFactorRatioPortraitCoefficient;
        private float _scaleFactorRatioLandscapeCoefficient;
        private float _portraitScaleFactor;
        private float _landscapeScaleFactor;

        public Vector2 PortraitDeckSize => _portraitDeckSize;
        public Vector2 LandscapeDeckSize => _landscapeDeckSize;
        public float LandscapeOrientationScaleDifference => _landscapeOrientationScaleDifference;

        public ScreenOrientation CurrentScreenOrientation => Screen.height > Screen.width ? ScreenOrientation.Portrait : ScreenOrientation.LandscapeLeft;

        public Vector2 CurrentDeckSize => CurrentScreenOrientation == ScreenOrientation.Portrait ? _portraitDeckSize : _landscapeDeckSize;

        private void Start()
        {
            InitializeDeckSizes();
        }

        private void InitializeDeckSizes()
        {
            bool isPortraitOrientation = CurrentScreenOrientation == ScreenOrientation.Portrait;
            _portraitScaleFactor = CalculateScaleFactor(isPortrait: isPortraitOrientation);
            _landscapeScaleFactor = CalculateScaleFactor(isPortrait: !isPortraitOrientation);

            _scaleFactorRatioPortraitCoefficient = 1;
            _scaleFactorRatioLandscapeCoefficient = 1;

            if (isPortraitOrientation)
            {
                _scaleFactorRatioLandscapeCoefficient = _portraitScaleFactor / _landscapeScaleFactor;
            }
            else
            {
                _scaleFactorRatioPortraitCoefficient = _portraitScaleFactor / _landscapeScaleFactor;
            }

            _portraitDeckSize = CalculateDeckSize(_portraitDeckElement, _portraitScaleFactor, _scaleFactorRatioPortraitCoefficient);
            _landscapeDeckSize = CalculateDeckSize(_landscapeDeckElement, _landscapeScaleFactor, _scaleFactorRatioLandscapeCoefficient);

            _landscapeOrientationScaleDifference = _landscapeDeckSize.y / _portraitDeckSize.y;
        }

        private float CalculateScaleFactor(bool isPortrait)
        {
            float aspectRatioFactor = (float)Screen.height / Screen.width;
            return GetOrientationScaleFactor() * (isPortrait ? 1 : aspectRatioFactor);
        }

        private Vector2 CalculateDeckSize(HandOrientationElement deckElement, float scaleFactor, float scaleFactorRatio)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(deckElement.RectRoot);

            Vector3[] corners = new Vector3[4];
            deckElement.RectRoot.GetWorldCorners(corners);

            return new Vector2(
                (corners[2].x - corners[0].x) / scaleFactor * scaleFactorRatio,
                (corners[2].y - corners[0].y) / scaleFactor * scaleFactorRatio
            );
        }

        public float GetOrientationScaleFactor() => _canvas != null ? _canvas.scaleFactor : 1f;
    }
}