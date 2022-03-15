using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    [Header("Igonre Axis")]
    [SerializeField] bool _isIgnoreY = true;

    [Header("Execution")]
    [SerializeField] bool _onStart = false;
    [SerializeField] bool _onUpdate = false;

    Vector3 _lookAtPosition;

    void Start()
    {
        _lookAtPosition = Unit_Manager.Instance._player.transform.position;

        if (_isIgnoreY)
        {
            _lookAtPosition.y = transform.position.y;
        }

        if(!_onStart) { return; }
        transform.LookAt(_lookAtPosition);
    }

    void Update()
    {
        if (!_onUpdate) { return; }
        transform.LookAt(_lookAtPosition);
    }

    public void _LookAtPlayer()
    {
        transform.LookAt(_lookAtPosition);
    }
}
