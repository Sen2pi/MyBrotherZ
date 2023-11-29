using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBrotherZ.Quest
{
    public class QuestGiver : MonoBehaviour
    {
        [SerializeField] private Quest quest;

        public void GiveQuest()
        {
            QuestList questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>(); 
            questList.AddQuest(quest);
        }
    }
}