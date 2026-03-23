using System.Collections.Generic;
using UnityEngine;

namespace SimpleSolitaire.Controller.WordSolitaire
{
    /// <summary>
    /// 词库类别数据 - ScriptableObject，定义词组类别及其包含的单词
    /// </summary>
    [CreateAssetMenu(fileName = "WordCategoryData", menuName = "WordSolitaire/Word Category Data")]
    public class WordCategoryData : ScriptableObject
    {
        [Header("基础信息")]
        [Tooltip("词组唯一ID（数字），作为主键使用")]
        public int CategoryId;
        
        [Tooltip("词组英文名称，用于显示")]
        public string CategoryName;
        
        [Tooltip("词组名称的多语言Key，格式: category_{CategoryId}")]
        public string NameKey;
        
        [Header("视觉")]
        [Tooltip("类别图标")]
        public Sprite Icon;
        
        [Header("单词列表")]
        [Tooltip("该类别包含的所有单词")]
        public List<WordItem> Words = new List<WordItem>();
        
        /// <summary>
        /// 数据验证 - 检查字段完整性
        /// </summary>
        /// <returns>验证结果</returns>
        public bool Validate()
        {
            bool isValid = true;
            
            if (CategoryId <= 0)
            {
                Debug.LogError($"[WordCategoryData] CategoryId必须大于0");
                isValid = false;
            }
            
            if (string.IsNullOrEmpty(NameKey))
            {
                Debug.LogError($"[WordCategoryData] CategoryId={CategoryId} 的NameKey不能为空");
                isValid = false;
            }
            
            if (Icon == null)
            {
                Debug.LogWarning($"[WordCategoryData] CategoryId={CategoryId} 未设置Icon图标");
            }
            
            if (Words == null || Words.Count == 0)
            {
                Debug.LogWarning($"[WordCategoryData] CategoryId={CategoryId} 的Words列表为空");
            }
            else
            {
                // 验证每个单词项
                foreach (var word in Words)
                {
                    if (word != null && !word.Validate())
                    {
                        isValid = false;
                    }
                }
            }
            
            return isValid;
        }
        
        /// <summary>
        /// 根据单词ID获取单词项
        /// </summary>
        /// <param name="wordId">单词ID</param>
        /// <returns>单词项，未找到返回null</returns>
        public WordItem GetWordById(string wordId)
        {
            if (Words == null || string.IsNullOrEmpty(wordId))
                return null;
                
            return Words.Find(w => w != null && w.WordId == wordId);
        }
        
        /// <summary>
        /// 获取该类别下的单词数量
        /// </summary>
        /// <returns>单词数量</returns>
        public int GetWordCount()
        {
            return Words != null ? Words.Count : 0;
        }

        /// <summary>
        /// 生成分类卡数据 - 用于创建代表该类别的分类卡
        /// </summary>
        /// <returns>分类卡单词项</returns>
        public WordItem CreateCategoryCardItem()
        {
            return new WordItem
            {
                WordId = $"CATEGORY_{CategoryId}",
                CategoryId = CategoryId,
                CardType = CardType.CategoryCard,
                TextKey = NameKey,
                Image = Icon
            };
        }
    }
}
