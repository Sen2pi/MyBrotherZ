using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Timeline;

namespace MyBrotherZ.Quest
{
   
public class QuestUITooltip : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI title;
    [SerializeField] Transform objectiveContainer;
    [SerializeField] GameObject objectivePrefab;
    [SerializeField] GameObject objectivePrefabIncomplete;
    [SerializeField] TextMeshProUGUI rewardText;
    public void Setup(QuestStatus status)
    {
        Quest quest = status.GetQuest();
        title.text = quest.GetTitle();
        foreach (Transform item in objectiveContainer)
        {
            Destroy(item.gameObject);
        }
        foreach (var objective in quest.GetObjectives())
        {
            GameObject prefab = objectivePrefabIncomplete;
            
            if (status.IsCompleted(objective.reference))
            {
                prefab = objectivePrefab;
            }
            GameObject objectivesInstance = Instantiate(prefab, objectiveContainer);
            TextMeshProUGUI objectivetext = objectivesInstance.GetComponentInChildren<TextMeshProUGUI>();
            objectivetext.text = objective.description;
        }

        rewardText.text = GetRewardText(quest);
    }

    private string GetRewardText(Quest quest)
    {
        string rewardText = "";
        foreach (var reward in quest.GetRewards())
        {
            if (rewardText != "")
            {
                rewardText += ", ";
            }
            rewardText += reward.number + " " + reward.item.itemName + "\n";
        }

        if (rewardText == "")
        {
            rewardText = "No Reward";
        }
        return rewardText;
        
    }
}
}
