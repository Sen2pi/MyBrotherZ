using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyBrotherZ.Quest;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MyBrotherZ.Dialogue
{
    
    public class PlayerConversant : Singleton<PlayerConversant>
    { 
        Dialogue currentDialogue;
        DialogueNode currentNode = null;
        AIConversant currentConversant = null;
        private bool isChoosing = false;
        private string playerName = "Karim";

        public void StartDialogue(AIConversant newConversant, Dialogue newDialogue)
        {
            
            currentConversant = newConversant;
            currentDialogue = newDialogue;
            if (currentDialogue != null)
            {
                currentNode = currentDialogue.GetRootNode();
                TriggerEnterAction();
                onConversationUpdated();
            }
        }

        public event Action onConversationUpdated;
        

        public void Quit()
        {
            TriggerExitAction();
            currentDialogue = null;
            currentNode = null;
            isChoosing = false;
            currentConversant.gameObject.GetComponent<DialogueTrigger>().Trigger(currentConversant.gameObject.GetComponent<DialogueTrigger>().GetAction());
            currentConversant = null;
            onConversationUpdated();
        }
        public bool isChosing()
        {
            return isChoosing;
        }
        public string GetText()
        {
            if (currentNode == null)
            {
                return "";
            }
            return currentNode.GetText();
        }

        public IEnumerable<DialogueNode> GetChoices()
        {
            return FilterCondition(currentDialogue.GetPlayerChildren(currentNode));
        }

        public void SelectChoice(DialogueNode choice)
        {
            currentNode = choice;
            TriggerEnterAction();
            isChoosing = false;
            Next();
        }

        public bool isActive()
        {
            return currentDialogue != null;
        }
        public void Next()
        {
            int numPlayerResponses = FilterCondition(currentDialogue.GetPlayerChildren(currentNode)).Count();
            if (numPlayerResponses > 0)
            {
                isChoosing = true;
                TriggerExitAction();
                onConversationUpdated();
                return;
            }
            DialogueNode[] children = FilterCondition(currentDialogue.GetAIChildren(currentNode)).ToArray();
            int randomIndex = UnityEngine.Random.Range(0, children.Count());
            TriggerExitAction();
            currentNode = children[randomIndex];
            TriggerEnterAction();
            onConversationUpdated();
        }

        public bool HasNext()
        {
            return FilterCondition(currentDialogue.GetAllChildren(currentNode)).Count() > 0;
        }

        private void TriggerEnterAction()
        {
            if (currentNode != null )
            {
                currentConversant.gameObject.GetComponent<Animator>().SetBool("isWalking", false);
              TriggerAction(currentNode.GetOnEnterAction());
            }
        }

        private IEnumerable<DialogueNode> FilterCondition(IEnumerable<DialogueNode> inputNode)
        {
            foreach (var node in inputNode)
            {
                if (node.Checkcondition(GetEvaluators()))
                {
                    yield return node;
                }
            }
        }

        private IEnumerable<IPredicateEvaluator> GetEvaluators()
        {
            
            // Instantiate the InventoryManager and QuestList objects
            IPredicateEvaluator inventoryManager = InventoryManager.Instance;
            IPredicateEvaluator questList = QuestList.Instance;

            // Return the list of evaluators
            return new List<IPredicateEvaluator> { inventoryManager, questList };
        }

        private void TriggerExitAction()
        {
            if (currentNode != null )
            {
                TriggerAction(currentNode.GetOnExitAction());
            }
        }

        private void TriggerAction(string action)
        {
            if (action == "") return;
            foreach (DialogueTrigger trigger in currentConversant.GetComponents<DialogueTrigger>())
            {
                trigger.Trigger(action);
            }
        }

        public string GetCurrentConversantName()
        {
            if (isChoosing)
            {
                return playerName;
            }
            else
            {
                return currentConversant.GetName();
                
            }
        }
    }
}
