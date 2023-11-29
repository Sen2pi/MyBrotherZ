using System.Collections.Generic;
using Managers.Scene_Management;
using SaveAndLoad;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SaveSlotsMenu : Menu
{
    [Header("Menu Navigation")] [SerializeField]
    private MainMenu mainMenu;

    [Header("Back Menu Button")] [SerializeField]
    private Button backButton;

    [Header("Confirmation PopUp")] [SerializeField]
    private PopUp confirmationPopUp;

    private SaveSlot[] slots;
    private bool isLoadingGame = false;

    private void Awake()
    {
        slots = this.GetComponentsInChildren<SaveSlot>();
    }


    public void OnBackClicked()
    {
        mainMenu.ActivateMenu();
        this.DeactivateMenu();
    }

    public void ActivateMenu(bool isLoadingGame)
    {
        this.gameObject.SetActive(true);
        this.isLoadingGame = isLoadingGame;
        Dictionary<string, GameData> profilesData = DataPersistenceManager.Instance.getAllProfilesGameData();

        backButton.interactable = true;

        GameObject firstSelectedButton = backButton.gameObject;
        foreach (SaveSlot slot in slots)
        {
            GameData profileData = null;
            profilesData.TryGetValue(slot.GetProfileId(), out profileData);
            slot.SetData(profileData);
            if (profileData == null && isLoadingGame)
            {
                slot.SetInteractable(false);
            }
            else
            {
                slot.SetInteractable(true);
                if (firstSelectedButton.Equals(backButton.gameObject))
                {
                    firstSelectedButton = slot.gameObject;
                }
            }
        }

        StartCoroutine(this.SetFirstSelectedRoutine(firstSelectedButton));
    }

    public void OnClearClicked(SaveSlot saveSlot)
    {
        DisableMenuButtons();
        confirmationPopUp.ActivateMenu(
            "Are you sure you want to clear this slot?",
            //YES
            () => {  DataPersistenceManager.Instance.DeleteProfileData(saveSlot.GetProfileId());
                ActivateMenu(isLoadingGame);},
            //NO
            () => { ActivateMenu(isLoadingGame);});
       
    }

    public void OnSaveClicked(SaveSlot saveSlot)
    {
        DisableMenuButtons();
        if (isLoadingGame)
        {
            DataPersistenceManager.Instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
            SaveGameAndLoadScene();
        }
        else if (saveSlot.HasData)
        {
            confirmationPopUp.ActivateMenu(
                "Starting a New Game with this Slot will override the currently Save Data , are you sure? ",
                // if we say yes 
                () =>
                {
                    DataPersistenceManager.Instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
                    DataPersistenceManager.Instance.NewGame();
                    SaveGameAndLoadScene();
                },
                // if we say no
                () => { this.ActivateMenu(isLoadingGame); });
        }
        else
        {
            DataPersistenceManager.Instance.ChangeSelectedProfileId(saveSlot.GetProfileId());
            DataPersistenceManager.Instance.NewGame();
            SaveGameAndLoadScene();
        }
    }

    private void SaveGameAndLoadScene()
    {
        DataPersistenceManager.Instance.SaveGame();
        SceneManager.LoadSceneAsync("Scene_2");
    }

    private void DisableMenuButtons()
    {
        foreach (SaveSlot slot in slots)
        {
            slot.SetInteractable(false);
        }

        backButton.interactable = false;
    }

    public void DeactivateMenu()
    {
        this.gameObject.SetActive(false);
    }
}