using SimpleSolitaire.Controller.UI;
using SimpleSolitaire.Utility;
using SimpleSolitaire.Controller.WordSolitaire;
using UnityEngine.UI;

namespace SimpleSolitaire.Controller.WordSolitaire.UI
{
    /// <summary>
    /// Word Solitaire 游戏选项弹窗 - 暂停菜单
    /// 功能：继续游戏、重新开始、返回主菜单
    /// </summary>
    public class WordSolitaireGameLayerUI : UILayerBase
    {
        // LayerKey 保持与 GameLayerMediator.GameLayer 常量一致
        public override string LayerKey => "GameLayerUI";

        // ── 外部依赖 ──────────────────────────────────────────────────────────
        private WordSolitaireGameManager _gameManager;
        private GameLayerMediator _mediator;

        // ── 内部组件 ──────────────────────────────────────────────────────────
        private Button _continueButton;
        private Button _restartButton;
        private Button _mainMenuButton;
        private Button _settingsButton;
        private Button _closeButton;

        protected override void OnBindComponents()
        {
            // 查找游戏管理器
            _gameManager = this.FindInScene<WordSolitaireGameManager>();
            _mediator = this.FindInScene<GameLayerMediator>();

            // 绑定按钮组件
            _continueButton = ComponentFinder.Find<Button>(transform, "ContinueButton");
            _restartButton = ComponentFinder.Find<Button>(transform, "RestartButton");
            _mainMenuButton = ComponentFinder.Find<Button>(transform, "MainMenuButton");
            _settingsButton = ComponentFinder.Find<Button>(transform, "SettingsButton");
            _closeButton = ComponentFinder.Find<Button>(transform, "CloseButton");

            // 绑定按钮事件
            _continueButton?.onClick.AddListener(OnClickContinue);
            _restartButton?.onClick.AddListener(OnClickRestart);
            _mainMenuButton?.onClick.AddListener(OnClickMainMenu);
            _settingsButton?.onClick.AddListener(OnClickSettings);
            _closeButton?.onClick.AddListener(OnClickClose);

            // 背景遮罩点击关闭
            ComponentFinder.Find<Button>(transform, "BGBlocker")?.onClick.AddListener(OnClickClose);
        }

        protected override void OnLayerShow()
        {
            // 暂停游戏
            if (_gameManager != null)
            {
                _gameManager.IsPause = true;
            }
        }

        protected override void OnLayerHide()
        {
            // 恢复游戏
            if (_gameManager != null)
            {
                _gameManager.IsPause = false;
            }
        }

        // ── 按钮回调 ──────────────────────────────────────────────────────────

        /// <summary>
        /// 点击继续游戏按钮
        /// </summary>
        private void OnClickContinue()
        {
            UILayerManager.Instance?.Hide(LayerKey);
        }

        /// <summary>
        /// 点击重新开始按钮
        /// </summary>
        private void OnClickRestart()
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

        /// <summary>
        /// 点击设置按钮
        /// </summary>
        private void OnClickSettings()
        {
            _mediator?.ShowSettingLayer();
        }

        /// <summary>
        /// 点击关闭按钮
        /// </summary>
        private void OnClickClose()
        {
            UILayerManager.Instance?.Hide(LayerKey);
        }
    }
}
