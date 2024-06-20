using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionLogger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "enemy")
        {
            Debug.Log("Enemy entered the trigger");

            if (RoadHoverDetector.instance != null)
            {
                RoadHoverDetector.instance.isnNotinRoad = false;
            }
            else
            {
                Debug.LogWarning("RoadHoverDetector.instance is null");
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "enemy")
        {
            // Debug.Log("Enemy is in the trigger");
            RoadHoverDetector.instance.isnNotinRoad = false;//isNotinRoad


        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "enemy")
        {
            Debug.Log("Enemy exited the trigger");

            if (RoadHoverDetector.instance != null)
            {
                RoadHoverDetector.instance.isnNotinRoad = true;
            }
            else
            {
                Debug.LogWarning("RoadHoverDetector.instance is null");
            }
        }
    }
}
