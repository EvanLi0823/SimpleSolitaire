using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimpleSolitaire.Controller
{
    public class OrientationObjectActivator : OrientationObject
    {
        [SerializeField] private List<OrientationActivePair> _activePairs = new List<OrientationActivePair>();

        public override void DoAction(OrientationScreen screen)
        {
            var pair = _activePairs.FirstOrDefault(x => x.Screen == screen);

            gameObject.SetActive(pair.IsActive);
        }
    }
}