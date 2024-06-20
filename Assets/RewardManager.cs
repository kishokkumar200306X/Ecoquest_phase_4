using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RewardManager : MonoBehaviour
{
    [SerializeField] private GameObject PileOfCoinParent;
    //[SerializeField] private TextMeshProUGUI Counter;
    [SerializeField] private Vector3[] InitialPos;
    [SerializeField] private Quaternion[] InitialRotation;
    [SerializeField] private int CoinCount;

    // public RectTransform imageRectTransform;

    public static RewardManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        InitialPos = new Vector3[CoinCount];
        InitialRotation = new Quaternion[CoinCount];

        for (int i = 0; i < PileOfCoinParent.transform.childCount; i++)
        {
            InitialPos[i] = PileOfCoinParent.transform.GetChild(i).position;
            InitialRotation[i] = PileOfCoinParent.transform.GetChild(i).rotation;
        }
        
    }


    private void Reset()
    {
        for (int i = 0; i < PileOfCoinParent.transform.childCount; i++)
        {
            PileOfCoinParent.transform.GetChild(i).position = InitialPos[i];
            PileOfCoinParent.transform.GetChild(i).rotation = InitialRotation[i];
        }
    }

    public void RewardPileOfCoin(int noCoin)
    {
        Reset();

        var delay = 0f;

        PileOfCoinParent.SetActive(true);

        Vector2 imagePosition = new Vector2(100f, 460f);

        for (int i = 0; i < PileOfCoinParent.transform.childCount; i++)
        {
            Transform coin = PileOfCoinParent.transform.GetChild(i);
            RectTransform coinRect = coin.GetComponent<RectTransform>();

            coinRect.DOScale(1f, 0.3f).SetDelay(delay).SetEase(Ease.OutBack);
            coinRect.DOAnchorPos(imagePosition, 1f).SetDelay(delay).SetEase(Ease.OutBack);

            coinRect.DOScale(0f, 0.3f).SetDelay(delay + 1f).SetEase(Ease.OutBack);

            delay += 0.2f;
        }
    }
}
