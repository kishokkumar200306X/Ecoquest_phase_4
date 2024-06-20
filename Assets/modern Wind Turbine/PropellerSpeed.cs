using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropellerSpeed : MonoBehaviour
{
    [SerializeField] private float _rotationAngle, _speed;
    [SerializeField] GameObject _propeller = null;

    void Update()
    {
        float rotationIncrement = _speed * Time.deltaTime;
       //fan.transform.Rotate(0, 0, rotationIncrement);
        _propeller.transform.Rotate(rotationIncrement, 0,0);
    }
}
