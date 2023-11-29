using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "CreaftingRecipe", menuName = "Crafting/Recipe")]
public class CraftingRecipe : ScriptableObject
{
    [Header("Crafting Recipes")] public Slot[] inputItems;
    public Slot outputItem;
    public Sprite icon;

    public bool CanCraft(InventoryManager inventory)
    {
        for (int i = 0; i < inputItems.Length; i++)
        {
            if (inventory.HasItem(inputItems[i].Item) && inputItems[i].Quantity <= inventory.GetQuantity(inputItems[i].Item))
            {
                return true;
            }
        }
        if (inventory.IsFull())
        {
            return false;
        }
        return false;
    }

    public string RecipeDescription()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < inputItems.Length; i++)
        {
            sb.Append($"\n{inputItems[i].Quantity} x {inputItems[i].Item.name}");
        }

        return sb.ToString();
    }

    public void Craft(InventoryManager inventory)
    {
        inventory.RefresUI();
        if (CanCraft(inventory))
        {
            for (int i = 0; i < inputItems.Length; i++)
            {
                inventory.Remove(inputItems[i].Item, inputItems[i].Quantity);
            }

            inventory.Add(outputItem.Item, outputItem.Quantity);
        }
        
    }
}