using UnityEngine;

namespace SimpleSolitaire.Controller
{
    public static class OrientationDataHelper
    {
        public static OrientationScreen ToOrientationScreen(this ScreenOrientation orientation)
        {
            switch (orientation)
            {
                case ScreenOrientation.LandscapeLeft:
                case ScreenOrientation.LandscapeRight:
                    return OrientationScreen.Landscape;

                default: return OrientationScreen.Portrait;
            }
        }

        public static OrientationType ToOrientationType(this ScreenOrientation orientation)
        {
            switch (orientation)
            {
                case ScreenOrientation.LandscapeLeft:
                case ScreenOrientation.LandscapeRight:
                    return OrientationType.Landscape;

                default: return OrientationType.Portrait;
            }
        }
    }
}