using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;

public class UI_Building : MonoBehaviour
{
    [SerializeField] private string _prefabId = "";
    //[SerializeField] private int _prefabIndex = 0;
    [SerializeField] private Button _button = null;
    [SerializeField] private int _cost = 0;

    private bool isCarbonSaturated = false;

    private string showText = "";

    private void Start()
    {
        _button.onClick.AddListener(Clicked);
    }

    private void Clicked()
    {
        tooltipSystem.Hide();

        building build = UI_Main.instance.GetBuildingPrefabById(_prefabId);

        //Check if the slider.text is greater than 100
        isCarbonSaturated = int.Parse(UI_Main.instance._carbonEmission.text) >= 100;

        if ((build._levels[build.currentLevel - 1].costWattCoins <= int.Parse(UI_Main.instance._wattCoin.text)) && (build._levels[build.currentLevel - 1].costStorageCoins <= int.Parse(UI_Main.instance._storageCoins.text))&& !(isCarbonSaturated) )
        {
            UI_Shop.instance.SetStatus(false);
            UI_Main.instance.SetStatus(true);

            Vector3 position = Vector3.zero;

            //building Building = Instantiate(UI_Main.instance._buildingPrefabs[_prefabIndex], position, Quaternion.identity);
            building Building = Instantiate(UI_Main.instance.GetBuildingPrefabById(_prefabId), position, Quaternion.identity);

            
            // Building.PlaceOnGrid(20, 20);
            building.buildInstance = Building;
            CameraController.instance.isPlacingBuilding = true;

            // To show the build confirm and cancel buttons
            UI_Build.instance.SetStatus(true);
        }
        else
        {
            if (isCarbonSaturated)
            {
                showText = "You have reached the maximum carbon emission limit";
            }
            else
            {
                showText = "You dont have enough resources to buy the building";
            }

            UI_Shop.instance.NotEnoughResourcePopUP(showText,isCarbonSaturated);
            Debug.Log("You dont have resources to buy the building");
        }
        
    }

}
