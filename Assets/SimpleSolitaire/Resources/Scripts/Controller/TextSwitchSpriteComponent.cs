using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleSolitaire
{
    [Serializable]
    public class TextSpriteData
    {
        public string Value;
        public Sprite Sprite;
    }

    public class TextSwitchSpriteComponent : MonoBehaviour
    {
        [SerializeField] private List<TextSpriteData> _spritesData = new List<TextSpriteData>();
        
        [Space(5f)] 
        [SerializeField] private Image _switchImage;

        public void UpdateSwitchImg(string value)
        {
            var spriteData = _spritesData.FirstOrDefault(x => x.Value == value);
            _switchImage.overrideSprite = spriteData?.Sprite;
        }

        public void UpdateSwitchImg<T>(T value) where T : Enum => UpdateSwitchImg(value.ToString());
    }
}