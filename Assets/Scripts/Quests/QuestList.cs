using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MyBrotherZ.Quest
{
    [Serializable]
    public class QuestList : Singleton<QuestList>, IDataPersisence, IPredicateEvaluator
    {
        private List<QuestStatus> statuses = new List<QuestStatus>();

        private List<QuestStatus> completedQuests = new List<QuestStatus>();

        // Start is called before the first frame update
        public event Action onUpdate;

        public IEnumerable<QuestStatus> GetStatuses()
        {
            return statuses;
        }

        public void AddQuest(Quest quest)
        {
            if (HasQuest(quest)) return;
            QuestStatus newstatus = new QuestStatus(quest);
            statuses.Add(newstatus);
            if (onUpdate != null)
            {
                onUpdate();
            }
        }

        public bool HasQuest(Quest quest)
        {
            return GetQuestStatus(quest) != null;
        }

        public void CompleteObjective(Quest quest, string objective)
        {
            QuestStatus status = GetQuestStatus(quest);
            status.CompleteObjective(objective);
            if (status.IsComplete())
            {
                GiveReward(quest);
                completedQuests.Add(status);
                statuses.Remove(status);
            }

            if (onUpdate != null)
            {
                onUpdate();
            }
        }

        public bool HasCompletedQuest(Quest quest)
        {
            return completedQuests.Contains(GetQuestStatus(quest));
        }

        private void GiveReward(Quest quest)
        {
            foreach (Quest.Reward reward in quest.GetRewards())
            {
                InventoryManager.Instance.Add(reward.item, reward.number);
            }
        }

        private QuestStatus GetQuestStatus(Quest quest)
        {
            foreach (QuestStatus status in statuses)
            {
                if (status.GetQuest() == quest)
                {
                    return status;
                }
            }

            return null;
        }

        public void LoadData(GameData data)
        {
            try
            {
                List<QuestStatusRecord> stateList = new List<QuestStatusRecord>();
                for (int i = 0; i < data.quests.Length; i++)
                {
                    stateList.Add(data.quests[i]);
                }

                if (stateList == null)
                {
                    return;
                }

                List<QuestStatusRecord> completedList = new List<QuestStatusRecord>();
                for (int i = 0; i < data.completedQuests.Length; i++)
                {
                    completedList.Add(data.completedQuests[i]);
                }

                completedQuests.Clear();
                statuses.Clear();
                foreach (QuestStatusRecord objectState in stateList)
                {
                    statuses.Add(new QuestStatus(objectState));
                }

                foreach (QuestStatusRecord objectState in completedList)
                {
                    completedQuests.Add(new QuestStatus(objectState));
                }
            }
            catch
            {
                return;
            }
        }


        public void SaveData(ref GameData data)
        {
            int counter = 0;
            data.quests = new QuestStatusRecord[statuses.Count];

            foreach (QuestStatus status in statuses)
            {
                data.quests[counter] = status.CaptureState();
                counter++;
            }

            data.completedQuests = new QuestStatusRecord[completedQuests.Count];
            counter = 0;
            foreach (QuestStatus quest in completedQuests)
            {
                data.completedQuests[counter] = quest.CaptureState();
                counter++;
            }


            Debug.Log("Save: " + data.quests.Length);
            Debug.Log("Save Quests Completed: " + data.completedQuests.Length);
        }

        public bool? Evaluate(string predicate, string[] parameters)
        {
            switch (predicate)
            {
                case "HasQuest":
                    return HasQuest(Quest.GetByName(parameters[0]));
                case "CompletedQuest":
                    return GetQuestStatus(Quest.GetByName(parameters[0])).IsComplete();
                case "HasCompletedQuest":
                    return HasCompletedQuest(Quest.GetByName(parameters[0]));
            }

            return null;
        }
    }
}