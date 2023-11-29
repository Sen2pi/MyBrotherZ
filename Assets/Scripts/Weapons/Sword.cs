using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Singleton<Sword>,IWeapon
{
    
    [SerializeField] private WeaponInfo weaponInfo;
    [SerializeField] private ItemClass scriptable;
    private GameObject slash;
    private Animator slashAnim;




    private Transform weaponCollider;


    protected override void Awake()
    {
        base.Awake();
        slash = PlayerController.Instance.transform.GetChild(6).gameObject;
        slashAnim = slash.GetComponent<Animator>();
        if (InventoryManager.Instance.Contains(scriptable) != null)
        {
            transform.gameObject.SetActive(true);
            
        }
        else if (InventoryManager.Instance.Contains(scriptable) == null)
        {
            transform.gameObject.SetActive(false);
        }
    }
    private void Start()
    {
        weaponCollider = PlayerController.Instance.GetWeaponCollider();

        if (InventoryManager.Instance.Contains(scriptable) != null)
        {
            transform.gameObject.SetActive(true);
        }
        else if (InventoryManager.Instance.Contains(scriptable) == null)
        {
            transform.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (InventoryManager.Instance.Contains(scriptable) != null)
        {
            transform.gameObject.SetActive(true);
        }
        else if (InventoryManager.Instance.Contains(scriptable) == null)
        {
          
            transform.gameObject.SetActive(false);
        }
        MouseFollowOffset();
    }



    public void Attack()
    {
        if (InventoryManager.Instance.Contains(scriptable) != null)
        {
           
            slash.gameObject.SetActive(true);
            transform.gameObject.SetActive(true);
            if (Stamina.Instance.CurrentStamina > 0)
            {
                PlayerController.Instance.GetComponent<Animator>().SetTrigger("Attack");
                slashAnim.SetTrigger("Attack");
                Stamina.Instance.UseStamina();
        
                weaponCollider.gameObject.SetActive(true);
            }
        }
        else if (InventoryManager.Instance.Contains(scriptable) == null)
        {
            transform.gameObject.SetActive(false);
            slash.gameObject.SetActive(false);
        }
       
        
    }


    public WeaponInfo GetWeaponInfo()
    {
        return weaponInfo;
    }


    public void StopAttackAnim()
    {
        weaponCollider.gameObject.SetActive(false);
    }

    private void MouseFollowOffset()
    {

        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(PlayerController.Instance.transform.position);

        if (mousePos.x > playerScreenPoint.x)
        {
           
            weaponCollider.transform.rotation = Quaternion.Euler(0, -180, 0);
        }
        else if (mousePos.x < playerScreenPoint.x)
        {
           
            weaponCollider.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        
        if(mousePos.y < playerScreenPoint.y && mousePos.x > playerScreenPoint.x)
        {
        
            // Weapon faces upward
            weaponCollider.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else if (mousePos.y > playerScreenPoint.y && mousePos.x < playerScreenPoint.x)
        {
       
            // Weapon faces downward
            weaponCollider.transform.rotation = Quaternion.Euler(0, 0, -90);
        }


    }
}
