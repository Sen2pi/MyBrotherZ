using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "new Tool", menuName = "Item/Tool")]
public class Tool : ItemClass
{
    public ToolType toolType;
    public GameObject prefab;

    public enum ToolType
    {
        weapon,
        pickaxe,
        hammer,
        axe,
        chanBreaker,
        saw,
        torch,
        key,
    }

    public override Tool GetTool() { return this; }

    public override void Use(PlayerController player)
    {
        base.Use(player);
        switch (toolType)
        {
            case ToolType.weapon:
                //Weapon Stuf
                break;
            case ToolType.pickaxe:
                //pickaxe Stuf
                break;
            case ToolType.hammer:
                //hammer Stuf
                break;
            case ToolType.axe:
                //axe Stuf
                break;
            case ToolType.chanBreaker:
                //chain breaker Stuf
                break;
            case ToolType.saw:
                //saw Stuf
                break;
            case ToolType.torch:
                Instantiate(prefab,PlayerController.Instance.transform.position, Quaternion.identity);
                InventoryManager.Instance.Remove(this);
                break;
            case ToolType.key:
                break;
           
        }
    }
}
