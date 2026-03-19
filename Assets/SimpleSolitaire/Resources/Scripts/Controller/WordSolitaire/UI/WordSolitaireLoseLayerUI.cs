using SimpleSolitaire.Controller.UI;
using SimpleSolitaire.Utility;
using SimpleSolitaire.Controller.WordSolitaire;  // 添加命名空间引用
using UnityEngine;
using UnityEngine.UI;

namespace SimpleSolitaire.Controller.WordSolitaire.UI
{
    /// <summary>
    /// Word Solitaire 失败弹窗
    /// 显示：步数耗尽提示
    /// 按钮：重试、返回主菜单
    /// </summary>
    public class WordSolitaireLoseLayerUI : UILayerBase
    {
        // LayerKey
        public override string LayerKey => "LoseLayerUI";

        // ── 外部依赖 ──────────────────────────────────────────────────────────
        private WordSolitaireGameManager _gameManager;
        private GameLayerMediator _mediator;

        // ── 内部组件 ──────────────────────────────────────────────────────────
        private Text _failText;
        private Text _stepsExhaustedText;
        private Button _retryButton;
        private Button _mainMenuButton;

        protected override void OnBindComponents()
        {
            // 查找依赖
            _gameManager = this.FindInScene<WordSolitaireGameManager>();
            _mediator = this.FindInScene<GameLayerMediator>();

            // 绑定文本组件
            _failText = ComponentFinder.Find<Text>(transform, "FailText");
            _stepsExhaustedText = ComponentFinder.Find<Text>(transform, "StepsExhaustedText");

            // 绑定按钮组件
            _retryButton = ComponentFinder.Find<Button>(transform, "RetryButton");
            _mainMenuButton = ComponentFinder.Find<Button>(transform, "MainMenuButton");

            // 绑定按钮事件
            _retryButton?.onClick.AddListener(OnClickRetry);
            _mainMenuButton?.onClick.AddListener(OnClickMainMenu);

            // 背景遮罩点击关闭（可选）
            ComponentFinder.Find<Button>(transform, "BGBlocker")?.onClick.AddListener(OnClickRetry);
        }

        protected override void OnLayerShow()
        {
            // 刷新显示内容
            RefreshDisplay();
        }

        protected override void OnLayerHide()
        {
            // 清理状态
        }

        /// <summary>
        /// 刷新显示内容
        /// </summary>
        private void RefreshDisplay()
        {
            var localization = LocalizationManager.Instance;

            // 失败标题
            if (_failText != null)
            {
                _failText.text = "Level Failed!";
            }

            // 步数耗尽提示
            if (_stepsExhaustedText != null)
            {
                _stepsExhaustedText.text = "Out of Moves!";
            }
        }

        // ── 按钮回调 ──────────────────────────────────────────────────────────

        /// <summary>
        /// 点击重试按钮
        /// </summary>
        private void OnClickRetry()
        {
            UILayerManager.Instance?.Hide(LayerKey);
            _gameManager?.RestartGame();
        }

        /// <summary>
        /// 点击返回主菜单按钮
        /// </summary>
        private void OnClickMainMenu()
        {
            UILayerManager.Instance?.Hide(LayerKey);
            _gameManager?.ReturnToMainMenu();
        }
    }
}
