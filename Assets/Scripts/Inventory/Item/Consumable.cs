using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "new Consumable", menuName = "Item/Consumable")]
public class Consumable : ItemClass
{
    public float AmountAdded;
    [SerializeField] private int AmountHealth;
    [SerializeField] private int AmountStamina;


    public ConsumableType cType;
    public enum ConsumableType
    {
        apple,
        beer,
        water,
        cookie,
        bread,
        wine,
        cookedMeal,
        chestKey,
        Ammo,
        Arrow
    }
  
    public override Consumable GetConsumable()
    {
        return this;
    }

    public override  void Use(PlayerController player)
    {
        base.Use(player);
        switch (cType)
        {
            case ConsumableType.apple:
                if (PlayerHealth.Instance.CurrentHealth != PlayerHealth.Instance.MaxHealth)
                {
                    PlayerHealth.Instance.Heal(AmountHealth);
                    InventoryManager.Instance.Remove(this);
                }
                break;
            case ConsumableType.beer:
                //Beer Stuf
                break;
            case ConsumableType.water:
                //water Stuf
                break;
            case ConsumableType.cookie:
                //cookie Stuf
                break;
            case ConsumableType.bread:
                //breed Stuf
                break;
            case ConsumableType.wine:
                //wine Stuf
                break;
            case ConsumableType.chestKey:
                InventoryManager.Instance.Remove(this);
                break;
            case ConsumableType.cookedMeal:
                if (PlayerHealth.Instance.CurrentHealth != PlayerHealth.Instance.MaxHealth)
                {
                    PlayerHealth.Instance.Heal(AmountHealth);
                    Stamina.Instance.StaminaRefreshRat(AmountStamina);
                    InventoryManager.Instance.Remove(this);
                }
                break;
            case ConsumableType.Ammo:
                break;
            case ConsumableType.Arrow:
                break;
        }
    }
}