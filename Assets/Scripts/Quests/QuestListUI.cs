using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBrotherZ.Quest
{
    public class QuestListUI : MonoBehaviour
    {
        [SerializeField] QuestItemUI questPrefab;
        private QuestList questList;
        private void Start()
        {
            questList = GameObject.FindGameObjectWithTag("Player").GetComponent<QuestList>();
            questList.onUpdate += Redraw;
            Redraw();
        }

        private void Redraw()
        {
            foreach (Transform item in transform)
            {
                Destroy(item.gameObject);
            }
           
            foreach (QuestStatus status in questList.GetStatuses())
            {
                QuestItemUI questUI = Instantiate<QuestItemUI>(questPrefab, transform);
                questUI.Setup(status);
            }
            
        }
    }
}
