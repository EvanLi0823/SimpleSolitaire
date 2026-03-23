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
        public int CategoryId;              // 所属类别ID
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
        [SerializeField] private Text _categoryNameText;              // 分类名称文本（显示类别名称）
        
        // 公开访问属性
        public Text WordText => _wordText;
        public Image WordImage => _wordImage;
        public GameObject JokerIcon => _jokerIcon;
        
        // 卡牌正面显示状态
        private bool _isFaceUp = false;
        
        /// <summary>
        /// 是否正面朝上
        /// </summary>
        public bool IsFaceUp => _isFaceUp;
        
        /// <summary>
        /// 是否是万能卡
        /// </summary>
        public bool IsJoker => WordCardType == global::SimpleSolitaire.Controller.WordSolitaire.CardType.Joker;
        
        /// <summary>
        /// 是否是分类卡
        /// </summary>
        public bool IsCategoryCard => WordCardType == global::SimpleSolitaire.Controller.WordSolitaire.CardType.CategoryCard;
          
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
                    
                    // 查找分类名称文本
                    _categoryNameText = this.Get<Text>("Front/FrontCategories/CategoryName");
                    
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
                case global::SimpleSolitaire.Controller.WordSolitaire.CardType.CategoryCard:
                    ShowCategoryCard();
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
        /// 显示分类卡
        /// </summary>
        private void ShowCategoryCard()
        {
            // 显示分类图标（左上角标识）
            if (_categoryIcon != null)
            {
                _categoryIcon.gameObject.SetActive(true);
            }
            
            // 显示分类计数（右上角0/N格式）
            if (_categoryCountText != null)
            {
                _categoryCountText.gameObject.SetActive(true);
                // 格式: "0/N" - 当前收集数量/目标数量
                // 这里使用默认目标值，实际由CategorySlot更新
                _categoryCountText.text = $"0/5"; 
            }
            
            // 显示分类名称（中间文本）
            if (_categoryNameText != null)
            {
                _categoryNameText.gameObject.SetActive(true);
                // 显示类别ID对应的名称（实际应从Categories数据获取）
                _categoryNameText.text = GetCategoryName(CategoryId);
            }
            
            // 显示分类卡正面图片
            if (_frontCategoriesImage != null)
            {
                _frontCategoriesImage.gameObject.SetActive(true);
            }
        }
        
        /// <summary>
        /// 获取分类名称
        /// </summary>
        private string GetCategoryName(int categoryId)
        {
            // 实际应从CategoryData获取，这里返回默认名称
            return $"类别{categoryId}";
        }
        
        /// <summary>
        /// 设置卡牌正反面显示
        /// </summary>
        /// <param name="faceUp">true为正面，false为背面</param>
        public override void SetCardFace(bool faceUp)
        {
            _isFaceUp = faceUp;
            
            if (faceUp)
            {
                // 显示正面容器
                if (_frontContainer != null)
                {
                    _frontContainer.SetActive(true);
                }
                // 更新卡牌视觉
                UpdateCardVisual();
            }
            else
            {
                // 隐藏正面容器
                if (_frontContainer != null)
                {
                    _frontContainer.SetActive(false);
                }
                // 显示卡背
                RestoreBackView();
            }
        }
        
        /// <summary>
        /// 更新分类计数显示
        /// </summary>
        /// <param name="current">当前收集数量</param>
        /// <param name="target">目标数量</param>
        public void UpdateCategoryCount(int current, int target)
        {
            if (_categoryCountText != null)
            {
                _categoryCountText.text = $"{current}/{target}";
            }
        }
        
        /// <summary>
        /// 更新分类名称显示
        /// </summary>
        /// <param name="name">分类名称</param>
        public void UpdateCategoryName(string name)
        {
            if (_categoryNameText != null)
            {
                _categoryNameText.text = name;
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
