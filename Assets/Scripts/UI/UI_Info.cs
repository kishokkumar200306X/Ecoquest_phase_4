using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UI_Info : MonoBehaviour
{
    [SerializeField] public GameObject _elements = null;
    private static UI_Info _instance = null; public static UI_Info instance { get { return _instance; } }

    [SerializeField] private Button _closeButton = null;
    [SerializeField] private TextMeshProUGUI infoText = null;
    [SerializeField] private Image icon = null;
    private building Building;

    private int hello;
    private void Awake()
    {
        _instance = this;
        _elements.SetActive(false);
    }

    private void Start()
    {
        _closeButton.onClick.AddListener(CloseInfo);
    }

    public void OpenInfo()
    {
        //Todo
        Building = building.selectedInstance;
        if (string.IsNullOrEmpty(Building.id))
        {

        }
        else
        {
            icon.sprite = Building._buildingImage;
            infoText.text = Building._buildingInfo;

        }
        // should get building data to check the next level
        _elements.SetActive(true);
    }

    private void CloseInfo()
    {
        _elements.SetActive(false);
    }

    
}
