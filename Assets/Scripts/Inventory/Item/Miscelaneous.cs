using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "new Miscelaneous", menuName = "Item/Miscelaneous")]
public class Miscelaneous : ItemClass
{
    public MiscType miscType;
    public enum MiscType
    {
        wood,
        plant,
        Bottle,
        ZCoin,
        FoodIngredient,
        WeaponIngredient,
        ToolIngredient,
        ZBlood,
        QuestIngredient
    }

  
    public override Miscelaneous GetMisc()
    { return this; }

}