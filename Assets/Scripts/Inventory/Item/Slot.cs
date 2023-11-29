using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Slot
{
    
    [SerializeField] private ItemClass item;
    [SerializeField] private int quantity;
    public ItemClass Item { get => item; set => item = value; }
    public int Quantity { get => quantity; set =>  quantity = value; }
    
    public Slot()
    {

        Item = null;
        Quantity = 0;
    }

    public Slot(ItemClass item, int quantity)
    {
       
        Item = item;
        Quantity = quantity;
    }

    public Slot(Slot slot)
    {
        
        this.Item = slot.Item;
        this.Quantity = slot.Quantity;
    }
    
    public void Clear()
    {
        
        Item = null;
        Quantity = 0;
    }
    public void AddItem(ItemClass item, int quantity)
    {
        Item = item;
        Quantity = quantity;
    }


}
