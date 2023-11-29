using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIButtonInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Button button;
    private Color normalColor;
    public Color highlightColor = Color.yellow;

    private void Start()
    {
        button = GetComponent<Button>();
        normalColor = button.colors.normalColor;

        // Add listeners for button interactions
        button.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        // Remove listeners to prevent memory leaks
        button.onClick.RemoveListener(OnClick);
    }

    private void OnClick()
    {
        // Replace this with your desired onClick functionality
        Debug.Log("Button clicked: " + button.name);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Highlight();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        button.image.color = normalColor;
    }

    private void Highlight()
    {
        ColorBlock colors = button.colors;
        colors.normalColor = highlightColor;
        button.colors = colors;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }
}
