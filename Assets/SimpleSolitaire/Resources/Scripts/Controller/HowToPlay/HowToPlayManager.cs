using System.Collections;
using SimpleSolitaire.Controller.UI;
using UnityEngine;

namespace SimpleSolitaire.Controller
{
    [System.Serializable]
    public class HowToPlayData
    {
        public Sprite Preview;

        [TextArea(3, 10)] public string Text;
    }

    public abstract class HowToPlayManager : MonoBehaviour
    {
        [SerializeField] private HowToPlayDataContainer _container;
        [SerializeField] private HowToPlayItem _item;

        protected abstract string FirstPlayKey { get; }

        /// <summary>
        /// 将教程页生成到指定容器中（由 HowToPlayLayerUI 在弹窗打开时调用）。
        /// </summary>
        public void GeneratePagesInto(RectTransform content)
        {
            for (int i = 0; i < _container.Pages.Count; i++)
            {
                HowToPlayData page = _container.Pages[i];
                HowToPlayItem item = Instantiate(_item, content);
                item.Initialize(page);
            }
        }

        /// <summary>
        /// Is first play or not.
        /// </summary>
        public bool IsHasKey()
        {
            return PlayerPrefs.HasKey(FirstPlayKey);
        }

        /// <summary>
        /// 首次游玩时自动弹出教程弹窗。
        /// </summary>
        private IEnumerator Start()
        {
            yield return new WaitForSeconds(0.1f);

            if (!IsHasKey())
            {
                PlayerPrefs.SetInt(FirstPlayKey, 1);
                UILayerManager.Instance?.Show(GameLayerMediator.HowToPlayLayer);
            }
        }
    }
}
