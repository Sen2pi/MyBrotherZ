using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpSpawner : MonoBehaviour
{
    
    [SerializeField] private GameObject zCoin, healthGlobe, staminaGlobe, bullet, stone , wood, gas, zBlood, Hblood;
    public int randomNum = 0;


    public void DropItem(int index)
    {
        if (index == 1)
        {
            int randomAmountOfZCoin = Random.Range(1, 3);
            for (int i = 0; i < randomAmountOfZCoin; i++)
            {
                Instantiate(zCoin, transform.position, Quaternion.identity);
            }
        }

        if (index == 2)
        {
            int randomAmountOfBullets = Random.Range(1, 10);
            for (int i = 0; i < randomAmountOfBullets; i++)
            {
                Instantiate(bullet, transform.position, Quaternion.identity);
            }
        }

        if (index == 3)
        {
            Instantiate(healthGlobe, transform.position, Quaternion.identity);
        }

        if (index == 4)
        {
            Instantiate(staminaGlobe, transform.position, Quaternion.identity);
        }
        if (index == 5)
        {
            int randomAmountOfStone = Random.Range(1, 3);
            for (int i = 0; i < randomAmountOfStone; i++)
            {
                Instantiate(stone, transform.position, Quaternion.identity);
            }
        }
        if (index == 6)
        {
            int randomAmountOfwood = Random.Range(1, 3);
            for (int i = 0; i < randomAmountOfwood; i++)
            {
                Instantiate(wood, transform.position, Quaternion.identity);
            }
        }
        if (index == 7)
        {
            int randomAmountOfGas = Random.Range(1, 10);
            for (int i = 0; i < randomAmountOfGas; i++)
            {
                Instantiate(gas, transform.position, Quaternion.identity);
            }
        }
        if (index == 8)
        {
            Instantiate(zBlood, transform.position, Quaternion.identity);
        }
        if (index == 9)
        {
            Instantiate(Hblood, transform.position, Quaternion.identity);
        }
    }

    public void DropRandomItems()
    {
        if (randomNum != 0)
        {
            DropItem(randomNum);
        }
        else
        {
            int randomNum = Random.Range(1, 10);


            if (randomNum == 1)
            {
            
                int randomAmountOfZCoin = Random.Range(1, 3);
                for (int i = 0; i < randomAmountOfZCoin; i++)
                {
                    Instantiate(zCoin, transform.position, Quaternion.identity);
                }
            }

            if (randomNum == 2)
            {
            
                int randomAmountOfBullets = Random.Range(1, 10);
                for (int i = 0; i < randomAmountOfBullets; i++)
                {
                    Instantiate(bullet, transform.position, Quaternion.identity);
                }
            }

            if (randomNum == 3)
            {
            
                Instantiate(healthGlobe, transform.position, Quaternion.identity);
            }

            if (randomNum == 4)
            {
           
                Instantiate(staminaGlobe, transform.position, Quaternion.identity);
            }
        }
    }


   
}