using SimpleSolitaire.Model.Config;
using SimpleSolitaire.Utility;
using UnityEngine.UI;

namespace SimpleSolitaire.Controller.UI
{
    public class WinLayerUI : UILayerBase
    {
        private Text _timeWinLabel;
        private Text _scoreWinLabel;
        private Text _stepsWinLabel;

        private CongratulationManager _congratulationManager;
        private GameManager           _gameManager;

        protected override void OnBindComponents()
        {
            _timeWinLabel  = this.Find<Text>("TimeWinLabel");
            _scoreWinLabel = this.Find<Text>("ScoreWinLabel");
            _stepsWinLabel = this.Find<Text>("StepsWinLabel");

            _congratulationManager = this.FindInScene<CongratulationManager>();
            _gameManager           = this.FindInScene<GameManager>();

            if (_gameManager != null)
                ComponentFinder.Find<Button>(transform, "NewGameButton")?.onClick.AddListener(_gameManager.OnClickWinNewGame);
        }

        protected override void OnLayerShow()
        {
            if (_gameManager == null) return;

            _congratulationManager?.CongratulationTextFill();

            int time  = _gameManager.TimeCount;
            int score = _gameManager.ScoreCount + (time > 0 ? Public.SCORE_NUMBER / time : 0);
            int steps = _gameManager.StepCount;

            if (_timeWinLabel  != null) _timeWinLabel.text  = "YOUR TIME: "  + FormatTime(time);
            if (_scoreWinLabel != null) _scoreWinLabel.text = "YOUR SCORE: " + score;
            if (_stepsWinLabel != null) _stepsWinLabel.text = "YOUR MOVES: " + steps;
        }

        protected override void OnLayerHide() { }

        private static string FormatTime(int seconds)
        {
            int sec = seconds % 60;
            int min = seconds % 3600 / 60;
            return $"{min.ToString().PadLeft(2, '0')}:{sec.ToString().PadLeft(2, '0')}";
        }
    }
}
