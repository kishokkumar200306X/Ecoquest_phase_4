using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UI_Tooltip : MonoBehaviour
{
    public TextMeshProUGUI headerField;
    public TextMeshProUGUI contentField;
    public LayoutElement layoutElement;

    public int characterWrapLimit;

    private static UI_Tooltip instance;

    private RectTransform tooltipRectTransform;

    private void Awake()
    {
        instance = this;
        tooltipRectTransform = GetComponent<RectTransform>();
    }

    public void SetText(string content, string header = "")
    {
        if (string.IsNullOrEmpty(header))
        {
            headerField.gameObject.SetActive(false);
        }
        else
        {
            headerField.gameObject.SetActive(true);
            headerField.text = header;
        }

        contentField.text = content;

        int headerLength = headerField.text.Length;
        int contentLength = contentField.text.Length;

        layoutElement.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit) ? true : false;
    }

    private void Update()
    {
        if (Application.isEditor)
        {
            int headerLength = headerField.text.Length;
            int contentLength = contentField.text.Length;

            layoutElement.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit) ? true : false;

            // Get the position of the UI element under the cursor
            Vector2 position;
            if (GetHoveredElementPosition(out position))
            {
                // Offset the tooltip position to be just above the hovered element
                position.y += tooltipRectTransform.rect.height * 4;
                transform.position = position;
            }
        }
    }

    private bool GetHoveredElementPosition(out Vector2 position)
    {
        position = Vector2.zero;
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Mouse.current.position.ReadValue()
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results)
        {
            if (result.gameObject.GetComponent<Button>() != null)
            {
                RectTransform buttonRectTransform = result.gameObject.GetComponent<RectTransform>();
                position = buttonRectTransform.position;
                return true;
            }
        }
        return false;
    }
}
