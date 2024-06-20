//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//using UnityEngine.UI;
//using UnityEngine.SceneManagement;
//using UnityEngine.Networking;
//using UnityEngine.Playables;
//using Unity.VisualScripting.Antlr3.Runtime;
//using UnityEngine.UIElements;
//using UnityEditor.Playables;
//using static UnityEditor.Experimental.GraphView.Port;
using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Playables;
using System.Linq;

public class UI_Main : MonoBehaviour
{
    // Need to change this according to the currency types
    [SerializeField] public GameObject _elements = null;
    [SerializeField] public TextMeshProUGUI _storageCoins = null;
    [SerializeField] public TextMeshProUGUI _wattCoin = null;
    [SerializeField] public TextMeshProUGUI _carbonEmission = null;
    [SerializeField] public TextMeshProUGUI _population = null;

    [SerializeField] public Slider _carbonEmissionSlider = null;

    [SerializeField] private UnityEngine.UI.Button _shopButton = null;

    [SerializeField] public buildGrid _grid = null;
    //[SerializeField] public building[] _buildingPrefabs = null;
    [SerializeField] private List<BuildingEntry> buildingEntries = new List<BuildingEntry>();
    private Dictionary<string, building> _buildingPrefabs = new Dictionary<string, building>();

    public UI_Bar barBuild = null;

    [Header("Buttons")]
    public Transform buttonParent = null;
    public UI_Button buttonCollectCoin = null;
    private static UI_Main _instance = null; public static UI_Main instance { get { return _instance; } }

    private bool _active = true; public bool isActive { get { return _active; } }

    //bool hasPark = gameData.energySources.Any(source => source.buildingName == "park");

    private void Awake()
    {
        //Do not destroy the UI_Main object when loading a new scene
     //   DontDestroyOnLoad(this.gameObject);

        _instance = this;
        _elements.SetActive(true);

        Debug.Log("UI_Main Awake"+_elements.ToString());

        AudioManager.instance.PlaySFX("GameSceneOpen");

        // added
        foreach (BuildingEntry entry in buildingEntries)
        {
            if (!_buildingPrefabs.ContainsKey(entry.id))
            {
                _buildingPrefabs.Add(entry.id, entry.prefab);
                Debug.Log(entry.id);
            }
            else
            {
                Debug.LogWarning("Duplicate building ID found: " + entry.id);
            }
        }


    }

    private void Start()
    {
        _shopButton.onClick.AddListener(ShopButtonClicked);
        AccessGameData();

        //_carbonEmission.text = "85";

        // Initialize the slider value based on the initial text value
        int initialValue = int.Parse(_carbonEmission.text);
        _carbonEmissionSlider.value = Mathf.Clamp(initialValue, 0, 100);

        // Set up the listener for slider value changes
        _carbonEmissionSlider.onValueChanged.AddListener(OnSliderValueChanged);

        // Update the initial display and color
        UpdateCarbonEmissionDisplay();
        UpdateSliderFillColor();

        StartCoroutine(IncreaseStorageCoins());

       // AudioManager.instance.PlayMusic("CityEnvironment");
      // AudioManager.instance.PlaySFX("CityAmbience");
    }

    private void Update()
    {
        //UpdateCarbonEmissionDisplay();
        //UpdateSliderFillColor();
    }

    public building GetBuildingPrefabById(string id)
    {
        if (_buildingPrefabs.ContainsKey(id))
        {
            return _buildingPrefabs[id];
        }
        else
        {
            Debug.LogError("No building found with ID: " + id);
            return null;
        }
    }

    private void ShopButtonClicked()
    {
        // cancel a build if not wanted
        if (!UI_Build.instance._buildingPlaced)
        {
            UI_Build.instance.Cancel();
        }
    
        UI_Shop.instance.SetStatus(true);
        SetStatus(false);
        
    }

    public void SetStatus(bool status)
    {
        _active = status;
        _elements.SetActive(status);
    }
 

    private void AccessGameData()
    {
        
        if (GameDataManager.instance != null)
        {
            GameData gameData = GameDataManager.instance.gameData;
            if (gameData != null)
            {
                Debug.Log("Game data accessed successfully!");

                _wattCoin.text = gameData.totalCoins.ToString();
                _storageCoins.text = gameData.storageCoins.ToString();

                if ((gameData.carbonPercentage == 0 )&& !(gameData.energySources.Any(source => source.buildingName == "park") ))
                {
                    _carbonEmission.text = "5";

                    _carbonEmissionSlider.value = int.Parse(UI_Main.instance._carbonEmission.text);
                }
                else
                {

                    _carbonEmission.text = gameData.carbonPercentage.ToString();
                    //  _carbonEmission.text = $"{gameData.carbonPercentage}%";

                    _carbonEmissionSlider.value = int.Parse(UI_Main.instance._carbonEmission.text);
                   // _carbonEmissionSlider.value = int.Parse(UI_Main.instance._carbonEmission.text.Replace("%", ""));

                }
                

                if (gameData.noOfPeople == 0)
                {
                    _population.text = "20";
                }
                else
                {
                    _population.text = gameData.noOfPeople.ToString();
                }
                

                int x; 
                int y;
                string name;
                int level;

                Vector3 position;
                building currentBuilding;

                for (int i =0; i < gameData.investmentBuildings.Count; i++)
                {
                    x = gameData.investmentBuildings[i].plotNumber[0];
                    y = gameData.investmentBuildings[i].plotNumber[1];

                    name = gameData.investmentBuildings[i].buildingName;
                    level = gameData.investmentBuildings[i].level;

                    position = new Vector3(x, 0, y);

                    // currentBuilding = Instantiate(_buildingPrefabs[id], position, Quaternion.identity);
                    currentBuilding = Instantiate(GetBuildingPrefabById(name), position, Quaternion.identity);

                    currentBuilding.currentX = x;
                    currentBuilding.currentY = y;

                    currentBuilding.uniqueId = gameData.investmentBuildings[i].uniqueId;
                    Debug.Log("Datetime in the database " + gameData.investmentBuildings[i].storageCoinCollectedTimeString);

                    currentBuilding._lastCollectedTime = DateTime.Parse(gameData.investmentBuildings[i].storageCoinCollectedTimeString);

                    currentBuilding.storage = GetAccumulatedCoins(currentBuilding._levels[level - 1].speed, currentBuilding._lastCollectedTime, currentBuilding._levels[level - 1].capacity);

                    Debug.Log(currentBuilding.storage);

                    building.buildInstance = currentBuilding;
                    UI_Build.instance.LoadingBuildings();
                    building.buildInstance.SetCurrentLevel(level, 0, 0);
                }

                for (int i = 0; i < gameData.energySources.Count; i++)
                {
                    x = gameData.energySources[i].plotNumber[0];
                    y = gameData.energySources[i].plotNumber[1];

                    name = gameData.energySources[i].buildingName;
                    level = gameData.energySources[i].level;

                    position = new Vector3(x, 0, y);

                    // currentBuilding = Instantiate(_buildingPrefabs[i+4], position, Quaternion.identity);
                    currentBuilding = Instantiate(GetBuildingPrefabById(name), position, Quaternion.identity);

                    currentBuilding.currentX = x;
                    currentBuilding.currentY = y;

                    building.buildInstance = currentBuilding;
                    UI_Build.instance.LoadingBuildings();
                    building.buildInstance.SetCurrentLevel(level, 0, 0);
                }

                //UI_Build.instance._buildingPlaced = false;
            }
            else
            {
                Debug.LogError("Game data is not yet loaded.");
            }
        }
        else
        {
            Debug.LogError("GameDataManager instance is not set.");
        }
        
    }

    public int GetAccumulatedCoins(float speed, DateTime? lastCollectedTime, float capacity)
    {
        if (!lastCollectedTime.HasValue)
        {
            return 0; // If lastCollectedTime is null, return 0
        }

        DateTime currentTime = DateTime.Now;

        // Calculate the elapsed time in seconds
        double elapsedTimeInSeconds = (currentTime - lastCollectedTime.Value).TotalSeconds;

        // Calculate the accumulated coins based on the elapsed time and accumulation speed
        double accumulatedCoins = (elapsedTimeInSeconds / 10) * speed;

        // Limit the accumulated coins to the building's capacity
        int coinsToCollect = (int)Math.Min(accumulatedCoins, capacity);

        return coinsToCollect;
    }

    private IEnumerator IncreaseStorageCoins()
    {
        while (true)
        {
            yield return new WaitForSeconds(30);
            AddStorageCoinsBasedOnPopulation();
        }
    }

    private void AddStorageCoinsBasedOnPopulation()
    {
        int population = int.Parse(_population.text);
        int increaseAmount = population / 50;
        _storageCoins.text = (int.Parse(_storageCoins.text) + increaseAmount).ToString();

        Debug.Log("Coins increased with population");
        GameData gameData = GameDataManager.instance.gameData;

        gameData.storageCoins = int.Parse(_storageCoins.text);

    }

    /// <summary>
    /// //////////// carbon emision
    /// </summary>
    /// <param name="value"></param>
    private void OnSliderValueChanged(float value)
    {
        value = Mathf.Clamp(value, 0, 100);
        _carbonEmissionSlider.value = value;

        // Update the carbon emission text
        UpdateCarbonEmissionDisplay();

        // Update the fill color based on the value
        UpdateSliderFillColor();
    }

    public void UpdateCarbonEmissionText(string text)
    {
        // Parse the text to get the carbon emission value
        if (int.TryParse(text, out int value))
        {
            // Clamp the value between 0 and 100
            value = Mathf.Clamp(value, 0, 100);

            // Update the slider value
            _carbonEmissionSlider.value = value;

            // Update the text field
            _carbonEmission.text = value.ToString();

            // Update the fill color based on the value
            UpdateSliderFillColor();
        }
    }

    private void UpdateCarbonEmissionDisplay()
    {
        // Update the text to match the slider value
        _carbonEmission.text = _carbonEmissionSlider.value.ToString("0");
    }

    private void UpdateSliderFillColor()
    {
        // Access the fill area of the slider
        Image fillImage = _carbonEmissionSlider.fillRect.GetComponent<Image>();

        // Change the color based on the slider value
        if (_carbonEmissionSlider.value > 80)
        {
            fillImage.color = Color.red;
        }
        else
        {
            fillImage.color = Color.green;
        }
    }


    [System.Serializable]
    private class PowerConsumptionResponse
    {
        public float currentConsumption;
    }

}

[System.Serializable]
public class BuildingEntry
{
    public string id;
    public building prefab;
}