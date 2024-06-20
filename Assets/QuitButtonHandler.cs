using UnityEngine;
using UnityEngine.UI;

public class QuitButtonHandler : MonoBehaviour
{
public Button quitButton; // Reference to the button

    // This method will be called when the button is clicked

    private void Start()
    {
        if (quitButton != null)
        {
            // Add a listener to the button's onClick event
            quitButton.onClick.AddListener(OnQuitButtonClicked);
        }
    }

    public void OnQuitButtonClicked()
    {
        AudioManager.instance.PlaySFX("ButtonClick");
        //Debug.Log("Quit button clicked");
        
    }

}
