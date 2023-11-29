using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class ItemClass : ScriptableObject, ISerializationCallbackReceiver
{
    [Tooltip("Auto-generated UUID for saving/loading. Clear this field if you want to generate a new one.")]
    public string itemID = null;
    [Tooltip("Item name to be displayed in UI.")]
    public string itemName;
    [Tooltip("The UI icon to represent this item in the inventory.")]
    public Sprite itemIcon;
    [Tooltip("Item description to be displayed in UI.")]
    public string itemDescription;
    [Tooltip("If true, multiple items of this type can be stacked in the same inventory slot.")]
    public bool isStackable = true;
    [Tooltip("If the item was collected")]
    public bool collected = false;
    [Tooltip("Item price.")][Min(1)]
    public float price = 1;
    //state
    static Dictionary<string, ItemClass> itemLookupCache;
    
    public virtual void Use(PlayerController player)
    {
        Debug.Log($"Used Item {itemName}");
    }
    
    public virtual ItemClass GetItem() { return this; }
    public virtual Tool GetTool() { return null; }
    public virtual Miscelaneous GetMisc() { return null; }
    public virtual Consumable GetConsumable() { return null; }

    [CanBeNull]
    public static ItemClass GetFromID(string itemID)
    {
        if (itemLookupCache == null)
        {
            itemLookupCache = new Dictionary<string, ItemClass>();
            var itemList = Resources.LoadAll<ItemClass>("");
            foreach (var item in itemList)
            {
                if (itemLookupCache.ContainsKey(item.itemID))
                {
                    Debug.LogError(string.Format("Looks like there's a duplicate GameDevTV.UI.InventorySystem ID for objects: {0} and {1}", itemLookupCache[item.itemID], item));
                    continue;
                }

                itemLookupCache[item.itemID] = item;
            }
        }

        if (itemID == null || !itemLookupCache.ContainsKey(itemID)) return null;
        return itemLookupCache[itemID];
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        // Generate and save a new UUID if this is blank.
        if (string.IsNullOrWhiteSpace(itemID))
        {
            itemID = System.Guid.NewGuid().ToString();
        }
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        // Require by the ISerializationCallbackReceiver but we don't need
        // to do anything with it.
    }
}