using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimpleSolitaire.Controller
{
    public class OrientationComponentActivator : OrientationObject
    {
        [SerializeField] private List<OrientationActivePair> _activePairs = new List<OrientationActivePair>();
        [SerializeField] private MonoBehaviour _targetComponent;

        public override void DoAction(OrientationScreen screen)
        {
            var pair = _activePairs.FirstOrDefault(x => x.Screen == screen);

            _targetComponent.enabled = pair.IsActive;
        }
    }
}