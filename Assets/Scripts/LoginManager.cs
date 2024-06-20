using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;



public class NewBehaviourScript : MonoBehaviour

{
    private float _StartMusicVolume = 0.5f;

    void Start()
    {
        // changeData();


        // string apiKey = "NjVkNDIyMjNmMjc3NmU3OTI5MWJmZGI2OjY1ZDQyMjIzZjI3NzZlNzkyOTFiZmRhYw";
        // Start the authentication process
        // StartCoroutine(SendApi2(apiKey));

        // Check if the instance exists before accessing it
        if (AudioManager.instance != null)
        {
            AudioManager.instance.StartMusicVolume = _StartMusicVolume;
            //
            // You can now use the musicVolume value as needed
        }
        else
        {
            Debug.LogError("AudioManager not found in the scene.");
        }

    }

    //[SerializeField] private TMP_InputField apiKeyInputField;
    public void OnSubmitLogin()
    {
        //Play SFX
        AudioManager.instance.PlaySFX("Success");

        //Play Music
        AudioManager.instance.PlayMusic("Theme");

        //Control the volume of the music
        AudioManager.instance.MusicVolume(_StartMusicVolume);


        string apiKey = "NjVkNDIyMjNmMjc3NmU3OTI5MWJmZGI2OjY1ZDQyMjIzZjI3NzZlNzkyOTFiZmRhYw";// NjVkNDIyMjNmMjc3NmU3OTI5MWJmZGI2OjY1ZDQyMjIzZjI3NzZlNzkyOTFiZmRhYw    apiKeyInputField.text
        StartCoroutine(AuthenticatePlayer(apiKey));
        Debug.Log("ApiKey: " + apiKey);
    }


    private IEnumerator AuthenticatePlayer(string apiKey)
    {
        string jsonBody = "{\"apiKey\": \"" + apiKey + "\"}";

        UnityWebRequest request = UnityWebRequest.Post("http://20.15.114.131:8080/api/login", jsonBody, "application/json");
        
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // Parse the response to get the token
            TokenResponse tokenResponse = JsonUtility.FromJson<TokenResponse>(request.downloadHandler.text);
            string token = tokenResponse.token;
            PlayerPrefs.SetString("Token", token);
            

            PlayerPrefs.Save();
            SceneManager.LoadSceneAsync("playerProfile");

        }
        else
        {
            Debug.Log(request.error);
        }

        

        /*
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            Debug.Log("Response: " + request.downloadHandler.text);
            // Parse the response to get the token
            TokenResponse tokenResponse = JsonUtility.FromJson<TokenResponse>(request.downloadHandler.text);
            string token = tokenResponse.token;
            Debug.Log("JWT Token: " + token);
            // You can now use the token for further HTTP requests
        }
        */


    }

    private void changeData() 
    {
        string jsonData = "{\"_id\":\"oversight_g17\",\"userName\":\"oversight_g17\",\"lastUpdated\":null,\"totalCoins\":18675,\"storageCoins\":18843,\"noOfPeople\":20,\"carbonPercentage\":70,\"coinsPerDay\":[],\"investmentBuildings\":[{\"plotNumber\":[-4,3],\"buildingName\":\"apartment\",\"level\":1,\"uniqueId\":1,\"expectedEarnings\":0,\"storageCoinCollectedTimeString\":\"2024-06-01T23:25:39.8849578+05:30\"}],\"energySources\":[{\"plotNumber\":[-3,-5],\"buildingName\":\"windmill\",\"level\":1,\"uniqueId\":2,\"expectedEarnings\":40,\"storageCoinCollectedTimeString\":null}]}";
        StartCoroutine(UpdatingBuildingDetails(jsonData));

    }

    IEnumerator UpdatingBuildingDetails(string jsonData)
    {
        UnityWebRequest www = UnityWebRequest.Put("https://ecoquest-420605.el.r.appspot.com/user/updateUserProfile", jsonData);
        www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
        www.uploadHandler.contentType = "application/json";

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Building data updated successfully.");
        }
        else
        {
            // Improved error handling
            string responseBody = www.downloadHandler?.text;
            Debug.LogError($"Failed to update building data. Server responded with status code: {www.responseCode} and message: {responseBody}");
        }
    }

[System.Serializable]
    private class TokenResponse
    {
        public string token;
    }
}