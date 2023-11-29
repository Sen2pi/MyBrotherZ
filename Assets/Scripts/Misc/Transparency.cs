using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Transparency : MonoBehaviour
{
    [Range (0,1)]
    [SerializeField] private float TransparencyAmount = .8f;
    [SerializeField] private float fadeTime = .4f;
    private int initialSortingOrder;

    private SpriteRenderer sprite;
    private Tilemap tilemap;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        tilemap = GetComponent<Tilemap>();
        if (GetComponent<TilemapRenderer>())
        {
            initialSortingOrder = GetComponent<TilemapRenderer>().sortingOrder;
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerController>())
        {
            if(GetComponent<TilemapRenderer>())
            {
                GetComponent<TilemapRenderer>().sortingOrder = 0;
            }
            if (sprite)
            {
                StartCoroutine(FadeRoutine(sprite, fadeTime, sprite.color.a,TransparencyAmount));
            }
            else if (tilemap)
            {
                StartCoroutine(FadeRoutine(tilemap, fadeTime, tilemap.color.a, TransparencyAmount));
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<PlayerController>())
        {
            if (GetComponent<TilemapRenderer>())
            {

                GetComponent<TilemapRenderer>().sortingOrder = initialSortingOrder;
            }
            if (sprite)
            {
                StartCoroutine(FadeRoutine(sprite, fadeTime, sprite.color.a, 1f));
            }else if (tilemap)
            {
                StartCoroutine(FadeRoutine(tilemap, fadeTime, tilemap.color.a, 1f));
            }
            
        }
    }
    private IEnumerator FadeRoutine(SpriteRenderer sprite, float fadeTime, float startValue, float targetTransparency)
    {
        float elapsedTime = 0;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startValue, targetTransparency, elapsedTime / fadeTime);
            sprite.color = new Color(sprite.color.r,sprite.color.g,sprite.color.b, newAlpha);
            yield return null;
        }
    }
    private IEnumerator FadeRoutine(Tilemap tilemap, float fadeTime, float startValue, float targetTransparency)
    {
        float elapsedTime = 0;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startValue, targetTransparency, elapsedTime / fadeTime);
            tilemap.color = new Color(tilemap.color.r, tilemap.color.g, tilemap.color.b, newAlpha);
            yield return null;
        }
    }


}
