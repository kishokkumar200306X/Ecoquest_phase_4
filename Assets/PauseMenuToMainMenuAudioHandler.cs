using UnityEngine;
using UnityEngine.UI;
public class PauseMenuToMainMenuAudioHandler : MonoBehaviour
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
        AudioManager.instance.PlaySFX("ButtonClick");
        // Debug.Log("Button clicked");

        AudioManager.instance.ToggleMusic();

        AudioManager.instance.MusicVolume(AudioManager.instance.CurrentMusicVolume);
    }
}
