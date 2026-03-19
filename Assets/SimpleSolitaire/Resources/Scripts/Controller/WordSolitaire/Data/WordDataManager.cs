using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimpleSolitaire.Controller.WordSolitaire
{
    /// <summary>
    /// 单词数据项
    /// </summary>
    [System.Serializable]
    public class WordItem
    {
        public string WordId;                  // 单词ID
        public string CategoryId;              // 所属类别ID
        public string TextKey;                 // 多语言Key
        public CardVisualType CardType;        // 卡牌类型
        public Sprite Image;                   // 图片（仅图片卡使用）
    }
    
    /// <summary>
    /// 词库类别数据
    /// </summary>
    public class WordCategoryData : ScriptableObject
    {
        public string CategoryId;                              // 类别ID
        public string NameKey;                                 // 多语言Key
        public Sprite Icon;                                    // 类别图标
        public List<WordItem> Words;                           // 词汇列表
    }
    
    /// <summary>
    /// 词库数据管理器
    /// </summary>
    public class WordDataManager : MonoBehaviour
    {
        private Dictionary<string, WordCategoryData> _categories = new Dictionary<string, WordCategoryData>();
        
        /// <summary>
        /// 获取指定类别的词库数据
        /// </summary>
        public WordCategoryData GetCategory(string categoryId)
        {
            if (_categories.ContainsKey(categoryId))
            {
                return _categories[categoryId];
            }
            
            // 从Resources加载
            string path = $"Data/WordSolitaire/Words/Category_{categoryId}";
            var category = Resources.Load<WordCategoryData>(path);
            
            if (category != null)
            {
                _categories[categoryId] = category;
            }
            else
            {
                Debug.LogWarning($"[WordDataManager] 词库类别不存在: {path}");
            }
            
            return category;
        }
        
        /// <summary>
        /// 获取指定类别的所有单词
        /// </summary>
        public List<WordItem> GetWordsByCategory(string categoryId)
        {
            var category = GetCategory(categoryId);
            return category?.Words ?? new List<WordItem>();
        }
        
        /// <summary>
        /// 获取随机单词
        /// </summary>
        public WordItem GetRandomWord(string categoryId)
        {
            var words = GetWordsByCategory(categoryId);
            if (words.Count == 0) return null;
            
            int index = Random.Range(0, words.Count);
            return words[index];
        }
        
        /// <summary>
        /// 预加载指定类别的词库
        /// </summary>
        public void PreloadCategory(string categoryId)
        {
            GetCategory(categoryId);
        }
        
        /// <summary>
        /// 清除缓存
        /// </summary>
        public void ClearCache()
        {
            _categories.Clear();
        }
    }
}
