using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBrotherZ.Quest
{
    [CreateAssetMenu(fileName = "New Quest", menuName = "Quest/Quest")]
    public class Quest : ScriptableObject
    {
        [SerializeField] private List<Objective> objectives = new List<Objective>();
        [SerializeField] private List<Reward> rewards = new List<Reward>();
        
        [Serializable]
        public class Reward
        {
            [Min(1)]
            public int number;
            public ItemClass item;
        }
        
        [Serializable]
        public class Objective
        {
            public string reference;
            public string description;
        }

    public string GetTitle()
        {
            return name;
        }

        public int GetObjectivesCount()
        {
            return objectives.Count;
        }

        public IEnumerable<Objective> GetObjectives()
        {
            return objectives;
        }
        
        public IEnumerable<Reward> GetRewards()
        {
            return rewards;
        }
        

        public bool HasObjective(string reference)
        {
            foreach (Objective objective in objectives)
            {
                if (objective.reference == reference)
                {
                    return true;
                }
            }
            return false;
        }

        public static Quest GetByName(string questName)
        {
            foreach (Quest quest in Resources.LoadAll<Quest>(""))
            {
                if (quest.name == questName)
                {
                    return quest;
                }
            }
            return null;
        }
    }

}
