using System;
using System.Collections;
using System.Collections.Generic;
using MyBrotherZ.Dialogue;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace MyBrotherZ.Dialogue
{
    public class DialogueUI : MonoBehaviour
    {
        private PlayerConversant playerConversant;
        [SerializeField] private TextMeshProUGUI AIText;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private GameObject aiResponse; 
        [SerializeField] private Transform choiceRoot;
        [SerializeField] private GameObject choicePrefab;
        [SerializeField] private TextMeshProUGUI conversantName;
        [SerializeField] Image dialogueImage;
        [SerializeField] private GameObject speakerName;
        [SerializeField] private GameObject exit;
        
        
        private void Start()
        {
            playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();
            playerConversant.onConversationUpdated += UpdateUI;
            nextButton.onClick.AddListener(Next);
            quitButton.onClick.AddListener(Quit);
            UpdateUI();
            
        }
        
        
        public void Next()
        {
            playerConversant.Next();
        }

        public void Quit()
        {
            playerConversant.Quit();

        }

        void UpdateUI()
        {
            
            exit.SetActive(playerConversant.isActive());
            speakerName.SetActive(playerConversant.isActive());
            dialogueImage.enabled = playerConversant.isActive();
            gameObject.SetActive(playerConversant.isActive());
            if(!playerConversant.isActive()) return;
            conversantName.text = playerConversant.GetCurrentConversantName();
            AIText.text = playerConversant.GetText();
            nextButton.gameObject.SetActive(playerConversant.HasNext());
            aiResponse.SetActive(!playerConversant.isChosing());
            choiceRoot.gameObject.SetActive(playerConversant.isChosing());
            if (playerConversant.isChosing())
            {
                BuildChoiceList();
            }
            else
            {
                AIText.text = playerConversant.GetText();
                nextButton.gameObject.SetActive(playerConversant.HasNext());
            }
            
        }
        
        private void BuildChoiceList()
        {
            if (choiceRoot.transform.childCount > 0)
            {
                foreach (Transform item in choiceRoot)
                {
                    Destroy(item.gameObject);
                }
            }

            foreach (DialogueNode choice in playerConversant.GetChoices())
            {
                GameObject choiceInstance = Instantiate(choicePrefab, choiceRoot);
                var textComp = choiceInstance.GetComponentInChildren<TextMeshProUGUI>();
                textComp.text = choice.GetText();
                Button button = choiceInstance.GetComponentInChildren<Button>();
                button.onClick.AddListener(() =>
                {
                    playerConversant.SelectChoice(choice);
                });
            }
        }
    }
}