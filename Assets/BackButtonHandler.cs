using UnityEngine;
using UnityEngine.UI;

public class BackButtonHandler : MonoBehaviour
{
    public Button backButton; // Reference to the button

    // This method will be called when the button is clicked

    void Start()
    {
        if (backButton != null)
        {
            // Add a listener to the button's onClick event
            backButton.onClick.AddListener(OnBackButtonClicked);
        }
    }

    public void OnBackButtonClicked()
    {
        AudioManager.instance.PlaySFX("ButtonClick");
       // Debug.Log("Back button clicked");
    }
}
