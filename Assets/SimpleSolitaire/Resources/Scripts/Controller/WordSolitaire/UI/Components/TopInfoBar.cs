using UnityEngine;
using UnityEngine.UI;
using SimpleSolitaire.Controller.UI;

namespace SimpleSolitaire.Controller.WordSolitaire.UI
{
    /// <summary>
    /// 顶部信息栏组件
    /// 显示：金币数量、当前关卡、设置按钮
    /// 监听：GameEventBus.OnCoinsChanged事件
    /// </summary>
    public class TopInfoBar : MonoBehaviour
    {
        // ── Inspector配置 ────────────────────────────────────────────────────
        [Header("UI组件")]
        [SerializeField] private Text _coinsText;
        [SerializeField] private Text _levelText;
        [SerializeField] private Button _settingsButton;

        [Header("图标")]
        [SerializeField] private Image _coinsIcon;
        [SerializeField] private Image _levelIcon;

        // ── 外部依赖 ──────────────────────────────────────────────────────────
        private GameLayerMediator _mediator;
        private LevelDataManager _levelDataManager;

        // ── 数据 ──────────────────────────────────────────────────────────────
        private int _currentCoins;

        // ── Unity生命周期 ─────────────────────────────────────────────────────
        private void Awake()
        {
            // 如果Inspector未赋值，尝试自动查找
            if (_coinsText == null)
                _coinsText = transform.Find("CoinsText")?.GetComponent<Text>();
            if (_levelText == null)
                _levelText = transform.Find("LevelText")?.GetComponent<Text>();
            if (_settingsButton == null)
                _settingsButton = transform.Find("SettingsButton")?.GetComponent<Button>();
        }

        private void Start()
        {
            // 查找依赖
            _mediator = FindObjectOfType<GameLayerMediator>();
            _levelDataManager = FindObjectOfType<LevelDataManager>();

            // 绑定按钮事件
            if (_settingsButton != null)
            {
                _settingsButton.onClick.AddListener(OnClickSettings);
            }

            // 初始化显示
            RefreshDisplay();
        }

        private void OnEnable()
        {
            // 订阅金币变化事件
            GameEventBus.OnCoinsChanged += OnCoinsChanged;
        }

        private void OnDisable()
        {
            // 注销金币变化事件
            GameEventBus.OnCoinsChanged -= OnCoinsChanged;
        }

        // ── 事件处理 ──────────────────────────────────────────────────────────

        /// <summary>
        /// 金币变化事件回调
        /// </summary>
        private void OnCoinsChanged(int coins)
        {
            _currentCoins = coins;
            RefreshCoinsDisplay();
        }

        // ── 刷新显示 ──────────────────────────────────────────────────────────

        /// <summary>
        /// 刷新所有显示
        /// </summary>
        public void RefreshDisplay()
        {
            RefreshCoinsDisplay();
            RefreshLevelDisplay();
        }

        /// <summary>
        /// 刷新金币显示
        /// </summary>
        private void RefreshCoinsDisplay()
        {
            if (_coinsText != null)
            {
                _coinsText.text = _currentCoins.ToString("N0");
            }
        }

        /// <summary>
        /// 刷新关卡显示
        /// </summary>
        private void RefreshLevelDisplay()
        {
            if (_levelText != null && _levelDataManager != null)
            {
                int currentLevel = _levelDataManager.CurrentLevelId;
                _levelText.text = $"Level {currentLevel}";
            }
        }

        /// <summary>
        /// 设置金币数量（外部调用）
        /// </summary>
        public void SetCoins(int coins)
        {
            _currentCoins = coins;
            RefreshCoinsDisplay();
        }

        /// <summary>
        /// 设置关卡显示（外部调用）
        /// </summary>
        public void SetLevel(int level)
        {
            if (_levelText != null)
            {
                _levelText.text = $"Level {level}";
            }
        }

        // ── 按钮回调 ──────────────────────────────────────────────────────────

        /// <summary>
        /// 点击设置按钮
        /// </summary>
        private void OnClickSettings()
        {
            _mediator?.ShowSettingLayer();
        }
    }
}
