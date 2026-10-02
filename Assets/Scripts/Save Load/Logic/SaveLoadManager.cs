using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace HarvestShore.Save
{
    public class SaveLoadManager : Singleton<SaveLoadManager>
    {
        private List<ISaveable> saveableList = new List<ISaveable>();

        public List<DataSlot> dataSlots = new List<DataSlot>(new DataSlot[3]);

        private string jsonFolder;
        private int currentDataIndex;

        protected override void Awake()
        {
            base.Awake();
            jsonFolder = Application.persistentDataPath + "/SAVE DATA/";
            ReadSaveData();
        }
        private void OnEnable()
        {
            EventHandler.StartNewGameEvent += OnStartNewGameEvent;
            EventHandler.EndGameEvent += OnEndGameEvent;
        }

        private void OnDisable()
        {
            EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
            EventHandler.EndGameEvent -= OnEndGameEvent;
        }


        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.I))
                Save(currentDataIndex);
            if (Input.GetKeyDown(KeyCode.O))
                Load(currentDataIndex);
#endif
        }


        private void OnEndGameEvent()
        {
            Save(currentDataIndex);
        }
        private void OnStartNewGameEvent(int index)
        {
            currentDataIndex = index;
        }
        public void RegisterSaveable(ISaveable saveable)
        {
            if (!saveableList.Contains(saveable))
                saveableList.Add(saveable);
        }

        private void ReadSaveData()
        {
            if (Directory.Exists(jsonFolder))
            {
                for (int i = 0; i < dataSlots.Count; i++)
                {
                    var resultPath = jsonFolder + "data" + i + ".json";
                    if (File.Exists(resultPath))
                    {
                        var stringData = File.ReadAllText(resultPath);
                        var jsonData = JsonConvert.DeserializeObject<DataSlot>(stringData);
                        if (jsonData != null)
                            dataSlots[i] = jsonData;
                    }
                }
            }
        }


        private void Save(int index)
        {
            if (index < 0 || index >= dataSlots.Count)
                return;

            DataSlot data = new DataSlot();

            foreach (var saveable in saveableList)
            {
                if (!string.IsNullOrEmpty(saveable.GUID))
                    data.dataDict[saveable.GUID] = saveable.GenerateSaveData();
            }
            dataSlots[index] = data;

            var resultPath = jsonFolder + "data" + index + ".json";

            var jsonData = JsonConvert.SerializeObject(dataSlots[index], Formatting.Indented);

            if (!File.Exists(resultPath))
            {
                Directory.CreateDirectory(jsonFolder);
            }
            File.WriteAllText(resultPath, jsonData);
        }

        public void Load(int index)
        {
            if (index < 0 || index >= dataSlots.Count)
                return;

            currentDataIndex = index;

            var resultPath = jsonFolder + "data" + index + ".json";

            if (!File.Exists(resultPath))
                return;

            var stringData = File.ReadAllText(resultPath);

            var jsonData = JsonConvert.DeserializeObject<DataSlot>(stringData);

            if (jsonData == null || jsonData.dataDict == null)
                return;

            foreach (var saveable in saveableList)
            {
                if (jsonData.dataDict.TryGetValue(saveable.GUID, out var saveData))
                    saveable.RestoreData(saveData);
            }
        }
    }
}
