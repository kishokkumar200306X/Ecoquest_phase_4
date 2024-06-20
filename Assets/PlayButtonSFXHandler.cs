using UnityEngine;
using UnityEngine.UI;

public class PlayButtonSFXHandler : MonoBehaviour
{

    public Button playButton; // Reference to the button

    // This method will be called when the button is clicked

    private void Start()
    {
        if (playButton != null)
        {
            // Add a listener to the button's onClick event
            playButton.onClick.AddListener(OnPlayButtonClicked);
        }
    }

    public void OnPlayButtonClicked()
    {
        AudioManager.instance.PlaySFX("ButtonClick");
        //Debug.Log("Play button clicked");

        AudioManager.instance.ToggleMusic();

    }
}
