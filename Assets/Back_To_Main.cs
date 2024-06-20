using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Back_To_Main : MonoBehaviour
{
    public static Back_To_Main instance;

   

    public void Back_To_MainScene(int sceneId)
    {
        StartCoroutine(LoadScene(sceneId));
    }

    private IEnumerator LoadScene(int sceneId)
    {
        // Optionally, you can add some logic here before loading the scene
        // For example, displaying a loading screen or some animation

        // Wait for the end of the frame to ensure any previous operations are completed
        yield return new WaitForEndOfFrame();

        // Load the scene
        SceneManager.LoadScene(sceneId);

        // Optionally, you can add some logic here after the scene is loaded
    }
}

