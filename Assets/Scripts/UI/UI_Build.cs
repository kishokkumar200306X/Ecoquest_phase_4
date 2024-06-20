using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Linq;
using System;
using JetBrains.Annotations;
using static building;

public class UI_Build : MonoBehaviour
{

    [SerializeField] public GameObject _elements = null;    // Elements belong to UI Build gameobject
    public RectTransform buttonConfirm = null;
    public RectTransform buttonCancel = null;
    [HideInInspector] public Button clickConfirmButton = null;

    // State variables
    public bool _buildingPlaced = false;
    public bool _isContructing = false;

    // Private variables for population and carbon emission
    private int population;
    private int carbonEmission;

    private static UI_Build _instance = null; public static UI_Build instance { get { return _instance; } }

    private void Awake()
    {
        _instance = this;
        _elements.SetActive(false);
        clickConfirmButton = buttonConfirm.gameObject.GetComponent<Button>();
    }

    private void Start()
    {
        // Add listeners to buttons
        buttonConfirm.gameObject.GetComponent<Button>().onClick.AddListener(Confirm);
        buttonCancel.gameObject.GetComponent<Button>().onClick.AddListener(Cancel);

        // Set anchors to bottom-left corner
        buttonConfirm.anchorMin = Vector3.zero;
        buttonConfirm.anchorMax = Vector3.zero;

        buttonCancel.anchorMin = Vector3.zero;
        buttonCancel.anchorMax = Vector3.zero;
    }

    private void Update()
    {
        // Check if essential instances are assigned
        if (building.buildInstance == null || UI_Main.instance == null || UI_Main.instance._grid == null || CameraController.instance == null)
        {
          //  Debug.LogWarning("UI_Build: Essential components are not assigned.");
            Debug.LogWarning("Building: " + building.buildInstance+"UI_Main: "+UI_Main.instance+"UI_Main_Grid: "+UI_Main.instance._grid+"CameraController: "+CameraController.instance);
            return;
        }

        // Check if a building is being placed
        if (building.buildInstance != null && CameraController.instance.isPlacingBuilding)
        {
            // Get end position of the building in the grid
            Vector3 end = UI_Main.instance._grid.GetEndPosition(building.buildInstance);

            // Get boundaries of the camera's plan
            Vector3 planDownLeft = CameraController.instance.planDownLeft;
            Vector3 planTopRight = CameraController.instance.planTopRight;

            // Calculate width and height of the plan
            float w = planTopRight.x - planDownLeft.x;
            float h = planTopRight.z - planDownLeft.z;

            // Calculate end position within the plan
            float endw = end.x - planDownLeft.x;
            float endh = end.z - planDownLeft.z;

            // Convert to screen coordinates
            Vector2 screenPoint = new Vector2(endw / w * Screen.width, endh / h * Screen.height);

            // Set confirm button position
            Vector2 confirmPosition = screenPoint;
            confirmPosition.x += (buttonConfirm.rect.width + 10f);
            buttonConfirm.anchoredPosition = confirmPosition;

            // Set cancel button position
            Vector2 cancelPosition = screenPoint;
            cancelPosition.x -= (buttonCancel.rect.width + 10f);
            buttonCancel.anchoredPosition = cancelPosition;

            // Debug.Log("isNotinRoad: " + CollisionLogger.instance.StatusOfRoadCollision());
        }
    }

    // Sets the active status of the _elements GameObject
    public void SetStatus(bool status)
    {
        _elements.SetActive(status);
    }


    // Function to confirm building placement
    public void Confirm()
    {
        // Check if a building instance exists and can be placed on the grid
        if (building.buildInstance != null && UI_Main.instance._grid.CanPlaceBuilding(building.buildInstance, building.buildInstance.currentX, building.buildInstance.currentY))
        {
            // Update population and carbon emission based on building type
            population = building.buildInstance._levels[buildInstance.currentLevel - 1].population;
            carbonEmission = 5;

            // Update population for apartments
            if (building.buildInstance.id == "apartment")
            {
                UI_Main.instance._population.text = (int.Parse(UI_Main.instance._population.text) + population).ToString();
            }
            else
            {
                building.buildInstance.populationCapacity = 0;
            }

            // Update carbon emission for investment buildings
            if (BuildingCatalog.InvestmentBuildings.Contains(building.buildInstance.id.ToLower()))
            {
                if ((int.Parse(UI_Main.instance._carbonEmission.text) + carbonEmission) <= 100)
                {
                    UI_Main.instance._carbonEmission.text = (int.Parse(UI_Main.instance._carbonEmission.text) + carbonEmission).ToString();
                    UI_Main.instance._carbonEmissionSlider.value = int.Parse(UI_Main.instance._carbonEmission.text);
                }
                else
                {
                    UI_Main.instance._carbonEmission.text = "100";
                    UI_Main.instance._carbonEmissionSlider.value = 100;
                }   
            }

            // Special case for parks to reduce carbon emission
            if (building.buildInstance.id.ToLower() == "park")
            {
                int newEmission = int.Parse(UI_Main.instance._carbonEmission.text) - 20;
                UI_Main.instance._carbonEmission.text = Mathf.Max(0, newEmission).ToString();
                UI_Main.instance._carbonEmissionSlider.value = int.Parse(UI_Main.instance._carbonEmission.text);
            }

            // Get the current grid position of the building
            int x = building.buildInstance.currentX;
            int y = building.buildInstance.currentY;

            // Set to level 1 at the beginning
            building.buildInstance.SetCurrentLevel(1, building.buildInstance._levels[0].costWattCoins, building.buildInstance._levels[0].costStorageCoins);

            // Finalize the building placement at the current position
            building.buildInstance.PlaceOnGrid(x, y);

            UI_Main.instance._grid.buildings.Add(building.buildInstance);

            // Setting building palced time as the lastcollected time when building initializing
            building.buildInstance._lastCollectedTime = DateTime.Now;

            // Send building data to the server
            AddingBuildingToServer(building.buildInstance, population, carbonEmission);

            // Play placement sound effect
            AudioManager.instance.PlaySFX(building.buildInstance.id);

            // Hide the building's base after placement
            building.buildInstance.HideBase();

            // Disable placing mode
            CameraController.instance.isPlacingBuilding = false;

            // Deactivate the build UI
            SetStatus(false);
            _buildingPlaced = true;
        }
        else if (building.buildInstance == null)
        {
            Debug.LogWarning("build instance is null");
        }
        else
        {
            Debug.Log("Not confirmed");
        }

    }


    // Need to move to the UI_Building script
    public void LoadingBuildings()
    {
        if (building.buildInstance != null)
        {
            //_isContructing = true;

            // Get the current grid position of the building
            int x = building.buildInstance.currentX;
            int y = building.buildInstance.currentY;

            // Finalize the building placement at the current position
            building.buildInstance.PlaceOnGrid(x, y);

            UI_Main.instance._grid.buildings.Add(building.buildInstance);

            // Hide the base after placing the building
            building.buildInstance.HideBase();

            // Disable placing mode
            CameraController.instance.isPlacingBuilding = false;


            // Deactivate the build UI
            SetStatus(false);
            _buildingPlaced = true;
        }
        else if (building.buildInstance == null)
        {
            Debug.Log("build instance is null");
        }
        else
        {
            Debug.Log("Not confirmed");
        }

    }

    // Need to move to the UI_Building script
    public void AddingBuildingToServer(building Building, int population, int carbonEmission)
    {

        GameData gameData = GameDataManager.instance.gameData;

        int MaxUniqueId = 1;

        List<int> plotNumbers = new List<int>() {Building.currentX, Building.currentY };

        string collectedTime = Building._lastCollectedTime.ToString("o");

        gameData.noOfPeople += population;
        gameData.carbonPercentage += carbonEmission;

        if(gameData.investmentBuildings.Count + gameData.energySources.Count == 0)
        {
            MaxUniqueId = 1;
        }
        else if (gameData.investmentBuildings.Count == 0)
        {
            // int MaxUniqueIdInvestmentBuilding = gameData.investmentBuildings.Max(building => building.uniqueId);
            int MaxUniqueIdEnergySource = gameData.energySources.Max(building => building.uniqueId);

            // MaxUniqueId = Math.Max(MaxUniqueId, MaxUniqueIdInvestmentBuilding);
            MaxUniqueId = Math.Max(MaxUniqueId, MaxUniqueIdEnergySource);
        }
        else if (gameData.energySources.Count == 0)
        {
            int MaxUniqueIdInvestmentBuilding = gameData.investmentBuildings.Max(building => building.uniqueId);
            // int MaxUniqueIdEnergySource = gameData.energySources.Max(building => building.uniqueId);

            MaxUniqueId = Math.Max(MaxUniqueId, MaxUniqueIdInvestmentBuilding);
            // MaxUniqueId = Math.Max(MaxUniqueId, MaxUniqueIdEnergySource);
        }
        else
        {
            int MaxUniqueIdInvestmentBuilding = gameData.investmentBuildings.Max(building => building.uniqueId);
            int MaxUniqueIdEnergySource = gameData.energySources.Max(building => building.uniqueId);

            MaxUniqueId = Math.Max(MaxUniqueId, MaxUniqueIdInvestmentBuilding);
            MaxUniqueId = Math.Max(MaxUniqueId, MaxUniqueIdEnergySource);
        }


        string jsonData = "";

        if (BuildingCatalog.InvestmentBuildings.Contains(Building.id))
        {
            Debug.Log(Building.id + " is an investment building.");

            InvestmentBuilding newBuilding = new InvestmentBuilding
            {
                uniqueId = MaxUniqueId + 1,
                buildingName = Building.id,
                expectedEarnings = 0,
                level = Building.currentLevel,
                plotNumber = plotNumbers,
                storageCoinCollectedTimeString = collectedTime
            };

            gameData.investmentBuildings.Add(newBuilding);

            Debug.Log("Maxid: " + MaxUniqueId);

            jsonData = JsonUtility.ToJson(gameData);

            Debug.Log($"JSON Data to be sent for gamedata: {jsonData}");
            
        }
        else if (BuildingCatalog.EnergySources.Contains(Building.id))
        {
            Debug.Log(Building.id + " is an energy source building.");

            EnergySource newBuilding = new EnergySource
            {
                uniqueId = MaxUniqueId + 1,
                buildingName = Building.id,
                expectedEarnings = Building._levels[Building.currentLevel - 1].expectedEarning,
                level = 1,
                plotNumber = plotNumbers
            };

            gameData.energySources.Add(newBuilding);

            jsonData = JsonUtility.ToJson(gameData);

            Debug.Log($"JSON Data to be sent for gamedata: {jsonData}");
        }
        else
        {
            Debug.Log("Not recognized: " + Building.id);
            Debug.Log("Building type not recognized.");
        }
               
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

    public void Cancel()
    {
        if (building.buildInstance != null)
        {
            CameraController.instance.isPlacingBuilding = false;
            building.buildInstance.RemoveFromGrid();
        }
    }

}
