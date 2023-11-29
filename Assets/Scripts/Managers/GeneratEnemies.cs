using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratEnemies : MonoBehaviour
{
    public GameObject enemy;
    public int xPos;
    public int yPos;
    public int enemyCount;
    public int enemyQuantity;
  
    private void Start()
    {
        try { 
        StartCoroutine(EnemyDrop());
        }catch(MissingReferenceException) { }
    }

    IEnumerator EnemyDrop()
    {
        while (enemyCount < enemyQuantity) { 
            xPos = Random.Range(-9, 9);
            yPos = Random.Range(-5 , 15);
            Instantiate(enemy, new Vector3(xPos, yPos, 0), Quaternion.identity);
            yield return new WaitForSeconds(0.1f);
            enemyCount++;
        }
    }

}
