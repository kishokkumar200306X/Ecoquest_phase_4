using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class pauseMenu : MonoBehaviour
{
    [SerializeField] GameObject PauseMenu;
    // Start is called before the first frame update

    public void PauseButton()
    {
        PauseMenu.SetActive(true);
        Time.timeScale = 0;
    }
    public void MainMenu()
    {
        //  PauseMenu.SetActive(false);
        // UI_Main.instance._elements.SetActive(false);

        //Destroy all the game objects in scene id 3
        //  Destroy(GameObject.Find("Player"));
        //  Destroy(GameObject.Find("Main Camera"));
          Destroy(GameObject.Find("Canvas"));

        SceneManager.LoadScene(2);

        Time.timeScale = 1;

    }

    // Update is called once per frame
    public void Resume()
    {
        PauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void Options()
    {
        //Time.timeScale = 1;
    }

    public void QuitGame()
    {
        Time.timeScale = 1;
        Debug.Log("Quit");
        Application.Quit();
    }
}
