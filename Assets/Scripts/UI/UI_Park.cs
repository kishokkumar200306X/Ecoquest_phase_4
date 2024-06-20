using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;

public class UI_Park : MonoBehaviour
{
    [SerializeField] private string _prefabId = "";
    //[SerializeField] private int _prefabIndex = 0;
    [SerializeField] private Button _button = null;
    [SerializeField] private int _cost = 0;

    private void Start()
    {
        _button.onClick.AddListener(Clicked);
    }

    private void Clicked()
    {
        building build = UI_Main.instance.GetBuildingPrefabById("park");

        if ((build._levels[0].costWattCoins <= int.Parse(UI_Main.instance._wattCoin.text)) && (build._levels[0].costStorageCoins <= int.Parse(UI_Main.instance._storageCoins.text)))
        {
            Vector3 position = Vector3.zero;


            building Building = Instantiate(UI_Main.instance.GetBuildingPrefabById(_prefabId), position, Quaternion.identity);

            building.buildInstance = Building;
            CameraController.instance.isPlacingBuilding = true;

            // To show the build confirm and cancel buttons
            UI_Build.instance.SetStatus(true);

            // Check if the build is not placed and set buildInstance to null
            if (!UI_Build.instance._buildingPlaced)
            {
                UI_Build.instance.Cancel();
            }
        }
        else
        {
            Debug.Log("You don't have resources to buy this building");
        }
    }
}
