using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UI_BuildingUpgrade : MonoBehaviour
{
    [SerializeField] public GameObject _elements = null;
    private static UI_BuildingUpgrade _instance = null; public static UI_BuildingUpgrade instance { get { return _instance; } }
    
    [SerializeField] private Button _closeButton = null;
    [SerializeField] private Button _upgradeButton = null;
    [SerializeField] private TextMeshProUGUI nextLevel = null;
    [SerializeField] private TextMeshProUGUI reqStorageCoins = null;
    [SerializeField] private TextMeshProUGUI reqWattCoins = null;
    [SerializeField] private TextMeshProUGUI updateChanges = null;

    [SerializeField] private GameObject Costs = null;
    [SerializeField] private GameObject Updates = null;

    private building Building;

    private int hello;
    private void Awake()
    {
        _instance = this;
        _elements.SetActive(false);
    }

    private void Start()
    {
        _closeButton.onClick.AddListener(Close);
        _upgradeButton.onClick.AddListener(Upgrade);
    }

    public void Open()
    {
        //Todo
        Building = building.selectedInstance;
        if (string.IsNullOrEmpty(Building.id))
        {

        }
        else
        {
            if ((Building.currentLevel + 1) > Building._levels.Length)
            {
                nextLevel.text = "Max Level Reached";
                Costs.SetActive(false);
                Updates.SetActive(false);
                _upgradeButton.interactable = false;

            }
            else
            {
                building.Level nextLevelData = Array.Find(Building._levels, level => level.level == Building.currentLevel + 1);

                Costs.SetActive(true);
                Updates.SetActive(true);

                nextLevel.text = "Next Level: " + nextLevelData.level.ToString();
                reqStorageCoins.text = nextLevelData.costStorageCoins.ToString();
                reqWattCoins.text = nextLevelData.costWattCoins.ToString();

                updateChanges.text = string.Empty; // Clear the current text

                if (nextLevelData.capacity != 0)
                {
                    updateChanges.text += $"<color=#FF0000>Capacity:</color> <color=#00FF00> {nextLevelData.capacity}</color>\n";
                }

                if (nextLevelData.capacity != 0)
                {
                    updateChanges.text += $"<color=#FF0000>Speed:</color> <color=#00FF00> {nextLevelData.speed}</color>\n";
                }

                if (nextLevelData.expectedEarning != 0)
                {
                    updateChanges.text += $"<color=#FF0000>Expected Earning:</color> <color=#00FF00> {nextLevelData.expectedEarning}</color>\n";
                }

                if (nextLevelData.population != 0)
                {
                    updateChanges.text += $"<color=#FF0000>Population:</color> <color=#00FF00>{nextLevelData.population}</color>\n";
                }

                _upgradeButton.interactable = true;
            }
            
        }
            // should get building data to check the next level
        _elements.SetActive(true);
    }

    private void Close()
    {
        _elements.SetActive(false);
    }

    private void Upgrade()
    {
        // building Building = building.selectedInstance;
        if (Building == null)
        {
            Debug.LogError("Selected instance is null.");
            return;
        }

        int costWattCoins = Building._levels[Building.currentLevel].costStorageCoins;
        int costStorageCoins = Building._levels[Building.currentLevel].costStorageCoins;

        // send the level in the backend
        if (costWattCoins <= int.Parse(UI_Main.instance._wattCoin.text) && costStorageCoins <= int.Parse(UI_Main.instance._storageCoins.text))
        {
            Building.currentLevel += 1;
            Debug.Log("Building upgraded to level: " + Building.currentLevel);

            UI_Main.instance._population.text = (int.Parse(UI_Main.instance._population.text) +Building._levels[Building.currentLevel-1].population).ToString();

            AudioManager.instance.PlaySFX("Upgrade");

            Building.SetCurrentLevel(Building.currentLevel, costWattCoins, costStorageCoins);
            Debug.Log(Building.id);
            // Todo
            Debug.Log("Building successfully upgraded.");
            _elements.SetActive(false);
        }
        else
        {
            Debug.Log("You dont have enough coins to upgrade");
        }
        
    }
}
