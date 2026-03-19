using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using SimpleSolitaire.Controller;
using SimpleSolitaire.Model.Config;

namespace SimpleSolitaire.Controller.WordSolitaire
{
    /// <summary>
    /// Word Solitaire 撤销系统
    /// 负责保存游戏状态快照和执行撤销操作
    /// </summary>
    public class WordSolitaireUndoPerformer : UndoPerformer
    {
        /// <summary>
        /// 存档键
        /// </summary>
        protected override string LastGameKey => "WordSolitaire_LastGame";

        /// <summary>
        /// 撤销数据
        /// </summary>
        public override UndoData StatesData
        {
            get => _statesData;
            set => _statesData = (WordSolitaireUndoData)value;
        }

        /// <summary>
        /// Word Solitaire 专属撤销数据
        /// </summary>
        private WordSolitaireUndoData _statesData = new WordSolitaireUndoData();

        /// <summary>
        /// 卡牌逻辑组件引用
        /// </summary>
        private WordSolitaireCardLogic Logic => _cardLogicComponent as WordSolitaireCardLogic;

        /// <summary>
        /// 保存游戏状态
        /// </summary>
        /// <param name="time">游戏时间</param>
        /// <param name="steps">已走步数</param>
        /// <param name="score">当前分数</param>
        public override void SaveGame(int time, int steps, int score)
        {
            _statesData.IsCountable = IsCountableLogic;
            _statesData.AvailableUndoCounts = AvailableUndoCounts;
            _statesData.Time = time;
            _statesData.Steps = steps;
            _statesData.Score = score;
            _statesData.CardsNums = Logic?.CardNumberArray;
            _statesData.LevelId = Logic?.CurrentLevel?.LevelId ?? 0;
            _statesData.RemainingSteps = Logic?.CurrentLevel != null 
                ? Logic.CurrentLevel.MaxMoves - steps 
                : 999;

            // 序列化并保存
            string game = SerializeData(_statesData);
            PlayerPrefs.SetString(LastGameKey, game);
            PlayerPrefs.Save();

            Debug.Log($"[WordSolitaireUndoPerformer] 游戏已保存 - 关卡:{_statesData.LevelId}, 步数:{steps}, 时间:{time}");
        }

        /// <summary>
        /// 添加撤销状态
        /// </summary>
        /// <param name="allDeckArray">所有牌堆数组</param>
        /// <param name="isTemp">是否为临时状态</param>
        public override void AddUndoState(Deck[] allDeckArray, bool isTemp = false)
        {
            // 保存当前关卡信息
            if (Logic != null)
            {
                _statesData.LevelId = Logic.CurrentLevel?.LevelId ?? 0;
                _statesData.RemainingSteps = Logic.CurrentLevel != null 
                    ? Logic.CurrentLevel.MaxMoves - _gameMgrComponent.StepCount 
                    : 999;
            }

            base.AddUndoState(allDeckArray, isTemp);
        }

        /// <summary>
        /// 加载游戏
        /// </summary>
        public override void LoadGame()
        {
            if (!PlayerPrefs.HasKey(LastGameKey))
            {
                Debug.LogWarning("[WordSolitaireUndoPerformer] 没有找到存档");
                return;
            }

            string lastGameData = PlayerPrefs.GetString(LastGameKey);
            StatesData = DeserializeData<WordSolitaireUndoData>(lastGameData);

            if (_statesData.States.Count == 0)
            {
                Debug.LogWarning("[WordSolitaireUndoPerformer] 存档状态为空");
                return;
            }

            // 恢复牌库
            if (Logic != null && _statesData.CardsNums != null)
            {
                Logic.PackDeck.PushCardArray(Logic.CardsArray.ToArray(), false, 0);
            }

            // 重置提示状态
            if (_hintComponent != null)
            {
                _hintComponent.IsHintWasUsed = false;
            }

            // 重置牌库标记
            if (Logic != null)
            {
                Logic.IsNeedResetPack = false;
            }

            // 恢复撤销计数设置
            IsCountable = _statesData.IsCountable;
            AvailableUndoCounts = _statesData.AvailableUndoCounts;

            // 恢复关卡
            if (Logic != null && _statesData.LevelId > 0)
            {
                (Logic as WordSolitaireCardLogic)?.InitializeLevelById(_statesData.LevelId);
            }

            // 恢复卡牌编号
            InitCardsNumberArray();

            // 执行撤销恢复状态
            UndoProcess();

            // 清除临时状态
            _statesData.States.RemoveAll(x => x.IsTemp);

            // 更新提示
            _hintComponent?.UpdateAvailableForDragCards();

            // 激活撤销按钮
            ActivateUndoButton();

            Debug.Log($"[WordSolitaireUndoPerformer] 游戏已加载 - 关卡:{_statesData.LevelId}, 状态数:{_statesData.States.Count}");
        }

        /// <summary>
        /// 检查是否有存档
        /// </summary>
        /// <returns>是否有存档</returns>
        public override bool IsHasGame()
        {
            if (!PlayerPrefs.HasKey(LastGameKey))
            {
                return false;
            }

            string lastGameData = PlayerPrefs.GetString(LastGameKey);
            UndoData data = DeserializeData<WordSolitaireUndoData>(lastGameData);

            return data != null && data.States.Count > 0;
        }

        /// <summary>
        /// 保存状态（供外部调用）
        /// </summary>
        public void SaveState()
        {
            if (_cardLogicComponent != null && _cardLogicComponent.AllDeckArray != null)
            {
                AddUndoState(_cardLogicComponent.AllDeckArray, false);
                Debug.Log("[WordSolitaireUndoPerformer] 状态已保存");
            }
        }

        /// <summary>
        /// 执行撤销（供外部调用）
        /// </summary>
        public void PerformUndo()
        {
            Undo(false);
        }

        /// <summary>
        /// 序列化数据
        /// </summary>
        /// <param name="data">数据对象</param>
        /// <returns>JSON字符串</returns>
        public string SerializeData(object data)
        {
            return JsonConvert.SerializeObject(data, Public.UNDO_DATA_SETTINGS);
        }

        /// <summary>
        /// 反序列化数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="json">JSON字符串</param>
        /// <returns>数据对象</returns>
        public T DeserializeData<T>(string json) where T : UndoData
        {
            return JsonConvert.DeserializeObject<T>(json, Public.UNDO_DATA_SETTINGS);
        }

        /// <summary>
        /// 删除存档
        /// </summary>
        public void DeleteSave()
        {
            PlayerPrefs.DeleteKey(LastGameKey);
            PlayerPrefs.Save();
            Debug.Log("[WordSolitaireUndoPerformer] 存档已删除");
        }

        /// <summary>
        /// 获取存档信息
        /// </summary>
        /// <returns>存档信息字符串</returns>
        public string GetSaveInfo()
        {
            if (!IsHasGame())
            {
                return "无存档";
            }

            string lastGameData = PlayerPrefs.GetString(LastGameKey);
            var data = DeserializeData<WordSolitaireUndoData>(lastGameData);

            if (data == null)
            {
                return "存档损坏";
            }

            return $"关卡:{data.LevelId}, 步数:{data.Steps}, 时间:{data.Time}s, 分数:{data.Score}";
        }
    }

    /// <summary>
    /// Word Solitaire 撤销数据
    /// </summary>
    [System.Serializable]
    public class WordSolitaireUndoData : UndoData
    {
        /// <summary>
        /// 关卡ID
        /// </summary>
        public int LevelId;

        /// <summary>
        /// 剩余步数
        /// </summary>
        public int RemainingSteps;
    }
}
