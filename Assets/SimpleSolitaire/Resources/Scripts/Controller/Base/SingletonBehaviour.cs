using UnityEngine;

namespace SimpleSolitaire.Controller
{
    /// <summary>
    /// 通用单例基类，供需要全局唯一实例的 MonoBehaviour 继承使用。
    /// 子类可通过重写 DontDestroyOnSceneChange 控制跨场景持久化行为。
    /// </summary>
    public class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
    {
        private static T _instance;

        /// <summary>
        /// 是否在场景切换时保持单例（默认 false）。
        /// 子类可重写此属性以启用跨场景持久化。
        /// </summary>
        protected virtual bool DontDestroyOnSceneChange => false;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();

                    if (_instance == null)
                    {
                        // 查找或创建 GameManagers 容器节点
                        GameObject container = GameObject.Find("GameManagers");
                        if (container == null)
                            container = new GameObject("GameManagers");

                        GameObject singletonObject = new GameObject(typeof(T).Name);
                        singletonObject.transform.SetParent(container.transform);
                        _instance = singletonObject.AddComponent<T>();

                        if (_instance.DontDestroyOnSceneChange)
                            DontDestroyOnLoad(container);
                    }
                }
                return _instance;
            }
            private set => _instance = value;
        }

        public virtual void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = (T)this;

            if (DontDestroyOnSceneChange)
                DontDestroyOnLoad(transform.root.gameObject);
        }
    }
}
