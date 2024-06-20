using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Shop : MonoBehaviour
{

    [SerializeField] public GameObject _elements = null;
    [SerializeField] private Button _closeButton = null;
    [SerializeField] private GameObject _popUpMsg = null;
    [SerializeField] private Button _okButton = null;
    [SerializeField] private TextMeshProUGUI _popUpText = null;

    [SerializeField] private Image panelImage = null; // Reference to the panel's Image component
    private static UI_Shop _instance = null; public static UI_Shop instance { get { return _instance; } }

    private void Awake()
    {
        _instance = this;
        _elements.SetActive(false);
        _popUpMsg.SetActive(false);
    }

    private void Start()
    {
        _closeButton.onClick.AddListener(CloseShop);
        _okButton.onClick.AddListener(ok);
    }

    public void SetStatus(bool status)
    {
        _elements.SetActive(status);
    }

    private void CloseShop()
    {
        SetStatus(false);
        UI_Main.instance.SetStatus(true);
    }

    public void NotEnoughResourcePopUP(string popUpText,bool isCarbonSaturated)
    {
        if (isCarbonSaturated)
        {
                    // Change the panel color to reddish and reduce transparency
            panelImage.color = new Color(1f, 0f, 0f, 0.5f); // Red color with 50% transparency
        }
        else
        {
                        // Reset the panel color if needed
            panelImage.color = new Color(1f, 1f, 1f, 0.9f); // Default color with 90% transparency
        }

        _popUpText.text = popUpText;
        _popUpMsg.SetActive(true);

    }

    private void ok()
    {
        _popUpMsg.SetActive(false);
    }
}
