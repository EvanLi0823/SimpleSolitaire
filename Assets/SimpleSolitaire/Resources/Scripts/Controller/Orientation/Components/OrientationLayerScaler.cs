using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimpleSolitaire.Controller
{
    public class OrientationLayerScaler : OrientationObject
    {
        [SerializeField] private RectTransform _rect;

        [Header("Additional scale options:")] [SerializeField]
        private bool _useCustomScale = false;

        [SerializeField] private List<OrientationScalePair> _customScalePairs = new List<OrientationScalePair>();

        public override void DoAction(OrientationScreen screen)
        {
            if (_rect != null)
            {
                var scale = screen == OrientationScreen.Portrait ? _orientationManager.PortraitLayerScale : _orientationManager.LandscapeLayerScale;
                var scalePair = _customScalePairs.FirstOrDefault(x => x.Screen == screen);

                _rect.localScale = _useCustomScale && scalePair != null ? scalePair.Scale : new Vector3(scale, scale, scale);
            }
        }
    }
}