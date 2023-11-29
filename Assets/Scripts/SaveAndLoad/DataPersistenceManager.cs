using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UnityEngine.SceneManagement;

namespace SaveAndLoad
{
    public class DataPersistenceManager : Singleton<DataPersistenceManager>
    {
        [Header("Debugg")] [SerializeField] private bool initializedDataIfNull = false;

        [Header("File Storage Config")] [SerializeField]
        private string filename;

        [SerializeField] private bool useEncryption = true;

        [Header("Auto Saving Configuration")] [SerializeField]
        private float autoSaveTime = 60f;

        public GameData gameData;
        private List<IDataPersisence> DataPersisencesObjects;
        private FileDataHandler dataHandler;
        private Coroutine autosaveCoroutine;
        private string selectedProfileId = "";

        protected override void Awake()
        {
            base.Awake();

            this.dataHandler = new FileDataHandler(Application.persistentDataPath, filename, useEncryption);
            InitializeSelectedProfieleId();
        }


        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        public void DeleteProfileData(string profileId)
        {
            dataHandler.Delete(profileId);

            InitializeSelectedProfieleId();

            LoadGame();
        }

        private void InitializeSelectedProfieleId()
        {
            this.selectedProfileId = dataHandler.GetMostRecentlyProfileId();
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            this.DataPersisencesObjects = FindAllDatatPersistenceObjects();
            LoadGame();

            if (autosaveCoroutine != null)
            {
                StopCoroutine(autosaveCoroutine);
            }

            autosaveCoroutine = StartCoroutine(AutoSave());
        }

        public void OnSceneUnloaded(Scene scene)
        {
            SaveGame();
        }

        public void timeStamp(string name)
        {
            name = DateTime.Now.ToString("f");
        }

        public void NewGame()
        {
            this.gameData = new GameData();
        }

        public void LoadGame()
        {
            if (dataHandler == null)
            {
                Debug.LogError("DataHandler is null.");
                return;
            }
            
            // LOAD GAME DATA HERE
            this.gameData = dataHandler.Load(selectedProfileId);
            if (this.gameData == null && initializedDataIfNull)
            {
                NewGame();
            }

            if (this.gameData != null)
            {
                // Push loaded data
                foreach (IDataPersisence dataPersistence in DataPersisencesObjects)
                {
                    dataPersistence.LoadData(this.gameData);
                }
                
        }

            // Add this line to call the LoadData method for InventoryManager
            InventoryManager.Instance.LoadData(this.gameData);
        }

        public void ChangeSelectedProfileId(string newProfileId)
        {
            this.selectedProfileId = newProfileId;
            LoadGame();
        }

        public void SaveGame()
        {
            if (this.gameData == null)
            {
                return;
            }
            // SAVE GAME DATA HERE

            foreach (IDataPersisence iDataPersisencesObject in DataPersisencesObjects)
            {
                iDataPersisencesObject.SaveData(ref gameData);
            }

            gameData.lastUpdated = System.DateTime.Now.ToBinary();
            dataHandler.Save(gameData, selectedProfileId);
        }

        public Dictionary<string, GameData> getAllProfilesGameData()
        {
            return dataHandler.LoadAllProfiles();
        }

        private void OnApplicationQuit()
        {
            SaveGame();
        }

        private void ContinueGame()
        {
            
        }
        private List<IDataPersisence> FindAllDatatPersistenceObjects()
        {
            IEnumerable<IDataPersisence> dataPersisenceObjects =
                FindObjectsOfType<MonoBehaviour>().OfType<IDataPersisence>();
            return new List<IDataPersisence>(dataPersisenceObjects);
        }

        public bool HasGameData()
        {
            return gameData != null;
        }

        private IEnumerator AutoSave()
        {
            while (true)
            {
                yield return new WaitForSeconds(autoSaveTime);
                SaveGame();
            }
        }
    }
}