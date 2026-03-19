using SimpleSolitaire.Controller.UI;
using SimpleSolitaire.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleSolitaire.Controller.WordSolitaire.UI
{
    /// <summary>
    /// Word Solitaire 设置弹窗
    /// 功能：音量控制、语言切换、振动开关
    /// </summary>
    public class WordSolitaireSettingLayerUI : UILayerBase
    {
        // LayerKey
        public override string LayerKey => "SettingLayerUI";

        // ── 外部依赖 ──────────────────────────────────────────────────────────
        private GameLayerMediator _mediator;

        // ── 内部组件 ──────────────────────────────────────────────────────────
        // 音量控制
        private Slider _musicVolumeSlider;
        private Slider _sfxVolumeSlider;
        private Toggle _musicMuteToggle;
        private Toggle _sfxMuteToggle;

        // 语言切换
        private Dropdown _languageDropdown;

        // 振动开关
        private Toggle _vibrationToggle;

        // 按钮
        private Button _closeButton;
        private Button _rulesButton;

        // ── 数据 ──────────────────────────────────────────────────────────────
        private bool _isInitializing = false;

        protected override void OnBindComponents()
        {
            // 查找依赖
            _mediator = this.FindInScene<GameLayerMediator>();

            // 绑定音量组件
            _musicVolumeSlider = ComponentFinder.Find<Slider>(transform, "MusicVolumeSlider");
            _sfxVolumeSlider = ComponentFinder.Find<Slider>(transform, "SfxVolumeSlider");
            _musicMuteToggle = ComponentFinder.Find<Toggle>(transform, "MusicMuteToggle");
            _sfxMuteToggle = ComponentFinder.Find<Toggle>(transform, "SfxMuteToggle");

            // 绑定语言组件
            _languageDropdown = ComponentFinder.Find<Dropdown>(transform, "LanguageDropdown");

            // 绑定振动组件
            _vibrationToggle = ComponentFinder.Find<Toggle>(transform, "VibrationToggle");

            // 绑定按钮
            _closeButton = ComponentFinder.Find<Button>(transform, "CloseButton");
            _rulesButton = ComponentFinder.Find<Button>(transform, "RulesButton");

            // 绑定事件
            BindEvents();
        }

        /// <summary>
        /// 绑定所有UI事件
        /// </summary>
        private void BindEvents()
        {
            // 音量滑块
            _musicVolumeSlider?.onValueChanged.AddListener(OnMusicVolumeChanged);
            _sfxVolumeSlider?.onValueChanged.AddListener(OnSfxVolumeChanged);

            // 静音开关
            _musicMuteToggle?.onValueChanged.AddListener(OnMusicMuteChanged);
            _sfxMuteToggle?.onValueChanged.AddListener(OnSfxMuteChanged);

            // 语言下拉框
            _languageDropdown?.onValueChanged.AddListener(OnLanguageChanged);

            // 振动开关
            _vibrationToggle?.onValueChanged.AddListener(OnVibrationChanged);

            // 按钮
            _closeButton?.onClick.AddListener(OnClickClose);
            _rulesButton?.onClick.AddListener(OnClickRules);

            // 背景遮罩点击关闭
            ComponentFinder.Find<Button>(transform, "BGBlocker")?.onClick.AddListener(OnClickClose);
        }

        protected override void OnLayerShow()
        {
            _isInitializing = true;
            
            // 初始化UI状态
            InitializeUI();
            
            _isInitializing = false;
        }

        protected override void OnLayerHide()
        {
            // 保存设置
            SaveSettings();
        }

        /// <summary>
        /// 初始化UI状态
        /// </summary>
        private void InitializeUI()
        {
            // 加载音量设置
            if (_musicVolumeSlider != null)
            {
                _musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
            }
            if (_sfxVolumeSlider != null)
            {
                _sfxVolumeSlider.value = PlayerPrefs.GetFloat("SfxVolume", 1f);
            }
            if (_musicMuteToggle != null)
            {
                _musicMuteToggle.isOn = PlayerPrefs.GetInt("MusicMuted", 0) == 1;
            }
            if (_sfxMuteToggle != null)
            {
                _sfxMuteToggle.isOn = PlayerPrefs.GetInt("SfxMuted", 0) == 1;
            }

            // 初始化语言下拉框
            InitializeLanguageDropdown();

            // 加载振动设置
            if (_vibrationToggle != null)
            {
                _vibrationToggle.isOn = PlayerPrefs.GetInt("VibrationEnabled", 1) == 1;
            }
        }

        /// <summary>
        /// 初始化语言下拉框
        /// </summary>
        private void InitializeLanguageDropdown()
        {
            if (_languageDropdown == null) return;

            _languageDropdown.ClearOptions();

            // 添加语言选项
            var options = new System.Collections.Generic.List<string>
            {
                "English",
                "简体中文",
                "繁體中文",
                "日本語",
                "한국어",
                "Français",
                "Español",
                "Deutsch"
            };
            _languageDropdown.AddOptions(options);

            // 设置当前语言
            var currentLanguage = LocalizationManager.Instance?.CurrentLanguage ?? SystemLanguage.English;
            int selectedIndex = GetLanguageIndex(currentLanguage);
            _languageDropdown.value = selectedIndex;
        }

        /// <summary>
        /// 获取语言对应的下拉框索引
        /// </summary>
        private int GetLanguageIndex(SystemLanguage language)
        {
            switch (language)
            {
                case SystemLanguage.ChineseSimplified: return 1;
                case SystemLanguage.ChineseTraditional: return 2;
                case SystemLanguage.Japanese: return 3;
                case SystemLanguage.Korean: return 4;
                case SystemLanguage.French: return 5;
                case SystemLanguage.Spanish: return 6;
                case SystemLanguage.German: return 7;
                default: return 0; // English
            }
        }

        /// <summary>
        /// 获取下拉框索引对应的语言
        /// </summary>
        private SystemLanguage GetLanguageByIndex(int index)
        {
            switch (index)
            {
                case 1: return SystemLanguage.ChineseSimplified;
                case 2: return SystemLanguage.ChineseTraditional;
                case 3: return SystemLanguage.Japanese;
                case 4: return SystemLanguage.Korean;
                case 5: return SystemLanguage.French;
                case 6: return SystemLanguage.Spanish;
                case 7: return SystemLanguage.German;
                default: return SystemLanguage.English;
            }
        }

        /// <summary>
        /// 保存设置
        /// </summary>
        private void SaveSettings()
        {
            PlayerPrefs.Save();
        }

        // ── 事件回调 ──────────────────────────────────────────────────────────

        private void OnMusicVolumeChanged(float value)
        {
            if (_isInitializing) return;
            PlayerPrefs.SetFloat("MusicVolume", value);
            // 通知音频管理器
            GameEventBus.PublishSettingsChanged();
        }

        private void OnSfxVolumeChanged(float value)
        {
            if (_isInitializing) return;
            PlayerPrefs.SetFloat("SfxVolume", value);
            // 通知音频管理器
            GameEventBus.PublishSettingsChanged();
        }

        private void OnMusicMuteChanged(bool isMuted)
        {
            if (_isInitializing) return;
            PlayerPrefs.SetInt("MusicMuted", isMuted ? 1 : 0);
            GameEventBus.PublishSettingsChanged();
        }

        private void OnSfxMuteChanged(bool isMuted)
        {
            if (_isInitializing) return;
            PlayerPrefs.SetInt("SfxMuted", isMuted ? 1 : 0);
            GameEventBus.PublishSettingsChanged();
        }

        private void OnLanguageChanged(int index)
        {
            if (_isInitializing) return;
            
            var language = GetLanguageByIndex(index);
            LocalizationManager.Instance?.SetLanguage(language);
        }

        private void OnVibrationChanged(bool isEnabled)
        {
            if (_isInitializing) return;
            PlayerPrefs.SetInt("VibrationEnabled", isEnabled ? 1 : 0);
        }

        // ── 按钮回调 ──────────────────────────────────────────────────────────

        /// <summary>
        /// 点击关闭按钮
        /// </summary>
        private void OnClickClose()
        {
            UILayerManager.Instance?.Hide(LayerKey);
        }

        /// <summary>
        /// 点击规则按钮
        /// </summary>
        private void OnClickRules()
        {
            _mediator?.ShowHowToPlayLayer(returnToSetting: true);
        }
    }
}
