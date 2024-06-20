using UnityEngine;
using UnityEngine.UI;

public class LeaderboardButtonHandler : MonoBehaviour
{
    public Button leaderboardButton; // Reference to the button

    private void Start()
    {
        if (leaderboardButton != null)
        {
            // Add a listener to the button's onClick event
            leaderboardButton.onClick.AddListener(OnLeaderboardButtonClicked);
        }
        else
        {
            Debug.LogError("LeaderboardButton reference is not assigned.");
        }
    }

    public void OnLeaderboardButtonClicked()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX("ButtonClick");
        }
        else
        {
            Debug.LogError("AudioManager instance is not initialized.");
        }

        if (Lea_Brd.instance != null)
        {
            Lea_Brd.instance.Lea_BrdScene(1); // Replace 1 with the actual scene ID for the leaderboard
        }
        else
        {
            Debug.LogError("Lea_Brd instance is not initialized.");
        }
    }
}
