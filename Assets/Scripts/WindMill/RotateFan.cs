using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotateFan : MonoBehaviour
{
    [SerializeField] private float _rotationAngle,_speed;
    [SerializeField] GameObject _fan = null;

    void Update()
    {
        float rotationIncrement = _speed * Time.deltaTime;
        _fan.transform.Rotate(0, 0, rotationIncrement);
    }
}
