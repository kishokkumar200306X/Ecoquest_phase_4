using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Networking;

public class loadScene : MonoBehaviour
{
    public GameObject loadingScreen;
    public Slider loadingBar;

    public static loadScene instance;
    public void LoadScene(int sceneId)
    {
        //StartCoroutine(GameDataManager.instance.ReceiveDataCoroutine());
        StartCoroutine(LoadSceneAsync(sceneId));

       // AudioManager.instance.PlaySFX("GameSceneOpen");
    }

    IEnumerator LoadSceneAsync(int sceneId)
    {
        loadingScreen.SetActive(true);

        yield return StartCoroutine(GameDataManager.instance.ReceiveDataCoroutine());


        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);

        

        // loadingBar.value = 0.25f;

        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress / 0.9f);
            // loadingBar.value = 0.25f + progressValue * 0.75f;
            
            Debug.Log($"Loading progress: {loadingBar.value * 100}%");

            loadingBar.value = progressValue;

            yield return null;
        }

        
    }

   
}
