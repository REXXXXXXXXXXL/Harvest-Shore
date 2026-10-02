using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HarvestShore.Transition;
namespace HarvestShore.Save
{
    public class DataSlot
    {
        /// <summary>
        /// セーブデータ。キーはGUID。
        /// </summary>
        public Dictionary<string, GameSaveData> dataDict = new Dictionary<string, GameSaveData>();

        #region セーブスロット表示用
        public string DataTime
        {
            get
            {
                var key = TimeManager.Instance.GUID;

                if (dataDict.ContainsKey(key))
                {
                    var timeData = dataDict[key];
                    return timeData.timeDict["gameYear"] + "年/" + (Season)timeData.timeDict["gameSeason"] + "/" + timeData.timeDict["gameMonth"] + "月/" + timeData.timeDict["gameDay"] + "日/";
                }
                else return string.Empty;
            }
        }
        #endregion

        public string DataScene
        {
            get
            {
                var key = TransitionManager.Instance.GUID;
                if (dataDict.ContainsKey(key))
                {
                    var transitionData = dataDict[key];
                    return transitionData.dataSceneName switch
                    {
                        "00.Start" => "農場",
                        "01.Field" => "農場",
                        "02.Home" => "小屋",
                        "03.Stall" => "市場",
                        // "04.Path" => "小道",
                        // "05.House01" => "Traceの家",
                        _ => string.Empty
                    };
                }
                else return string.Empty;
            }
        }
    }
}
