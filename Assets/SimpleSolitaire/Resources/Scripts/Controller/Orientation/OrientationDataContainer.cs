using System.Collections.Generic;
using System.Linq;
using SimpleSolitaire.Model.Enum;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleSolitaire.Controller
{
    public enum OrientationScreen
    {
        Portrait,
        Landscape
    }

    public enum OrientationElementKey
    {
        Unknown = 0,

        WastePack = 1,
        PackDeck = 2,
        EmptyDeck = 3,

        AceDeck_1 = 10,
        AceDeck_2 = 11,
        AceDeck_3 = 12,
        AceDeck_4 = 13,
        AceDeck_5 = 14,
        AceDeck_6 = 15,
        AceDeck_7 = 16,
        AceDeck_8 = 17,

        BottomDeck_1 = 20,
        BottomDeck_2 = 21,
        BottomDeck_3 = 22,
        BottomDeck_4 = 23,
        BottomDeck_5 = 24,
        BottomDeck_6 = 25,
        BottomDeck_7 = 26,
        BottomDeck_8 = 27,
        BottomDeck_9 = 28,
        BottomDeck_10 = 29,

        FreecellDeck_1 = 40,
        FreecellDeck_2 = 41,
        FreecellDeck_3 = 42,
        FreecellDeck_4 = 43,

        TripeaksDeck = 50,

        PyramidDeck = 60,
        PyramidTrashDeck = 61,
        
        CorrectlyDeck = 999
    }

    [CreateAssetMenu(fileName = "OrientationDataContainer", menuName = "Orientation/OrientationDataContainer")]
    public class OrientationDataContainer : ScriptableObject
    {
        [SerializeField] private List<Orientation> _orientations = new List<Orientation>();

        private Orientation _currentOrientation;
        private Dictionary<OrientationElementKey, OrientationElement> _currentElements = new Dictionary<OrientationElementKey, OrientationElement>();

        public Dictionary<OrientationElementKey, OrientationElement> CurrentElements => _currentElements;
        public Orientation CurrentOrientation => _currentOrientation;
        public List<Orientation> Orientations => _orientations;

        public void Initialize(OrientationScreen orientation, HandOrientation hand)
        {
            _currentElements.Clear();
            _currentOrientation = _orientations.FirstOrDefault(x => x.ScrOrientation == orientation && hand == x.Hand);

            if (_currentOrientation == null)
            {
                Debug.LogError($"Can't find orientation, please make sure that you have setup orientation in container for Screen {orientation} and Hand {hand}");
                return;
            }

            for (int i = 0; i < _currentOrientation.Elements.Count; i++)
            {
                var element = _currentOrientation.Elements[i];
                _currentElements[element.Key] = element;
            }
        }
    }

    [System.Serializable]
    public class Orientation
    {
        public OrientationScreen ScrOrientation;
        public HandOrientation Hand;
        public AspectRatioFitter.AspectMode AspectMode;
        public List<OrientationElement> Elements = new List<OrientationElement>();
    }

    [System.Serializable]
    public class OrientationElement
    {
        public OrientationElementKey Key;
        public OrientationPosition Position;
        public OrientationAnchor Anchor;
        public OrientationPivot Pivot;
        public OrientationSize Size;
    }

    [System.Serializable]
    public class OrientationSize
    {
        public Vector2 Value;
    }
    
    [System.Serializable]
    public class OrientationPosition
    {
        public Vector2 Value;
    }

    [System.Serializable]
    public class OrientationAnchor
    {
        public Vector2 Min;
        public Vector2 Max;
    }

    [System.Serializable]
    public class OrientationPivot
    {
        public Vector2 Value;
    }
}