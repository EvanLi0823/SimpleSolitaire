using UnityEngine;
using UnityEngine.UI;
using SimpleSolitaire.Utility;

namespace SimpleSolitaire.Controller.WordSolitaire
{
    /// <summary>
    /// Word Solitaire 词语卡牌
    /// 继承自Card基类，实现词语卡牌的显示和交互
    /// </summary>
    public class WordSolitaireCard : Card
    {
        [Header("词语卡牌属性")]
        public string WordId;                  // 单词ID
        public string CategoryId;              // 所属类别ID
        public CardType WordCardType;          // 卡牌类型（Text/Image/Joker）
        public Sprite WordImageSprite;         // 单词图片（仅图片卡使用）
        
        [Header("UI组件")]
        [SerializeField] private GameObject _frontContainer;          // 正面容器（Front）
        [SerializeField] private Image _frontNormalImage;             // 普通卡正面图片
        [SerializeField] private Image _frontCategoriesImage;         // 分类卡正面图片
        [SerializeField] private Image _frontJokerImage;              // 万能卡正面图片（注意：预制体中拼写为Jocker）
        [SerializeField] private Text _wordText;                      // 单词文本显示
        [SerializeField] private Image _wordImage;                    // 单词图片显示组件
        [SerializeField] private GameObject _jokerIcon;               // 万能卡标识
        [SerializeField] private Image _categoryIcon;                 // 分类图标（仅分类卡使用）
        [SerializeField] private Text _categoryCountText;             // 分类计数文本（显示如"1/5"）
        
        // 公开访问属性
        public Text WordText => _wordText;
        public Image WordImage => _wordImage;
        public GameObject JokerIcon => _jokerIcon;
        
        /// <summary>
        /// 是否是万能卡
        /// </summary>
        public bool IsJoker => WordCardType == global::SimpleSolitaire.Controller.WordSolitaire.CardType.Joker;
          
        protected void Awake()
        {
            // 使用ComponentFinder自动查找BackgroundImage（如果未配置）
            if (BackgroundImage == null)
            {
                BackgroundImage = this.Get<Image>("Background");
            }
            
            // 使用ComponentFinder自动查找Front相关子节点（如果未配置）
            if (_frontContainer == null)
            {
                _frontContainer = this.Get<Transform>("Front")?.gameObject;
                
                if (_frontContainer != null)
                {
                    // 查找Front下的各个子节点
                    _frontNormalImage = this.Get<Image>("Front/FrontNormal");
                    _frontCategoriesImage = this.Get<Image>("Front/FrontCategories");
                    _frontJokerImage = this.Get<Image>("Front/FrontJocker");
                    _wordText = this.Get<Text>("Front/Text");
                    _jokerIcon = this.Get<Transform>("Front/FrontJocker")?.gameObject;
                    _categoryIcon = this.Get<Image>("Front/Icon");
                    
                    // 查找分类计数文本（在FrontCategories下）
                    _categoryCountText = this.Get<Text>("Front/FrontCategories/CategoriesCount");
                    
                    // wordImage 默认指向 FrontNormal，实际使用时根据卡牌类型动态设置
                    if (_frontNormalImage != null)
                    {
                        _wordImage = _frontNormalImage;
                    }
                }
            }
        }
        
        /// <summary>
        /// 初始化卡牌
        /// </summary>
        public override void InitWithNumber(int cardNum)
        {
            CardNumber = cardNum;
            Number = cardNum;
            UpdateCardVisual();
        }
        
        /// <summary>
        /// 使用WordItem数据初始化
        /// </summary>
        public void InitWithWordItem(WordItem wordItem)
        {
            if (wordItem == null) return;
            
            WordId = wordItem.WordId;
            CategoryId = wordItem.CategoryId;
            WordCardType = wordItem.CardType;
            WordImageSprite = wordItem.Image;
            
            UpdateCardVisual();
        }
        
        /// <summary>
        /// 更新卡牌视觉显示
        /// </summary>
        private void UpdateCardVisual()
        {
            // 重置所有显示元素
            if (WordText != null) WordText.gameObject.SetActive(false);
            if (WordImage != null) WordImage.gameObject.SetActive(false);
            if (JokerIcon != null) JokerIcon.SetActive(false);
            
            switch (WordCardType)
            {
                case global::SimpleSolitaire.Controller.WordSolitaire.CardType.Text:
                    ShowTextCard();
                    break;
                case global::SimpleSolitaire.Controller.WordSolitaire.CardType.Image:
                    ShowImageCard();
                    break;
                case global::SimpleSolitaire.Controller.WordSolitaire.CardType.Joker:
                    ShowJokerCard();
                    break;
            }
        }
        
        /// <summary>
        /// 显示文字卡牌
        /// </summary>
        private void ShowTextCard()
        {
            if (WordText != null)
            {
                WordText.text = WordId; // 实际应使用本地化文本
                WordText.gameObject.SetActive(true);
            }
        }
        
        /// <summary>
        /// 显示图片卡牌
        /// </summary>
        private void ShowImageCard()
        {
            if (WordImage != null && WordImageSprite != null)
            {
                WordImage.sprite = WordImageSprite;
                WordImage.gameObject.SetActive(true);
            }
        }
        
        /// <summary>
        /// 显示万能卡
        /// </summary>
        private void ShowJokerCard()
        {
            if (JokerIcon != null)
            {
                JokerIcon.SetActive(true);
            }
        }
        
        /// <summary>
        /// 获取卡牌类型名称（用于纹理加载）
        /// </summary>
        public override string GetTypeName()
        {
            return $"Word_{CategoryId}_{WordId}";
        }
        
        /// <summary>
        /// 双击卡牌时的处理
        /// </summary>
        protected override void OnTapToPlace()
        {
            // Word Solitaire中双击卡牌尝试自动匹配
            if (CardLogicComponent is WordSolitaireCardLogic wordLogic)
            {
                wordLogic.TryAutoMatchCard(this);
            }
        }
    }
}
