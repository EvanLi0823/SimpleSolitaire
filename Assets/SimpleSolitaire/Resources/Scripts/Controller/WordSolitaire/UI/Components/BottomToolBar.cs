using UnityEngine;
using UnityEngine.UI;

namespace SimpleSolitaire.Controller.WordSolitaire.UI
{
    /// <summary>
    /// 底部道具栏组件
    /// 显示：提示、撤回、万能牌按钮及剩余数量
    /// 绑定：道具点击事件
    /// </summary>
    public class BottomToolBar : MonoBehaviour
    {
        // ── Inspector配置 ────────────────────────────────────────────────────
        [Header("道具按钮")]
        [SerializeField] private Button _hintButton;
        [SerializeField] private Button _undoButton;
        [SerializeField] private Button _jokerButton;

        [Header("数量文本")]
        [SerializeField] private Text _hintCountText;
        [SerializeField] private Text _undoCountText;
        [SerializeField] private Text _jokerCountText;

        [Header("图标")]
        [SerializeField] private Image _hintIcon;
        [SerializeField] private Image _undoIcon;
        [SerializeField] private Image _jokerIcon;

        // ── 外部依赖 ──────────────────────────────────────────────────────────
        private WordSolitaireGameManager _gameManager;

        // ── 数据 ──────────────────────────────────────────────────────────────
        private int _hintCount;
        private int _undoCount;
        private int _jokerCount;
        private bool _isJokerActivated;

        // ── Unity生命周期 ─────────────────────────────────────────────────────
        private void Awake()
        {
            // 如果Inspector未赋值，尝试自动查找
            AutoFindComponents();
        }

        private void Start()
        {
            // 查找游戏管理器
            _gameManager = FindObjectOfType<WordSolitaireGameManager>();

            // 绑定按钮事件
            BindButtonEvents();

            // 初始化显示
            RefreshDisplay();
        }

        private void OnEnable()
        {
            // 订阅道具数量变化事件（如果有的话）
            // GameEventBus.OnHintCountChanged += OnHintCountChanged;
            // GameEventBus.OnUndoCountChanged += OnUndoCountChanged;
            // GameEventBus.OnJokerCountChanged += OnJokerCountChanged;
        }

        private void OnDisable()
        {
            // 注销事件
            // GameEventBus.OnHintCountChanged -= OnHintCountChanged;
            // GameEventBus.OnUndoCountChanged -= OnUndoCountChanged;
            // GameEventBus.OnJokerCountChanged -= OnJokerCountChanged;
        }

        // ── 组件查找 ──────────────────────────────────────────────────────────

        /// <summary>
        /// 自动查找组件
        /// </summary>
        private void AutoFindComponents()
        {
            if (_hintButton == null)
                _hintButton = transform.Find("HintButton")?.GetComponent<Button>();
            if (_undoButton == null)
                _undoButton = transform.Find("UndoButton")?.GetComponent<Button>();
            if (_jokerButton == null)
                _jokerButton = transform.Find("JokerButton")?.GetComponent<Button>();

            if (_hintCountText == null)
                _hintCountText = _hintButton?.transform.Find("CountText")?.GetComponent<Text>();
            if (_undoCountText == null)
                _undoCountText = _undoButton?.transform.Find("CountText")?.GetComponent<Text>();
            if (_jokerCountText == null)
                _jokerCountText = _jokerButton?.transform.Find("CountText")?.GetComponent<Text>();
        }

        /// <summary>
        /// 绑定按钮事件
        /// </summary>
        private void BindButtonEvents()
        {
            if (_hintButton != null)
                _hintButton.onClick.AddListener(OnClickHint);
            if (_undoButton != null)
                _undoButton.onClick.AddListener(OnClickUndo);
            if (_jokerButton != null)
                _jokerButton.onClick.AddListener(OnClickJoker);
        }

        // ── 刷新显示 ──────────────────────────────────────────────────────────

        /// <summary>
        /// 刷新所有显示
        /// </summary>
        public void RefreshDisplay()
        {
            RefreshHintCount();
            RefreshUndoCount();
            RefreshJokerCount();
            RefreshJokerState();
        }

        /// <summary>
        /// 刷新提示数量
        /// </summary>
        private void RefreshHintCount()
        {
            if (_hintCountText != null)
            {
                _hintCountText.text = _hintCount.ToString();
            }

            // 数量为0时禁用按钮或变灰
            if (_hintButton != null)
            {
                _hintButton.interactable = _hintCount > 0;
            }
        }

        /// <summary>
        /// 刷新撤回数量
        /// </summary>
        private void RefreshUndoCount()
        {
            if (_undoCountText != null)
            {
                _undoCountText.text = _undoCount.ToString();
            }

            if (_undoButton != null)
            {
                _undoButton.interactable = _undoCount > 0;
            }
        }

        /// <summary>
        /// 刷新万能牌数量
        /// </summary>
        private void RefreshJokerCount()
        {
            if (_jokerCountText != null)
            {
                _jokerCountText.text = _jokerCount.ToString();
            }

            if (_jokerButton != null)
            {
                _jokerButton.interactable = _jokerCount > 0 && !_isJokerActivated;
            }
        }

        /// <summary>
        /// 刷新万能牌激活状态
        /// </summary>
        private void RefreshJokerState()
        {
            if (_jokerButton != null)
            {
                // 激活状态下高亮显示
                var buttonImage = _jokerButton.GetComponent<Image>();
                if (buttonImage != null)
                {
                    buttonImage.color = _isJokerActivated ? Color.yellow : Color.white;
                }
            }
        }

        // ── 公开API ───────────────────────────────────────────────────────────

        /// <summary>
        /// 设置提示道具数量
        /// </summary>
        public void SetHintCount(int count)
        {
            _hintCount = Mathf.Max(0, count);
            RefreshHintCount();
        }

        /// <summary>
        /// 设置撤回道具数量
        /// </summary>
        public void SetUndoCount(int count)
        {
            _undoCount = Mathf.Max(0, count);
            RefreshUndoCount();
        }

        /// <summary>
        /// 设置万能牌道具数量
        /// </summary>
        public void SetJokerCount(int count)
        {
            _jokerCount = Mathf.Max(0, count);
            RefreshJokerCount();
        }

        /// <summary>
        /// 设置万能牌激活状态
        /// </summary>
        public void SetJokerActivated(bool activated)
        {
            _isJokerActivated = activated;
            RefreshJokerState();
            RefreshJokerCount();
        }

        /// <summary>
        /// 获取当前提示数量
        /// </summary>
        public int GetHintCount() => _hintCount;

        /// <summary>
        /// 获取当前撤回数量
        /// </summary>
        public int GetUndoCount() => _undoCount;

        /// <summary>
        /// 获取当前万能牌数量
        /// </summary>
        public int GetJokerCount() => _jokerCount;

        // ── 按钮回调 ──────────────────────────────────────────────────────────

        /// <summary>
        /// 点击提示按钮
        /// </summary>
        private void OnClickHint()
        {
            if (_hintCount <= 0) return;

            _hintCount--;
            RefreshHintCount();

            // 调用游戏管理器的提示功能
            _gameManager?.UseHint();
        }

        /// <summary>
        /// 点击撤回按钮
        /// </summary>
        private void OnClickUndo()
        {
            if (_undoCount <= 0) return;

            _undoCount--;
            RefreshUndoCount();

            // 调用游戏管理器的撤回功能
            _gameManager?.UseUndo();
        }

        /// <summary>
        /// 点击万能牌按钮
        /// </summary>
        private void OnClickJoker()
        {
            if (_jokerCount <= 0 || _isJokerActivated) return;

            _jokerCount--;
            _isJokerActivated = true;
            RefreshJokerCount();
            RefreshJokerState();

            // 调用游戏管理器的万能牌功能
            _gameManager?.ActivateJoker();
        }

        /// <summary>
        /// 消耗万能牌效果（当使用万能牌放置卡牌后调用）
        /// </summary>
        public void ConsumeJoker()
        {
            if (_isJokerActivated)
            {
                _isJokerActivated = false;
                RefreshJokerState();
                RefreshJokerCount();
            }
        }
    }
}
