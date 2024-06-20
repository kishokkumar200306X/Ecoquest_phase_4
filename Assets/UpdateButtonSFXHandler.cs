using UnityEngine;
using UnityEngine.UI;

public class UpdateButtonSFXHandler : MonoBehaviour
{
    public Button button; // Reference to the button

    // This method will be called when the button is clicked

    private void Start()
    {
        if (button != null)
        {
            // Add a listener to the button's onClick event
            button.onClick.AddListener(OnButtonClicked);
        }
    }

    public void OnButtonClicked()
    {
        AudioManager.instance.PlaySFX("Update");
        // Debug.Log("Button clicked");

    }
}
