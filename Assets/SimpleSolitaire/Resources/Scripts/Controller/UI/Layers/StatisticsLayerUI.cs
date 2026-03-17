using SimpleSolitaire.Model.Enum;
using SimpleSolitaire.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleSolitaire.Controller.UI
{
    public class StatisticsLayerUI : UILayerBase
    {
        private StatisticsController _statisticsController;

        private Text _gameTimeText;
        private Text _averageTimeText;
        private Text _bestTimeText;
        private Text _bestMovesText;
        private Text _playedGamesText;
        private Text _wonGamesText;
        private Text _movesText;
        private Text _averageScoreText;
        private Text _gameVersionText;

        private Toggle _oneRuleToggle;
        private Toggle _threeRuleToggle;

        protected override void OnBindComponents()
        {
            _statisticsController = this.FindInScene<StatisticsController>();

            _gameTimeText     = ComponentFinder.Get<Text>(transform, "GameTimeAmountText");
            _averageTimeText  = ComponentFinder.Get<Text>(transform, "AverageGameTimeText");
            _bestTimeText     = ComponentFinder.Get<Text>(transform, "BestGameTimeText");
            _bestMovesText    = ComponentFinder.Get<Text>(transform, "BestGameMovesText");
            _playedGamesText  = ComponentFinder.Get<Text>(transform, "PlayedGamesAmountText");
            _wonGamesText     = ComponentFinder.Get<Text>(transform, "WonGamesAmountText");
            _movesText        = ComponentFinder.Get<Text>(transform, "MovesAmountText");
            _averageScoreText = ComponentFinder.Get<Text>(transform, "AverageScoreAmountText");
            _gameVersionText  = ComponentFinder.Get<Text>(transform, "GameVersionText");

            _oneRuleToggle   = ComponentFinder.Find<Toggle>(transform, "OneDrawRuleToggle");
            _threeRuleToggle = ComponentFinder.Find<Toggle>(transform, "ThreeDrawRuleToggle");

            _oneRuleToggle?.onValueChanged.AddListener(isOn => { if (isOn) OnRuleToggleChanged(DeckRule.ONE_RULE); });
            _threeRuleToggle?.onValueChanged.AddListener(isOn => { if (isOn) OnRuleToggleChanged(DeckRule.THREE_RULE); });

            ComponentFinder.Find<Button>(transform, "CloseButtonField")?.onClick.AddListener(OnClickClose);
            ComponentFinder.Find<Button>(transform, "BGBlocker")?.onClick.AddListener(OnClickClose);
        }

        protected override void OnLayerShow()
        {
            RefreshDisplay();
        }

        protected override void OnLayerHide() { }

        private void OnClickClose()
        {
            UILayerManager.Instance?.Hide(GameLayerMediator.StatisticsLayer);
        }

        private void OnRuleToggleChanged(DeckRule rule)
        {
            if (_statisticsController is KlondikeStatisticsController klondike)
                klondike.ChangeStatisticType(rule);

            RefreshDisplay();
        }

        private void RefreshDisplay()
        {
            if (_statisticsController == null)
            {
                Debug.LogWarning("[StatisticsLayerUI] _statisticsController 未赋值，无法刷新统计数据。");
                return;
            }

            StatisticsData data = _statisticsController.BuildDisplayData();

            SetText(_gameTimeText,     data.GameTimeAmount);
            SetText(_averageTimeText,  data.AverageGameTime);
            SetText(_bestTimeText,     data.BestGameTime);
            SetText(_bestMovesText,    data.BestGameMoves);
            SetText(_playedGamesText,  data.PlayedGamesAmount);
            SetText(_wonGamesText,     data.WonGamesAmount);
            SetText(_movesText,        data.MovesAmount);
            SetText(_averageScoreText, data.AverageScoreAmount);
            SetText(_gameVersionText,  data.GameVersion);
        }

        private static void SetText(Text target, string value)
        {
            if (target != null) target.text = value;
        }
    }
}
