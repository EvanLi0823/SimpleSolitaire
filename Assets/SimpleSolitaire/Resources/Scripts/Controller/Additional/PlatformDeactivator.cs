using System.Collections.Generic;
using UnityEngine;

namespace SimpleSolitaire.Controller.Additional
{
    public class PlatformDeactivator : MonoBehaviour
    {
        [SerializeField] public List<RuntimePlatform> _platforms = new List<RuntimePlatform>();

        public void OnEnable()
        {
            Deactivate();
        }

        private void Deactivate()
        {
            bool isPlatformForDeactivate = _platforms.Contains(Application.platform);
            gameObject.SetActive(!isPlatformForDeactivate);
        }
    }
}