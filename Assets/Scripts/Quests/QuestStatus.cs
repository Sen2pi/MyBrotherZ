using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace MyBrotherZ.Quest
{
    [Serializable]
    public class QuestStatus
    {
        Quest quest;
        private List<string> completedObjectives = new List<string>();

        public IEnumerable<string> GetCompletedObjectives()
        {
            return completedObjectives;
        }
        public QuestStatus(Quest quest)
        {
            this.quest = quest;
        }

        public QuestStatus(object objectState)
        {
            QuestStatusRecord state = objectState as QuestStatusRecord;
            quest = Quest.GetByName(state.questName);
            completedObjectives = state.completedObjectives;
        }

        public Quest GetQuest()
        {
            return quest;
        }
        
        public int GetCompletedCount()
        {
            return completedObjectives.Count;
        }

        public bool IsCompleted(string objective)
        {
            return completedObjectives.Contains(objective);
        }

        public void CompleteObjective(string objective)
        {
            if(quest.HasObjective(objective))
            {
                completedObjectives.Add(objective);
            }
            
        }
        
        public QuestStatusRecord CaptureState()
        {
           QuestStatusRecord state = new QuestStatusRecord();
           state.questName = quest.name;
           state.completedObjectives = completedObjectives;
           return state;
           
        }

        public bool IsComplete()
        {
            foreach (var objective in quest.GetObjectives())
            {
                if(!completedObjectives.Contains(objective.reference))
                {
                    return false;
                }
            }
            
            return true;
        }
    }
    
    [System.Serializable]
    public class QuestStatusRecord
    {
        public string questName;
        public List<string> completedObjectives;
    }
}
