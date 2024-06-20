using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Bar : MonoBehaviour
{
    public Image bar = null;
    public RectTransform rect = null;

    private float max = 1;
    private float min = 0;
    private float value = 0.5f;

    public void SetMin(float input)
    {
        min = input;
        AdjustUI();
    }

    public void SetMax(float input)
    {
        max = input;
        AdjustUI();
    }

    public void SetValue(float input)
    {
        value = input;
        AdjustUI();
    }

    private void AdjustUI()
    {
        bar.fillAmount = value / Mathf.Abs(max - min);
    }
}
