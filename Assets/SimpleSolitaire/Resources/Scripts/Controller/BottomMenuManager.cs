using UnityEngine;
using UnityEngine.UI.Extensions;

namespace SimpleSolitaire.Controller
{
    public class BottomMenuManager : MonoBehaviour
    {
        [Header("Bottom menu fields:")] [SerializeField]
        private Animator _settingsPanelAnimator;

        [SerializeField] private RectTransform _bottomPanel;
        [SerializeField] private TableLayoutGroup _bottomBarGroup;
        [Space] [SerializeField] private OrientationManager _orientationManager;
        [SerializeField] private DeckSizeManager _deckSizeManager;
        [SerializeField] private AdsManager _adsManager;

        [Header("Bottom menu settings:")] 
        [SerializeField] private float _landscapeBottomBarAdditionalOffsetMultiplier = 1.3f;

        private bool _isBarActive;

        private readonly string _showBottomBarKey = "ShowBar";

        public GameObject BottomPanel => _bottomPanel.gameObject;

        public void Awake()
        {
            _adsManager.BannerStateAction += OnBannerState;
            _orientationManager.OnOrientationChanged += OnOnOrientationChanged;

            _isBarActive = true;
        }

        public void OnDestroy()
        {
            _adsManager.BannerStateAction -= OnBannerState;
            _orientationManager.OnOrientationChanged -= OnOnOrientationChanged;
        }

        private void OnOnOrientationChanged(ScreenOrientation obj)
        {
            TryResizeBottomPanel();
        }

        private void OnBannerState(bool state)
        {
            TryResizeBottomPanel();
        }

        private void TryResizeBottomPanel()
        {
            var scaleFactor = _deckSizeManager.GetOrientationScaleFactor();
            var additionalOffsetMultiplier = _orientationManager.OrientationType == OrientationType.Landscape ? _landscapeBottomBarAdditionalOffsetMultiplier : 1f;
            var height = _adsManager.BannerHeight > 0 ? (_adsManager.BannerHeight * additionalOffsetMultiplier) / scaleFactor : 0;

            InitializeBottomPanel(height);
        }

        private void InitializeBottomPanel(float offset)
        {
            if (_bottomPanel == null)
            {
                return;
            }

            _bottomPanel.anchoredPosition = new Vector2(0, offset);
        }

        public void SetBottomBarStartCorner(TableLayoutGroup.Corner corner)
        {
            if (_bottomBarGroup == null)
            {
                return;
            }

            _bottomBarGroup.StartCorner = corner;
        }

        public void SetBottomBarItemWidth(float width)
        {
            if (_bottomBarGroup == null)
            {
                return;
            }

            _bottomBarGroup.MinimumRowHeight = width;

            for (int i = 0; i < _bottomBarGroup.ColumnWidths.Length; i++)
            {
                _bottomBarGroup.ColumnWidths[i] = width;
            }
        }

        public void TryShowBar()
        {
            _isBarActive = !_isBarActive;
            _settingsPanelAnimator.SetBool(_showBottomBarKey, _isBarActive);
        }
    }
}