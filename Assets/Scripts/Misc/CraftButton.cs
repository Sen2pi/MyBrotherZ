using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftButton : MonoBehaviour
{
    private InventoryManager inventory;

    private void Awake()
    {
        inventory = InventoryManager.Instance;
    }

    public void Craft()
    {
        InventoryManager.Instance.Craft();
    }
}
