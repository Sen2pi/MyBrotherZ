using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBrotherZ.Quest
{
    public class QuestToolTipSpawner : TooltipSpawner
    {
        public override void UpdateTooltip(GameObject tooltip)
        {
            QuestStatus status = GetComponent<QuestItemUI>().GetQuestStatus();
            tooltip.GetComponent<QuestUITooltip>().Setup(status);
        }

        public override bool CanCreateTooltip()
        {
            return true;
        }
    }
}



