using SimpleSolitaire.Controller.UI;
using SimpleSolitaire.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleSolitaire.Controller.WordSolitaire.UI
{
    /// <summary>
    /// Word Solitaire 胜利弹窗
    /// 显示：关卡完成信息、金币奖励
    /// 按钮：下一关、重玩、返回主菜单
    /// </summary>
    public class WordSolitaireWinLayerUI : UILayerBase
    {
        // LayerKey
        public override string LayerKey => "WinLayerUI";

        // ── 外部依赖 ──────────────────────────────────────────────────────────
        private WordSolitaireGameManager _gameManager;
        private LevelDataManager _levelDataManager;
        private GameLayerMediator _mediator;

        // ── 内部组件 ──────────────────────────────────────────────────────────
        private Text _levelCompleteText;
        private Text _coinsRewardText;
        private Text _timeText;
        private Text _stepsText;
        private Button _nextLevelButton;
        private Button _replayButton;
        private Button _mainMenuButton;

        // ── 数据 ──────────────────────────────────────────────────────────────
        private int _rewardCoins;

        protected override void OnBindComponents()
        {
            // 查找依赖
            _gameManager = this.FindInScene<WordSolitaireGameManager>();
            _levelDataManager = this.FindInScene<LevelDataManager>();
            _mediator = this.FindInScene<GameLayerMediator>();

            // 绑定文本组件
            _levelCompleteText = ComponentFinder.Find<Text>(transform, "LevelCompleteText");
            _coinsRewardText = ComponentFinder.Find<Text>(transform, "CoinsRewardText");
            _timeText = ComponentFinder.Find<Text>(transform, "TimeText");
            _stepsText = ComponentFinder.Find<Text>(transform, "StepsText");

            // 绑定按钮组件
            _nextLevelButton = ComponentFinder.Find<Button>(transform, "NextLevelButton");
            _replayButton = ComponentFinder.Find<Button>(transform, "ReplayButton");
            _mainMenuButton = ComponentFinder.Find<Button>(transform, "MainMenuButton");

            // 绑定按钮事件
            _nextLevelButton?.onClick.AddListener(OnClickNextLevel);
            _replayButton?.onClick.AddListener(OnClickReplay);
            _mainMenuButton?.onClick.AddListener(OnClickMainMenu);
        }

        protected override void OnLayerShow()
        {
            // 刷新显示数据
            RefreshDisplay();
        }

        protected override void OnLayerHide()
        {
            // 清理数据
            _rewardCoins = 0;
        }

        /// <summary>
        /// 设置奖励金币数量
        /// </summary>
        public void SetRewardCoins(int coins)
        {
            _rewardCoins = coins;
        }

        /// <summary>
        /// 刷新显示内容
        /// </summary>
        private void RefreshDisplay()
        {
            var localization = LocalizationManager.Instance;
            var currentLevel = _levelDataManager?.GetCurrentLevel();

            // 关卡完成文本
            if (_levelCompleteText != null)
            {
                string levelText = currentLevel != null 
                    ? $"Level {currentLevel.LevelId} Complete!" 
                    : "Level Complete!";
                _levelCompleteText.text = levelText;
            }

            // 金币奖励
            if (_coinsRewardText != null)
            {
                _coinsRewardText.text = $"+{_rewardCoins}";
            }

            // 游戏时间
            if (_timeText != null && _gameManager != null)
            {
                _timeText.text = $"Time: {FormatTime(_gameManager.TimeCount)}";
            }

            // 步数
            if (_stepsText != null && _gameManager != null)
            {
                _stepsText.text = $"Steps: {_gameManager.StepCount}";
            }

            // 检查是否有下一关，控制下一关按钮显示
            if (_nextLevelButton != null)
            {
                bool hasNextLevel = _levelDataManager?.GetNextLevel() != null;
                _nextLevelButton.gameObject.SetActive(hasNextLevel);
            }
        }

        /// <summary>
        /// 格式化时间显示
        /// </summary>
        private string FormatTime(int seconds)
        {
            int minutes = seconds / 60;
            int secs = seconds % 60;
            return $"{minutes:D2}:{secs:D2}";
        }

        // ── 按钮回调 ──────────────────────────────────────────────────────────

        /// <summary>
        /// 点击下一关按钮
        /// </summary>
        private void OnClickNextLevel()
        {
            UILayerManager.Instance?.Hide(LayerKey, onComplete: () =>
            {
                _gameManager?.StartNextLevel();
            });
        }

        /// <summary>
        /// 点击重玩按钮
        /// </summary>
        private void OnClickReplay()
        {
            UILayerManager.Instance?.Hide(LayerKey, onComplete: () =>
            {
                _gameManager?.RestartGame();
            });
        }

        /// <summary>
        /// 点击返回主菜单按钮
        /// </summary>
        private void OnClickMainMenu()
        {
            UILayerManager.Instance?.Hide(LayerKey, onComplete: () =>
            {
                _gameManager?.ReturnToMainMenu();
            });
        }
    }
}
