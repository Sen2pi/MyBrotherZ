using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MyBrotherZ.Quest
{
    public class QuestItemUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI progress;
        
        QuestStatus status;
        
        public void Setup(QuestStatus status)
        {
            
            this.status = status;
            if (status != null && status.GetQuest() != null)
            {
                title.text = status.GetQuest().GetTitle();
                progress.text = $"{status.GetCompletedCount()} / {status.GetQuest().GetObjectivesCount()}";
            }
        }

        public QuestStatus GetQuestStatus()
        {
            return status;
        }
    }
}
