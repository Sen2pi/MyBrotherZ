using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DisableLightDay : MonoBehaviour
{
    [SerializeField] Light2D myLight;
    [SerializeField]private Sprite bonFireOn;
    [SerializeField]private Sprite bonFireOff;
    [SerializeField] private GameObject effectOn;
    [SerializeField] private ParticleSystem effectOff;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (DayAndNightCicle.Instance.IsDay())
        {
            
            animator.SetTrigger("Off");
            myLight.gameObject.SetActive(false);
            GetComponent<SpriteRenderer>().sprite = bonFireOff;
            if (transform.childCount > 1)
            {
                StopAllCoroutines();
                StartCoroutine(EffectRoutine());
            }
        }
        else
        {
            animator.SetTrigger("On");
            myLight.gameObject.SetActive(true);
            if (transform.childCount > 1)
            {
                StopAllCoroutines();
                StartCoroutine(EffectRoutine());
            }
        }
    }

    private IEnumerator EffectRoutine()
    {
        if (DayAndNightCicle.Instance.IsDay())
        {
            effectOn.gameObject.SetActive(false);
            effectOff.gameObject.SetActive(true);
            yield return new WaitForSeconds(3f);
            effectOff.gameObject.SetActive(false);
        }
        else
        {
            effectOff.gameObject.SetActive(false);
            effectOn.gameObject.SetActive(true);
            
        }
        
    }
}
