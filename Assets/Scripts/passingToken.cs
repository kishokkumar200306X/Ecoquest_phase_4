using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class passingToken : MonoBehaviour
{
private string receivedToken;
    void Start()
    {
      receivedToken = PlayerPrefs.GetString("Token", "");

        // For checking purposes only
        if (!string.IsNullOrEmpty(receivedToken))
        {
            // Token is available, perform actions accordingly
            Debug.Log("Token is available in the new scene: " + receivedToken);
        }
        else
        {
            // Token is not available, handle accordingly (e.g., show an error message)
            Debug.LogError("Token is not available in the new scene.");
        }
    }
        
    

    // Update is called once per frame
    void Update()
    {
        
    }
}

