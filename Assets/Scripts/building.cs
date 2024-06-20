using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using System;
using System.Collections;
using UnityEngine.Networking;
using System.Linq;
using UnityEngine.Playables;
using UnityEngine.UI;
//using static UnityEditor.Experimental.GraphView.Port;

#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
#endif


public class building : MonoBehaviour
{
    // unique id to building
    public string id = "";

    public int uniqueId;

    private static building _buildInstance = null; public static building buildInstance { get { return _buildInstance; } set { _buildInstance = value; } }
    private static building _selectedInstance = null; public static building selectedInstance { get { return _selectedInstance; } set { _selectedInstance = value; } }
    [System.Serializable] public class Level
    {
        public int level = 1;
        public Sprite icon = null;
        public GameObject mesh = null;
        public int costStorageCoins;
        public int costWattCoins;
        public float speed;
        public float capacity;
        public int expectedEarning;
        public int population;
    }

    private buildGrid _grid = null;
    private float _storage = 0; public float storage { get { return _storage; } set { _storage = value; } }
    private bool isPlaced = false;
    private bool _maxReached = false;

    public DateTime _lastCollectedTime;

    private int _populationCapacity = 0; public int populationCapacity { get { return _populationCapacity; } set { _populationCapacity = value; } }
    private int _currentLevel = 1; public int currentLevel { get { return _currentLevel; } set { _currentLevel = value; } }

    [SerializeField] private int _rows = 1; public int rows { get { return _rows; } }
    [SerializeField] private int _columns = 1; public int columns { get { return _columns; } }

    [SerializeField] private MeshRenderer _baseArea = null;

    [SerializeField] public Level[] _levels = null;

    [SerializeField] public Sprite _buildingImage = null;

    [SerializeField] public string _buildingInfo;

    [HideInInspector] public UI_Button collectButton = null;
    [HideInInspector] public bool collecting = false;

    [HideInInspector] public UI_Bar buildBar = null;

    private int _currentX = 0; public int currentX { get { return _currentX; } set { _currentX = value; } }
    private int _currentY = 0; public int currentY { get { return _currentY; } set { _currentY = value; } }

    private int _X = 0;
    private int _Y = 0;
    private int _originalX = 0;
    private int _originalY = 0;


    private float elapsedTime = 0f; // Variable to keep track of time
    private const float updateInterval = 10f; // 10 seconds interval

    public RewardManager rewardManager;

    private void Awake()
    {
        if (UI_Build.instance._buildingPlaced)
        {
            if (UI_Main.instance == null)
            {
                Debug.LogError("UI_Main instance is not initialized.");
                return;
            }

            if (collectButton == null)
            {
                Debug.Log("Collect button is being instantiated.");
                collectButton = Instantiate(UI_Main.instance.buttonCollectCoin, UI_Main.instance.buttonParent);
                collectButton.gameObject.SetActive(false);
                collectButton.button.onClick.AddListener(Collect);
            }
        }
        
    }

    void Start()
    {
        // Ensure rewardManager is assigned
        if (rewardManager == null)
        {
            rewardManager = RewardManager.instance;
        }
    }


    private void Update()
    {
        // Increment the elapsed time by the time passed since the last frame
        elapsedTime += Time.deltaTime;

        // Check if 10 seconds have passed
        if (elapsedTime >= updateInterval)
        {
            // Reset the elapsed time
            elapsedTime = 0f;

            // Call the UpdateStorage function
            if (_storage < _levels[currentLevel-1].capacity)
            {
                UpdateStorage();
            }
            else
            {
                _storage = _levels[currentLevel-1].capacity;

            }
        }

        AdjustUI();

    }

    private void AdjustUI()
    {
        if (collectButton != null)
        {
            if (_storage >= 10)
            {
                // Debug.Log("Collect button appeared");
                if (!collectButton.gameObject.activeSelf)
                {
                    collectButton.gameObject.SetActive(true);
                }
                collectButton.button.onClick.RemoveAllListeners();
                collectButton.button.onClick.AddListener(Collect);

                Vector3 end = UI_Main.instance._grid.GetEndPosition(this);

                Vector3 planDownLeft = CameraController.instance.planDownLeft;
                Vector3 planTopRight = CameraController.instance.planTopRight;

                float w = planTopRight.x - planDownLeft.x;
                float h = planTopRight.z - planDownLeft.z;

                float endw = end.x - planDownLeft.x;
                float endh = end.z - planDownLeft.z;

                Vector2 screenPoint = new Vector2(endw / w * Screen.width, endh / h * Screen.height);

                collectButton.rect.anchoredPosition = screenPoint;
            }
            else
            {
                collectButton.gameObject.SetActive(false);
            }
        }
        else
        {
            collectButton = Instantiate(UI_Main.instance.buttonCollectCoin, UI_Main.instance.buttonParent);
            collectButton.gameObject.SetActive(_storage >= 10);
            collectButton.button.onClick.AddListener(Collect);
        }
    }

    public void Collect()
    {
        _lastCollectedTime = DateTime.Now;
        Debug.Log(_lastCollectedTime);

        RewardManager.instance.RewardPileOfCoin(6);

        int collectedAmount = (int)Math.Floor(_storage);

        int newCoins;
        int.TryParse(UI_Main.instance._storageCoins.text, out newCoins);

        Debug.Log("Coins collected");

        AudioManager.instance.PlaySFX("Collect");

        UI_Main.instance._storageCoins.text = (newCoins + collectedAmount).ToString();

        if (GameDataManager.instance != null)
        {
            GameData gameData = GameDataManager.instance.gameData;

            string jsonDataBefore = JsonUtility.ToJson(gameData);

            Debug.Log("JsonData before the update: " + jsonDataBefore);

            InvestmentBuilding building = gameData.investmentBuildings.Find(b => b.uniqueId == uniqueId);

            if (building != null)
            {
                gameData.investmentBuildings.Find(b => b.uniqueId == uniqueId).storageCoinCollectedTimeString = _lastCollectedTime.ToString("o");
                // building.storageCoinCollectedTimeString = building.storageCoinCollectedTime.ToString("o");

                Debug.Log("String time: " + gameData.investmentBuildings.Find(b => b.uniqueId == uniqueId).storageCoinCollectedTimeString);
            }
            else
            {
                Console.WriteLine($"Building with uniqueId {uniqueId} not found.");
            }

            Console.WriteLine(gameData);
            Debug.Log(gameData);

            InvestmentBuildingListWrapper wrapper = new InvestmentBuildingListWrapper(gameData.investmentBuildings);

            // Serialize only the updated investment buildings to JSON
            string jsonDataInvestment = JsonUtility.ToJson(wrapper);
            Debug.Log(jsonDataInvestment);
            // Send the updated investment buildings to the backend
            // StartCoroutine(UpdateInvestmentBuildings(jsonDataInvestment));


            if (gameData != null)
            {
                Debug.Log("Game data not null");

                string jsonData = "";

                gameData.storageCoins = (newCoins + collectedAmount);
                jsonData = JsonUtility.ToJson(gameData);

                Debug.Log(jsonData);

                StartCoroutine(UpdatingGameData(jsonData));
            }
            else
            {
                Debug.Log("Game data is null");
            }
            

        }
        else
        {
            Debug.Log("GameDataManager instance is null");
        }
        collectButton.gameObject.SetActive(false);
        collecting = true;
        _storage = 0;


    }
    IEnumerator UpdatingGameData(string jsonData)
    {
        UnityWebRequest www = UnityWebRequest.Put("https://ecoquest-420605.el.r.appspot.com/user/updateUserProfile", jsonData);
        www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
        www.uploadHandler.contentType = "application/json";

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Database updated successfully.");
        }
        else
        {
            // Improved error handling
            string responseBody = www.downloadHandler?.text;
            Debug.LogError($"Failed to update building data. Server responded with status code: {www.responseCode} and message: {responseBody}");
        }
    }

    IEnumerator UpdateInvestmentBuildings(string jsonData)
    {
        UnityWebRequest www = UnityWebRequest.Put("https://ecoquest-420605.el.r.appspot.com/user/updateLastTimeStorageCoin", jsonData);
        www.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
        www.uploadHandler.contentType = "application/json";

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Investement building collect time updated successfully.");
        }
        else
        {
            // Improved error handling
            string responseBody = www.downloadHandler?.text;
            Debug.LogError($"Failed to update building data. Server responded with status code: {www.responseCode} and message: {responseBody}");
        }
    }

    private void UpdateStorage()
    {
        // Check if capacity and speed are both non-zero
        if (isPlaced && _levels[currentLevel-1].capacity > 0 && _levels[currentLevel-1].speed > 0)
        {
            // Reduce speed by half
            float adjustedSpeed = _levels[currentLevel-1].speed;

            Debug.Log(_levels[currentLevel-1].speed);

            // Increment storage by adjusted speed
            // Debug.Log("Storage is updating");
            if (_storage + adjustedSpeed >= _levels[currentLevel - 1].capacity)
            {
                _storage = _levels[currentLevel - 1].capacity;
                _maxReached = true;
            }
            else
            {
                _storage += adjustedSpeed;
            }

            // Ensure storage does not exceed capacity
            Debug.Log("Storage is updating" + _storage);
        }
    }

    public void PlaceOnGrid(int x, int y)
    {
        _currentX = x;
        _currentY = y;
        _X = x;
        _Y = y;
        _originalX = x;
        _originalY = y;
        Vector3 position = UI_Main.instance._grid.GetCenterPosition(x, y, _rows, _columns);
        transform.position = position;
        SetBaseColor();
        isPlaced = true;
    }

    public void StartMovingOnGrid()
    {
        _X = _currentX;
        _Y = _currentY;
    }
    public void RemoveFromGrid()
    {
        _buildInstance = null;
        // Disable buttons confirm and cancel
        UI_Build.instance.SetStatus(false);
        CameraController.instance.isPlacingBuilding = false;
        Destroy(gameObject);
    }
    public void UpdateGridPosition(Vector3 basePosition, Vector3 currentPosition)
    {
        Vector3 dir = UI_Main.instance._grid.transform.TransformPoint(currentPosition) - UI_Main.instance._grid.transform.TransformPoint(basePosition);

        int xDis = Mathf.RoundToInt(dir.z / UI_Main.instance._grid.cellSize);
        int yDis = Mathf.RoundToInt(-dir.x / UI_Main.instance._grid.cellSize);

        _currentX = _X + xDis;
        _currentY = _Y + yDis;

        Vector3 position = UI_Main.instance._grid.GetCenterPosition(_currentX, _currentY, _rows, _columns);
        transform.position = position;

        SetBaseColor();
    }

    // To hide the base of the building
    public void HideBase()
    {
        if (_baseArea != null)
        {
            _baseArea.enabled = false;
        }
    }

    public void ShowBase()
    {
        if (_baseArea != null)
        {
            _baseArea.enabled = true;
        }
    }

    private void SetBaseColor()
    {
        if(UI_Main.instance._grid.CanPlaceBuilding(this, currentX, currentY))
        {
            // Make the placing confirm button interactable is building can place there
            UI_Build.instance.clickConfirmButton.interactable = true;
            _baseArea.sharedMaterial.color = Color.green;
        }
        else
        {
            // Make the placing confirm button uninteractable is building cannot place there
            UI_Build.instance.clickConfirmButton.interactable = false;
            _baseArea.sharedMaterial.color = Color.red;
        }
    }

    public void Selected()
    {
        
        if(selectedInstance != null)
        {
            if (selectedInstance == this)
            {
                return;
            }
            else
            {
                selectedInstance.Deselected();
            }
        }

        _baseArea.gameObject.SetActive(true);

        _originalX = _currentX;
        _originalY = _currentY;
        selectedInstance = this;

        if(id.ToLower() != "park")
        {
            UI_BuildingOptions.instance.SetStatus(true);
        }
        
    }

    public void Deselected()
    {
        Debug.Log("Building deselect function called");
        UI_BuildingOptions.instance.SetStatus(false);

        selectedInstance = null;
        CameraController.instance.isReplacingBuilding = false;
        _baseArea.gameObject.SetActive(false);

        if (_originalX != _currentX || _originalY != _currentY)
        {
            if(UI_Main.instance._grid.CanPlaceBuilding(this, _currentX, _currentY))
            {

                List<int> newPlotNumbers = new List<int> { _currentX, _currentY };
                PlaceOnGrid(_currentX, _currentY);

                UpdatePlotNumbers(GameDataManager.instance.gameData, uniqueId, newPlotNumbers);
            }
            else
            {
                PlaceOnGrid(_originalX, _originalY);
            }
        }

    }

    public void UpdatePlotNumbers(GameData gameData, int uniqueId, List<int> newPlotNumbers)
    {
        var energySource = gameData.energySources.FirstOrDefault(es => es.uniqueId == uniqueId);
        if (energySource != null)
        {
            Debug.Log("updated successfully");
            energySource.plotNumber = newPlotNumbers;
            string jsonData = JsonUtility.ToJson(gameData);

            StartCoroutine(UpdatingGameData(jsonData));
            return;
        }

        var investmentBuilding = gameData.investmentBuildings.FirstOrDefault(ib => ib.uniqueId == uniqueId);
        if (investmentBuilding != null)
        {
            Debug.Log("updated successfully");
            investmentBuilding.plotNumber = newPlotNumbers;
            string jsonData = JsonUtility.ToJson(gameData);

            StartCoroutine(UpdatingGameData(jsonData));
            return;
        }

        

        Console.WriteLine("Building with uniqueId not found.");
    }


    private void UpdateMesh()
    {
        // Ensure levels are defined
        if (_levels == null || _levels.Length == 0)
        {
            Debug.LogError("No levels defined.");
            return;
        }

        // Disable all meshes
        for (int i = 0; i < _levels.Length; i++)
        {
            if (_levels[i].mesh != null)
            {
                _levels[i].mesh.SetActive(false);
            }
        }

        // Enable the mesh of the current level
        Level currentLevel = System.Array.Find(_levels, level => level.level == _currentLevel);
        if (currentLevel != null && currentLevel.mesh != null)
        {
            
            currentLevel.mesh.SetActive(true);
        }
        else
        {
            Debug.LogError($"Mesh for level {_currentLevel} not found.");
        }
    }

    // Change the level and update the mesh
    public void SetCurrentLevel(int newLevel, int costWatt, int costStorage)
    {
        if (newLevel < 1 || newLevel > _levels.Length)
        {
            Debug.LogError("Invalid level.");
            return;
        }

        UI_Main.instance._wattCoin.text = (int.Parse(UI_Main.instance._wattCoin.text) - costWatt).ToString();
        UI_Main.instance._storageCoins.text = (int.Parse(UI_Main.instance._storageCoins.text) - costStorage).ToString();

        GameDataManager.instance.gameData.totalCoins = int.Parse(UI_Main.instance._wattCoin.text);
        GameDataManager.instance.gameData.storageCoins = int.Parse(UI_Main.instance._storageCoins.text);
        GameDataManager.instance.gameData.noOfPeople = int.Parse(UI_Main.instance._population.text);
        GameDataManager.instance.gameData.carbonPercentage = int.Parse(UI_Main.instance._carbonEmission.text);

        _currentLevel = newLevel;
        UpdateMesh();
        UpdateLevelInServer(GameDataManager.instance.gameData, uniqueId, newLevel);
    }

    public void UpdateLevelInServer(GameData gameData, int uniqueId, int newLevel)
    {
        var energySource = gameData.energySources.FirstOrDefault(es => es.uniqueId == uniqueId);
        if (energySource != null)
        {
            Debug.Log("updated successfully");
            energySource.level = newLevel;
            string jsonData = JsonUtility.ToJson(gameData);

            StartCoroutine(UpdatingGameData(jsonData));
            return;
        }

        var investmentBuilding = gameData.investmentBuildings.FirstOrDefault(ib => ib.uniqueId == uniqueId);
        if (investmentBuilding != null)
        {
            Debug.Log("updated successfully");
            investmentBuilding.level = newLevel;
            string jsonData = JsonUtility.ToJson(gameData);

            StartCoroutine(UpdatingGameData(jsonData));
            return;
        }



        Console.WriteLine("Building with uniqueId not found.");
    }

    [Serializable]
    public class InvestmentBuildingListWrapper
    {
        public List<InvestmentBuilding> investmentBuildings;

        public InvestmentBuildingListWrapper(List<InvestmentBuilding> investmentBuildings)
        {
            this.investmentBuildings = investmentBuildings;
        }
    }



    public static class BuildingCatalog
    {
        public static List<string> InvestmentBuildings = new List<string>()
    {
        "hotel",
        "restaurant",
        "supermarket",
        "apartment",
        "stadium"
    };

        public static List<string> EnergySources = new List<string>()
    {
        "windmill",
        "solarplant",
        "geothermalplant",
        "biogasplant",
        "wastetoenergy",
        "park"
    };
    }


}
