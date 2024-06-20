using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DynamicPeopleManager : MonoBehaviour
{
    public string powerConsumptionApiUrl = "http://20.15.114.131:8080/api/power-consumption/current/view";
    public float updateInterval = 10f; // Time interval to fetch data from API
    public GameObject[] personPrefabs; // Array to hold the different person prefabs
    public Transform[] pathContainers; // Array to hold the paths for each prefab

    private List<GameObject> peopleList = new List<GameObject>();
    private float initialPowerConsumption;
    private bool initialFetchDone = false;

    void Start()
    {
        // Start fetching data from the API
        StartCoroutine(FetchPowerConsumption());
        InvokeRepeating("FetchPowerConsumptionPeriodically", updateInterval, updateInterval);
    }

    IEnumerator FetchPowerConsumption()
    {
        UnityWebRequest www = UnityWebRequest.Get(powerConsumptionApiUrl);
        string token = PlayerPrefs.GetString("Token", "");
        www.SetRequestHeader("Authorization", "Bearer " + token);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string jsonResponse = www.downloadHandler.text;
            PowerConsumptionResponse response = JsonUtility.FromJson<PowerConsumptionResponse>(jsonResponse);

            if (response != null)
            {
                float powerConsumption = response.currentConsumption;

                if (!initialFetchDone)
                {
                    initialPowerConsumption = powerConsumption;
                    initialFetchDone = true;
                    Debug.Log("Initial power consumption: " + initialPowerConsumption);
                }
                else
                {
                    float increase = powerConsumption - initialPowerConsumption;
                    int peopleToAdd = Mathf.FloorToInt(increase / 5); // Add a person for every 5 unit increase
                    UpdatePeopleCount(peopleToAdd);
                    Debug.Log("Current power consumption: " + powerConsumption + ", Increase: " + increase + ", People to add: " + peopleToAdd);
                }
            }
            else
            {
                Debug.LogError("Invalid power consumption response.");
            }
        }
        else
        {
            Debug.LogError(www.error);
        }
    }

    void FetchPowerConsumptionPeriodically()
    {
        StartCoroutine(FetchPowerConsumption());
    }

    public void UpdatePeopleCount(int newCount)
    {
        int currentCount = peopleList.Count;

        if (newCount > currentCount)
        {
            for (int i = currentCount; i < newCount; i++)
            {
                InstantiatePerson();
            }
        }
        else if (newCount < currentCount)
        {
            for (int i = currentCount - 1; i >= newCount; i--)
            {
                Destroy(peopleList[i]);
                peopleList.RemoveAt(i);
            }
        }
    }

    void InstantiatePerson()
    {
        if (personPrefabs.Length == 0 || pathContainers.Length != personPrefabs.Length)
        {
            Debug.LogError("Person prefabs or path containers are not properly assigned.");
            return;
        }

        int randomIndex = Random.Range(0, personPrefabs.Length);
        GameObject selectedPrefab = personPrefabs[randomIndex];
        Transform selectedPath = pathContainers[randomIndex];

        GameObject newPerson = Instantiate(selectedPrefab);
        newPerson.GetComponent<peopleController>().PATH = selectedPath.gameObject;
        peopleList.Add(newPerson);
    }
}

[System.Serializable]
public class PowerConsumptionResponse
{
    public float currentConsumption;
}
