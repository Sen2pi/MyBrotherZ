using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CursorManager : MonoBehaviour
{
    public Image cursorImage;
    public Sprite defaultCursorSprite;
    public Color highlightColor = Color.yellow;

    private Button highlightedButton; // Store the highlighted button
    private Color normalButtonColor; // Store the normal color of the button

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
        cursorImage.sprite = defaultCursorSprite;
    }

    private void Start()
    {
        normalButtonColor = Color.white; // Assuming the normal color is white; you can adjust this as needed
    }

    private void Update()
    {
        Vector2 cursorPos = Input.mousePosition;
        cursorImage.rectTransform.position = cursorPos;

        HandleUIInteractions();
    }

    private void HandleUIInteractions()
    {
        if (EventSystem.current == null)
            return;

        // Check if the cursor is over any UI element
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        // Find the first button in the raycast results
        Button newHighlightedButton = null;
        foreach (var result in results)
        {
            newHighlightedButton = result.gameObject.GetComponent<Button>();
            if (newHighlightedButton != null)
                break;
        }

        // Update the highlighted button
        if (newHighlightedButton != highlightedButton)
        {
            if (highlightedButton != null)
                highlightedButton.image.color = normalButtonColor;

            highlightedButton = newHighlightedButton;

            if (highlightedButton != null)
                highlightedButton.image.color = highlightColor;
        }

        // Handle button click
        if (Input.GetMouseButtonDown(0) && highlightedButton != null)
        {
            highlightedButton.onClick.Invoke();
        }
    }
}
