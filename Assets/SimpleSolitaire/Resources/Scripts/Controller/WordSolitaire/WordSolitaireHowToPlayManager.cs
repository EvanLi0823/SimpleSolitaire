namespace SimpleSolitaire.Controller
{
    /// <summary>
    /// WordSolitaire 玩法教程管理器
    /// 管理首次游玩时的教程弹窗展示
    /// </summary>
    public class WordSolitaireHowToPlayManager : HowToPlayManager
    {
        /// <summary>
        /// 首次游玩的PlayerPrefs键值
        /// </summary>
        protected override string FirstPlayKey => "WordSolitaireFirstPlay";
    }
}
