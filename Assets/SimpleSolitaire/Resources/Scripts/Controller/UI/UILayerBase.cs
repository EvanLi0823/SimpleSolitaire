using System;
using System.Collections;
using SimpleSolitaire.Controller;
using SimpleSolitaire.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleSolitaire.Controller.UI
{
    /// <summary>
    /// 所有 UI 弹窗的抽象基类。
    /// 挂载到每个弹窗 GameObject 根节点。
    ///
    /// 生命周期回调链：
    ///   Show() → SetActive(true) → Appear动画 → OnLayerShow() → OnShowCompleted
    ///   Hide() → Disappear动画 → 延迟0.42s → OnLayerHide() → SetActive(false) → OnHideCompleted
    ///   被高优先级弹窗压栈 → Pause() → OnLayerPause()
    ///   上层弹窗弹出后恢复 → Resume() → OnLayerResume()
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public abstract class UILayerBase : MonoBehaviour
    {
        // ── Inspector 配置 ────────────────────────────────────────────────────
        [Header("弹窗配置")]
        [SerializeField] private LayerPriority     _priority      = LayerPriority.Feature;
        [SerializeField] private LayerAnimationType _animationType = LayerAnimationType.Standard;

        [Header("背景遮罩（BGBlocker 子节点）")]
        [SerializeField] [Range(0f, 1f)] private float _blockerAlpha = 0.7f;

        // ── 公开属性 ──────────────────────────────────────────────────────────
        public LayerPriority Priority => _priority;

        /// <summary>
        /// 弹窗唯一 Key，默认取挂载脚本的类名，无需手动配置。
        /// 子类可 override 以保持与父类相同的 Key（用于多态弹窗场景）。
        /// </summary>
        public virtual string LayerKey => GetType().Name;

        /// <summary>弹窗当前是否处于激活显示状态（包含被压栈暂停的情况）。</summary>
        public bool IsVisible { get; private set; }

        /// <summary>弹窗是否处于暂停状态（被更高优先级弹窗压栈）。</summary>
        public bool IsPaused  { get; private set; }

        // ── 事件 ──────────────────────────────────────────────────────────────
        /// <summary>弹窗完全出现后触发（Appear 动画触发后，业务逻辑执行后）。</summary>
        public event Action OnShowCompleted;

        /// <summary>弹窗完全隐藏后触发（GO 已 SetActive(false)）。</summary>
        public event Action OnHideCompleted;

        // ── 内部引用 ──────────────────────────────────────────────────────────
        private Animator _animator;
        private Coroutine _hideCoroutine;
        private Image _blockerImage;

        private const string BlockerNodeName = "BGBlocker";

        // 与现有 GameManager 保持一致的 Animator 触发器名称
        private const string AppearTrigger    = "Appear";
        private const string DisappearTrigger = "Disappear";

        // ── Unity 生命周期 ────────────────────────────────────────────────────
        protected virtual void Awake()
        {
            _animator = GetComponent<Animator>();

            // 查找 BGBlocker 子节点的 Image 组件（用于控制背景遮罩透明度）
            Transform blocker = transform.Find(BlockerNodeName);
            if (blocker != null)
                _blockerImage = blocker.GetComponent<Image>();

            // 子类在此绑定子节点组件，无需 Inspector 拖拽
            OnBindComponents();
            // 注册由 UILayerManager 在实例化时统一完成，此处不调用 Register
        }

        protected virtual void OnDestroy()
        {
            // 清理本节点在 ComponentFinder 中的缓存条目
            ComponentFinder.ClearCache(transform);
            // 缓存注销由 UILayerManager 统一管理，此处不调用 Unregister
        }

        // ── 公开 API（由 UILayerManager 统一调度）────────────────────────────

        /// <summary>
        /// 显示弹窗。请通过 UILayerManager.Show(key) 调用，不建议外部直接调用。
        /// </summary>
        public void Show()
        {
            IsVisible = true;
            IsPaused  = false;
            gameObject.SetActive(true);

            // 设置背景遮罩为黑色，透明度由 _blockerAlpha 控制
            if (_blockerImage != null)
                _blockerImage.color = new Color(0f, 0f, 0f, _blockerAlpha);

            if (_animationType == LayerAnimationType.Standard && _animator != null)
            {
                AudioController.Instance?.Play(AudioController.AudioType.WindowOpen);
                _animator.SetTrigger(AppearTrigger);
            }

            OnLayerShow();
            OnShowCompleted?.Invoke();
        }

        /// <summary>
        /// 隐藏弹窗。请通过 UILayerManager.Hide(key) 调用，不建议外部直接调用。
        /// </summary>
        /// <param name="onComplete">动画结束、GO 隐藏后的回调。</param>
        public void Hide(Action onComplete = null)
        {
            // 防止重复触发
            if (_hideCoroutine != null)
                StopCoroutine(_hideCoroutine);

            if (_animationType == LayerAnimationType.Standard && _animator != null)
            {
                AudioController.Instance?.Play(AudioController.AudioType.WindowClose);
                _animator.SetTrigger(DisappearTrigger);
                _hideCoroutine = StartCoroutine(DelayedHide(UILayerManager.WindowAnimationTime, onComplete));
            }
            else
            {
                FinishHide(onComplete);
            }
        }

        /// <summary>
        /// 通知弹窗被高优先级弹窗压栈，进入暂停状态（保持显示但不响应操作）。
        /// </summary>
        public void Pause()
        {
            IsPaused = true;
            OnLayerPause();
        }

        /// <summary>
        /// 通知弹窗从暂停状态恢复（上层弹窗关闭后调用）。
        /// </summary>
        public void Resume()
        {
            IsPaused = false;
            OnLayerResume();
        }

        // ── 子类实现的模板方法 ────────────────────────────────────────────────

        /// <summary>
        /// 组件绑定钩子，在 Awake 中自动调用。
        /// 子类在此通过 this.Get&lt;T&gt;() / this.Find&lt;T&gt;() 查找子节点组件，
        /// 无需在 Inspector 中拖拽赋值。
        /// </summary>
        protected virtual void OnBindComponents() { }

        /// <summary>弹窗显示时的业务逻辑（填充数据、重置状态等）。GO 已激活，动画已触发。</summary>
        protected abstract void OnLayerShow();

        /// <summary>弹窗隐藏完成时的业务逻辑（清理状态等）。GO 即将 SetActive(false)。</summary>
        protected abstract void OnLayerHide();

        /// <summary>被高优先级弹窗压栈时调用。默认空实现，子类可 override 暂停内部动画等。</summary>
        protected virtual void OnLayerPause() { }

        /// <summary>从暂停状态恢复时调用。默认空实现。</summary>
        protected virtual void OnLayerResume() { }

        // ── 内部实现 ──────────────────────────────────────────────────────────

        private IEnumerator DelayedHide(float delay, Action onComplete)
        {
            yield return new WaitForSeconds(delay);
            _hideCoroutine = null;
            FinishHide(onComplete);
        }

        private void FinishHide(Action onComplete)
        {
            IsVisible = false;
            IsPaused  = false;
            OnLayerHide();
            gameObject.SetActive(false);
            onComplete?.Invoke();
            OnHideCompleted?.Invoke();
        }
    }
}
