using System.Collections;
using System.Collections.Generic;
using SimpleSolitaire.Model.Enum;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleSolitaire.Controller.WordSolitaire
{
    /// <summary>
    /// WordSolitaire 专用的屏幕方向管理器
    /// 支持动态创建的 UI 元素（分类槽、列区等）
    /// </summary>
    public class WordSolitaireOrientationManager : OrientationManager
    {
        /// <summary>
        /// 动态元素字典：Key = 元素标识, Value = 游戏对象
        /// </summary>
        private Dictionary<OrientationElementKey, GameObject> _dynamicElements;

        /// <summary>
        /// 是否已初始化动态元素系统
        /// </summary>
        private bool _isDynamicSystemInitialized;

        /// <summary>
        /// 初始化动态元素系统
        /// </summary>
        private void Awake()
        {
            _dynamicElements = new Dictionary<OrientationElementKey, GameObject>();
            _isDynamicSystemInitialized = true;
            
            Debug.Log("[WordSolitaireOrientationManager] 动态方向管理系统已初始化");
        }

        /// <summary>
        /// 注册动态创建的 UI 元素
        /// 在创建分类槽或列区时调用
        /// </summary>
        /// <param name="gameObject">UI 元素的游戏对象</param>
        /// <param name="key">元素标识 Key</param>
        /// <returns>是否注册成功</returns>
        public bool RegisterDynamicElement(GameObject gameObject, OrientationElementKey key)
        {
            if (gameObject == null)
            {
                Debug.LogError($"[WordSolitaireOrientationManager] 无法注册 null 对象，Key: {key}");
                return false;
            }

            if (_dynamicElements.ContainsKey(key))
            {
                Debug.LogWarning($"[WordSolitaireOrientationManager] Key {key} 已存在，将覆盖原有元素");
            }

            // 添加到动态字典
            _dynamicElements[key] = gameObject;

            // 获取或创建 HandOrientationElement 组件
            HandOrientationElement element = gameObject.GetComponent<HandOrientationElement>();
            if (element == null)
            {
                element = gameObject.AddComponent<HandOrientationElement>();
            }

            // 配置 HandOrientationElement
            element.Key = key;
            element.RectRoot = gameObject.GetComponent<RectTransform>();
            
            // 确保有 AspectRatioFitter
            if (element.Fitter == null)
            {
                element.Fitter = gameObject.GetComponent<AspectRatioFitter>();
                if (element.Fitter == null)
                {
                    element.Fitter = gameObject.AddComponent<AspectRatioFitter>();
                }
            }

            // 添加到基类的列表中
            DeckElements.Add(element);

            Debug.Log($"[WordSolitaireOrientationManager] 注册动态元素: {key}, 总数: {DeckElements.Count}");

            return true;
        }

        /// <summary>
        /// 批量注册多个动态元素
        /// </summary>
        /// <param name="elements">元素列表</param>
        public void RegisterDynamicElements(List<(GameObject gameObject, OrientationElementKey key)> elements)
        {
            foreach (var element in elements)
            {
                RegisterDynamicElement(element.gameObject, element.key);
            }
        }

        /// <summary>
        /// 取消注册动态元素
        /// </summary>
        /// <param name="key">元素标识 Key</param>
        /// <returns>是否取消成功</returns>
        public bool UnregisterDynamicElement(OrientationElementKey key)
        {
            if (!_dynamicElements.ContainsKey(key))
            {
                Debug.LogWarning($"[WordSolitaireOrientationManager] 尝试取消不存在的 Key: {key}");
                return false;
            }

            // 从字典中移除
            GameObject gameObject = _dynamicElements[key];
            _dynamicElements.Remove(key);

            // 从基类列表中移除
            HandOrientationElement element = gameObject.GetComponent<HandOrientationElement>();
            if (element != null)
            {
                DeckElements.Remove(element);
                Destroy(element);
            }

            Debug.Log($"[WordSolitaireOrientationManager] 取消注册动态元素: {key}, 剩余: {DeckElements.Count}");

            return true;
        }

        /// <summary>
        /// 清空所有动态元素
        /// 在重新生成关卡时调用
        /// </summary>
        public void ClearAllDynamicElements()
        {
            int removedCount = 0;
            
            // 收集所有需要移除的 Keys
            List<OrientationElementKey> keysToRemove = new List<OrientationElementKey>(_dynamicElements.Keys);
            
            // 逐个移除
            foreach (var key in keysToRemove)
            {
                UnregisterDynamicElement(key);
                removedCount++;
            }

            _dynamicElements.Clear();

            Debug.Log($"[WordSolitaireOrientationManager] 清空完成，移除了 {removedCount} 个动态元素");
        }

        /// <summary>
        /// 立即应用当前方向设置到所有动态元素
        /// 在注册完所有元素后调用
        /// </summary>
        public void ApplyOrientationToDynamicElements()
        {
            if (OrientationContainer == null)
            {
                Debug.LogError("[WordSolitaireOrientationManager] OrientationContainer 未配置");
                return;
            }

            StartCoroutine(ApplyOrientationCoroutine());
        }

        /// <summary>
        /// 应用方向的协程
        /// </summary>
        private IEnumerator ApplyOrientationCoroutine()
        {
            yield return new WaitForEndOfFrame();

            OrientationScreen screen = ScreenOrientation.ToOrientationScreen();
            HandOrientation hand = HandOrientation;

            // 使用基类的公共方法来应用方向设置
            yield return StartCoroutine(SetSpecificOrientation(screen, hand));

            Debug.Log("[WordSolitaireOrientationManager] 方向已应用到所有动态元素");
        }

        /// <summary>
        /// 获取下一个可用的分类槽 Key
        /// </summary>
        /// <param name="index">槽位索引</param>
        /// <returns>OrientationElementKey</returns>
        public static OrientationElementKey GetCategorySlotKey(int index)
        {
            // AceDeck_1 到 AceDeck_8
            if (index >= 0 && index < 8)
            {
                return OrientationElementKey.AceDeck_1 + index;
            }
            
            Debug.LogWarning($"[WordSolitaireOrientationManager] 分类槽索引 {index} 超出范围，返回 Unknown");
            return OrientationElementKey.Unknown;
        }

        /// <summary>
        /// 获取下一个可用的列区 Key
        /// </summary>
        /// <param name="index">列索引</param>
        /// <returns>OrientationElementKey</returns>
        public static OrientationElementKey GetColumnDeckKey(int index)
        {
            // BottomDeck_1 到 BottomDeck_10
            if (index >= 0 && index < 10)
            {
                return OrientationElementKey.BottomDeck_1 + index;
            }
            
            Debug.LogWarning($"[WordSolitaireOrientationManager] 列索引 {index} 超出范围，返回 Unknown");
            return OrientationElementKey.Unknown;
        }
    }
}
