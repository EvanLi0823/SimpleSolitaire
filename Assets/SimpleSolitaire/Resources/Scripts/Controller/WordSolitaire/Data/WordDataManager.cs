using System.Collections.Generic;
using UnityEngine;

namespace SimpleSolitaire.Controller.WordSolitaire
{
    /// <summary>
    /// 词库数据管理器 - 负责加载和管理所有词库类别数据
    /// </summary>
    public class WordDataManager : MonoBehaviour
    {
        // ── 单例模式 ──────────────────────────────────────────────────────────
        public static WordDataManager Instance { get; private set; }

        // ── Inspector配置 ────────────────────────────────────────────────────
        [Header("数据路径配置")]
        [SerializeField] private string _categoriesPath = "Data/WordSolitaire/Categories";

        // ── 内部数据结构 ──────────────────────────────────────────────────────
        /// <summary>词库类别缓存字典：Key = categoryId, Value = WordCategoryData</summary>
        private Dictionary<string, WordCategoryData> _categoriesCache;

        /// <summary>是否已完成数据加载</summary>
        private bool _isLoaded;

        // ── Unity生命周期 ─────────────────────────────────────────────────────
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            _categoriesCache = new Dictionary<string, WordCategoryData>();
            _isLoaded = false;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        // ── 公开API ───────────────────────────────────────────────────────────

        /// <summary>
        /// 预加载所有词库类别数据（建议在场景启动时调用）
        /// </summary>
        public void PreloadAllCategories()
        {
            if (_isLoaded) return;

            var categories = Resources.LoadAll<WordCategoryData>(_categoriesPath);
            
            foreach (var category in categories)
            {
                if (category != null && !string.IsNullOrEmpty(category.CategoryId))
                {
                    _categoriesCache[category.CategoryId] = category;
                }
            }

            _isLoaded = true;
            Debug.Log($"[WordDataManager] 已加载 {_categoriesCache.Count} 个词库类别");
        }

        /// <summary>
        /// 根据类别ID获取词库数据
        /// </summary>
        /// <param name="categoryId">类别ID</param>
        /// <returns>词库类别数据，不存在返回null</returns>
        public WordCategoryData GetCategoryById(string categoryId)
        {
            if (!_isLoaded)
            {
                PreloadAllCategories();
            }

            if (_categoriesCache.TryGetValue(categoryId, out var category))
            {
                return category;
            }

            // 尝试动态加载
            var dynamicCategory = LoadCategoryDynamic(categoryId);
            if (dynamicCategory != null)
            {
                _categoriesCache[categoryId] = dynamicCategory;
                return dynamicCategory;
            }

            Debug.LogWarning($"[WordDataManager] 未找到类别: {categoryId}");
            return null;
        }

        /// <summary>
        /// 获取多个类别的词库数据
        /// </summary>
        /// <param name="categoryIds">类别ID数组</param>
        /// <returns>词库类别数据列表</returns>
        public List<WordCategoryData> GetCategoriesByIds(string[] categoryIds)
        {
            var result = new List<WordCategoryData>();
            
            if (categoryIds == null) return result;

            foreach (var id in categoryIds)
            {
                var category = GetCategoryById(id);
                if (category != null)
                {
                    result.Add(category);
                }
            }

            return result;
        }

        /// <summary>
        /// 获取所有已加载的类别数据
        /// </summary>
        public List<WordCategoryData> GetAllCategories()
        {
            if (!_isLoaded)
            {
                PreloadAllCategories();
            }

            return new List<WordCategoryData>(_categoriesCache.Values);
        }

        /// <summary>
        /// 检查类别是否存在
        /// </summary>
        public bool HasCategory(string categoryId)
        {
            if (!_isLoaded)
            {
                PreloadAllCategories();
            }

            return _categoriesCache.ContainsKey(categoryId);
        }

        /// <summary>
        /// 获取类别数量
        /// </summary>
        public int GetCategoryCount()
        {
            if (!_isLoaded)
            {
                PreloadAllCategories();
            }

            return _categoriesCache.Count;
        }

        // ── 内部方法 ──────────────────────────────────────────────────────────

        /// <summary>
        /// 动态加载指定类别数据
        /// </summary>
        private WordCategoryData LoadCategoryDynamic(string categoryId)
        {
            string path = $"{_categoriesPath}/Category_{categoryId}";
            var category = Resources.Load<WordCategoryData>(path);
            
            if (category == null)
            {
                // 尝试其他命名格式
                path = $"{_categoriesPath}/{categoryId}";
                category = Resources.Load<WordCategoryData>(path);
            }

            return category;
        }

        /// <summary>
        /// 清除缓存（用于内存优化或数据重载）
        /// </summary>
        public void ClearCache()
        {
            _categoriesCache.Clear();
            _isLoaded = false;
        }
    }
}
