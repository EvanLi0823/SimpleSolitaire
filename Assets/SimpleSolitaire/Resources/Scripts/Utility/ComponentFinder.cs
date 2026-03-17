using System.Collections.Generic;
using UnityEngine;

namespace SimpleSolitaire.Utility
{
    /// <summary>
    /// 组件查找工具 —— 通过路径或名称定位子节点组件，免除 Inspector 手动拖拽赋值。
    ///
    /// ── 使用方式（在 MonoBehaviour 内）────────────────────────────────────────
    ///
    ///   // 1. 路径查找（最精确，推荐）：沿 "父/子/目标" 路径定位
    ///   _title = this.Get&lt;Text&gt;("Header/TitleText");
    ///
    ///   // 2. 名称查找（全子树搜索，适合不关心层级时）：
    ///   _title = this.Find&lt;Text&gt;("TitleText");
    ///
    ///   // 3. 跨 GameObject 引用（场景全局查找，带缓存）：
    ///   _gameManager = ComponentFinder.FindInScene&lt;GameManager&gt;();
    ///
    /// ── 缓存策略 ──────────────────────────────────────────────────────────────
    ///   所有查找结果默认缓存。缓存 Key = "{根节点InstanceID}:{路径/名称}:{类型名}"。
    ///   组件销毁时自动失效（下次访问时清理）。
    ///   场景切换时调用 ComponentFinder.ClearAll() 清理全部缓存。
    ///
    /// ── 注意事项 ──────────────────────────────────────────────────────────────
    ///   - 路径查找使用 Unity Transform.Find，支持 "/" 分隔符，仅搜索已激活路径上的节点。
    ///   - 名称查找支持 includeInactive 参数，默认包含未激活节点。
    ///   - FindInScene 内部调用 Object.FindObjectOfType，首次调用较慢，建议在 Awake 阶段执行。
    /// </summary>
    public static class ComponentFinder
    {
        // 缓存 Key 格式："{rootInstanceID}:p:{path}:{T}" 或 "{rootInstanceID}:n:{name}:{T}" 或 "scene:{T}"
        private static readonly Dictionary<string, Component> _cache = new Dictionary<string, Component>();

        // ── 路径查找 ──────────────────────────────────────────────────────────

        /// <summary>
        /// 按相对路径查找子节点上的组件，使用 Unity 原生 Transform.Find 路径语法（"/" 分隔）。
        /// </summary>
        /// <param name="root">搜索起点 Transform（通常为弹窗根节点）。</param>
        /// <param name="path">相对于 root 的节点路径，例如 "Panel/Header/TitleText"。</param>
        /// <param name="useCache">是否使用查找缓存（默认 true）。</param>
        public static T Get<T>(Transform root, string path, bool useCache = true) where T : Component
        {
            if (root == null)
            {
                Debug.LogWarning("[ComponentFinder] root Transform 为 null，跳过查找。");
                return null;
            }

            string key = $"{root.GetInstanceID()}:p:{path}:{typeof(T).Name}";

            if (useCache && _cache.TryGetValue(key, out Component cached))
            {
                if (cached != null) return (T)cached;
                _cache.Remove(key); // 组件已销毁，清理失效条目
            }

            Transform node = root.Find(path);
            if (node == null)
            {
                Debug.LogWarning($"[ComponentFinder] '{root.name}' → 路径 \"{path}\" 未找到节点。");
                return null;
            }

            T component = node.GetComponent<T>();
            if (component == null)
            {
                Debug.LogWarning($"[ComponentFinder] '{root.name}/{path}' 节点上不存在组件 <{typeof(T).Name}>。");
                return null;
            }

            if (useCache) _cache[key] = component;
            return component;
        }

        // ── 名称查找 ──────────────────────────────────────────────────────────

        /// <summary>
        /// 在 root 的所有子节点中，按 GameObject 名称递归查找组件（深度优先，返回第一个匹配）。
        /// </summary>
        /// <param name="root">搜索起点 Transform。</param>
        /// <param name="nodeName">目标节点的 GameObject 名称（精确匹配）。</param>
        /// <param name="includeInactive">是否包含未激活的节点（默认 true）。</param>
        /// <param name="useCache">是否使用查找缓存（默认 true）。</param>
        public static T Find<T>(Transform root, string nodeName, bool includeInactive = true, bool useCache = true) where T : Component
        {
            if (root == null)
            {
                Debug.LogWarning("[ComponentFinder] root Transform 为 null，跳过查找。");
                return null;
            }

            string key = $"{root.GetInstanceID()}:n:{nodeName}:{typeof(T).Name}";

            if (useCache && _cache.TryGetValue(key, out Component cached))
            {
                if (cached != null) return (T)cached;
                _cache.Remove(key);
            }

            T component = FindRecursive<T>(root, nodeName, includeInactive);
            if (component == null)
            {
                Debug.LogWarning($"[ComponentFinder] '{root.name}' 子树中未找到名为 \"{nodeName}\" 的 <{typeof(T).Name}> 节点。");
                return null;
            }

            if (useCache) _cache[key] = component;
            return component;
        }

        // ── 场景全局查找 ──────────────────────────────────────────────────────

        /// <summary>
        /// 在整个场景中查找指定类型的组件（首次调用使用 Object.FindObjectOfType，后续使用缓存）。
        /// 适用于跨 GameObject 的引用（如 GameManager、CardLogic 等单例型组件）。
        /// 建议仅在 Awake / Start 阶段调用，避免每帧搜索。
        /// </summary>
        /// <param name="includeInactive">是否包含未激活的 GameObject（默认 false）。</param>
        public static T FindInScene<T>(bool includeInactive = false) where T : Component
        {
            string key = $"scene:{typeof(T).Name}:{includeInactive}";

            if (_cache.TryGetValue(key, out Component cached))
            {
                if (cached != null) return (T)cached;
                _cache.Remove(key);
            }

            T component = Object.FindObjectOfType<T>(includeInactive);
            if (component == null)
            {
                Debug.LogWarning($"[ComponentFinder] 场景中未找到 <{typeof(T).Name}>（includeInactive={includeInactive}）。");
                return null;
            }

            _cache[key] = component;
            return component;
        }

        // ── 批量查找 ──────────────────────────────────────────────────────────

        /// <summary>
        /// 获取 root 子树中所有匹配类型的组件（不缓存，每次均全量搜索）。
        /// </summary>
        /// <param name="includeInactive">是否包含未激活的子节点（默认 true）。</param>
        public static T[] GetAll<T>(Transform root, bool includeInactive = true) where T : Component
        {
            if (root == null) return System.Array.Empty<T>();
            return root.GetComponentsInChildren<T>(includeInactive);
        }

        // ── 缓存管理 ──────────────────────────────────────────────────────────

        /// <summary>
        /// 清理指定根节点相关的全部缓存（在该节点 OnDestroy 中调用，防止内存泄漏）。
        /// </summary>
        public static void ClearCache(Transform root)
        {
            if (root == null) return;
            string prefix = root.GetInstanceID().ToString();
            var toRemove = new List<string>(4);
            foreach (string key in _cache.Keys)
                if (key.StartsWith(prefix)) toRemove.Add(key);
            foreach (string key in toRemove) _cache.Remove(key);
        }

        /// <summary>
        /// 清理全部缓存（场景切换时调用）。
        /// </summary>
        public static void ClearAll() => _cache.Clear();

        // ── 内部实现 ──────────────────────────────────────────────────────────

        private static T FindRecursive<T>(Transform current, string targetName, bool includeInactive) where T : Component
        {
            int childCount = current.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform child = current.GetChild(i);
                if (!includeInactive && !child.gameObject.activeInHierarchy) continue;

                if (child.name == targetName)
                {
                    T comp = child.GetComponent<T>();
                    if (comp != null) return comp;
                }

                T found = FindRecursive<T>(child, targetName, includeInactive);
                if (found != null) return found;
            }
            return null;
        }
    }

    // ── Transform 扩展方法 ────────────────────────────────────────────────────

    /// <summary>
    /// Transform 的扩展方法，提供链式调用风格的组件查找。
    /// </summary>
    public static class TransformComponentExtensions
    {
        /// <summary>按路径查找子节点组件：transform.Get&lt;Text&gt;("Panel/Title")。</summary>
        public static T Get<T>(this Transform root, string path, bool useCache = true) where T : Component
            => ComponentFinder.Get<T>(root, path, useCache);

        /// <summary>按名称递归查找子节点组件：transform.Find&lt;Text&gt;("TitleText")。</summary>
        public static T Find<T>(this Transform root, string nodeName, bool includeInactive = true, bool useCache = true) where T : Component
            => ComponentFinder.Find<T>(root, nodeName, includeInactive, useCache);

        /// <summary>获取子树中所有匹配组件：transform.GetAll&lt;Button&gt;()。</summary>
        public static T[] GetAll<T>(this Transform root, bool includeInactive = true) where T : Component
            => ComponentFinder.GetAll<T>(root, includeInactive);
    }

    // ── MonoBehaviour / Component 扩展方法 ───────────────────────────────────

    /// <summary>
    /// MonoBehaviour 的扩展方法，在组件内部可直接 this.Get&lt;T&gt;() 调用。
    /// </summary>
    public static class MonoBehaviourComponentExtensions
    {
        /// <summary>
        /// 在本组件所在 GameObject 的子节点中，按路径查找组件。
        /// 等效于 ComponentFinder.Get&lt;T&gt;(this.transform, path)。
        ///
        /// <code>
        /// // 在 UILayerBase 子类的 OnBindComponents() 中：
        /// _title = this.Get&lt;Text&gt;("Header/TitleText");
        /// </code>
        /// </summary>
        public static T Get<T>(this Component self, string path, bool useCache = true) where T : Component
            => ComponentFinder.Get<T>(self.transform, path, useCache);

        /// <summary>
        /// 在本组件所在 GameObject 的子节点中，按名称递归查找组件。
        /// 等效于 ComponentFinder.Find&lt;T&gt;(this.transform, name)。
        /// </summary>
        public static T Find<T>(this Component self, string nodeName, bool includeInactive = true, bool useCache = true) where T : Component
            => ComponentFinder.Find<T>(self.transform, nodeName, includeInactive, useCache);

        /// <summary>
        /// 在场景中全局查找指定类型组件（跨 GameObject 引用）。
        /// 等效于 ComponentFinder.FindInScene&lt;T&gt;()。
        ///
        /// <code>
        /// // 在 OnBindComponents() 中获取非子节点的 GameManager：
        /// _gameManager = this.FindInScene&lt;GameManager&gt;();
        /// </code>
        /// </summary>
        public static T FindInScene<T>(this Component self, bool includeInactive = false) where T : Component
            => ComponentFinder.FindInScene<T>(includeInactive);
    }
}
