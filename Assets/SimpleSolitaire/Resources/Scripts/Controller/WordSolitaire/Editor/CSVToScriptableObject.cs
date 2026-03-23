using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SimpleSolitaire.Controller.WordSolitaire.Editor
{
    /// <summary>
    /// CSV转ScriptableObject编辑器工具
    /// 将CSV文件转换为ScriptableObject资源
    /// </summary>
    public class CSVToScriptableObject : EditorWindow
    {
        // CSV文件路径
        private string _wordsCSVPath = "Assets/SimpleSolitaire/Resources/Data/WordSolitaire/CSV/Words.csv";
        private string _levelsCSVPath = "Assets/SimpleSolitaire/Resources/Data/WordSolitaire/CSV/Levels.csv";
        private string _categoriesCSVPath = "Assets/SimpleSolitaire/Resources/Data/WordSolitaire/CSV/Categories.csv";
        private string _localizationCSVPath = "Assets/SimpleSolitaire/Resources/Data/WordSolitaire/CSV/Localization.csv";
        
        // 输出路径
        private string _outputPath = "Assets/SimpleSolitaire/Resources/Data/WordSolitaire/ScriptableObjects";
        
        // 日志
        private Vector2 _logScrollPosition;
        private List<string> _logs = new List<string>();
        private bool _showSuccessLogs = true;
        private bool _showWarningLogs = true;
        private bool _showErrorLogs = true;
        
        // 转换状态
        private bool _isConverting = false;
        private int _totalSteps = 0;
        private int _currentStep = 0;
        
        [MenuItem("Tools/WordSolitaire/Convert CSV to SO")]
        public static void ShowWindow()
        {
            GetWindow<CSVToScriptableObject>("CSV to SO Converter");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("WordSolitaire CSV 转换工具", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            // 文件路径设置
            EditorGUILayout.LabelField("CSV文件路径", EditorStyles.boldLabel);
            _categoriesCSVPath = EditorGUILayout.TextField("Categories.csv", _categoriesCSVPath);
            _wordsCSVPath = EditorGUILayout.TextField("Words.csv", _wordsCSVPath);
            _levelsCSVPath = EditorGUILayout.TextField("Levels.csv", _levelsCSVPath);
            _localizationCSVPath = EditorGUILayout.TextField("Localization.csv", _localizationCSVPath);
            
            EditorGUILayout.Space();
            
            // 输出路径
            EditorGUILayout.LabelField("输出路径", EditorStyles.boldLabel);
            _outputPath = EditorGUILayout.TextField("输出目录", _outputPath);
            
            EditorGUILayout.Space();
            
            // 按钮
            EditorGUILayout.BeginHorizontal();
            
            GUI.enabled = !_isConverting;
            
            if (GUILayout.Button("转换所有", GUILayout.Height(30)))
            {
                ConvertAll();
            }
            
            if (GUILayout.Button("仅转换词汇", GUILayout.Height(30)))
            {
                ConvertWordsOnly();
            }
            
            if (GUILayout.Button("仅转换关卡", GUILayout.Height(30)))
            {
                ConvertLevelsOnly();
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("验证数据", GUILayout.Height(25)))
            {
                ValidateAll();
            }
            
            if (GUILayout.Button("清空日志", GUILayout.Height(25)))
            {
                _logs.Clear();
            }
            
            GUI.enabled = true;
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            // 进度显示
            if (_isConverting)
            {
                float progress = _totalSteps > 0 ? (float)_currentStep / _totalSteps : 0f;
                EditorGUILayout.LabelField($"转换进度: {_currentStep}/{_totalSteps} ({progress:P0})");
                EditorGUILayout.Space();
            }
            
            // 日志过滤器
            EditorGUILayout.LabelField("日志显示", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            _showSuccessLogs = EditorGUILayout.Toggle("成功", _showSuccessLogs);
            _showWarningLogs = EditorGUILayout.Toggle("警告", _showWarningLogs);
            _showErrorLogs = EditorGUILayout.Toggle("错误", _showErrorLogs);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space();
            
            // 日志显示区域
            EditorGUILayout.LabelField("转换日志", EditorStyles.boldLabel);
            
            _logScrollPosition = EditorGUILayout.BeginScrollView(_logScrollPosition, 
                GUILayout.Height(200));
            
            foreach (var log in _logs)
            {
                // 根据日志类型过滤
                if (log.StartsWith("[成功]") && !_showSuccessLogs) continue;
                if (log.StartsWith("[警告]") && !_showWarningLogs) continue;
                if (log.StartsWith("[错误]") && !_showErrorLogs) continue;
                
                // 根据类型设置颜色
                if (log.StartsWith("[成功]"))
                {
                    GUI.color = Color.green;
                }
                else if (log.StartsWith("[警告]"))
                {
                    GUI.color = Color.yellow;
                }
                else if (log.StartsWith("[错误]"))
                {
                    GUI.color = Color.red;
                }
                else
                {
                    GUI.color = Color.white;
                }
                
                EditorGUILayout.LabelField(log, EditorStyles.wordWrappedLabel);
                GUI.color = Color.white;
            }
            
            EditorGUILayout.EndScrollView();
        }
        
        /// <summary>
        /// 转换所有CSV文件
        /// </summary>
        private void ConvertAll()
        {
            _logs.Clear();
            _isConverting = true;
            _currentStep = 0;
            _totalSteps = 3; // Categories, Words, Levels
            
            try
            {
                // 确保输出目录存在
                EnsureDirectoryExists(_outputPath);
                EnsureDirectoryExists($"{_outputPath}/Categories");
                EnsureDirectoryExists($"{_outputPath}/Words");
                EnsureDirectoryExists($"{_outputPath}/Levels");
                
                // 1. 转换类别
                _currentStep++;
                var categories = ConvertCategories();
                
                // 2. 转换词汇
                _currentStep++;
                ConvertWords(categories);
                
                // 3. 转换关卡
                _currentStep++;
                ConvertLevels(categories);
                
                // 保存资源
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                
                LogSuccess("所有数据转换完成！");
            }
            catch (Exception e)
            {
                LogError($"转换过程中发生错误: {e.Message}\n{e.StackTrace}");
            }
            finally
            {
                _isConverting = false;
            }
        }
        
        /// <summary>
        /// 仅转换词汇
        /// </summary>
        private void ConvertWordsOnly()
        {
            _logs.Clear();
            _isConverting = true;
            _currentStep = 0;
            _totalSteps = 2;

            try
            {
                EnsureDirectoryExists(_outputPath);
                EnsureDirectoryExists($"{_outputPath}/Words");

                _currentStep++;
                // 先转换类别，获取Dictionary
                var categoryDict = ConvertCategories();

                _currentStep++;
                ConvertWords(categoryDict);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                LogSuccess("词汇数据转换完成！");
            }
            catch (Exception e)
            {
                LogError($"转换过程中发生错误: {e.Message}");
            }
            finally
            {
                _isConverting = false;
            }
        }
        
        /// <summary>
        /// 仅转换关卡
        /// </summary>
        private void ConvertLevelsOnly()
        {
            _logs.Clear();
            _isConverting = true;
            _currentStep = 0;
            _totalSteps = 2;

            try
            {
                EnsureDirectoryExists(_outputPath);
                EnsureDirectoryExists($"{_outputPath}/Levels");

                _currentStep++;
                // 先转换类别，获取Dictionary
                var categoryDict = ConvertCategories();

                _currentStep++;
                ConvertLevels(categoryDict);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                LogSuccess("关卡数据转换完成！");
            }
            catch (Exception e)
            {
                LogError($"转换过程中发生错误: {e.Message}");
            }
            finally
            {
                _isConverting = false;
            }
        }
        
        /// <summary>
        /// 验证所有数据
        /// </summary>
        private void ValidateAll()
        {
            _logs.Clear();
            LogInfo("开始验证数据...");
            
            try
            {
                // 验证文件存在性
                if (!File.Exists(_wordsCSVPath))
                {
                    LogError($"Words.csv文件不存在: {_wordsCSVPath}");
                    return;
                }
                
                if (!File.Exists(_levelsCSVPath))
                {
                    LogError($"Levels.csv文件不存在: {_levelsCSVPath}");
                    return;
                }
                
                if (!File.Exists(_categoriesCSVPath))
                {
                    LogError($"Categories.csv文件不存在: {_categoriesCSVPath}");
                    return;
                }
                
                if (!File.Exists(_localizationCSVPath))
                {
                    LogError($"Localization.csv文件不存在: {_localizationCSVPath}");
                    return;
                }
                
                LogSuccess("所有CSV文件存在性验证通过");
                
                // 加载并验证数据
                var categories = LoadCategoriesFromCSV();
                var words = LoadWordsFromCSV();
                var levels = LoadLevelsFromCSV();
                
                // 验证关联关系
                ValidateRelationships(categories, words, levels);
                
                LogSuccess("数据验证完成！");
            }
            catch (Exception e)
            {
                LogError($"验证过程中发生错误: {e.Message}");
            }
        }
        
        #region 类别转换
        
        /// <summary>
        /// 转换类别数据
        /// </summary>
        private Dictionary<int, WordCategoryData> ConvertCategories()
        {
            LogInfo("开始转换类别数据...");
            
            var categories = LoadCategoriesFromCSV();
            var categoryDict = new Dictionary<int, WordCategoryData>();
            
            foreach (var category in categories)
            {
                // 创建ScriptableObject
                var asset = CreateInstance<WordCategoryData>();
                asset.CategoryId = category.CategoryId;
                asset.CategoryName = category.CategoryName;
                asset.NameKey = category.NameKey;
                asset.Words = new System.Collections.Generic.List<WordItem>();
                
                // 保存资源
                string assetPath = $"{_outputPath}/Categories/{category.CategoryId}.asset";
                AssetDatabase.CreateAsset(asset, assetPath);
                
                categoryDict[category.CategoryId] = asset;
                LogSuccess($"创建类别: {category.CategoryId} ({category.CategoryName})");
            }
            
            LogSuccess($"类别数据转换完成，共{categories.Count}个类别");
            return categoryDict;
        }
        
        /// <summary>
        /// 从CSV加载类别数据
        /// CSV格式: categoryId,categoryName,nameKey
        /// </summary>
        private List<CategoryCSVData> LoadCategoriesFromCSV()
        {
            var list = new List<CategoryCSVData>();
            
            if (!File.Exists(_categoriesCSVPath))
            {
                LogError($"Categories.csv文件不存在: {_categoriesCSVPath}");
                return list;
            }
            
            try
            {
                string[] lines = File.ReadAllLines(_categoriesCSVPath);
                if (lines.Length < 2)
                {
                    LogWarning("Categories.csv文件为空或格式不正确");
                    return list;
                }
                
                // 解析表头
                string[] headers = ParseCSVLine(lines[0]);
                
                for (int i = 1; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i])) continue;
                    
                    string[] values = ParseCSVLine(lines[i]);
                    if (values.Length < 3) continue;
                    
                    var data = new CategoryCSVData
                    {
                        CategoryId = int.Parse(values[0]),
                        CategoryName = values[1],
                        NameKey = values[2]
                    };
                    
                    list.Add(data);
                }
                
                LogSuccess($"从CSV加载了{list.Count}个类别");
            }
            catch (Exception e)
            {
                LogError($"加载Categories.csv失败: {e.Message}");
            }
            
            return list;
        }
        
        #endregion
        
        #region 词汇转换
        
        /// <summary>
        /// 转换词汇数据
        /// </summary>
        private void ConvertWords(Dictionary<int, WordCategoryData> categories)
        {
            LogInfo("开始转换词汇数据...");
            
            var words = LoadWordsFromCSV();
            int successCount = 0;
            int failCount = 0;
            
            foreach (var word in words)
            {
                // 验证类别存在
                if (!categories.ContainsKey(word.CategoryId))
                {
                    LogError($"词汇 {word.WordId} 引用了不存在的类别: {word.CategoryId}");
                    failCount++;
                    continue;
                }
                
                // 创建WordItem
                var wordItem = new WordItem
                {
                    WordId = word.WordId,
                    CategoryId = word.CategoryId,
                    TextKey = word.TextKey,
                    CardType = ParseCardType(word.CardType),
                    Image = null
                };
                
                // 添加到对应类别
                categories[word.CategoryId].Words.Add(wordItem);
                successCount++;
                
                EditorUtility.SetDirty(categories[word.CategoryId]);
            }
            
            LogSuccess($"词汇数据转换完成，成功:{successCount}，失败:{failCount}");
        }
        
        /// <summary>
        /// 从CSV加载词汇数据
        /// CSV格式: wordId,categoryId,textKey,cardType,imagePath
        /// </summary>
        private List<WordCSVData> LoadWordsFromCSV()
        {
            var list = new List<WordCSVData>();
            
            if (!File.Exists(_wordsCSVPath))
            {
                LogError($"Words.csv文件不存在: {_wordsCSVPath}");
                return list;
            }
            
            try
            {
                string[] lines = File.ReadAllLines(_wordsCSVPath);
                if (lines.Length < 2)
                {
                    LogWarning("Words.csv文件为空或格式不正确");
                    return list;
                }
                
                for (int i = 1; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i])) continue;
                    
                    string[] values = ParseCSVLine(lines[i]);
                    if (values.Length < 4) continue;
                    
                    var data = new WordCSVData
                    {
                        WordId = values[0],
                        CategoryId = int.Parse(values[1]),
                        TextKey = values[2],
                        CardType = values[3],
                        ImagePath = values.Length > 4 ? values[4] : ""
                    };
                    
                    list.Add(data);
                }
                
                LogSuccess($"从CSV加载了{list.Count}个词汇");
            }
            catch (Exception e)
            {
                LogError($"加载Words.csv失败: {e.Message}");
            }
            
            return list;
        }
        
        #endregion
        
        #region 关卡转换
        
        /// <summary>
        /// 转换关卡数据
        /// </summary>
        private void ConvertLevels(Dictionary<int, WordCategoryData> categories)
        {
            LogInfo("开始转换关卡数据...");
            
            var levels = LoadLevelsFromCSV();
            int successCount = 0;
            int failCount = 0;
            
            foreach (var level in levels)
            {
                // 验证类别
                bool categoriesValid = true;
                foreach (var catId in level.CategoryIds)
                {
                    if (!categories.ContainsKey(catId))
                    {
                        LogError($"关卡 {level.LevelId} 引用了不存在的类别: {catId}");
                        categoriesValid = false;
                    }
                }
                
                if (!categoriesValid)
                {
                    failCount++;
                    continue;
                }
                
                // 创建ScriptableObject
                var asset = CreateInstance<LevelData>();
                asset.LevelId = level.LevelId;
                asset.CardCount = level.CardCount;
                asset.MaxMoves = level.MaxMoves;
                asset.ColumnCount = level.ColumnCount;
                asset.SlotCount = level.SlotCount;
                asset.CategoryIds = level.CategoryIds;
                asset.InitialCardsPerColumn = level.InitialCardsPerColumn;
                asset.IsTutorial = level.IsTutorial;
                asset.IsShowResultAd = level.IsShowResultAd;
                asset.IsShowMatchAd = level.IsShowMatchAd;
                
                // 保存资源
                string assetPath = $"{_outputPath}/Levels/Level{level.LevelId}.asset";
                AssetDatabase.CreateAsset(asset, assetPath);
                
                successCount++;
                LogSuccess($"创建关卡: Level{level.LevelId}");
            }
            
            LogSuccess($"关卡数据转换完成，成功:{successCount}，失败:{failCount}");
        }
        
        /// <summary>
        /// 从CSV加载关卡数据
        /// CSV格式: levelId,cardCount,maxMoves,columnCount,slotCount,categoryIds,isTutorial,isShowResultAd,isShowMatchAd,initialCardsPerColumn
        /// </summary>
        private List<LevelCSVData> LoadLevelsFromCSV()
        {
            var list = new List<LevelCSVData>();
            
            if (!File.Exists(_levelsCSVPath))
            {
                LogError($"Levels.csv文件不存在: {_levelsCSVPath}");
                return list;
            }
            
            try
            {
                string[] lines = File.ReadAllLines(_levelsCSVPath);
                if (lines.Length < 2)
                {
                    LogWarning("Levels.csv文件为空或格式不正确");
                    return list;
                }
                
                for (int i = 1; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i])) continue;
                    
                    string[] values = ParseCSVLine(lines[i]);
                    if (values.Length < 9) continue;
                    
                    // 解析categoryIds (格式: 1001|1002|1004)
                    string[] categoryIdStrings = values[5].Split('|');
                    int[] categoryIds = new int[categoryIdStrings.Length];
                    for (int j = 0; j < categoryIdStrings.Length; j++)
                    {
                        int.TryParse(categoryIdStrings[j], out categoryIds[j]);
                    }
                    
                    // 解析initialCardsPerColumn (格式: 4|5|6)
                    int[] initialCardsPerColumn = null;
                    if (values.Length > 9 && !string.IsNullOrEmpty(values[9]))
                    {
                        string[] cardsStrings = values[9].Split('|');
                        initialCardsPerColumn = new int[cardsStrings.Length];
                        for (int j = 0; j < cardsStrings.Length; j++)
                        {
                            int.TryParse(cardsStrings[j], out initialCardsPerColumn[j]);
                        }
                    }
                    
                    var data = new LevelCSVData
                    {
                        LevelId = int.Parse(values[0]),
                        CardCount = int.Parse(values[1]),
                        MaxMoves = int.Parse(values[2]),
                        ColumnCount = int.Parse(values[3]),
                        SlotCount = int.Parse(values[4]),
                        CategoryIds = categoryIds,
                        InitialCardsPerColumn = initialCardsPerColumn,
                        IsTutorial = values[6].ToLower() == "true",
                        IsShowResultAd = values[7].ToLower() == "true",
                        IsShowMatchAd = values[8].ToLower() == "true"
                    };
                    
                    if (data.LevelId > 0)
                    {
                        list.Add(data);
                    }
                }
                
                LogSuccess($"从CSV加载了{list.Count}个关卡");
            }
            catch (Exception e)
            {
                LogError($"加载Levels.csv失败: {e.Message}");
            }
            
            return list;
        }
        
        #endregion
        
        #region 辅助方法
        
        /// <summary>
        /// 解析CSV行（处理引号内的逗号）
        /// </summary>
        private string[] ParseCSVLine(string line)
        {
            var result = new List<string>();
            bool inQuotes = false;
            string current = "";
            
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(current.Trim());
                    current = "";
                }
                else
                {
                    current += c;
                }
            }
            
            result.Add(current.Trim());
            return result.ToArray();
        }
        
        /// <summary>
        /// 解析卡牌类型
        /// </summary>
        private CardType ParseCardType(string typeStr)
        {
            switch (typeStr?.ToLower())
            {
                case "text": return CardType.Text;
                case "image": return CardType.Image;
                case "joker": return CardType.Joker;
                default: return CardType.Text;
            }
        }
        
        /// <summary>
        /// 确保目录存在
        /// </summary>
        private void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                LogInfo($"创建目录: {path}");
            }
        }
        
        /// <summary>
        /// 验证数据关联关系
        /// </summary>
        private void ValidateRelationships(List<CategoryCSVData> categories, 
            List<WordCSVData> words, List<LevelCSVData> levels)
        {
            var categoryIds = categories.Select(c => c.CategoryId).ToHashSet();
            
            // 验证词汇的类别
            foreach (var word in words)
            {
                if (!categoryIds.Contains(word.CategoryId))
                {
                    LogError($"词汇 {word.WordId} 引用了不存在的类别: {word.CategoryId}");
                }
            }
            
            // 验证关卡的类别
            foreach (var level in levels)
            {
                foreach (var catId in level.CategoryIds)
                {
                    if (!categoryIds.Contains(catId))
                    {
                        LogError($"关卡 {level.LevelId} 引用了不存在的类别: {catId}");
                    }
                }
            }
            
            LogSuccess("数据关联关系验证完成");
        }
        
        #endregion
        
        #region 日志方法
        
        private void LogInfo(string message)
        {
            _logs.Add($"[信息] {message}");
            Debug.Log($"[CSVToSO] {message}");
        }
        
        private void LogSuccess(string message)
        {
            _logs.Add($"[成功] {message}");
            Debug.Log($"[CSVToSO] {message}");
        }
        
        private void LogWarning(string message)
        {
            _logs.Add($"[警告] {message}");
            Debug.LogWarning($"[CSVToSO] {message}");
        }
        
        private void LogError(string message)
        {
            _logs.Add($"[错误] {message}");
            Debug.LogError($"[CSVToSO] {message}");
        }
        
        #endregion
        
        #region 数据结构
        
        private class CategoryCSVData
        {
            public int CategoryId;
            public string CategoryName;
            public string NameKey;
        }
        
        private class WordCSVData
        {
            public string WordId;
            public int CategoryId;
            public string TextKey;
            public string CardType;
            public string ImagePath;
        }
        
        private class LevelCSVData
        {
            public int LevelId;
            public int CardCount;
            public int MaxMoves;
            public int ColumnCount;
            public int SlotCount;
            public int[] CategoryIds;
            public int[] InitialCardsPerColumn;
            public bool IsTutorial;
            public bool IsShowResultAd;
            public bool IsShowMatchAd;
        }
        
        #endregion
    }
}
