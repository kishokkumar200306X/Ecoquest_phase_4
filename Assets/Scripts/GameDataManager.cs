using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
//using Unity.VisualScripting.Antlr3.Runtime;
//using UnityEditor.PackageManager.Requests;

// Define data structures
[Serializable]
public class EnergySource
{
    public int uniqueId;
    public string buildingName;
    public int expectedEarnings;
    public int level;
    public List<int> plotNumber;
    public DateTime storageCoinCollectedTime;

    public override string ToString()
    {
        return $"EnergySource: {{ uniqueId: {uniqueId}, buildingName: {buildingName}, expectedEarnings: {expectedEarnings}, level: {level}, plotNumber: [{string.Join(", ", plotNumber)}], storageCoinCollectedTime: {storageCoinCollectedTime} }}";
    }
}

[Serializable]
public class InvestmentBuilding
{
    public int uniqueId;
    public string buildingName;
    public int expectedEarnings;
    public int level;
    public List<int> plotNumber;
    public string storageCoinCollectedTimeString;

    // Non-serialized DateTime field
    [NonSerialized]
    public DateTime storageCoinCollectedTime;

    public override string ToString()
    {
        return $"InvestmentBuilding: {{ uniqueId: {uniqueId}, buildingName: {buildingName}, expectedEarnings: {expectedEarnings}, level: {level}, plotNumber: [{string.Join(", ", plotNumber)}], storageCoinCollectedTime: {storageCoinCollectedTime} }}";
    }
}

[Serializable]
public class GameData
{
    public string _id;
    public List<int> coinsPerDay;
    public List<EnergySource> energySources;
    public List<InvestmentBuilding> investmentBuildings;
    public DateTime lastUpdated;
    public int storageCoins;
    public int totalCoins;
    public string userName;
    public int noOfPeople;
    public int carbonPercentage;

    public override string ToString()
    {
        var energySourcesStr = string.Join(", ", energySources);
        var investmentBuildingsStr = string.Join(", ", investmentBuildings);

        return $"GameData: {{ _id: {_id}, coinsPerDay: [{string.Join(", ", coinsPerDay)}], energySources: [{energySourcesStr}], investmentBuildings: [{investmentBuildingsStr}], lastUpdated: {lastUpdated}, storageCoins: {storageCoins}, totalCoins: {totalCoins}, userName: {userName} }}";
    }
}
public class GameDataManager : MonoBehaviour
{
    private string backendUrl;
    public GameData gameData;

    public static GameDataManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            Debug.Log("GameDataManager instance set in Awake.");
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            Debug.Log("Duplicate GameDataManager destroyed.");
        }

        // Retrieve the PlayerPrefs value in Awake
        string username = PlayerPrefs.GetString("Username");
        // string username = "oversight_g17";  // Should be deleted
        backendUrl = "https://ecoquest-420605.el.r.appspot.com/user/getuser/{userName}?userName=" + username;
        Debug.Log(username);
    }
    
    private void Start()
    {
        //GetGameData();
    }

    // Serialize game data to JSON
    private string SerializeGameData()
    {
        return JsonUtility.ToJson(gameData);
    }

    // Save game data and send to backend server
    public void GetGameData()
    {
        //StartCoroutine(ReceiveDataCoroutine());
    }

    // Coroutine to send data to the backend server
    public IEnumerator ReceiveDataCoroutine()
    {
        string jsonData = SerializeGameData();

        UnityWebRequest request = UnityWebRequest.Get(backendUrl);

        yield return request.SendWebRequest();
        Debug.Log(request.result);

        if (request.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = request.downloadHandler.text;
            Debug.Log(jsonResponse);
            gameData = JsonUtility.FromJson<GameData>(jsonResponse);
            Debug.Log("Data successfully received from the server");
        }
        else
        {
            Debug.LogError("Error receiving data: " + request.error);
        }
    }

  

}
