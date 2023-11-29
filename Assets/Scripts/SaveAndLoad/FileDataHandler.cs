using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SaveAndLoad
{
    public class FileDataHandler
    {
        private string dataDirPath="";
        private string dataFileName="";
        private bool useEncryption = false;
        private readonly string encryptionKey = "BetaniaEKarim";
        private readonly string backupExtension = ".bak";
        public FileDataHandler(string dataDirPath, string dataFileName,bool useEncryption)
        {
            this.dataDirPath = dataDirPath;
            this.dataFileName = dataFileName;
            this.useEncryption = useEncryption;
        }

        public GameData Load(string profileId, bool allowedRestoreFromBackup = true)
        {
            if (profileId == null)
            {
                return null;
            }
            string fullPath = Path.Combine(dataDirPath, profileId , dataFileName);
            GameData loadedData = null;
            if (File.Exists(fullPath))
            {
                try
                {
                    // load the seriazed data from file
                    string dataToLoad = "";
                    using (FileStream stream = new FileStream(fullPath,FileMode.Open))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            dataToLoad = reader.ReadToEnd();
                        }
                    }
                    
                    // Decrypt the data before load
                    if (useEncryption)
                    {
                        dataToLoad = EncryptDecrypt(dataToLoad);
                    }
                    loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
                }
                catch (Exception e)
                {
                    if (allowedRestoreFromBackup)
                    {
                        Debug.LogError("Error occured when trying to load data from file: " + fullPath + "\n" + e);
                        bool rollbackSuccess = AttemptToRollBack(fullPath);
                        if (rollbackSuccess)
                        {
                            loadedData = Load(profileId, false);
                        }
                    }
                    
                }
            }

            return loadedData;
        }

        public string GetMostRecentlyProfileId()
        {
            string mostRecentlyProfileId = "";
            Dictionary<string, GameData> profilesGameData = LoadAllProfiles();
            foreach (KeyValuePair<string, GameData> pair in profilesGameData)
            {
                string profileId = pair.Key;
                GameData gamedata = pair.Value;

                if (gamedata == null)
                {
                    continue;
                }

                if (mostRecentlyProfileId == null)
                {
                    mostRecentlyProfileId = profileId;
                }
                else
                {
                    DateTime mostRecentlyProfileIdDate = DateTime.FromBinary(profilesGameData[mostRecentlyProfileId].lastUpdated);
                    DateTime newDateTime = DateTime.FromBinary(gamedata.lastUpdated);
                    if (newDateTime > mostRecentlyProfileIdDate)
                    {
                        mostRecentlyProfileId = profileId;
                    }
                }
            }
            return mostRecentlyProfileId;
        }
        public Dictionary<string, GameData> LoadAllProfiles()
        {
            Dictionary<string, GameData> profileDictionary = new Dictionary<string, GameData>();
            IEnumerable<DirectoryInfo> dirInfos = new DirectoryInfo(dataDirPath).EnumerateDirectories();
            foreach (DirectoryInfo dirInfo in dirInfos)
            {
             string profileId = dirInfo.Name;   
             string fullPath = Path.Combine(dataDirPath,profileId,dataFileName);
             if (!File.Exists(fullPath))
             {
                 continue;
             }

             GameData profileData = Load(profileId);
             if (profileData != null)
             {
                 profileDictionary.Add(profileId, profileData);
             }
            }
            return profileDictionary;
        }

        public void Delete(string profileId)
        {
            if (profileId == null)
            {
                return;
            }

            string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
            try
            {
                if (File.Exists(fullPath))
                {
                    Directory.Delete(Path.GetDirectoryName(fullPath), true);
                    
                }
            }
            catch 
            {
                
            }
        }
        public void Save(GameData data, string profileId)
        {
            if (profileId == null)
            {
                return;
            }

            string fullPath = Path.Combine(dataDirPath, profileId,dataFileName);
            string backupFilePath = fullPath + backupExtension;
            try
            {
                //create a directory the file will be written to if it doesn't already exist
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                
                //seriaze the c# game data object in JSON 
                string dataToStore = JsonUtility.ToJson(data, true);
                //optional Encrypt
                if (useEncryption)
                {
                    dataToStore = EncryptDecrypt(dataToStore);
                }
                //write the serialized data to the fie 
                using (FileStream stream = new FileStream(fullPath, FileMode.Create))
                {
                    using (StreamWriter streamWriter = new StreamWriter(stream))    
                    {
                        streamWriter.Write(dataToStore);
                    }
                }

                GameData verifiedGameData = Load(profileId);
                if (verifiedGameData != null)
                {
                    File.Copy(fullPath,backupFilePath, true);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
                
            }
        }
        
        // simple implementation of XOR Algoritm
        private string EncryptDecrypt(string data)
        {
            string modifiedData = "";
            for (int i = 0; i < data.Length; i++)
            {
                modifiedData += (char)(data[i] ^ encryptionKey[i % encryptionKey.Length]);

            }

            return modifiedData;
        }

        private bool AttemptToRollBack( string fullPath)
        {
            bool sucess = false;
            string backupFilePath = fullPath + backupExtension;
            try
            {
                if (File.Exists(backupFilePath))
                {
                    File.Copy(backupFilePath, fullPath, true);
                    sucess = true;
                }
            }
            catch 
            {
                
            }
            return sucess;
        }
    }
}