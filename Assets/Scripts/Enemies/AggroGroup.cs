using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class AggroGroup : MonoBehaviour
{
    [SerializeField] private EnemyAI[] enemys;
    [SerializeField] private bool activateOnStart;

    private void Start()
    {
        Active(activateOnStart);
    }

    public void Active(bool shouldActivate)
    {
        foreach (EnemyAI enemy in enemys)
        {
            enemy.enabled = shouldActivate;
        }
    }
}
