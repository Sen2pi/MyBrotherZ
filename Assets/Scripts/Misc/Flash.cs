using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flash : MonoBehaviour
{
    [SerializeField] private Material whiteFlashMat;
    [SerializeField] private float restoreDefaultMatTime = .2f;
    private Material defaultMat;
    private SpriteRenderer sp;
    private void Awake()
    {
        sp = GetComponent<SpriteRenderer>();
        defaultMat = sp.material;
    }

    public IEnumerator FlashRoutine()
    {
        sp.material = whiteFlashMat;
        yield return new WaitForSeconds(restoreDefaultMatTime);
        sp.material = defaultMat;
    }

    public float GetRestoreMatTime()
    {
        return restoreDefaultMatTime;
    }
}
