using System.Collections.Generic;
using UnityEngine;

namespace SimpleSolitaire.Controller.UI
{
    /// <summary>
    /// UI 弹窗管理器单例（MonoBehaviour）。
    ///
    /// 职责：
    ///   1. 维护弹窗缓存字典（GameObject 缓存策略，关闭时 SetActive(false) 不销毁）
    ///   2. 维护活跃弹窗堆栈（优先级驱动的压栈/出栈）
    ///   3. 维护等待队列（低优先级请求被高优先级阻塞时入队）
    ///   4. 自动管理 _cardLayer 的显隐
    ///
    /// 挂载位置：Managers 父节点，Script Execution Order 需早于所有 UILayerBase 子类。
    /// </summary>
    public class UILayerManager : MonoBehaviour
    {
        // ── 单例 ──────────────────────────────────────────────────────────────
        public static UILayerManager Instance { get; private set; }

        /// <summary>与现有 GameManager._windowAnimationTime 保持一致（0.42s）。</summary>
        public const float WindowAnimationTime = 0.42f;

        // ── Inspector 配置 ────────────────────────────────────────────────────
        [Header("游戏主牌桌层（非弹窗，随堆栈状态自动管理显隐）")]
        [SerializeField] private GameObject _cardLayer;

        [Header("弹窗父节点（显示时将弹窗挂载到此节点下；为空则使用本 GameObject）")]
        [SerializeField] private Transform _popupRoot;

        [Header("弹窗预制体列表（按需实例化，首次 Show 时自动创建）")]
        [SerializeField] private List<UILayerBase> _layerPrefabs = new List<UILayerBase>();

        // ── 核心数据结构 ──────────────────────────────────────────────────────

        /// <summary>弹窗缓存字典：Key = LayerKey，Value = UILayerBase 实例。</summary>
        private readonly Dictionary<string, UILayerBase> _layerCache
            = new Dictionary<string, UILayerBase>();


        /// <summary>活跃弹窗堆栈。栈顶为当前正在展示的弹窗。</summary>
        private readonly Stack<UILayerBase> _layerStack
            = new Stack<UILayerBase>();

        /// <summary>
        /// 等待队列：低优先级弹窗被高优先级阻塞时入队，栈顶关闭后自动处理。
        /// </summary>
        private readonly Queue<string> _waitingQueue
            = new Queue<string>();

        // ── Unity 生命周期 ────────────────────────────────────────────────────
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        // ── 注册/注销（内部使用）────────────────────────────────────────────────

        // ── 核心调度 API ──────────────────────────────────────────────────────

        /// <summary>
        /// 请求显示指定弹窗。
        ///
        /// 优先级规则：
        ///   - 堆栈为空 → 直接入栈并显示。
        ///   - 新弹窗优先级 >= 栈顶 → 压栈（栈顶 Pause），立即显示。
        ///   - 新弹窗优先级 < 栈顶 → 进入等待队列，待栈顶关闭后自动处理。
        /// </summary>
        /// <param name="layerKey">弹窗的 LayerKey（UILayerBase._layerKey）。</param>
        /// <param name="forceShow">为 true 时跳过优先级检查强制入栈（谨慎使用）。</param>
        public void Show(string layerKey, bool forceShow = false)
        {
            // 缓存中不存在时尝试从预制体列表实例化
            if (!_layerCache.ContainsKey(layerKey))
                TryInstantiatePrefab(layerKey);

            if (!TryGetLayer(layerKey, out UILayerBase layer)) return;

            if (layer.IsVisible)
            {
                Debug.Log($"[UILayerManager] 弹窗 '{layerKey}' 已在显示中，跳过。");
                return;
            }

            bool canShow = forceShow
                           || _layerStack.Count == 0
                           || (int)layer.Priority >= (int)_layerStack.Peek().Priority;

            if (canShow)
            {
                PushLayer(layer);
            }
            else
            {
                // 优先级不足，进入等待队列
                if (!IsInWaitingQueue(layerKey))
                    _waitingQueue.Enqueue(layerKey);

                Debug.Log($"[UILayerManager] 弹窗 '{layerKey}' 优先级({layer.Priority}) < 栈顶({_layerStack.Peek().Priority})，已入等待队列。");
            }
        }

        /// <summary>
        /// 请求隐藏指定弹窗。若是栈顶弹窗则出栈，否则从栈中移除（兼容非栈顶关闭）。
        /// </summary>
        public void Hide(string layerKey)
        {
            if (!TryGetLayer(layerKey, out UILayerBase layer)) return;
            if (!layer.IsVisible) return;

            PopLayer(layer);
        }

        /// <summary>
        /// 隐藏当前栈顶弹窗（等效于"返回"操作）。
        /// </summary>
        public void HideTop()
        {
            if (_layerStack.Count == 0) return;
            PopLayer(_layerStack.Peek());
        }

        /// <summary>
        /// 隐藏所有弹窗并清空堆栈（游戏重置等场景使用）。立即隐藏，不播放动画。
        /// </summary>
        public void HideAll()
        {
            while (_layerStack.Count > 0)
            {
                var top = _layerStack.Pop();
                top.Hide();
            }

            _waitingQueue.Clear();
            UpdateCardLayerVisibility();
        }

        /// <summary>
        /// 查询指定弹窗当前是否处于显示状态。
        /// </summary>
        public bool IsShowing(string layerKey)
        {
            return _layerCache.TryGetValue(layerKey, out var layer) && layer.IsVisible;
        }

        /// <summary>
        /// 获取当前栈顶弹窗实例（无弹窗时返回 null）。
        /// </summary>
        public UILayerBase GetTopLayer()
        {
            return _layerStack.Count > 0 ? _layerStack.Peek() : null;
        }

        /// <summary>
        /// 从缓存中取出指定 Key 对应的弹窗并转型返回，不存在或类型不符时返回 null。
        /// </summary>
        public T GetLayer<T>(string key) where T : UILayerBase
        {
            if (_layerCache.TryGetValue(key, out var layer))
                return layer as T;
            return null;
        }

        /// <summary>
        /// 手动控制 _cardLayer 可见性（供特殊业务需求调用，正常情况自动管理）。
        /// </summary>
        public void SetMainLayerVisible(bool visible)
        {
            if (_cardLayer != null)
                _cardLayer.SetActive(visible);
        }

        // ── 内部实现 ──────────────────────────────────────────────────────────

        /// <summary>
        /// 压栈：暂停当前栈顶（若存在），将弹窗重新挂载到 _popupRoot，压入并显示。
        /// </summary>
        private void PushLayer(UILayerBase layer)
        {
            // 将弹窗挂载到指定父节点（保持本地坐标不变）
            Transform root = _popupRoot != null ? _popupRoot : transform;
            if (layer.transform.parent != root)
                layer.transform.SetParent(root, false);

            if (_layerStack.Count > 0)
                _layerStack.Peek().Pause();

            _layerStack.Push(layer);
            layer.Show();

            UpdateCardLayerVisibility();
        }

        /// <summary>
        /// 出栈：隐藏指定弹窗，恢复下层弹窗，处理等待队列。
        /// </summary>
        private void PopLayer(UILayerBase layer)
        {
            // 从堆栈中移除目标弹窗（兼容非栈顶关闭：临时出栈再入栈）
            RemoveFromStack(layer);

            layer.Hide(onComplete: () =>
            {
                // 恢复新的栈顶（若存在）
                if (_layerStack.Count > 0)
                    _layerStack.Peek().Resume();

                UpdateCardLayerVisibility();
                ProcessWaitingQueue();
            });
        }

        /// <summary>
        /// 从堆栈中移除指定弹窗（不触发 Hide，仅修改数据结构）。
        /// </summary>
        private void RemoveFromStack(UILayerBase target)
        {
            if (_layerStack.Count == 0) return;

            // 快速路径：目标在栈顶
            if (_layerStack.Peek() == target)
            {
                _layerStack.Pop();
                return;
            }

            // 通用路径：目标在栈中间
            var temp = new Stack<UILayerBase>();
            while (_layerStack.Count > 0)
            {
                var top = _layerStack.Pop();
                if (top == target) break;
                temp.Push(top);
            }

            while (temp.Count > 0)
                _layerStack.Push(temp.Pop());
        }

        /// <summary>
        /// 处理等待队列中的下一个弹窗请求（每次出栈后调用）。
        /// </summary>
        private void ProcessWaitingQueue()
        {
            while (_waitingQueue.Count > 0)
            {
                string nextKey = _waitingQueue.Peek();

                if (!TryGetLayer(nextKey, out UILayerBase nextLayer))
                {
                    _waitingQueue.Dequeue();
                    continue;
                }

                bool canShow = _layerStack.Count == 0
                               || (int)nextLayer.Priority >= (int)_layerStack.Peek().Priority;

                if (canShow)
                {
                    _waitingQueue.Dequeue();
                    PushLayer(nextLayer);
                    break; // 一次只处理一个，避免连锁
                }
                else
                {
                    break; // 队列头优先级仍不足，等待
                }
            }
        }

        /// <summary>
        /// 根据堆栈状态更新 _cardLayer 可见性。
        /// 规则：堆栈为空时显示牌桌，否则隐藏。
        /// </summary>
        private void UpdateCardLayerVisibility()
        {
            if (_cardLayer != null)
                _cardLayer.SetActive(_layerStack.Count == 0);
        }

        private bool TryGetLayer(string key, out UILayerBase layer)
        {
            if (_layerCache.TryGetValue(key, out layer)) return true;
            Debug.LogError($"[UILayerManager] 未找到弹窗 Key：'{key}'，请检查 UILayerBase.LayerKey 字段是否正确设置且 GameObject 已激活过一次（触发 Awake 注册）。");
            return false;
        }

        private bool IsInWaitingQueue(string key)
        {
            foreach (var k in _waitingQueue)
                if (k == key) return true;
            return false;
        }

        /// <summary>
        /// 尝试从 _layerPrefabs 找到匹配的预制体并实例化到 _popupRoot 下。
        /// Manager 直接写入 _layerCache，不依赖 UILayerBase.Awake() 自注册。
        /// </summary>
        private void TryInstantiatePrefab(string key)
        {
            if (_layerPrefabs == null) return;

            UILayerBase prefab = _layerPrefabs.Find(p => p != null && p.LayerKey == key);
            if (prefab == null)
            {
                Debug.LogWarning($"[UILayerManager] 预制体列表中未找到 Key='{key}' 的弹窗预制体。");
                return;
            }

            Transform root = _popupRoot != null ? _popupRoot : transform;
            UILayerBase instance = Instantiate(prefab, root);
            instance.gameObject.SetActive(false); // 初始隐藏，等 Show() 时激活
            _layerCache[key] = instance;          // Manager 掌控缓存注册
        }
    }
}
