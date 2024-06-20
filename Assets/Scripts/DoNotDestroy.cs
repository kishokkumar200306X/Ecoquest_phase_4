using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoNotDestroy : MonoBehaviour
{

    private void Awake()
    {
        GameObject[] soundObj = GameObject.FindGameObjectsWithTag("ThemeMusic");
        if (soundObj.Length > 1)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }
}
