using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI.Extensions;

namespace SimpleSolitaire.Controller
{
    public class OrientationTableLayoutGroup : OrientationObject
    {
        [SerializeField] private TableLayoutGroup _group;
        [SerializeField] private List<OrientationPaddingPair> _pairs = new List<OrientationPaddingPair>();

        public override void DoAction(OrientationScreen screen)
        {
            if (_group != null)
            {
                var pair = _pairs.FirstOrDefault(x => x.Screen == screen);
                if (pair == null)
                {
                    Debug.LogError($"Can't set pair for orientation {screen}");
                    return;
                }

                _group.padding = pair.Padding;
            }
        }
    }
}