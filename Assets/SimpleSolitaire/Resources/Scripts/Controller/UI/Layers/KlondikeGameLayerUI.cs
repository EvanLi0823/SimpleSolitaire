using SimpleSolitaire.Model.Enum;
using SimpleSolitaire.Utility;
using UnityEngine.UI;

namespace SimpleSolitaire.Controller.UI
{
    /// <summary>
    /// Klondike 游戏选项弹窗（KlondikeGameLayer）的专属子类。
    ///
    /// 职责边界：
    ///   - 自绑定规则 Toggle（OneDrawRuleToggle / ThreeDrawRuleToggle），无跨节点 Inspector 引用
    ///   - 弹窗打开时将 Toggle 同步至 KlondikeCardLogic.CurrentRule
    ///   - Toggle 变化时直接写入 KlondikeCardLogic.TempRule（View → Controller，方向合法）
    ///   - KlondikeCardLogic 不再持有任何 Toggle 引用
    ///
    /// LayerKey 覆写为 "GameLayerUI"，与 GameLayerMediator.GameLayer 常量保持一致。
    /// </summary>
    public class KlondikeGameLayerUI : GameLayerUI
    {
        // LayerKey 保持与父类相同，GameLayerMediator.GameLayer = "GameLayerUI" 无需修改
        public override string LayerKey => "GameLayerUI";

        // ── 外部依赖（运行时场景查找，避免 Prefab 实例化时 Inspector 引用丢失）──
        private KlondikeCardLogic _cardLogic;

        // ── 内部 Toggle（自绑定，禁止跨节点 Inspector 拖拽）─────────────────
        private Toggle _oneRuleToggle;
        private Toggle _threeRuleToggle;

        protected override void OnBindComponents()
        {
            base.OnBindComponents();

            _cardLogic = this.FindInScene<KlondikeCardLogic>();

            // ── Toggle 绑定（Find = 递归名称查找，支持任意深度子节点）─────
            _oneRuleToggle   = ComponentFinder.Find<Toggle>(transform, "OneDrawToggle");
            _threeRuleToggle = ComponentFinder.Find<Toggle>(transform, "ThreeDrawToggle");

            _oneRuleToggle?.onValueChanged.AddListener(isOn =>
            {
                if (isOn) _cardLogic.TempRule = DeckRule.ONE_RULE;
            });
            _threeRuleToggle?.onValueChanged.AddListener(isOn =>
            {
                if (isOn) _cardLogic.TempRule = DeckRule.THREE_RULE;
            });

            // ── 按钮绑定（Find = 递归查找，避免 Prefab 实例化后 Inspector 引用丢失）──
            var gameManager = this.FindInScene<GameManager>();
            if (gameManager != null)
            {
                ComponentFinder.Find<Button>(transform, "NewGameButton")?.onClick.AddListener(gameManager.OnClickModalRandom);
                ComponentFinder.Find<Button>(transform, "ReplayButton")?.onClick.AddListener(gameManager.OnClickModalReplay);
                ComponentFinder.Find<Button>(transform, "CloseButtonField")?.onClick.AddListener(gameManager.OnClickModalClose);
                ComponentFinder.Find<Button>(transform, "BGBlocker")?.onClick.AddListener(gameManager.OnClickModalClose);
            }
        }

        protected override void OnLayerShow()
        {
            // 弹窗打开时将 Toggle 同步到当前规则（不触发 onValueChanged）
            _oneRuleToggle?.SetIsOnWithoutNotify(_cardLogic.CurrentRule == DeckRule.ONE_RULE);
            _threeRuleToggle?.SetIsOnWithoutNotify(_cardLogic.CurrentRule == DeckRule.THREE_RULE);
            _cardLogic.TempRule = _cardLogic.CurrentRule;

            base.OnLayerShow(); // 触发 OnLayerShown 事件
        }
    }
}
