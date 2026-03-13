using System.Collections.Generic;
using UnityEngine;

namespace SimpleSolitaire.Controller
{
    [System.Serializable]
    public enum DeckSpaceDividerType
    {
        Height = 0,
        Width = 1
    }

    [System.Serializable]
    public class DeckSpaceData
    {
        public DeckSpacesTypes Type;
        public DeckSpaceDividerType DividerType;
        public float Divider;
    }

    [System.Serializable]
    public class OrientationDeckSpaceData
    {
        public OrientationScreen Screen;
        public List<DeckSpaceData> Data = new List<DeckSpaceData>();
    }

    [CreateAssetMenu(fileName = "DeckSpacesContainer", menuName = "Deck/Deck Spaces Container")]
    public class DeckSpacesContainer : ScriptableObject
    {
        public List<OrientationDeckSpaceData> Data = new List<OrientationDeckSpaceData>();
    }
}