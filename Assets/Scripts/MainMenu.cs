using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button _howToPlayButton = null;
    [SerializeField] private GameObject HowToPlayObject = null;
    [SerializeField] private GameObject MainMenuObject = null;
    [SerializeField] private GameObject OptionsObject = null;
    [SerializeField] private GameObject MainMenuText = null;
    [SerializeField] private GameObject QuitConfirm = null;

    [SerializeField] private Button _playButton = null;
    [SerializeField] private Button _closeButton = null;
    [SerializeField] private Button _optionsButton = null;
    [SerializeField] private Button _quitButton = null;

    [SerializeField] private TextMeshProUGUI _userName = null;


    private void Awake()
    {
        _userName.text = PlayerPrefs.GetString("Username");
        HowToPlayObject.SetActive(false);
        OptionsObject.SetActive(false);
        QuitConfirm.SetActive(false);
        MainMenuText.SetActive(true);

        _playButton.onClick.AddListener(PlayGame);
        _howToPlayButton.onClick.AddListener(HowToPlay);
        _optionsButton.onClick.AddListener(GameOptions);
        _closeButton.onClick.AddListener(CloseHowtoPlay);
        _quitButton.onClick.AddListener(QuitGame);
    }

    
    // Start is called before the first frame update
    //public AudioSource clickAudio;
    private void PlayGame()
    {
        AudioManager.instance.PlaySFX("ButtonClick");

        //loadScene.instance.LoadScene(3);
        //clickAudio.Play();
        // SceneManager.LoadScene(3);
    }

    private void GameOptions()
    {
        AudioManager.instance.PlaySFX("ButtonClick");

        MainMenuText.SetActive(false);
        MainMenuObject.SetActive(false);
        OptionsObject.SetActive(true);
        
    }

    public void QuitGame()
    {
        AudioManager.instance.PlaySFX("ButtonClick");

        MainMenuObject.SetActive(false);
        QuitConfirm.SetActive(true);

        // clickAudio.Play();
        Debug.Log("Quit");
        Application.Quit();
    }

    private void HowToPlay()
    {
        AudioManager.instance.PlaySFX("ButtonClick");

        MainMenuObject.SetActive(false);
        HowToPlayObject.SetActive(true);
    }

    private void CloseHowtoPlay()
    {
        AudioManager.instance.PlaySFX("ButtonClick");

        HowToPlayObject.SetActive(false);
        MainMenuObject.SetActive(true);
        
    }
}
