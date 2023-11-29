using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarBehavior : MonoBehaviour
{
    [SerializeField] private Slider mySlider;
    [SerializeField] private Color low;
    [SerializeField] private Color high;
    [SerializeField] private Vector3 offset;
    
    private void Update()
    {
        mySlider.transform.position = Camera.main.WorldToScreenPoint(transform.parent.position + offset);
    }
    public void SetHealth(int health, int maxHealth)
    {
        mySlider.gameObject.SetActive(health<maxHealth);
        mySlider.value = health;
        mySlider.maxValue = maxHealth;
        mySlider.fillRect.GetComponentInChildren<Image>().color = Color.Lerp(low, high, mySlider.normalizedValue);
    }

}
