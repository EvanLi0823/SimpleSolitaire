namespace SimpleSolitaire.Controller
{
    public class KlondikeGameManager : GameManager
    {
        // KlondikeGameLayerUI 已在弹窗打开时自行同步 Toggle 与 TempRule，
        // GameManager 层无需额外处理游戏规则的 UI 初始化
        protected override void InitCardLogic() { }
    }
}
