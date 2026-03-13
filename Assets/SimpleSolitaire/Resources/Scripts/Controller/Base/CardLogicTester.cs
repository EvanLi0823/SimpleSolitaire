using SimpleSolitaire.Controller;
using UnityEngine;

namespace SimpleSolitaire
{
    [RequireComponent(typeof(CardLogic))]
    public class CardLogicTester : MonoBehaviour
    {
        [SerializeField] private CardLogic _logic;
        [SerializeField] private KeyCode _refreshKeyCode = KeyCode.I;

        public void Update()
        {
            if (Input.GetKeyDown(_refreshKeyCode))
            {
                _logic.InitializeSpacesDictionary();
                foreach (var deck in _logic.AllDeckArray)
                {
                    deck.UpdateCardsPosition(false);
                }
            }
        }
    }
}