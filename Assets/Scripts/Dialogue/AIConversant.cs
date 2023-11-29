using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization; 
using UnityEngine.UI;

namespace MyBrotherZ.Dialogue
{
    public class AIConversant : MonoBehaviour
    {
        [SerializeField] private float conversationDistance = 2.0f;
        [SerializeField] Dialogue currentDialogue;
        [SerializeField] private string AIConversantName;
        private PlayerConversant playerConversant;
        private bool isSpeaking = false;
        [SerializeField] private Transform ballonTransform;
        private void Update()
        {
            InteractOnApproach();
        }

        private void InteractOnApproach()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            
            if (player != null && Vector2.Distance(transform.position, player.transform.position) <= conversationDistance)
            {
                if (currentDialogue != null)
                {
                    if (!isSpeaking)
                    {
                        ballonTransform.gameObject.SetActive(true);
                    }
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                       
                    StartConversation();
                }
            }
            else
            {
                isSpeaking = false;
                ballonTransform.gameObject.SetActive(false);
            }
        }

        public string GetName()
        {
            return AIConversantName;
        }
        private void StartConversation()
        {
            if (currentDialogue != null)
            {
                isSpeaking = true;
                ballonTransform.gameObject.SetActive(false);
                playerConversant = PlayerConversant.Instance;
                playerConversant.StartDialogue(this,currentDialogue);
            }
            
        }
    }
}