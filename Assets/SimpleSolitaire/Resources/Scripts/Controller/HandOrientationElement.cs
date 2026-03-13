using System;
using SimpleSolitaire.Controller;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleSolitaire
{
    public class HandOrientationElement : MonoBehaviour
    {
        public OrientationElementKey Key;

        public RectTransform RectRoot;
        public AspectRatioFitter Fitter;

        public void OnValidate()
        {
#if UNITY_EDITOR
            if (Key != OrientationElementKey.Unknown)
            {
                return;
            }

            var parsedName = name.Replace("_Data", "");
            Key = Enum.TryParse(parsedName, out OrientationElementKey result) ? result : OrientationElementKey.Unknown;
#endif
        }
    }
}