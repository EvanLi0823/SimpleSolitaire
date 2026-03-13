using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimpleSolitaire.Controller
{
    public class OrientationObjectScaler : OrientationObject
    {
        [SerializeField] private RectTransform _rect;
        [SerializeField] private List<OrientationScalePair> _scalePairs = new List<OrientationScalePair>();

        public override void DoAction(OrientationScreen screen)
        {
            if (_rect != null)
            {
                var scalePair = _scalePairs.FirstOrDefault(x => x.Screen == screen);
                var scale = scalePair?.Scale ?? Vector3.one;

                _rect.localScale = scale;
            }
        }
    }
}