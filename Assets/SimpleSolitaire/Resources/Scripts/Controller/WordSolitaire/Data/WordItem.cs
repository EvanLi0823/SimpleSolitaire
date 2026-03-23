using UnityEngine;

namespace SimpleSolitaire.Controller.WordSolitaire
{
    /// <summary>
    /// 单词项数据结构 - 定义单个单词卡牌的数据
    /// </summary>
    [System.Serializable]
    public class WordItem
    {
        [Header("基础信息")]
        [Tooltip("单词唯一ID，格式: categoryId_序号")]
        public string WordId;
        
        [Tooltip("所属词组ID（数字），关联CategoryData")]
        public int CategoryId;
        
        [Header("文本")]
        [Tooltip("单词文本的多语言Key，格式: word_{wordId}")]
        public string TextKey;
        
        [Header("类型")]
        [Tooltip("卡牌类型: Text(文字)/Image(图片)/Joker(万能卡)")]
        public CardType CardType;
        
        [Header("图片")]
        [Tooltip("图片资源，仅CardType=Image时使用")]
        public Sprite Image;
        
        /// <summary>
        /// 数据验证 - 检查字段完整性
        /// </summary>
        /// <returns>验证结果</returns>
        public bool Validate()
        {
            if (string.IsNullOrEmpty(WordId))
            {
                Debug.LogError($"[WordItem] WordId不能为空");
                return false;
            }
            
            if (CategoryId <= 0)
            {
                Debug.LogError($"[WordItem] WordId={WordId} 的CategoryId必须大于0");
                return false;
            }
            
            if (string.IsNullOrEmpty(TextKey))
            {
                Debug.LogError($"[WordItem] WordId={WordId} 的TextKey不能为空");
                return false;
            }
            
            // 图片类型必须有关联图片
            if (CardType == CardType.Image && Image == null)
            {
                Debug.LogWarning($"[WordItem] WordId={WordId} 是图片类型但未设置Image");
            }
            
            return true;
        }
    }
}
