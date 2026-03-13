using System;

namespace SimpleSolitaire.Controller
{
    [Serializable]
    public class OrientationRectPair
    {
        public OrientationScreen Screen;
        public OrientationPosition Position;
        public OrientationAnchor Anchor;
        public OrientationPivot Pivot;
        public OrientationSize Size;
    }
}