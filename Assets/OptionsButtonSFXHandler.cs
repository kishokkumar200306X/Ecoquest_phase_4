using UnityEngine;
using UnityEngine.UI;

public class OptionsButtonSFXHandler : MonoBehaviour
{
    public Button optionsButton; // Reference to the button

    // This method will be called when the button is clicked

    private void Start()
    {
        if (optionsButton != null)
        {
            // Add a listener to the button's onClick event
            optionsButton.onClick.AddListener(OnOptionsButtonClicked);
        }
    }

    public void OnOptionsButtonClicked()
    {
        AudioManager.instance.PlaySFX("ButtonClick");
        Debug.Log("Options button clicked");

    }
}
