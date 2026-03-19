using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SimpleSolitaire.Controller.WordSolitaire.Editor
{
    /// <summary>
    /// Excel转ScriptableObject编辑器工具
    /// 将Words.xlsx和Levels.xlsx转换为ScriptableObject资源
    /// </summary>
    public class ExcelToScriptableObject : EditorWindow
    {
        // Excel文件路径
        private string _wordsExcelPath = "Assets/SimpleSolitaire/Data/Words.xlsx";
        private string _levelsExcelPath = "Assets/SimpleSolitaire/Data/Levels.xlsx";
        private string _categoriesExcelPath = "Assets/SimpleSolitaire/Data/Categories.xlsx";
        
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
        
        [MenuItem("Tools/WordSolitaire/Convert Excel to SO")]
        public static void ShowWindow()
        {
            GetWindow<ExcelToScriptableObject>("Excel to SO Converter");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("WordSolitaire Excel 转换工具", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            
            // 文件路径设置
            EditorGUILayout.LabelField("Excel文件路径", EditorStyles.boldLabel);
            _wordsExcelPath = EditorGUILayout.TextField("Words.xlsx", _wordsExcelPath);
            _levelsExcelPath = EditorGUILayout.TextField("Levels.xlsx", _levelsExcelPath);
            _categoriesExcelPath = EditorGUILayout.TextField("Categories.xlsx", _categoriesExcelPath);
            
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
        /// 转换所有Excel文件
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
                var categoriesData = LoadCategoriesFromExcel();
                
                _currentStep++;
                // 将List转换为Dictionary
                var categoriesDict = new Dictionary<string, WordCategoryData>();
                foreach (var catData in categoriesData)
                {
                    var category = Resources.Load<WordCategoryData>("Data/WordSolitaire/Categories/" + catData.CategoryId);
                    if (category != null)
                    {
                        categoriesDict[catData.CategoryId] = category;
                    }
                }
                ConvertWords(categoriesDict);
                
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
                var categoriesData = LoadCategoriesFromExcel();
                
                _currentStep++;
                // 将List转换为Dictionary
                var categoriesDict = new Dictionary<string, WordCategoryData>();
                foreach (var catData in categoriesData)
                {
                    var category = Resources.Load<WordCategoryData>("Data/WordSolitaire/Categories/" + catData.CategoryId);
                    if (category != null)
                    {
                        categoriesDict[catData.CategoryId] = category;
                    }
                }
                ConvertLevels(categoriesDict);
                
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
                if (!File.Exists(_wordsExcelPath))
                {
                    LogError($"Words.xlsx文件不存在: {_wordsExcelPath}");
                    return;
                }
                
                if (!File.Exists(_levelsExcelPath))
                {
                    LogError($"Levels.xlsx文件不存在: {_levelsExcelPath}");
                    return;
                }
                
                if (!File.Exists(_categoriesExcelPath))
                {
                    LogError($"Categories.xlsx文件不存在: {_categoriesExcelPath}");
                    return;
                }
                
                LogSuccess("所有Excel文件存在性验证通过");
                
                // 加载并验证数据
                var categories = LoadCategoriesFromExcel();
                var words = LoadWordsFromExcel();
                var levels = LoadLevelsFromExcel();
                
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
        private Dictionary<string, WordCategoryData> ConvertCategories()
        {
            LogInfo("开始转换类别数据...");
            
            var categories = LoadCategoriesFromExcel();
            var categoryDict = new Dictionary<string, WordCategoryData>();
            
            foreach (var category in categories)
            {
                // 创建ScriptableObject
                var asset = CreateInstance<WordCategoryData>();
                asset.CategoryId = category.CategoryId;
                asset.NameKey = category.NameKey;
                asset.Icon = LoadSprite(category.IconPath);
                asset.Words = new System.Collections.Generic.List<WordItem>();
                
                // 保存资源
                string assetPath = $"{_outputPath}/Categories/{category.CategoryId}.asset";
                AssetDatabase.CreateAsset(asset, assetPath);
                
                categoryDict[category.CategoryId] = asset;
                LogSuccess($"创建类别: {category.CategoryId}");
            }
            
            LogSuccess($"类别数据转换完成，共{categories.Count}个类别");
            return categoryDict;
        }
        
        /// <summary>
        /// 从Excel加载类别数据
        /// </summary>
        private List<CategoryExcelData> LoadCategoriesFromExcel()
        {
            var list = new List<CategoryExcelData>();
            
            if (!File.Exists(_categoriesExcelPath))
            {
                LogError($"Categories.xlsx文件不存在: {_categoriesExcelPath}");
                return list;
            }
            
            try
            {
                DataTable table = ReadExcel(_categoriesExcelPath, "Categories");
                
                for (int i = 1; i < table.Rows.Count; i++) // 跳过表头
                {
                    var row = table.Rows[i];
                    
                    var data = new CategoryExcelData
                    {
                        CategoryId = GetCellString(row, 0),
                        NameKey = GetCellString(row, 1),
                        IconPath = GetCellString(row, 2),
                        Difficulty = GetCellString(row, 3),
                        DescriptionKey = GetCellString(row, 4)
                    };
                    
                    if (!string.IsNullOrEmpty(data.CategoryId))
                    {
                        list.Add(data);
                    }
                }
                
                LogSuccess($"从Excel加载了{list.Count}个类别");
            }
            catch (Exception e)
            {
                LogError($"加载Categories.xlsx失败: {e.Message}");
            }
            
            return list;
        }
        
        #endregion
        
        #region 词汇转换
        
        /// <summary>
        /// 转换词汇数据
        /// </summary>
        private void ConvertWords(Dictionary<string, WordCategoryData> categories)
        {
            LogInfo("开始转换词汇数据...");
            
            var words = LoadWordsFromExcel();
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
                    Image = LoadSprite(word.ImagePath)
                };
                
                // 添加到对应类别
                categories[word.CategoryId].Words.Add(wordItem);
                successCount++;
                
                EditorUtility.SetDirty(categories[word.CategoryId]);
            }
            
            LogSuccess($"词汇数据转换完成，成功:{successCount}，失败:{failCount}");
        }
        
        /// <summary>
        /// 从Excel加载词汇数据
        /// </summary>
        private List<WordExcelData> LoadWordsFromExcel()
        {
            var list = new List<WordExcelData>();
            
            if (!File.Exists(_wordsExcelPath))
            {
                LogError($"Words.xlsx文件不存在: {_wordsExcelPath}");
                return list;
            }
            
            try
            {
                DataTable table = ReadExcel(_wordsExcelPath, "Words");
                
                for (int i = 1; i < table.Rows.Count; i++)
                {
                    var row = table.Rows[i];
                    
                    var data = new WordExcelData
                    {
                        WordId = GetCellString(row, 0),
                        CategoryId = GetCellString(row, 1),
                        TextKey = GetCellString(row, 2),
                        CardType = GetCellString(row, 3),
                        ImagePath = GetCellString(row, 4),
                        HintKey = GetCellString(row, 5)
                    };
                    
                    if (!string.IsNullOrEmpty(data.WordId))
                    {
                        list.Add(data);
                    }
                }
                
                LogSuccess($"从Excel加载了{list.Count}个词汇");
            }
            catch (Exception e)
            {
                LogError($"加载Words.xlsx失败: {e.Message}");
            }
            
            return list;
        }
        
        #endregion
        
        #region 关卡转换
        
        /// <summary>
        /// 转换关卡数据
        /// </summary>
        private void ConvertLevels(Dictionary<string, WordCategoryData> categories)
        {
            LogInfo("开始转换关卡数据...");
            
            var levels = LoadLevelsFromExcel();
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
        /// 从Excel加载关卡数据
        /// </summary>
        private List<LevelExcelData> LoadLevelsFromExcel()
        {
            var list = new List<LevelExcelData>();
            
            if (!File.Exists(_levelsExcelPath))
            {
                LogError($"Levels.xlsx文件不存在: {_levelsExcelPath}");
                return list;
            }
            
            try
            {
                DataTable table = ReadExcel(_levelsExcelPath, "Levels");
                
                for (int i = 1; i < table.Rows.Count; i++)
                {
                    var row = table.Rows[i];
                    
                    var data = new LevelExcelData
                    {
                        LevelId = GetCellInt(row, 0),
                        CardCount = GetCellInt(row, 1),
                        MaxMoves = GetCellInt(row, 2),
                        ColumnCount = GetCellInt(row, 3),
                        SlotCount = GetCellInt(row, 4),
                        CategoryIds = GetCellString(row, 5)?.Split('|') ?? new string[0],
                        IsTutorial = GetCellBool(row, 6),
                        IsShowResultAd = GetCellBool(row, 7),
                        IsShowMatchAd = GetCellBool(row, 8)
                    };
                    
                    if (data.LevelId > 0)
                    {
                        list.Add(data);
                    }
                }
                
                LogSuccess($"从Excel加载了{list.Count}个关卡");
            }
            catch (Exception e)
            {
                LogError($"加载Levels.xlsx失败: {e.Message}");
            }
            
            return list;
        }
        
        #endregion
        
        #region 辅助方法
        
        /// <summary>
        /// 读取Excel文件
        /// </summary>
        private DataTable ReadExcel(string filePath, string sheetName)
        {
            // 使用ExcelDataReader读取Excel
            // 注意：需要安装ExcelDataReader包
            
            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                #if EXCEL_DATA_READER
                using (var reader = ExcelDataReader.ExcelReaderFactory.CreateReader(stream))
                {
                    DataSet result = ExcelDataReader.ExcelDataReaderExtensions.AsDataSet(reader);
                #else
                using (StreamReader reader = null)
                {
                    DataSet result = null;
                    Debug.LogError($"[ExcelToSO] ExcelDataReader库未安装，无法读取Excel文件。请安装ExcelDataReader包。");
                    return null;
                #endif
                    
                    if (result.Tables.Count > 0)
                    {
                        // 如果指定了sheet名，尝试找到对应的表
                        if (!string.IsNullOrEmpty(sheetName))
                        {
                            for (int i = 0; i < result.Tables.Count; i++)
                            {
                                if (result.Tables[i].TableName == sheetName)
                                {
                                    return result.Tables[i];
                                }
                            }
                        }
                        
                        // 返回第一个表
                        return result.Tables[0];
                    }
                }
            }
            
            return new DataTable();
        }
        
        /// <summary>
        /// 获取单元格字符串值
        /// </summary>
        private string GetCellString(DataRow row, int columnIndex)
        {
            if (columnIndex < 0 || columnIndex >= row.Table.Columns.Count)
                return string.Empty;
                
            var value = row[columnIndex];
            return value != null && value != DBNull.Value ? value.ToString().Trim() : string.Empty;
        }
        
        /// <summary>
        /// 获取单元格整数值
        /// </summary>
        private int GetCellInt(DataRow row, int columnIndex)
        {
            string str = GetCellString(row, columnIndex);
            if (int.TryParse(str, out int result))
                return result;
            return 0;
        }
        
        /// <summary>
        /// 获取单元格布尔值
        /// </summary>
        private bool GetCellBool(DataRow row, int columnIndex)
        {
            string str = GetCellString(row, columnIndex).ToLower();
            return str == "true" || str == "1" || str == "yes";
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
        /// 加载Sprite资源
        /// </summary>
        private Sprite LoadSprite(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;
                
            // 尝试从Resources加载
            string resourcePath = path.Replace("Resources/", "").Replace(".png", "").Replace(".jpg", "");
            Sprite sprite = Resources.Load<Sprite>(resourcePath);
            
            if (sprite == null)
            {
                // 尝试从AssetDatabase加载
                string fullPath = $"Assets/{path}";
                sprite = AssetDatabase.LoadAssetAtPath<Sprite>(fullPath);
            }
            
            return sprite;
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
        private void ValidateRelationships(List<CategoryExcelData> categories, 
            List<WordExcelData> words, List<LevelExcelData> levels)
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
            Debug.Log($"[ExcelToSO] {message}");
        }
        
        private void LogSuccess(string message)
        {
            _logs.Add($"[成功] {message}");
            Debug.Log($"[ExcelToSO] {message}");
        }
        
        private void LogWarning(string message)
        {
            _logs.Add($"[警告] {message}");
            Debug.LogWarning($"[ExcelToSO] {message}");
        }
        
        private void LogError(string message)
        {
            _logs.Add($"[错误] {message}");
            Debug.LogError($"[ExcelToSO] {message}");
        }
        
        #endregion
        
        #region 数据结构
        
        private class CategoryExcelData
        {
            public string CategoryId;
            public string NameKey;
            public string IconPath;
            public string Difficulty;
            public string DescriptionKey;
        }
        
        private class WordExcelData
        {
            public string WordId;
            public string CategoryId;
            public string TextKey;
            public string CardType;
            public string ImagePath;
            public string HintKey;
        }
        
        private class LevelExcelData
        {
            public int LevelId;
            public int CardCount;
            public int MaxMoves;
            public int ColumnCount;
            public int SlotCount;
            public string[] CategoryIds;
            public bool IsTutorial;
            public bool IsShowResultAd;
            public bool IsShowMatchAd;
        }
        
        #endregion
    }
}
