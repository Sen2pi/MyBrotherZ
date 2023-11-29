using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerAmmo : Singleton<PlayerAmmo>, IDataPersisence
{
    [SerializeField] private TMP_Text bulletCount;
    [SerializeField] private TMP_Text ArrowCount;
    public bool HaveAmmo { get; private set; } = true;
    private int currentAmmo;
    [SerializeField] private ItemClass BulletScriptable;
    public bool HaveArrow { get; private set; } = true;
    private int currentArrows;
    [SerializeField] private ItemClass ArrowScriptable;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        currentAmmo = InventoryManager.Instance.Contains(BulletScriptable).Quantity;
        InventoryManager.Instance.Contains(BulletScriptable).Quantity = currentAmmo;
        InventoryManager.Instance.RefresUI();
        currentArrows = InventoryManager.Instance.Contains(ArrowScriptable).Quantity;
        InventoryManager.Instance.Contains(ArrowScriptable).Quantity = currentArrows;
        InventoryManager.Instance.RefresUI();
        HaveAmmo = true;
    }

    public void UpdateBulletCountText()
    {
        if (!HaveAmmo)
        {
            InventoryManager.Instance.Remove(BulletScriptable);
            InventoryManager.Instance.RefresUI();
        }
        bulletCount.text = currentAmmo.ToString();
    }
    public void UpdateArrowCountText()
    {
        if (!HaveArrow)
        {
            
            InventoryManager.Instance.Remove(ArrowScriptable);
            InventoryManager.Instance.RefresUI();
        }
        
        ArrowCount.text = currentArrows.ToString();
    }

    public void CollectAmmo()
    {
        InventoryManager.Instance.Contains(BulletScriptable).Quantity += 1;
        InventoryManager.Instance.RefresUI();
        currentAmmo += 1;
        HaveAmmo = true;
    }
    
    public void CollectArrow()
    {
        InventoryManager.Instance.Contains(ArrowScriptable).Quantity += 1;
        InventoryManager.Instance.RefresUI();
        currentArrows += 1;
        HaveArrow = true;
    }

    public void ConsumeAmmo()
    {
        
        if (HaveAmmo)
        {
            InventoryManager.Instance.Contains(BulletScriptable).Quantity -= 1;
            InventoryManager.Instance.RefresUI();
            currentAmmo -= 1;
        }
    }
    
    public void ConsumeArrow()
    {
        if (HaveArrow)
        {
            InventoryManager.Instance.Contains(ArrowScriptable).Quantity -= 1;
            InventoryManager.Instance.RefresUI();
            currentArrows -= 1;
        }
    }

    public void CheckIfHaveAmmo()
    {
        if (currentAmmo <= 0)
        {
            currentAmmo = 0;
            HaveAmmo = false;
        }
       
    }
    
    public void CheckIfHaveArrow()
    {
        if (currentArrows <= 0)
        {
            currentArrows = 0;
            HaveArrow = false;
        }
    }

    public void LoadData(GameData data)
    {
        this.currentAmmo = data.currentAmmo;
        this.currentArrows = data.currentArrows;
    }

    public void SaveData(ref GameData data)
    {
        data.currentAmmo = this.currentAmmo;
        data.currentArrows = this.currentArrows;
    }
}