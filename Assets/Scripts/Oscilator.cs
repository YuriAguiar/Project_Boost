using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscilator : MonoBehaviour
{
    [SerializeField] private Vector3 _movementVector;
    [Range(0, 1)] [SerializeField] private float _movementFactor;
    private Vector3 _startingPos;

    // Start is called before the first frame update
    void Start()
    {
        _startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 offset = _movementVector * _movementFactor;
        transform.position = _startingPos + offset;
    }
}
