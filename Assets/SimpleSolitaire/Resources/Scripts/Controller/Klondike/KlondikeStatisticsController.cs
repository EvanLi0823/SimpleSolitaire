using SimpleSolitaire.Model.Enum;

namespace SimpleSolitaire.Controller
{
    public class KlondikeStatisticsController : StatisticsController
    {
        public DeckRule CurrentStatisticRule { get; set; }

        protected override string StatisticPrefs => $"STATISTICS_KLONDIKE";
        private KlondikeCardLogic Logic => _cardLogicComponent as KlondikeCardLogic;

        private string OneRulePrefs   => $"{StatisticPrefs}_{DeckRule.ONE_RULE}";
        private string ThreeRulePrefs => $"{StatisticPrefs}_{DeckRule.THREE_RULE}";

        // Toggle 订阅/取消订阅已移至 StatisticsLayerUI（弹窗自己负责 Toggle 交互）

        /// <summary>
        /// 切换统计规则并重新从 PlayerPrefs 加载数据，由 StatisticsLayerUI 的 Toggle 回调驱动。
        /// </summary>
        public void ChangeStatisticType(DeckRule rule)
        {
            if (CurrentStatisticRule == rule) return;
            CurrentStatisticRule = rule;
            GetStatisticFromPrefs();
        }

        /// <summary>
        /// Save statistics to <see cref="_statisticOneRulePrefs"/> prefs.
        /// </summary>
        protected override void SaveStatisticInPrefs()
        {
            switch (Logic.CurrentRule)
            {
                case DeckRule.ONE_RULE:
                    SaveByRule(OneRulePrefs);
                    break;
                case DeckRule.THREE_RULE:
                    SaveByRule(ThreeRulePrefs);
                    break;
            }
        }

        /// <summary>
        /// Get all game statistic values from player prefs and parse it to variables.
        /// </summary>
        protected override void GetStatisticFromPrefs()
        {
            GetDataFromPrefsByRule(CurrentStatisticRule);
        }

        /// <summary>
        /// Time counter.
        /// </summary>
        protected override void GameTimer()
        {
            if (CurrentStatisticRule != Logic.CurrentRule)
                return;

            base.GameTimer();
        }

        public override void IncreasePlayedGamesAmount()
        {
            GetDataFromPrefsByRule(Logic.CurrentRule);
            base.IncreasePlayedGamesAmount();
        }

        private void GetDataFromPrefsByRule(DeckRule rule)
        {
            switch (rule)
            {
                case DeckRule.ONE_RULE:
                    GetByRule(OneRulePrefs);
                    break;
                case DeckRule.THREE_RULE:
                    GetByRule(ThreeRulePrefs);
                    break;
            }
        }
    }
}
