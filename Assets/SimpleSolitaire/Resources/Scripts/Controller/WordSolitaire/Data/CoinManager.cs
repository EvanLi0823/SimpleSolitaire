using System;
using System.Text;
using UnityEngine;

namespace SimpleSolitaire.Controller.WordSolitaire
{
    /// <summary>
    /// 金币管理器
    /// 负责金币的增减、持久化存储和事件通知
    /// </summary>
    public class CoinManager : MonoBehaviour
    {
        /// <summary>
        /// PlayerPrefs存储键
        /// </summary>
        private const string COINS_KEY = "WordSolitaire_Coins";

        /// <summary>
        /// 加密密钥（简单XOR加密）
        /// </summary>
        private const string ENCRYPTION_KEY = "WS2026";

        /// <summary>
        /// 当前金币数量
        /// </summary>
        private int _currentCoins;

        /// <summary>
        /// 当前金币数量（只读）
        /// </summary>
        public int CurrentCoins => _currentCoins;

        /// <summary>
        /// 金币变化事件
        /// </summary>
        public event Action<int> OnCoinsChanged;

        /// <summary>
        /// 初始化金币管理器
        /// </summary>
        /// <param name="initialCoins">初始金币数量（首次游戏）</param>
        public void Initialize(int initialCoins = 100)
        {
            // 从PlayerPrefs加载金币（带解密）
            _currentCoins = LoadCoins(initialCoins);
            
            // 发布初始金币数量
            PublishCoinsChanged();
            
            Debug.Log($"[CoinManager] 初始化完成，当前金币: {_currentCoins}");
        }

        /// <summary>
        /// 增加金币
        /// </summary>
        /// <param name="amount">增加数量</param>
        public void AddCoins(int amount)
        {
            if (amount <= 0) return;

            _currentCoins += amount;
            SaveCoins();
            PublishCoinsChanged();

            Debug.Log($"[CoinManager] 增加金币: +{amount}，当前: {_currentCoins}");
        }

        /// <summary>
        /// 消耗金币
        /// </summary>
        /// <param name="amount">消耗数量</param>
        /// <returns>是否成功消耗</returns>
        public bool SpendCoins(int amount)
        {
            if (amount <= 0) return false;

            if (_currentCoins >= amount)
            {
                _currentCoins -= amount;
                SaveCoins();
                PublishCoinsChanged();

                Debug.Log($"[CoinManager] 消耗金币: -{amount}，当前: {_currentCoins}");
                return true;
            }

            Debug.LogWarning($"[CoinManager] 金币不足，需要: {amount}，当前: {_currentCoins}");
            return false;
        }

        /// <summary>
        /// 检查是否有足够金币
        /// </summary>
        /// <param name="amount">需要数量</param>
        /// <returns>是否足够</returns>
        public bool HasEnoughCoins(int amount)
        {
            return _currentCoins >= amount;
        }

        /// <summary>
        /// 设置金币数量（用于调试或补偿）
        /// </summary>
        /// <param name="amount">目标数量</param>
        public void SetCoins(int amount)
        {
            _currentCoins = Mathf.Max(0, amount);
            SaveCoins();
            PublishCoinsChanged();

            Debug.Log($"[CoinManager] 设置金币为: {_currentCoins}");
        }

        /// <summary>
        /// 保存金币到PlayerPrefs（带加密）
        /// </summary>
        private void SaveCoins()
        {
            string encryptedValue = Encrypt(_currentCoins.ToString());
            PlayerPrefs.SetString(COINS_KEY, encryptedValue);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// 从PlayerPrefs加载金币（带解密）
        /// </summary>
        /// <param name="defaultValue">默认值</param>
        /// <returns>金币数量</returns>
        private int LoadCoins(int defaultValue)
        {
            if (PlayerPrefs.HasKey(COINS_KEY))
            {
                string encryptedValue = PlayerPrefs.GetString(COINS_KEY);
                string decryptedValue = Decrypt(encryptedValue);

                if (int.TryParse(decryptedValue, out int savedCoins))
                {
                    return savedCoins;
                }
            }

            // 首次游戏，设置默认值
            PlayerPrefs.SetString(COINS_KEY, Encrypt(defaultValue.ToString()));
            PlayerPrefs.Save();
            return defaultValue;
        }

        /// <summary>
        /// 发布金币变化事件
        /// </summary>
        private void PublishCoinsChanged()
        {
            // 通过GameEventBus发布事件
            GameEventBus.PublishCoinsChanged(_currentCoins);
            
            // 同时触发本地事件
            OnCoinsChanged?.Invoke(_currentCoins);
        }

        /// <summary>
        /// 简单XOR加密
        /// </summary>
        /// <param name="input">原始字符串</param>
        /// <returns>加密后的Base64字符串</returns>
        private string Encrypt(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                char c = (char)(input[i] ^ ENCRYPTION_KEY[i % ENCRYPTION_KEY.Length]);
                sb.Append(c);
            }

            // 转换为Base64以便存储
            byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// 简单XOR解密
        /// </summary>
        /// <param name="input">加密后的Base64字符串</param>
        /// <returns>原始字符串</returns>
        private string Decrypt(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            try
            {
                // 从Base64解码
                byte[] bytes = Convert.FromBase64String(input);
                string xorString = Encoding.UTF8.GetString(bytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < xorString.Length; i++)
                {
                    char c = (char)(xorString[i] ^ ENCRYPTION_KEY[i % ENCRYPTION_KEY.Length]);
                    sb.Append(c);
                }

                return sb.ToString();
            }
            catch (Exception e)
            {
                Debug.LogError($"[CoinManager] 解密失败: {e.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// 重置金币（用于测试）
        /// </summary>
        public void ResetCoins(int amount = 100)
        {
            _currentCoins = amount;
            SaveCoins();
            PublishCoinsChanged();

            Debug.Log($"[CoinManager] 重置金币为: {_currentCoins}");
        }

        /// <summary>
        /// 清除金币数据（用于测试）
        /// </summary>
        public void ClearCoinsData()
        {
            PlayerPrefs.DeleteKey(COINS_KEY);
            PlayerPrefs.Save();

            Debug.Log("[CoinManager] 金币数据已清除");
        }
    }
}
