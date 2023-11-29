using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockBack : MonoBehaviour
{
    private Rigidbody2D rb;
    public bool GetingKnockedBack { get; private set; }
    [SerializeField] private float knockBackTime = .02f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void GetKnockedBack(Transform damageSource, float knockBackthrust)
    {
        GetingKnockedBack = true;
        Vector2 difference = (transform.position - damageSource.position).normalized * knockBackthrust * rb.mass;
        rb.AddForce(difference, ForceMode2D.Impulse);
        StartCoroutine(KnockRoutine());
    }

    private IEnumerator KnockRoutine()
    {
        yield return new WaitForSeconds(knockBackTime);
        rb.velocity = Vector2.zero;
        GetingKnockedBack = false;
    }

}
