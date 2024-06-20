using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lea_Brd_Main : MonoBehaviour
{
    private Transform entryContainer;
    private Transform entryTemplate;

    private void Awake()
    {
        entryContainer = transform.Find("LeabrdEntryContainer");
        if (entryContainer == null)
        {
            Debug.LogError("Leaderboard_container not found!");
            return;
        }

        entryTemplate = entryContainer.Find("LeabrdEntryTemplate");
        if (entryTemplate == null)
        {
            Debug.LogError("Leaderboard_temp not found!");
            return;
        }

        entryTemplate.gameObject.SetActive(false);

        float templateheight = 20f;
        for (int i = 0; i < 5; i++)
        {
            Transform entryTransform = Instantiate(entryTemplate, entryContainer);
            RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
            entryRectTransform.anchoredPosition = new Vector2(0, -templateheight * 4 * i);
            entryTransform.gameObject.SetActive(true);
        }
    }




}
