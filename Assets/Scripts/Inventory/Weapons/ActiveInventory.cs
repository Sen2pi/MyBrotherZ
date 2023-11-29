using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveInventory : Singleton<ActiveInventory>
{
    private int activeSlotIndexNum = 0;
    public int ActiveSlotIndexNum{ get => activeSlotIndexNum; private set => activeSlotIndexNum = value; }
    private PlayerControls playerControls;

    protected override void Awake()
    {
        base.Awake();
        playerControls = new PlayerControls();
    }
    private void Start()
    {
        playerControls.Inventory.Keybord.performed += ctx =>ToggleActiveSlot((int)ctx.ReadValue<float>());
        
    }

    public void EquipStartingWeapon()
    {
        ToggleActiveHighLight(0);
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void ToggleActiveSlot(int numValue)
    {
        
        ToggleActiveHighLight(numValue-1);
    }
    private void ToggleActiveHighLight(int indexNum)
    {
        activeSlotIndexNum = indexNum;
        if (activeSlotIndexNum == 0)
        {
            PlayerController.Instance.gameObject.GetComponent<Animator>().SetTrigger("SwapSword");
            PlayerController.Instance.gameObject.GetComponent<Animator>().SetBool("Sword",true);
            PlayerController.Instance.gameObject.GetComponent<Animator>().SetBool("Bow",false);
            PlayerController.Instance.gameObject.GetComponent<Animator>().SetBool("MK9",false);
            PlayerController.Instance.gameObject.GetComponent<Animator>().SetBool("Laser",false);
        }else if (activeSlotIndexNum == 1)
        {
            PlayerController.Instance.gameObject.GetComponent<Animator>().SetTrigger("SwapMK9");
            PlayerController.Instance.transform.GetComponent<Animator>().SetBool("Sword",false);
            PlayerController.Instance.transform.GetComponent<Animator>().SetBool("Bow",false);
            PlayerController.Instance.transform.GetComponent<Animator>().SetBool("MK9",true);
            PlayerController.Instance.transform.GetComponent<Animator>().SetBool("Laser",false);
            
        }
        else if(activeSlotIndexNum == 2)
        {
            PlayerController.Instance.gameObject.GetComponent<Animator>().SetTrigger("SwapLaser");
            PlayerController.Instance.transform.GetComponent<Animator>().SetBool("Sword",false);
            PlayerController.Instance.transform.GetComponent<Animator>().SetBool("Bow",false);
            PlayerController.Instance.transform.GetComponent<Animator>().SetBool("MK9",false);
            PlayerController.Instance.transform.GetComponent<Animator>().SetBool("Laser",true);
        }else if(activeSlotIndexNum == 3)
        {
            PlayerController.Instance.gameObject.GetComponent<Animator>().SetTrigger("SwapBow");
            PlayerController.Instance.transform.GetComponent<Animator>().SetBool("Sword",false);
            PlayerController.Instance.transform.GetComponent<Animator>().SetBool("Bow",true);
            PlayerController.Instance.transform.GetComponent<Animator>().SetBool("MK9",false);
            PlayerController.Instance.transform.GetComponent<Animator>().SetBool("Laser",false);
        }
        foreach(Transform inventorySlot in this.transform)
        {
            inventorySlot.GetChild(0).gameObject.SetActive(false);
        }
        this.transform.GetChild(indexNum).GetChild(0).gameObject.SetActive(true);
        ChangeActiveWeapon();
    }

    private void ChangeActiveWeapon()
    {
        if (PlayerHealth.Instance.IsDead) { return; }   
        
        Transform childTransform = transform.GetChild(activeSlotIndexNum);
        InventorySlot inventorySlot = childTransform.GetComponentInChildren<InventorySlot>();
        WeaponInfo weaponInfo = inventorySlot.GetWeaponInfo();
        GameObject weaponToSpawn = weaponInfo.weaponPrefab;

        if (ActiveWeapon.Instance.CurrentActiveWeapon != null)
        {
            Destroy(ActiveWeapon.Instance.CurrentActiveWeapon.gameObject);
        }
        if (weaponInfo==null)
        {
            ActiveWeapon.Instance.WeaponNull();
            return;
        }
        
        
        GameObject newWeapon = Instantiate(weaponToSpawn, ActiveWeapon.Instance.transform.position, Quaternion.identity);
        ActiveWeapon.Instance.transform.rotation = Quaternion.Euler(0, 0, 0);
        newWeapon.transform.parent = ActiveWeapon.Instance.transform;
        ActiveWeapon.Instance.NewWeapon(newWeapon.GetComponent<MonoBehaviour>());
    }
}
