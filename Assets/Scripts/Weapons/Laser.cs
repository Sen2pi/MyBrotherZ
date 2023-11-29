using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private float laserGrowTime= 2f;
    private SpriteRenderer sprite;
    private float laserRange;
    private CapsuleCollider2D customCollider;
    private bool isGrowing = true;
    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        customCollider = GetComponent<CapsuleCollider2D>();
    }

    void Start()
    {
        LaserFaceMouse();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Indestructable>()&& !collision.isTrigger)
        {
            isGrowing = false;
        } 
    }
    public void UpdateLaserRange(float laserRange)
    {
        this.laserRange = laserRange;
        StartCoroutine(IncreaseLaserLenghtRoutine());
    }
    private IEnumerator IncreaseLaserLenghtRoutine()
    {
        float timePassed = 0f;
        while (sprite.size.x < laserRange && isGrowing)
        {
            timePassed += Time.deltaTime;
            float linearT = timePassed / laserGrowTime;

            sprite.size = new Vector2(Mathf.Lerp(1f, laserRange, linearT), 1f);
            customCollider.size = new Vector2(Mathf.Lerp(1f, laserRange, linearT), customCollider.size.y);
            customCollider.offset = new Vector2((Mathf.Lerp(1f, laserRange, linearT))/2, customCollider.offset.y);
            yield return null;
        }
        StartCoroutine(GetComponent<SpriteFade>().SlowFadeRoutine());
    }
    private void LaserFaceMouse()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector2 direction = transform.position - mousePosition;
        transform.right = -direction;
    }
    

  
}
