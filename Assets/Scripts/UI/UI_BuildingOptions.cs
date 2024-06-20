using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BuildingOptions : MonoBehaviour
{

    [SerializeField] public GameObject _elements = null;
    private static UI_BuildingOptions _instance = null; public static UI_BuildingOptions instance { get { return _instance; } }

    public RectTransform infoPanel = null;
    public RectTransform upgradePanel = null;

    public Button infoButton = null;
    public Button upgradeButton = null;
    private void Awake()
    {
        _instance = this;
        _elements.SetActive(false);
    }

    public void SetStatus(bool status)
    {
        if(status && building.selectedInstance != null)
        {
            infoPanel.gameObject.SetActive(true);  // should change this to not show if building is being constructing (isbuildingconstructing == false)
            upgradePanel.gameObject.SetActive(true);
        }
        _elements.SetActive(status);
    }

}
