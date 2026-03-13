using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimpleSolitaire.Controller
{
    public class OrientationObjectRectSetter : OrientationObject
    {
        [SerializeField] private RectTransform _rect;
        [SerializeField] private List<OrientationRectPair> _pairs = new List<OrientationRectPair>();

        public override void DoAction(OrientationScreen screen)
        {
            if (_rect != null)
            {
                var element = _pairs.FirstOrDefault(x => x.Screen == screen);

                if (element == null)
                {
                    Debug.LogError($"Can't set rect pair for orientation {screen}");
                    return;
                }

                _rect.anchorMin = element.Anchor.Min;
                _rect.anchorMax = element.Anchor.Max;
                _rect.pivot = element.Pivot.Value;
                _rect.anchoredPosition = element.Position.Value;
                _rect.sizeDelta = element.Size.Value;
            }
        }
    }
}