using System;
using System.Collections;
using System.Collections.Generic;
using SimpleSolitaire.Model.Enum;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI.Extensions;

namespace SimpleSolitaire.Controller
{
    public enum OrientationType
    {
        Portrait = 0,
        Landscape = 1,
    }

    public class OrientationManager : MonoBehaviour
    {
        [SerializeField] private DeckSizeManager _deckSizeManager;
        [SerializeField] private OrientationDataContainer _orientationContainer;
        [SerializeField] private HintManager _hintManager;
        [SerializeField] private CardLogic _cardLogic;
        [SerializeField] private AdsManager _adsManager;
        [SerializeField] private BottomMenuManager _bottomMenuManager;
        [SerializeField] private List<HandOrientationElement> _deckElements;

        [Header("Bottom bar:")] 
        [SerializeField] private float _portraitBottomBarItemWidth = 100f;
        [SerializeField] private float _landscapeBottomBarItemWidth = 50f;

        [Header("Layers:")] 
        [SerializeField] private float _portraitLayerScale = 1;
        [SerializeField] private float _landscapeLayerScale = 0.5f;

        [Header("Settings:")] 
        [SerializeField] private float _changeOrientationTime = 0.1f;
        [SerializeField] private OrientationType _orientationType = OrientationType.Portrait;

        [Header("Standalone settings:")] 
        [SerializeField] private int _standaloneDefaultWidth = 768;
        [SerializeField] private int _standaloneDefaultHeight = 1024;

        private float _orientationDeckWidth;
        private float _orientationDeckHeight;

        private bool _firstUpdate = true;
        private HandOrientation _handOrientation;
        private ScreenOrientation _lastOrientation;

        public OrientationType OrientationType => _orientationType;
        public HandOrientation HandOrientation => _handOrientation;
        public OrientationDataContainer OrientationContainer => _orientationContainer;
        public List<HandOrientationElement> DeckElements => _deckElements;

        public float LandscapeLayerScale => _landscapeLayerScale;
        public float PortraitLayerScale => _portraitLayerScale;

        public event Action<ScreenOrientation> OnOrientationChanged;

        public ScreenOrientation ScreenOrientation
        {
            get
            {
                if (Screen.height > Screen.width)
                {
                    return ScreenOrientation.Portrait;
                }
                else
                {
                    return ScreenOrientation.LandscapeLeft;
                }
            }
        }

        private void Start()
        {
            ChangeOrientationRoutine();
        }

        private void ChangeOrientationRoutine()
        {
            if (_orientationType == OrientationType.Portrait)
            {
                StartCoroutine(InternalOrientationApply(OrientationScreen.Portrait, ScreenOrientation.Portrait));
            }
            else if (_orientationType == OrientationType.Landscape)
            {
                StartCoroutine(InternalOrientationApply(OrientationScreen.Landscape, ScreenOrientation.LandscapeLeft));
            }
        }

        private IEnumerator InternalOrientationApply(OrientationScreen orientation, ScreenOrientation target)
        {
            var last = _lastOrientation.ToOrientationScreen();

            if (_firstUpdate || last != orientation)
            {
                _firstUpdate = false;
                _lastOrientation = target;
                Screen.orientation = _lastOrientation;
#if UNITY_STANDALONE && !UNITY_EDITOR
                bool isPortrait = _lastOrientation.ToOrientationScreen() == OrientationScreen.Portrait;
                int x = isPortrait ? _standaloneDefaultWidth : _standaloneDefaultHeight;
                int y = isPortrait ? _standaloneDefaultHeight : _standaloneDefaultWidth;
                Screen.SetResolution(x, y, FullScreenMode.Windowed);
#endif
                yield return new WaitForSeconds(_changeOrientationTime);

                SetOrientation();
            }
        }

        public IEnumerator UpdateResolution()
        {
            yield return new WaitForEndOfFrame();
            _cardLogic.InitializeSpacesDictionary();
            _hintManager.UpdateAvailableForDragCards();
        }

        public IEnumerator SetSpecificOrientation(OrientationScreen screen, HandOrientation hand)
        {
            _orientationContainer.Initialize(screen, hand);

            _deckElements.ForEach(x =>
            {
                if (_orientationContainer.CurrentElements.TryGetValue(x.Key, out OrientationElement element))
                {
                    x.Fitter.aspectMode = _orientationContainer.CurrentOrientation.AspectMode;
                    x.RectRoot.anchorMin = element.Anchor.Min;
                    x.RectRoot.anchorMax = element.Anchor.Max;
                    x.RectRoot.pivot = element.Pivot.Value;
                    x.RectRoot.anchoredPosition = element.Position.Value;
                    x.RectRoot.sizeDelta = element.Size.Value;
                }
            });

            yield return new WaitForEndOfFrame();

            _cardLogic.InitializeSpacesDictionary();

            if (screen == OrientationScreen.Portrait)
            {
                _orientationDeckWidth = _deckSizeManager.PortraitDeckSize.x;
                _orientationDeckHeight = _deckSizeManager.PortraitDeckSize.y;
            }
            else
            {
                _orientationDeckWidth = _deckSizeManager.LandscapeDeckSize.x;
                _orientationDeckHeight = _deckSizeManager.LandscapeDeckSize.y;
            }

            for (int i = 0; i < _cardLogic.CardsArray.Length; i++)
            {
                Card card = _cardLogic.CardsArray[i];
                card.CardChildRect.sizeDelta = new Vector2(_orientationDeckWidth, _orientationDeckHeight);
            }

            for (int i = 0; i < _cardLogic.AllDeckArray.Length; i++)
            {
                Deck targetDeck = _cardLogic.AllDeckArray[i];
                targetDeck.UpdateCardsPosition(false);
            }

            _hintManager.UpdateAvailableForDragCards();

            var bottomBarStartCorner = HandOrientation == HandOrientation.RIGHT ? TableLayoutGroup.Corner.UpperLeft : TableLayoutGroup.Corner.UpperRight;
            _bottomMenuManager.SetBottomBarStartCorner(bottomBarStartCorner);

            var bottomBarItemWidth = _orientationContainer.CurrentOrientation.ScrOrientation == OrientationScreen.Portrait ? _portraitBottomBarItemWidth : _landscapeBottomBarItemWidth;
           _bottomMenuManager.SetBottomBarItemWidth(bottomBarItemWidth);

            OnOrientationChanged?.Invoke(_lastOrientation);
        }

#if UNITY_EDITOR
        public void SetSpecificOrientationInEditor(OrientationScreen screen, HandOrientation hand)
        {
            _orientationContainer.Initialize(screen, hand);

            _deckElements.ForEach(x =>
            {
                if (_orientationContainer.CurrentElements.TryGetValue(x.Key, out OrientationElement element))
                {
                    x.Fitter.aspectMode = _orientationContainer.CurrentOrientation.AspectMode;
                    x.RectRoot.anchorMin = element.Anchor.Min;
                    x.RectRoot.anchorMax = element.Anchor.Max;
                    x.RectRoot.pivot = element.Pivot.Value;
                    x.RectRoot.anchoredPosition = element.Position.Value;
                    x.RectRoot.sizeDelta = element.Size.Value;
                    UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(x.Fitter);
                    UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(x.RectRoot);
                }
            });

            _cardLogic.InitializeSpacesDictionary();

            if (screen == OrientationScreen.Portrait)
            {
                _orientationDeckWidth = _deckSizeManager.PortraitDeckSize.x;
                _orientationDeckHeight = _deckSizeManager.PortraitDeckSize.y;
            }
            else
            {
                _orientationDeckWidth = _deckSizeManager.LandscapeDeckSize.x;
                _orientationDeckHeight = _deckSizeManager.LandscapeDeckSize.y;
            }

            for (int i = 0; i < _cardLogic.CardsArray.Length; i++)
            {
                Card card = _cardLogic.CardsArray[i];
                card.CardChildRect.sizeDelta = new Vector2(_orientationDeckWidth, _orientationDeckHeight);
                UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(card.CardChildRect);
            }

            var bottomBarStartCorner = HandOrientation == HandOrientation.RIGHT ? TableLayoutGroup.Corner.UpperLeft : TableLayoutGroup.Corner.UpperRight;
            _bottomMenuManager.SetBottomBarStartCorner(bottomBarStartCorner);

            var bottomBarItemWidth = _orientationContainer.CurrentOrientation.ScrOrientation == OrientationScreen.Portrait ? _portraitBottomBarItemWidth : _landscapeBottomBarItemWidth;
            _bottomMenuManager.SetBottomBarItemWidth(bottomBarItemWidth);

            var orientationObjects = FindObjectsOfType<OrientationObject>();
            foreach (var orientationObject in orientationObjects)
            {
                orientationObject.DoAction(screen);
            }

            UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(_bottomMenuManager.BottomPanel);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }
#endif

        public void SetOrientation()
        {
            StartCoroutine(SetSpecificOrientation(_lastOrientation.ToOrientationScreen(), HandOrientation));
        }

        public void SwitchOrientationType(OrientationType type)
        {
            _orientationType = type;

            ChangeOrientationRoutine();
        }

        public void SetHandOrientation(HandOrientation handOrientation)
        {
            _handOrientation = handOrientation;
        }
    }
}