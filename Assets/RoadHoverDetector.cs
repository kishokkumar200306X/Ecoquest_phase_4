using UnityEngine;

public class RoadHoverDetector : MonoBehaviour
{
    private static RoadHoverDetector _instance = null;
    public static RoadHoverDetector instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("RoadHoverDetector instance is null. Make sure RoadHoverDetector script is attached to a GameObject in the scene.");
            }
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }

    // public bool isnNotinRoad = false;


    [SerializeField]
    private bool _isNotinRoad = false; // Private backing field

    public bool isnNotinRoad
    {
        get { return _isNotinRoad; }
        set { _isNotinRoad = value; }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        Debug.Log("RoadHoverDetector instance is set in Awake.");
    }
}
