using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Bow : MonoBehaviour,IWeapon
{
    [SerializeField] private WeaponInfo weaponInfo;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowSpawnPoint;
    [SerializeField] private ItemClass scriptable;
   

    readonly int FIRE_HASH = Animator.StringToHash("Fire");

    private Animator myAnimator;
    private SpriteRenderer sprite;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        myAnimator = GetComponent<Animator>();
        if(InventoryManager.Instance.Contains(scriptable) != null)
        {
            transform.gameObject.SetActive(true);
        }
        else if (InventoryManager.Instance.Contains(scriptable) == null)
        {
            transform.gameObject.SetActive(false);
        }
    }

    private void Update()
    {   if(InventoryManager.Instance.Contains(scriptable) != null)
        {
            transform.gameObject.SetActive(true);
            if (scriptable.itemName == "Metrelhadora")
            {
                PlayerAmmo.Instance.UpdateBulletCountText();
            }
            else if (scriptable.itemName == "Bow")
            {
                PlayerAmmo.Instance.UpdateArrowCountText();
            }
            
            
            
            if (PlayerController.Instance.GetComponent<SpriteRenderer>().flipX == true)
            {
                sprite.flipY = true;
            }
            else
            {
                sprite.flipY = false;
            }
        }
        else if(InventoryManager.Instance.Contains(scriptable) == null)
        {
            transform.gameObject.SetActive(false);
        }
    }

    public void Attack()
    {
        if (InventoryManager.Instance.Contains(scriptable) != null)
        {
            transform.gameObject.SetActive(true);
            
            if (scriptable.itemName =="Metrelhadora")
            {
                PlayerAmmo.Instance.CheckIfHaveAmmo();
                if (PlayerAmmo.Instance.HaveAmmo && !PlayerHealth.Instance.IsDead)
                {
                    PlayerAmmo.Instance.ConsumeAmmo();
                    PlayerController.Instance.GetComponent<Animator>().SetTrigger("Attack");
                    myAnimator.SetTrigger(FIRE_HASH);
                    GameObject newBullet = Instantiate(arrowPrefab, arrowSpawnPoint.position, ActiveWeapon.Instance.transform.rotation);
                    newBullet.GetComponent<Projectile>().UpdateProjectileRange(weaponInfo.weaponRange);
                }
            }else if (scriptable.itemName =="Bow")
            {
                PlayerAmmo.Instance.CheckIfHaveArrow();
                if (PlayerAmmo.Instance.HaveArrow && !PlayerHealth.Instance.IsDead)
                {
                    PlayerAmmo.Instance.ConsumeArrow();
                    PlayerController.Instance.GetComponent<Animator>().SetTrigger("Attack");
                    myAnimator.SetTrigger(FIRE_HASH);
                    GameObject newArrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, ActiveWeapon.Instance.transform.rotation);
                    newArrow.GetComponent<Projectile>().UpdateProjectileRange(weaponInfo.weaponRange);
                }
            }
            
        }
        else if (InventoryManager.Instance.Contains(scriptable) == null)
        {
            transform.gameObject.SetActive(false);
        }
        
    }
        

    public WeaponInfo GetWeaponInfo()
    {
        return weaponInfo;
    }

}
