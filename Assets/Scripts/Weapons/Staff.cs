using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staff : MonoBehaviour, IWeapon
{
    [SerializeField] private WeaponInfo weaponInfo;
    [SerializeField] private GameObject magicLaser;
    [SerializeField] private Transform magicLaserSpawnPoint;
    [SerializeField] private ItemClass scriptable;
    private Transform weaponCollider;

    private Animator animator;
    private SpriteRenderer sprite;
    

    readonly int ATTACK_HASH = Animator.StringToHash("Attack");
    private void Start()
    {
        weaponCollider = PlayerController.Instance.GetWeaponCollider();
    }
    private void Awake()
    {
      
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
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
        if (PlayerController.Instance.GetComponent<SpriteRenderer>().flipX == true)
        {
            Debug.Log("Entrou");
            sprite.flipY = true;
        }
        else
        {
            Debug.Log("Saiu");
            sprite.flipY = false;
        }

    }

    public void Attack()
    {
        if (InventoryManager.Instance.Contains(scriptable) != null)
        {
            transform.gameObject.SetActive(true);
            PlayerController.Instance.GetComponent<Animator>().SetTrigger("Attack");
            animator.SetTrigger(ATTACK_HASH);
        }
        else if (InventoryManager.Instance.Contains(scriptable) == null)
        {
            transform.gameObject.SetActive(false);
        }
        
    }
    public void SpawnStaffProjectileAnimEvent()
    {
        GameObject newLaser = Instantiate(magicLaser, magicLaserSpawnPoint.position, Quaternion.identity);
        newLaser.GetComponent<Laser>().UpdateLaserRange(weaponInfo.weaponRange);
    }

    public WeaponInfo GetWeaponInfo()
    {
        return weaponInfo;
    }
}
